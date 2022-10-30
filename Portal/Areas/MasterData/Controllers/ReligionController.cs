using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Extensions;
using Portal.EntityModels;
using System.Data.Entity;
using Portal.Resources;

namespace MasterData.Controllers
{
    public class ReligionController : BaseController
    {
        // GET: Religion Tôn giáo
        #region Index
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(Religion searchViewModel)
        {
            return ExecuteSearch(() => {
                var list = (from tongiao in _context.Religions
                            where
                            (searchViewModel.ReligionName == null || tongiao.ReligionName.Contains(searchViewModel.ReligionName))
                            && (searchViewModel.Actived == null || tongiao.Actived == searchViewModel.Actived)
                            select tongiao
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
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PortalAuthorization]
        public JsonResult Create(Religion model) {
            return ExecuteContainer(() =>
            {
                model.ReligionId = Guid.NewGuid();
                model.CreatedAccountId = new Guid(CurrentUser.AccountId);
                model.CreatedTime = DateTime.Now;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.ReligionId.ToLower())
                });
            });
        }
        #endregion Create

        #region Edit
        [PortalAuthorization]
        public ActionResult Edit(Guid id)
        {
            var edit = _context.Religions.FirstOrDefault(it => it.ReligionId == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.ReligionId.ToLower()) });
            }
            return View(edit);
        }
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PortalAuthorization]
        public JsonResult Edit(Religion model)
        {
            return ExecuteContainer(() =>
            {
                var edit = _context.Religions.FirstOrDefault(it => it.ReligionId == model.ReligionId);
                if (edit != null)
                {
                    edit.ReligionCode = model.ReligionCode;
                    edit.ReligionName = model.ReligionName;
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
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.ReligionId.ToLower())
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
                var page = _context.Religions.FirstOrDefault(p => p.ReligionId == id);
                if (page != null)
                {
                    _context.Entry(page).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.ReligionId.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.ReligionId.ToLower())
                    });
                }
            });
        }
        #endregion Delete
    }
}