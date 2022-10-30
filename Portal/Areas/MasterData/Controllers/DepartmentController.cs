using Portal.Extensions;
using Portal.ViewModels;
using Portal.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Portal.Resources;

namespace MasterData.Controllers
{
    public class DepartmentController : BaseController
    {
        // GET: Department Đơn vị
        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public ActionResult _Search(DepartmentViewModel searchViewModel) {
            return ExecuteSearch(()=> {
                var listDeparment = (from dv in _context.Departments
                                     join khoi in _context.DepartmentCategories on dv.DepartmentCategoryID equals khoi.DepartmentCategoryID into cg
                                     from c in cg.DefaultIfEmpty()
                                     orderby dv.OrderIndex, dv.CreatedTime descending
                                     where
                                     // tìm khối
                                     (searchViewModel.DepartmentCategoryID == null || dv.DepartmentCategoryID == searchViewModel.DepartmentCategoryID)
                                     && (searchViewModel.DepartmentName == null || dv.DepartmentName == searchViewModel.DepartmentName)
                                     && (searchViewModel.Actived == null || dv.Actived == searchViewModel.Actived)
                                     select new DepartmentViewModel
                                     {
                                         DepartmentID = dv.DepartmentID,
                                         DepartmentCategoryID = c.DepartmentCategoryID,
                                         DepartmentCategoryName = c.DepartmentCategoryName,
                                         DepartmentCode = dv.DepartmentCode,
                                         DepartmentName = dv.DepartmentName,
                                         OrderIndex = dv.OrderIndex,
                                         Actived = dv.Actived
                                     }
                                     ).ToList();
                return PartialView(listDeparment);
            });
        }
        #endregion Index

        #region Create
        [PortalAuthorization]
        public ActionResult Create() {
            CreateViewBag();
            return View();
        }
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Create(Department model) {
            return ExecuteContainer(() =>
            {
                model.DepartmentID = Guid.NewGuid();
                model.CreatedAccountId = new Guid(CurrentUser.AccountId);
                model.CreatedTime = DateTime.Now;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.Department.ToLower())
                });
            });
        }
        #endregion Create

        #region Edit
        [PortalAuthorization]
        public ActionResult Edit(Guid id) {
            var edit = (from p in _context.Departments
                        where p.DepartmentID == id
                        select new DepartmentViewModel() {
                            DepartmentID = p.DepartmentID,
                            DepartmentCode = p.DepartmentCode,
                            DepartmentName = p.DepartmentName,
                            Actived = p.Actived,
                            DepartmentCategoryID = p.DepartmentCategoryID,
                            Description = p.Description,
                            CreatedAccountId = p.CreatedAccountId,
                            CreatedTime = p.CreatedTime,
                            LastModifiedAccountId = p.LastModifiedAccountId,
                            LastModifiedTime = p.LastModifiedTime,
                            OrderIndex = p.OrderIndex
                    
                        }).FirstOrDefault();
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Department.ToLower()) });
            }
            CreateViewBag(edit.DepartmentCategoryID);
            return View(edit);
        }
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Edit(Department model)
        {
            return ExecuteContainer(() => {
                var edit = _context.Departments.FirstOrDefault(it => it.DepartmentID == model.DepartmentID);
                if (edit != null)
                {
                    edit.Actived = model.Actived;
                    edit.OrderIndex = model.OrderIndex;
                    edit.Description = model.Description;
                    edit.DepartmentCode = model.DepartmentCode;
                    edit.DepartmentName = model.DepartmentName;
                    edit.DepartmentCategoryID = model.DepartmentCategoryID;
                    edit.LastModifiedAccountId = new Guid(CurrentUser.AccountId);
                    edit.LastModifiedTime = DateTime.Now;
                    _context.Entry(edit).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Department.ToLower())
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
                var del = _context.Departments.FirstOrDefault(p => p.DepartmentID == id);
                if (del != null)
                {
                    _context.Entry(del).State = EntityState.Deleted;
                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Department.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.Department.ToLower())
                    });
                }
            });
        }
        #endregion Delete

        #region CreateViewBag
        public void CreateViewBag(Guid? DepartmentCategoryID = null)
        {
            var departmentCategoriesList = _context.DepartmentCategories.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).ToList();
            ViewBag.DepartmentCategoryID = new SelectList(departmentCategoriesList, "DepartmentCategoryID", "DepartmentCategoryName", DepartmentCategoryID);
        }
        #endregion CreateViewBag
    }
}