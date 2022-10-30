using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Extensions;
using Portal.EntityModels;
using System.Data.Entity;
using Portal.Resources;
using Portal.Repositories;
using MasterData.Models;

namespace MasterData.Controllers
{

    [LogAttribute]
    public class ShiftWorkController : BaseController
    {
        // GET: ShiftWork Ca làm việc
        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(ShiftWork search)
        {
            return ExecuteSearch(() => {
                var listShiftWorks = (from work in _context.ShiftWorks
                                      where (search.ShiftWorkName == null || work.ShiftWorkName.Contains(search.ShiftWorkName))
                                      && (search.Actived == null || work.Actived == search.Actived)
                                       select work).ToList();
                return PartialView(listShiftWorks);
            });
        }
        #endregion Index
        #region Create 
        [PortalAuthorization]
        public ActionResult Create() {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateAjax]
        [PortalAuthorization]
        public JsonResult Create(ShiftWork model) {
            return ExecuteContainer(() =>
            {
                model.ShiftWorkId = Guid.NewGuid();
                model.CreatedAccountId = new Guid(CurrentUser.AccountId);
                model.CreatedTime = DateTime.Now;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.ShiftWork.ToLower())
                });
            });
        }
        #endregion Create
        #region Edit
        [PortalAuthorization]
        public ActionResult Edit(Guid id) {
            var edit = (from p in _context.ShiftWorks
                        where p.ShiftWorkId == id
                        select p).FirstOrDefault();
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.ShiftWork.ToLower()) });
            }
            return View(edit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateAjax]
        [PortalAuthorization]
        public JsonResult Edit(ShiftWork model) {
            return ExecuteContainer(() =>
            {
                var edit = _context.ShiftWorks.FirstOrDefault(it => it.ShiftWorkId == model.ShiftWorkId);
                if (edit != null)
                {
                    edit.StartTime = model.StartTime;
                    edit.EndTimeBetweenShift = model.EndTimeBetweenShift;
                    edit.StartTimeBetweenShift = model.StartTimeBetweenShift;
                    edit.OvertimeStartTime = model.OvertimeStartTime;
                    edit.TotalWorkTime = model.TotalWorkTime;
                    edit.NumberMinuteLate = model.NumberMinuteLate;
                    edit.EndTime = model.EndTime;
                    edit.Actived = model.Actived;
                    edit.OrderIndex = model.OrderIndex;
                    edit.Description = model.Description;
                    edit.ShiftWorkCode = model.ShiftWorkCode;
                    edit.ShiftWorkName = model.ShiftWorkName;
                    edit.StandardHour = model.StandardHour;
                    edit.IsNightShift = model.IsNightShift;
                    edit.LastModifiedAccountId = new Guid(CurrentUser.AccountId);
                    edit.LastModifiedTime = DateTime.Now;

                    HistoryRepository _repository = new HistoryRepository(_context);
                    _repository.SaveUpdateHistory(edit.ShiftWorkId, CurrentUser.UserName, edit);
                    _context.Entry(edit).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Permission_AccountModel.ToLower())
                });
            });
        }
        #endregion Edit
        #region Delete
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Delete(Guid id) {
            return ExecuteDelete(() =>
            {
                var del = _context.ShiftWorks.FirstOrDefault(p => p.ShiftWorkId == id);
                if (del != null)
                {
                    _context.Entry(del).State = EntityState.Deleted;
                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.ShiftWork.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.ShiftWork.ToLower())
                    });
                }
            });
        }
        #endregion Delete
    }
}