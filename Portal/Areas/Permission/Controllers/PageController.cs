using Portal.Constant;
using Portal.EntityModels;
using Portal.Extensions;
using Portal.Resources;
using Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Permission.Controllers
{
    public class PageController : BaseController
    {
        //GET: /Page/Index
        #region Index, _Search
        [PortalAuthorizationAttribute]
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public ActionResult _Search(string PageName, bool? Actived, Guid? MenuId)
        {
            return ExecuteSearch(() =>
            {
                var PageNameIsNullOrEmpty = string.IsNullOrEmpty(PageName);
                var pageList = _context.PageModels.Where(p => (PageNameIsNullOrEmpty || p.PageName.ToLower().Contains(PageName.ToLower()))
                                                                && (Actived == null || p.Actived == Actived)
                                                                && (MenuId == null || p.MenuId == MenuId))
                                                  .OrderBy(p => p.MenuModel.OrderIndex)
                                                  .ThenBy(p => p.OrderIndex)
                                                  .ToList();
                return PartialView(pageList);
            });
        }
        #endregion

        //GET: /Page/Create
        #region Create
        [PortalAuthorizationAttribute]
        public ActionResult Create()
        {
            PageFunctionViewModel page = new PageFunctionViewModel()
            {
                Actived = true,
                isQuickAccess = false,
                FunctionList = funcWithOrderBy(),
                ActivedFunctionList = funcWithOrderBy().Where(p => p.FunctionId == ConstFunction.Access).ToList()
            };
            CreateViewBag();
            return View(page);
        }

        //POST: Create
        [HttpPost]
        [ValidateAjax]
        [PortalAuthorizationAttribute]
        public JsonResult Create(PageModel model, List<string> FunctionId)
        {
            return ExecuteContainer(() =>
            {
                //Save data in PageModel
                model.PageId = Guid.NewGuid();
                model.Actived = true;
                model.Visiable = true;
                model.isSystem = false;
                //Save data in PageFunctionModel
                if (FunctionId != null)
                {
                    ManyToMany(model, FunctionId);
                }
                //Save
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.Permission_PageModel.ToLower())
                });
            });
        }
        #endregion
        //GET: /Page/Edit
        #region Edit
        [PortalAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            var page = (from p in _context.PageModels.AsEnumerable()
                        where p.PageId == id
                        select new PageFunctionViewModel()
                        {
                            PageId = p.PageId,
                            PageName = p.PageName,
                            PageUrl = p.PageUrl,
                            MenuId = p.MenuId,
                            OrderIndex = p.OrderIndex,
                            Icon = p.Icon,
                            isQuickAccess = p.isQuickAccess,
                            Actived = p.Actived,
                            FunctionList = funcWithOrderBy(),
                            ActivedFunctionList = p.FunctionModels.ToList()
                        }).FirstOrDefault();
            CreateViewBag(page.MenuId);
            return View(page);
        }
        //POST: Edit
        [HttpPost]
        [PortalAuthorizationAttribute]
        public JsonResult Edit(PageModel model, List<string> FunctionId)
        {
            return ExecuteContainer(() =>
            {
                var page = _context.PageModels.Where(p => p.PageId == model.PageId)
                                                   .Include(p => p.FunctionModels).FirstOrDefault();
                if (page != null)
                {
                    //master page
                    page.PageName = model.PageName;
                    page.PageUrl = model.PageUrl;
                    page.MenuId = model.MenuId;
                    page.OrderIndex = model.OrderIndex;
                    page.Icon = model.Icon;
                    page.isQuickAccess = model.isQuickAccess;
                    page.Actived = model.Actived;
                    //detail function
                    if (FunctionId != null)
                    {
                        ManyToMany(page, FunctionId);
                    }
                    _context.Entry(page).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Permission_PageModel.ToLower())
                });
            });
        }
        #endregion

        #region Delete
        [HttpPost]
        [PortalAuthorizationAttribute]
        public ActionResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var page = _context.PageModels.FirstOrDefault(p => p.PageId == id);
                if (page != null)
                {
                    if (page.FunctionModels != null)
                    {
                        page.FunctionModels.Clear();
                    }
                    _context.Entry(page).State = EntityState.Deleted;

                    //Delete in PagePermission
                    var pagePermission = _context.PagePermissionModels.Where(p => p.PageId == id).ToList();
                    if (pagePermission != null && pagePermission.Count > 0)
                    {
                        _context.PagePermissionModels.RemoveRange(pagePermission);
                    }

                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Permission_PageModel.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.Permission_PageModel.ToLower())
                    });
                }
            });
        }
        #endregion
        #region Helper
        private void CreateViewBag(Guid? MenuId = null, Guid? FunctionId = null)
        {
            // MenuId
            var MenuList = _context.MenuModels.OrderBy(p => p.OrderIndex).ToList();
            ViewBag.MenuId = new SelectList(MenuList, "MenuId", "MenuName", MenuId);

            // FunctionId
            var FunctionList = _context.FunctionModels.OrderBy(p => p.FunctionId).ToList();
            ViewBag.FunctionId = new SelectList(FunctionList, "FunctionId", "FunctionName", FunctionId);
        }

        private void ManyToMany(PageModel model, List<string> funcList)
        {
            if (model.FunctionModels != null)
            {
                model.FunctionModels.Clear();
            }
            if (funcList != null && funcList.Count > 0)
            {
                foreach (var item in funcList)
                {
                    var itemAdd = _context.FunctionModels.Find(item);
                    model.FunctionModels.Add(itemAdd);
                }
            }
        }
        public List<FunctionModel> funcWithOrderBy()
        {
            //get all function
            var funcList = _context.FunctionModels
                                .OrderBy(p => p.FunctionId == ConstFunction.Access ? 0 : 1)
                                .ThenBy(p => p.FunctionId)
                                .ToList();
            return funcList;
        }
        #endregion
    }
}