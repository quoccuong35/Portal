using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.ViewModels;
using Portal.EntityModels;
using HRMS.Models;
using System.Data.Entity;
using Portal.Resources;
using Portal.Repositories;
using System.Transactions;

namespace HRMS.Controllers
{
    public class ConfirmWorkingTimeController : BaseController
    {
        // GET: ConfirmWorkingTime
        // Xác nhận giờ vào hoặc ra khi quên chấm công
        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(DateTime? FromDate, DateTime? ToDate)
        {
            return ExecuteSearch(() =>
            {
                if (FromDate == null)
                    FromDate = FirstDay();
                if (ToDate == null)
                    ToDate = LastDay();
                Guid accID = new Guid(CurrentUser.AccountId);
                var data = _context.ConfirmWorkingTimeModels.Where(it => it.CreatedAccountId == accID &&
                            it.Date1 >= FromDate && it.Date1 <= ToDate).ToList();
                            
                return PartialView(data);
            });
        }
        #endregion Index
        #region Create
        public ActionResult Create() {
            ConfirmWorkingTimeViewModel model = new ConfirmWorkingTimeViewModel();
            model.Date1 = DateTime.Now.Date;
            model.Disable1 = false;
            var employee = clsFunction.SearchEmployee(CurrentUser.EmployeeCode,false,_context);
            model.Employee = employee;
            ViewBag.Type1 = ConfirmWorkingTimeFunction.GetType();
            return View(model);
        }
        [PortalAuthorization]
        [ValidateAntiForgeryToken]
        [ValidateAjax]
        [HttpPost]
        public JsonResult Create(ConfirmWorkingTimeViewModel model) {
            return ExecuteContainer(() =>
            {
                List<string> errorList = new List<string>();
                ConfirmWorkingTimeModel add = new ConfirmWorkingTimeModel();
                ConfirmWorkingTimeFunction.Create(model, add, errorList, _context,new Guid(CurrentUser.AccountId));
                // add quy trình duyệt
                if (errorList != null && errorList.Count > 0)
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = false,
                        Data = errorList
                    });
                }
                _context.Entry(add).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.ConfirmWorkingTime.ToLower())
                });
            });
        }
        #endregion Create
        [PortalAuthorization]
        [HttpGet]
        public ActionResult Edit(Guid id) {
            Guid currenID = new Guid(CurrentUser.AccountId);
            var edit = _context.ConfirmWorkingTimeModels.FirstOrDefault(it => it.ConfirmWorkingTimeID == id && it.CreatedAccountId == currenID);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.FurloughModel.ToLower()) });
            }
            var model = ConfirmWorkingTimeFunction.GetData(edit, CurrentUser.AccountId,false);
            ViewBag.Type1 = ConfirmWorkingTimeFunction.GetType(model.Type1);
            return View(model);
        }
        [PortalAuthorization]
        [ValidateAjax]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(ConfirmWorkingTimeViewModel model) {
            return ExecuteContainer(() =>
            {
                Guid currenID = new Guid(CurrentUser.AccountId);
                var edit = _context.ConfirmWorkingTimeModels.FirstOrDefault(it => it.ConfirmWorkingTimeID == model.ConfirmWorkingTimeID && it.CreatedAccountId == currenID);
                if (edit != null)
                {
                    List<string> errorList = new List<string>();
                    ConfirmWorkingTimeFunction.UpData(model, edit, errorList, currenID, _context);

                    if (errorList.Count > 0)
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.NotModified,
                            Success = false,
                            Data = errorList
                        });
                    }
                    HistoryRepository _repository = new HistoryRepository(_context);
                    _repository.SaveUpdateHistory(edit.ConfirmWorkingTimeID, CurrentUser.UserName, edit);

                    edit.LastModifiedAccountId = new Guid(CurrentUser.AccountId);
                    edit.LastModifiedTime = DateTime.Now;
                    _context.Entry(edit).State = EntityState.Modified;

                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.FurloughModel.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.CheckData, LanguageResource.ConfirmWorkingTime.ToLower())
                    });
                }
            });
        }

        #region Delete
        [PortalAuthorization]
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var del = _context.ConfirmWorkingTimeModels.FirstOrDefault(it => it.ConfirmWorkingTimeID == id &&
                it.BrowseStatusID == "1" && it.CreatedAccountId == new Guid(CurrentUser.AccountId));
                if (del != null)
                {
                    _context.Entry(del).State = EntityState.Deleted;
                    var hisApp = _context.ApprovalHistoryModels.Where(it => it.ApprovalId == del.ConfirmWorkingTimeID && it.Type1 == Portal.Constant.ConstFunction.XacNhanCong).ToList();
                    if (hisApp != null && hisApp.Count > 0)
                    {
                        _context.ApprovalHistoryModels.RemoveRange(hisApp);
                    }
                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.ConfirmWorkingTime.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.CheckData, LanguageResource.ConfirmWorkingTime.ToLower())
                    });
                }
            });
        }
        #endregion Delete

        #region SendEmail
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult SendEmail(Guid idSendEmail)
        {
            using (var tran = new TransactionScope())
            {
                try
                {
                    var model = _context.ConfirmWorkingTimeModels.FirstOrDefault(it => it.ConfirmWorkingTimeID == idSendEmail && it.CreatedAccountId == new Guid(CurrentUser.AccountId));
                    Guid accountID = new Guid(CurrentUser.AccountId);
                    var account = _context.Accounts.FirstOrDefault(it => it.AccountId == accountID);
                    string cc = account.EmployeeModel.CompanyEmail, to = "", receiver = ""; ;
                    if (model != null)
                    {
                        List<String> errorList = new List<string>();
                        // kiểm tra xem kỳ công có khóa chưa
                        if (clsFunction.checkKyCongNguoiDung(model.Date1))
                        {
                            errorList.Add(string.Format(LanguageResource.CheckFeat, LanguageResource.SendMail.ToLower()));
                        }
                        if ( !model.Disable1)
                        {
                            model.Disable1 = true;
                            model.BrowseStatusID = "2";
                            _context.SaveChanges();

                        }
                        // lấy to và cc đã lưu quy trình duyệt

                        var check = model.ApprovalHistoryModels.Where(it => it.BrowseStatusID == "2").ToList();
                        if (check.Count > 0)
                        {
                            foreach (var item in check)
                            {
                                to += item.EmployeeModel.CompanyEmail + ",";
                                receiver += item.EmployeeModel.FullName + ",";
                            }
                        }

                        if (to != "")
                        {
                            to = to.Substring(0, to.Length - 1);
                            receiver = receiver.Substring(0, receiver.Length - 1);
                            string body = ConfirmWorkingTimeFunction.EmailContents(receiver, model, "Anh/Chị có yêu cầu duyệt xác nhận công được gửi từ <b>" + account.EmployeeModel.FullName + "</b>");
                            if (body != "" && FunctionExtensions.SendMail(LanguageResource.TitleConfrimWorkingTime, to, cc, body))
                            {
                                tran.Complete();
                                return Json(new
                                {
                                    Code = System.Net.HttpStatusCode.NotModified,
                                    Success = true,
                                    Data = LanguageResource.Sendmail_Success
                                });

                            }
                            else
                            {
                                return Json(new
                                {
                                    Code = System.Net.HttpStatusCode.NotModified,
                                    Success = false,
                                    Data = LanguageResource.Sendmail_Error
                                });
                            }

                        }
                        else
                        {
                            return Json(new
                            {
                                Code = System.Net.HttpStatusCode.NotModified,
                                Success = false,
                                Data = "Không tìm thấy thông tin mail gửi đi"
                            });
                        }

                    }
                    else
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.NotModified,
                            Success = false,
                            Data = string.Format(LanguageResource.Error_NotExist, LanguageResource.ConfirmWorkingTime.ToLower())
                        });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = ex.Message.ToString()
                    });
                }

            }
        }

        #endregion SendEmail
    }
}