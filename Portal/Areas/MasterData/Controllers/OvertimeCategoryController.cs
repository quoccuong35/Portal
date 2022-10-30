using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.EntityModels;
using Portal.Resources;
using System.Data.Entity;

namespace MasterData.Controllers
{
    public class OvertimeCategoryController : BaseController
    {
        // GET: OvertimeCategory
        // Quản lý loại tăng ca
        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(OvertimeCategory search) {
            return ExecuteSearch(() => {
                var data = _context.OvertimeCategories.Where(it => (search.OvertimeCategoryName == null || it.OvertimeCategoryName.Contains(search.OvertimeCategoryName))
                                && (search.Actived == null || it.Actived == search.Actived)
                            ).ToList();
                return PartialView(data);
            });
        }
        #endregion Index
        #region Create
        [PortalAuthorization]
        public ActionResult Create() {
            return View();
        }

        [PortalAuthorization]
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult Create(OvertimeCategory add)
        {
            return ExecuteContainer(()=>{
                add.OvertimeCategoryId = Guid.NewGuid();
                add.CreatedTime = DateTime.Now;
                add.CreatedAccountId = new Guid(CurrentUser.AccountId);
                _context.Entry(add).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.OvertimeCategory.ToLower())
                });
            });
        }
        #endregion Create

        #region Edit 
        [PortalAuthorization]
        public ActionResult Edit(Guid id) {
            var edit = _context.OvertimeCategories.FirstOrDefault(it => it.OvertimeCategoryId == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Bank.ToLower()) });
            }
            return View(edit);
        }
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Edit(OvertimeCategory model)
        {
            return ExecuteContainer(() =>
            {
                var edit = _context.OvertimeCategories.FirstOrDefault(it => it.OvertimeCategoryId == model.OvertimeCategoryId);
                if (edit != null)
                {
                    edit.OvertimeCategoryName = model.OvertimeCategoryName;
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
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.OvertimeCategory.ToLower())
                });
            });
        }
        #endregion Edit
        #region Delete
        [HttpPost]
        [PortalAuthorizationAttribute]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var bank = _context.OvertimeCategories.FirstOrDefault(p => p.OvertimeCategoryId == id);
                if (bank != null)
                {
                    _context.Entry(bank).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.OvertimeCategory.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.OvertimeCategory.ToLower())
                    });
                }
            });
        }
        #endregion Delete
    }
}