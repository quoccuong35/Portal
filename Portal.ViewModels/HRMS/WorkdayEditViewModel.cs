using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.ViewModels
{
    public class WorkdayEditViewModel
    {
        public Guid? Id { get; set; }

        public EmployeeInfoView Employee { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Date1")]
        public DateTime Date1 { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "InTime")]
        public TimeSpan? InTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OutDate")]
        public DateTime? OutDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OutTime")]
        public TimeSpan? OutTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkTime")]
        public string WorkTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OT")]
        public string OT { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OTAfter22h")]
        public string OTAfter22h { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IsNightShift")]
        public bool IsNightShift { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ShiftWorkId")]
        public Guid? ShiftWorkId { get; set; }
    }
}
