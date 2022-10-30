using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Portal.EntityModels;

namespace Portal.Extensions
{
    public static class Data
    {
        private static PortalEntities _context =  new PortalEntities();
        // Khối
        public static SelectList DepartmentCategoryViewBag(Guid? DepartmentCategoryID = null) {
            var departmentCategoriesList = _context.DepartmentCategories.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).ToList();
            return(new SelectList(departmentCategoriesList, "DepartmentCategoryID", "DepartmentCategoryName", DepartmentCategoryID));
        }
        public static SelectList DepartmentViewBag(Guid? DepartmentID = null)
        {
            var departmentList = _context.Departments.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).ToList();
            return new SelectList(departmentList, "DepartmentID", "DepartmentName", DepartmentID);
        }
        public static SelectList PositionViewBag(Guid? PositionID = null)
        {
            var positionList = _context.Positions.Where(p => p.Actived == true
            ).OrderBy(p => p.OrderIndex).ToList();
            return  new SelectList(positionList, "PositionID", "PositionName", PositionID);
        }
        public static SelectList EmployeeStatusCategoryNameViewBag(Guid? EmployeeStatusCategoryId = null) {
            var employeeStatusCategoryList = _context.EmployeeStatusCategories.ToList();
            return new SelectList(employeeStatusCategoryList, "EmployeeStatusCategoryId", "EmployeeStatusCategoryName", EmployeeStatusCategoryId);
        }
        public static SelectList ShiftWorkViewBag(Guid? ShiftWorkId = null) {
            var shiftWorkList = _context.ShiftWorks.Where(it => it.Actived == true).ToList();
            return  new SelectList(shiftWorkList, "ShiftWorkId", "ShiftWorkName", ShiftWorkId);
        }

        public static SelectList BankViewBag(Guid? BankId = null) {
            var bankList = _context.Banks.Where(it => it.Actived == true).ToList();
            return new SelectList(bankList, "BankId", "BankName", BankId);
        }

        public static SelectList WorkPlaceViewBag(Guid? WorkPlaceID = null) {
            var workPlaceList = _context.WorkPlaces.Where(it => it.Actived == true).ToList();
            return new SelectList(workPlaceList, "WorkPlaceID", "WorkPlaceName", WorkPlaceID);
        }

        public static SelectList ReligionViewBag(Guid? ReligionId = null) {
            var religionList = _context.Religions.Where(it => it.Actived == true).ToList();
            return new SelectList(religionList, "ReligionId", "ReligionName", ReligionId);
        }

        public static SelectList NationalityViewBag(Guid? NationalityId = null) {
            var nationalityList = _context.Nationalities.Where(it => it.Actived == true).ToList();
            return new SelectList(nationalityList, "NationalityId", "NationalityName", NationalityId);
        }

        public static SelectList FolkViewBag(Guid? FolkId = null) {
            var folkList = _context.Folks.Where(it => it.Actived == true).ToList();
            return new SelectList(folkList, "FolkId", "FolkName", FolkId);
        }

        public static SelectList EducationViewBag(Guid? EducationID = null) {
            var educationList = _context.Educations.Where(it => it.Actived == true).ToList();
            return new SelectList(educationList, "EducationId", "EducationName", EducationID);
        }

        public static SelectList ParentViewBag(Guid? ParentId = null) {
            var parents = _context.EmployeeModels.Where(it => it.IsHead == true).Select(it => new { ParentId = it.EmployeeId, FullName = it.EmployeeCode +"-"+ it.FullName}).ToList();
            return new SelectList(parents, "ParentId", "FullName", ParentId);
        }

        public static SelectList LeaveCategorieBag(Guid? LeaveCategoryId = null)
        {
            var LeaveCategories = _context.LeaveCategories.Where(it => it.Actived == true).ToList();
            return new SelectList(LeaveCategories, "LeaveCategoryId", "LeaveCategoryName", LeaveCategoryId);
        }

        public static SelectList OvertimeCategoryBag(Guid? OvertimeCategoryId = null)
        {
            var overtimeCategorys = _context.OvertimeCategories.Where(it => it.Actived == true).ToList();
            return new SelectList(overtimeCategorys, "OvertimeCategoryId", "OvertimeCategoryName", OvertimeCategoryId);
        }
       
    }
}
