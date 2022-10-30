using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Extensions;
using Portal.EntityModels;
using Portal.Resources;
using System.Data.Entity;

namespace MasterData.Controllers
{
    public class EducationController : BaseController
    {
        // GET: Education Trình độ văn hóa
        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(Education searchViewModel) {
            return ExecuteSearch(() => {
                var list = (from trinhdo in _context.Educations
                            where
                            (searchViewModel.EducationName == null || trinhdo.EducationName.Contains(searchViewModel.EducationName))
                            && (searchViewModel.Actived == null || trinhdo.Actived == searchViewModel.Actived)
                            select trinhdo
                           ).ToList();
                return PartialView(list);
            });
        }
        #endregion Index

        #region Create
        [PortalAuthorization]
        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Create(Education model) {
            return ExecuteContainer(() =>
            {
                model.EducationId = Guid.NewGuid();
                model.CreatedAccountId = new Guid(CurrentUser.AccountId);
                model.CreatedTime = DateTime.Now;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.Report.ToLower())
                });
            });
        }
        #endregion Create

        #region Edit
        [PortalAuthorization]
        public ActionResult Edit(Guid id) {
            var edit = _context.Educations.FirstOrDefault(it => it.EducationId == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Education.ToLower()) });
            }
            return View(edit);
        }
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Edit(Education model) {
            return ExecuteContainer(() =>
            {
                var edit = _context.Educations.FirstOrDefault(it => it.EducationId == model.EducationId);
                if (edit != null)
                {
                    edit.EducationName = model.EducationName;
                    edit.LastModifiedAccountId = new Guid(CurrentUser.AccountId);
                    edit.LastModifiedTime = DateTime.Now;
                    edit.Actived = model.Actived;
                    edit.Description = model.Description;
                    _context.Entry(edit).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Education.ToLower())
                });
            });
        }
        #endregion Edit

        #region Delete
        [HttpPost]
        [PortalAuthorizationAttribute]
        public ActionResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var page = _context.Educations.FirstOrDefault(p => p.EducationId == id);
                if (page != null)
                {
                    _context.Entry(page).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Education.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.Education.ToLower())
                    });
                }
            });
        }
        #endregion Delete
    }
}