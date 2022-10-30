using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Resources;
using Portal.ViewModels;
using HRMS.Models;
using Portal.Repositories;
using System.Data.Entity;

namespace HRMS.Controllers
{
    public class OvertimeHRMController : BaseController
    {
        // GET: OvertimeHRM
        // Quản lý tang ca
        #region
        public ActionResult Index()
        {
            ViewBag.DepartmentID = Data.DepartmentViewBag();
            return View();
        }
        public ActionResult _Search(FurloughSearchView model) {
            if (model.FromDate == null)
                model.FromDate = FirstDay();
            if (model.ToDate == null)
                model.ToDate = LastDay();

            Guid employeeID = GetEmployeeID(CurrentUser.EmployeeID);
            var data = (from ot in _context.OvertimeModels
                        where ot.OvertimeDay >= model.FromDate && ot.OvertimeDay <= model.ToDate
                        && (model.DepartmentID == null || ot.DepartmentID == model.DepartmentID)
                        select ot
                        ).ToList();
            return PartialView(data);
        }
        #endregion
        #region Edit
        [PortalAuthorization]
        public ActionResult Edit(Guid id)
        {
            var info = _context.OvertimeModels.FirstOrDefault(it => it.OvertimeId == id);
            if (info == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Overtime.ToLower()) });
            }
            OvertimeViewModel model = OvertimeFunction.getDatTa(info, CurrentUser.AccountId, true);
            ViewBag.DepartmentID = Data.DepartmentViewBag(info.DepartmentID);
            TrangThaiDuyet();
            return View(model);
        }
        [PortalAuthorization]
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(OvertimeViewModel model)
        {
            return ExecuteContainer(() =>
            {
                var edit = _context.OvertimeModels.FirstOrDefault(it => it.OvertimeId == model.OvertimeId);

                List<string> errorList = new List<string>();
                if (edit != null)
                {
                    OvertimeFunction.Update(model, edit, errorList, true, GetAccountID(CurrentUser.AccountId), "nhansu");

                }
                if (errorList.Count > 0)
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = errorList
                    });
                }
                HistoryRepository _repository = new HistoryRepository(_context);
                _repository.SaveUpdateHistory(edit.OvertimeId, CurrentUser.UserName, edit);
                _context.Entry(edit).State = EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Overtime.ToLower())
                });
            });
        }
        #endregion Edit
        #region Helper
        private void TrangThaiDuyet(string BrowseStatusID = null)
        {
            var browseStatus = _context.BrowseStatusModels.Where(it => it.BrowseStatusID == "4" || it.BrowseStatusID == "2").ToList();

            ViewBag.BrowseStatusID = new SelectList(browseStatus, "BrowseStatusID", "BrowseStatusName", BrowseStatusID);
        }
        public ActionResult DelEmployee(List<OvertimeDetailViewModel> listOT, string employeeCode)
        {
            if (listOT != null && listOT.Count > 0)
            {
                var del = listOT.FirstOrDefault(it => it.EmployeeCode == employeeCode);

                if (del.OvertimeId != null) // Xóa dưới data
                {

                    if (listOT.Count == 1)
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.Created,
                            Success = false,
                            Data = "Thông tin tăng ca có 1 dòng không thể xóa"
                        });
                    }
                    //var info = _context.OvertimeModels.FirstOrDefault(it => it.OvertimeId == del.OvertimeId);
                    //if (info.Disable1 == true)
                    //{
                    //    return Json(new
                    //    {
                    //        Code = System.Net.HttpStatusCode.Created,
                    //        Success = false,
                    //        Data = "Đã gửi mail không thể xóa"
                    //    });
                    //}
                    var delData = _context.OvertimeDetailModels.FirstOrDefault(it => it.OvertimeId == del.OvertimeId && it.EmployeeId == del.EmployeeId);
                    if (delData != null)
                    {
                        delData.Del = true;
                        delData.DelAccountId = new Guid(CurrentUser.AccountId);
                        delData.DelTime = DateTime.Now;
                        HistoryRepository _repository = new HistoryRepository(_context);
                        _repository.SaveUpdateHistory(delData.OvertimeId, CurrentUser.UserName, delData);
                        _context.SaveChanges();
                    }
                }

                listOT.Remove(del);
                return PartialView("~/Areas/HRMS/Views/Overtime/_OvertimeDetailInfo.cshtml", listOT);
            }
            else
            {
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.NotImplemented,
                    Success = false,
                    Data = "Không có dữ liệu xóa"
                });
            }

        }
        #endregion Helper
    }
}