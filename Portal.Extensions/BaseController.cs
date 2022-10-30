using Portal.EntityModels;
using Portal.Resources;
using Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Portal.Extensions
{
    public class BaseController : System.Web.Mvc.Controller
    {
        //Entity
        public PortalEntities _context = new PortalEntities();

        public List<ExcelHeadingTemplate> heading = new List<ExcelHeadingTemplate>();

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
          
            GetMenuList();
            GetQuickAccessMenuList();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }

        protected JsonResult ValidationInvalid()
        {
            var errorList = ModelState.Values.SelectMany(v => v.Errors).ToList();
            ModelState.Clear();
            foreach (var error in errorList)
            {
                if (string.IsNullOrEmpty(error.ErrorMessage) && error.Exception != null)
                {
                    ModelState.AddModelError("", error.Exception.Message);
                }
                else
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }
            }
            return Json(new
            {
                Code = System.Net.HttpStatusCode.InternalServerError,
                Success = false,
                Data = ModelState.Values.SelectMany(v => v.Errors).ToList()
            });
        }

        #region Execute
        protected JsonResult ExecuteContainer(Func<JsonResult> codeToExecute)
        {
            //1. using: ModelState.IsValid
            if (ModelState.IsValid)
            {
                try
                {
                    // All code will run here
                    // Usage: return ExecuteContainer(() => { ALL RUNNING CODE HERE, remember to return });
                    return codeToExecute.Invoke();
                }
                //2. handle: DbUpdateException
                catch (DbUpdateException ex)
                {
                    foreach (var errorMessage in ErrorHepler.GetaAllMessages(ex))
                    {
                        ModelState.AddModelError("", errorMessage);
                    }
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = ModelState.Values.SelectMany(v => v.Errors)
                    });
                }
                // handlw:DbEntityValidationException
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                        eve.Entry.Entity.GetType().Name,
                                                        eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                        ve.PropertyName,
                                                        ve.ErrorMessage));
                        }
                    }
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = sb.ToString()
                    });
                }
                //3. handle: Exception
                catch (Exception ex)
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = ex.Message
                    });
                }
            }//4. using: ValidationInvalid()
            return ValidationInvalid();
        }


        protected ActionResult ExecuteSearch(Func<PartialViewResult> codeToExecute)
        {
            try
            {
                return codeToExecute.Invoke();
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.InternalServerError,
                    Success = false,
                    Data = ex.Message
                });
            }
        }

        protected JsonResult ExecuteDelete(Func<JsonResult> codeToExecute)
        {
            try
            {
                // All code will run here
                // Usage: return ExecuteContainer(() => { ALL RUNNING CODE HERE, remember to return });
                return codeToExecute.Invoke();
            }
            //1. handle: DbUpdateException
            catch (DbUpdateException ex)
            {
                foreach (var errorMessage in ErrorHepler.GetaAllMessages(ex))
                {
                    ModelState.AddModelError("", errorMessage);
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.NotModified,
                    Success = false,
                    Data = ModelState.Values.SelectMany(v => v.Errors)
                });
            }
            //2. handle: Exception
            catch (Exception ex)
            {
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.NotModified,
                    Success = false,
                    Data = ex.Message
                });
            }
        }
        #endregion Execute

        #region Language
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string lang = null;
            HttpCookie langCookie = Request.Cookies["culture"];
            if (langCookie != null)
            {
                lang = langCookie.Value;
            }
            else
            {
                var userLanguage = Request.UserLanguages;
                var userLang = userLanguage != null ? userLanguage[0] : "";
                if (userLang != "")
                {
                    lang = userLang;
                }
                else
                {
                    lang = MultiLanguage.GetDefaultLanguage();
                }
            }

            new MultiLanguage().SetLanguage(lang);
            return base.BeginExecuteCore(callback, state);
        }
        #endregion Language

        #region Permission
        public AppUserPrincipal CurrentUser
        {
            get
            {
                return new AppUserPrincipal(this.User as ClaimsPrincipal);
            }
        }
        public AppUserPrincipalWebsite CurrentUserWebsite
        {
            get
            {
                return new AppUserPrincipalWebsite(this.User as ClaimsPrincipal);
            }
        }
        public bool isDeveloper
        {
            get
            {
                var accountId = new Guid(CurrentUser.AccountId);
                var currentAccount = _context.Accounts.Where(p => p.AccountId == accountId).FirstOrDefault();
                if (currentAccount.RolesModels != null && currentAccount.RolesModels.Count > 0)
                {
                    var role = currentAccount.RolesModels.Where(p => p.OrderIndex == 0).FirstOrDefault();
                    if (role != null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public void GetMenuList()
        {
            if (Session["Menu"] == null)
            {
                PermissionViewModel permission = null;
                //check login
                //just get user identity AFTER login -> must set function GetMenuList in Home/Index
                if (User.Identity.IsAuthenticated && !string.IsNullOrEmpty(CurrentUser.UserName))
                {
                    permission = new PermissionViewModel();
                    //using dataset to get multiple table in store procedure: page, menu, page permission
                    DataSet ds = new DataSet();
                    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["cnStr"].ConnectionString))
                    {
                        using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                        {
                            cmd.CommandText = "pms.QTHT_PagePermission_GetPagePermissionByAccountId";
                            cmd.Parameters.AddWithValue("@AccountId", CurrentUser.AccountId);
                            cmd.Connection = conn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            conn.Open();
                            System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                            adapter.Fill(ds);
                            conn.Close();
                        }
                    }
                    ds.Tables[0].TableName = "PageModel";
                    ds.Tables[1].TableName = "MenuModel";
                    ds.Tables[2].TableName = "PagePermissionModel";

                    //Convert datatable into list
                    var pageList = (from p in ds.Tables[0].AsEnumerable()
                                    select new PageViewModel()
                                    {
                                        PageId = p.Field<Guid>("PageId"),
                                        PageName = p.Field<string>("PageName"),
                                        PageUrl = p.Field<string>("PageUrl"),
                                        MenuId = p.Field<Guid>("MenuId")
                                    }).ToList();

                    var menuList = (from p in ds.Tables[1].AsEnumerable()
                                    select new MenuViewModel()
                                    {
                                        MenuId = p.Field<Guid>("MenuId"),
                                        MenuName = p.Field<string>("MenuName"),
                                        Icon = p.Field<string>("Icon")
                                    }).ToList();

                    var funcList = (from p in ds.Tables[2].AsEnumerable()
                                    select new PagePermissionViewModel()
                                    {
                                        PageId = p.Field<Guid>("PageId"),
                                        FunctionId = p.Field<string>("FunctionId"),
                                    }).ToList();

                    //add list into model Permission
                    permission.PageModel = pageList;
                    permission.MenuModel = menuList;
                    permission.PagePermissionModel = funcList;
                }
                Session["Menu"] = permission;
            }
        }
        //Quick Access
        public void GetQuickAccessMenuList()
        {
            if (Session["QuickAccessMenu"] == null)
            {
                PermissionViewModel permission = null;
                //check login
                //just get user identity AFTER login -> must set function GetMenuList in Home/Index
                if (User.Identity.IsAuthenticated && !string.IsNullOrEmpty(CurrentUser.UserName))
                {
                    permission = new PermissionViewModel();
                    //using dataset to get multiple table in store procedure: page, menu, page permission
                    DataSet ds = new DataSet();
                    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["cnStr"].ConnectionString))
                    {
                        using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                        {
                            cmd.CommandText = "pms.QTHT_PagePermission_GetQuickAccessPagePermissionByAccountId";
                            cmd.Parameters.AddWithValue("@AccountId", CurrentUser.AccountId);
                            cmd.Connection = conn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            conn.Open();
                            System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                            adapter.Fill(ds);
                            conn.Close();
                        }
                    }
                    ds.Tables[0].TableName = "PageModel";
                    ds.Tables[1].TableName = "MenuModel";
                    ds.Tables[2].TableName = "PagePermissionModel";

                    //Convert datatable into list
                    var pageList = (from p in ds.Tables[0].AsEnumerable()
                                    select new PageViewModel()
                                    {
                                        PageId = p.Field<Guid>("PageId"),
                                        PageName = p.Field<string>("PageName"),
                                        PageUrl = p.Field<string>("PageUrl"),
                                        MenuId = p.Field<Guid>("MenuId"),
                                        Icon = p.Field<string>("Icon")
                                    }).ToList();

                    var menuList = (from p in ds.Tables[1].AsEnumerable()
                                    select new MenuViewModel()
                                    {
                                        MenuId = p.Field<Guid>("MenuId"),
                                        MenuName = p.Field<string>("MenuName"),
                                        Icon = p.Field<string>("Icon")
                                    }).ToList();

                    var funcList = (from p in ds.Tables[2].AsEnumerable()
                                    select new PagePermissionViewModel()
                                    {
                                        PageId = p.Field<Guid>("PageId"),
                                        FunctionId = p.Field<string>("FunctionId"),
                                    }).ToList();

                    //add list into model Permission
                    permission.PageModel = pageList;
                    permission.MenuModel = menuList;
                    permission.PagePermissionModel = funcList;
                }
                Session["QuickAccessMenu"] = permission;
            }
        }

        #endregion

        #region Import Excel
        protected DataSet GetDataSetFromExcel()
        {
            DataSet ds = new DataSet();
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        //Check file is excel
                        //Notes: Châu bổ sung .xlsb
                        if (file.FileName.Contains("xls") || file.FileName.Contains("xlsx") || file.FileName.Contains("xlsb"))
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var mapPath = Server.MapPath("~/Upload/ImportExcel/");
                            if (!Directory.Exists(mapPath))
                            {
                                Directory.CreateDirectory(mapPath);
                            }
                            var path = Path.Combine(mapPath, fileName);
                            file.SaveAs(path);

                            using (ClassImportExcel excelHelper = new ClassImportExcel(path))
                            {
                                excelHelper.Hdr = "YES";
                                excelHelper.Imex = "1";
                                ds = excelHelper.ReadDataSet();
                            }
                        }
                    }
                }
                return ds;
            }
            //handle: Exception
            catch (Exception ex)
            {
                return null;
            }
        }

        protected ActionResult ExcuteImportExcel(Func<JsonResult> codeToExecute)
        {
            try
            {
                // All code will run here
                // Usage: return ExecuteContainer(() => { ALL RUNNING CODE HERE, remember to return });
                return codeToExecute.Invoke();
            }
            //1. handle: DbUpdateException
            catch (DbUpdateException ex)
            {
                string error = "";
                foreach (var errorMessage in ErrorHepler.GetaAllMessages(ex))
                {
                    error += errorMessage + "/n";
                    //ModelState.AddModelError("", errorMessage);
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.NotModified,
                    Success = false,
                    Data = error
                });
            }
            // 2 handlw:DbEntityValidationException
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eve in ex.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                    eve.Entry.Entity.GetType().Name,
                                                    eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                    ve.PropertyName,
                                                    ve.ErrorMessage));
                    }
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.NotModified,
                    Success = false,
                    Data = sb.ToString()
                });
            }
            //3. handle: Exception
            catch (Exception ex)
            {
                string Message = "";
                if (ex.InnerException != null)
                {
                    Message = ex.InnerException.Message;
                }
                else
                {
                    Message = ex.Message;
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.NotModified,
                    Success = false,
                    Data = "Lỗi: " + Message
                });
            }
        }

        #region GetTypeFunction
        protected dynamic GetTypeFunction<T>(string value, int index)
        {
            if (typeof(T) == typeof(bool))
            {
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }
                else
                {
                    if (value.ToUpper() == "X")
                    {
                        return true;
                    }
                    else
                    {
                        return string.Empty;
                    }
                    //return value.ToUpper() == "X" ? true : false;
                }
            }
            else if (typeof(T) == typeof(DateTime))
            {
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }
                else
                {
                    DateTime result = DateTime.Parse(ConvertToDateTime(value.ToString()));
                    return result;
                }
            }
            else if (typeof(T) == typeof(int))
            {
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }
                else
                {
                    return Convert.ToInt32(value);
                }
            }
            else if (typeof(T) == typeof(decimal?) || typeof(T) == typeof(decimal))
            {
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }
                else
                {
                    if (value.Contains(","))
                    {
                        value = value.Replace(",", "");
                        return Convert.ToDecimal(value);
                    }
                    return Convert.ToDecimal(value);
                }
            }
            else if (typeof(T) == typeof(Guid))
            {
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }
                else
                {
                    return new Guid(value);
                }
            }
            else
            {
                return null;
            }
        }
        #endregion GetTypeFunction
        #endregion Import Excel

        #region Export Excel

        protected void CreateExportHeader(string fileheader, string controllerCode)
        {
            //Default:
            //1. heading[0] is controller code
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = controllerCode,
                RowsToIgnore = 1,
                isWarning = false,
                isCode = true,
                isHasBorder = false,
            });
            //2. heading[1] is file name
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = fileheader.ToUpper(),
                RowsToIgnore = 1,
                isWarning = false,
                isCode = false,
                isHasBorder = false,
            });
            //3. heading[2] is warning (edit)
            //Warning
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = LanguageResource.Export_ExcelWarning1,
                RowsToIgnore = 0,
                isWarning = true,
                isCode = false,
                isHasBorder = true,
            });
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = LanguageResource.Export_ExcelWarning2,
                RowsToIgnore = 1,
                isWarning = true,
                isCode = false,
                isHasBorder = false,
            });
        }
        protected void CreateExportHeader(List<ExcelHeadingTemplate> heading2, string fileheader, string controllerCode)
        {
            //Default:
            //1. heading[0] is controller code
            heading2.Add(new ExcelHeadingTemplate()
            {
                Content = controllerCode,
                RowsToIgnore = 1,
                isWarning = false,
                isCode = true,
                isHasBorder = false,
            });
            //2. heading[1] is file name
            heading2.Add(new ExcelHeadingTemplate()
            {
                Content = fileheader.ToUpper(),
                RowsToIgnore = 1,
                isWarning = false,
                isCode = false,
                isHasBorder = false,
            });
            //3. heading[2] is warning (edit)
            //Warning
            heading2.Add(new ExcelHeadingTemplate()
            {
                Content = LanguageResource.Export_ExcelWarning1,
                RowsToIgnore = 0,
                isWarning = true,
                isCode = false,
                isHasBorder = true,
            });
            heading2.Add(new ExcelHeadingTemplate()
            {
                Content = LanguageResource.Export_ExcelWarning2,
                RowsToIgnore = 1,
                isWarning = true,
                isCode = false,
                isHasBorder = false,
            });
        }
        #endregion Export Excel

        #region RemoveSign For Vietnamese String
        private static readonly string[] VietnameseSigns = new string[]
        {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
        };

        public static string RemoveSign4VietnameseString(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }
        #endregion RemoveSign For Vietnamese String

        #region Upload Image
        protected string Upload(HttpPostedFileBase file, string folder, int minWidth = 300, int maxWidth = 1600, int minHeight = 300, int maxHeight = 1600)
        {
            string ret = "";
            string parth = "";
            string thumparth = "";
            try
            {
                if (file != null && file.ContentLength > 0)
                {// nếu có chọn file

                    string filename = RepositoryLibrary.ConvertToNoMarkString(file.FileName);
                    string type = filename.Substring(filename.Length - 4);
                    //Nếu là jpeg thì đổi thành jpg 
                    if (type.ToLower() == "jpeg")
                    {
                        filename = filename.Substring(0, filename.Length - 5) + ".jpg";
                    }
                    //Đổi tên lại thành chuỗi phân biệt tránh trùng
                    filename = filename.Substring(0, filename.Length - 3) + "." + filename.Substring(filename.Length - 3);
                    string strName = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + filename;

                    type = filename.Substring(filename.Length - 3);

                    //folder path
                    var existPath = Server.MapPath("~/Upload/" + folder);
                    Directory.CreateDirectory(existPath);

                    var existThumbPath = Server.MapPath("~/Upload/" + folder + "/thum/");
                    Directory.CreateDirectory(existThumbPath);

                    //file path
                    parth = Server.MapPath("~/Upload/" + folder + "/" + strName);
                    thumparth = Server.MapPath("~/Upload/" + folder + "/thum/" + strName);

                    //gán giá trị trả về
                    ret = strName;

                    //Nếu không phải ảnh động hay ảnh trong suốt thì tiến hành resize
                    if (type.ToLower() != "gif" && type.ToLower() != "png")
                    {
                        var img = System.Drawing.Image.FromStream(file.InputStream, true, true);
                        int w = img.Width;
                        int h = img.Height;
                        //save to root folder
                        if (w >= maxWidth || h >= maxHeight)
                        {
                            RepositoryLibrary.ResizeStream(maxWidth, maxHeight, file.InputStream, parth);
                        }
                        else
                        {
                            RepositoryLibrary.ResizeStream(w, h, file.InputStream, parth);
                        }
                        //save to thum
                        if (w >= minWidth || h >= minHeight)
                        {
                            RepositoryLibrary.ResizeStream(maxWidth, minHeight, file.InputStream, thumparth);
                        }
                        else
                        {
                            RepositoryLibrary.ResizeStream(w, h, file.InputStream, thumparth);
                        }
                    }
                    else
                    {
                        file.SaveAs(parth);
                        file.SaveAs(thumparth);
                    }
                }
                else
                {
                    ret = "noimage.jpg";
                }
            }
            catch
            {
                ret = "noimage.jpg";
            }
            return ret;
        }
        protected string Upload(HttpPostedFileBase file, string folder,string fileName, int minWidth = 300, int maxWidth = 1600, int minHeight = 300, int maxHeight = 1600)
        {
            string ret = "";
            string parth = "";
            try
            {
                if (file != null && file.ContentLength > 0)
                {// nếu có chọn file

                    // string filename = RepositoryLibrary.ConvertToNoMarkString(file.FileName);
                    string filename = file.FileName;
                    string type = filename.Substring(filename.IndexOf('.')+1);
                    //Nếu là jpeg thì đổi thành jpg 
                    if (type.ToLower() == "jpeg")
                    {
                        filename = filename.Substring(0, filename.Length - 5) + ".jpg";
                    }
                    //Đổi tên lại thành chuỗi phân biệt tránh trùng
                    filename = filename.Substring(0, filename.Length - 3) + "." + filename.Substring(filename.Length - 3);
                    // string strName = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + filename;
                    string strName = fileName + "." + type;
                    //type = filename.Substring(filename.Length - 3);

                    //folder path
                    var existPath = Server.MapPath("~/Upload/" + folder);
                    Directory.CreateDirectory(existPath);

                    //var existThumbPath = Server.MapPath("~/Upload/" + folder + "/thum/");
                    //Directory.CreateDirectory(existThumbPath);

                    //file path
                    parth = Server.MapPath("~/Upload/" + folder + "/" + strName);
                  //  thumparth = Server.MapPath("~/Upload/" + folder + "/thum/" + strName);

                    //gán giá trị trả về
                    ret = strName;

                    //Nếu không phải ảnh động hay ảnh trong suốt thì tiến hành resize
                    if (type.ToLower() != "gif" && type.ToLower() != "png")
                    {
                        var img = System.Drawing.Image.FromStream(file.InputStream, true, true);
                        int w = img.Width;
                        int h = img.Height;
                        //save to root folder
                        if (w >= maxWidth || h >= maxHeight)
                        {
                            RepositoryLibrary.ResizeStream(maxWidth, maxHeight, file.InputStream, parth);
                        }
                        else
                        {
                            RepositoryLibrary.ResizeStream(w, h, file.InputStream, parth);
                        }
                        ////save to thum
                        //if (w >= minWidth || h >= minHeight)
                        //{
                        //    RepositoryLibrary.ResizeStream(maxWidth, minHeight, file.InputStream, thumparth);
                        //}
                        //else
                        //{
                        //    RepositoryLibrary.ResizeStream(w, h, file.InputStream, thumparth);
                        //}
                    }
                    else
                    {
                        file.SaveAs(parth);
                        //file.SaveAs(thumparth);
                    }
                }
                else
                {
                    ret = null;
                }
            }
            catch
            {
                ret = null;
            }
            return ret;
        }
        #endregion

        #region Upload file still own name
        public string UploadFileName(HttpPostedFileBase file, string folder)
        {
            var fileName = Path.GetFileName(file.FileName);

            //Create dynamically folder to save file
            var existPath = Server.MapPath(string.Format("~/Upload/{0}", folder));
            Directory.CreateDirectory(existPath);
            var path = Path.Combine(existPath, fileName);

            file.SaveAs(path);

            return fileName;
        }
        #endregion

        #region Excel Convert
        public static string ConvertToDateTime(string strExcelDate)
        {
            double excelDate;
            try
            {
                excelDate = Convert.ToDouble(strExcelDate);
            }
            catch
            {
                return strExcelDate;
            }
            if (excelDate < 1)
            {
                throw new ArgumentException("Excel dates cannot be smaller than 0.");
            }
            DateTime dateOfReference = new DateTime(1900, 1, 1);
            if (excelDate > 60d)
            {
                excelDate = excelDate - 2;
            }
            else
            {
                excelDate = excelDate - 1;
            }
            return dateOfReference.AddDays(excelDate).ToShortDateString();
        }
        #endregion

        public ActionResult ChangeLanguage(string lang, string returnUrl)
        {
            new MultiLanguage().SetLanguage(lang);
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "/";
            }
            return Redirect(returnUrl);
        }


        public String CheckLanguage()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            var lang = currentCulture.TwoLetterISOLanguageName;
            return lang;
        }

        //
        public  Guid GetEmployeeID(string employeeId)
        {
            return new Guid(employeeId);
        }

        public  Guid GetAccountID(string accountId)
        {
            return new Guid(accountId);
        }
        public  DateTime FirstDay() {
            DateTime dNow = DateTime.Now;
            return new DateTime(dNow.Year, dNow.Month, 01);
        }
        public  DateTime LastDay()
        {
            DateTime dNow = DateTime.Now;
            return new DateTime(dNow.Year, dNow.Month,DateTime.DaysInMonth(dNow.Year,dNow.Month));
        }
      
    }
}
