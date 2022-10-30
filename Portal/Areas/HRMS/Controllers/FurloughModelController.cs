using Portal.EntityModels;
using Portal.Extensions;
using Portal.Resources;
using Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using HRMS.Models;
using System.Transactions;
using Portal.Repositories;
using System.Text;
using System.Web.Mvc.Html;

namespace HRMS.Controllers
{
    public class FurloughModelController : BaseController
    {
        // GET: FurloughModel nghỉ phép
       
        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(FurloughModel search)
        {
            Guid accountId = new Guid(CurrentUser.AccountId);

            if (search.FromDate.Date == new DateTime(0001, 01, 01))
            {
                search.FromDate = FirstDay();
            }
            if (search.ToDate.Date == new DateTime(0001, 01, 01))
            {
                search.ToDate = LastDay();
            }
            var furloughs = _context.FurloughModels.Where(it => it.CreatedAccountId == accountId
                && it.FurloughDetailModels.Any(p=>p.DayOff>=search.FromDate && p.DayOff <= search.ToDate && p.Del !=true)).OrderByDescending(it=>it.ToDate).ToList();
            return View(furloughs);
        }
        #endregion Index

        #region Create
        [PortalAuthorization]
        public ActionResult Create()
        {
            Guid lamView = new Guid("F3827595-B7AA-457F-92D9-80B0E9DF458A");
            var employee = _context.EmployeeModels.FirstOrDefault(it => it.EmployeeCode == CurrentUser.EmployeeCode && it.Actived == true && it.EmployeeStatusCategoryId == lamView);
            EmployeeInfoView data = new EmployeeInfoView();
            data.DepartmentName = employee.Department.DepartmentName;
            data.EmployeeCode = employee.EmployeeCode;
            data.FullName = employee.FullName;
            data.EmployeeId = employee.EmployeeId;
            data.RemainingLeavedays = employee.RemainingLeavedays!=null? employee.RemainingLeavedays.ToString():String.Empty;

            FurloughViewModel furlough = new FurloughViewModel();
            furlough.FromDate = furlough.ToDate = DateTime.Now;

            furlough.Employee = data;
         
            List<FurloughDetailViewModel> detail = new List<FurloughDetailViewModel>();
            List<ApprovalHistoryModel> app = new List<ApprovalHistoryModel>();
            //detail.Add(new FurloughDetailViewModel() { Check = true, TypeDate = "1", DayOff = DateTime.Now.Date, Note = "" });
            //ViewBag.TypeDate = FurloughFunction.ListLeaveType();
            ViewBag.LeaveCategoryId = Data.LeaveCategorieBag();
            furlough.FurloughDetail = detail;
            furlough.AppHistory = app;
            return View(furlough);
        }
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Create(FurloughViewModel model)
        {
            return ExecuteContainer(() =>
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    List<string> errorList = new List<string>();

                    if (model.FurloughDetail.Count > 0)
                    {
                        if (clsFunction.checkKyCongNguoiDung(model.FromDate))
                        {
                            errorList.Add(string.Format(LanguageResource.CheckFeat, LanguageResource.FurloughModel.ToLower()));
                        }
                        if (model.Employee.EmployeeId == null)
                        {
                            errorList.Add("Chưa có thông tin nhân sự nghỉ phép");
                        }
                        Guid accountID = new Guid(CurrentUser.AccountId);
                        var account = _context.Accounts.FirstOrDefault(it => it.AccountId == accountID);
                        double daysOfLeave = 0.0;
                        var employee = _context.EmployeeModels.FirstOrDefault(it => it.EmployeeId == model.Employee.EmployeeId);
                        FurloughModel add = new FurloughModel();
                        add.FurloughId = Guid.NewGuid();
                        add.CreatedAccountId =  accountID;
                        add.CreatedTime = DateTime.Now;
                        add.FromDate = model.FromDate;
                        add.ToDate = model.ToDate;
                        add.Reason = model.Reason;
                        add.LeaveCategoryId = model.LeaveCategoryId.Value;
                        add.EmployeeId = model.Employee.EmployeeId;
                        add.BrowseStatusID = "1";
                        add.Lock = false;
                        add.EmployeeModel = employee;
                        List<FurloughDetailModel> addDetails = new List<FurloughDetailModel>();
                        List<ApprovalHistoryModel> list = new List<ApprovalHistoryModel>();
                        double dDay = 0;
                        foreach (var item in model.FurloughDetail)
                        {
                            if (item.Check == null || item.Check == false)
                                continue;
                            if (item.TypeDate == "1")
                            {
                                dDay = 1;
                            }
                            else
                            {
                                dDay = 0.5;
                            }
                            // Kiểm tra tồn tại
                            var checkSubsist = (from a in _context.FurloughModels
                                                join b in _context.FurloughDetailModels on a.FurloughId equals b.FurloughId
                                                where a.BrowseStatusID != "4" && b.Del != true
                                                 && a.EmployeeId == add.EmployeeId
                                                 && b.DayOff == item.DayOff
                                                select b).SingleOrDefault();
                            if (checkSubsist!= null)
                            {
                                if (item.TypeDate == "1" || checkSubsist.TypeDate == "1" || item.TypeDate == checkSubsist.TypeDate)
                                {
                                    errorList.Add("Đã tồn tại thông tin nghỉ phép ngày " + item.DayOff.ToString("dd/MM/yyyy"));
                                }
                            }
                            daysOfLeave += dDay;
                            addDetails.Add(new FurloughDetailModel { FurloughId = add.FurloughId, DayOff = item.DayOff, TypeDate = item.TypeDate, NumberDay = dDay });
                        }
                        if (daysOfLeave % 1 > 0 && add.LeaveCategoryId != new Guid("44F08DD8-DC39-4A63-A8B9-E731B10A0368") && add.LeaveCategoryId != new Guid("D8EB0F0A-C7BA-477C-A39D-E25EE7071167"))
                        {
                            errorList.Add("Loại phép bạn chọn không được nghỉ 0.5 ngày");
                        }
                       
                        add.NumberOfDaysOff = daysOfLeave;
                        // Nếu là Nghỉ phép P xem có đủ ngày phép nghỉ không.
                        if (add.LeaveCategoryId == new Guid("44F08DD8-DC39-4A63-A8B9-E731B10A0368"))
                        {
                            
                            double remainingLeavedays = employee.RemainingLeavedays == null ? 0 : employee.RemainingLeavedays.Value;
                            if (daysOfLeave > remainingLeavedays)
                            {
                                errorList.Add("Số ngày phép bạn yêu cầu nghỉ được hưởng lương vượt quá ngày phép còn lại");
                            }
                            else
                            {
                                // trừ ngày phép
                                add.EmployeeModel.RemainingLeavedays = add.EmployeeModel.RemainingLeavedays - daysOfLeave;
                            }
                        }

                        if (account.EmployeeModel == null)
                        {
                            errorList.Add("Người dùng chưa có liên kết nhân sự");
                        }
                       
                        if (account.EmployeeModel != null && account.EmployeeModel.EmployeeModel2 == null)
                        {
                            errorList.Add("Người tạo chưa có cấp quản lý trực tiếp");
                        }

                        if (account.EmployeeModel != null && account.EmployeeModel.Department != null && account.EmployeeModel.EmployeeModel2 != null)
                        {
                            // Luu History apprival
                            var checkExitApproval = _context.MaTrixFurloughModels.Where(it => it.DepartmentID == account.EmployeeModel.DepartmentID && it.FromDay < add.NumberOfDaysOff && it.ToDay > add.NumberOfDaysOff && it.Actived == true).OrderBy(it => it.ApprovalLevel).ToList();
                            if (checkExitApproval.Count == 0)
                            {
                                errorList.Add("Chưa có quy trình duyệt cho đơn vị " + account.EmployeeModel.Department.DepartmentName);
                            }
                            else
                            {
                                // Add quy trình duyệt
                                int i = 1;
                                string[] teamAppro;
                                string status = "";
                                foreach (var item in checkExitApproval)
                                {
                                    teamAppro = item.ApprovalName.Split(';');
                                    if (i == 1)
                                    {
                                        status = "2";
                                    }
                                    else
                                    {
                                        status = "1";
                                    }
                                    // có tồn tại hơn 2 người duyệt 1 bước
                                    if (teamAppro.Length > 0)
                                    {
                                        foreach (var str in teamAppro)
                                        {
                                            if (str == null || str == "")
                                                continue;

                                            AddHistoryMaTrix(str, item, list, account, errorList, accountID, add.FurloughId, i, status,item.ApprovalLevel);
                                        }
                                    }
                                    else
                                    {
                                        AddHistoryMaTrix(item.ApprovalName, item, list, account, errorList, accountID, add.FurloughId, i, status, item.ApprovalLevel);
                                    }
                                    i++;
                                }
                            }
                        }
                        if (errorList != null && errorList.Count > 0)
                        {
                            return Json(new
                            {
                                Code = System.Net.HttpStatusCode.Created,
                                Success = false,
                                Data = errorList
                            });
                        }
                        ManyToMany(add, addDetails,list);
                        _context.Entry(add).State = EntityState.Added;
                        _context.SaveChanges();
                        ts.Complete();
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.Created,
                            Success = true,
                            Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.FurloughModel.ToLower())
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.Created,
                            Success = true,
                            Data = string.Format(LanguageResource.CheckData, LanguageResource.FurloughModel.ToLower())
                        });
                    }

                }
            });
        }

        public ActionResult AddDays(DateTime FromDate, DateTime ToDate)
        {
            TimeSpan day = ToDate - FromDate;
            int idays = day.Days;
            List<FurloughDetailViewModel> days = new List<FurloughDetailViewModel>();
            DateTime temp2, temp = FromDate;
            string note = "";
            for (int i = 0; i < idays + 1; i++)
            {
                note = "";
                temp2 = temp.AddDays(i);
                if (temp2.DayOfWeek == DayOfWeek.Sunday)
                    continue;
                if ((temp2.DayOfWeek == DayOfWeek.Saturday))
                    note = "Thứ 7";

                days.Add(new FurloughDetailViewModel() { Check = true, DayOff = temp2, Note = note, TypeDate = "1" });
            }
            ViewBag.TypeDate = FurloughFunction.ListLeaveType();
            return PartialView("_FurloughDetailInfo", days);
        }
        #endregion Create

        #region Edit
        [PortalAuthorization]
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var edit = _context.FurloughModels.FirstOrDefault(it => it.FurloughId == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.FurloughModel.ToLower()) });
            }
            var model = FurloughFunction.getDatTa(edit, CurrentUser.AccountId);
            ViewBag.TypeDate = FurloughFunction.ListLeaveType();
            ViewBag.LeaveCategoryId = Data.LeaveCategorieBag(model.LeaveCategoryId);
            return View(model);

        }
        [PortalAuthorization]
        [HttpPost]
        [ValidateAjax]
        public JsonResult Edit(FurloughViewModel model,string type = null)
        {
            return ExecuteContainer(() =>
            {
                var edit = _context.FurloughModels.FirstOrDefault(it => it.FurloughId == model.FurloughId);
              
                List<string> errorList = new List<string>();
                if (edit != null)
                {
                    FurloughFunction.UpData(model, edit, errorList, false,new Guid(CurrentUser.AccountId));

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
                _repository.SaveUpdateHistory(edit.FurloughId, CurrentUser.UserName, edit);

                edit.LastModifiedAccountId = new Guid(CurrentUser.AccountId);
                edit.LastModifiedTime = DateTime.Now;
                _context.Entry(edit).State = EntityState.Modified;

                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.FurloughModel.ToLower())
                });
            });
            
        }

        #endregion Edit

        #region Del 
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var del = _context.FurloughModels.FirstOrDefault(p => p.FurloughId == id && p.Lock != true && p.BrowseStatusID == "1" && p.CreatedAccountId == new Guid(CurrentUser.AccountId));
                if (del != null)
                {
                   
                    var employee = _context.EmployeeModels.FirstOrDefault(it => it.EmployeeId == del.EmployeeId);
                    if (del.LeaveCategory.LeaveCategoryCode.ToLower() == "p")
                    {
                        // nghỉ phép của năm trả phép lại cho nhân sự
                       
                        if (employee != null)
                        {
                            employee.RemainingLeavedays = employee.RemainingLeavedays + del.FurloughDetailModels.ToList().Where(it => it.Del != true).Select(it => it.NumberDay).Sum();
                        }
                        
                    }
                    if (del.FurloughDetailModels.Count > 0)
                    {
                        del.FurloughDetailModels.Clear();
                    }
                    var hisApp = _context.ApprovalHistoryModels.Where(it => it.ApprovalId == del.FurloughId && it.Type1 == Portal.Constant.ConstFunction.NghiPhep).ToList();
                    if (hisApp != null && hisApp.Count > 0)
                    {
                        _context.ApprovalHistoryModels.RemoveRange(hisApp);
                    }
                    _context.Entry(del).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.FurloughModel.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.FurloughModel.ToLower())
                    });
                }
            });
        }
        #endregion Del

        #region SendMail

        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult SendEmail(Guid idSendEmail)
        {
            using (var tran = new TransactionScope())
            {
                try
                {
                    var model = _context.FurloughModels.FirstOrDefault(it => it.FurloughId == idSendEmail);
                    Guid accountID = new Guid(CurrentUser.AccountId);
                    var account = _context.Accounts.FirstOrDefault(it => it.AccountId == accountID);
                    string cc = account.EmployeeModel.CompanyEmail, to = "", receiver = ""; ;
                    if (model != null)
                    {
                        // kiểm tra xem kỳ công có khóa chưa
                        if (clsFunction.checkKyCongNguoiDung(model.FromDate))
                        {
                            return Json(new
                            {
                                Code = System.Net.HttpStatusCode.NotModified,
                                Success = false,
                                Data = string.Format(LanguageResource.CheckFeat, LanguageResource.SendMail.ToLower())
                            });
                        }
                        if (model.Lock == null || !model.Lock.Value)
                        {
                            model.Lock = true;
                            model.BrowseStatusID = "2";
                            _context.SaveChanges();
                            
                        }
                       
                        // lấy to và cc đã lưu quy trình duyệt

                        var check = model.ApprovalHistoryModels.Where(it => it.BrowseStatusID == "2").ToList();
                        if (check.Count > 0)
                        {
                            foreach (var item in check)
                            {
                                to += item.EmployeeModel.CompanyEmail + ",";
                                receiver += item.EmployeeModel.FullName + ",";
                            }
                        }

                        if (to != "")
                        {
                            to = to.Substring(0, to.Length - 1);
                            receiver = receiver.Substring(0, receiver.Length - 1);
                            string body = FurloughFunction.EmailContentsFurlough(receiver, model.LeaveCategory.LeaveCategoryName, model, "Anh/Chị có yêu cầu duyệt nghỉ phép được gửi từ <b>" + account.EmployeeModel.FullName + "</b>");
                            if (body != "" && FunctionExtensions.SendMail(LanguageResource.TitleFurlough, to, cc, body))
                            {
                                tran.Complete();
                                return Json(new
                                {
                                    Code = System.Net.HttpStatusCode.NotModified,
                                    Success = true,
                                    Data = LanguageResource.Sendmail_Success
                                });
                              
                            }
                            else
                            {
                                return Json(new
                                {
                                    Code = System.Net.HttpStatusCode.NotModified,
                                    Success = false,
                                    Data = LanguageResource.Sendmail_Error
                                });
                            }

                        }
                        else
                        {
                            return Json(new
                            {
                                Code = System.Net.HttpStatusCode.NotModified,
                                Success = false,
                                Data = "Không tìm thấy thông tin mail gửi đi"
                            });
                        }

                    }
                    else
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.NotModified,
                            Success = false,
                            Data = string.Format(LanguageResource.Error_NotExist, LanguageResource.FurloughModel.ToLower())
                        });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = ex.Message.ToString()
                    });
                }
                
            }
        }

        public void AddHistoryMaTrix(string apprval,MaTrixFurloughModel matrix, List<ApprovalHistoryModel> list, Account acc,List<string> errorList, Guid accID,Guid apprvalId,int index, string status,int approvalLevel)
        {
            if (apprval.ToLower() == "head")
            {
                if (list.Where(it => it.EmployeeId == acc.EmployeeModel.ParentId.Value && it.ApprovalLevel == approvalLevel).ToList().Count > 0)
                {
                    errorList.Add("Đã tồn tại người duyệt có mã " + acc.EmployeeModel.EmployeeCode + " ở bước " + index.ToString() + " không thể thêm");
                }
                list.Add(new ApprovalHistoryModel()
                {
                    Type1 = Portal.Constant.ConstFunction.NghiPhep,
                    EmployeeId = acc.EmployeeModel.ParentId.Value,
                    ApprovalLevel = matrix.ApprovalLevel,
                    BrowseStatusID = status,
                    CreatedAccountId = accID,
                    CreatedTime = DateTime.Now,
                    MaTrixId = matrix.MaTrixFurloughID,
                    ApprovalId = apprvalId,
                    Id = Guid.NewGuid()
                });
            }
            else
            {
                Guid lamViec = new Guid("F3827595-B7AA-457F-92D9-80B0E9DF458A");
                var checkEmployee = _context.EmployeeModels.FirstOrDefault(it => it.EmployeeCode == apprval && it.EmployeeStatusCategoryId == lamViec);
                if (checkEmployee == null)
                {
                    errorList.Add("Không tồn tại người duyệt có mã " + apprval);
                }
                if (checkEmployee != null && list.Where(it => it.EmployeeId == checkEmployee.EmployeeId && it.ApprovalLevel == approvalLevel).ToList().Count > 0)
                {
                    errorList.Add("Đã tồn tại người duyệt có mã " + acc.EmployeeModel.EmployeeCode + " ở bước " + index.ToString() + " không thể thêm");
                }
                else if(checkEmployee != null)
                {
                    list.Add(new ApprovalHistoryModel()
                    {
                        Type1 = Portal.Constant.ConstFunction.NghiPhep,
                        EmployeeId = checkEmployee.EmployeeId,
                        ApprovalLevel = matrix.ApprovalLevel,
                        BrowseStatusID = status,
                        CreatedAccountId = accID,
                        CreatedTime = DateTime.Now,
                        MaTrixId = matrix.MaTrixFurloughID,
                        ApprovalId = apprvalId,
                        Id = Guid.NewGuid()
                    });
                }
            }
            
        }
        #endregion SendMail

        #region helper
        private void ManyToMany(FurloughModel model, List<FurloughDetailModel> funcList, List<ApprovalHistoryModel> list)
        {
            if (model.FurloughDetailModels != null)
            {
                model.FurloughDetailModels.Clear();
            }
            if (funcList != null && funcList.Count > 0)
            {
                foreach (var item in funcList)
                {
                    model.FurloughDetailModels.Add(item);
                }
            }
            if (model.ApprovalHistoryModels != null)
            {
                model.ApprovalHistoryModels.Clear();
            }

            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    model.ApprovalHistoryModels.Add(item);
                }
            }
        }
        #endregion helper

    }
    

}