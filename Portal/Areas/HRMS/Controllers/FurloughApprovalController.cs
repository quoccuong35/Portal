using Portal.Extensions;
using Portal.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.EntityModels;
using Portal.ViewModels;
using System.Transactions;
using System.Text;
using System.Data.Entity.Infrastructure;
using HRMS.Models;
namespace HRMS.Controllers
{
    public class FurloughApprovalController : BaseController
    {
        // GET: FurloughApproval Duyệt nghỉ phép
        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            ViewBag.LeaveCategoryId = Data.LeaveCategorieBag();
            ViewBag.DepartmentID = Data.DepartmentViewBag();
            return View();
        }
        public ActionResult _Search(FurloughSearchView search)
        {
            Guid acc = new Guid(CurrentUser.AccountId);
            var account = _context.Accounts.FirstOrDefault(it => it.AccountId == acc);
            Guid employeeId = new Guid(CurrentUser.EmployeeID);
            DateTime dNow = DateTime.Now;
            if (search.FromDate == null || search.FromDate == new DateTime(0001, 01, 01))
            {
                search.FromDate = new DateTime(dNow.Date.Year, dNow.Month, 01);
            }
            if (search.ToDate == null || search.ToDate == new DateTime(0001, 01, 01))
            {
                search.ToDate = dNow;
            }
            var data = (from np in _context.FurloughModels
                        from b in _context.TimeKeepingPeriodModels.Where(it => it.FromDate <= np.FromDate && np.FromDate <= it.ToDate && it.Actived == true && it.Type == "1")
                        where np.BrowseStatusID == "2" &&
                        np.ApprovalHistoryModels.Any(it => it.BrowseStatusID == "2" && it.EmployeeId == employeeId)
                        && (search.LeaveCategoryId == null || np.LeaveCategoryId == search.LeaveCategoryId)
                                   && (search.EmployeeCode == null || np.EmployeeModel.EmployeeCode.Contains(search.EmployeeCode))
                                   && (search.FullName == null || np.EmployeeModel.FullName.Contains(search.FullName))
                                  && (search.DepartmentID == null || np.EmployeeModel.DepartmentID == search.DepartmentID)
                                  && (np.FromDate >= search.FromDate && np.FromDate <= search.ToDate)
                        select np).ToList().SelectMany(it => it.ApprovalHistoryModels).ToList();

            return PartialView(data);
        }
        #endregion Index

        #region Duyệt nghỉ phép
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Approval(List<Guid> lid)
        {
            try
            {
                using (var tran = new TransactionScope())
                {
                    Guid accId = new Guid(CurrentUser.AccountId);
                    var acc = _context.Accounts.FirstOrDefault(it => it.AccountId == accId);
                    List<string> errorList = new List<string>();
                    DateTime dtime = DateTime.Now;
                    List<Email> sendEmail = new List<Email>();
                    foreach (var id in lid)
                    {
                        //var furlough = _context.FurloughModels.FirstOrDefault(it => it.Del != true && it.BrowseStatusID != "4" && it.FurloughId == id);
                        var furlough = (from a in _context.FurloughModels
                                        from b in _context.TimeKeepingPeriodModels.Where(it => a.FromDate >= it.FromDate && a.FromDate <= it.ToDate && it.Actived == true && it.Type == "1")
                                        where a.BrowseStatusID == "2" && a.FurloughId == id
                                        select a).FirstOrDefault();
                        // var checkAccAp = furlough.ApprovalHistoryModels.ToList().Where(it => it.BrowseStatusID == "2" && it.EmployeeId == acc.EmployeeId).ToList(); ;
                        if (furlough == null)
                        {
                            errorList.Add(string.Format(LanguageResource.Error_NotExist, "có mã " + id.ToString()));
                            continue;
                        }
                        else if (clsFunction.checkKyCongNguoiDung(furlough.FromDate))
                        {
                            errorList.Add(string.Format(LanguageResource.CheckFeatApproval, "nghỉ phép " + furlough.EmployeeModel.FullName + " " + furlough.ToDate.ToString("dd/MM/yyyy")));
                            continue;
                        }
                        else
                        {
                            var checkAccAp = furlough.ApprovalHistoryModels.ToList().Where(it => it.BrowseStatusID == "2" && it.EmployeeId == acc.EmployeeId && it.ApprovalId == id).ToList(); ;
                            if (checkAccAp.Count == 0)
                            {
                                errorList.Add(string.Format("Không có quyền duyệt thông tin phép " + " " + furlough.EmployeeModel.FullName + " " + furlough.ToDate.ToString("dd/MM/yyyy")));
                            }
                            else
                            {
                                ApprovalItem(furlough, accId, dtime, sendEmail);
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
                        Data = string.Format(LanguageResource.Alert_App_Success, LanguageResource.FurloughModel.ToLower())
                    });
                }
            }
            catch (DbUpdateException ex)
            {
                string error = "";
                foreach (var errorMessage in ErrorHepler.GetaAllMessages(ex))
                {
                    error += errorMessage + "/n";
                    //ModelState.AddModelError("", errorMessage);
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.NotModified,
                    Success = false,
                    Data = error
                });
            }
            // 2 handlw:DbEntityValidationException
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eve in ex.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                    eve.Entry.Entity.GetType().Name,
                                                    eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                    ve.PropertyName,
                                                    ve.ErrorMessage));
                    }
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.NotModified,
                    Success = false,
                    Data = sb.ToString()
                });
            }
            //3. handle: Exception
            catch (Exception ex)
            {
                string Message = "";
                if (ex.InnerException != null)
                {
                    Message = ex.InnerException.Message;
                }
                else
                {
                    Message = ex.Message;
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.NotModified,
                    Success = false,
                    Data = "Lỗi: " + Message
                });
            }
        }
        private void ApprovalItem(FurloughModel item, Guid acc, DateTime dtime, List<Email> email)
        {
            bool complete = false;
            int index = 0, approvalLevel = 0, count = 1;
            var listDeteil = item.ApprovalHistoryModels.Where(it => it.BrowseStatusID != "3" && it.BrowseStatusID != "4" && it.Type1 == Portal.Constant.ConstFunction.NghiPhep).OrderBy(it => it.ApprovalLevel).ToList();
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
                body = FurloughFunction.EmailContentsFurlough(receiver, item.LeaveCategory.LeaveCategoryName, item, "Yêu cầu nghỉ phép bên dưới của anh/chị đã hoàn tất");
            }
            else
            {
                to = to.Substring(0, to.Length - 1);
                receiver = receiver.Substring(0, receiver.Length - 1);
                cc = accCreate.EmployeeModel.CompanyEmail;
                body = FurloughFunction.EmailContentsFurlough(receiver, item.LeaveCategory.LeaveCategoryName, item, "Anh/Chị có yêu cầu duyệt nghỉ phép được gửi từ <b>" + accCreate.EmployeeModel.FullName + "</b>");
            }
            email.Add(new Email { To = to, CC = cc, Body = body, Title = LanguageResource.TitleFurlough });
            _context.SaveChanges();
        }
        #endregion Duyệt nghỉ phép

        #region Cancel nghỉ phép
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Cancel(List<Guid> lid, string lyDo)
        {
            try
            {
                using (var tran = new TransactionScope())
                {
                    Guid accId = new Guid(CurrentUser.AccountId);
                    var acc = _context.Accounts.FirstOrDefault(it => it.AccountId == accId);
                    List<string> errorList = new List<string>();
                    DateTime dtime = DateTime.Now;
                    List<Email> sendEmail = new List<Email>();
                    string body = "";
                    foreach (var id in lid)
                    {
                        var furlough = _context.FurloughModels.FirstOrDefault(it => it.BrowseStatusID == "2" && it.FurloughId == id);
                        if (furlough == null)
                        {
                            errorList.Add(string.Format(LanguageResource.Error_NotExist, "có mã " + id.ToString()));
                        }
                        else
                        {
                            if (clsFunction.checkKyCongNguoiDung(furlough.FromDate))
                            {
                                errorList.Add(string.Format(LanguageResource.CheckFeatApproval, "nghỉ phép " + furlough.EmployeeModel.FullName + " " + furlough.ToDate.ToString("dd/MM/yyyy")));
                            }
                            else
                            {
                                var checkAccAp = furlough.ApprovalHistoryModels.ToList().Where(it => it.BrowseStatusID == "2" && it.EmployeeId == acc.EmployeeId && it.ApprovalId == id).ToList(); ;
                                if (checkAccAp.Count == 0)
                                {
                                    errorList.Add(string.Format("Không có quyền hủy thông tin phép " + " " + furlough.EmployeeModel.FullName + " " + furlough.ToDate.ToString("dd/MM/yyyy")));
                                }
                                else
                                {
                                    var accCreate = _context.Accounts.FirstOrDefault(it => it.AccountId == furlough.CreatedAccountId);
                                    furlough.BrowseStatusID = "4";
                                    furlough.ReasonStop = lyDo;
                                    furlough.LastModifiedTime = dtime;
                                    furlough.LastModifiedAccountId = accId;
                                    var details = furlough.ApprovalHistoryModels.Where(it => it.BrowseStatusID != "3").ToList();
                                    details.ForEach(it => { it.BrowseStatusID = "4"; it.LastModifiedTime = dtime; it.LastModifiedAccountId = accId; });
                                    furlough.FurloughDetailModels.ToList().ForEach(it => { it.Del = true; it.DelAccountId = accId; it.DelTime = dtime; });

                                    body = FurloughFunction.EmailContentsFurlough(accCreate.EmployeeModel.FullName, furlough.LeaveCategory.LeaveCategoryName, furlough,
                                        "Yêu cầu nghỉ phép bên dưới của anh/chị đã bị từ chối bởi <b>" + acc.EmployeeModel.FullName + "</b>", lyDo);

                                    sendEmail.Add(new Email { To = accCreate.EmployeeModel.CompanyEmail, CC = acc.EmployeeModel.CompanyEmail, Title = LanguageResource.TitleFurlough, Body = body });
                                }

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
                    _context.SaveChanges();
                    tran.Complete();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Cancel_Success, LanguageResource.FurloughModel.ToLower())
                    });
                }
            }
            catch (DbUpdateException ex)
            {
                string error = "";
                foreach (var errorMessage in ErrorHepler.GetaAllMessages(ex))
                {
                    error += errorMessage + "/n";
                    //ModelState.AddModelError("", errorMessage);
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.NotModified,
                    Success = false,
                    Data = error
                });
            }
            // 2 handlw:DbEntityValidationException
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eve in ex.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                    eve.Entry.Entity.GetType().Name,
                                                    eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                    ve.PropertyName,
                                                    ve.ErrorMessage));
                    }
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.NotModified,
                    Success = false,
                    Data = sb.ToString()
                });
            }
            //3. handle: Exception
            catch (Exception ex)
            {
                string Message = "";
                if (ex.InnerException != null)
                {
                    Message = ex.InnerException.Message;
                }
                else
                {
                    Message = ex.Message;
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.NotModified,
                    Success = false,
                    Data = "Lỗi: " + Message
                });
            }
        }
        #endregion Cancel nghỉ phép

        #region View 
        [PortalAuthorization]
        public ActionResult View(Guid id)
        {
            var edit = _context.FurloughModels.FirstOrDefault(it => it.FurloughId == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.FurloughModel.ToLower()) });
            }
            var model = FurloughFunction.getDatTa(edit, CurrentUser.AccountId);
            ViewBag.TypeDate = FurloughFunction.ListLeaveType();
            ViewBag.LeaveCategoryId = Data.LeaveCategorieBag(model.LeaveCategoryId);
            return View(model);
        }
        #endregion View()
    }
}