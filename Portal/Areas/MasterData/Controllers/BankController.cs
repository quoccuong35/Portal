using Portal.EntityModels;
using Portal.Extensions;
using Portal.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterData.Controllers
{
    public class BankController : BaseController
    {
        // GET: Bank Ngân hàng
        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(Bank searchModel) {
            return ExecuteSearch(() => {
                var listData = _context.Banks.Where(it => (searchModel.BankName == null || it.BankName.Contains(searchModel.BankName))
                       && (searchModel.Actived == null || it.Actived == searchModel.Actived)).OrderBy(x => x.OrderIndex).ToList();
                return PartialView(listData);
            });
        }
        #endregion
        #region Create 
        [PortalAuthorization]
        public ActionResult Create() {
            return View();
        }
        [PortalAuthorization]
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Bank model) {
            return ExecuteContainer(() => {
                model.BankId = Guid.NewGuid();
                model.CreatedAccountId = new Guid(CurrentUser.AccountId);
                model.CreatedTime = DateTime.Now;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.Bank.ToLower())
                });
            });
        }
        #endregion
        #region Edit
        [PortalAuthorization]
        public ActionResult Edit(Guid id)
        {
            var edit = _context.Banks.FirstOrDefault(it => it.BankId == id);
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
        public JsonResult Edit(Bank model)
        {
            return ExecuteContainer(() =>
            {
                var edit = _context.Banks.FirstOrDefault(it => it.BankId == model.BankId);
                if (edit != null)
                {
                    edit.BankCode = model.BankCode;
                    edit.BankName = model.BankName;
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
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Bank.ToLower())
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
                var bank = _context.Banks.FirstOrDefault(p => p.BankId == id);
                if (bank != null)
                {
                    _context.Entry(bank).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Bank.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.Bank.ToLower())
                    });
                }
            });
        }
        #endregion Delete
    }
}