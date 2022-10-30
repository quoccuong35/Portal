using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Portal.Extensions;
using System;
using System.Web.ModelBinding;

[assembly: OwinStartup(typeof(Portal.App_Start.Startup))]
namespace Portal.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Permission/Auth/Login"),
                ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(System.Configuration.ConfigurationManager.AppSettings["expireTimeSpan"].ToString())),
            });
        }
    }
}