using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Portal.Constant
{
    public class ConstantClass
    {
        #region Variable Session And Cookied
        public const string Session_AccountInfo = "Session_AccountInfo";
        public const string Session_UserId = "UserId";
        public const string Session_UserName = "UserName";
        public const string Session_FullName = "FullName";
        public const string Session_Email = "Email";
        public const string Session_ObjectType = "ObjectType";
        public const string Session_BranchID = "BranchID";
        public const string Session_BranchName = "BranchName";
        public const string Session_BranchCode = "BranchCode";
        public const string Session_BranchShortName = "BranchShortName";
        public const string Session_DepartmentID = "DepartmentID";
        public const string Session_DepartmentName = "DepartmentName";
        public const string Session_DepartmentCode = "DepartmentCode";
        public const string Session_CurrentYear = "CurrentYear";
        public const string Session_CurrentPeriod = "CurrentPeriod";
        public const string Session_KTVID = "KTVID";
        public const string Session_KTVName = "KTVName";
        public const string Session_DaiLyID = "DaiLyID";
        public const string Session_DaiLyName = "DaiLyName";
        public const string Session_RoleId = "RoleId";
        public const string Session_PageId = "PageId";
        public const string Session_PageUrl = "PageUrl";
        public const string Session_CountryId = "CountryId";
        public const string Session_ProvinceId = "ProvinceId";
        public const string Session_DistrictId = "DistrictId";
        public const string Session_PageAccess = "PageAccess";
        public const string Session_PageAccessJson = "PageAccessJson";
        public const string Session_Admin = "Admin";
        public const string Session_CoverBranch = "CoverBranch";
        public const string Session_CompanyRole = "CompanyRole";
        public const string CreateRight = "CreateRight";
        public const string DeleteRight = "DeleteRight";
        public const string PrintRight = "PrintRight";
        public const string UpdateRight = "UpdateRight";
        public const string ViewRight = "ViewRight";
        public const string CreateCus = "CreateCus";

        public const string UploadTemp = "~/UploadTemp";

        public const string Session_BranchCover = "BranchCover";
        public const string Session_UserTypeName = "UserType";

        public const string Session_Menu = "Menu";
        public const string Session_Page = "Page";
        public const string Session_HtmlPage = "HtmlPage";
        public const string Session_ChangeLanguage = "ChangeLanguage";
        public const string Session_SiteMap = "SiteMap";

        public const string Session_Language = "Language";
        public const string Session_CultureLanguage = "CultureLanguage";

        public const string Cookies_userInfo = "userInfo";
        public const string Cookies_username = "username";
        public const string Cookies_password = "password";

        public const string Session_CurrentTime = "CurrentTime";
        public const string Session_CaptchaText = "CaptchaText";

        public const string AppName_IAS = "IAS";
        public const string AppName_IBMS = "IBMS";
        public const string ApplicationID = "FDA5E089-8749-4ABF-BB12-A015F2EC750B";
        public const int Error = -1;
        public const int Success = 1;

        public enum BusinessType { Config, Documents, Lists, Query, ReportDetail, ReportGeneral, Systems, Dynamic, Html };
        public enum ObjectType { All = 0, Branch = 1, Department = 2, SaleForce = 3, AgentTC = 4, AgentDL = 5, User = 6, Agent = 45 }
        public enum Excel_TextAlign { Left = 0, Center = 1, Right = 2 }
        public enum Excel_ObjectType { String = 0, Money = 1, Rate = 2, DateTime = 3 }


        public static string[] SQLCommand = { " create ", " drop ", " use ", " alter ", " describe ", " insert ", " update ", " delete ", " replace ", " truncate ", " start transaction ", " commit ", " rollback " };

        #endregion

        #region Variable Server
        public const long HeadOffice = 0; // Do sua truyen vo 0 thi load het
        public const long HO_ID = 100001;
        public const string AdminRole = "Quản trị hệ thống";
        public const string CompanyRole = "Toàn công ty";
        public const string CoverBranchRole = "Xem Chi nhánh - Phòng ban khác";
        #endregion

        public class CommonFunction
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="info"></param>
            /// <param name="format"></param>
            /// <returns></returns>
            public static string DateTimeToString(DateTime? info, string format)
            {
                try
                {
                    if (info.HasValue)
                    {
                        return info.Value.ToString(format);
                    }
                    return String.Empty;
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.Message);
                    return String.Empty;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="info"></param>
            /// <returns></returns>
            public static string ToString(object info)
            {
                if (info == null)
                {
                    return string.Empty;
                }
                return info.ToString();
            }

            #region Parse Object To Object
            /// <summary>
            /// Parse Object To Integer Type
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int ParseInt(object obj)
            {
                try
                {
                    return Int32.Parse(obj.ToString());
                }
                catch (Exception)
                {
                    return 0;
                }
            }

            /// <summary>
            /// Parse Object To Double Type
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public double ParseDouble(object obj)
            {
                try
                {
                    return Double.Parse(obj.ToString());
                }
                catch (Exception)
                {
                    return 0.00;
                }
            }

            /// <summary>
            /// Parse Object To Double Type
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public long ParseLong(object obj)
            {
                try
                {
                    return long.Parse(obj.ToString());
                }
                catch (Exception)
                {
                    return 0;
                }
            }

            /// <summary>
            /// Parse Object To Decimal Type
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public decimal ParseDecimal(object obj)
            {
                try
                {
                    return Decimal.Parse(obj.ToString());
                }
                catch (Exception)
                {
                    return 0;
                }
            }

            /// <summary>
            /// Parse Object To Boolean Type
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public bool ParseBoolean(object obj)
            {
                try
                {
                    return Boolean.Parse(obj.ToString());
                }
                catch (Exception)
                {
                    return false;
                }
            }

            /// <summary>
            /// Parse Object To String Type
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public string ParseString(object obj)
            {
                try
                {
                    return obj.ToString();
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            public int ParseBooleanToInt(Nullable<bool> obj)
            {
                try
                {
                    if (obj == true)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception)
                {
                    return 0;
                }
            }

            public int? ParseBooleanToIntReturnNull(Nullable<bool> obj)
            {
                try
                {
                    if (obj == true)
                    {
                        return 1;
                    }
                    else if (obj == false)
                    {
                        return 0;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            /// <summary>
            /// Parse Object To Integer Type
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int? ParseIntReturnNull(object obj)
            {
                try
                {
                    return Int32.Parse(obj.ToString());
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public decimal? ParseDecReturnNull(object obj)
            {
                try
                {
                    return Decimal.Parse(obj.ToString());
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public string ParseIntToStringSQL(object obj)
            {
                int? Result = ParseIntReturnNull(obj);
                if (Result != null)
                {
                    return Result.ToString();
                }
                else
                {
                    return "NULL";
                }
            }

            public bool ParseIntToBoolean(object obj)
            {
                int? Result = ParseIntReturnNull(obj);
                if (Result != null && Result == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public DateTime? ParseDateTimeReturnNull(object obj)
            {
                try
                {
                    return DateTime.Parse(obj.ToString());
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public string ParseDateTimeToString(object obj)
            {
                try
                {
                    return ((DateTime)obj).ToString("yyyy-MM-dd");
                }
                catch (Exception)
                {
                    return "";
                }

            }

            public string ParseDecToStringSQL(object obj)
            {
                decimal? Result = ParseDecReturnNull(obj);
                if (Result != null)
                {
                    return Result.ToString();
                }
                else
                {
                    return "NULL";
                }
            }

            /// <summary>
            /// Parse Object To Double Type
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public double? ParseDoubleReturnNull(object obj)
            {
                try
                {
                    return Double.Parse(obj.ToString());
                }
                catch (Exception)
                {
                    return null;
                }
            }

            /// <summary>
            /// Parse Object To Double Type
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public long? ParseLongReturnNull(object obj)
            {
                try
                {
                    return long.Parse(obj.ToString());
                }
                catch (Exception)
                {
                    return null;
                }
            }

            /// <summary>
            /// Parse Object To Decimal Type
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public decimal? ParseDecimalReturnNull(object obj)
            {
                try
                {
                    return Decimal.Parse(obj.ToString());
                }
                catch (Exception)
                {
                    return null;
                }
            }

            /// <summary>
            /// Parse Object To Boolean Type
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public bool? ParseBooleanReturnNull(object obj)
            {
                try
                {
                    return Boolean.Parse(obj.ToString());
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public string ParseBooleanToStringReturnNull(object obj)
            {
                try
                {
                    if (Boolean.Parse(obj.ToString()) == true)
                    {
                        return "1";
                    }
                    else if (Boolean.Parse(obj.ToString()) == false)
                    {
                        return "0";
                    }
                    else
                    {
                        return "NULL";
                    }
                }
                catch (Exception)
                {
                    return "NULL";
                }
            }

            public string ParseDateTimeToStringReturnNull(object obj)
            {
                try
                {
                    string result = string.Empty;
                    if (obj.GetType().Name == "String")
                    {
                        result = new DateTime(ParseInt(obj.ToString().Split('/')[2]), ParseInt(obj.ToString().Split('/')[1]), ParseInt(obj.ToString().Split('/')[0])).ToString("'yyyy-MM-dd'");
                    }
                    else if (IsDateTime(obj) == true)
                    {
                        result = DateTime.Parse(obj.ToString()).ToString("'yyyy-MM-dd'");
                    }
                    return result;
                }
                catch (Exception)
                {
                    return "NULL";
                }
            }

            public string ConvertNumberToString(object obj)
            {
                try
                {
                    if (IsDouble(obj) == true)
                    {
                        return String.Format("{0:#,0.#}", obj);
                    }
                    else
                    {
                        return "0";
                    }
                }
                catch (Exception)
                {
                    return "0";
                }
            }

            public string ConvertNumberToStringPercent(object obj)
            {
                try
                {
                    if (IsDouble(obj) == true)
                    {
                        return String.Format("{0:###.###.###.###.###,##}" + "%", obj);
                    }
                    else
                    {
                        return "0%";
                    }
                }
                catch (Exception)
                {
                    return "0%";
                }
            }

            public decimal ConvertStringToDecimal(string obj)
            {
                try
                {
                    if (IsDouble(obj) == true)
                    {
                        return Convert.ToDecimal(obj, CultureInfo.GetCultureInfo("vi-VN").NumberFormat);
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception)
                {
                    return 0;
                }
            }

            public decimal? ConvertStringToDecimalReturnNull(string obj)
            {
                try
                {
                    if (IsDouble(obj) == true)
                    {
                        return Convert.ToDecimal(obj, CultureInfo.GetCultureInfo("vi-VN").NumberFormat);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            /// <summary>
            /// Parse DateTime To DimDate
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int? ConvertDateTimeToDimDate(object obj)
            {
                try
                {
                    int? result = null;
                    if (obj.GetType().Name == "String")
                    {
                        result = Convert.ToInt32(new DateTime(ParseInt(obj.ToString().Split('/')[2]), ParseInt(obj.ToString().Split('/')[1]), ParseInt(obj.ToString().Split('/')[0])).ToString("yyyyMMdd"));
                    }
                    else if (IsDateTime(obj) == true)
                    {
                        result = Convert.ToInt32(DateTime.Parse(obj.ToString()).ToString("yyyyMMdd"));
                    }
                    return result;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public int? ConvertDateTimeToWorkingMonth(object obj)
            {
                try
                {
                    int? result = null;
                    if (obj.GetType().Name == "String")
                    {
                        result = Convert.ToInt32(new DateTime(ParseInt(obj.ToString().Split('/')[2]), ParseInt(obj.ToString().Split('/')[1]), ParseInt(obj.ToString().Split('/')[0])).ToString("yyyyMM"));
                    }
                    else if (IsDateTime(obj) == true)
                    {
                        result = Convert.ToInt32(DateTime.Parse(obj.ToString()).ToString("yyyyMM"));
                    }
                    return result;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public string ConvertDateTimeToDimDateString(DateTime? obj)
            {
                try
                {
                    return DateTime.Parse(obj.ToString()).ToString("dd/MM/yyyy");
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public string ConvertDateTimeToDimDateStringEN(DateTime? obj)
            {
                try
                {
                    return DateTime.Parse(obj.ToString()).ToString("MM/dd/yyyy");
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public DateTime? ConvertDimDateToDateTime(object obj)
            {
                try
                {
                    DateTime? result = null;
                    if (IsInt(obj) == true)
                    {
                        string strDateTime = obj.ToString();
                        result = new DateTime(ParseInt(strDateTime.Substring(0, 4)), ParseInt(strDateTime.Substring(4, 2)), ParseInt(strDateTime.Substring(6, 2)));
                    }
                    return result;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            #endregion

            #region Validate Date
            /// <summary>
            /// Validate Integer Type
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public bool IsInt(object obj)
            {
                try
                {
                    Int32.Parse(obj.ToString());
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            /// <summary>
            /// Validate Double Type
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public bool IsDouble(object obj)
            {
                try
                {
                    Double.Parse(obj.ToString());
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            /// <summary>
            /// Validate Decimal Type
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public bool IsDecimal(object obj)
            {
                try
                {
                    Decimal.Parse(obj.ToString());
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            /// <summary>
            /// Validate DateTime Type
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public bool IsDateTime(object obj)
            {
                try
                {
                    DateTime.Parse(obj.ToString());
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            #endregion

            #region Order
            public void ResizeStream(int maxWidth, int maxHeight, Stream filePath, string outputPath)
            {
                var image = Image.FromStream(filePath);

                var ratioX = (double)maxWidth / image.Width;
                var ratioY = (double)maxHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);


                var thumbnailBitmap = new Bitmap(newWidth, newHeight);

                var thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
                thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

                var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                thumbnailGraph.DrawImage(image, imageRectangle);

                thumbnailBitmap.Save(outputPath, image.RawFormat);
                thumbnailGraph.Dispose();
                thumbnailBitmap.Dispose();
                image.Dispose();
            }
            //Get MD5
            public string GetMd5Sum(string str)
            {
                // First we need to convert the string into bytes, which
                // means using a text encoder.

                Encoder enc = System.Text.Encoding.Unicode.GetEncoder();
                // Create a buffer large enough to hold the string

                byte[] unicodeText = new byte[str.Length * 2];

                enc.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);

                // Now that we have a byte array we can ask the CSP to hash it

                MD5 md5 = new MD5CryptoServiceProvider();

                byte[] result = md5.ComputeHash(unicodeText);

                // Build the final string by converting each byte

                // into hex and appending it to a StringBuilder

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < result.Length; i++)
                {

                    sb.Append(result[i].ToString("X2"));

                }
                // And return it
                return sb.ToString();
            }

            public string EncodePassword(string source)
            {
                if (string.IsNullOrEmpty(source))
                {
                    return "";
                }
                byte[] binarySource = Encoding.UTF8.GetBytes(source);
                System.Security.Cryptography.SymmetricAlgorithm rijn = System.Security.Cryptography.SymmetricAlgorithm.Create();
                MemoryStream ms = new MemoryStream();
                byte[] rgbIV = Encoding.ASCII.GetBytes("lkjhasdfyuiwhcnt");
                byte[] key = Encoding.ASCII.GetBytes("tkw123aaaa");
                CryptoStream cs = new CryptoStream(ms, rijn.CreateEncryptor(key, rgbIV), CryptoStreamMode.Write);
                cs.Write(binarySource, 0, binarySource.Length);
                cs.Close();
                return Convert.ToBase64String(ms.ToArray());
            }

            public string DecodePassword(string source)
            {
                byte[] binarySource = Convert.FromBase64String(source);
                MemoryStream ms = new MemoryStream();
                System.Security.Cryptography.SymmetricAlgorithm rijn = System.Security.Cryptography.SymmetricAlgorithm.Create();
                byte[] rgbIV = Encoding.ASCII.GetBytes("lkjhasdfyuiwhcnt");
                byte[] key = Encoding.ASCII.GetBytes("tkw123aaaa");
                CryptoStream cs = new CryptoStream(ms, rijn.CreateDecryptor(key, rgbIV),
                CryptoStreamMode.Write);
                cs.Write(binarySource, 0, binarySource.Length);
                cs.Close();
                return Encoding.UTF8.GetString(ms.ToArray());
            }

            public string ConvertToNoMarkString(string text)
            {
                try
                {
                    //Ky tu dac biet

                    for (int i = 32; i < 48; i++)
                    {
                        text = text.Replace(((char)i).ToString(), "-");
                    }
                    text = text.Replace(".", "");
                    text = text.Replace("?", "");
                    text = text.Replace("\"", "");
                    text = text.Replace(" ", "-");
                    text = text.Replace(",", "-");
                    text = text.Replace(";", "-");
                    text = text.Replace(":", "-");

                    text = text.Replace("–", "");
                    text = text.Replace("“", "");
                    text = text.Replace("”", "");

                    text = text.Replace("(", "-");
                    text = text.Replace(")", "-");
                    text = text.Replace("@", "-");
                    text = text.Replace("&", "-");
                    text = text.Replace("*", "-");
                    text = text.Replace("\\", "-");
                    text = text.Replace("+", "-");
                    text = text.Replace("/", "-");
                    text = text.Replace("#", "-");
                    text = text.Replace("$", "-");
                    text = text.Replace("%", "-");
                    text = text.Replace("^", "-");
                    text = text.Replace("--", "-");
                    text = text.Replace("--", "-");
                    if (text.Substring(0, 1) == "-")
                    {
                        text = text.Substring(1);
                    }
                    if (text.Substring(text.Length - 1) == "-")
                    {
                        text = text.Substring(0, text.Length - 1);
                    }
                    //'Dấu Ngang
                    text = text.Replace("A", "A");
                    text = text.Replace("a", "a");
                    text = text.Replace("Ă", "A");
                    text = text.Replace("ă", "a");
                    text = text.Replace("Â", "A");
                    text = text.Replace("â", "a");
                    text = text.Replace("E", "E");
                    text = text.Replace("e", "e");
                    text = text.Replace("Ê", "E");
                    text = text.Replace("ê", "e");
                    text = text.Replace("I", "I");
                    text = text.Replace("i", "i");
                    text = text.Replace("O", "O");
                    text = text.Replace("o", "o");
                    text = text.Replace("Ô", "O");
                    text = text.Replace("ô", "o");
                    text = text.Replace("Ơ", "O");
                    text = text.Replace("ơ", "o");
                    text = text.Replace("U", "U");
                    text = text.Replace("u", "u");
                    text = text.Replace("Ư", "U");
                    text = text.Replace("ư", "u");
                    text = text.Replace("Y", "Y");
                    text = text.Replace("y", "y");

                    //    'Dấu Huyền
                    text = text.Replace("À", "A");
                    text = text.Replace("à", "a");
                    text = text.Replace("Ằ", "A");
                    text = text.Replace("ằ", "a");
                    text = text.Replace("Ầ", "A");
                    text = text.Replace("ầ", "a");
                    text = text.Replace("È", "E");
                    text = text.Replace("è", "e");
                    text = text.Replace("Ề", "E");
                    text = text.Replace("ề", "e");
                    text = text.Replace("Ì", "I");
                    text = text.Replace("ì", "i");
                    text = text.Replace("Ò", "O");
                    text = text.Replace("ò", "o");
                    text = text.Replace("Ồ", "O");
                    text = text.Replace("ồ", "o");
                    text = text.Replace("Ờ", "O");
                    text = text.Replace("ờ", "o");
                    text = text.Replace("Ù", "U");
                    text = text.Replace("ù", "u");
                    text = text.Replace("Ừ", "U");
                    text = text.Replace("ừ", "u");
                    text = text.Replace("Ỳ", "Y");
                    text = text.Replace("ỳ", "y");

                    //'Dấu Sắc
                    text = text.Replace("Á", "A");
                    text = text.Replace("á", "a");
                    text = text.Replace("Ắ", "A");
                    text = text.Replace("ắ", "a");
                    text = text.Replace("Ấ", "A");
                    text = text.Replace("ấ", "a");
                    text = text.Replace("É", "E");
                    text = text.Replace("é", "e");
                    text = text.Replace("Ế", "E");
                    text = text.Replace("ế", "e");
                    text = text.Replace("Í", "I");
                    text = text.Replace("í", "i");
                    text = text.Replace("Ó", "O");
                    text = text.Replace("ó", "o");
                    text = text.Replace("Ố", "O");
                    text = text.Replace("ố", "o");
                    text = text.Replace("Ớ", "O");
                    text = text.Replace("ớ", "o");
                    text = text.Replace("Ú", "U");
                    text = text.Replace("ú", "u");
                    text = text.Replace("Ứ", "U");
                    text = text.Replace("ứ", "u");
                    text = text.Replace("Ý", "Y");
                    text = text.Replace("ý", "y");

                    //'Dấu Hỏi
                    text = text.Replace("Ả", "A");
                    text = text.Replace("ả", "a");
                    text = text.Replace("Ẳ", "A");
                    text = text.Replace("ẳ", "a");
                    text = text.Replace("Ẩ", "A");
                    text = text.Replace("ẩ", "a");
                    text = text.Replace("Ẻ", "E");
                    text = text.Replace("ẻ", "e");
                    text = text.Replace("Ể", "E");
                    text = text.Replace("ể", "e");
                    text = text.Replace("Ỉ", "I");
                    text = text.Replace("ỉ", "i");
                    text = text.Replace("Ỏ", "O");
                    text = text.Replace("ỏ", "o");
                    text = text.Replace("Ổ", "O");
                    text = text.Replace("ổ", "o");
                    text = text.Replace("Ở", "O");
                    text = text.Replace("ở", "o");
                    text = text.Replace("Ủ", "U");
                    text = text.Replace("ủ", "u");
                    text = text.Replace("Ử", "U");
                    text = text.Replace("ử", "u");
                    text = text.Replace("Ỷ", "Y");
                    text = text.Replace("ỷ", "y");

                    //'Dấu Ngã   
                    text = text.Replace("Ã", "A");
                    text = text.Replace("ã", "a");
                    text = text.Replace("Ẵ", "A");
                    text = text.Replace("ẵ", "a");
                    text = text.Replace("Ẫ", "A");
                    text = text.Replace("ẫ", "a");
                    text = text.Replace("Ẽ", "E");
                    text = text.Replace("ẽ", "e");
                    text = text.Replace("Ễ", "E");
                    text = text.Replace("ễ", "e");
                    text = text.Replace("Ĩ", "I");
                    text = text.Replace("ĩ", "i");
                    text = text.Replace("Õ", "O");
                    text = text.Replace("õ", "o");
                    text = text.Replace("Ỗ", "O");
                    text = text.Replace("ỗ", "o");
                    text = text.Replace("Ỡ", "O");
                    text = text.Replace("ỡ", "o");
                    text = text.Replace("Ũ", "U");
                    text = text.Replace("ũ", "u");
                    text = text.Replace("Ữ", "U");
                    text = text.Replace("ữ", "u");
                    text = text.Replace("Ỹ", "Y");
                    text = text.Replace("ỹ", "y");

                    //'Dẫu Nặng
                    text = text.Replace("Ạ", "A");
                    text = text.Replace("ạ", "a");
                    text = text.Replace("Ặ", "A");
                    text = text.Replace("ặ", "a");
                    text = text.Replace("Ậ", "A");
                    text = text.Replace("ậ", "a");
                    text = text.Replace("Ẹ", "E");
                    text = text.Replace("ẹ", "e");
                    text = text.Replace("Ệ", "E");
                    text = text.Replace("ệ", "e");
                    text = text.Replace("Ị", "I");
                    text = text.Replace("ị", "i");
                    text = text.Replace("Ọ", "O");
                    text = text.Replace("ọ", "o");
                    text = text.Replace("Ộ", "O");
                    text = text.Replace("ộ", "o");
                    text = text.Replace("Ợ", "O");
                    text = text.Replace("ợ", "o");
                    text = text.Replace("Ụ", "U");
                    text = text.Replace("ụ", "u");
                    text = text.Replace("Ự", "U");
                    text = text.Replace("ự", "u");
                    text = text.Replace("Ỵ", "Y");
                    text = text.Replace("ỵ", "y");
                    text = text.Replace("Đ", "D");
                    text = text.Replace("đ", "d");
                }
                catch
                {
                }
                return text.ToLower();

            }

            public string RandomString(int size)
            {

                StringBuilder builder = new StringBuilder();
                Random random = new Random();
                char ch;
                for (int i = 0; i < size; i++)
                {
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                    builder.Append(ch);
                }

                return builder.ToString().ToLower();
            }
            #endregion


            /// <summary>
            /// Convert string "on" to Bool
            /// </summary>
            /// <param name="stron"></param>
            /// <returns></returns>
            public static bool StronToBool(object stron)
            {
                if (stron == null)
                    return false;

                if ((stron.ToString().ToLower() == "on") || (stron.ToString().ToLower() == "true"))
                {
                    return true;
                }
                return false;
            }


        }

        public static class CommonFunction_Static
        {
            public static object GetDataValue(object value)
            {
                if (value == null)
                {
                    return DBNull.Value;
                }

                return value;
            }

            public static void DeleteImage(string url)
            {
                Thread.Sleep(10000);
                System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(url);
                foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                {
                    file.Delete();
                }
            }
        }
    }

    public static class ExtensionUtility
    {
        /*Converts List To DataTable*/
        public static DataTable ToDataTable<TSource>(this IList<TSource> data)
        {
            DataTable dataTable = new DataTable(typeof(TSource).Name);
            PropertyInfo[] props = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (TSource item in data)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        /*Converts DataTable To List*/
        public static List<TSource> ToList<TSource>(this DataTable dataTable) where TSource : new()
        {
            var dataList = new List<TSource>();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            var objFieldNames = (from PropertyInfo aProp in typeof(TSource).GetProperties(flags)
                                 select new { Name = aProp.Name, Type = Nullable.GetUnderlyingType(aProp.PropertyType) ?? aProp.PropertyType }).ToList();
            var dataTblFieldNames = (from DataColumn aHeader in dataTable.Columns
                                     select new { Name = aHeader.ColumnName, Type = aHeader.DataType }).ToList();
            var commonFields = objFieldNames.Intersect(dataTblFieldNames).ToList();

            foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
            {
                var aTSource = new TSource();
                foreach (var aField in commonFields)
                {
                    PropertyInfo propertyInfos = aTSource.GetType().GetProperty(aField.Name);
                    var value = (dataRow[aField.Name] == DBNull.Value) ? null : dataRow[aField.Name]; //if database field is nullable
                    propertyInfos.SetValue(aTSource, value, null);
                }
                dataList.Add(aTSource);
            }
            return dataList;
        }
    }


}
