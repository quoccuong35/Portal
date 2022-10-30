using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        //Get System Error
        public ActionResult Error(int statusCode, Exception exception)
        {
            //ajax return false
            if (Request.IsAjaxRequest())
            {
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.InternalServerError,
                    Success = false,
                    Data = string.Format("Error {0}: {1}", statusCode, exception.Message)
                });
            }
            //browser return false
            else
            {
                Response.StatusCode = statusCode;
                ViewBag.StatusCode = statusCode;
                if (exception.InnerException != null)
                {
                    ViewBag.ErrorMessage = exception.InnerException.Message;
                }
                else
                {
                    ViewBag.ErrorMessage = exception.Message;
                }
                return View();
            }
        }

        //Get User's custom Error
        public ActionResult ErrorText(int statusCode, string exception)
        {
            //ajax return false
            if (Request.IsAjaxRequest())
            {
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.InternalServerError,
                    Success = false,
                    Data = string.Format("Error {0}: {1}", statusCode, exception)
                });
            }
            //browser return false
            else
            {
                Response.StatusCode = statusCode;
                ViewBag.StatusCode = statusCode;
                ViewBag.ErrorMessage = exception;
                return View();
            }
        }
    }
}