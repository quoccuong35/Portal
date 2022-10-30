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
    public class RolesController : BaseController
    {
        // GET: Roles
        #region Index + Search
        [PortalAuthorizationAttribute]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(string RolesName)
        {
            return ExecuteSearch(() =>
            {
                var RolesNameIsNullOrEmpty = string.IsNullOrEmpty(RolesName);
                var roleList = _context.RolesModels.Where(p => (RolesNameIsNullOrEmpty || p.RolesName.ToLower().Contains(RolesName.ToLower()))
                                                              && p.Actived == true)
                                                  .OrderBy(p => p.OrderIndex).ToList();

                return PartialView(RolesInCurrentAccount(roleList));
            });
        }
        #endregion Index + Search

        //GET: /Roles/Create
        #region Create
        [PortalAuthorizationAttribute]
        public ActionResult Create()
        {
            //RolesViewModel viewModel = new RolesViewModel()
            //{
            //    TabsList = TabsInCurrentRole()
            //};
            return View();
        }
        [HttpPost]
        [ValidateAjax]
        public JsonResult Create(RolesModel model)
        {
            return ExecuteContainer(() =>
            {
                model.RolesId = Guid.NewGuid();
                model.RolesCode = model.RolesCode.ToUpper();
                //if (TabId != null && TabId.Count > 0)
                //{
                //    ManyToMany(model, TabId);
                //}
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.Permission_RolesModel.ToLower())
                });
            });
        }
        #endregion Create

        //GET: /Roles/Edit
        #region Edit
        [PortalAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            var role = (from p in _context.RolesModels.AsEnumerable()
                        where p.RolesId == id select p).FirstOrDefault();
            if (role == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Permission_RolesModel.ToLower()) });
            }
            return View(role);
        }
        [HttpPost]
        [ValidateAjax]
        [PortalAuthorizationAttribute]
        public JsonResult Edit(RolesModel model)
        {
            return ExecuteContainer(() =>
            {
                var role = _context.RolesModels.FirstOrDefault(p => p.RolesId == model.RolesId);
                if (role != null)
                {
                    role.RolesName = model.RolesName;
                    role.OrderIndex = model.OrderIndex;
                    role.Actived = model.Actived;
                    //if (TabId != null && TabId.Count > 0)
                    //{
                    //    ManyToMany(role, TabId);
                    //}
                    _context.Entry(role).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Permission_RolesModel.ToLower())
                });
            });
        }
        #endregion

        #region Remote Validation
        private bool IsExists(string RolesCode)
        {
            return (_context.RolesModels.FirstOrDefault(p => p.RolesCode == RolesCode) != null);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckExistingRolesCode(string RolesCode, string RolesCodeValid)
        {
            try
            {
                if (RolesCodeValid != RolesCode)
                {
                    return Json(!IsExists(RolesCode));
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

        //GET: /Roles/Delete
        #region Delete
        [HttpPost]
        [PortalAuthorizationAttribute]
        public ActionResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var role = _context.RolesModels.FirstOrDefault(p => p.RolesId == id);
                if (role != null)
                {
                    //Account
                    if (role.Accounts != null)
                    {
                        role.Accounts.Clear();
                    }
                    //Tabs config
                    //if (role.TabConfigModel != null)
                    //{
                    //    role.TabConfigModel.Clear();
                    //}
                    _context.Entry(role).State = EntityState.Deleted;

                    //Delete in PagePermission
                    var pagePermission = _context.PagePermissionModels.Where(p => p.RolesId == id).ToList();
                    if (pagePermission != null && pagePermission.Count > 0)
                    {
                        _context.PagePermissionModels.RemoveRange(pagePermission);
                    }

                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Permission_RolesModel.ToLower())
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
        #region Helper
        private void ManyToMany(RolesModel model, List<Guid> TabId)
        {
            if (model.TabConfigModels != null)
            {
                model.TabConfigModels.Clear();
            }
            foreach (var item in TabId)
            {
                var tab = _context.TabConfigModels.Find(item);
                if (tab != null)
                {
                    model.TabConfigModels.Add(tab);
                }
            }
        }
        public List<RolesModel> RolesInCurrentAccount(List<RolesModel> roleList)
        {
            var accountId = new Guid(CurrentUser.AccountId);
            var accountFilter = _context.Accounts.Where(p => p.AccountId == accountId).FirstOrDefault();
            if (accountFilter.RolesModels != null && accountFilter.RolesModels.Count > 0)
            {
                var filterRoles = roleList.Where(p => p.OrderIndex >= accountFilter.RolesModels.Min(e => e.OrderIndex)).OrderBy(p => p.OrderIndex).ToList();
                roleList = filterRoles;
            }
            return roleList;
        }
        public List<TabConfigModel> TabsInCurrentRole()
        {
            List<TabConfigModel> tabsList = new List<TabConfigModel>();
            if (CurrentUser.UserName == "sysadmin")
            {
                tabsList = _context.TabConfigModels.OrderBy(p => p.OrderIndex).ToList();
            }
            else
            {
                //1. Get current account
                //2. Get all roles of current account
                var accountId = new Guid(CurrentUser.AccountId);
                var roleFilter = (from p in _context.Accounts
                                  from r in p.RolesModels
                                  where p.AccountId == accountId
                                  select r).ToList();

                //1. Foreach roles of current account
                //2. Get all tabs of roles
                //3. Foreach tabs
                //4. Add tabs into tabsList (if not exist in list)
                foreach (var item in roleFilter)
                {
                    if (item.TabConfigModels != null && item.TabConfigModels.Count > 0)
                    {
                        foreach (var tab in item.TabConfigModels)
                        {
                            if (!tabsList.Contains(tab))
                            {
                                tabsList.Add(tab);
                            }
                        }
                    }
                }
                //re-order tablist
                tabsList = tabsList.OrderBy(p => p.OrderIndex).ToList();
            }
            return tabsList;
        }
        #endregion Helper

    }
}