using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.EntityModels;
using System.Data.Entity;
using Portal.Resources;
using Portal.Repositories;

namespace MasterData.Controllers
{
    public class MatrixOvertimeController : BaseController
    {
        // GET: MatrixOvertime
        //Quản lý matrix duyệt nghỉ phép
        #region Index
        [PortalAuthorization]
        public ActionResult Index()
        {
            ViewBag.OvertimeCategoryId = Data.OvertimeCategoryBag();
            return View();
        }
        public ActionResult _Search(Guid? overtimeCategoryId, bool? actived ) {
            return ExecuteSearch(() =>
            {
                var data = _context.MatrixOvertimeModels.Where(it => (overtimeCategoryId == null || it.OvertimeCategoryId == overtimeCategoryId)
                            && (actived == null || it.Actived == actived)).ToList();
                return PartialView(data);
            });
        }
        #endregion Index

        #region Create
        [PortalAuthorization]
        public ActionResult Create() {
            ViewBag.OvertimeCategoryId = Data.OvertimeCategoryBag();
            return View();
        }

        [PortalAuthorization]
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult Create(MatrixOvertimeModel create) {
            return ExecuteContainer(() =>
            {
                List<string> error = new List<string>();
                var checkLevel = _context.MatrixOvertimeModels.FirstOrDefault(it => it.OvertimeCategoryId == create.OvertimeCategoryId 
                                    && it.ApprovalLevel == create.ApprovalLevel && it.Actived == true);
                if (checkLevel != null)
                {
                    error.Add("Đã tồn tại cấp duyệt <b>" + create.ApprovalLevel.ToString() + "</b> của loại tăng ca <b>" + checkLevel.OvertimeCategory.OvertimeCategoryName+"</b>");
                }
                string[] app = create.ApprovalName.Split(';');
                if (app.Length > 0)
                {
                    foreach (var item in app)
                    {
                        if (item.ToLower() != "head") {
                            var checkEmployee = _context.EmployeeModels.FirstOrDefault(it => it.EmployeeCode == item && it.Actived == true && it.EmployeeStatusCategoryId == new Guid("F3827595-B7AA-457F-92D9-80B0E9DF458A"));
                            if (checkEmployee == null)
                            {
                                error.Add("Không tồn tại người duyệt có mã <b>" + item +"</b>");
                            }
                        }
                    }
                }
                if (error.Count > 0)
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = false,
                        Data = error
                    });
                }
                create.MatrixOvertimeId = Guid.NewGuid();
                create.CreatedAccountId = new Guid(CurrentUser.AccountId);
                create.CreatedTime = DateTime.Now;
                _context.Entry(create).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.MatrixOvertime.ToLower())
                });
            });

        }
        #endregion Create

        #region Edit
        [PortalAuthorization]
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var edit = _context.MatrixOvertimeModels.FirstOrDefault(it => it.MatrixOvertimeId == id);
            if (edit == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.ReligionId.ToLower()) });
            }
            ViewBag.OvertimeCategoryId = Data.OvertimeCategoryBag(edit.OvertimeCategoryId);
            return View(edit);
        }
        [PortalAuthorization]
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(MatrixOvertimeModel model) {
            return ExecuteContainer(() => {
                var edit = _context.MatrixOvertimeModels.FirstOrDefault(it => it.MatrixOvertimeId == model.MatrixOvertimeId);
                if (edit != null)
                {
                    List<string> error = new List<string>();
                    string[] app = model.ApprovalName.Split(';');
                    if (app.Length > 0)
                    {
                        foreach (var item in app)
                        {
                            if (item.ToLower() != "head")
                            {
                                var checkEmployee = _context.EmployeeModels.FirstOrDefault(it => it.EmployeeCode == item && it.Actived == true && it.EmployeeStatusCategoryId == new Guid("F3827595-B7AA-457F-92D9-80B0E9DF458A"));
                                if (checkEmployee == null)
                                {
                                    error.Add("Không tồn tại người duyệt có mã <b>" + item + "</b>");
                                }
                            }
                        }
                    }
                    if (error.Count > 0)
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.Created,
                            Success = false,
                            Data = error
                        });
                    }
                    edit.ApprovalName = model.ApprovalName;
                    edit.Note = model.Note;
                    edit.Actived = model.Actived;

                    HistoryRepository _repository = new HistoryRepository(_context);
                    _repository.SaveUpdateHistory(edit.MatrixOvertimeId, CurrentUser.UserName, edit);

                    edit.LastModifiedTime = DateTime.Now;
                    edit.LastModifiedAccountId = new Guid(CurrentUser.AccountId);

                    _context.Entry(edit).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.MatrixOvertime.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Error_NotExist, model.MatrixOvertimeId)
                    });
                }
            });
        }
        #endregion Edit

        #region Delete
        [PortalAuthorization]
        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var page = _context.MatrixOvertimeModels.FirstOrDefault(p => p.MatrixOvertimeId == id);
                if (page != null)
                {
                    _context.Entry(page).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.FolkId.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.FolkId.ToLower())
                    });
                }
            });
        }
        #endregion Delete

        #region Helper
        #endregion Helper
    }
}