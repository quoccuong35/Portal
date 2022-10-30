using Portal.Constant;
using Portal.Extensions;
using Portal.Repositories;
using Portal.Resources;
using Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Permission.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        // GET: Auth
        #region Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            HttpCookie userInfo = Request.Cookies["userInfo"];
            AccountLoginViewModel log = new AccountLoginViewModel()
            {
                RememberMe = false,
                Password = "",
                UserName = "",
                ReturnUrl = returnUrl
            };

            if (userInfo != null && userInfo["username"].ToString() != "")
            {
                try
                {
                    log.RememberMe = true;
                    log.UserName = userInfo["username"].ToString();
                    log.Password = userInfo["password"].ToString();
                }
                catch
                {
                }
            }
            return View("Login2", log);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(AccountLoginViewModel model)
        {
            string UserName = model.UserName.Trim();
            string Password = model.Password.Trim();

            if (model.RememberMe == true)
            {
                HttpCookie userInfo = new HttpCookie("userInfo");
                userInfo.HttpOnly = true;
                userInfo["username"] = UserName;
                userInfo["password"] = Password;
                userInfo.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(userInfo);
                Request.Cookies["userInfo"].Expires = DateTime.Now.AddDays(30);
            }
            else
            {
                HttpCookie userInfo = new HttpCookie("userInfo");
                userInfo["username"] = "";
                userInfo["password"] = "";
                Response.Cookies.Add(userInfo);
            }

            //Kiểm tra nếu tài khoản bị khóa thì không cho đăng nhập
            try
            {
                if (_context.Accounts.Where(p => p.UserName == model.UserName && p.Actived != true).FirstOrDefault() != null)
                {
                    string errorMessage = LanguageResource.Account_Locked;
                    ModelState.AddModelError("", errorMessage);
                    return View("Login2", model);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            //Kiểm tra đăng nhập
            if (CheckLogin(UserName, Password))
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl))
                {
                    return Redirect(GetRedirectUrl(model.ReturnUrl));
                }
                else
                {
                    //return RedirectToAction("Index", "Home", null);
                    return Redirect("~/Home/Index");
                }
            }
            else
            {
                string errorMessage = LanguageResource.Account_Confirm;
                ModelState.AddModelError("", errorMessage);
                return View("Login2",model);
            }
        }
        #endregion login
        #region GetRedirectUrl
        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("Index", "Home");
            }

            return returnUrl;
        }
        #endregion GetRedirectUrl
        #region Check Login
        private bool CheckLogin(string UserName, string Password)
        {
            //MD5 password to compare
            Password = RepositoryLibrary.GetMd5Sum(Password);
            var account = _context.Accounts.Where(p => p.UserName == UserName && p.Password == Password).FirstOrDefault();
            if (account != null && account.EmployeeModel != null)
            {
                 string linkAvatar = System.Configuration.ConfigurationManager.AppSettings["Avatar"].ToString();
                var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, account.UserName),//Username
                    new Claim(ClaimTypes.Sid, account.AccountId.ToString()), //AccountId
                    new Claim(ClaimTypes.GivenName,account.EmployeeModel.FullName),// ho và tên
                    new Claim(ClaimTypes.Locality,account.EmployeeModel.Department.DepartmentName), //phạm vi,
                    new Claim(ClaimTypes.MobilePhone,account.EmployeeModel.PhoneNumber), //So dien thoai
                    new Claim(ClaimTypes.Email,account.EmployeeModel.CompanyEmail), //email
                    new Claim(ClaimTypes.StreetAddress,account.EmployeeModel.Department.DepartmentName), //Tên Phòng ban
                    new Claim(ClaimTypes.Role,"All"), //Phan quyen,
                    new Claim(ClaimTypes.NameIdentifier,account.EmployeeModel.EmployeeCode), //EmployeeCode
                    new Claim(ClaimTypes.PrimarySid,account.EmployeeModel.EmployeeId.ToString()), //EmployeeID,
                    new Claim(ClaimTypes.GroupSid,account.EmployeeModel.Position.PositionName), //Chuc vu
                    new Claim(ClaimTypes.Uri,linkAvatar) //Hình avatar
                },
                   DefaultAuthenticationTypes.ApplicationCookie);

                var ctx = Request.GetOwinContext();
                var authManager = ctx.Authentication;

                authManager.SignIn(identity);
                return true;
            }
            else if(account != null && account.UserName.ToLower() =="admin")
            {
                string linkAvatar = System.Configuration.ConfigurationManager.AppSettings["Avatar"].ToString();
                var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, account.UserName),//Username
                    new Claim(ClaimTypes.Sid, account.AccountId.ToString()), //AccountId
                    new Claim(ClaimTypes.GivenName,account.UserName),// ho và tên
                    new Claim(ClaimTypes.Locality,"Tất cả các phòng ban"), //phạm vi,
                    new Claim(ClaimTypes.MobilePhone,""), //So dien thoai
                    new Claim(ClaimTypes.Email,""), //email
                    new Claim(ClaimTypes.StreetAddress,""), //Tên Phòng ban
                    new Claim(ClaimTypes.Role,""), //Phan quyen,
                    new Claim(ClaimTypes.NameIdentifier,""), //EmployeeCode
                    new Claim(ClaimTypes.PrimarySid,""), //EmployeeID,
                    new Claim(ClaimTypes.GroupSid,""), //Chuc vu
                    new Claim(ClaimTypes.Uri,linkAvatar) //Hình avatar
                },
                   DefaultAuthenticationTypes.ApplicationCookie);

                var ctx = Request.GetOwinContext();
                var authManager = ctx.Authentication;

                authManager.SignIn(identity);
                return true;
            }
            return false;
        }
        #endregion Check Login
        #region Logout
        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            //Identity
            authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //Session
            Session["Menu"] = null;
            Session["QuickAccessMenu"] = null;
            return RedirectToAction(ConstAction.Login, ConstController.Auth);
        }
        #endregion

        #region Change Password
        public ActionResult ChangePassword()
        {
            ChangePasswordViewModel changePassword = new ChangePasswordViewModel();
            var accountId = new Guid(CurrentUser.AccountId);
            var account = _context.Accounts.FirstOrDefault(p => p.AccountId == accountId);
            if (account != null)
            {
                changePassword.UserName = account.UserName;
            }
            return View(changePassword);
        }
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel changePassword)
        {
            return ExecuteContainer(() =>
            {
                //MD5 old password to compare
                var accountId = new Guid(CurrentUser.AccountId);
                changePassword.OldPassword = RepositoryLibrary.GetMd5Sum(changePassword.OldPassword);
                var account = _context.Accounts.FirstOrDefault(p => p.UserName == changePassword.UserName
                                                                 && p.Password == changePassword.OldPassword
                                                                 && p.AccountId == accountId);
                if (account != null)
                {
                    //MD5 new password
                    account.Password = RepositoryLibrary.GetMd5Sum(changePassword.NewPassword);

                    HistoryRepository _repository = new HistoryRepository(_context);
                    _repository.SaveUpdateHistory(account.AccountId, CurrentUser.UserName, account);

                    _context.Entry(account).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Password.ToLower())
                    });
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.NotModified,
                    Success = false,
                    Data = LanguageResource.Alert_ChangePassword
                });
            });
        }
        #endregion Change Password
    }
}