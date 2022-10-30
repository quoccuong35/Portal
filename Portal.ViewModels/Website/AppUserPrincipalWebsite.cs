using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
namespace Portal.ViewModels
{
    public class AppUserPrincipalWebsite : ClaimsPrincipal
    {
        public AppUserPrincipalWebsite(ClaimsPrincipal principal)
            : base(principal)
        {
        }

        public string AccountId
        {
            get
            {
                return this.FindFirst(ClaimTypes.Sid).Value;
            }
        }

        public string UserName
        {
            get
            {
                return this.FindFirst(ClaimTypes.Name).Value;
            }
        }
    }
}
