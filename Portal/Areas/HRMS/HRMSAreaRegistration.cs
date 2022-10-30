using System.Web.Mvc;

namespace PortalWebUI.Areas.HRMS
{
    public class HRMSAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "HRMS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "HRMS_default",
                "HRMS/{controller}/{action}/{id}",
                new { controller = "HRMS", action = "Index", id = UrlParameter.Optional },
                new string[] { "HRMS.Controllers" }
            );
        }
    }
}