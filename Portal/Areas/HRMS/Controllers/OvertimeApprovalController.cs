using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.ViewModels;
using System.Transactions;
using HRMS.Models;
using Portal.Resources;
using Portal.EntityModels;
using System.Data.Entity;

namespace HRMS.Controllers
{
    public class OvertimeApprovalController : BaseController
    {
        // GET: OvertimeApproval
        // duyệt tăng ca
        #region index
        public ActionResult Index()
        {
            ViewBag.DepartmentID = Data.DepartmentViewBag();
            return View();
        }
        public ActionResult _Search(FurloughSearchView model)
        {
            if (model.FromDate == null)
                model.FromDate = FirstDay();
            if (model.ToDate == null)
                model.ToDate = LastDay();

            Guid employeeID = GetEmployeeID(CurrentUser.EmployeeID);
            var data = (from ot in _context.OvertimeModels
                        from b in _context.TimeKeepingPeriodModels.Where(it => it.Actived == true && it.Type =="1" && it.FromDate <= ot.OvertimeDay && it.ToDate >= ot.OvertimeDay)
                        where ot.BrowseStatusID =="2"  && ot.ApprovalHistoryModels.Any(it=>it.BrowseStatusID =="2" && it.EmployeeId == employeeID && it.Type1 == Portal.Constant.ConstFunction.TangCa)
                        && ot.OvertimeDay>= model.FromDate && ot.OvertimeDay <= model.ToDate
                        && (model.DepartmentID == null || ot.DepartmentID == model.DepartmentID)
                        select ot
                        ).ToList();
            return PartialView(data);
        }
        #endregion index

        #region Approval
        [PortalAuthorization]
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public ActionResult Approval(List<Guid> lid)
        {
            return ExcuteImportExcel(() =>
            {
                using (var tran = new TransactionScope())
                {
                    Guid accId = GetAccountID(CurrentUser.AccountId);
                    //var acc = clsFunction.GetAccount(accId,_context);
                    Guid employeeId = GetEmployeeID(CurrentUser.EmployeeID);
                    List<string> errorList = new List<string>();
                    DateTime dtime = DateTime.Now;
                    List<Email> sendEmail = new List<Email>();
                    foreach (var id in lid)
                    {
                        var edit = _context.OvertimeModels.FirstOrDefault(it => it.BrowseStatusID == "2" &&
                                   it.ApprovalHistoryModels.Any(p => p.BrowseStatusID == "2" && p.EmployeeId == employeeId));
                        if (edit == null)
                        {
                            errorList.Add(string.Format(LanguageResource.Error_NotExist, "có mã " + id.ToString()));
                            continue;
                        }
                        else
                        {
                            if (clsFunction.checkKyCongNguoiDung(edit.OvertimeDay))
                            {
                                errorList.Add("Kỳ công đã khóa không thể duyệt tăng ca ngày " + edit.OvertimeDay.ToString("dd/MM/yyyy"));
                            }
                            else
                            {
                                ApprovalItem(edit, accId, dtime, sendEmail);
                            }
                        }
                    }
                    if (errorList != null && errorList.Count > 0)
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.Created,
                            Success = false,
                            Data = errorList
                        });
                    }
                    if (sendEmail.Count > 0)
                    {
                        foreach (var item in sendEmail)
                        {
                            FunctionExtensions.SendMail(item.Title, item.To, item.CC, item.Body);
                        }
                    }
                    tran.Complete();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_App_Success, LanguageResource.Overtime.ToLower())
                    });
                }
            });
        }
        private void ApprovalItem(OvertimeModel item, Guid acc, DateTime dtime, List<Email> email)
        {
            bool complete = false;
            int index = 0, approvalLevel = 0, count = 1;
            var listDeteil = item.ApprovalHistoryModels.Where(it => it.BrowseStatusID != "3" && it.BrowseStatusID != "4" && it.Type1 == Portal.Constant.ConstFunction.TangCa).OrderBy(it => it.ApprovalLevel).ToList();
            string to = "", cc = "", receiver = "", body = "";

            foreach (var detail in listDeteil)
            {

                if (detail.BrowseStatusID == "2")
                {
                    detail.BrowseStatusID = "3";
                    detail.LastModifiedAccountId = acc;
                    detail.LastModifiedTime = dtime;
                    approvalLevel = detail.ApprovalLevel;
                }
                else
                {
                    if ((index == 0 || index == detail.ApprovalLevel) && detail.ApprovalLevel > approvalLevel && detail.BrowseStatusID == "1")
                    {
                        detail.BrowseStatusID = "2";
                        //detail.LastModifiedAccountId = acc;
                        //detail.LastModifiedTime = dtime;
                        index = detail.ApprovalLevel;
                        to += detail.EmployeeModel.CompanyEmail + ",";
                        receiver += detail.EmployeeModel.FullName + ",";
                    }
                    else
                    {
                        break;
                    }
                }
                if (count == listDeteil.Count && detail.BrowseStatusID == "3")
                {
                    complete = true;
                }
                count++;
            }
            int songuoiOT = item.OvertimeDetailModels.Where(it => it.Del != true).Count();
            var accCreate = _context.Accounts.FirstOrDefault(it => it.AccountId == item.CreatedAccountId);

            if (complete)
            {
                item.BrowseStatusID = "3";
                cc = "";
                receiver = accCreate.EmployeeModel.FullName;
                to = accCreate.EmployeeModel.CompanyEmail;
                body = OvertimeFunction.EmailContent(receiver, songuoiOT.ToString(), item, "Yêu cầu tăng ca bên dưới của anh/chị đã hoàn tất");
            }
            else
            {
                to = to.Substring(0, to.Length - 1);
                receiver = receiver.Substring(0, receiver.Length - 1);
                cc = accCreate.EmployeeModel.CompanyEmail;
                body = OvertimeFunction.EmailContent(receiver, songuoiOT.ToString(), item, "Anh/Chị có yêu cầu duyệt tăng ca được gửi từ <b>" + accCreate.EmployeeModel.FullName + "</b>");
            }
            email.Add(new Email { To = to, CC = cc, Body = body, Title = LanguageResource.TitleOvertime });
            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();
        }
        #endregion Approval

        #region Cancel
        [PortalAuthorization]
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(List<Guid> lid, string lyDo)
        {
            return ExcuteImportExcel(() =>
            {
                using (var tran = new TransactionScope())
                {
                    Guid accId = new Guid(CurrentUser.AccountId);
                    var acc = clsFunction.GetAccount(accId, _context);
                    List<string> errorList = new List<string>();
                    DateTime dtime = DateTime.Now;
                    List<Email> sendEmail = new List<Email>();
                    string body = "";
                    Guid employeeId = GetEmployeeID(CurrentUser.EmployeeID);
                    foreach (var id in lid)
                    {
                        var edit = _context.OvertimeModels.FirstOrDefault(it => it.BrowseStatusID == "2" &&
                                  it.ApprovalHistoryModels.Any(p => p.BrowseStatusID == "2" && p.EmployeeId == employeeId));
                        if (edit == null)
                        {
                            errorList.Add(string.Format(LanguageResource.Error_NotExist, "có mã " + id.ToString()));
                            continue;
                        }
                        else
                        {
                            if (clsFunction.checkKyCongNguoiDung(edit.OvertimeDay))
                            {
                                errorList.Add("Kỳ công đã khóa không thể duyệt tăng ca ngày " + edit.OvertimeDay.ToString("dd/MM/yyyy"));
                            }
                            else
                            {
                                edit.BrowseStatusID = "4";
                                edit.ReasonStop = lyDo;
                                edit.LastModifiedAccountId = accId;
                                edit.LastModifiedTime = dtime;

                                /// Xóa luôn chi tiết
                                int iCount = edit.OvertimeDetailModels.Where(it => it.Del != true).ToList().Count;
                                edit.OvertimeDetailModels.Where(it=>it.Del !=true).ToList().ForEach(it => {
                                    it.Del = true;
                                    it.DelAccountId = accId;
                                    it.DelTime = dtime;
                                });
                                // Cập nhật luôn bên his tori
                                edit.ApprovalHistoryModels.Where(it => it.BrowseStatusID != "3").ToList().ForEach(it =>
                                {
                                    it.BrowseStatusID = "4";
                                    it.LastModifiedAccountId = accId;
                                    it.LastModifiedTime = dtime;
                                });
                                var accCreate = _context.Accounts.FirstOrDefault(it => it.AccountId == edit.CreatedAccountId);
                                body = OvertimeFunction.EmailContent(accCreate.EmployeeModel.FullName, iCount.ToString(), edit, "Yêu cầu tăng ca bên dưới của anh/chị đã bị từ chối bởi <b>" + acc.EmployeeModel.FullName + "</b>", lyDo);
                                sendEmail.Add(new Email { To = accCreate.EmployeeModel.CompanyEmail, CC = acc.EmployeeModel.CompanyEmail, Title = LanguageResource.TitleFurlough, Body = body });
                                _context.Entry(edit).State = EntityState.Modified;
                                _context.SaveChanges();
                            }
                        }
                    }
                    if (errorList != null && errorList.Count > 0)
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.Created,
                            Success = false,
                            Data = errorList
                        });
                    }
                    if (sendEmail.Count > 0)
                    {
                        foreach (var item in sendEmail)
                        {
                            FunctionExtensions.SendMail(item.Title, item.To, item.CC, item.Body);
                        }
                    }
                    tran.Complete();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Cancel_Success, LanguageResource.Overtime.ToLower())
                    });
                }
                   
            });
        }
        #endregion Cancel
        [PortalAuthorization]
        public ActionResult View(Guid id)
        {
            var info = _context.OvertimeModels.FirstOrDefault(it => it.OvertimeId == id);
            if (info == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Overtime.ToLower()) });
            }
            OvertimeViewModel model = OvertimeFunction.getDatTa(info, CurrentUser.AccountId, true);
            ViewBag.DepartmentID = Data.DepartmentViewBag(info.DepartmentID);
            return View(model);
        }
    }
}