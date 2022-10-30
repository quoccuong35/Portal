using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.ViewModels
{
    public class AccountResultViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AccountId")]
        public Guid AccountId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "UserName")]
        public string UserName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public bool? Actived { get; set; }
    }
}
