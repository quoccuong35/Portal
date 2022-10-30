using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Constant
{
    public static class ConstController
    {
        #region MasterData
        // khối
        public const string DepartmentCategory = "DepartmentCategory";
        // đơn vị
        public const string Department = "Department";
        // chức vụ
        public const string Position = "Position";
        // Nơi làm việc
        public const string Workplace = "Workplace";
        // Ca làm việc
        public const string ShiftWork = "ShiftWork";
        //Dân tộc
        public const string Folk = "Folk";
        //Tôn giáo
        public const string Religion = "Religion";
        // Trình độ văn hóa
        public const string Education = "Education";
        // Quốc tịch
        public const string Nationality = "Nationality";
        // Loại nghỉ phép
        public const string LeaveCategory = "LeaveCategory";
        // Bank Ngân hàng
        public const string Bank = "Bank"; 
        // ma trix duyệt nghĩ phép
        public const string MaTrixFurlough = "MaTrixFurlough";
        // Loại tăng ca
        public const string OvertimeCategory = "OvertimeCategory";
        //Matrix tăng ca
        public const string MatrixOvertime = "MatrixOvertime";

        #endregion MasterData

        #region Permission
        //Nhóm người dùng, Người dùng

        public const string Roles = "Roles";
        public const string Account = "Account";
        public const string Auth = "Auth";
        public const string Menu = "Menu";
        public const string Function = "Function";
        public const string Page = "Page";
        #endregion Permission
        //Phân quyền dữ liệu
        public const string GroupData = "GroupData";
        public const string Data = "Data";

        public const string Home = "Home";

        #region HRMS
        //Nhân sự
        public const string EmployeeModel = "EmployeeModel";
        //Nghỉ phép
        public const string FurloughModel = "FurloughModel";
        //
        //Nghỉ phép
        public const string EmployeeIn = "EmployeeIn";
        //Duyệt nghỉ phép
        public const string FurloughApproval = "FurloughApproval";
        //Quản ly nghỉ phép
        public const string FurloughHRM = "FurloughHRM";
        // Tang ca
        public const string Overtime = "Overtime";
        // Duyet tang ta
        public const string OvertimeApproval = "OvertimeApproval";
        //Quản ly tang ca
        public const string OvertimeHRM = "OvertimeHRM";
        // Xác nhận công
        public const string ConfirmWorkingTime = "ConfirmWorkingTime";
        // Duyệt xác nhận công 
        public const string ConfirmWorkingTimeApproval = "ConfirmWorkingTimeApproval";
        // Quản lý Xác nhận công 
        public const string ConfirmWorkingTimeHRM = "ConfirmWorkingTimeHRM";

        // CÔng
        public const string TimeKeepingPeriod = "TimeKeepingPeriod";
        // Ngày lễ
        public const string HoliDay = "HoliDay";
        // Công ngày
        public const string Workday = "Workday";

        #endregion HRMS
    }
    public static class ConstArea
    {
        public const string MasterData = "MasterData";
        public const string Permission = "Permission";
        public const string HRMS = "HRMS";
    }
    public static class ConstAction
    {
        public const string Login = "Login";
    }
}
