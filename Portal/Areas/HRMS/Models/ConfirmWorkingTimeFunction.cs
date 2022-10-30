using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.ViewModels;
using Portal.EntityModels;
using Portal.Resources;

namespace HRMS.Models
{
    public static class ConfirmWorkingTimeFunction
    {
        public static SelectList GetType(string Type1 = null)
        {
            List<TypeTime> list = new List<TypeTime>();
            list.Add(new TypeTime { Type1 = "1",Name = "Giờ vào" });
            list.Add(new TypeTime { Type1 = "2", Name = "Giờ ra" });
            return new SelectList(list, "Type1", "Name", Type1);
        }
        public static void Create(ConfirmWorkingTimeViewModel model, ConfirmWorkingTimeModel add, List<string> errorList, PortalEntities _context,Guid accID) {
            add.ConfirmWorkingTimeID = Guid.NewGuid();
            ApprovalHistoryModel item = new ApprovalHistoryModel();
            if (clsFunction.checkKyCongNguoiDung(model.Date1))
            {
                errorList.Add(string.Format(LanguageResource.CheckFeat, LanguageResource.ConfirmWorkingTime));
            }
            var acc = clsFunction.GetAccount(accID,_context);
            if (acc == null)
            {
                errorList.Add("Tài khoản đã bị khóa không thể thao tác");
            }
            else if (acc != null && acc.EmployeeModel == null)
            {
                errorList.Add("Người dùng chưa có liên kết nhân sự");
            }
            else if (acc != null && acc.EmployeeModel != null && acc.EmployeeModel.ParentId == null)
            {
                errorList.Add("Người dùng chưa có cấp quản lý trực tiếp");
            }
            else
            {
               
                item.EmployeeId = acc.EmployeeModel.ParentId.Value;
                item.ApprovalLevel = 10;
                item.Type1 = Portal.Constant.ConstFunction.XacNhanCong;
                item.BrowseStatusID = "2";
                item.ApprovalId = add.ConfirmWorkingTimeID;
                item.MaTrixId = add.ConfirmWorkingTimeID;
                item.Id = Guid.NewGuid();
                item.CreatedAccountId = accID;
                item.CreatedTime = DateTime.Now;
                if (add.ApprovalHistoryModels != null)
                    add.ApprovalHistoryModels.Clear();
               
            }
            var checkExist = _context.ConfirmWorkingTimeModels.Where(it => it.Type1 == model.Type1 && it.Date1 == model.Date1 && it.EmployeeId == model.Employee.EmployeeId && it.BrowseStatusID !="4").ToList();
            if (checkExist.Count > 0)
            {
                errorList.Add("Đã tồn tại xác nhận công ngày " + model.Date1.ToString("dd/MM/yyyy") );
            }
            add.ApprovalHistoryModels.Add(item);
            add.EmployeeId = model.Employee.EmployeeId;
            add.Time1 = TimeSpan.Parse(model.Time1);
            add.Date1 = model.Date1;
            add.Type1 = model.Type1;
            add.Reason = model.Reason;
            add.BrowseStatusID = "1";
            add.Disable1 = false;
            add.CreatedAccountId = accID;
            add.CreatedTime = DateTime.Now;
        }

        public static ConfirmWorkingTimeViewModel GetData(ConfirmWorkingTimeModel model, string accID = null, bool bnhanSu = false) {
            ConfirmWorkingTimeViewModel info = new ConfirmWorkingTimeViewModel();
            EmployeeInfoView employee = new EmployeeInfoView();
            employee.EmployeeId = model.EmployeeModel.EmployeeId;
            employee.EmployeeCode = model.EmployeeModel.EmployeeCode;
            employee.FullName = model.EmployeeModel.FullName;
            employee.DepartmentName = model.EmployeeModel.Department.DepartmentName;
            employee.DepartmentName = model.EmployeeModel.Department.DepartmentName;
            employee.StartTime = model.EmployeeModel.ShiftWork.StartTime.ToString(@"hh\:mm");
            employee.EndTime = model.EmployeeModel.ShiftWork.EndTime.ToString(@"hh\:mm");
            employee.RemainingLeavedays = model.EmployeeModel.RemainingLeavedays != null ? model.EmployeeModel.RemainingLeavedays.ToString() : String.Empty;
            employee.Readonly = true;
            info.Employee = employee;

            info.ConfirmWorkingTimeID = model.ConfirmWorkingTimeID;
            info.Date1 = model.Date1;
            info.Time1 = model.Time1.ToString(@"hh\:mm");
            info.Type1 = model.Type1;
            info.Disable1 = model.Disable1;
            info.Reason = model.Reason;
            info.BrowseStatusID = model.BrowseStatusID;
            info.CreatedAccountId = model.CreatedAccountId;
            info.CreatedTime = model.CreatedTime;
            info.AppHistory = new List<ApprovalHistoryModel>();
            info.AppHistory.AddRange(model.ApprovalHistoryModels.ToList());
            info.AccountID = new Guid(accID);
            return info;
        }

        public static void UpData(ConfirmWorkingTimeViewModel model, ConfirmWorkingTimeModel edit, List<string> errorList,Guid currenID,PortalEntities _context, string stype = null) {
            if (stype == null)
            {
                if (clsFunction.checkKyCongNguoiDung(edit.Date1) && edit.Disable1 != true)
                {
                    errorList.Add("Kỳ công đã khóa không thể sửa");
                }
            }
            if (edit.Disable1 == true && stype == null)
            {
                errorList.Add("Yêu cầu đã gửi mail thể sửa");
            }
            if (edit.BrowseStatusID == "4")
            {
                errorList.Add("Xác nhận công đã hủy không thể sửa");
            }
            else
            {
                var checkExist = _context.ConfirmWorkingTimeModels.Where(it => it.Type1 == model.Type1 && it.Date1 == model.Date1 && it.EmployeeId == model.Employee.EmployeeId && it.ConfirmWorkingTimeID != model.ConfirmWorkingTimeID).ToList();
                if (checkExist.Count > 0)
                {
                    errorList.Add("Đã tồn tại xác nhận công ngày " + model.Date1.ToString("dd/MM/yyyy"));
                }
                else
                {
                    edit.Time1 = TimeSpan.Parse(model.Time1);
                    edit.Date1 = model.Date1;
                    edit.Type1 = model.Type1;
                    edit.Reason = model.Reason;
                   // edit.Disable1 = false;

                    if (stype == "nhansu")
                    {
                        if (model.BrowseStatusID == "4" && (model.ReasonStop == null || model.ReasonStop == ""))
                        {
                            errorList.Add("Bạn chưa nhập lý do hủy");
                        }
                        edit.BrowseStatusID = model.BrowseStatusID;
                        edit.ReasonStop = model.ReasonStop;
                    }
                }

            }
        }
        public static string EmailContents(string receiver,ConfirmWorkingTimeModel item, string contents, string reasonStop = null) {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
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
                sb.Append("<td>Ngày: <b>" + Convert.ToDateTime(item.Date1).Date.ToString("dd/MM/yyyy") + "</b> </td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td>Thời gian: <b>" + item.Time1.ToString(@"hh\:mm") + "</b></td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td>Loại: <b>" + item.Type1.Replace("1","Giờ vào").Replace("2","Giờ ra") + "</b></td>");
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
    public class TypeTime {
        public string Type1 { get; set; }
        public string Name { get; set; }
    }
}