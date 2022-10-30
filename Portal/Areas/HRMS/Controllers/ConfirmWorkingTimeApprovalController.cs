using HRMS.Models;
using Portal.Extensions;
using Portal.Resources;
using Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Portal.EntityModels;

namespace HRMS.Controllers
{
    public class ConfirmWorkingTimeApprovalController : BaseController
    {
        // GET: ConfirmWorkingTimeApproval
        // Duyệt xác nhận công
        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            ViewBag.DepartmentID = Data.DepartmentViewBag();
            return View();
        }
        public ActionResult _Search(EmployeeSearch model)
        {
            return ExecuteSearch(() =>
            {
                if (model.FromDate == null)
                    model.FromDate = FirstDay();
                if (model.ToDate == null)
                    model.ToDate = LastDay();
                var data = (from cwtm in _context.ConfirmWorkingTimeModels
                            from b in _context.TimeKeepingPeriodModels.Where(it=>it.FromDate<= cwtm.Date1 && it.ToDate>= cwtm.Date1 && it.Actived==true && it.Type == "1")
                            where cwtm.BrowseStatusID == "2" &&
                            (model.EmployeeCode == null || cwtm.EmployeeModel.EmployeeCode.Contains(model.EmployeeCode)) &&
                            (model.FullName == null || cwtm.EmployeeModel.FullName.Contains(model.FullName)) &&
                            (model.DepartmentID == null || cwtm.EmployeeModel.DepartmentID == model.DepartmentID) &&
                            cwtm.ApprovalHistoryModels.Any(p=>p.BrowseStatusID == "2" && p.EmployeeId == new Guid(CurrentUser.EmployeeID))
                            select cwtm
                            ).ToList();
                return PartialView(data);
            });
        }
        #endregion
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
                        var edit = _context.ConfirmWorkingTimeModels.FirstOrDefault(it => it.BrowseStatusID == "2" &&
                                   it.ApprovalHistoryModels.Any(p => p.BrowseStatusID == "2" && p.EmployeeId == employeeId));
                        if (edit == null)
                        {
                            errorList.Add(string.Format(LanguageResource.Error_NotExist, "có mã " + id.ToString()));
                            continue;
                        }
                        else
                        {
                            if (clsFunction.checkKyCongNguoiDung(edit.Date1))
                            {
                                errorList.Add("Kỳ công đã khóa không thể duyệt ngày " + edit.Date1.ToString("dd/MM/yyyy"));
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
                        Data = string.Format(LanguageResource.Alert_App_Success, LanguageResource.ConfirmWorkingTime.ToLower())
                    });
                }
            });
        }
        private void ApprovalItem(ConfirmWorkingTimeModel item, Guid acc, DateTime dtime, List<Email> email)
        {
            bool complete = false;
            int index = 0, approvalLevel = 0, count = 1;
            var listDeteil = item.ApprovalHistoryModels.Where(it => it.BrowseStatusID != "3" && it.BrowseStatusID != "4" && it.Type1 == Portal.Constant.ConstFunction.XacNhanCong).OrderBy(it => it.ApprovalLevel).ToList();
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
            var accCreate = _context.Accounts.FirstOrDefault(it => it.AccountId == item.CreatedAccountId);

            if (complete)
            {
                item.BrowseStatusID = "3";
                cc = "";
                receiver = accCreate.EmployeeModel.FullName;
                to = accCreate.EmployeeModel.CompanyEmail;
                body = ConfirmWorkingTimeFunction.EmailContents(receiver, item, "Yêu cầu xác nhận công bên dưới của anh/chị đã hoàn tất");
            }
            else
            {
                to = to.Substring(0, to.Length - 1);
                receiver = receiver.Substring(0, receiver.Length - 1);
                cc = accCreate.EmployeeModel.CompanyEmail;
                body = ConfirmWorkingTimeFunction.EmailContents(receiver, item, "Anh/Chị có yêu cầu xác nhận công được gửi từ <b>" + accCreate.EmployeeModel.FullName + "</b>");
            }
            email.Add(new Email { To = to, CC = cc, Body = body, Title = LanguageResource.TitleConfrimWorkingTime });
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
                        var edit = _context.ConfirmWorkingTimeModels.FirstOrDefault(it => it.BrowseStatusID == "2" &&
                                  it.ApprovalHistoryModels.Any(p => p.BrowseStatusID == "2" && p.EmployeeId == employeeId));
                        if (edit == null)
                        {
                            errorList.Add(string.Format(LanguageResource.Error_NotExist, "có mã " + id.ToString()));
                            continue;
                        }
                        else
                        {
                            if (clsFunction.checkKyCongNguoiDung(edit.Date1))
                            {
                                errorList.Add("Kỳ công đã khóa không thể duyệt ngày " + edit.Date1.ToString("dd/MM/yyyy"));
                            }
                            else
                            {
                                edit.BrowseStatusID = "4";
                                edit.ReasonStop = lyDo;
                                edit.LastModifiedAccountId = accId;
                                edit.LastModifiedTime = dtime;

                                // Cập nhật luôn bên his tori
                                edit.ApprovalHistoryModels.Where(it => it.BrowseStatusID != "3").ToList().ForEach(it =>
                                {
                                    it.BrowseStatusID = "4";
                                    it.LastModifiedAccountId = accId;
                                    it.LastModifiedTime = dtime;
                                });
                                var accCreate = _context.Accounts.FirstOrDefault(it => it.AccountId == edit.CreatedAccountId);
                                body = ConfirmWorkingTimeFunction.EmailContents(accCreate.EmployeeModel.FullName,  edit, "Yêu cầu xác nhận công bên dưới của anh/chị đã bị từ chối bởi <b>" + acc.EmployeeModel.FullName + "</b>", lyDo);
                                sendEmail.Add(new Email { To = accCreate.EmployeeModel.CompanyEmail, CC = acc.EmployeeModel.CompanyEmail, Title = LanguageResource.TitleConfrimWorkingTime, Body = body });
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
                        Data = string.Format(LanguageResource.Alert_Cancel_Success, LanguageResource.ConfirmWorkingTime.ToLower())
                    });
                }

            });
        }

        #endregion Cancel
        [PortalAuthorization]
        [HttpGet]
        public ActionResult View(Guid id) {
            var info = _context.ConfirmWorkingTimeModels.FirstOrDefault(it => it.ConfirmWorkingTimeID == id);
            if (info == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.ConfirmWorkingTime.ToLower()) });
            }
            ConfirmWorkingTimeViewModel model = ConfirmWorkingTimeFunction.GetData(info, CurrentUser.AccountId, true);
            ViewBag.Type1 = ConfirmWorkingTimeFunction.GetType();
            return View(model);
        }
    }
}