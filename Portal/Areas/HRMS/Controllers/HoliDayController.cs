using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.EntityModels;
using Portal.Resources;
using System.Data.Entity;
using Portal.Repositories;

namespace HRMS.Controllers
{
    public class HoliDayController : BaseController
    {
        // GET: HoliDay
        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            HoliDayModel model = new HoliDayModel();
            model.Date1 = DateTime.Now;
            return View(model);
        }
        public ActionResult _Search(HoliDayModel model)
        {
            return ExecuteSearch(() =>
            {
                if (model.Date1.Date == new DateTime(0001, 01, 01))
                    model.Date1 = DateTime.Now.Date;
                var data = _context.HoliDayModels.Where(it => it.Date1.Year == model.Date1.Year &&
                            (model.Actived == null || it.Actived == model.Actived)).ToList();
                return PartialView(data);
            });
        }
        #endregion Index
        #region Create 
        [PortalAuthorization]
        [HttpGet]
        public ActionResult Create() {
            return View();
        }
        [PortalAuthorization]
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult Create(HoliDayModel add)
        {
            return ExecuteContainer(() =>
            {
                add.HolydayId = Guid.NewGuid();
                add.CreatedAccountId = new Guid(CurrentUser.AccountId);
                add.CreatedTime = DateTime.Now;
                _context.Entry(add).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.HoliDay.ToLower())
                });
            });
        }
        #endregion Create
        #region Edit
        [PortalAuthorization]
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var edit = _context.HoliDayModels.FirstOrDefault(it => it.HolydayId == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new
                {
                    area = "",
                    statusCode = 404,
                    exception =
                    string.Format(LanguageResource.Error_NotExist, LanguageResource.HoliDay.ToLower())
                });
            }
            return View(edit);
        }
        [PortalAuthorization]
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(HoliDayModel model)
        {
            return ExecuteContainer(() =>
            {
                var edit = _context.HoliDayModels.FirstOrDefault(it => it.HolydayId == model.HolydayId);
                if (edit != null)
                {
                    edit.Date1 = model.Date1;
                    edit.Name = model.Name;
                    edit.Actived = model.Actived;
                    HistoryRepository _repository = new HistoryRepository(_context);
                    _repository.SaveUpdateHistory(edit.HolydayId, CurrentUser.UserName, edit);
                    edit.LastModifiedAccountId = new Guid(CurrentUser.AccountId);
                    edit.LastModifiedTime = DateTime.Now;
                    _context.Entry(edit).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.HoliDay.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.UpgradeRequired,
                        Success = false,
                        Data = string.Format(LanguageResource.Error_NotExist, LanguageResource.HoliDay.ToLower())
                    });
                }
            });
        }
        #endregion Edit
        #region Del
        [PortalAuthorization]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var del = _context.HoliDayModels.FirstOrDefault(it => it.HolydayId == id);
                if (del != null)
                {
                    _context.Entry(del).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.HoliDay.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.HoliDay.ToLower())
                    });
                }
            });
        }
        #endregion Del
    }
}