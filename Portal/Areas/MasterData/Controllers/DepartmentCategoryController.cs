using Portal.EntityModels;
using Portal.Extensions;
using Portal.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MasterData.Models;

namespace MasterData.Controllers
{
    [Log]
    public class DepartmentCategoryController : BaseController
    {
        // GET: DepartmentCategory Khối
        #region index
        [PortalAuthorization]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(DepartmentCategory searchViewModel)
        {
            return ExecuteSearch(() =>
            {
                var CategoryList = (from p in _context.DepartmentCategories
                                    orderby p.OrderIndex.HasValue descending, p.OrderIndex
                                    where
                                    //search by ReportCategoryName
                                    (searchViewModel.DepartmentCategoryName == null || p.DepartmentCategoryName.Contains(searchViewModel.DepartmentCategoryName))
                                    //search by Actived
                                    && (searchViewModel.Actived == null || p.Actived == searchViewModel.Actived)
                                    select p).ToList();

                return PartialView(CategoryList);
            });
        }
        #endregion
        //GET: /DocumentionCategory2/Create
        #region Create
        [PortalAuthorization]
        public ActionResult Create()
        {
            return View();
        }
        //POST: Create
        [HttpPost]
        [ValidateAjax]
        [PortalAuthorizationAttribute]
        public JsonResult Create(DepartmentCategory model, HttpPostedFileBase ImageUrl)
        {
            return ExecuteContainer(() =>
            {
                model.DepartmentCategoryID = Guid.NewGuid();
                //if (ImageUrl != null)
                //{
                //    model.ImageUrl = Upload(ImageUrl, "DocumentationCategory");
                //}
                model.CreatedUser = CurrentUser.UserName;
                model.CreatedTime = DateTime.Now;

                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.DepartmentCategory.ToLower())
                });
            });
        }
        #endregion Create
        #region Edit
        [PortalAuthorization]
        public ActionResult Edit(Guid id) {
            var model = _context.DepartmentCategories.FirstOrDefault(it => it.DepartmentCategoryID == id);
            if (model == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.DepartmentCategory.ToLower()) });
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Edit(DepartmentCategory model) {
            return ExecuteContainer(() =>
            {
                var edit = _context.DepartmentCategories.FirstOrDefault(it => it.DepartmentCategoryID == model.DepartmentCategoryID);
                if (edit != null) {
                    edit.LastModifiedUser = CurrentUser.UserName;
                    edit.LastModifiedTime = DateTime.Now;
                    edit.DepartmentCategoryCode = model.DepartmentCategoryCode;
                    edit.DepartmentCategoryName = model.DepartmentCategoryName;
                    edit.Actived = model.Actived;
                    edit.OrderIndex = model.OrderIndex;
                    edit.Description = model.Description;
                    _context.Entry(edit).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.DepartmentCategory.ToLower())
                });
            });
        }
        #endregion Edit
        #region Delete
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public ActionResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var account = _context.DepartmentCategories.FirstOrDefault(p => p.DepartmentCategoryID == id);
                if (account != null)
                {
                    ////Account in roles
                    //if (account.RolesModel != null && account.RolesModel.Count > 0)
                    //{
                    //    account.RolesModel.Clear();
                    //}
                    _context.Entry(account).State = EntityState.Deleted;
                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.DepartmentCategory.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.DepartmentCategory.ToLower())
                    });
                }
            });
        }
        #endregion Delete
    }
}