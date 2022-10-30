using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portal.EntityModels;

namespace Portal.ViewModels
{
    public class FurloughViewModel
    {
        public Guid? FurloughId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public DateTime FromDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public DateTime ToDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LeaveCategory")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public Guid? LeaveCategoryId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reason")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string Reason { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ReasonStop")]
        public string ReasonStop { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BrowseStatusID")]
        public string BrowseStatusID { get; set; }
        public List<FurloughDetailViewModel> FurloughDetail { get; set; }

        public List<ApprovalHistoryModel> AppHistory { get; set; }
        public EmployeeInfoView Employee { get; set; }

        public Guid? CreatedAccountId { get; set; }

        public Guid? AccountID { get; set; }

        public bool? Lock { get; set; }

        public bool? Del { get; set; }

    }
    public class FurloughDetailViewModel {
        public bool Check { get; set; }
        public Guid? FurloughId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DayOff")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public DateTime DayOff { get; set; }

        //public double NumberDay { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TypeDate")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string TypeDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
        public string Note { get; set; }
    }
}
