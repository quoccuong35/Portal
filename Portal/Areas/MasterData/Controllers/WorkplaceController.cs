using Portal.Extensions;
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
    public class WorkplaceController : BaseController
    {
        //Nơi làm việc
        #region Index
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(WorkPlace searchViewModel) {
            return ExecuteSearch(() => {
                var list = (from noilamviec in _context.WorkPlaces
                            where
                            (searchViewModel.WorkPlaceName == null || noilamviec.WorkPlaceName.Contains(searchViewModel.WorkPlaceName))
                            && (searchViewModel.Actived == null || noilamviec.Actived == searchViewModel.Actived)
                            select noilamviec
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
        public JsonResult Create(WorkPlace model) {
            return ExecuteContainer(() =>
            {
                model.WorkPlaceID = Guid.NewGuid();
                model.CreatedAccountId = new Guid(CurrentUser.AccountId);
                model.CreatedTime = DateTime.Now;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.Workplace.ToLower())
                });
            });
        }
        #endregion Create

        #region Edit
        [PortalAuthorization]
        public ActionResult Edit(Guid id) {
            var edit = _context.WorkPlaces.FirstOrDefault(it => it.WorkPlaceID == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Workplace.ToLower()) });
            }
            return View(edit);
        }
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Edit(WorkPlace model) {
            return ExecuteContainer(() =>
            {
                var edit = _context.WorkPlaces.FirstOrDefault(it => it.WorkPlaceID == model.WorkPlaceID);
                if (edit != null)
                {
                    edit.WorkPlaceName = model.WorkPlaceName;
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
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Workplace.ToLower())
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
                var page = _context.WorkPlaces.FirstOrDefault(p => p.WorkPlaceID == id);
                if (page != null)
                {
                    _context.Entry(page).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Workplace.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.Workplace.ToLower())
                    });
                }
            });
        }
        #endregion Delete
    }
}