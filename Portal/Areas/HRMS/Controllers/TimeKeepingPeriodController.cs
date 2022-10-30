using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Resources;
using Portal.EntityModels;
using System.Data.Entity;
using Portal.Repositories;

namespace HRMS.Controllers
{
    public class TimeKeepingPeriodController : BaseController
    {
        // GET: TimeKeepingPeriod
        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            TypeViewBag();
            return View();
        }
        public ActionResult _Search(TimeKeepingPeriodModel model)
        {
            return ExecuteSearch(() =>
            {
                if (model.FromDate.Date == new DateTime(0001, 01, 01))
                    model.FromDate = new DateTime(DateTime.Now.Year, 01, 01);
                var data = _context.TimeKeepingPeriodModels.Where(it => it.FromDate <= model.FromDate && it.ToDate >= model.FromDate
                            && (model.Actived == null || it.Actived == model.Actived)
                            && model.Type == null || it.Type == model.Type).ToList();
                return PartialView(data);
            });
        }
        #endregion Index
        #region Create
        [PortalAuthorization]
        [HttpGet]
        public ActionResult Create() {
            TypeViewBag();
            return View();
        }
        [PortalAuthorization]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult Create(TimeKeepingPeriodModel model) {
            return ExecuteContainer(() =>
            {
                model.ID = Guid.NewGuid();
                model.CreatedAccountId = new Guid(CurrentUser.AccountId);
                model.CreatedTime = DateTime.Now;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.TimeKeepingPeriod.ToLower())
                });
            });
        }
        #endregion Create
        #region Edit
        [PortalAuthorization]
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var edit = _context.TimeKeepingPeriodModels.FirstOrDefault(it => it.ID == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = 
                    string.Format(LanguageResource.Error_NotExist, LanguageResource.TimeKeepingPeriod.ToLower()) });
            }
            TypeViewBag();
            return View(edit);
        }
        [PortalAuthorization]
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(TimeKeepingPeriodModel model)
        {
            return ExecuteContainer(() =>
            {
                var edit = _context.TimeKeepingPeriodModels.FirstOrDefault(it => it.ID == model.ID);
                if (edit != null)
                {
                    edit.FromDate = model.FromDate;
                    edit.ToDate = model.ToDate;
                    edit.Type = model.Type;
                    edit.Descriptions = model.Descriptions;
                    edit.Actived = model.Actived;
                    HistoryRepository _repository = new HistoryRepository(_context);
                    _repository.SaveUpdateHistory(edit.ID, CurrentUser.UserName, edit);
                    edit.LastModifiedAccountId = new Guid(CurrentUser.AccountId);
                    edit.LastModifiedTime = DateTime.Now;
                    _context.Entry(edit).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.TimeKeepingPeriod.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.UpgradeRequired,
                        Success = false,
                        Data = string.Format(LanguageResource.Error_NotExist, LanguageResource.TimeKeepingPeriod.ToLower())
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
                var del = _context.TimeKeepingPeriodModels.FirstOrDefault(it => it.ID == id);
                if (del != null)
                {
                    _context.Entry(del).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.TimeKeepingPeriod.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.TimeKeepingPeriod.ToLower())
                    });
                }
            });
        }
        #endregion Del
        #region Helper
        private void TypeViewBag(string type = null) {
            List<TimeKeepingPeriodType> list = new List<TimeKeepingPeriodType>();
            TimeKeepingPeriodType ad1 = new TimeKeepingPeriodType();
            ad1.Type = "1";
            ad1.Name = "Kỳ công người dùng";
            TimeKeepingPeriodType ad2 = new TimeKeepingPeriodType();
            ad2.Type = "2";
            ad2.Name = "Kỳ tính công";
            list.Add(ad1);
            list.Add(ad2);
            ViewBag.Type = new SelectList(list, "Type", "Name", type);
        }
        #endregion Helper
    }
    public class TimeKeepingPeriodType
    {
        public string Type { get; set; }
        public string Name{ get; set;}
    }
}