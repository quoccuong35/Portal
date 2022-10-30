using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Extensions;
using Portal.Resources;
using Portal.EntityModels;
using System.Data.Entity;

namespace MasterData.Controllers
{
    public class NationalityController : BaseController
    {
        // GET: Nationality // Quốc tịch

        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(Nationality searchModel) {
            var list = _context.Nationalities.Where(it => (searchModel.NationalityName == null || searchModel.NationalityName.Contains(it.NationalityName))
                        && (searchModel.Actived == null || it.Actived == searchModel.Actived)).ToList();
            return PartialView(list);

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
        public JsonResult Create(Nationality model) {
            return ExecuteContainer(() =>
            {
                model.NationalityId = Guid.NewGuid();
                model.CreatedAccountId = new Guid(CurrentUser.AccountId);
                model.CreatedTime = DateTime.Now;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.Nationality.ToLower())
                });
            });
        }
        #endregion Create

        #region Edit
        public ActionResult Edit(Guid id) {
            var edit = _context.Nationalities.FirstOrDefault(it => it.NationalityId == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Nationality.ToLower()) });
            }
            return View(edit);
        }
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Edit(Nationality model)
        {
            return ExecuteContainer(() =>
            {
                var edit = _context.Nationalities.FirstOrDefault(it => it.NationalityId == model.NationalityId);
                if (edit != null)
                {
                    edit.NationalityCode = model.NationalityCode;
                    edit.NationalityName = model.NationalityName;
                    edit.LastModifiedAccountId = new Guid(CurrentUser.AccountId);
                    edit.LastModifiedTime = DateTime.Now;
                    edit.Actived = model.Actived;
                    edit.Description = model.Description;
                    edit.OrderIndex = model.OrderIndex;
                    _context.Entry(edit).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Nationality.ToLower())
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
                var page = _context.Nationalities.FirstOrDefault(p => p.NationalityId == id);
                if (page != null)
                {
                    _context.Entry(page).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Nationality.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.Nationality.ToLower())
                    });
                }
            });
        }
        #endregion Delete
    }
}