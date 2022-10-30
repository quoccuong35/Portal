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
    public class LeaveCategoryController : BaseController
    {
        // loại nghỉ phép
        // GET: LeaveCategory
        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(LeaveCategory searchModel)
        {
            return ExecuteSearch(() => {
                var listData = _context.LeaveCategories.Where(it => (searchModel.LeaveCategoryName == null || it.LeaveCategoryName.Contains(searchModel.LeaveCategoryName))
                       && (searchModel.Actived == null || it.Actived == searchModel.Actived)).OrderBy(x=>x.OrderIndex).ToList();

                return PartialView(listData);
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
        public JsonResult Create(LeaveCategory model) {
            return ExecuteContainer(() =>
            {
                model.LeaveCategoryId = Guid.NewGuid();
                model.CreatedAccountId = new Guid(CurrentUser.AccountId);
                model.CreatedTime = DateTime.Now;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.Leaved.ToLower())
                });
            });
        }
        #endregion Create

        #region Edit
        [PortalAuthorization]
        public ActionResult Edit(Guid id)
        {
            var edit = _context.LeaveCategories.FirstOrDefault(it => it.LeaveCategoryId == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Leaved.ToLower()) });
            }
            return View(edit);
        }

        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Edit(LeaveCategory model)
        {
            return ExecuteContainer(() =>
            {
                var edit = _context.LeaveCategories.FirstOrDefault(it => it.LeaveCategoryId == model.LeaveCategoryId);
                if (edit != null)
                {
                    edit.LeaveCategoryCode = model.LeaveCategoryCode;
                    edit.LeaveCategoryName = model.LeaveCategoryName;
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
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Leaved.ToLower())
                });
            });
        }
        #endregion Edit

    }
}