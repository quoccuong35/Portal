using Portal.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.ViewModels
{
    public class PageFunctionViewModel : PageModel
    {
        public List<FunctionModel> FunctionList { get; set; }

        public List<FunctionModel> ActivedFunctionList { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Permission_FunctionModel")]
        public string FunctionId { get; set; }
    }
}
