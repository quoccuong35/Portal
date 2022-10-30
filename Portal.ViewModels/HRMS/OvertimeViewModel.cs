using Portal.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.ViewModels
{
    public class OvertimeViewModel
    {
        public System.Guid? OvertimeId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Department")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
        public Nullable<System.Guid> DepartmentID { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProjectID")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string ProjectID { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OvertimeDay")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public System.DateTime? OvertimeDay { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OvertimeStartTime")]
        [RegularExpression("([01]?[0-9]|2[0-3]):[0-5][0-9]", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_Time24H")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string OvertimeStartTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OvertimeEndTime")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        [RegularExpression("([01]?[0-9]|2[0-3]):[0-5][0-9]", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_Time24H")]
        public string OvertimeEndTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ReasonOT")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string Reason { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ReasonStop")]
        public string ReasonStop { get; set; }
        
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BrowseStatusID")]
        public string BrowseStatusID { get; set; }
        public bool Disable1 { get; set; }
        public Nullable<System.Guid> CreatedAccountId { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.Guid> LastModifiedAccountId { get; set; }
        public Nullable<System.DateTime> LastModifiedTime { get; set; }
        public Nullable<bool> Del { get; set; }
        public List<OvertimeDetailViewModel> OverDetail { get; set; }
        public Guid? AccountID { get; set; }
        public List<ApprovalHistoryModel> AppHistory { get; set; }
    }

    public class OvertimeDetailViewModel {
        public System.Guid? OvertimeId { get; set; }
        public System.Guid? EmployeeId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeCode")]
        public string EmployeeCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FullName")]
        public string FullName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Department")]
        public string DepartmentName { get; set; }
        public bool Disable1 { get; set; }
    }
}
