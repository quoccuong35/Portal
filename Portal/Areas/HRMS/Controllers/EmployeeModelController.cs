using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.EntityModels;
using Portal.ViewModels;
using System.Data.Entity;
using Portal.Resources;
using Portal.Repositories;
using Portal.Constant;
using System.Data;
using System.Transactions;

namespace HRMS.Controllers
{
    public class EmployeeModelController : BaseController
    {
        const string controllerCode = ConstExcelController.Employee;
        const int startIndex = 6;
        // GET: EmployeeModel Nhân sự
        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            ViewBag.DepartmentID = Data.DepartmentViewBag();
            ViewBag.EmployeeStatusCategoryId = Data.EmployeeStatusCategoryNameViewBag();
            return View();
        }
        public ActionResult _Search(EmployeeSearch search) {

            return ExecuteSearch(() =>
            {
                //var employees = _context.Proc_EmployeeModel(search.EmployeeCode, search.FullName, search.DepartmentID, search.EmployeeStatusCategoryId, search.PositionID, search.Actived).ToList();
                var employees = _context.EmployeeModels.Where(
                    it=>(it.EmployeeCode.Contains(search.EmployeeCode) || search.EmployeeCode == null)
                        && (search.FullName == null || it.FullName.Contains(search.FullName)) 
                        &&(search.EmployeeStatusCategoryId == null || it.EmployeeStatusCategoryId == search.EmployeeStatusCategoryId) 
                        && (search.DepartmentID == null || it.DepartmentID == search.DepartmentID) 
                        &&(search.Actived == null || it.Actived == search.Actived.Value)
                    ).OrderBy(it=>it.DepartmentID).ToList();
                return PartialView(employees);
            });
        }
        #endregion Index

        #region View 
        [PortalAuthorization]
        public ActionResult View(Guid id) {
            var edit = _context.EmployeeModels.FirstOrDefault(it => it.EmployeeId == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Report.ToLower()) });
            }
            ViewBag.PositionID = Data.PositionViewBag(edit.PositionID);
            ViewBag.DepartmentID = Data.DepartmentViewBag(edit.DepartmentID);
            ViewBag.EmployeeStatusCategoryId = Data.EmployeeStatusCategoryNameViewBag(edit.EmployeeStatusCategoryId);
            ViewBag.WorkPlaceID = Data.WorkPlaceViewBag(edit.WorkPlaceID);
            ViewBag.ShiftWorkId = Data.ShiftWorkViewBag(edit.ShiftWorkId);
            ViewBag.BankId = Data.BankViewBag(edit.BankId);
            ViewBag.EducationId = Data.EducationViewBag(edit.EducationId);
            ViewBag.ReligionId = Data.ReligionViewBag(edit.ReligionId);
            ViewBag.FolkId = Data.FolkViewBag(edit.FolkId);
            ViewBag.NationalityId = Data.NationalityViewBag(edit.NationalityId);
            ViewBag.ParentId = Data.ParentViewBag();
            if (edit.ImageUrl != null && edit.ImageUrl != "")
            {
                edit.ImageUrl = System.Configuration.ConfigurationManager.AppSettings["NhanSu"].ToString() + edit.ImageUrl;
            }
            else
            {
                edit.ImageUrl = System.Configuration.ConfigurationManager.AppSettings["Avatar"].ToString();
            }

            return View("Edit",edit);
        }
        #endregion View
        #region Create
        [PortalAuthorization]
        public ActionResult Create() {
            ViewBag.PositionID = Data.PositionViewBag();
            ViewBag.DepartmentID = Data.DepartmentViewBag();
            ViewBag.EmployeeStatusCategoryId = Data.EmployeeStatusCategoryNameViewBag();
            ViewBag.WorkPlaceID = Data.WorkPlaceViewBag();
            ViewBag.ShiftWorkId = Data.ShiftWorkViewBag();
            ViewBag.BankId = Data.BankViewBag();
            ViewBag.EducationId = Data.EducationViewBag();
            ViewBag.ReligionId = Data.ReligionViewBag();
            ViewBag.FolkId = Data.FolkViewBag();
            ViewBag.NationalityId = Data.NationalityViewBag();
            ViewBag.ParentId = Data.ParentViewBag();
            ViewBag.OvertimeCategoryId = Data.OvertimeCategoryBag();
            EmployeeModel create = new EmployeeModel();
            return View();
        }

        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Create(EmployeeModel model, HttpPostedFileBase ImageUrl)
        {
            return ExecuteContainer(() =>
            {
                model.EmployeeId = Guid.NewGuid();
                model.CreatedAccountId = new Guid(CurrentUser.AccountId);
                model.CreatedTime = DateTime.Now;
                model.IsHead = model.IsHead == null ? false : model.IsHead.Value;
                model.Actived = true;
                model.RemainingLeavedays = 0;
                if (ImageUrl != null)
                {
                    model.ImageUrl = Upload(ImageUrl, "NhanSu", model.EmployeeCode);
                }
                _context.Entry(model).State = EntityState.Added;
                
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.EmployeeModel.ToLower())
                });
            });
        }
        #endregion Create

        #region Edit
        [PortalAuthorization]
        public ActionResult Edit(Guid id) {
            var edit = _context.EmployeeModels.FirstOrDefault(it => it.EmployeeId == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.EmployeeModel.ToLower()) });
            }
            ViewBag.PositionID = Data.PositionViewBag(edit.PositionID);
            ViewBag.DepartmentID = Data.DepartmentViewBag(edit.DepartmentID);
            ViewBag.EmployeeStatusCategoryId = Data.EmployeeStatusCategoryNameViewBag(edit.EmployeeStatusCategoryId);
            ViewBag.WorkPlaceID = Data.WorkPlaceViewBag(edit.WorkPlaceID);
            ViewBag.ShiftWorkId = Data.ShiftWorkViewBag(edit.ShiftWorkId);
            ViewBag.BankId = Data.BankViewBag(edit.BankId);
            ViewBag.EducationId = Data.EducationViewBag(edit.EducationId);
            ViewBag.ReligionId = Data.ReligionViewBag(edit.ReligionId);
            ViewBag.FolkId = Data.FolkViewBag(edit.FolkId);
            ViewBag.NationalityId = Data.NationalityViewBag(edit.NationalityId);
            ViewBag.ParentId = Data.ParentViewBag(edit.ParentId);
            ViewBag.OvertimeCategoryId = Data.OvertimeCategoryBag(edit.OvertimeCategoryId);
            if (edit.ImageUrl != null && edit.ImageUrl != "")
            {
                edit.ImageUrl = System.Configuration.ConfigurationManager.AppSettings["NhanSu"].ToString() + edit.ImageUrl;
            }
            else
            {
                edit.ImageUrl = System.Configuration.ConfigurationManager.AppSettings["Avatar"].ToString();
            }
            
            return View(edit);
        }
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Edit(EmployeeModel model, HttpPostedFileBase ImageUrl)
        {
            return ExecuteContainer(() =>
            {
                var edit = _context.EmployeeModels.FirstOrDefault(it => it.EmployeeId == model.EmployeeId);
                if (edit != null)
                {
                    edit.FullName = model.FullName;
                    edit.Gender = model.Gender;
                    edit.DateOfBirth = model.DateOfBirth;
                    edit.PlaceOfBirth = model.PlaceOfBirth;
                    edit.EmployeeStatusCategoryId = model.EmployeeStatusCategoryId;
                    edit.WorkPlaceID = model.WorkPlaceID;
                    edit.DepartmentID = model.DepartmentID;
                    edit.ShiftWorkId = model.ShiftWorkId;
                    edit.StandardWorkingDay = model.StandardWorkingDay;
                    edit.CompanyEmail = model.CompanyEmail;
                    edit.PhoneNumber = model.PhoneNumber;
                    edit.DepartmentHint = model.DepartmentHint;
                    edit.PositionID = model.PositionID;
                    edit.Specialized = model.Specialized;
                    edit.WorkingDate = model.WorkingDate;
                    edit.IdentityCard = model.IdentityCard;
                    edit.DateOfIssue = model.DateOfIssue;
                    edit.PlaceOfIssue = model.PlaceOfIssue;
                    edit.ParentId = model.ParentId;
                    edit.TaxId = model.TaxId;
                    edit.SocialinsuranceNumber = model.SocialinsuranceNumber;
                    edit.RegistrationHospital = model.RegistrationHospital;
                    edit.BankId = model.BankId;
                    edit.BankCardNumber = model.BankCardNumber;
                    edit.BankBranch = model.BankBranch;
                    edit.EducationId = model.EducationId;
                    edit.ReligionId = model.ReligionId;
                    edit.FolkId = model.FolkId;
                    edit.NationalityId = model.NationalityId;
                    edit.TemporaryAddress = model.TemporaryAddress;
                    edit.HouseholdAddress = model.HouseholdAddress;
                    edit.TrialJobFromDay = model.TrialJobFromDay;
                    edit.TrialJobEndDay = model.TrialJobEndDay;
                    edit.OvertimeCategoryId = model.OvertimeCategoryId;
                    edit.EndDate = model.EndDate;
                    edit.Description = model.Description;
                    edit.IsHead = model.IsHead ==null?false: model.IsHead;
                    if (CurrentUser.UserName.ToLower() == "admin")
                    {
                        edit.Actived = model.Actived;
                    }
                  
                    edit.Description = model.Description;
                    if (ImageUrl != null)
                    {
                        edit.ImageUrl = Upload(ImageUrl, "NhanSu", model.EmployeeCode);
                    }
                    HistoryRepository _repository = new HistoryRepository(_context);
                    _repository.SaveUpdateHistory(edit.EmployeeId, CurrentUser.UserName, edit);
                    edit.LastModifiedAccountId = new Guid(CurrentUser.AccountId);
                    edit.LastModifiedTime = DateTime.Now;
                    _context.Entry(edit).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = false,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.EmployeeModel.ToLower())
                });
            });
        }
        #endregion Edit
        #region Delete
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        [PortalAuthorization]
        public JsonResult Delete(Guid id)
        {
            return ExecuteContainer(() =>
            {
                var del = _context.EmployeeModels.FirstOrDefault(p => p.EmployeeId == id);
                if (del != null)
                {
                    del.Actived = false;
                    _context.Entry(del).State = EntityState.Modified;
                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.EmployeeCode.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.EmployeeModel.ToLower())
                    });
                }
            });
        }
        #endregion Delete

        #region Export data
        //[PortalAuthorization]
        public ActionResult ExportCreate()
        {
            List<EmployeeExcel> menu = new List<EmployeeExcel>();
            return Export(menu);
        }
        //[PortalAuthorization]
        public ActionResult ExportEdit(EmployeeSearch search) {
            var data = _context.EmployeeModels.ToList().Select(x => new EmployeeExcel
            {
                EmployeeId = x.EmployeeId,
                EmployeeCode = x.EmployeeCode,
                FullName = x.FullName,
                TimekeepingCode = x.TimekeepingCode,
                Gender = x.Gender,
                DateOfBirth = x.DateOfBirth,
                PlaceOfBirth = x.PlaceOfBirth,
                EmployeeStatusCategoryName = x.EmployeeStatusCategory.EmployeeStatusCategoryName,
                WorkPlaceName = x.WorkPlace.WorkPlaceName,
                DepartmentName = x.Department.DepartmentName,
                DepartmentHint = x.DepartmentHint,
                ParentName = x.EmployeeModel2 != null? x.EmployeeModel2.EmployeeCode+ "-"+x.EmployeeModel2.FullName:"",
                PositionName = x.Position.PositionName,
                Specialized = x.Specialized,
                ShiftWorkName = x.ShiftWork.ShiftWorkName,
                StandardWorkingDay = x.StandardWorkingDay,
                IdentityCard = x.IdentityCard,
                DateOfIssue = x.DateOfIssue,
                PlaceOfIssue = x.PlaceOfIssue,
                PhoneNumber = x.PhoneNumber,
                CompanyEmail = x.CompanyEmail,
                PersonalEmail = x.PersonalEmail,
                WorkingDate = x.WorkingDate,
                TrialJobFromDay = x.TrialJobFromDay,
                TrialJobEndDay = x.TrialJobEndDay,
                EndDate = x.EndDate,
                BankName = x.Bank.BankName,
                BankCardNumber = x.BankCardNumber,
                BankBranch = x.BankBranch,
                TaxId = x.TaxId,
                SocialinsuranceNumber = x.SocialinsuranceNumber,
                RegistrationHospital = x.RegistrationHospital,
                EducationName = x.Education!=null?x.Education.EducationName:"",
                ReligionName = x.Religion!= null? x.Religion.ReligionName:"",
                FolkName = x.Folk!=null?x.Folk.FolkName:"",
                NationalityName = x.Nationality!=null?x.Nationality.NationalityName:"",
                TemporaryAddress = x.TemporaryAddress,
                HouseholdAddress = x.HouseholdAddress,
                Description = x.Description,
                IsHead = x.IsHead

            }).ToList();
            return Export(data);
        }
        public FileContentResult Export(List<EmployeeExcel> menu)
        {
            //Columns to take
            //string[] columns = { "MenuId", "MenuName", "OrderIndex", "Icon" };
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            columns.Add(new ExcelTemplate() { ColumnName = "EmployeeId", isAllowedToEdit = false, isText = true });
            columns.Add(new ExcelTemplate() { ColumnName = "EmployeeCode", isAllowedToEdit = false, isText = true });
            columns.Add(new ExcelTemplate() { ColumnName = "TimekeepingCode", isAllowedToEdit = false, isText = true });
            columns.Add(new ExcelTemplate() { ColumnName = "FullName", isAllowedToEdit = true, isText = true, });
            columns.Add(new ExcelTemplate() { ColumnName = "Gender", isAllowedToEdit = true, isBoolean = true, isComment = true, strComment = "Nam để chữ X nữ để trống" });
            columns.Add(new ExcelTemplate() { ColumnName = "DateOfBirth", isAllowedToEdit = true, isText = true });
            columns.Add(new ExcelTemplate() { ColumnName = "PlaceOfBirth", isAllowedToEdit = true, isText = true });

            var statusCategory = _context.EmployeeStatusCategories.ToList().Select(x => new DropdownModel { Id = x.EmployeeStatusCategoryId, Name = x.EmployeeStatusCategoryName }).ToList();
            columns.Add(new ExcelTemplate() { ColumnName = "EmployeeStatusCategoryName", isAllowedToEdit = true, isDropdownlist = true, DropdownData = statusCategory, TypeId = ConstExcelController.GuidId });

            var workPlace = _context.WorkPlaces.Where(it => it.Actived == true).ToList().Select(x => new DropdownModel { Id = x.WorkPlaceID, Name = x.WorkPlaceName, OrderIndex = x.OrderIndex }).ToList();
            columns.Add(new ExcelTemplate() { ColumnName = "WorkPlaceName", isAllowedToEdit = true, isDropdownlist = true, DropdownData = workPlace, TypeId = ConstExcelController.GuidId });


            var depaetments = _context.Departments.Where(it => it.Actived == true).ToList().Select(x => new DropdownModel { Id = x.DepartmentID, Name = x.DepartmentName, OrderIndex = x.OrderIndex }).ToList();
            columns.Add(new ExcelTemplate() { ColumnName = "DepartmentName", isAllowedToEdit = true, isDropdownlist = true, DropdownData = depaetments, TypeId = ConstExcelController.GuidId });

            columns.Add(new ExcelTemplate() { ColumnName = "DepartmentHint", isAllowedToEdit = true, isText = true });

            var ParentName = _context.EmployeeModels.Where(it => it.IsHead == true && it.Actived == true).ToList().Select(x => new DropdownModel { Id = x.EmployeeId, Name = x.EmployeeCode + "-" + x.FullName }).ToList();
            columns.Add(new ExcelTemplate() { ColumnName = "ParentName", isAllowedToEdit = true, DropdownData = ParentName, isDropdownlist = true, TypeId = ConstExcelController.GuidId });

            var positions = _context.Positions.Where(it => it.Actived == true).ToList().Select(x => new DropdownModel { Id = x.PositionID, Name = x.PositionName, OrderIndex = x.OrderIndex }).ToList();
            columns.Add(new ExcelTemplate() { ColumnName = "PositionName", isAllowedToEdit = true, isDropdownlist = true, DropdownData = positions, TypeId = ConstExcelController.GuidId });

            columns.Add(new ExcelTemplate() { ColumnName = "Specialized", isAllowedToEdit = true, isText = true });

            var shiftWork = _context.ShiftWorks.Where(it => it.Actived == true).ToList().Select(x => new DropdownModel { Id = x.ShiftWorkId, Name = x.ShiftWorkName, OrderIndex = x.OrderIndex }).ToList();
            columns.Add(new ExcelTemplate() { ColumnName = "ShiftWorkName", isAllowedToEdit = true, isDropdownlist = true, DropdownData = shiftWork, TypeId = ConstExcelController.GuidId });

            columns.Add(new ExcelTemplate() { ColumnName = "StandardWorkingDay", isAllowedToEdit = true });

            columns.Add(new ExcelTemplate() { ColumnName = "IdentityCard", isAllowedToEdit = true });

            columns.Add(new ExcelTemplate() { ColumnName = "DateOfIssue", isAllowedToEdit = true, isDateTime = true });

            columns.Add(new ExcelTemplate() { ColumnName = "PlaceOfIssue", isAllowedToEdit = true, isText = true });

            columns.Add(new ExcelTemplate() { ColumnName = "PhoneNumber", isAllowedToEdit = true });

            columns.Add(new ExcelTemplate() { ColumnName = "CompanyEmail", isAllowedToEdit = true, isText = true });

            columns.Add(new ExcelTemplate() { ColumnName = "PersonalEmail", isAllowedToEdit = true });

            columns.Add(new ExcelTemplate() { ColumnName = "WorkingDate", isAllowedToEdit = true, isDateTime = true });

            columns.Add(new ExcelTemplate() { ColumnName = "TrialJobFromDay", isAllowedToEdit = true, isDateTime = true });

            columns.Add(new ExcelTemplate() { ColumnName = "TrialJobEndDay", isAllowedToEdit = true, isDateTime = true });

            columns.Add(new ExcelTemplate() { ColumnName = "EndDate", isAllowedToEdit = true, isDateTime = true });

            var banks = _context.Banks.Where(it => it.Actived == true).ToList().Select(x => new DropdownModel { Id = x.BankId, Name = x.BankName, OrderIndex = x.OrderIndex }).ToList();
            columns.Add(new ExcelTemplate() { ColumnName = "BankName", isAllowedToEdit = true, isDropdownlist = true, DropdownData = banks, TypeId = ConstExcelController.GuidId });

            columns.Add(new ExcelTemplate() { ColumnName = "BankCardNumber", isAllowedToEdit = true, isText = true });

            columns.Add(new ExcelTemplate() { ColumnName = "BankBranch", isAllowedToEdit = true, isText = true });

            columns.Add(new ExcelTemplate() { ColumnName = "TaxId", isAllowedToEdit = true, isText = true });

            columns.Add(new ExcelTemplate() { ColumnName = "SocialinsuranceNumber", isAllowedToEdit = true, isText = true });

            columns.Add(new ExcelTemplate() { ColumnName = "RegistrationHospital", isAllowedToEdit = true, isText = true });


            var educations = _context.Educations.Where(it => it.Actived == true).ToList().Select(x => new DropdownModel { Id = x.EducationId, Name = x.EducationName }).ToList();
            columns.Add(new ExcelTemplate() { ColumnName = "EducationName", isAllowedToEdit = true, isDropdownlist = true, DropdownData = educations, TypeId = ConstExcelController.GuidId });

            var religions = _context.Religions.Where(it => it.Actived == true).ToList().Select(x => new DropdownModel { Id = x.ReligionId, Name = x.ReligionName }).ToList();
            columns.Add(new ExcelTemplate() { ColumnName = "ReligionName", isAllowedToEdit = true, isDropdownlist = true, DropdownData = religions, TypeId = ConstExcelController.GuidId });

            var folks = _context.Folks.Where(it => it.Actived == true).ToList().Select(x => new DropdownModel { Id = x.FolkId, Name = x.FolkName }).ToList();
            columns.Add(new ExcelTemplate() { ColumnName = "FolkName", isAllowedToEdit = true, isDropdownlist = true, DropdownData = folks, TypeId = ConstExcelController.GuidId });

            var nationalities = _context.Nationalities.Where(it => it.Actived == true).ToList().Select(x => new DropdownModel { Id = x.NationalityId, Name = x.NationalityName }).ToList();
            columns.Add(new ExcelTemplate() { ColumnName = "NationalityName", isAllowedToEdit = true, isDropdownlist = true, DropdownData = nationalities, TypeId = ConstExcelController.GuidId });

            columns.Add(new ExcelTemplate() { ColumnName = "TemporaryAddress", isAllowedToEdit = true, isText = true });

            columns.Add(new ExcelTemplate() { ColumnName = "HouseholdAddress", isAllowedToEdit = true, isText = true });

            columns.Add(new ExcelTemplate() { ColumnName = "Description", isAllowedToEdit = true, isText = true });

            columns.Add(new ExcelTemplate() { ColumnName = "IsHead", isAllowedToEdit = true, isBoolean = true, isComment = true, strComment = "Là head để X không để trống" });

            //Header

            //Header
            string fileheader = string.Format(LanguageResource.Export_ExcelHeader, LanguageResource.EmployeeModel);
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = controllerCode,
                RowsToIgnore = 1,
                isWarning = false,
                isCode = true
            });
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = fileheader.ToUpper(),
                RowsToIgnore = 1,
                isWarning = false,
                isCode = false
            });
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = LanguageResource.Export_ExcelWarning1 + "-" + LanguageResource.Export_ExcelWarning2,
                RowsToIgnore = 1,
                isWarning = true,
                isCode = false
            });

            //Body
            byte[] filecontent = ClassExportExcel.ExportExcel(menu, columns, heading, true);
            //File name
            string fileNameWithFormat = string.Format("{0}.xlsx", RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion Export data

        #region Inport data excel
        public ActionResult Import() {
            DataSet ds = GetDataSetFromExcel();
            List<string> errorList = new List<string>();
            return ExcuteImportExcel(() =>
            {
                if (ds.Tables.Count > 0) {
                    using (TransactionScope ts = new TransactionScope()) {
                        foreach (DataTable dt in ds.Tables)
                        {
                            string contCode = dt.Columns[0].ColumnName.ToString();
                            if (contCode == controllerCode) {

                                foreach (DataRow dr in dt.Rows)
                                {
                                    //string aa = dr.ItemArray[0].ToString();
                                    if (dt.Rows.IndexOf(dr) >= startIndex  ) {
                                        if (!string.IsNullOrEmpty(dr.ItemArray[0].ToString()))
                                        {
                                            var data = CheckTemplate(dr.ItemArray);
                                            if (!string.IsNullOrEmpty(data.Error))
                                            {
                                                errorList.Add(data.Error);
                                            }
                                            else
                                            {
                                                // Tiến hành cập nhật
                                                string result = ExecuteImportExcelMenu(data);
                                                if (result != LanguageResource.ImportSuccess)
                                                {
                                                    errorList.Add(result);
                                                }
                                            }
                                        }
                                        else
                                            break;
                                        //Check correct template
                                        
                                    }
                                }
                            }
                            else
                            {
                                string error = string.Format(LanguageResource.Validation_ImportCheckController, LanguageResource.EmployeeModel);
                                errorList.Add(error);
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
                        ts.Complete();
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.Created,
                            Success = true,
                            Data = LanguageResource.ImportSuccess
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = false,
                        Data = LanguageResource.Validation_ImportExcelFile
                    });
                }
            });
        }

        #region Insert/Update data from excel file
        public string ExecuteImportExcelMenu(EmployeeExcel employeeIsValid)
        {
            //Check:
            //1. If MenuId == "" then => Insert
            //2. Else then => Update
            if (employeeIsValid.isNullValueId == true)
            {
                EmployeeModel employee = new EmployeeModel();
                employee.EmployeeId = Guid.NewGuid();
                employee.EmployeeCode = employeeIsValid.EmployeeCode;
                employee.TimekeepingCode = employeeIsValid.TimekeepingCode;
                employee.FullName = employeeIsValid.FullName;
                employee.Gender = employeeIsValid.Gender;
                employee.PositionID = new Guid(employeeIsValid.PositionName);
                employee.DateOfBirth = employeeIsValid.DateOfBirth;
                employee.EmployeeStatusCategoryId = new Guid(employeeIsValid.EmployeeStatusCategoryName);
                employee.WorkPlaceID = new Guid(employeeIsValid.WorkPlaceName);
                employee.ShiftWorkId = new Guid(employeeIsValid.ShiftWorkName);
                employee.DepartmentID = new Guid(employeeIsValid.DepartmentName);
                employee.ParentId = employeeIsValid.ParentName != null ? new Guid(employeeIsValid.ParentName) : (Guid?)null;
                employee.PlaceOfBirth = employeeIsValid.PlaceOfBirth;
                employee.IdentityCard = employeeIsValid.IdentityCard;
                employee.DateOfIssue = employeeIsValid.DateOfIssue;
                employee.PlaceOfIssue = employeeIsValid.PlaceOfIssue;
                employee.PhoneNumber = employeeIsValid.PhoneNumber;
                employee.CompanyEmail = employeeIsValid.CompanyEmail;
                employee.PersonalEmail = employeeIsValid.PersonalEmail;
                employee.WorkingDate = employeeIsValid.WorkingDate;
                employee.StandardWorkingDay = employeeIsValid.StandardWorkingDay;
                employee.EndDate = employeeIsValid.EndDate;
                employee.TrialJobFromDay = employeeIsValid.TrialJobFromDay;
                employee.TrialJobEndDay = employeeIsValid.TrialJobEndDay;
                employee.TemporaryAddress = employeeIsValid.TemporaryAddress;
                employee.HouseholdAddress = employeeIsValid.HouseholdAddress;
                employee.BankId = new Guid(employeeIsValid.BankName);
                employee.BankCardNumber = employeeIsValid.BankCardNumber;
                employee.BankBranch = employeeIsValid.BankBranch;
                employee.TaxId = employeeIsValid.TaxId;
                employee.SocialinsuranceNumber = employeeIsValid.SocialinsuranceNumber;
                employee.RegistrationHospital = employeeIsValid.RegistrationHospital;
                employee.Specialized = employeeIsValid.Specialized;
                employee.DepartmentHint = employeeIsValid.DepartmentHint;
                employee.EducationId = employeeIsValid.EducationName!=null ? new Guid(employeeIsValid.EducationName): (Guid?)null;
                employee.ReligionId = employeeIsValid.ReligionName != null ? new Guid(employeeIsValid.ReligionName) : (Guid?)null;
                employee.FolkId = employeeIsValid.FolkName != null ? new Guid(employeeIsValid.FolkName) : (Guid?)null;
                employee.NationalityId = new Guid(employeeIsValid.NationalityName);
                employee.Actived = true;
                employee.Description = employeeIsValid.Description;
                employee.IsHead = employeeIsValid.IsHead;
                employee.CreatedAccountId = new Guid(CurrentUser.AccountId);
                employee.CreatedTime = DateTime.Now;

                _context.Entry(employee).State = EntityState.Added;
            }
            else
            {
                EmployeeModel employee = _context.EmployeeModels.Where(p => p.EmployeeId == employeeIsValid.EmployeeId).FirstOrDefault();
                if (employee != null)
                {
                    //employee.EmployeeId = Guid.NewGuid();
                    //employee.EmployeeCode = employeeIsValid.EmployeeCode;
                    //employee.TimekeepingCode = employeeIsValid.TimekeepingCode;
                    employee.FullName = employeeIsValid.FullName;
                    employee.Gender = employeeIsValid.Gender;
                    employee.PositionID = new Guid(employeeIsValid.PositionName);
                    employee.DateOfBirth = employeeIsValid.DateOfBirth;
                    employee.EmployeeStatusCategoryId = new Guid(employeeIsValid.EmployeeStatusCategoryName);
                    employee.WorkPlaceID = new Guid(employeeIsValid.WorkPlaceName);
                    employee.ShiftWorkId = new Guid(employeeIsValid.ShiftWorkName);
                    employee.DepartmentID = new Guid(employeeIsValid.DepartmentName);
                    employee.ParentId = employeeIsValid.ParentName != null ? new Guid(employeeIsValid.ParentName) : (Guid?)null;
                    employee.PlaceOfBirth = employeeIsValid.PlaceOfBirth;
                    employee.IdentityCard = employeeIsValid.IdentityCard;
                    employee.DateOfIssue = employeeIsValid.DateOfIssue;
                    employee.PlaceOfIssue = employeeIsValid.PlaceOfIssue;
                    employee.PhoneNumber = employeeIsValid.PhoneNumber;
                    employee.CompanyEmail = employeeIsValid.CompanyEmail;
                    employee.PersonalEmail = employeeIsValid.PersonalEmail;
                    employee.WorkingDate = employeeIsValid.WorkingDate;
                    employee.StandardWorkingDay = employeeIsValid.StandardWorkingDay;
                    employee.EndDate = employeeIsValid.EndDate;
                    employee.TrialJobFromDay = employeeIsValid.TrialJobFromDay;
                    employee.TrialJobEndDay = employeeIsValid.TrialJobEndDay;
                    employee.TemporaryAddress = employeeIsValid.TemporaryAddress;
                    employee.HouseholdAddress = employeeIsValid.HouseholdAddress;
                    employee.BankId = new Guid(employeeIsValid.BankName);
                    employee.BankCardNumber = employeeIsValid.BankCardNumber;
                    employee.BankBranch = employeeIsValid.BankBranch;
                    employee.TaxId = employeeIsValid.TaxId;
                    employee.SocialinsuranceNumber = employeeIsValid.SocialinsuranceNumber;
                    employee.RegistrationHospital = employeeIsValid.RegistrationHospital;
                    employee.Specialized = employeeIsValid.Specialized;
                    employee.DepartmentHint = employeeIsValid.DepartmentHint;
                    employee.EducationId = employeeIsValid.EducationName != null ? new Guid(employeeIsValid.EducationName) : (Guid?)null;
                    employee.ReligionId = employeeIsValid.ReligionName != null ? new Guid(employeeIsValid.ReligionName) : (Guid?)null;
                    employee.FolkId = employeeIsValid.FolkName != null ? new Guid(employeeIsValid.FolkName) : (Guid?)null;
                    employee.NationalityId = new Guid(employeeIsValid.NationalityName);
                    //employee.Actived = true;
                    employee.Description = employeeIsValid.Description;
                    employee.IsHead = employeeIsValid.IsHead;
                    employee.LastModifiedAccountId = new Guid(CurrentUser.AccountId);
                    employee.LastModifiedTime = DateTime.Now;
                    _context.Entry(employee).State = EntityState.Modified;
                    HistoryRepository _repository = new HistoryRepository(_context);
                    _repository.SaveUpdateHistory(employee.EmployeeId, CurrentUser.UserName, employee);
                }
                else
                {
                    return string.Format(LanguageResource.Validation_ImportExcelIdNotExist,
                                            LanguageResource.EmployeeModel, employeeIsValid.EmployeeId,
                                            string.Format(LanguageResource.Export_ExcelHeader,
                                            LanguageResource.EmployeeModel));
                }
            }
            _context.SaveChanges();
            return LanguageResource.ImportSuccess;
        }
        #endregion Insert/Update data from excel file

        #region Check data type 
        public EmployeeExcel CheckTemplate(object[] row)
        {
            EmployeeExcel data = new EmployeeExcel();
            for (int i = 0; i < row.Length; i++)
            {
                int index = i+1;
                switch (i)
                {
                    case 0:
                        //Row Index
                        data.RowIndex = index = int.Parse(row[i].ToString());
                        break;
                    case 1:
                        // ID
                        string employeeId = row[i].ToString();
                        if (string.IsNullOrEmpty(employeeId) || employeeId == "")
                        {
                            data.isNullValueId = true;
                        }
                        else
                        {
                            data.EmployeeId = Guid.Parse(employeeId);
                            data.isNullValueId = false;
                        }
                        break;
                    case 2:
                        // Mã nhân viên
                        string employeeCode = row[i].ToString();
                        if (string.IsNullOrEmpty(employeeCode))
                        {
                            data.Error = string.Format(LanguageResource.Validation_ImportRequired, string.Format(LanguageResource.Required, LanguageResource.EmployeeCode), index);
                        }
                        else
                        {
                            data.EmployeeCode = employeeCode;
                        }
                        break;
                    case 3:
                        // Mã chấm công
                        string timekeepingCode = row[i].ToString();
                        if (string.IsNullOrEmpty(timekeepingCode))
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format(LanguageResource.Required, LanguageResource.TimekeepingCode), index);
                        }
                        else
                        {
                            data.TimekeepingCode = timekeepingCode;
                        }
                        break;
                    case 4:
                        // Họ và tên
                        string fullName = row[i].ToString();
                        if (string.IsNullOrEmpty(fullName))
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format(LanguageResource.Required, LanguageResource.FullName), index);
                        }
                        else
                        {
                            data.FullName = fullName;
                        }
                        break;
                    case 5:
                        // Giới tinh
                        string gender = row[i].ToString();
                        if (string.IsNullOrEmpty(gender) || gender == "")
                        {
                            data.Gender = false;
                        }
                        else
                        {
                            data.Gender = true;
                        }
                        break;
                    case 6:
                        // ngày sinh
                        string dateOfBirth = row[i] == null?null: row[i].ToString();
                        if (string.IsNullOrEmpty(dateOfBirth) || dateOfBirth.Length <4) {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("Chiều dài tối da là 4", LanguageResource.DateOfBirth), index);
                        }
                        else
                        {
                            data.DateOfBirth = dateOfBirth;
                        }
                        break;
                    case 7:
                        // nơi sinh
                        string placeOfBirth = row[i] == null ? null : row[i].ToString();
                        data.PlaceOfBirth = placeOfBirth;
                        break;
                    case 8:
                        // tình trạng làm việc
                        string employeeStatusCategoryName = row[i] == null ? null : row[i].ToString();
                        if (string.IsNullOrEmpty(employeeStatusCategoryName))
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("chưa chọn thông tin", LanguageResource.EmployeeStatusCategory), index);
                        }
                        else
                        {
                            var Workplace = _context.EmployeeStatusCategories.FirstOrDefault(it => it.EmployeeStatusCategoryName == employeeStatusCategoryName);
                            if (Workplace != null)
                            {
                                data.EmployeeStatusCategoryName = Workplace.EmployeeStatusCategoryId.ToString();
                            }
                            else
                            {
                                data.Error += string.Format("Không tìm thấy tình trạng tên {0} ở dòng số {1} !", employeeStatusCategoryName, index);
                            }
                        }
                        break;
                    case 9:
                        //  Nơi làm việc
                        string workPlace = row[i] == null ? null : row[i].ToString();
                        if (string.IsNullOrEmpty(workPlace))
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("chưa chọn thông tin", LanguageResource.Workplace), index);
                        }
                        else
                        {
                            var Workplace = _context.WorkPlaces.FirstOrDefault(it => it.WorkPlaceName == workPlace);
                            if (Workplace != null)
                            {
                                data.WorkPlaceName = Workplace.WorkPlaceID.ToString();
                            }
                            else
                            {
                                data.Error += string.Format("Không tìm thấy Nơi làm việc tên {0} ở dòng số {1} !", workPlace, index);
                            }
                        }
                        break;
                    case 10:
                        //  Đon vị
                        string department = row[i] == null ? null : row[i].ToString();
                        if (string.IsNullOrEmpty(department))
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("chưa chọn thông tin", LanguageResource.Department), index);
                        }
                        else
                        {
                            var Department = _context.Departments.FirstOrDefault(it => it.DepartmentName == department);
                            if (Department != null)
                            {
                                data.DepartmentName = Department.DepartmentID.ToString();
                            }
                            else
                            {
                                data.Error += string.Format("Không tìm thấy đơn vị tên {0} ở dòng số {1} !", department, index);
                            }
                        }
                        break;
                    case 11:
                        //  Bộ phẩn
                        data.DepartmentHint = row[i] == null ? null : row[i].ToString();
                        break;
                    case 12:
                        //  Cấp quản lý trực tiếp
                        string parentName = row[i] == null ? null : row[i].ToString();
                        if (!string.IsNullOrEmpty(parentName))
                        {
                            var ParentName = _context.EmployeeModels.FirstOrDefault(it => it.EmployeeCode + "-" + it.FullName == parentName);
                            if (ParentName != null)
                            {
                                data.ParentName = ParentName.EmployeeId.ToString();
                            }
                            else
                            {
                                data.Error += string.Format("Không tìm thấy Cấp quản lý trực tiếp tên {0} ở dòng số {1} !", parentName, index);
                            }

                        }
                        break;
                    case 13:
                        //  Chuc Danh
                        string positionName = row[i] == null ? null : row[i].ToString();
                        if (string.IsNullOrEmpty(positionName))
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("chưa chọn thông tin", LanguageResource.Position), index);
                        }
                        else
                        {
                            var PositionName = _context.Positions.FirstOrDefault(it => it.PositionName == positionName);
                            if (PositionName != null)
                            {
                                data.PositionName = PositionName.PositionID.ToString();
                            }
                            else
                            {
                                data.Error += string.Format("Không tìm thấy Chức danh tên {0} ở dòng số {1} !", positionName, index);
                            }

                        }
                        break;
                    case 14:
                        //  Bộ phẩn
                        data.Specialized = row[i] == null ? null : row[i].ToString();
                        break;
                    case 15:
                        //  Ca làm việc (*)
                        string shiftWorkName = row[i] == null ? null : row[i].ToString();
                        if (string.IsNullOrEmpty(shiftWorkName))
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("chưa chọn thông tin", LanguageResource.ShiftWork), index);
                        }
                        else
                        {
                            var ShiftWork = _context.ShiftWorks.FirstOrDefault(it => it.ShiftWorkName == shiftWorkName);
                            if (ShiftWork != null)
                            {
                                data.ShiftWorkName = ShiftWork.ShiftWorkId.ToString();
                            }
                            else
                            {
                                data.Error += string.Format("Không tìm thấy Ca làm việc tên {0} ở dòng số {1} !", shiftWorkName, index);
                            }

                        }
                        break;
                    case 16:
                        //  Ngày công chuẩn (*)
                        string standardWorkingDay = row[i] == null ? null : row[i].ToString();
                        if (string.IsNullOrEmpty(standardWorkingDay))
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("chưa nhập thông tin", LanguageResource.StandardWorkingDay), index);
                        }
                        else
                        {
                            try
                            {
                                data.StandardWorkingDay = double.Parse(standardWorkingDay);
                            }
                            catch (Exception)
                            {
                                data.Error += string.Format("Kiểu dữ liệu cột {0} giá trị {1} ở dòng số {2} không hợp lệ!", LanguageResource.StandardWorkingDay, standardWorkingDay, index);
                            }

                        }
                        break;
                    case 17:
                        //  CMND/Thẻ căn cước (*)
                        string identityCard = row[i] == null ? null : row[i].ToString();
                        if (string.IsNullOrEmpty(identityCard) || identityCard =="")
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("chưa nhập thông tin", LanguageResource.IdentityCard),index);
                        }
                        else
                        {
                            data.IdentityCard = identityCard;
                        }
                        break;
                    case 18:
                        //  Ngày cấp (*)
                        string dateOfIssue = row[i] == null ? null : row[i].ToString();
                        if (string.IsNullOrEmpty(dateOfIssue) || dateOfIssue == "")
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("chưa nhập thông tin", LanguageResource.DateOfIssue), index);
                        }
                        else
                        {
                            try
                            {
                                data.DateOfIssue = DateTime.Parse(dateOfIssue);
                            }
                            catch (Exception)
                            {

                                data.Error += string.Format("Kiểu dữ liệu cột {0} giá trị {1} ở dòng số {2} không hợp lệ!", LanguageResource.DateOfIssue, dateOfIssue, index);
                            }

                        }
                        break;
                    case 19:
                        //  Nơi cấp (*)
                        string placeOfIssue = row[i] == null ? null : row[i].ToString();
                        if (string.IsNullOrEmpty(placeOfIssue) || placeOfIssue == "")
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("chưa nhập thông tin", LanguageResource.PlaceOfIssue), index);
                        }
                        else
                        {
                            data.PlaceOfIssue = placeOfIssue;
                        }
                        break;
                    case 20:
                        //  Điện thoại
                        data.PhoneNumber = row[i] == null ? null : row[i].ToString();
                        break;
                    case 21:
                        //  Mail công ty
                        data.CompanyEmail = row[i] == null ? null : row[i].ToString();
                        break;
                    case 22:
                        //  Mail cá nhân
                        data.PersonalEmail = row[i] == null ? null : row[i].ToString();
                        break;
                    case 23:
                        //  Ngày vào làm (*)
                        string workingDate = row[i] == null ? null : row[i].ToString();
                        if (string.IsNullOrEmpty(workingDate) || workingDate == "")
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("chưa nhập thông tin", LanguageResource.WorkingDate), index);
                        }
                        else
                        {
                            try
                            {
                                data.WorkingDate = DateTime.Parse(workingDate);
                            }
                            catch (Exception)
                            {

                                data.Error += string.Format("Kiểu dữ liệu cột {0} giá trị {1} ở dòng số {2} không hợp lệ!", LanguageResource.WorkingDate, workingDate, index);
                            }

                        }
                        break;
                    case 24:
                        //  Thử việc từ ngày (*)
                        string trialJobFromDay = row[i] == null ? null : row[i].ToString();
                        if (string.IsNullOrEmpty(trialJobFromDay) || trialJobFromDay == "")
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("chưa nhập thông tin", LanguageResource.TrialJobFromDay), index);
                        }
                        else
                        {
                            try
                            {
                                data.TrialJobFromDay = DateTime.Parse(trialJobFromDay);
                            }
                            catch (Exception)
                            {

                                data.Error += string.Format("Kiểu dữ liệu cột {0} giá trị {1} ở dòng số {2} không hợp lệ!", LanguageResource.TrialJobFromDay, trialJobFromDay, index);
                            }
                        }
                        break;
                    case 25:
                        //  Thử việc đến ngày (*)
                        string trialJobEndDay = row[i] == null ? null : row[i].ToString();
                        if (string.IsNullOrEmpty(trialJobEndDay) || trialJobEndDay == "")
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("chưa nhập thông tin", LanguageResource.TrialJobEndDay), index);
                        }
                        else
                        {
                            
                            try
                            {
                                data.TrialJobEndDay = DateTime.Parse(trialJobEndDay);
                            }
                            catch (Exception)
                            {

                                data.Error += string.Format("Kiểu dữ liệu cột {0} giá trị {1} ở dòng số {2} không hợp lệ!", LanguageResource.TrialJobEndDay, trialJobEndDay, index);
                            }
                        }
                        break;
                    case 26:
                        //  Ngày nghỉ việc
                        string endDate = row[i] == null ? null : row[i].ToString();
                        if (!string.IsNullOrEmpty(endDate) && endDate != "")
                        {
                            try
                            {
                                data.EndDate = DateTime.Parse(endDate);
                            }
                            catch (Exception)
                            {

                                data.Error += string.Format("Kiểu dữ liệu cột {0} giá trị {1} ở dòng số {2} không hợp lệ!",LanguageResource.EndDate, endDate, index);
                            }
                        }
                        break;
                    case 27:
                        //  Ngân hàng (*)
                        string bankName = row[i] == null ? null : row[i].ToString();
                        if (string.IsNullOrEmpty(bankName))
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("chưa chọn thông tin", LanguageResource.Bank), index);
                        }
                        else
                        {
                            var Bank = _context.Banks.FirstOrDefault(it => it.BankName == bankName);
                            if (Bank != null)
                            {
                                data.BankName = Bank.BankId.ToString();
                            }
                            else
                            {
                                data.Error += string.Format("Không tìm thấy Ca làm việc tên {0} ở dòng số {1} !", bankName, index);
                            }

                        }
                        break;
                    case 28:
                        //  số tải khoản
                        string bankCardNumber = row[i] == null ? null : row[i].ToString();
                        if (string.IsNullOrEmpty(bankCardNumber) || bankCardNumber == "")
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("chưa nhập thông tin", LanguageResource.BankCardNumber), index);
                        }
                        else
                        {
                            data.BankCardNumber = bankCardNumber;
                        }
                        break;
                    case 29:
                        //  Chi nhánh ngân hàng
                        string bankBranch = row[i] == null ? null : row[i].ToString();
                        if (string.IsNullOrEmpty(bankBranch) || bankBranch == "")
                        {
                            //data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("chưa nhập thông tin", LanguageResource.BankBranch), index);
                        }
                        else
                        {
                            data.BankBranch = bankBranch;
                        }
                        break;
                    case 30:
                        //  Mã số thuế
                        data.TaxId = row[i] == null ? null : row[i].ToString();
                        break;
                    case 31:
                        //  Số bảo hiểm xã hội
                        data.SocialinsuranceNumber = row[i] == null ? null : row[i].ToString();
                        break;
                    case 32:
                        //  Bệnh viện đăng ký
                        data.RegistrationHospital = row[i] == null ? null : row[i].ToString();
                        break;
                    case 33:
                        //  Trình độ văn hóa
                        string educationName = row[i] == null ? null : row[i].ToString();
                        if (!string.IsNullOrEmpty(educationName) && educationName != "")
                        {
                            var EducationName = _context.Educations.FirstOrDefault(it => it.EducationName == educationName);
                            if (EducationName != null)
                            {
                                data.EducationName = EducationName.EducationId.ToString();
                            }
                            else
                            {
                                data.Error += string.Format("Không tìm thấy Trình độ văn hóa tên {0} ở dòng số {1} !", educationName, index);
                            }
                        }
                        break;
                    case 34:
                        //  Tôn giáo
                        string religionName = row[i] == null ? null : row[i].ToString();
                        if (!string.IsNullOrEmpty(religionName) && religionName != "")
                        {
                            var Religion = _context.Religions.FirstOrDefault(it => it.ReligionName == religionName);
                            if (Religion != null)
                            {
                                data.ReligionName = Religion.ReligionId.ToString();
                            }
                            else
                            {
                                data.Error += string.Format("Không tìm thấy Tôn giáo tên {0} ở dòng số {1} !", religionName, index);
                            }
                        }
                        break;
                    case 35:
                        //  Dân tộc
                        string folkName = row[i] == null ? null : row[i].ToString();
                        if (!string.IsNullOrEmpty(folkName) && folkName != "")
                        {
                            var Folk = _context.Folks.FirstOrDefault(it => it.FolkName == folkName);
                            if (Folk != null)
                            {
                                data.FolkName = Folk.FolkId.ToString();
                            }
                            else
                            {
                                data.Error += string.Format("Không tìm thấy Dân tộc tên {0} ở dòng số {1} !", folkName, index);
                            }
                        }
                        break;
                    case 36:
                        //  Quốc tịch
                        string nationalityName = row[i] == null ? null : row[i].ToString();
                        if (!string.IsNullOrEmpty(nationalityName) && nationalityName != "")
                        {
                            var Nationalitie = _context.Nationalities.FirstOrDefault(it => it.NationalityName == nationalityName);
                            if (Nationalitie != null)
                            {
                                data.NationalityName = Nationalitie.NationalityId.ToString();
                            }
                            else
                            {
                                data.Error += string.Format("Không tìm thấy Quốc tịch tên {0} ở dòng số {1} !", nationalityName, index);
                            }
                        }
                        break;
                    case 37:
                        //  Địa chỉ tạm trú
                        data.TemporaryAddress = row[i] == null ? null : row[i].ToString();
                        break;
                    case 38:
                        //  Địa chỉ hộ khẩu (*)
                        string householdAddress = row[i] == null ? null : row[i].ToString();
                        if (string.IsNullOrEmpty(householdAddress) || householdAddress == "")
                        {
                            data.Error += string.Format(LanguageResource.Validation_ImportRequired, string.Format("chưa nhập thông tin", LanguageResource.HouseholdAddress), index);
                        }
                        else
                        {
                            data.HouseholdAddress = householdAddress;
                        }
                        break;
                    case 39:
                        //  Mô tã
                        data.Description = row[i] == null ? null : row[i].ToString();
                        break;
                    case 40:
                        // Is head
                        string isHead = row[i].ToString();
                        if (string.IsNullOrEmpty(isHead) || isHead == "")
                        {
                            data.IsHead = false;
                        }
                        else
                        {
                            data.IsHead = true;
                        }
                        break;
                }
            }
            return data;
        }
        #endregion Check data type 

        #endregion Inport data excel
       
    }
}