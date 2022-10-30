using Portal.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.ViewModels
{
    public class AccountViewModel : Account
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "retypePassword")]
        [NotMapped]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Password)]
        [CompareAttribute("Password", ErrorMessage = "",
                                      ErrorMessageResourceName = "CheckretypePassword",
                                      ErrorMessageResourceType = typeof(Resources.LanguageResource))]
        public string retypePassword { get; set; }

        public string Password { get; set; }
        public List<RolesModel> RolesList { get; set; }

        public List<RolesModel> ActivedRolesList { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Permission_RolesModel")]
        public Guid RolesId { get; set; }
    }
}
