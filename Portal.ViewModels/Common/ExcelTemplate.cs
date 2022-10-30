using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.ViewModels
{
    public class ExcelTemplate
    {
        public string ColumnName { get; set; }
        public bool isAllowedToEdit { get; set; }
        public bool isDateTime { get; set; }
        public bool isCurrency { get; set; }
        public bool isBoolean { get; set; }
        public bool isText { get; set; }
        public bool isDetail { get; set; }
        public string TypeId { get; set; }
        public bool isDepentDropdown { get; set; }
        public bool isComment { get; set; }
        public string strComment { get; set; }

        //VlookUp
        public bool isVlookUp { get; set; }
        public bool isVlookUpRouting { get; set; }

        //Dropdownlist
        public bool isDropdownlist { get; set; }
        public List<DropdownModel> DropdownData { get; set; }
        public List<DropdownIdTypeIntModel> DropdownIdTypeIntData { get; set; }
        public List<DropdownIdTypeStringModel> DropdownIdTypeStringData { get; set; }
    }
    public class ExcelHeadingTemplate
    {
        public string Content { get; set; }
        public int RowsToIgnore { get; set; }
        public bool isWarning { get; set; }
        public bool isHasBorder { get; set; }
        public bool isCode { get; set; }
        public bool isHeadingDetail { get; set; }
    }

    public class DropdownModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? OrderIndex { get; set; }
    }

    public class DropdownIdTypeIntModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? OrderIndex { get; set; }
    }

    public class DropdownIdTypeStringModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
