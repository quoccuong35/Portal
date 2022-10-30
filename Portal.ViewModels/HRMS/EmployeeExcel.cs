using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.ViewModels
{
    public class EmployeeExcel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ID")]
        //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public Guid? EmployeeId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeCode")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string EmployeeCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TimekeepingCode")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string TimekeepingCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Customer_FullName")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string FullName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Gender")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public bool Gender { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DateOfBirth")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Độ dài nhỏ nhất là 4 và lớn nhất là 10 ký tự")]
        public string DateOfBirth { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlaceOfBirth")]
        public string PlaceOfBirth { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeStatusCategory")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
        public System.String EmployeeStatusCategoryName { get; set; }


        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkplaceID")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
        public System.String WorkPlaceName { get; set; }


        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Department")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
        public System.String DepartmentName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DepartmentHint")]
        public string DepartmentHint { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ParentId")]
        //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
        public System.String ParentName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Position")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
        public System.String PositionName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Specialized")]
        public string Specialized { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ShiftWork")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
        public System.String ShiftWorkName { get; set; }


        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StandardWorkingDay")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public double StandardWorkingDay { get; set; }


        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IdentityCard")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        [StringLength(12, MinimumLength = 9, ErrorMessage = "Độ dài nhỏ nhất là 9 và lớn nhất là 12 ký tự")]
        public string IdentityCard { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DateOfIssue")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public System.DateTime DateOfIssue { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlaceOfIssue")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string PlaceOfIssue { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PhoneNumber")]
        [StringLength(12, ErrorMessage = "Độ dài tối đa là 12 ký tự")]
        public string PhoneNumber { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CompanyEmail")]
        public string CompanyEmail { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PersonalEmail")]
        public string PersonalEmail { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkingDate")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public System.DateTime WorkingDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TrialJobFromDay")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public System.DateTime TrialJobFromDay { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TrialJobEndDay")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public System.DateTime TrialJobEndDay { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EndDate")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> EndDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BankName")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
        public System.String BankName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BankCardNumber")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string BankCardNumber { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BankBranch")]
        public string BankBranch { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TaxId")]
        public string TaxId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SocialinsuranceNumber")]
        public string SocialinsuranceNumber { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RegistrationHospital")]
        public string RegistrationHospital { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Education")]
        public string EducationName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ReligionId")]
        public string ReligionName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FolkId")]
        public String FolkName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "NationalityId")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public System.String NationalityName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TemporaryAddress")]
        public string TemporaryAddress { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "HouseholdAddress")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string HouseholdAddress { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IsHead")]
        public Nullable<bool> IsHead { get; set; }


        ////Excel
        public int RowIndex { get; set; }
        public string Error { get; set; }
        public bool isNullValueId { get; set; }
    }
}
