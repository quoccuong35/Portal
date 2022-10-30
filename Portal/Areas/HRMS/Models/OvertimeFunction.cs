using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Portal.EntityModels;
using Portal.ViewModels;

namespace HRMS.Models
{
    public static class OvertimeFunction
    {
        public static string CheckExistOT(DateTime date, TimeSpan startTime, TimeSpan endTime, Guid id, Guid employeeId,string employeeCode,PortalEntities _context)
        {
            string result = "";
            //var oT = db.OvertimeModels.Where(it => it.OvertimeDay == date && it.Del != true && it.BrowseStatusID != "4" && it.OvertimeId != id 
            //        && it.OvertimeDetailModels.Any(c=>c.EmployeeId == employeeId && c.Del != true)).ToList();

            var oT = (from it in _context.OvertimeModels
                      where it.OvertimeDay == date && it.BrowseStatusID != "4" && it.OvertimeId != id
                         && it.OvertimeDetailModels.Any(c => c.EmployeeId == employeeId && c.Del != true)
                      select it).ToList();
            if (oT.Count > 0)
            {
                foreach (var item in oT)
                {
                    if (startTime > item.OvertimeStartTime && startTime < item.OvertimeEndTime
                    || endTime > item.OvertimeStartTime && endTime < item.OvertimeEndTime
                    || startTime <= item.OvertimeStartTime && endTime >= item.OvertimeEndTime
                    || startTime >= item.OvertimeStartTime && endTime <= item.OvertimeEndTime)
                    {
                        result = "Đã tồn tại tăng ca từ giờ " + item.OvertimeStartTime.ToString() + " đến giờ " + item.OvertimeEndTime.ToString() + " của mã " + employeeCode;
                        break;
                    }
                }
            }
            return result;
        }

        public static OvertimeViewModel getDatTa(OvertimeModel edit, string accID = null,bool bnhanSu = false)
        {
            OvertimeViewModel model = new OvertimeViewModel();
            if (edit != null)
            {
                model.OvertimeId = edit.OvertimeId;
                model.OvertimeDay = edit.OvertimeDay;
                model.OvertimeStartTime = edit.OvertimeStartTime.ToString(@"hh\:mm");
                model.OvertimeEndTime = edit.OvertimeEndTime.ToString(@"hh\:mm");
                model.DepartmentID = edit.DepartmentID;
                model.Disable1 = edit.Disable1 == null?false:edit.Disable1.Value;
                model.ProjectID = edit.ProjectID;
                model.BrowseStatusID = edit.BrowseStatusID;
                model.Reason = edit.Reason;
                bool disabled = false;
                if (!bnhanSu)
                {
                    disabled = edit.Disable1 == null?false:edit.Disable1.Value;
                }
                disabled = edit.OvertimeDetailModels.Where(it => it.Del != true).Count() == 1 ? true : disabled;
                List<OvertimeDetailViewModel> detail = new List<OvertimeDetailViewModel>();
                detail = edit.OvertimeDetailModels.Where(it=>it.Del != true).
                    Select(it => new OvertimeDetailViewModel { OvertimeId = it.OvertimeId,EmployeeCode = it.EmployeeModel.EmployeeCode,
                    EmployeeId = it.EmployeeId,DepartmentName = it.EmployeeModel.Department.DepartmentName,FullName = it.EmployeeModel.FullName, Disable1 = disabled
                    }).ToList();
                model.OverDetail = new List<OvertimeDetailViewModel>();
                model.OverDetail.AddRange(detail);
                model.CreatedAccountId = edit.CreatedAccountId;
                model.CreatedTime = edit.CreatedTime;
                model.AppHistory = new List<ApprovalHistoryModel>();
                model.AppHistory.AddRange(edit.ApprovalHistoryModels.ToList());
                model.AccountID = new Guid(accID);
            }
            return model;
        }

        public static void Update(OvertimeViewModel model, OvertimeModel edit, List<string> errorList, bool isHasPermission,Guid currenID, string stype = null)
        {
            if (TimeSpan.Parse(model.OvertimeStartTime) > TimeSpan.Parse(model.OvertimeEndTime))
            {
                errorList.Add("Thời gian tăng ca từ giờ đến giờ không hợp lệ");
            }
            if (clsFunction.checkKyCongNguoiDung(model.OvertimeDay.Value) && edit.Disable1 != true  && !isHasPermission)
            {
                errorList.Add("Kỳ công đã khóa không thể sửa");
            }
            if (edit.Disable1 == true && !isHasPermission)
            {
                errorList.Add("Yêu cầu đã gửi mail thể sửa");
            }
            if (edit.BrowseStatusID == "4")
            {
                errorList.Add("Tăng ca đã hủy không thể sửa");
            }
            else
            {
                edit.Reason = model.Reason;
                edit.DepartmentID = model.DepartmentID;
                edit.OvertimeStartTime = TimeSpan.Parse(model.OvertimeStartTime);
                edit.OvertimeEndTime = TimeSpan.Parse(model.OvertimeEndTime);
                if (stype == "nhansu" && isHasPermission)
                {
                    if (model.BrowseStatusID == "4" && (model.ReasonStop == null || model.ReasonStop == ""))
                    {
                        errorList.Add("Bạn chưa nhập lý do hủy");
                    }
                    else if(model.BrowseStatusID == "4" && model.ReasonStop != "")
                    {
                        foreach (var item in edit.ApprovalHistoryModels)
                        {
                            if (item.BrowseStatusID != "3")
                            {
                                item.BrowseStatusID = "4";
                                item.LastModifiedAccountId = currenID;
                                item.LastModifiedTime = DateTime.Now;
                            }
                        }
                        edit.OvertimeDetailModels.Where(it => it.Del != true).ToList().ForEach(it => {
                            it.Del = true;
                            it.DelAccountId = currenID;
                            it.DelTime = DateTime.Now;
                        });
                    }
                    edit.BrowseStatusID = model.BrowseStatusID;
                    edit.ReasonStop = model.ReasonStop;
                }
            }
        }

        public static string EmailContent(string receiver, string soNguoiOT, OvertimeModel item, string contents, string reasonStop = null)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<table width='100%' border='0'>");
                sb.Append("<tr>");
                sb.Append("<td>Anh/Chị <b>" + receiver + "</b> thân mến</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td>" + contents + "</td>");
                sb.Append("</tr>");
                if (reasonStop != null && reasonStop != "")
                {
                    sb.Append("<tr>");
                    sb.Append("<td>Lý do từ chối: <b>" + reasonStop + "</b></td>");
                    sb.Append("</tr>");
                }
                sb.Append("<tr>");
                sb.Append("<td> Ngày: <b>" + Convert.ToDateTime(item.OvertimeDay).Date.ToString("dd/MM/yyyy") + "</b></td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td>Số người tăng ca: <b>" + soNguoiOT + "</b></td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td>Từ giờ: <b>" +item.OvertimeStartTime.ToString(@"hh\:mm")
                        + "</b> Đến giờ: <b>" + item.OvertimeEndTime.ToString(@"hh\:mm") + "</b></td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td>Lý do: <b>" + item.Reason + "<b></td>");
                sb.Append("</tr>");
                sb.Append("<tr><td>Đây là email tự động từ hệ thống - vui lòng không phản hồi.</td></tr>");
                sb.Append("<tr>");
                sb.Append("<td>Vui lòng truy cập vào hệ thống nghỉ phép để xem thông tin chi tiết hơn và xem xét duyệt yêu cầu này (Click vào link sau để vào hệ thống)</td>");
                sb.Append("</tr>");
                sb.Append("</table>");

                return sb.ToString();
            }
            catch (Exception)
            {

                return "";
            }
        }
    }
}