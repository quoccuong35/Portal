using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
namespace Portal.ViewModels
{
    public class AppUserPrincipal : ClaimsPrincipal
    {
        public AppUserPrincipal(ClaimsPrincipal principal)
            : base(principal)
        {
        }
        //ID người dùng đăng nhập
        public string AccountId
        {
            get
            {
                return this.FindFirst(ClaimTypes.Sid).Value;
            }
        }
        //Tên tàikhoản đăng nhập
        public string UserName
        {
            get
            {
                return this.FindFirst(ClaimTypes.Name).Value;
            }
        }
        //Họ và tên
        public string FullName
        {
            get
            {
                return this.FindFirst(ClaimTypes.GivenName).Value;
            }
        }
        // Phạm vi theo đơn vị
        public string Scope 
        {
            get
            {
                return this.FindFirst(ClaimTypes.Locality).Value;
            }
        }
        // Số điện thoại
        public string MobilePhone
        {
            get
            {
                return this.FindFirst(ClaimTypes.MobilePhone).Value;
            }
        }
        //Email
        public string Email
        {
            get
            {
                return this.FindFirst(ClaimTypes.Email).Value;
            }
        }
        //PhongBan
        public string Department
        {
            get
            {
                return this.FindFirst(ClaimTypes.StreetAddress).Value;
            }
        }
        // nhóm quyền
        public string Roles
        {
            get
            {
                return this.FindFirst(ClaimTypes.Role).Value;
            }
        }
        // Mã nhân viên
        public string EmployeeCode
        {
            get
            {
                return this.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
        }
        // ID Nhan vien

        public string EmployeeID
        {
            get
            {
                return this.FindFirst(ClaimTypes.PrimarySid).Value;
            }
        }
        // Chức vụ
        public string Position
        {
            get
            {
                return this.FindFirst(ClaimTypes.GroupSid).Value;
            }
        }

        // hình đại diện
        public string Avatar
        {
            get
            {
                return this.FindFirst(ClaimTypes.Uri).Value;
            }
        }
    }
}
