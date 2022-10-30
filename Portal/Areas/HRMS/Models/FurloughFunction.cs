using HRMS.Controllers;
using Portal.EntityModels;
using Portal.Extensions;
using Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Models
{
    public static class FurloughFunction
    {
        public static FurloughViewModel getDatTa(FurloughModel edit,string accID = null) {

            FurloughViewModel model = new FurloughViewModel();
            if (edit != null)
            {
                EmployeeInfoView employee = new EmployeeInfoView();
                List<FurloughDetailViewModel> detail = new List<FurloughDetailViewModel>();
                employee.EmployeeCode = edit.EmployeeModel.EmployeeCode;
                employee.DepartmentName = edit.EmployeeModel.Department.DepartmentName;
                employee.RemainingLeavedays = edit.EmployeeModel.RemainingLeavedays.ToString();
                employee.EmployeeId = edit.EmployeeModel.EmployeeId;
                employee.FullName = edit.EmployeeModel.FullName;
                employee.Readonly = true;

                model.Employee = employee;
                model.FromDate = edit.FromDate;
                model.ToDate = edit.ToDate;
                model.LeaveCategoryId = edit.LeaveCategoryId;
                model.Reason = edit.Reason;
                model.ReasonStop = edit.ReasonStop;
                model.FurloughId = edit.FurloughId;
                model.FurloughDetail = new List<FurloughDetailViewModel>();
                model.Lock = edit.Lock;
                model.CreatedAccountId = edit.CreatedAccountId;
                model.AccountID = new Guid(accID);
                model.BrowseStatusID = edit.BrowseStatusID;
                foreach (var item in edit.FurloughDetailModels)
                {
                    if (item.Del == true)
                        continue;
                    string note = "";
                    if (item.DayOff.DayOfWeek == DayOfWeek.Saturday)
                    {
                        note = "Thứ 7";
                    }
                    detail.Add(new FurloughDetailViewModel()
                    {
                        DayOff = item.DayOff,
                        TypeDate = item.TypeDate,
                        FurloughId = edit.FurloughId,
                        Check = true,
                        Note = note
                    });
                }
                model.FurloughDetail = detail;

                model.AppHistory = new List<ApprovalHistoryModel>();
                model.AppHistory.AddRange(edit.ApprovalHistoryModels.ToList());
            }
            return model;
        }

        public static void UpData(FurloughViewModel model, FurloughModel edit, List<string> errorList, bool isHasPermission, Guid currenID, string stype = null)
        {
            Guid phepP = new Guid("44F08DD8-DC39-4A63-A8B9-E731B10A0368"); //Loại phép P
            if (!isHasPermission)
            {
                if (clsFunction.checkKyCongNguoiDung(edit.FromDate) && edit.Lock != true)
                {
                    errorList.Add("Kỳ công đã khóa không thể sửa");
                }
            }
            if (edit.Lock == true && !isHasPermission)
            {
                errorList.Add("Yêu cầu đã gửi mail thể sửa");
            }
             if (edit.BrowseStatusID == "4" )
            {
                errorList.Add("Phép đã hủy không thể sửa");
            }
            else
            {
                edit.Reason = model.Reason;
                double soNgayPhepNghi = edit.FurloughDetailModels.Where(it => it.Del != true).Select(it => it.NumberDay).Sum();

                if (soNgayPhepNghi % 1 > 0 && model.LeaveCategoryId != new Guid("44F08DD8-DC39-4A63-A8B9-E731B10A0368") && model.LeaveCategoryId != new Guid("D8EB0F0A-C7BA-477C-A39D-E25EE7071167"))
                {
                    errorList.Add("Loại phép bạn chọn không được nghỉ 0.5 ngày");
                }

                if (edit.LeaveCategoryId == phepP && model.LeaveCategoryId != phepP)
                {
                    edit.EmployeeModel.RemainingLeavedays = edit.EmployeeModel.RemainingLeavedays + soNgayPhepNghi;
                }
                else if (edit.LeaveCategoryId != phepP && model.LeaveCategoryId == phepP)
                {
                    double remainingLeavedays = edit.EmployeeModel.RemainingLeavedays == null ? 0 : edit.EmployeeModel.RemainingLeavedays.Value;
                    if (soNgayPhepNghi > remainingLeavedays)
                    {
                        errorList.Add("Số ngày phép bạn yêu cầu nghỉ được hưởng lương vượt quá ngày phép còn lại");
                    }
                    else
                    {
                        edit.EmployeeModel.RemainingLeavedays = edit.EmployeeModel.RemainingLeavedays - soNgayPhepNghi;
                    }
                }
                edit.LeaveCategoryId = model.LeaveCategoryId.Value;
                if (stype == "nhansu")
                {
                    if (model.BrowseStatusID == "4" && (model.ReasonStop == null || model.ReasonStop == ""))
                    {
                        errorList.Add("Bạn chưa nhập lý do hủy");
                    }
                    foreach (var item in edit.ApprovalHistoryModels)
                    {
                        if (item.BrowseStatusID != "3")
                        {
                            item.BrowseStatusID = "4";
                            item.LastModifiedAccountId = currenID;
                            item.LastModifiedTime = DateTime.Now;
                        }
                    }
                    // nếu là từ trối và là phép năm cộng phép lại cho ta
                    if (model.BrowseStatusID == "4" && edit.LeaveCategoryId == phepP)
                    {
                        edit.EmployeeModel.RemainingLeavedays = edit.EmployeeModel.RemainingLeavedays + soNgayPhepNghi;
                    }
                    edit.BrowseStatusID = model.BrowseStatusID;
                    edit.ReasonStop = model.ReasonStop;
                }

            }
        }
        public static List<LeaveTypeDate> ListLeaveType()
        {
            List<LeaveTypeDate> list = new List<LeaveTypeDate>();
            list.Add(new LeaveTypeDate() { TypeDate = "1", TypeDateName = "Cả ngày" });
            list.Add(new LeaveTypeDate() { TypeDate = "2", TypeDateName = "Buổi sáng" });
            list.Add(new LeaveTypeDate() { TypeDate = "3", TypeDateName = "Buổi chiều" });
            //ViewBag.TypeDate =  (); ;
            return list;
        }
        public static SelectList LeaveTypeDateViewBag(string TypeDate = null)
        {
            var data = ListLeaveType();
            return new SelectList(data, "TypeDate", "TypeDateName", TypeDate);
        }
        public static string EmailContentsFurlough(string receiver, string leaveCategoryName, FurloughModel item, string contents, string reasonStop = null)
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
                sb.Append("<td> Từ Ngày: <b>" + Convert.ToDateTime(item.FromDate).Date.ToString("dd/MM/yyyy") + "</b> - Đến Ngày: <b>" + Convert.ToDateTime(item.ToDate).Date.ToString("dd/MM/yyyy") + "</b> </td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td>Số ngày nghỉ: <b>" + item.NumberOfDaysOff.ToString() + "</b></td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td>Loại phép: <b>" + leaveCategoryName + "</b></td>");
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
    public class LeaveTypeDate
    {
        public string TypeDate { get; set; }
        public string TypeDateName { get; set; }
    }
}