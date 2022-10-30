using Portal.ViewModels;
using Portal.Resources;
using Portal.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Portal.Extensions
{
    public class PortalAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string areaName = "";

            if (filterContext.RouteData.DataTokens["area"] != null)
            {
                areaName = filterContext.RouteData.DataTokens["area"].ToString();
            }
            if (!CheckAccessRight(areaName, actionName, controllerName))/*&& !CheckQuickAccessRight(areaName, actionName, controllerName)*/
            {
                //filterContext.Result = new HttpStatusCodeResult(404);
                filterContext.Result = new RedirectResult(string.Format("/Error/ErrorText?statusCode={0}&exception={1}", 404, LanguageResource.Error_MenuNotExist));
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
        //Decentralize menu and pages
        private bool CheckAccessRight(string Area, string Action, string Controller)
        {
            if (HttpContext.Current.Session["Menu"] != null)
            {
                string pageUrl = string.Empty;
                string actionUrl = string.Empty;
                //Get PageUrl from user input
                if (!string.IsNullOrEmpty(Area))
                {
                    pageUrl = string.Format("/{0}/{1}", Area, Controller);
                    actionUrl = string.Format("/{0}/{1}/{2}", Area, Controller, Action);
                }
                else
                {
                    pageUrl = string.Format("/{0}/{1}", Controller, Action);
                }
                //Get PageUrl from Session["Menu"]
                PermissionViewModel permission = (PermissionViewModel)HttpContext.Current.Session["Menu"];
                var pageId = permission.PageModel.Where(p => p.PageUrl == pageUrl)
                                                .Select(p => p.PageId)
                                                .FirstOrDefault();
                var actionPageId = permission.PageModel.Where(p => p.PageUrl == actionUrl)
                                                .Select(p => p.PageId)
                                                .FirstOrDefault();
                //Compare with PageId in PagePermission
                var pagePermission = permission.PagePermissionModel.Where(p => p.PageId == pageId && p.FunctionId == Action.ToUpper()).FirstOrDefault();
                var actionPagePermission = permission.PagePermissionModel.Where(p => p.PageId == actionPageId).FirstOrDefault();
                if (pagePermission != null || actionPagePermission != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //Decentralize quick access button on menubar (just has function INDEX)
        private bool CheckQuickAccessRight(string Area, string Action, string Controller)
        {
            if (HttpContext.Current.Session["QuickAccessMenu"] != null)
            {
                string pageUrl = string.Empty;
                string actionUrl = string.Empty;
                //Get PageUrl from user input
                if (!string.IsNullOrEmpty(Area))
                {
                    pageUrl = string.Format("/{0}/{1}/{2}", Area, Controller, Action);
                    actionUrl = string.Format("/{0}/{1}/{2}", Area, Controller, Action);
                }
                else
                {
                    pageUrl = string.Format("/{0}/{1}", Controller, Action);
                }
                //Get PageUrl from Session["QuickAccessMenu"]
                PermissionViewModel permission = (PermissionViewModel)HttpContext.Current.Session["QuickAccessMenu"];
                var pageId = permission.PageModel.Where(p => p.PageUrl == pageUrl)
                                                .Select(p => p.PageId)
                                                .FirstOrDefault();
                var actionPageId = permission.PageModel.Where(p => p.PageUrl == actionUrl)
                                                      .Select(p => p.PageId)
                                                      .FirstOrDefault();
                //Compare with PageId in PagePermission
                var pagePermission = permission.PagePermissionModel
                                               .Where(p => p.PageId == pageId
                                                        && p.FunctionId == ConstFunction.Access).FirstOrDefault();
                var actionPagePermission = permission.PagePermissionModel.Where(p => p.PageId == actionPageId).FirstOrDefault();

                if (pagePermission != null && actionPagePermission != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
