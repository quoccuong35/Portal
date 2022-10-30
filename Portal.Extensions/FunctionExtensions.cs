using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Portal.Extensions
{
    public static class FunctionExtensions
    {
        //convert string to none unicode: Vũ Hoài Nam => vu-hoai-nam
        public static string RemoveUnicode(this string text)
        {
            //remove unicode
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                                "đ",
                                "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                                "í","ì","ỉ","ĩ","ị",
                                "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                                "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                                "ý","ỳ","ỷ","ỹ","ỵ"
            };
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                                "d",
                                "e","e","e","e","e","e","e","e","e","e","e",
                                "i","i","i","i","i",
                                "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
                                "u","u","u","u","u","u","u","u","u","u","u",
                                "y","y","y","y","y"
            };
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            text = text.ToLower();
            //replace all special characters by '-'
            text = Regex.Replace(text, @"[^0-9a-zA-Z]+", "-");
            //replace all white spaces between words by '-'
            text = Regex.Replace(text, "\\s+", "-");
            return text;
        }

        //get first letter of each word and uppercase
        public static string GetFirstLetter(this string text)
        {
            string[] strSplit = text.Split();
            foreach (string res in strSplit)
            {
                text += res.Substring(0, 1);
            }
            text = text.ToUpper();
            return text;
        }

        
        public static bool SendMail(string Subject, string To, string CC, string Body)
        {
            bool bsendmai = bool.Parse( System.Configuration.ConfigurationManager.AppSettings["isSendEmail"].ToString());
            if (!bsendmai)
                return true;
            MailMessage mail = new MailMessage();
            string host = System.Configuration.ConfigurationManager.AppSettings["Host"].ToString();
            int port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["Port"].ToString());
            string email = System.Configuration.ConfigurationManager.AppSettings["Email"].ToString();
            string pass = System.Configuration.ConfigurationManager.AppSettings["Passmail"].ToString();
            //SmtpClient SmtpServer = new SmtpClient("mail.truongthanh.com");
            SmtpClient SmtpServer = new SmtpClient(host);
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;


                mail.From = new System.Net.Mail.MailAddress(email);
                mail.To.Add(To);
                if (CC.Trim().Length > 0)
                {
                    string[] CCList = CC.Trim().Split(';');
                    foreach (var item in CCList)
                    {
                        mail.CC.Add(item);
                    }
                }
                mail.Subject = Subject;
                mail.IsBodyHtml = true;
                mail.Body = Body;

                //System.Net.Mail.Attachment attachment;
                //attachment = new System.Net.Mail.Attachment(url);
                //mail.Attachments.Add(attachment);
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.Port = port;
                SmtpServer.EnableSsl = true;
                SmtpServer.Credentials = new System.Net.NetworkCredential(email, pass);

                SmtpServer.Send(mail);
                SmtpServer.Dispose();
                mail.Dispose();
                return true;
            }
            catch (SmtpException )
            {
                SmtpServer.Dispose();
                mail.Dispose();
                return false;
            }

        }
    }
}
