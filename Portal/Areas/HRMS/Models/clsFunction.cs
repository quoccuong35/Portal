using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using Portal.Constant;
using Portal.EntityModels;
using Portal.ViewModels;

namespace HRMS.Models
{
    public class clsFunction
    {
        public static bool checkKyCongNguoiDung(DateTime ngay)
        {
            using (var db = new PortalEntities())
            {
                bool b = false;
                var model = db.TimeKeepingPeriodModels.FirstOrDefault(it => it.FromDate <= ngay.Date && it.ToDate >= ngay.Date && it.Type == "1");
                if (model == null || model.Actived != true)
                {
                    b = true;
                }
                return b;
            }
        }
        public static EmployeeInfoView SearchEmployee(string employeeCode, bool bAll, PortalEntities _context)
        {
            Guid lamViec = new Guid(ConstFunction.WorkingId); // mã làm việc
            var employee = _context.EmployeeModels.Where(it => it.EmployeeCode == employeeCode && it.Actived == true && it.EmployeeStatusCategoryId == lamViec).FirstOrDefault();
            EmployeeInfoView info = new EmployeeInfoView();
            info.EmployeeId = employee.EmployeeId;
            info.EmployeeCode = employee.EmployeeCode;
            info.FullName = employee.FullName;
            info.DepartmentName = employee.Department.DepartmentName;
            info.StartTime = employee.ShiftWork.StartTime.ToString(@"hh\:mm");
            info.EndTime = employee.ShiftWork.EndTime.ToString(@"hh\:mm");
            info.RemainingLeavedays = employee.RemainingLeavedays != null ? employee.RemainingLeavedays.ToString() : String.Empty;
            return info;
        }

        public static Account GetAccount(Guid acc, PortalEntities _context)
        {
            var data = (from a in _context.Accounts
                        where a.AccountId == acc
                        && a.Actived == true
                        select a).FirstOrDefault();
            return data;
        }

        public static List<WorkdayViewModel> WorkDays(EmployeeSearch model, PortalEntities _context)
        {
            List<WorkdayViewModel> list = new List<WorkdayViewModel>();
            // Lấy thông tin nhân sự
            var employees = _context.EmployeeModels.Where(it => it.Actived == true &&
                              (model.EmployeeCode == null || it.EmployeeCode == model.EmployeeCode) &&
                               (model.FullName == null || it.FullName.Contains(model.FullName)) &&
                              (model.DepartmentID == null || it.DepartmentID == model.DepartmentID) &&
                              (it.EndDate == null || it.EndDate >= model.FromDate)
                              && it.WorkingDate <= model.ToDate).ToList();
            if (employees.Count > 0)
            {
               
                // Lấy thong tin ngày lễ
                var holidays = _context.HoliDayModels.Where(it => it.Date1 >= model.FromDate && it.Date1 <= model.ToDate).ToList();


                WorkdayViewModel result;
                DateTime date , ToDateAdd = model.ToDate.Value.AddDays(1).Date;

                var listWork = (from o in _context.RALogs
                                where o.Del != true
                                && DbFunctions.TruncateTime(o.Date).Value >= DbFunctions.TruncateTime(model.FromDate).Value && DbFunctions.TruncateTime(o.Date).Value <= ToDateAdd
                                &&  ( model.EmployeeCode == null ||   o.UID == model.EmployeeCode)
                                group o by new { o.UID, Date = DbFunctions.TruncateTime(o.Date).Value }
                                                        into grp
                                select new DLVT
                                {
                                    UID = grp.Key.UID,
                                    Date = grp.Key.Date,
                                    InTime = grp.Min(o => o.Time),
                                    OutTime = grp.Max(o => o.Time)
                                }).AsQueryable().ToList();

                // Lấy  thông tin phép
                var furloughs = _context.FurloughModels.Where(it => it.BrowseStatusID == "3" 
                                && it.FurloughDetailModels.Any(p => p.Del != true)
                                && it.FurloughDetailModels.Any(p => p.DayOff >= model.FromDate && p.DayOff <= model.ToDate)
                                && (model.EmployeeCode == null || it.EmployeeModel.EmployeeCode == model.EmployeeCode)
                                && (model.FullName == null || it.EmployeeModel.FullName.Contains(model.FullName))
                                && (model.DepartmentID == null || it.EmployeeModel.DepartmentID == model.DepartmentID)
                                ).ToList();

                // lấy tăng ca
                var overTimes = _context.OvertimeModels.Where(it => it.BrowseStatusID == "3"
                                 && it.OvertimeDay >= model.FromDate && it.OvertimeDay <= ToDateAdd
                                && it.OvertimeDetailModels.Any(p => p.Del != true)
                                && (model.EmployeeCode == null || it.OvertimeDetailModels.Any(p=>p.EmployeeModel.EmployeeCode == model.EmployeeCode))
                                 && (model.FullName == null || it.OvertimeDetailModels.Any(p => p.EmployeeModel.FullName.Contains( model.FullName)))
                                && (model.DepartmentID == null || it.OvertimeDetailModels.Any(p => p.EmployeeModel.DepartmentID == model.DepartmentID))
                                ).ToList();

                // lay thông tin xác nhận công

                var confirmWorks = _context.ConfirmWorkingTimeModels.Where(it => it.BrowseStatusID == "3"
                                            && it.Date1 >= model.FromDate && it.Date1 <= ToDateAdd
                                            && (model.EmployeeCode == null || it.EmployeeModel.EmployeeCode == model.EmployeeCode)
                                            && (model.FullName == null || it.EmployeeModel.FullName.Contains(model.FullName))
                                            && (model.DepartmentID == null || it.EmployeeModel.DepartmentID == model.DepartmentID)
                                    ).ToList();
                // công hiệu chỉnh
                var listWorkHC = _context.WorkdayEditModels.Where(it => it.Actived == true 
                                                                    && it.Date1 >= model.FromDate && it.Date1 <= ToDateAdd 
                                                                    && (model.EmployeeCode == null || it.EmployeeModel.EmployeeCode == model.EmployeeCode)
                                                                    && (model.FullName == null || it.EmployeeModel.FullName.Contains(model.FullName))
                                                                    && (model.DepartmentID == null || it.EmployeeModel.DepartmentID == model.DepartmentID)).ToList();

                foreach (var ns in employees)
                {
                   
                    

                    date = model.FromDate.Value.Date;
                    while (date <= model.ToDate.Value.Date)
                    {
                        if (date > DateTime.Now.Date)
                            break;
                        result = new WorkdayViewModel();
                        var cong = listWork.FirstOrDefault(it => it.Date == date && it.UID == ns.EmployeeCode);
                        var phep = furloughs.Where(it => it.EmployeeId == ns.EmployeeId && it.FurloughDetailModels.Any(p => p.DayOff == date && p.Del != true)).ToList();

                      
                        var workHC = listWorkHC.SingleOrDefault(it => it.Date1 == date && it.EmployeeId == ns.EmployeeId);
                        var ot = overTimes.Where(it => it.OvertimeDay == date && it.OvertimeDetailModels.Any(p => p.EmployeeId == ns.EmployeeId)).OrderBy(it => it.OvertimeStartTime).ToList();
                        var confirm = confirmWorks.Where(it => it.EmployeeId == ns.EmployeeId && it.Date1 == date).ToList();


                        result.FullName = ns.FullName;
                        result.EmployeeId = ns.EmployeeId;
                        result.EmployeeCode = ns.EmployeeCode;
                        result.TimekeepingCode = ns.TimekeepingCode;
                        // lấy thông tin ca làm việc
                        GetShiftWorkDay(result, ns.ShiftWork);

                        result.DayOfWeek = GetDayOfWeek(date);

                        result.DepartmentName = ns.Department.DepartmentName;
                        result.InDate = date;
                        result.OutDate = date;
                        result.Date = date;
                     
                        if (date.DayOfWeek == DayOfWeek.Saturday)
                        {
                            result.Saturday = SaturdayTh(date, _context);
                        }

                        var holiday = holidays.SingleOrDefault(it => it.Date1 == date);
                        result.StatusWordday = GetStatusWorkday(date, phep, ns, holiday);

                        if (ns.EmployeeSpecialModel != null && ns.EmployeeSpecialModel.Type1 == 1)
                        {
                            /// nhân sự đặc biệt không chấm vâng tay này ko có tính tăng ca
                            if (date.Date <= DateTime.Now.Date)
                            {
                                result.OutDate = date;
                                if (ns.StandardWorkingDay == 24)
                                {
                                    if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)//Thứ 2->6
                                    {
                                        result.WorkTime = 8;
                                    }
                                    else if (date.DayOfWeek == DayOfWeek.Saturday)//thứ 7
                                    {
                                        //if (objCongNgay.TuanThuMayCuaThang == 1 || objCongNgay.TuanThuMayCuaThang == 3)
                                        if (result.Saturday == 1 || result.Saturday == 3)
                                        {
                                            //Thứ 7 tuần 1 và 3
                                            result.WorkTime = 8;
                                        }
                                    }
                                }
                                else if (ns.StandardWorkingDay == 26)
                                {
                                    if (date.DayOfWeek != DayOfWeek.Sunday)//Thứ 2->7
                                    {
                                        result.WorkTime = 8;
                                    }
                                }
                            }
                        }
                        else
                        {
                            WorkDay(date, result, workHC, ns, cong, ot, confirm, holiday,phep);
                        }
                        result.LateOrEarly = false;
                        date = date.AddDays(1);
                        list.Add(result);

                    }
                }

            }
            // Lấy thông tin công
            return list;
        }
        private static void WorkDay(DateTime date, WorkdayViewModel result, WorkdayEditModel workHC, EmployeeModel ns, DLVT cong, List<OvertimeModel> ot, List<ConfirmWorkingTimeModel> confirmWorks,HoliDayModel holiday,List<FurloughModel> phep)
        {
            bool cD = false;
            TimeSpan inTime = new TimeSpan(0, 0, 0), outTime = new TimeSpan(0, 0, 0);
            if (workHC != null)
            {
                result.Status = "HC";
                // kiem tra gio vào hiệu chỉnh
                if (workHC.InTime != null)
                {
                    result.InTime = workHC.InTime;
                    result.InTimeEdit = workHC.InTime.Value.ToString(@"hh\:mm");
                }
                if (workHC.OutTime != null)
                {
                    result.OutTime = result.OutTimeEdit = workHC.OutTime;
                }
                if (workHC.OutDate != null)
                {
                    result.OutDate = workHC.OutDate.Value;
                    result.OutDateEdit = result.OutDate;
                }
                if (workHC.WorkTime != null && workHC.WorkTime != "")
                {

                    result.WorkTimeEdit = workHC.WorkTime;
                }
                if (workHC.OT != null && workHC.OT != "")
                {

                    result.OTEdit = workHC.OT;
                }
                if (workHC.OTAfter22h != null && workHC.OTAfter22h != "")
                {

                    result.OTAfter22h = double.Parse(workHC.OTAfter22h);
                    if (workHC.IsNightShift == true)
                    {
                        cD = true;
                    }
                }
            }
            if (result.InTime == null)
            {
                var xnc = confirmWorks.FirstOrDefault(it => it.Type1 == "1");
                if (xnc != null)
                {
                    // Có Xác nhận công giờ vào
                    result.InTime = xnc.Time1;
                    result.InTimeCSS = "CoXNC";
                    result.InDate = xnc.Date1;
                    if (xnc.IsNightShift == true)
                        cD = true;
                }
                else
                {
                    // Không có xác nhận công.
                    result.InDate = date;
                    if (cong != null && cong.InTime != "")
                    {
                        result.InTime = TimeSpan.Parse(cong.InTime);

                    }
                    else if (ns.WorkingDate == date && cong == null)
                    {
                        // Ngày đầu nhận việc ko có chấm công
                        result.InTime = ns.ShiftWork.StartTime;
                    }
                }
                result.OutDate = result.InDate;
            }
            // Gio ra
            if (result.OutTime == null)
            {
                if (cD)
                {
                    // dữ liệu ca đêm

                }
                else
                {
                    // Ca ngày
                    var xnc = confirmWorks.FirstOrDefault(it => it.Type1 == "2");
                    if (xnc != null)
                    {
                        // Có Xác nhận công giờ vào
                        result.OutTime = xnc.Time1;
                        result.OutDate = xnc.Date1;
                        result.OutTimeCSS = "CoXNC";
                    }
                    else
                    {
                        // Không có xác nhận công.

                        if (cong != null && cong.OutTime != "")
                        {
                            result.OutTime = TimeSpan.Parse(cong.OutTime);
                            result.OutDate = cong.Date.Value;
                        }
                        else if (ns.WorkingDate == date && cong == null)
                        {
                            // Ngày đầu nhận việc ko có chấm công
                            result.InTime = ns.ShiftWork.EndTime;
                            result.OutDate = date;
                        }
                    }
                }
            }
            // Kiểm tra nếu là đc đi làm trễ
            if (result.InTime != null && result.OutTime != null)
            {

                double breakTime = 0, standardHour = 0;
                if (ns.EmployeeSpecialModel != null && ns.EmployeeSpecialModel.Type1 == 2)
                {
                    inTime = result.InTime.Value;
                    outTime = result.OutTime.Value;
                    if (ns.EmployeeSpecialModel.Late == true && ns.EmployeeSpecialModel.Minute1 > 0 && (result.InTime.Value - result.StartTime).TotalMinutes <= ns.EmployeeSpecialModel.Minute1)
                    {
                        inTime = result.StartTime;
                    }
                }
                else if (ns.EmployeeSpecialModel == null)
                {
                    inTime = ns.ShiftWork.StartTime;
                    if (result.InTime >= inTime)
                    {
                        inTime = result.InTime.Value;
                        if (!cD)
                        {
                            result.LateOrEarly = true;
                        }
                    }
                    outTime = ns.ShiftWork.EndTime;
                    if (result.OutTime < outTime)
                    {
                        outTime = result.OutTime.Value;
                        if (!cD)
                        {
                            result.LateOrEarly = true;
                        }
                    }

                }
                result.Hour = Math.Floor((outTime - inTime).TotalHours);
                if (result.Hour < 0)
                {
                    result.Hour = 0;
                }
                result.Minute = Math.Floor((outTime - inTime).TotalMinutes - (result.Hour.Value * 60) + 0.5);
                if (result.Minute < 0)
                {
                    result.Minute = 0;
                }
                if (!cD)
                {
                    breakTime = BreakTime(ns, inTime, outTime);


                    if (ns.ShiftWork != null && ns.ShiftWork.NumberMinuteLate > 0)
                    {
                        // tính giờ chuẩn khi dc tính trễ theo số phút.
                        standardHour = StandardHour(result);
                    }
                    result.TotalWH = Math.Round((outTime - inTime).TotalHours, 2);
                    if (result.TotalWH.Value < 0)
                        result.TotalWH = 0;
                    // tính công
                    WorkTime(result, ns, breakTime, standardHour);

                    if (result.OutTime > result.EndTime)
                    {
                        result.OutOfficeHours = Math.Round((result.OutTime.Value - result.EndTime).TotalHours, 2);
                        result.OutOfficeHours = result.OutOfficeHours > 0.5 ? result.OutOfficeHours : null;
                    }

                    // xử lý tang ca
                    HandleOT(result, ot, ns, date);
                    // các trường hợp xóa công
                    if (date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        // chủ nhật tính qua tăng ca
                        result.WorkTime = null;
                    }
                    else if (ns.StandardWorkingDay == 22 && (date.DayOfWeek == DayOfWeek.Saturday))
                    {
                        //  thứ 7 đối với 22 công 
                        result.WorkTime = null;
                    }
                    else if (ns.StandardWorkingDay == 24 && (result.Saturday == 2 || result.Saturday == 4 || result.Saturday == 5))
                    {
                        // các thứ 7 tuần 2,4,5 ko tính công
                        result.WorkTime = null;
                    }
                    else if (result.StatusWordday == "L")
                    {
                        // lễ có đi làm ko tính công, qua tăng ca
                        result.WorkTime = null;
                    }
                    else if (phep.Count > 0)
                    {
                        double leaveNumber = 0;
                        foreach (var item in phep)
                        {
                            var checkExist = item.FurloughDetailModels.Where(it => it.DayOff == date && it.Del != true).ToList();
                            if (checkExist.Count > 0)
                            {
                                leaveNumber += checkExist.Sum(it => it.NumberDay);
                            }
                        }
                        if (leaveNumber > 0.5)
                        {
                            result.WorkTime = null;
                        }
                    }

                }
            }
        }
        private static void GetShiftWorkDay( WorkdayViewModel result, ShiftWork shiftWork ) {
            result.StartTime = shiftWork.StartTime;
            result.EndTime = shiftWork.EndTime;
            result.EndTimeBetweenShift = shiftWork.EndTimeBetweenShift;
            result.StartTimeBetweenShift = shiftWork.StartTimeBetweenShift;
            result.OvertimeStartTime = shiftWork.OvertimeStartTime;
            result.NumberMinuteLate = shiftWork.NumberMinuteLate;
            result.StandardHour = shiftWork.StandardHour;
            result.ShiftWorkId = shiftWork.ShiftWorkId;
            result.TotalWorkTime = shiftWork.TotalWorkTime;
            
        }
        private static void WorkTime(WorkdayViewModel result, EmployeeModel ns, double breakTime, double standardHour)
        {
            TimeSpan inTime = result.InTime.Value, outTime = result.OutTime.Value;
            if (outTime > result.EndTime)
            {
                outTime = result.EndTime;
            }
            result.WorkTime = Math.Round((outTime - inTime).TotalHours,2);
            if (result.WorkTime.Value >= standardHour)
            {
                result.WorkTime = result.StandardHour;
            }
            else
            {
                result.WorkTime = result.WorkTime - breakTime;
            }
            result.WorkTime = result.WorkTime < 0 ? 0 : result.WorkTime;
        }

        // nghỉ giải lao
        private static double BreakTime(EmployeeModel ns, TimeSpan inTime, TimeSpan outTime)
        {
            double breakTime = 0;
            TimeSpan startTimeBetweenShift = ns.ShiftWork.StartTimeBetweenShift, endTimeBetweenShift = ns.ShiftWork.EndTimeBetweenShift;
            //tính trừ thời gian nghỉ giữa ca
            if (outTime <= endTimeBetweenShift || inTime >= startTimeBetweenShift)
                breakTime = 0;
            else if (inTime >= endTimeBetweenShift && inTime <= startTimeBetweenShift)
            {
                breakTime = Math.Round((startTimeBetweenShift - inTime).TotalHours, 2);
            }
            else if (outTime >= endTimeBetweenShift && outTime <= startTimeBetweenShift)
            {
                breakTime = Math.Round((outTime - endTimeBetweenShift).TotalHours, 2);
            }
            else
            {
                breakTime = Math.Round((startTimeBetweenShift - endTimeBetweenShift).TotalHours, 2);
            }

            return breakTime;
        }

        // giờ công chuẩn sao khi trừ giờ đi trễ
        private static double StandardHour(WorkdayViewModel result)
        {
            return Math.Round((result.EndTime - result.StartTime).TotalHours - (result.NumberMinuteLate * 1.0 / 60), 2);
        }

        private static void HandleOT(WorkdayViewModel result, List<OvertimeModel> ot, EmployeeModel ns, DateTime date)
        {
            /// chia làm 3 trường hợp
            if (ot.Count > 0)
            {
                TimeSpan inTime, outTime;
                double breakTime = 0, oT = 0, oTAfter22H = 0;

                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    // chủ nhật
                    inTime = result.StartTime;

                }
                else if (ns.StandardWorkingDay == 24 && (result.Saturday == 2 || result.Saturday == 4 || result.Saturday == 5))
                {
                    /// tính tăng cả ngày thức 7 cho 24 công đối với thứ 7 tuần 2,4,5
                    inTime = result.StartTime;
                }
                else if (ns.StandardWorkingDay == 2 && date.DayOfWeek == DayOfWeek.Saturday)
                {
                    inTime = result.StartTime;
                }
                else if (result.StatusWordday == "L")
                {
                    inTime = result.StartTime;
                }
                else
                {
                    inTime = result.OvertimeStartTime;
                }


                foreach (var item in ot)
                {
                    breakTime = 0;
                    outTime = item.OvertimeEndTime;
                    // kiểm tra giờ vào sao với giờ chấm
                    if (inTime < result.InTime)
                    {
                        // nếu cận vào mà nhỏ hơn giờ chấm vào thì lấy giờ chấm vào
                        inTime = result.InTime.Value;
                    }
                    if (item.OvertimeStartTime > inTime)
                    {
                        // nếu cận vào nhỏ hơn giờ bắt đầu tính tăng ca thì lấy giờ bắt đầu tính tăng ca
                        inTime = item.OvertimeStartTime;
                    }
                    if (outTime > result.OutTime)
                    {
                        // nếu giờ tang ca lớn hơn giờ chấm thì lấy giờ chấm
                        outTime = result.OutTime.Value;
                    }
                    breakTime = BreakTime(ns, inTime, outTime);
                    /// check nếu có thời gian tăng ca sau 22h
                    if (outTime >= new TimeSpan(22, 00, 00))
                    {
                        oTAfter22H += Math.Round((outTime - (new TimeSpan(22, 00, 00))).TotalHours, 2);
                    }
                    oT += Math.Round((outTime - inTime).TotalHours, 2) - breakTime;
                }
                result.OT = oT - oTAfter22H;
                if (result.OT < 0.5)
                {
                    result.OT = null;
                }
                result.OTAfter22h = oTAfter22H;
                if (result.OTAfter22h < 0.5)
                {
                    result.OTAfter22h = null;
                }
            }

        }

        private static string GetStatusWorkday(DateTime date, List<FurloughModel> phep, EmployeeModel ns, HoliDayModel holiday)
        {
            /// M new employee
            /// NV: quit one's job
            /// L: Holiday
            /// other leave category
            string result = "";
            if (date < ns.WorkingDate && date.DayOfWeek != DayOfWeek.Sunday)
            {
                // Nhân sự nhận việc trong tháng;
                result = "M";
            }
            else if (date > ns.EndDate && date.DayOfWeek != DayOfWeek.Sunday)
            {
                // nhân sự nghỉ việc trong tháng
                result = "NV";
            }
            else if (holiday != null)
            {
                result = "L";
            }
            else if (phep.Count > 0)
            {
                foreach (var item in phep)
                {
                    var checkDay = item.FurloughDetailModels.SingleOrDefault(it => it.DayOff == date);
                    if (checkDay != null)
                    {
                        result += item.LeaveCategory.LeaveCategoryCode + "|";
                        if (checkDay.NumberDay > 1)
                        {
                            result += item.LeaveCategory.LeaveCategoryCode + "/2" + "|";
                        }
                    }
                }
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }
        private static string GetDayOfWeek(DateTime date)
        {
            string result = "";
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    result = "Chủ nhật";
                    break;
                case DayOfWeek.Monday:
                     result =  "Thứ 2";
                    break;
                case DayOfWeek.Tuesday:
                    result = "Thứ 3";
                    break;
                case DayOfWeek.Wednesday:
                    result = "Thứ 4";
                    break;
                case DayOfWeek.Thursday:
                    result = "Thứ 5";
                    break;
                case DayOfWeek.Friday:
                    result = "Thứ 6";
                    break;
                case DayOfWeek.Saturday:
                    result = "Thứ 7";
                    break;
                default:
                    break;
            }
            return result;
        }
        private static int SaturdayTh(DateTime date, PortalEntities _context)
        {
            int kq = 0;
            var timePeriod = _context.TimeKeepingPeriodModels.FirstOrDefault(it => it.FromDate <= date && it.ToDate >= date && it.Type == "2");
            if (timePeriod != null)
            {
                DateTime ngay = timePeriod.FromDate.Date;
                while (ngay <= date)
                {
                    if (ngay.DayOfWeek == DayOfWeek.Saturday)
                    {
                        kq++;
                    }
                    ngay = ngay.AddDays(1);
                }
            }
            return kq;
        }
    }
    public class DLVT
    {
        public string UID { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> Thu { get; set; }
    }
}