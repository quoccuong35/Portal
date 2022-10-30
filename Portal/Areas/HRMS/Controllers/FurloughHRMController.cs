using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.EntityModels;
using Portal.ViewModels;
using HRMS.Models;
using System.Web.Helpers;
using Portal.Resources;
using System.Web.Mvc.Html;
using Portal.Repositories;
using System.Data.Entity;

namespace HRMS.Controllers
{
    public class FurloughHRMController : BaseController
    {
        // GET: FurloughHRM 
        // Quản lý nghỉ phép nhân sự
        // Quyền truy cập
        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            ViewBag.LeaveCategoryId = Data.LeaveCategorieBag();
            ViewBag.DepartmentID = Data.DepartmentViewBag();
            return View();
        }
        public ActionResult _Search(FurloughSearchView model) {
            return ExecuteSearch(() =>
            {
                DateTime dnow = DateTime.Now;
                if (model.FromDate == null)
                {
                    model.FromDate = new DateTime(dnow.Year, dnow.Month, 01);
                }
                if (model.ToDate == null)
                {
                    model.ToDate = new DateTime(dnow.Year, dnow.Month, DateTime.DaysInMonth(dnow.Year, dnow.Month));
                }
                var furloughs = _context.FurloughModels.Where(it =>
                        it.FurloughDetailModels.Any(p=>p.DayOff>=model.FromDate && p.DayOff <= model.ToDate) &&
                        (model.LeaveCategoryId == null || it.LeaveCategoryId == model.LeaveCategoryId) &&
                        (model.DepartmentID == null || it.EmployeeModel.DepartmentID == model.DepartmentID) &&
                        (model.FullName == null || it.EmployeeModel.FullName.Contains(model.FullName)) &&
                        (model.EmployeeCode == null || it.EmployeeModel.EmployeeCode.Contains(model.EmployeeCode))
                    ).ToList();
                return PartialView(furloughs);
            });
        }
        #endregion Index
        #region Edit
        [PortalAuthorization]
        public ActionResult Edit(Guid id)
        {
            var edit = _context.FurloughModels.FirstOrDefault(it => it.FurloughId == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.FurloughModel.ToLower()) });
            }
            var model = FurloughFunction.getDatTa(edit, CurrentUser.AccountId);
            ViewBag.TypeDate = FurloughFunction.ListLeaveType();
            ViewBag.LeaveCategoryId = Data.LeaveCategorieBag(model.LeaveCategoryId);
            TrangThaiDuyet();
            return View(model);
        }
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Edit(FurloughViewModel model)
        {
            return ExecuteContainer(() =>
            {
                var edit = _context.FurloughModels.FirstOrDefault(it => it.FurloughId == model.FurloughId);

                List<string> errorList = new List<string>();
                if (edit != null)
                {
                    FurloughFunction.UpData(model, edit, errorList, true,new Guid(CurrentUser.AccountId),"nhansu");

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
                _repository.SaveUpdateHistory(edit.FurloughId, CurrentUser.UserName, edit);
                _context.Entry(edit).State = EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.FurloughModel.ToLower())
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
