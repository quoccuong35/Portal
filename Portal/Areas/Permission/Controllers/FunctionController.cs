using Portal.EntityModels;
using Portal.Extensions;
using Portal.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Permission.Controllers
{
    public class FunctionController : BaseController
    {
        //GET: /Function/Index
        #region Index, _Search
        [PortalAuthorizationAttribute]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(string FunctionName)
        {
            return ExecuteSearch(() =>
            {
                var FunctionNameIsNullOrEmpty = string.IsNullOrEmpty(FunctionName);
                var funcList = _context.FunctionModels.Where(p => FunctionNameIsNullOrEmpty || p.FunctionName.ToLower().Contains(FunctionName.ToLower()))
                                                  .ToList();
                return PartialView(funcList);
            });
        }
        #endregion

        //GET: /Function/Create
        #region Create
       // [ISDAuthorizationAttribute]
        public ActionResult Create()
        {
            return View();
        }
        //POST: Create
        [HttpPost]
        [ValidateAjax]
        //[ISDAuthorizationAttribute]
        public JsonResult Create(FunctionModel model)
        {
            return ExecuteContainer(() =>
            {
                model.FunctionId = model.FunctionId.ToUpper();
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.Permission_FunctionModel.ToLower())
                });
            });
        }
        #endregion

        //GET: /Function/Edit
        #region Edit
        [PortalAuthorizationAttribute]
        public ActionResult Edit(string id)
        {
            var func = _context.FunctionModels.FirstOrDefault(p => p.FunctionId == id);
            if (func == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Permission_FunctionModel.ToLower()) });
            }
            return View(func);
        }
        //POST: Edit
        [HttpPost]
        //[ISDAuthorizationAttribute]
        public JsonResult Edit(FunctionModel model)
        {
            return ExecuteContainer(() =>
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Permission_FunctionModel.ToLower())
                });
            });
        }
        #endregion

        //GET: /Function/Delete
        #region Delete
        [HttpPost]
        [PortalAuthorizationAttribute]
        public ActionResult Delete(string id)
        {
            return ExecuteDelete(() =>
            {
                var func = _context.FunctionModels.FirstOrDefault(p => p.FunctionId == id);
                if (func != null)
                {
                    if (func.PageModels != null)
                    {
                        func.PageModels.Clear();
                    }
                    _context.Entry(func).State = EntityState.Deleted;

                    //Delete in PagePermission
                    var pagePermission = _context.PagePermissionModels.Where(p => p.FunctionId == id).ToList();
                    if (pagePermission != null && pagePermission.Count > 0)
                    {
                        _context.PagePermissionModels.RemoveRange(pagePermission);
                    }
                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Permission_FunctionModel.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = ""
                    });
                }
            });
        }
        #endregion 

        //GET: /Function/ExportCreate
        public ActionResult ExportCreate()
        {
            return RedirectToAction("Index");
        }

        public ActionResult ImportCreate()
        {

            return RedirectToAction("Index");
        }
        #region Remote Validation
        private bool IsExists(string FunctionId)
        {
            return (_context.FunctionModels.FirstOrDefault(p => p.FunctionId == FunctionId) != null);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckExistingFunctionId(string FunctionId, string FunctionIdValid)
        {
            try
            {
                if (!string.IsNullOrEmpty(FunctionIdValid) && !string.IsNullOrEmpty(FunctionId) && FunctionIdValid.ToLower() != FunctionId.ToLower())
                {
                    return Json(!IsExists(FunctionId));
                }
                else
                {
                    return Json(true);
                }
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        #endregion
    }
}