using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.EntityModels
{
    [MetadataTypeAttribute(typeof(EmployeeModel.MetaData))]
    public partial class EmployeeModel
    {
        internal sealed class MetaData
        {
            public System.Guid EmployeeId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeCode")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string EmployeeCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Position")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
            public System.Guid PositionID { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeStatusCategory")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
            public System.Guid EmployeeStatusCategoryId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkplaceID")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
            public System.Guid WorkPlaceID { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Department")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
            public System.Guid DepartmentID { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ShiftWork")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
            public System.Guid ShiftWorkId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ParentId")]
            //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
            public System.Guid ParentId { get; set; }

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
            [StringLength(10,MinimumLength =4,ErrorMessage ="Độ dài nhỏ nhất là 4 và lớn nhất là 10 ký tự")]
            public string DateOfBirth { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlaceOfBirth")]
            public string PlaceOfBirth { get; set; }

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

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StandardWorkingDay")]
            //[RegularExpression("([1-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public double StandardWorkingDay { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EndDate")]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public Nullable<System.DateTime> EndDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TrialJobFromDay")]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public System.DateTime TrialJobFromDay { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TrialJobEndDay")]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public System.DateTime TrialJobEndDay { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TemporaryAddress")]
            public string TemporaryAddress { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "HouseholdAddress")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string HouseholdAddress { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Bank")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
            public System.Guid BankId { get; set; }

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

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Specialized")]
            public string Specialized { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DepartmentHint")]
            public string DepartmentHint { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Education")]
            public Nullable<System.Guid> EducationId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ReligionId")]
            public Nullable<System.Guid> ReligionId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FolkId")]
            public Nullable<System.Guid> FolkId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "NationalityId")]
            public System.Guid NationalityId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
            public string Description { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccountId")]
            public Nullable<System.Guid> CreatedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedTime")]
            public Nullable<System.DateTime> CreatedTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedAccountId")]
            public Nullable<System.Guid> LastModifiedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedTime")]
            public Nullable<System.DateTime> LastModifiedTime { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RemainingLeavedays")]
            //[RegularExpression("([1-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            public double RemainingLeavedays { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IsHead")]
            public Nullable<bool> IsHead { get; set; }

            public string ImageUrl { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OvertimeCategory")]
            public Nullable<System.Guid> OvertimeCategoryId { get; set; }
        }
    }
    [MetadataTypeAttribute(typeof(TimeKeepingPeriodModel.MetaData))]
    public partial class TimeKeepingPeriodModel {
        internal sealed class MetaData {
            public System.Guid ID { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public System.DateTime FromDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public System.DateTime ToDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Type2")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
            public string Type { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
            public string Descriptions { get; set; }
            public Nullable<System.Guid> CreatedAccountId { get; set; }
            public Nullable<System.DateTime> CreatedTime { get; set; }
            public Nullable<System.Guid> LastModifiedAccountId { get; set; }
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(HoliDayModel.MetaData))]
    public partial class HoliDayModel
    {
        internal sealed class MetaData
        {
            public System.Guid HolydayId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Date1")]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public System.DateTime Date1 { get; set; }
         
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "HoliDayName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string Name { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            public Nullable<System.Guid> CreatedAccountId { get; set; }
            public Nullable<System.DateTime> CreatedTime { get; set; }
            public Nullable<System.Guid> LastModifiedAccountId { get; set; }
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }
    }
}
