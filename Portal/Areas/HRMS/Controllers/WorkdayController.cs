using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.ViewModels;
using Portal.Resources;
using HRMS.Models;
using System.Diagnostics;
using Portal.EntityModels;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.History;
using Portal.Repositories;

namespace HRMS.Controllers
{
    public class WorkdayController : BaseController
    {
        // GET: Workday
        #region Hiển thị thông tin công
        [PortalAuthorization]
        public ActionResult Index()
        {
            ViewBag.DepartmentID = Data.DepartmentViewBag();
            return View();
        }
        public ActionResult _Search(EmployeeSearch model) {
            List<WorkdayViewModel> data = new List<WorkdayViewModel>();
            Stopwatch sw ;
            sw = Stopwatch.StartNew();
            if (model.FromDate != null && model.ToDate != null)
            {
               
                data = clsFunction.WorkDays(model, _context);
                data = data.OrderBy(it => it.DepartmentName).OrderBy(it => it.Date).ToList();
               
            }
            //Debug.WriteLine("Thời gian thực hiện" + sw.ElapsedMilliseconds.ToString());
            return PartialView(data);

        }
        #endregion Hiển thị thông tin công

        #region SaveData
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateAjax]
        [PortalAuthorization]
        public JsonResult Edit(DateTime Day, Guid EmployeeID, string Value, string Name) {
            try
            {
                var exist = _context.WorkdayEditModels.SingleOrDefault(it => it.Date1 == Day && it.EmployeeId == EmployeeID);
                if (exist != null)
                {
                   
                    EditModel(exist, Value, Name);
                    HistoryRepository _repository = new HistoryRepository(_context);
                    _repository.SaveUpdateHistory(exist.ID, CurrentUser.UserName, exist);
                    exist.LastModifiedAccountId = Guid.Parse(CurrentUser.AccountId);
                    exist.LastModifiedTime = DateTime.Now;
                }
                else
                {
                    WorkdayEditModel add = new WorkdayEditModel();
                    add.ID = Guid.NewGuid();
                    add.InDate = add.Date1 = Day;
                    add.EmployeeId = EmployeeID;
                    add.CreatedAccountId = Guid.Parse(CurrentUser.AccountId);
                    add.CreatedTime = DateTime.Now;
                    add.Actived = true;
                    EditModel(add, Value, Name);
                    _context.WorkdayEditModels.Add(add);
                }
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Workday.ToLower())
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = "Lỗi cập nhật liên hệ phòng HTTT " + ex.Message,
                });
            }
        }
        private WorkdayEditModel EditModel(WorkdayEditModel result,  string Value, string Name) {
            switch (Name.ToLower())
            {
                case "intimeedit":
                    result.InTime = TimeSpan.Parse(Value);
                    break;
                case "outdateedit":
                    result.OutDate = DateTime.Parse(Value);
                    break;
                case "outtimeedit":
                    result.OutTime = TimeSpan.Parse(Value);
                    break;
                case "worktimeedit":
                    result.WorkTime = Value;
                    break;
                case "otedit":
                    result.OT = Value;
                    break;
                case "otafter22hedit":
                    result.OTAfter22h = Value;
                    break;
                case "isnightshift":
                    result.IsNightShift = bool.Parse(Value);
                    break;
                default:
                    break;
            }
            return result;
        }
        #endregion SaveData
        #region Info cong ngày
        public ActionResult InfoWorkDay(Guid id, DateTime date)
        {
            var employee = _context.EmployeeModels.SingleOrDefault(it => it.EmployeeId == id);
            InfoWorkDay info = new InfoWorkDay();
            if (employee == null) {
                
                info.raLogs = _context.RALogs.Where(it => it.UID == employee.TimekeepingCode && it.Date == date);
                info.oTs = _context.OvertimeModels.Where(it => it.OvertimeDay == date && it.OvertimeDetailModels.Any(p => p.EmployeeId == id));
                info.conFirms = _context.ConfirmWorkingTimeModels.Where(it => it.Date1 == date && it.EmployeeId == id);
                info.workDayEdit = _context.WorkdayEditModels.Where(it => it.Date1 == date && it.EmployeeId == id);
                info.furloughs = _context.FurloughModels.Where(it => it.EmployeeId == id && it.FurloughDetailModels.Any(p => p.Del != true && p.DayOff == date));
            }
            return PartialView(info);
        }
        #endregion Info công ngày

    }
}