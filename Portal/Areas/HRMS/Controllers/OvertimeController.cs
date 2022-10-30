using Portal.Extensions;
using Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Models;
using Portal.Resources;
using Portal.Repositories;
using Portal.EntityModels;
using Portal.Constant;
using System.Data;
using System.Transactions;
using System.Data.Entity;
namespace HRMS.Controllers
{
    public class OvertimeController : BaseController
    {
        const string controllerCode = ConstExcelController.Overtime;
        const int startIndex = 6;
        // GET: Overtime
        #region Tang ca của bạn
        [PortalAuthorization]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(DateTime? FromDate, DateTime? ToDate) {
            return ExecuteSearch(() =>
            {
                DateTime dNow = DateTime.Now;
                if (FromDate == null)
                {
                    FromDate = new DateTime(dNow.Year, dNow.Month, 01);
                }
                if (ToDate == null)
                {
                    ToDate = new DateTime(dNow.Year, dNow.Month, DateTime.DaysInMonth(dNow.Year, dNow.Month));
                }
                var data = _context.OvertimeModels.Where(it => it.OvertimeDay >= FromDate && ToDate >=it.OvertimeDay
                           && it.CreatedAccountId.Value == new Guid(CurrentUser.AccountId)).ToList();
                return PartialView(data);
            });
        }
        #endregion Tang ca của bạn
        #region Create 
        [PortalAuthorization]
        [HttpGet]
        public ActionResult Create() {
            ViewBag.DepartmentID = Data.DepartmentViewBag();
            OvertimeViewModel create = new OvertimeViewModel();
            var add = clsFunction.SearchEmployee(CurrentUser.EmployeeCode,true,_context);

            List<OvertimeDetailViewModel> list = new List<OvertimeDetailViewModel>();
            OvertimeDetailViewModel aa = new OvertimeDetailViewModel();
            aa.FullName = add.FullName;
            aa.EmployeeCode = add.EmployeeCode;
            aa.EmployeeId = add.EmployeeId;
            aa.DepartmentName = add.DepartmentName;
            aa.OvertimeId = null;
            list.Add(aa);
            create.Disable1 = false;
            create.OverDetail = list;
            return View(create);
        }
        [PortalAuthorization]
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult Create(OvertimeViewModel model)
        {
            return ExecuteContainer(() =>
            {
                using (var tran = new TransactionScope())
                {
                    if (model.OverDetail.Count > 0)
                    {
                        List<string> errorList = new List<string>();
                        List<OvertimeDetailModel> addDetails = new List<OvertimeDetailModel>();
                        List<ApprovalHistoryModel> list = new List<ApprovalHistoryModel>();
                        OvertimeModel ot = new OvertimeModel();

                        ot.OvertimeId = Guid.NewGuid();
                       
                        ot.OvertimeStartTime = TimeSpan.Parse(model.OvertimeStartTime);
                        ot.OvertimeEndTime = TimeSpan.Parse(model.OvertimeEndTime);
                        ot.Reason = model.Reason;
                        ot.ProjectID = model.ProjectID;
                        ot.DepartmentID = model.DepartmentID;
                        ot.OvertimeDay = model.OvertimeDay.Value;
                        ot.Disable1 = false;
                        ot.BrowseStatusID = "1";
                        ot.CreatedAccountId = new Guid(CurrentUser.AccountId);
                        ot.CreatedTime = DateTime.Now;
                        if (ot.OvertimeStartTime > ot.OvertimeEndTime)
                        {
                            errorList.Add("Thời gian tăng ca từ giờ đến giờ không hợp lệ");
                        }
                        if (clsFunction.checkKyCongNguoiDung(model.OvertimeDay.Value.Date))
                        {
                            errorList.Add(string.Format(LanguageResource.CheckFeat, LanguageResource.Overtime));
                        }
                        var accout = clsFunction.GetAccount(new Guid(CurrentUser.AccountId),_context);
                        if (accout.EmployeeModel == null)
                        {
                            errorList.Add("Người dùng chưa có liên kết nhân sự");
                        }
                        else if (accout.EmployeeModel != null)
                        {
                            if (accout.EmployeeModel.ParentId == null)
                            {
                                errorList.Add("Người dùng chưa có cấp quản lý trực tiếp");
                            }
                            if (accout.EmployeeModel.OvertimeCategoryId == null)
                            {
                                errorList.Add("Người dùng chưa chọn loại tăng ca");
                            }
                            if (accout.EmployeeModel.OvertimeCategoryId != null)
                            {
                                var matrix = _context.MatrixOvertimeModels.Where(it => it.OvertimeCategoryId == accout.EmployeeModel.OvertimeCategoryId).ToList();
                                if (matrix.Count == 0)
                                {
                                    errorList.Add("Chưa tạo quy trình duyệt tăng ca" + accout.EmployeeModel.OvertimeCategory.OvertimeCategoryName);
                                }
                                else
                                {
                                    int i = 1;
                                    string[] teamAppro;
                                    string status = "";
                                    foreach (var item in matrix)
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

                                                AddHistoryMaTrix(str, item, list, accout, errorList, new Guid(CurrentUser.AccountId), ot.OvertimeId, i, status, item.ApprovalLevel);
                                            }
                                        }
                                        else
                                        {
                                            AddHistoryMaTrix(item.ApprovalName, item, list, accout, errorList, new Guid(CurrentUser.AccountId), ot.OvertimeId, i, status, item.ApprovalLevel);
                                        }
                                        i++;
                                    }
                                }
                            }
                        }
                        // Add chi tiết tang ca
                        foreach (var item in model.OverDetail)
                        {

                            string kq = OvertimeFunction.CheckExistOT(model.OvertimeDay.Value, ot.OvertimeStartTime, ot.OvertimeEndTime, ot.OvertimeId, item.EmployeeId.Value, item.EmployeeCode,_context);
                            if (kq != "")
                            {
                                errorList.Add(kq);
                            }
                            else
                            {
                                OvertimeDetailModel dtl = new OvertimeDetailModel();
                                dtl.EmployeeId = item.EmployeeId.Value;
                                dtl.OvertimeId = ot.OvertimeId;
                                dtl.Del = false;
                                addDetails.Add(dtl);
                            }
                        }
                        // add quy trình duyệt
                        if (errorList != null && errorList.Count > 0)
                        {
                            return Json(new
                            {
                                Code = System.Net.HttpStatusCode.Created,
                                Success = false,
                                Data = errorList
                            });
                        }
                        ManyToMany(ot, addDetails, list);
                        _context.Entry(ot).State = EntityState.Added;
                        _context.SaveChanges();
                        tran.Complete();
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.Created,
                            Success = true,
                            Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.Overtime.ToLower())
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.Created,
                            Success = true,
                            Data = string.Format(LanguageResource.CheckData, LanguageResource.Overtime.ToLower())
                        });
                    }
                    
                }
            });
        }
        #endregion Create

        #region Edit
        [PortalAuthorization]
        public ActionResult Edit(Guid id) {
            Guid accID = new Guid(CurrentUser.AccountId);
            var edit = _context.OvertimeModels.FirstOrDefault(it => it.OvertimeId == id && it.CreatedAccountId ==accID  && it.OvertimeDetailModels.Any(p=>p.Del != true));
           
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Overtime.ToLower()) });
            }
            var model = OvertimeFunction.getDatTa(edit, CurrentUser.AccountId);
            ViewBag.DepartmentID = Data.DepartmentViewBag(edit.DepartmentID);
            return View(model);
        }
        [PortalAuthorization]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult Edit(OvertimeViewModel model) {
            return ExecuteContainer(() =>
            {
                var edit = _context.OvertimeModels.FirstOrDefault(it => it.OvertimeId == model.OvertimeId  && it.CreatedAccountId == new Guid(CurrentUser.AccountId));

                List<string> errorList = new List<string>();
                if (edit != null)
                {
                    OvertimeFunction.Update(model, edit, errorList, false,new Guid(CurrentUser.AccountId));

                }
                // Kiểm tra giờ tang ca có bị trùng ko
                foreach (var item in model.OverDetail)
                {

                    string kq = OvertimeFunction.CheckExistOT(model.OvertimeDay.Value, TimeSpan.Parse(model.OvertimeStartTime), TimeSpan.Parse(model.OvertimeEndTime), edit.OvertimeId, item.EmployeeId.Value, item.EmployeeCode, _context);
                    if (kq != "")
                    {
                        errorList.Add(kq);
                    }
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
                edit.LastModifiedAccountId = new Guid(CurrentUser.AccountId);
                edit.LastModifiedTime = DateTime.Now;

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

        #region Gui mail
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public ActionResult SendEMail(Guid idSendEmail)
        {
            using (var tran = new TransactionScope()) {
                try
                {
                    var model = _context.OvertimeModels.FirstOrDefault(it => it.OvertimeId == idSendEmail && it.BrowseStatusID != "3" && it.BrowseStatusID !="4" && it.OvertimeDetailModels.Any(p=>p.Del!= true));
                    Guid accountID = new Guid(CurrentUser.AccountId);
                    var account = _context.Accounts.FirstOrDefault(it => it.AccountId == accountID);
                    string cc = account.EmployeeModel.CompanyEmail, to = "", receiver = "";
                    if (model != null)
                    {
                        // kiểm tra xem kỳ công có khóa chưa
                        if (clsFunction.checkKyCongNguoiDung(model.OvertimeDay))
                        {
                            return Json(new
                            {
                                Code = System.Net.HttpStatusCode.NotModified,
                                Success = false,
                                Data = string.Format(LanguageResource.CheckFeat, LanguageResource.SendMail.ToLower())
                            });
                        }
                        if (model.Disable1 == null || !model.Disable1.Value)
                        {
                            model.Disable1 = true;
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
                            string sL = model.OvertimeDetailModels.Where(it=>it.Del!= true).Count().ToString();
                            string body = OvertimeFunction.EmailContent(receiver, sL, model, "Anh/Chị có yêu cầu duyệt tăng ca được gửi từ <b>" + account.EmployeeModel.FullName + "</b>");
                            if (body != "" && FunctionExtensions.SendMail(LanguageResource.TitleOvertime, to, cc, body))
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
                            Data = string.Format(LanguageResource.Error_NotExist, LanguageResource.Overtime.ToLower())
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
        #endregion Gui mail

        #region Delete
        public ActionResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var del = _context.OvertimeModels.FirstOrDefault(p => p.OvertimeId == id && p.Disable1 != true 
                    && p.BrowseStatusID == "1" && p.CreatedAccountId == new Guid(CurrentUser.AccountId));
                if (del != null)
                {
                    if (del.OvertimeDetailModels.Count > 0)
                    {
                        del.OvertimeDetailModels.Clear();
                    }
                    _context.Entry(del).State = EntityState.Deleted;
                    var hisApp = _context.ApprovalHistoryModels.Where(it => it.ApprovalId == del.OvertimeId && it.Type1 == Portal.Constant.ConstFunction.TangCa).ToList();
                    if (hisApp != null && hisApp.Count > 0)
                    {
                        _context.ApprovalHistoryModels.RemoveRange(hisApp);
                    }
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Overtime.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.Overtime.ToLower())
                    });
                }
            });
        }
        #endregion Delete

        #region Helper
        public void AddHistoryMaTrix(string apprval, MatrixOvertimeModel matrix, List<ApprovalHistoryModel> list, Account acc, List<string> errorList, Guid accID, Guid apprvalId, int index, string status, int approvalLevel)
        {
            if (apprval.ToLower() == "head")
            {
                if (list.Where(it => it.EmployeeId == acc.EmployeeModel.ParentId.Value && it.ApprovalLevel == approvalLevel).ToList().Count > 0)
                {
                    errorList.Add("Đã tồn tại người duyệt có mã " + acc.EmployeeModel.EmployeeCode + " ở bước " + index.ToString() + " không thể thêm");
                }
                list.Add(new ApprovalHistoryModel()
                {
                    Type1 = Portal.Constant.ConstFunction.TangCa,
                    EmployeeId = acc.EmployeeModel.ParentId.Value,
                    ApprovalLevel = matrix.ApprovalLevel,
                    BrowseStatusID = status,
                    CreatedAccountId = accID,
                    CreatedTime = DateTime.Now,
                    MaTrixId = matrix.MatrixOvertimeId,
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
                else if (checkEmployee != null)
                {
                    list.Add(new ApprovalHistoryModel()
                    {
                        Type1 = Portal.Constant.ConstFunction.TangCa,
                        EmployeeId = checkEmployee.EmployeeId,
                        ApprovalLevel = matrix.ApprovalLevel,
                        BrowseStatusID = status,
                        CreatedAccountId = accID,
                        CreatedTime = DateTime.Now,
                        MaTrixId = matrix.MatrixOvertimeId,
                        ApprovalId = apprvalId,
                        Id = Guid.NewGuid()
                    });
                }
            }

        }
        private void ManyToMany(OvertimeModel ot, List<OvertimeDetailModel> funcList, List<ApprovalHistoryModel> list)
        {
            if (ot.OvertimeDetailModels != null)
            {
                ot.OvertimeDetailModels.Clear();
            }
            if (funcList != null && funcList.Count > 0)
            {
                foreach (var item in funcList)
                {
                    ot.OvertimeDetailModels.Add(item);
                }
            }
            if (ot.ApprovalHistoryModels != null)
            {
                ot.ApprovalHistoryModels.Clear();
            }
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    ot.ApprovalHistoryModels.Add(item);
                }
            }
        }
        public ActionResult AddEmployee(List<OvertimeDetailViewModel> listOT, string employeeCode)
        {
            if (listOT == null)
            {
                listOT = new List<OvertimeDetailViewModel>();
            }
            var searchEmployee = clsFunction.SearchEmployee(employeeCode,true,_context);
            if (searchEmployee == null)
            {
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.GatewayTimeout,
                    Success = false,
                    Data = string.Format("Không tìm thấy nhân viên có mã {0}", employeeCode)
                });
            }
            OvertimeDetailViewModel add = new OvertimeDetailViewModel();
            add.FullName = searchEmployee.FullName;
            add.EmployeeCode = searchEmployee.EmployeeCode;
            add.EmployeeId = searchEmployee.EmployeeId;
            add.DepartmentName = searchEmployee.DepartmentName;
            listOT.Add(add);
            listOT = listOT.GroupBy(o => new { o.EmployeeId, o.FullName, o.EmployeeCode, o.DepartmentName,o.OvertimeId}).Select(it => it.FirstOrDefault()).ToList();
            return PartialView("_OvertimeDetailInfo", listOT);
        }
        public ActionResult DelEmployee(List<OvertimeDetailViewModel> listOT, string employeeCode)
        {
            if (listOT != null && listOT.Count >0)
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
                    var info = _context.OvertimeModels.FirstOrDefault(it => it.OvertimeId == del.OvertimeId);
                    if (info.Disable1 == true)
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.Created,
                            Success = false,
                            Data = "Đã gửi mail không thể xóa"
                        });
                    }
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
                return PartialView("_OvertimeDetailInfo", listOT);
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
        #region Export file mẫu
        public ActionResult ExportFilemau() {
            List<OvertimeDetailViewModel> menu = new List<OvertimeDetailViewModel>();
            return Export(menu);
        }
        public FileContentResult Export(List<OvertimeDetailViewModel> menu) {
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            columns.Add(new ExcelTemplate() { ColumnName = "EmployeeCode", isText = true });
            columns.Add(new ExcelTemplate() { ColumnName = "FullName",  isText = true });
            columns.Add(new ExcelTemplate() { ColumnName = "DepartmentName", isAllowedToEdit = true, isText = true, });
            //Header
            string fileheader = string.Format(LanguageResource.Export_ExcelHeader, LanguageResource.Overtime);
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = controllerCode,
                RowsToIgnore = 1,
                isWarning = false,
                isCode = true
            });
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = fileheader,
                RowsToIgnore = 1,
                isWarning = false,
                isCode = false
            });
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = "Chú ý nhập đúng mã nhân viên và không để trùng",
                RowsToIgnore = 1,
                isWarning = true,
                isCode = false
            });
            byte[] filecontent = ClassExportExcel.ExportExcel(menu, columns, heading, true);
            //File name
            string fileNameWithFormat = string.Format("{0}.xlsx", RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion Export file mẫu

        #region Import
        public ActionResult Import() {
            DataSet ds = GetDataSetFromExcel();
            List<string> errorList = new List<string>();
            List<OvertimeDetailViewModel> OT = new List<OvertimeDetailViewModel>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataTable dt in ds.Tables)
                {
                    string contCode = dt.Columns[0].ColumnName.ToString();
                    if (contCode == controllerCode)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dt.Rows.IndexOf(dr) >= startIndex)
                            {
                                var employeeCode = dr.ItemArray[1];
                                if (employeeCode == null || employeeCode.ToString() == "")
                                    break;
                                var employee = clsFunction.SearchEmployee(employeeCode.ToString(),true,_context);
                                if (employee == null)
                                {
                                    errorList.Add(string.Format("Không tìm thấy nhân sự có mã {0} dòng số {1} ", employeeCode.ToString(), dr.ItemArray[0].ToString()));
                                }
                                else
                                {
                                    OvertimeDetailViewModel add = new OvertimeDetailViewModel();
                                    add.EmployeeCode = employee.EmployeeCode;
                                    add.FullName = employee.FullName;
                                    add.EmployeeId = employee.EmployeeId;
                                    add.DepartmentName = employee.DepartmentName;
                                    OT.Add(add);
                                }
                            }
                        }
                    }
                }
            }
            if (errorList.Count > 0)
            {
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.NotImplemented,
                    Success = false,
                    Data = errorList
                });
            }
            return PartialView("_OvertimeDetailInfo", OT);
        }
        #endregion Import

        #endregion Helper
    }
}