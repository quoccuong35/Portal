using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.ViewModels
{
    public class EmployeeSearch
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeCode")]
        public string EmployeeCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Position")]
        public System.Guid? PositionID { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeStatusCategory")]
        public System.Guid? EmployeeStatusCategoryId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Department")]
        public System.Guid? DepartmentID { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Customer_FullName")]
        public string FullName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? FromDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? ToDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public Nullable<bool> Actived { get; set; }

    }
}
