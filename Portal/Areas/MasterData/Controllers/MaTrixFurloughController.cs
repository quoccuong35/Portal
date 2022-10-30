using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.EntityModels;
using System.Data.Entity;
using Portal.Resources;
using Portal.Repositories;

namespace MasterData.Controllers
{
    public class MaTrixFurloughController : BaseController
    {
        // GET: MaTrixFurlough Matrix duyệt nghỉ phép

        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(MaTrixFurloughModel search)
        {
            //var model = _context.MaTrixFurloughModels.Where(it =>
            //                it.DepartmentID == search.DepartmentID || search.DepartmentID == null
            //            ).ToList();
            var model = _context.MaTrixFurloughModels.ToList().OrderBy(it=>it.FromDay);
            return PartialView(model);
        }
        #endregion Index

        #region Create
        [PortalAuthorization]
        public ActionResult Create() {
            ViewBag.DepartmentID = Data.DepartmentViewBag();
            return View();
        }

        [PortalAuthorization]
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult Create(MaTrixFurloughModel model) {
            return ExecuteContainer(() => {
                model.MaTrixFurloughID = Guid.NewGuid();
                model.CreatedAccountId = new Guid(CurrentUser.AccountId);
                model.CreatedTime = DateTime.Now;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.MaTrixFurlough.ToLower())
                });
            });
        }
        #endregion Create

        #region Edit
        [PortalAuthorization]
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var edit = _context.MaTrixFurloughModels.FirstOrDefault(it => it.MaTrixFurloughID == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.MaTrixFurlough.ToLower()) });
            }
            ViewBag.DepartmentID = Data.DepartmentViewBag(edit.DepartmentID);
            return View(edit);
        }

        public JsonResult Edit(MaTrixFurloughModel model)
        {
            return ExecuteContainer(() => {
                var edit = _context.MaTrixFurloughModels.FirstOrDefault(it => it.MaTrixFurloughID == model.MaTrixFurloughID);
                if (edit == null)
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Error_NotExist, model.MaTrixFurloughName)
                    });
                }
                else
                {
                    edit.ApprovalName = model.ApprovalName;
                    edit.Note = model.Note;
                    edit.Actived = model.Actived;

                    HistoryRepository _repository = new HistoryRepository(_context);
                    _repository.SaveUpdateHistory(edit.MaTrixFurloughID, CurrentUser.UserName, edit);

                    edit.LastModifiedTime = DateTime.Now;
                    edit.LastModifiedAccountId = new Guid(CurrentUser.AccountId);

                    _context.Entry(edit).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.MaTrixFurlough.ToLower())
                    });

                }
            });
            
        }
        #endregion Edit
    }
}