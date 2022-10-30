using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.ViewModels
{
    public class AccountLoginViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "UserName")]
        public string UserName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public string errorMessage { get; set; }
    }
}
