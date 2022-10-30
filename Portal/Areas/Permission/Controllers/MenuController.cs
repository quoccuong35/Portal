using Portal.Constant;
using Portal.EntityModels;
using Portal.Extensions;
using Portal.Resources;
using Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Permission.Controllers
{
    public class MenuController : BaseController
    {
        const string controllerCode = ConstExcelController.Menu;
        const int startIndex = 6;
        //GET: /Menu/Index
        // GET: Menu
        #region Index, _Search
        [PortalAuthorizationAttribute]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(string MenuName)
        {
            return ExecuteSearch(() =>
            {
                var MenuNameIsNullOrEmpty = string.IsNullOrEmpty(MenuName);
                var menuList = _context.MenuModels.Where(p => MenuNameIsNullOrEmpty || p.MenuName.ToLower().Contains(MenuName.ToLower()))
                                                 .OrderBy(p => p.OrderIndex)
                                                 .ToList();
                return PartialView(menuList);
            });
        }
        #endregion Index, _Search
        //GET: /Menu/Create
        #region Create
        [PortalAuthorizationAttribute]
        public ActionResult Create()
        {
            return View();
        }
        //POST: Create
        [HttpPost]
        [ValidateAjax]
        [PortalAuthorizationAttribute]
        public JsonResult Create(MenuModel model)
        {
            return ExecuteContainer(() =>
            {
                model.MenuId = Guid.NewGuid();
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.Permission_MenuModel.ToLower())
                });
            });
        }
        #endregion Create
        //GET: /Menu/Edit
        #region Edit
        [PortalAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            var menu = _context.MenuModels.FirstOrDefault(p => p.MenuId == id);
            if (menu == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Permission_MenuModel.ToLower()) });
            }
            return View(menu);
        }
        //POST: Edit
        [HttpPost]
       [PortalAuthorizationAttribute]
        public JsonResult Edit(MenuModel model)
        {
            return ExecuteContainer(() =>
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Permission_MenuModel.ToLower())
                });
            });
        }
        #endregion Edit

        #region Delete
        [HttpPost]
        [PortalAuthorizationAttribute]
        public ActionResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var menu = _context.MenuModels.FirstOrDefault(p => p.MenuId == id);
                if (menu != null)
                {
                    _context.Entry(menu).State = EntityState.Deleted;
                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Permission_MenuModel.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = ""
                    });
                }
            });
        }
        #endregion


        //GET: /Menu/ExportToExcel
        #region Export to excel
        public ActionResult ExportCreate()
        {
            List<MenuModel> menu = new List<MenuModel>();
            return Export(menu);
        }
        public ActionResult ExportEdit()
        {
            //Get data from server
            List<MenuModel> menu = _context.MenuModels.OrderBy(p => p.OrderIndex).ToList();
            return Export(menu);
        }

        public FileContentResult Export(List<MenuModel> menu)
        {
            //Columns to take
            //string[] columns = { "MenuId", "MenuName", "OrderIndex", "Icon" };
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            columns.Add(new ExcelTemplate() { ColumnName = "MenuId", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate() { ColumnName = "MenuName", isAllowedToEdit = true, isDropdownlist = true });
            columns.Add(new ExcelTemplate() { ColumnName = "OrderIndex", isAllowedToEdit = true });
            columns.Add(new ExcelTemplate() { ColumnName = "Icon", isAllowedToEdit = true });
            //Header
            string fileheader = string.Format(LanguageResource.Export_ExcelHeader, LanguageResource.Permission_MenuModel);

            //List<ExcelHeadingTemplate> heading initialize in BaseController
            //Default:
            //          1. heading[0] is controller code
            //          2. heading[1] is file name
            //          3. headinf[2] is warning (edit)
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
                Content = LanguageResource.Export_ExcelWarning1,
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
        #endregion Export to excel

        //POST: /Menu/ImportFromExcel
        #region Import from excel
        [PortalAuthorizationAttribute]
        public ActionResult Import()
        {
            DataSet ds = GetDataSetFromExcel();
            List<string> errorList = new List<string>();
            return ExcuteImportExcel(() =>
            {
                if (ds.Tables.Count > 0)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            //Get controller code from Excel file
                            string contCode = dt.Columns[0].ColumnName.ToString();
                            //Import data with accordant controller and action
                            if (contCode == controllerCode)
                            {
                                foreach (DataRow dr in dt.Rows)
                                {
                                    if (dt.Rows.IndexOf(dr) >= startIndex && !string.IsNullOrEmpty(dr.ItemArray[0].ToString()))
                                    {
                                        //Check correct template
                                        MenuViewModel menuIsValid = CheckTemplate(dr.ItemArray);

                                        if (!string.IsNullOrEmpty(menuIsValid.Error))
                                        {
                                            string error = menuIsValid.Error;
                                            errorList.Add(error);
                                        }
                                        else
                                        {
                                            string result = ExecuteImportExcelMenu(menuIsValid);
                                            if (result != LanguageResource.ImportSuccess)
                                            {
                                                errorList.Add(result);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                string error = string.Format(LanguageResource.Validation_ImportCheckController, LanguageResource.Permission_MenuModel);
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
        public string ExecuteImportExcelMenu(MenuViewModel menuIsValid)
        {
            //Check:
            //1. If MenuId == "" then => Insert
            //2. Else then => Update
            if (menuIsValid.isNullValueId == true)
            {
                MenuModel menu = new MenuModel();
                menu.MenuId = Guid.NewGuid();
                menu.MenuName = menuIsValid.MenuName;
                menu.OrderIndex = menuIsValid.OrderIndex;
                menu.Icon = menuIsValid.Icon;

                _context.Entry(menu).State = EntityState.Added;
            }
            else
            {
                MenuModel menu = _context.MenuModels.Where(p => p.MenuId == menuIsValid.MenuId).FirstOrDefault();
                if (menu != null)
                {
                    if (menu.MenuName != menuIsValid.MenuName)
                    {
                        menu.MenuName = menuIsValid.MenuName;
                    }
                    if (menu.OrderIndex != menuIsValid.OrderIndex)
                    {
                        menu.OrderIndex = menuIsValid.OrderIndex;
                    }
                    if (menu.Icon != menuIsValid.Icon)
                    {
                        menu.Icon = menuIsValid.Icon;
                    }
                    _context.Entry(menu).State = EntityState.Modified;
                }
                else
                {
                    return string.Format(LanguageResource.Validation_ImportExcelIdNotExist,
                                            LanguageResource.MenuId, menuIsValid.MenuId,
                                            string.Format(LanguageResource.Export_ExcelHeader,
                                            LanguageResource.Permission_MenuModel));
                }
            }
            _context.SaveChanges();
            return LanguageResource.ImportSuccess;
        }
        #endregion Insert/Update data from excel file

        #region Check data type 
        public MenuViewModel CheckTemplate(object[] row)
        {
            MenuViewModel menuVM = new MenuViewModel();
            for (int i = 0; i <= row.Length; i++)
            {
                //Row index
                if (i == 0)
                {
                    int rowIndex = int.Parse(row[i].ToString());
                    menuVM.RowIndex = rowIndex;
                }
                //MenuId
                else if (i == 1)
                {
                    string menuId = row[i].ToString();
                    if (string.IsNullOrEmpty(menuId))
                    {
                        menuVM.isNullValueId = true;
                    }
                    else
                    {
                        menuVM.MenuId = Guid.Parse(menuId);
                        menuVM.isNullValueId = false;
                    }
                }
                //MenuName
                else if (i == 2)
                {
                    string menuName = row[i].ToString();
                    if (string.IsNullOrEmpty(menuName))
                    {
                        menuVM.Error = string.Format(LanguageResource.Validation_ImportRequired, string.Format(LanguageResource.Required, LanguageResource.MenuName), menuVM.RowIndex);
                    }
                    else
                    {
                        menuVM.MenuName = menuName;
                    }
                }
                //OrderIndex
                else if (i == 3)
                {
                    int orderIndex = string.IsNullOrEmpty(row[i].ToString()) ? 0 : int.Parse(row[i].ToString());
                    menuVM.OrderIndex = orderIndex;
                }
                //Icon
                else if (i == 4)
                {
                    menuVM.Icon = row[i].ToString();
                }
            }
            return menuVM;
        }
        #endregion Check data type

        #endregion Import from excel
    }
}