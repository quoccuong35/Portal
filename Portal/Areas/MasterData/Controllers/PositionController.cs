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
    public class PositionController : BaseController
    {
        // GET: Position Chức vụ
        #region Index
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Search(Position searchViewModel) {
            return ExecuteSearch(() => {
                var list = (from chucvu in _context.Positions
                           where 
                           (searchViewModel.PositionName == null || chucvu.PositionName.Contains(searchViewModel.PositionName))
                           && (searchViewModel.Actived == null || chucvu.Actived == searchViewModel.Actived)
                           select chucvu
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
        public JsonResult Create(Position model) {
            return ExecuteContainer(() =>
            {
                model.PositionID = Guid.NewGuid();
                model.CreatedAccountId = new Guid(CurrentUser.AccountId);
                model.CreatedTime = DateTime.Now;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.Position.ToLower())
                });
            });
        }
        #endregion Create

        #region Edit
        [PortalAuthorization]
        public ActionResult Edit(Guid id) {
            var edit = _context.Positions.FirstOrDefault(it => it.PositionID == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Position.ToLower()) });
            }
            return View(edit);
        }
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Edit(Position model) {
            return ExecuteContainer(() =>
            {
                var edit = _context.Positions.FirstOrDefault(it => it.PositionID == model.PositionID);
                if (edit != null) {
                    edit.PositionCode = model.PositionCode;
                    edit.PositionName = model.PositionName;
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
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Position.ToLower())
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
                var page = _context.Positions.FirstOrDefault(p => p.PositionID == id);
                if (page != null)
                {
                    _context.Entry(page).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Position.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.Position.ToLower())
                    });
                }
            });
        }
        #endregion Delete
    }
}