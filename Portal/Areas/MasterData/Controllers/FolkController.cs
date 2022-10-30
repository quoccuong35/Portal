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
    public class FolkController : BaseController
    {
        // GET: Folk Dân tộc
        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(Folk searchViewModel)
        {
            return ExecuteSearch(() => {
                var list = (from dantoc in _context.Folks
                            where
                            (searchViewModel.FolkName == null || dantoc.FolkName.Contains(searchViewModel.FolkName))
                            && (searchViewModel.Actived == null || dantoc.Actived == searchViewModel.Actived)
                            select dantoc
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
        public JsonResult Create(Folk model) {
            return ExecuteContainer(() =>
            {
                model.FolkId = Guid.NewGuid();
                model.CreatedAccountId = new Guid(CurrentUser.AccountId);
                model.CreatedTime = DateTime.Now;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.FolkId.ToLower())
                });
            });
        }
        #endregion Create

        #region Edit 
        [PortalAuthorization]
        public ActionResult Edit(Guid id) {
            var edit = _context.Folks.FirstOrDefault(it => it.FolkId == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.FolkId.ToLower()) });
            }
            return View(edit);
        }
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Edit(Folk model)
        {
            return ExecuteContainer(() =>
            {
                var edit = _context.Folks.FirstOrDefault(it => it.FolkId == model.FolkId);
                if (edit != null)
                {
                    edit.FolKCode = model.FolKCode;
                    edit.FolkName = model.FolkName;
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
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.FolkId.ToLower())
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
                var page = _context.Folks.FirstOrDefault(p => p.FolkId == id);
                if (page != null)
                {
                    _context.Entry(page).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.FolkId.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.FolkId.ToLower())
                    });
                }
            });
        }
        #endregion Delete
    }
}