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
    public class AccessController : BaseController
    {
        // GET: Access/Index
        #region Index
        [PortalAuthorizationAttribute]
        public ActionResult Index()
        {
            //get all roles
            var roleList = _context.RolesModels.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).ToList();
            //Get Account by AccountId
            var accountId = new Guid(CurrentUser.AccountId);
            var accountFilter = _context.Accounts.Where(p => p.AccountId == accountId).FirstOrDefault();
            if (accountFilter.RolesModels != null && accountFilter.RolesModels.Count > 0)
            {
                var filterRoles = roleList.Where(p => p.OrderIndex >= accountFilter.RolesModels.Min(e => e.OrderIndex)).OrderBy(p => p.OrderIndex).ToList();
                roleList = filterRoles;
            }
            ViewBag.RolesId = new SelectList(roleList, "RolesId", "RolesName", null);

            return View();
        }

        public ActionResult _AccessFormPartial(Guid? RolesId)
        {
            bool isSystemAdmin = false;
            //Get Roles by AccountId
            var accountId = new Guid(CurrentUser.AccountId);
            var roles = _context.Accounts.Where(p => p.AccountId == accountId).FirstOrDefault();
            if (roles.RolesModels != null && roles.RolesModels.Count > 0)
            {
                foreach (var role in roles.RolesModels)
                {
                    if (role.OrderIndex == ConstRoles.isSysAdmin)
                    {
                        isSystemAdmin = true;
                    }
                }
            }
            List<MenuViewModel> menuLst = new List<MenuViewModel>();
            menuLst = _context.MenuModels.Select(p => new MenuViewModel()
            {
                Icon = p.Icon,
                MenuId = p.MenuId,
                MenuName = p.MenuName,
                OrderIndex = p.OrderIndex,
                PageViewModels = p.PageModels.Where(e => e.isSystem == false || (e.isSystem == true && isSystemAdmin)).Select(e => new PageViewModel()
                {
                    PageId = e.PageId,
                    PageName = e.PageName,
                    OrderIndex = e.OrderIndex,
                    FunctionViewModels = e.FunctionModels.Select(f => new FunctionViewModel()
                    {
                        FunctionId = f.FunctionId,
                        FunctionName = f.FunctionName,
                        PageId = e.PageId
                    }).OrderBy(f => f.FunctionId == ConstFunction.Access ? 0 : 1).ToList()
                }).OrderBy(e => e.OrderIndex).ToList()
            }).ToList();

            //select 
            if (menuLst != null && menuLst.Count > 0)
            {
                foreach (var menu in menuLst)
                {
                    if (menu.PageViewModels != null && menu.PageViewModels.Count > 0)
                    {
                        foreach (var page in menu.PageViewModels)
                        {
                            if (page.FunctionViewModels != null && page.FunctionViewModels.Count > 0)
                            {
                                foreach (var function in page.FunctionViewModels)
                                {
                                    var fp = _context.PagePermissionModels.FirstOrDefault(p => p.PageId == page.PageId && p.FunctionId == function.FunctionId && p.RolesId == RolesId);
                                    if (fp != null)
                                    {
                                        function.Selected = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return PartialView(menuLst);
        }
        #endregion


        #region Edit permission
        [HttpPost]
        public ActionResult EditPermission(Guid RolesId, Guid PageId, string FunctionId, bool Check)
        {
            return ExecuteContainer(() =>
            {
                //Checkbox has value = TRUYCAP is required before check remaining functions
                var allFunc = _context.PagePermissionModels.Where(p => p.RolesId == RolesId && p.PageId == PageId).ToList();
                if (FunctionId != ConstFunction.Access)
                {
                    var accessFunc = allFunc.Where(p => p.FunctionId == ConstFunction.Access).FirstOrDefault();
                    if (accessFunc == null)
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.Created,
                            Success = false,
                            Data = string.Format(LanguageResource.Alert_Access)
                        });
                    }
                }
                //1. Check = true => Thêm
                //2. Check = false => Xóa
                var permission = _context.PagePermissionModels.Where(p => p.RolesId == RolesId && p.PageId == PageId && p.FunctionId == FunctionId).FirstOrDefault();
                if (permission == null && Check == true)
                {
                    PagePermissionModel perModel = new PagePermissionModel();
                    perModel.RolesId = RolesId;
                    perModel.PageId = PageId;
                    perModel.FunctionId = FunctionId;
                    _context.Entry(perModel).State = EntityState.Added;
                }
                else
                {
                    if (FunctionId == ConstFunction.Access)
                    {
                        if (allFunc != null && allFunc.Count > 0)
                        {
                            _context.PagePermissionModels.RemoveRange(allFunc);
                        }
                    }
                    else
                    {
                        _context.Entry(permission).State = EntityState.Deleted;
                    }
                }
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Permission.ToLower())
                });
            });
        }
        #endregion
    }
}