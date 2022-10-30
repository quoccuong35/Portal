using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Portal.EntityModels
{
    [MetadataTypeAttribute(typeof(DepartmentCategory.MetaData))]
    public partial class DepartmentCategory
    {
        internal sealed class MetaData
        {
            public System.Guid DepartmentCategoryID { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DepartmentCategoryCode")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string DepartmentCategoryCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DepartmentCategoryName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string DepartmentCategoryName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
            public string Description { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedUser")]
            public string CreatedUser { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
            public string OrderIndex { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedTime")]
            public Nullable<System.DateTime> CreatedTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedUser")]
            public string LastModifiedUser { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedTime")]
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }

    }
    [MetadataTypeAttribute(typeof(Position.MetaData))]
    public partial class Position
    {
        internal sealed class MetaData
        {
            public System.Guid PositionID { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PositionCode")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string PositionCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PositionName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string PositionName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
            public string Description { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccountId")]
            public Nullable<System.Guid> CreatedAccountId { get; set; }

            //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [RegularExpression("([0-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
            public string OrderIndex { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedTime")]
            public Nullable<System.DateTime> CreatedTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedAccountId")]
            public Nullable<System.Guid> LastModifiedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedTime")]
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }

    }
    [MetadataTypeAttribute(typeof(WorkPlace.MetaData))]
    public partial class WorkPlace
    {
        internal sealed class MetaData
        {
            public System.Guid WorkPlaceID { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkPlaceName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string WorkPlaceName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
            public string Description { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccountId")]
            public Nullable<System.Guid> CreatedAccountId { get; set; }

            //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [RegularExpression("([0-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
            public string OrderIndex { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedTime")]
            public Nullable<System.DateTime> CreatedTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedAccountId")]
            public Nullable<System.Guid> LastModifiedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedTime")]
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }

    }
    [MetadataTypeAttribute(typeof(ShiftWork.MetaData))]
    public partial class ShiftWork
    {
        internal sealed class MetaData
        {
            public System.Guid ShiftWorkId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ShiftWorkCode")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string ShiftWorkCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ShiftWorkName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string ShiftWorkName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StartTime")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public TimeSpan StartTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EndTimeBetweenShift")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public TimeSpan EndTimeBetweenShift { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StartTimeBetweenShift")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public TimeSpan StartTimeBetweenShift { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EndTime_1")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public TimeSpan EndTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OvertimeStartTime")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public TimeSpan OvertimeStartTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalWorkTime")]
            [Range(8,12)]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [DisplayFormat(DataFormatString = @"{0:0\.0}", ApplyFormatInEditMode = true)]
            public double TotalWorkTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "NumberMinuteLate")]
            [RegularExpression("([0-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            [Range(0, 60)]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public int NumberMinuteLate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
            public string Description { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccountId")]
            public Nullable<System.Guid> CreatedAccountId { get; set; }

            //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [RegularExpression("([0-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
            public string OrderIndex { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "StandardHour")]

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StandardHour")]
            public double StandardHour { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "IsNightShift")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IsNightShift")]
            public bool IsNightShift { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedTime")]
            public Nullable<System.DateTime> CreatedTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedAccountId")]
            public Nullable<System.Guid> LastModifiedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedTime")]
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }
    }
    [MetadataTypeAttribute(typeof(Folk.MetaData))]
    public partial class Folk {
        internal sealed class MetaData {

            public System.Guid FolkId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FolkCode")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string FolKCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FolkName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string FolkName { get; set; }

            [RegularExpression("([0-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
            public int OrderIndex { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
            public string Description { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccountId")]
            public Nullable<System.Guid> CreatedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedTime")]
            public Nullable<System.DateTime> CreatedTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedAccountId")]
            public Nullable<System.Guid> LastModifiedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedTime")]
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(Religion.MetaData))]
    public partial class Religion
    {
        internal sealed class MetaData
        {
            public System.Guid ReligionId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ReligionCode")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string ReligionCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ReligionName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string ReligionName { get; set; }

            [RegularExpression("([0-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
            public int OrderIndex { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
            public string Description { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccountId")]
            public Nullable<System.Guid> CreatedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedTime")]
            public Nullable<System.DateTime> CreatedTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedAccountId")]
            public Nullable<System.Guid> LastModifiedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedTime")]
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }
    }
    [MetadataTypeAttribute(typeof(Education.MetaData))]
    public partial class Education
    {
        internal sealed class MetaData
        {
            public System.Guid EducationId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EducationName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string EducationName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
            public string Description { get; set; }

            //[RegularExpression("([0-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
            //public int OrderIndex { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccountId")]
            public Nullable<System.Guid> CreatedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedTime")]
            public Nullable<System.DateTime> CreatedTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedAccountId")]
            public Nullable<System.Guid> LastModifiedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedTime")]
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(Nationality.MetaData))]
    public partial class Nationality
    {
        internal sealed class MetaData
        {
            public System.Guid NationalityId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "NationalityCode")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string NationalityCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "NationalityName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string NationalityName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
            public string Description { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccountId")]
            public Nullable<System.Guid> CreatedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedTime")]
            public Nullable<System.DateTime> CreatedTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedAccountId")]
            public Nullable<System.Guid> LastModifiedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedTime")]
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(LeaveCategory.MetaData))]
    public partial class LeaveCategory {
        internal sealed class MetaData {
            public System.Guid LeaveCategoryId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LeaveCategoryCode")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string LeaveCategoryCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LeaveCategoryName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string LeaveCategoryName { get; set; }

            [RegularExpression("([0-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
            public int OrderIndex { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
            public string Description { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccountId")]
            public Nullable<System.Guid> CreatedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedTime")]
            public Nullable<System.DateTime> CreatedTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedAccountId")]
            public Nullable<System.Guid> LastModifiedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedTime")]
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }
    }
    [MetadataTypeAttribute(typeof(Bank.MetaData))]
    public partial class Bank
    {
        internal sealed class MetaData {
            public System.Guid BankId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BankCode")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string BankCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BankName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string BankName { get; set; }

            [RegularExpression("([0-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
            public int OrderIndex { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
            public string Description { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccountId")]
            public Nullable<System.Guid> CreatedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedTime")]
            public Nullable<System.DateTime> CreatedTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedAccountId")]
            public Nullable<System.Guid> LastModifiedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedTime")]
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }

    }

    [MetadataTypeAttribute(typeof(MaTrixFurloughModel.MetaData))]
    public partial class MaTrixFurloughModel
    {
        internal sealed class MetaData
        {
            public System.Guid MaTrixFurloughID { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MaTrixFurloughName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string MaTrixFurloughName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Department")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
            public string DepartmentID { get; set; }

            [RegularExpression("([0-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
            public int FromDay { get; set; }

            [RegularExpression("([0-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
            public int ToDay { get; set; }

            [RegularExpression("([0-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ApprovalLevel")]
            public int ApprovalLevel { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ApprovalName")]
            public string ApprovalName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
            public string Note { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccountId")]
            public Nullable<System.Guid> CreatedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedTime")]
            public Nullable<System.DateTime> CreatedTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedAccountId")]
            public Nullable<System.Guid> LastModifiedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedTime")]
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }

    }
    // Loại tang ca
    [MetadataTypeAttribute(typeof(OvertimeCategory.MetaData))]
    public partial class OvertimeCategory
    {
        internal sealed class MetaData
        {
            public System.Guid OvertimeCategoryId { get; set; }
          

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OvertimeCategoryName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string OvertimeCategoryName { get; set; }

            [RegularExpression("([0-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
            public int OrderIndex { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
            public string Description { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccountId")]
            public Nullable<System.Guid> CreatedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedTime")]
            public Nullable<System.DateTime> CreatedTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedAccountId")]
            public Nullable<System.Guid> LastModifiedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedTime")]
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }

    }
    // Matrix tăng ca

    [MetadataTypeAttribute(typeof(MatrixOvertimeModel.MetaData))]
    public partial class MatrixOvertimeModel
    {
        internal sealed class MetaData
        {
            public System.Guid MatrixOvertimeId { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OvertimeCategoryName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required_Dropdownlist")]
            public string OvertimeCategoryId { get; set; }


            [RegularExpression("([0-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ApprovalLevel")]
            public int ApprovalLevel { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ApprovalName")]
            public string ApprovalName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
            public string Note { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccountId")]
            public Nullable<System.Guid> CreatedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedTime")]
            public Nullable<System.DateTime> CreatedTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedAccountId")]
            public Nullable<System.Guid> LastModifiedAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedTime")]
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }

    }
}
