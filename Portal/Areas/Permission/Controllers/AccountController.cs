using Microsoft.SqlServer.Server;
using Portal.EntityModels;
using Portal.Extensions;
using Portal.Repositories;
using Portal.Resources;
using Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Permission.Controllers
{
    public class AccountController : BaseController
    {
        //GET: /Account/Index
        #region Index
        [PortalAuthorizationAttribute]
        public ActionResult Index()
        {
            //get all roles
            var roles = _context.RolesModels.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).ToList();

            AccountViewModel viewModel = new AccountViewModel()
            {
                RolesList = RolesInCurrentAccount(roles)
            };
            return View(viewModel);
        }

        public ActionResult _Search(string UserName, bool? Actived, Guid[] RolesId = null)
        {
            return ExecuteSearch(() =>
            {
                //Parameters for your query
                string sqlQuery = "exec uspAccount_Search @isDeveloper, @UserName, @Actived";
                List<SqlParameter> parameters = new List<SqlParameter>()
                {
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        ParameterName = "isDeveloper",
                        Value = isDeveloper
                    },
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.NVarChar,
                        Direction = ParameterDirection.Input,
                        ParameterName = "UserName",
                        Value = UserName
                    },
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Input,
                        ParameterName = "Actived",
                        Value = Actived
                    }
                };
                #region RolesId parameter
                //Build your record
                var tableSchema = new List<SqlMetaData>(1)
                {
                    new SqlMetaData("Id", SqlDbType.UniqueIdentifier)
                }.ToArray();

                //And a table as a list of those records
                var table = new List<SqlDataRecord>();
                if (RolesId != null && RolesId.Length > 0)
                {
                    foreach (var r in RolesId)
                    {
                        var tableRow = new SqlDataRecord(tableSchema);
                        tableRow.SetGuid(0, r);
                        table.Add(tableRow);
                    }
                    parameters.Add(
                        new SqlParameter
                        {
                            SqlDbType = SqlDbType.Structured,
                            Direction = ParameterDirection.Input,
                            ParameterName = "RolesId",
                            TypeName = "[dbo].[GuidList]", //Don't forget this one!
                            Value = table
                        });
                    sqlQuery += ", @RolesId";
                }
                #endregion
                var accountList = _context.Database.SqlQuery<AccountResultViewModel>(sqlQuery, parameters.ToArray()).ToList();

                return PartialView(accountList);
            });
        }
        #endregion Index
        //GET: /Account/Create
        #region Create
        [PortalAuthorizationAttribute]
        public ActionResult Create()
        {
            //get all roles
            var roles = _context.RolesModels.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).ToList();

            AccountViewModel viewModel = new AccountViewModel()
            {
                RolesList = RolesInCurrentAccount(roles)
            };
            EmployeeViewBag();
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorizationAttribute]
        public ActionResult Create(Account model, List<Guid> RolesId)
        {
            return ExecuteContainer(() =>
            {
                model.AccountId = Guid.NewGuid();
                //MD5 password
                model.Password = RepositoryLibrary.GetMd5Sum(model.Password);
                model.Actived = true;
                //roles
                if (RolesId != null && RolesId.Count > 0)
                {
                    ManyToMany(model, RolesId);
                }
                else
                {
                    //Bắt buộc nhập thông tin role
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = Portal.Resources.LanguageResource.TTApp_Account_Role_Required
                    });
                }

                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.Permission_AccountModel.ToLower())
                });
            });
        }
        #endregion

        //GET: /Account/Edit
        #region Edit
        [PortalAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            //get all roles
            var list = _context.RolesModels.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).ToList();

            var account = (from p in _context.Accounts.AsEnumerable()
                           where p.AccountId == id
                           select new AccountViewModel()
                           {
                               AccountId = p.AccountId,
                               UserName = p.UserName,
                               Password = p.Password,
                               Actived = p.Actived,
                               RolesList = RolesInCurrentAccount(list),
                               ActivedRolesList = p.RolesModels.ToList()
                           }).FirstOrDefault();

            return View(account);
        }
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorizationAttribute]
        public ActionResult Edit(Account model, List<Guid> RolesId)
        {
            return ExecuteContainer(() =>
            {
                var account = _context.Accounts.Where(p => p.AccountId == model.AccountId)
                                                   .Include(p => p.RolesModels).FirstOrDefault();
                if (account != null)
                {
                    //master
                    account.UserName = model.UserName;
                    account.EmployeeCode = model.EmployeeCode;
                    account.Actived = model.Actived;

                    HistoryRepository _repository = new HistoryRepository(_context);
                    _repository.SaveUpdateHistory(account.AccountId, CurrentUser.UserName, account);

                    //detail roles
                    if (RolesId != null && RolesId.Count > 0)
                    {
                        ManyToMany(account, RolesId);
                    }
                    else
                    {
                        //Bắt buộc nhập thông tin Role
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.NotModified,
                            Success = false,
                            Data = Portal.Resources.LanguageResource.TTApp_Account_Role_Required
                        });
                    }

                    _context.Entry(account).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Permission_AccountModel.ToLower())
                });
            });
        }
        #endregion Edit

        //GET: /Account/Delete
        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PortalAuthorizationAttribute]
        public ActionResult Delete(Guid id)
        {
            return ExecuteContainer(() =>
            {
                var account = _context.Accounts.FirstOrDefault(p => p.AccountId == id);
                if (account != null)
                {
                    ////Account in roles
                    //if (account.RolesModels != null && account.RolesModels.Count > 0)
                    //{
                    //    account.RolesModels.Clear();
                    //}
                    account.Actived = false;
                   
                    _context.Entry(account).State = EntityState.Deleted;
                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Permission_AccountModel.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.Permission_AccountModel.ToLower())
                    });
                }
            });
        }
        #endregion

        #region Helper
        private void ManyToMany(Account model, List<Guid> RolesId)
        {
            if (model.RolesModels != null)
            {
                model.RolesModels.Clear();
            }
            foreach (var item in RolesId)
            {
                var role = _context.RolesModels.Find(item);
                if (role != null)
                {
                    model.RolesModels.Add(role);
                }
            }
        }
        //Get roles by current account
        public List<RolesModel> RolesInCurrentAccount(List<RolesModel> roleList)
        {
            var accountId = new Guid(CurrentUser.AccountId);
            var accountFilter = _context.Accounts.Where(p => p.AccountId == accountId).FirstOrDefault();
            if (accountFilter.RolesModels != null && accountFilter.RolesModels.Count > 0)
            {
                var filterRoles = roleList.Where(p => p.OrderIndex >= accountFilter.RolesModels.Min(e => e.OrderIndex)).OrderBy(p => p.OrderIndex).ToList();
                roleList = filterRoles;
            }
            return roleList;
        }
        #endregion
        #region Remote Validation
        private bool IsExists(string UserName)
        {
            return (_context.Accounts.FirstOrDefault(p => p.UserName == UserName && p.Actived == true) != null);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckExistingUserName(string UserName, string UserNameValid)
        {
            try
            {
                if (!string.IsNullOrEmpty(UserNameValid) && !string.IsNullOrEmpty(UserName) && UserNameValid.ToLower() != UserName.ToLower())
                {
                    return Json(!IsExists(UserName));
                }
                else
                {
                    return Json(true);
                }
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        private bool IsEmployeeExists(string EmployeeCode)
        {
            return (_context.Accounts.FirstOrDefault(p => p.EmployeeCode == EmployeeCode && p.Actived == true) != null);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckExistingEmployeeCode(string EmployeeCode, string EmployeeCodeValid)
        {
            try
            {
                if (!string.IsNullOrEmpty(EmployeeCodeValid) && !string.IsNullOrEmpty(EmployeeCode) && EmployeeCodeValid.ToLower() != EmployeeCode.ToLower())
                {
                    return Json(!IsEmployeeExists(EmployeeCode));
                }
                else
                {
                    return Json(true);
                }
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        #endregion

        #region Change Password
        [PortalAuthorizationAttribute]
        public ActionResult ChangePassword(string id)
        {
            ChangePasswordAccountViewModel changePassword = new ChangePasswordAccountViewModel();
            var accountId = Guid.Parse(id);
            var account = _context.Accounts.FirstOrDefault(p => p.AccountId == accountId);
            if (account != null)
            {
                changePassword.AccountId = account.AccountId;
                changePassword.UserName = account.UserName;

            }

            return View(changePassword);
        }
        [HttpPost]
        [PortalAuthorizationAttribute]
        public ActionResult ChangePassword(ChangePasswordAccountViewModel changePassword)
        {
            return ExecuteContainer(() =>
            {
                //MD5 old password to compare
                //var accountId = changePassword.AccountId;
                //changePassword.OldPassword = RepositoryLibrary.GetMd5Sum(changePassword.OldPassword);
                var account = _context.Accounts.Where(p => p.AccountId == changePassword.AccountId).FirstOrDefault();
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

        private void EmployeeViewBag(Guid? employeeId = null)
        {
            var Employees = _context.EmployeeModels.Where(it => it.Actived == true && it.CompanyEmail != "" && it.CompanyEmail != null && it.Accounts.Count==0).ToList().Select(it=>new { EmployeeId  = it.EmployeeId, FullName = it.EmployeeCode+"-"+it.FullName});


            ViewBag.EmployeeId =  new SelectList(Employees, "EmployeeId", "FullName", Employees);
        } 
        #endregion
    }
}