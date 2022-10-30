using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Portal.EntityModels
{
    [MetadataTypeAttribute(typeof(RolesModel.MetaData))]
    public partial class RolesModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Export_ExcelCode")]
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
        }
    }

    [MetadataTypeAttribute(typeof(Account.MetaData))]
    public partial class Account
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "UserName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Remote("CheckExistingUserName", "Account", AdditionalFields = "UserNameValid", HttpMethod = "POST", ErrorMessageResourceName = "Validation_Already_Exists", ErrorMessageResourceType = typeof(Resources.LanguageResource))]
            public string UserName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Password")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Permission_RolesModel")]
            public ICollection<RolesModel> RolesModels { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeModel")]
            public Nullable<Guid> EmployeeId { get; set; }
        }
    }
    [MetadataTypeAttribute(typeof(MenuModel.MetaData))]
    public partial class MenuModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MenuId")]
            public System.Guid MenuId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MenuName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string MenuName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderBy")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [RegularExpression("([1-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            public Nullable<int> OrderIndex { get; set; }

            public Nullable<int> Icon { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(FunctionModel.MetaData))]
    public partial class FunctionModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FunctionId")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Remote("CheckExistingFunctionId","Function",AdditionalFields = "FunctionIdValid", HttpMethod ="POST",ErrorMessageResourceName = "Validation_Already_Exists", ErrorMessageResourceType = typeof(Resources.LanguageResource))]
            public string FunctionId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FunctionName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string FunctionName { get; set; }
        }
    }
    [MetadataTypeAttribute(typeof(PageModel.MetaData))]
    public partial class PageModel
    {
        internal sealed class MetaData
        {
            public System.Guid PageId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PageName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string PageName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PageUrl")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string PageUrl { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Permission_MenuModel")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public System.Guid MenuId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderBy")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [RegularExpression("([1-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            public Nullable<int> OrderIndex { get; set; }

            public Nullable<int> Icon { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "isQuickAccess")]
            public Nullable<bool> isQuickAccess { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }
        }
    }
}
