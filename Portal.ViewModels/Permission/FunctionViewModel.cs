using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.ViewModels
{
    public class FunctionViewModel
    {
        public Guid PageId { get; set; }
        public string FunctionId { get; set; }
        public string FunctionName { get; set; }
        public bool Selected { get; set; }
    }
}
