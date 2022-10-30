using Portal.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.ViewModels
{
    public class ConfirmWorkingTimeViewModel
    {
        public System.Guid ConfirmWorkingTimeID { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Date1")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public System.DateTime Date1 { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Time1")]
        [RegularExpression("([01]?[0-9]|2[0-3]):[0-5][0-9]", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_Time24H")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string Time1 { get; set; }
      
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Type1")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
        public string Type1 { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reason")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string Reason { get; set; }

        public string BrowseStatusID { get; set; }
        public bool Del { get; set; }
        public System.Guid CreatedAccountId { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.Guid> LastModifiedAccountId { get; set; }
        public Nullable<System.DateTime> LastModifiedTime { get; set; }
        public EmployeeInfoView Employee { get; set; }

        public bool Disable1 { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ReasonStop")]
        public string ReasonStop { get; set; }
        public List<ApprovalHistoryModel> AppHistory { get; set; }
        public Guid AccountID { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IsNightShift")]
        public bool IsNightShift { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ShiftWork")]
        public Nullable<System.Guid> ShiftWorkId { get; set; }
       
    }
}
