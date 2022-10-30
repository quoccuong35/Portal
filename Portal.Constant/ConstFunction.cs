using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Constant
{
    public static class ConstFunction
    {
        //Permission: Create, Edit, Delete, Import, Export,...

        //Permission on master 
        public const string Access = "INDEX";
        public const string Create = "CREATE";
        public const string Import = "IMPORT";
        public const string Export = "EXPORT";
        public const string Order = "ORDER";

        //Permission on details
        public const string Edit = "EDIT";
        public const string Delete = "DELETE";
        public const string Upload = "UPLOAD";
        public const string View = "VIEW";
        public const string Approval = "APPROVAL";
        public const string Cancel = "CANCEL";

        // Mã tình trạng làm việc của nhân sự
        public const string WorkingId = "F3827595-B7AA-457F-92D9-80B0E9DF458A";
        public const string XacNhanCong = "XacNhanCong";
        public const string TangCa = "TangCa";
        public const string NghiPhep = "NghiPhep";
    }
}
