using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portal.EntityModels;

namespace Portal.ViewModels
{
    public class WorkdayViewModel : ShiftWork
    {
        public Guid EmployeeId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeCode")]
        public string EmployeeCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TimekeepingCode")]
        public string TimekeepingCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FullName")]
        public string FullName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Department")]
        public string DepartmentName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Position")]
        public string PositionName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "InDate")]
        public DateTime InDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "InTime")]
        public TimeSpan? InTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "InTimeEdit")]
        [RegularExpression("([01]?[0-9]|2[0-3]):[0-5][0-9]", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_Time24H")]
        public string InTimeEdit { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OutDate")]
        public DateTime OutDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OutTime")]
        public TimeSpan? OutTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OutDateEdit")]
        public DateTime? OutDateEdit { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OutTimeEdit")]
        public TimeSpan? OutTimeEdit { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Minute")]
        public double? Minute { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Hour")]
        public double? Hour { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalWH")]
        public double? TotalWH { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkTime")]
        public double? WorkTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkTimeEdit")]
        public string WorkTimeEdit { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OutOfficeHours")]
        public double? OutOfficeHours { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OT")]
        public double? OT { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OTEdit")]
        public string OTEdit { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OTAfter22h")]
        public double? OTAfter22h { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OTAfter22hEdit")]
        public double? OTAfter22hEdit { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Saturday")]
        public int? Saturday { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Status")]
        public string Status { get; set; }

        public bool LateOrEarly { get; set; }
        public string InTimeCSS { get; set; }
        public string OutTimeCSS { get; set; }
        public DateTime Date { get; set; }
        public string StatusWordday { get; set; }
        public string DayOfWeek { get; set; }
    }
}
