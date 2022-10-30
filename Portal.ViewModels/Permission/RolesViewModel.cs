using Portal.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace Portal.ViewModels
{
    public class RolesViewModel
    {
        public Guid RolesId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RolesCode")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        [Remote("CheckExistingRolesCode", "Roles", AdditionalFields = "RolesCodeValid", HttpMethod = "POST", ErrorMessageResourceName = "Validation_Already_Exists", ErrorMessageResourceType = typeof(Resources.LanguageResource))]
        public string RolesCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RolesName")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string RolesName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderBy")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        [RegularExpression("([0-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
        public Nullable<int> OrderIndex { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public Nullable<bool> Actived { get; set; }

        //Tab Configuration
        public List<TabConfigModel> TabsList { get; set; }

        public List<TabConfigModel> ActivedTabsList { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TabConfig")]
        public Guid TabId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TabConfig")]
        public ICollection<TabConfigModel> TabConfigModel { get; set; }
    }
}
