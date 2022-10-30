using Portal.Extensions;
using Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class SharedController : BaseController
    {
        // GET: Shared
        public ActionResult _Sidebar()
        {
            PermissionViewModel lst = (PermissionViewModel)Session["Menu"];
            return PartialView("./Partials/_Sidebar", lst);
        }

        public ActionResult _QuickAccessButton()
        {
            PermissionViewModel lst = (PermissionViewModel)Session["QuickAccessMenu"];
            return PartialView("./Partials/_QuickAccessButton", lst);
        }
    }
}