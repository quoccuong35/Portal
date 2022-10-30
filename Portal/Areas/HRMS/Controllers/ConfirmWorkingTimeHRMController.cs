using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.EntityModels;
using Portal.ViewModels;
using HRMS.Models;
using Portal.Resources;
using Portal.Repositories;
using System.Data.Entity;

namespace HRMS.Controllers
{
    public class ConfirmWorkingTimeHRMController : BaseController
    {
        // GET: ComfirmWorkingTimeHRM
        // quản lý chung thông tin xác nhận công

        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            ViewBag.DepartmentID = Data.DepartmentViewBag();
            return View();
        }
        public ActionResult _Search(EmployeeSearch model) {
            return ExecuteSearch(() =>
            {
                if (model.FromDate == null)
                    model.FromDate = FirstDay();
                if (model.ToDate == null)
                    model.ToDate = LastDay();
                var data = _context.ConfirmWorkingTimeModels.Where(it => it.Date1 >= model.FromDate && it.Date1 <= model.ToDate &&
                          (model.DepartmentID == null || it.EmployeeModel.DepartmentID == model.DepartmentID) &&
                           (model.EmployeeCode == null || it.EmployeeModel.EmployeeCode.Contains(model.EmployeeCode)) &&
                            (model.FullName == null || it.EmployeeModel.FullName.Contains(model.FullName))
                          ).ToList();
                return PartialView(data);
            });
        }
        #endregion Index
        #region Edit
        [PortalAuthorization]
        [HttpGet]
        public ActionResult Edit(Guid id) {
            var edit = _context.ConfirmWorkingTimeModels.FirstOrDefault(it => it.ConfirmWorkingTimeID == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.TitleConfrimWorkingTime.ToLower()) });
            }
            var data = ConfirmWorkingTimeFunction.GetData(edit, CurrentUser.AccountId, true);
            TrangThaiDuyet();
            ViewBag.Type1 = ConfirmWorkingTimeFunction.GetType();
            return View(data);
        }
        [PortalAuthorization]
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(ConfirmWorkingTimeViewModel model)
        {
            return ExecuteContainer(() => {
                var edit = _context.ConfirmWorkingTimeModels.FirstOrDefault(it => it.ConfirmWorkingTimeID == model.ConfirmWorkingTimeID);
                List<string> errorList = new List<string>();
                if (edit != null)
                {
                    ConfirmWorkingTimeFunction.UpData(model, edit, errorList, new Guid(CurrentUser.AccountId), _context,"nhansu");

                }
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
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.ConfirmWorkingTime.ToLower())
                });
            });
        }
        #endregion Edit
        #region Helper
        private void TrangThaiDuyet(string BrowseStatusID = null)
        {
            var browseStatus = _context.BrowseStatusModels.Where(it => it.BrowseStatusID == "4" || it.BrowseStatusID == "2").ToList();

            ViewBag.BrowseStatusID = new SelectList(browseStatus, "BrowseStatusID", "BrowseStatusName", BrowseStatusID);
        }
        #endregion Helper
    }
}