using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Constant
{
    public static class ConstRoles
    {
        public const int isSysAdmin = 0;

        public const string SYSADMIN = "SYSADMIN";

        public const string CAP1 = "CAP1";
        public const string CAP2 = "CAP2";
        public const string CAP3 = "CAP3";
        public const string CAP4 = "CAP4";

        public const string GUEST = "GUEST";

        public const string Customer = "CUSTOMER";
    }
    public static class ConstRolesForTienThuApp
    {
        #region - Chưa Sử dụng
        ////Web - Admin
        //public static Guid ADMIN = new Guid ("A8C06D08-16DF-4188-9515-58C39D393E08");
        ////SysAdmin - Developer
        //public static Guid SYSADMIN = new Guid("43A65EB8-5A75-4235-8CA8-92C091823642");
        ////Mobile - Bán hàng
        //public static Guid SALE = new Guid("758F03C6-01C4-45B0-8D10-A20E2331150E");
        ////Mobile - Dịch vụ
        //public static Guid SERVICE = new Guid("D4304F0A-067F-4D67-A40C-0B5666A01018");
        #endregion

        //Mobile - Khách hàng
        public static Guid CUSTOMER = new Guid("60B32D4F-EE5D-47E7-99A2-6324F1229C59");
    }
}
