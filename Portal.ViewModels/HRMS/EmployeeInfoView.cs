using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.ViewModels
{
    public class EmployeeInfoView
    {
        public Guid EmployeeId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeCode")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string EmployeeCode { get; set; }


        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FullName")]
        //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string FullName { get; set; }

        [DisplayFormat(DataFormatString = "{0.00}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RemainingLeavedays")]
        public string RemainingLeavedays { get; set; }


        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DepartmentName")]
        public string DepartmentName { get; set; }


        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StartTime")]
        public string StartTime { get; set; }


        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EndTime_1")]
        public string EndTime { get; set; }

        public bool Readonly { get; set; }
        public bool bAll { get; set; }
    }
}
