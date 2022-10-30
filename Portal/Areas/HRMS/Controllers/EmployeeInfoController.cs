using Portal.Extensions;
using Portal.Resources;
using Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.EntityModels;
using HRMS.Models;

namespace HRMS.Controllers
{
    public class EmployeeInfoController : BaseController
    {
        public ActionResult SearchEmployeeInfo(string employeeCode, string employeeCodeOld, bool bAll = true)
        {
            return ExecuteSearch(() => {
                //employeeCode = "10000338";
               

                EmployeeInfoView data = new EmployeeInfoView();
                data = clsFunction.SearchEmployee(employeeCode,true,_context);
                if (data == null)
                {
                    ViewBag.Error = "Không tìm thấy nhân viên có mã " + employeeCode;
                    return PartialView("_EmployeeInfo", new EmployeeInfoView());
                    //employee = _context.EmployeeModels.FirstOrDefault(it => it.EmployeeCode == employeeCodeOld && it.Actived == true && it.EmployeeStatusCategoryId == lamView);
                }
                data.bAll = bAll;
                return PartialView("_EmployeeInfo", data);
            });
        }
    }
}