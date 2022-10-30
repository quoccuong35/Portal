using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.ViewModels
{
    public class MenuViewModel
    {
        public System.Guid MenuId { get; set; }
        public string MenuName { get; set; }
        public string Icon { get; set; }
        public int? OrderIndex { get; set; }

        public List<PageViewModel> PageViewModels { get; set; }

        //Excel
        public int RowIndex { get; set; }
        public string Error { get; set; }
        public bool isNullValueId { get; set; }
    }
}
