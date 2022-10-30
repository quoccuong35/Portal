using Portal.ViewModels;
using System.Security.Claims;
using System.Web.Mvc;

namespace Portal.App_Start
{
    public abstract class AppViewPage<TModel> : WebViewPage<TModel>
    {
        protected AppUserPrincipal CurrentUser
        {
            get
            {
                return new AppUserPrincipal(this.User as ClaimsPrincipal);
            }
        }
    }
    public abstract class AppViewPage : AppViewPage<dynamic>
    {
    }
}