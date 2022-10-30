using System;
using System.ComponentModel.DataAnnotations;

namespace Portal.ViewModels
{
    public class DepartmentViewModel
    {
        public System.Guid DepartmentID { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DepartmentCategory")]
        //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
        public Nullable<System.Guid> DepartmentCategoryID { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DepartmentCategory")]
        public String DepartmentCategoryName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DepartmentCode")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string DepartmentCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DepartmentName")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string DepartmentName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
        public Nullable<int> OrderIndex { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public Nullable<bool> Actived { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedUser")]
        public Nullable<System.Guid> CreatedAccountId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedTime")]
        public Nullable<System.DateTime> CreatedTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedUser")]
        public Nullable<System.Guid> LastModifiedAccountId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedTime")]
        public Nullable<System.DateTime> LastModifiedTime { get; set; }
    }
}
