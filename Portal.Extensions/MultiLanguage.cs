using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Portal.Extensions
{
    public class Languages
    {
        public string LanguageFullName { get; set; }
        public string LanguageCultureName { get; set; }
    }
    public class MultiLanguage
    {
        public static List<Languages> AvailableLanguages = new List<Languages> {
            new Languages {
                LanguageFullName = "Tiếng việt", LanguageCultureName = "vi"
            },
            new Languages {
                LanguageFullName = "English", LanguageCultureName = "en"
            },
        };
        public static bool IsLanguageAvailable(string lang)
        {
            return AvailableLanguages.Where(a => a.LanguageCultureName.Equals(lang)).FirstOrDefault() != null ? true : false;
        }
        public static string GetDefaultLanguage()
        {
            return AvailableLanguages[0].LanguageCultureName;
        }
        public static string GetCurrentLanguage()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("culture");
            return cookie.Value;
        }

        public void SetLanguage(string lang)
        {
            try
            {
                if (!IsLanguageAvailable(lang)) lang = GetDefaultLanguage();
                //var cultureInfo = new CultureInfo(lang);
                //Thread.CurrentThread.CurrentUICulture = cultureInfo;
                //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);

                 string culture = System.Configuration.ConfigurationManager.AppSettings["DefaultLanguage"];
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;


                HttpCookie langCookie = new HttpCookie("culture", lang);
                langCookie.Expires = DateTime.Now.AddYears(1);
                HttpContext.Current.Response.Cookies.Add(langCookie);
            }
            catch (Exception) { }
        }
    }
}
