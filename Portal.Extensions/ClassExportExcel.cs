using OfficeOpenXml;
using OfficeOpenXml.Style;
using Portal.Constant;
using Portal.Resources;
using Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.ModelBinding;

namespace Portal.Extensions
{
    public class ClassExportExcel
    {

        public static string ExcelContentType
        {
            get
            { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
        }

        #region Convert List<T> to DataTable method
        public static DataTable ListToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dataTable = new DataTable();

            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor property = properties[i];
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            object[] values = new object[properties.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
        #endregion Convert List<T> to DataTable method

        public static byte[] ExportExcel<T>(DataTable dataTable, List<ExcelTemplate> columnsToTake, List<ExcelHeadingTemplate> heading, bool showSrNo = false)
        {
            byte[] result = null;
            bool isEdit = false;
            if (dataTable.Rows.Count > 0)
            {
                isEdit = true;
            }
            using (ExcelPackage package = new ExcelPackage())
            {
                bool isHeadingHasValue = (heading != null && heading.Count > 0);
                string headingMainContent = "";
                int headingTotalRows = 0;
                if (isHeadingHasValue == true)
                {
                    //Set necessary attributes of heading
                    headingMainContent = heading[1].Content;
                    headingTotalRows = heading.Count();
                }

                // Creates a TextInfo based on the "en-US" culture.
                TextInfo headingWithFormat = new CultureInfo("en-US", false).TextInfo;
                //SheetName
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format("{0}", headingWithFormat.ToTitleCase(headingMainContent.ToLower())));
                int startRowFrom = (isHeadingHasValue == true) ? headingTotalRows + 1 : 1;
                int startBodyColumnFrom = 1;

                if (showSrNo)
                {
                    DataColumn dataColumn = dataTable.Columns.Add(string.Format(LanguageResource.Export_ExcelRequired, LanguageResource.NumberIndex), typeof(int));
                    dataColumn.SetOrdinal(0);
                    int index = 1;
                    if (isEdit == true)
                    {
                        foreach (DataRow item in dataTable.Rows)
                        {
                            item[0] = index;
                            index++;
                        }
                    }
                    else
                    {
                        dataTable.Rows.Add(index);
                    }

                }
                //removed ignored columns
                for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                {
                    if (i == 0 && showSrNo)
                    {
                        continue;
                    }
                    if (!columnsToTake.Select(p => p.ColumnName).Contains(dataTable.Columns[i].ColumnName))
                    {
                        dataTable.Columns.RemoveAt(i);
                    }
                }

                #region initialize list contain column with format
                List<int> columnIsAllowedToEdit = new List<int>();
                List<int> columnIsDateTime = new List<int>();
                List<int> columnIsCurrency = new List<int>();
                List<int> columnIsBoolean = new List<int>();
                List<int> columnIsDropdownlist = new List<int>();
                List<int> columnIsComment = new List<int>();
                List<List<DropdownModel>> dropdownModels = new List<List<DropdownModel>>();
                List<List<DropdownIdTypeIntModel>> dropdownIdTypeIntModels = new List<List<DropdownIdTypeIntModel>>();
                List<List<DropdownIdTypeStringModel>> dropdownIdTypeStringModels = new List<List<DropdownIdTypeStringModel>>();
                #endregion

                //Dynamic Column Headers 
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    var type = typeof(T);
                    var metadataType = type.GetCustomAttributes(typeof(MetadataTypeAttribute), true)
                        .OfType<MetadataTypeAttribute>().FirstOrDefault();
                    var metaData = (metadataType != null)
                        ? ModelMetadataProviders.Current.GetMetadataForType(null, metadataType.MetadataClassType)
                        : ModelMetadataProviders.Current.GetMetadataForType(null, type);

                    var property = metaData.ModelType.GetProperty(dataTable.Columns[i].ColumnName);
                    if (property != null)
                    {
                        //DisplayName
                        var display = property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                        if (display != null)
                        {
                            //dd.GetName(): Get value by DisplayAttribute
                            dataTable.Columns[i].ColumnName = display.GetName();
                        }
                        //Required
                        var required = property.GetCustomAttribute(typeof(RequiredAttribute)) as RequiredAttribute;
                        if (required != null)
                        {
                            dataTable.Columns[i].ColumnName = string.Format(LanguageResource.Export_ExcelRequired, dataTable.Columns[i].ColumnName);
                        }
                        //Get column name if it is allowed to edit => isAllowedToEdit = true
                        var columnName = columnsToTake.Where(p => p.isAllowedToEdit == true).Select(p => p.ColumnName).ToList();
                        if (columnName.Contains(property.Name))
                        {
                            //get type of field
                            //dataType.Add(property.PropertyType.FullName);
                            columnIsAllowedToEdit.Add(i);
                        }
                        //date time 
                        var dateTime = columnsToTake.Where(p => p.isDateTime == true).Select(p => p.ColumnName).ToList();
                        if (dateTime != null && dateTime.Contains(property.Name))
                        {
                            columnIsDateTime.Add(i);
                        }
                        //currency
                        var currency = columnsToTake.Where(p => p.isCurrency == true).Select(p => p.ColumnName).ToList();
                        if (currency != null && currency.Contains(property.Name))
                        {
                            columnIsCurrency.Add(i);
                        }
                        //boolean
                        var isBoolean = columnsToTake.Where(p => p.isBoolean == true).Select(p => p.ColumnName).ToList();
                        if (isBoolean != null && isBoolean.Contains(property.Name))
                        {
                            columnIsBoolean.Add(i);
                        }
                        //dropdownlist
                        var isDropdownlist = columnsToTake.Where(p => p.isDropdownlist == true).Select(p => p.ColumnName).ToList();
                        if (isDropdownlist != null && isDropdownlist.Contains(property.Name))
                        {
                            columnIsDropdownlist.Add(i);
                            //List to dropdown
                            var dropdown = columnsToTake.Where(p => p.isDropdownlist == true && p.isAllowedToEdit == true);
                            dropdownModels = dropdown.Where(p => p.TypeId == ConstExcelController.GuidId).Select(p => p.DropdownData).ToList();
                            dropdownIdTypeIntModels = dropdown.Where(p => p.TypeId == ConstExcelController.IntId).Select(p => p.DropdownIdTypeIntData).ToList();
                            dropdownIdTypeStringModels = dropdown.Where(p => p.TypeId == ConstExcelController.StringId).Select(p => p.DropdownIdTypeStringData).ToList();
                        }
                        // comment hedder
                        var isComment = columnsToTake.Where(p => p.isComment == true).Select(p => p.ColumnName).ToList();
                        if (isComment != null && isComment.Contains(property.Name))
                        {
                            columnIsComment.Add(i);
                        }
                    }
                }


                //Insert Comment headder

                //if (columnIsComment.Count > 0)
                //{
                //    foreach (var idex in columnIsComment)
                //    {
                //        ExcelRange cellAddComment = workSheet.Cells[startRowFrom, (idex + 1), startRowFrom, (idex + 1)];
                //        cellAddComment.AddComment("text" + idex.ToString(), "Lý Quốc Cường");
                //    }
                //}

                // add the content into the Excel file
                workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);

                // autofit width of cells with small content
                int columnIndex = 1;

                //format serial number column
                ExcelRange serialNumber = workSheet.Cells[startRowFrom + startBodyColumnFrom, columnIndex, startRowFrom + dataTable.Rows.Count, columnIndex];
                serialNumber.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                foreach (DataColumn column in dataTable.Columns)
                {
                    ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                    //ExcelRange columnheaderCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.Start.Row, columnIndex];
                    if (isEdit == false)
                    {
                        columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.Start.Row, columnIndex];
                        int maxLength = columnCells.Max(cell => cell.Value.ToString().Count());
                        if (maxLength < 150)
                        {
                            workSheet.Column(columnIndex).AutoFit(18);
                        }
                    }
                    else
                    {
                        int maxLength = 0;
                        maxLength = columnCells.Max(cell => cell.Value == null ? 0 : (cell.Value).ToString().Count());
                        if (maxLength < 150)
                        {
                            workSheet.Column(columnIndex).AutoFit();
                        }
                    }
                    columnIndex++;
                }

                //If sheet has dropdownlist => color header column contains dropdown data
                var toColumn = dataTable.Columns.Count;
                if (columnIsDropdownlist != null && columnIsDropdownlist.Count > 0)
                {
                    toColumn = toColumn + 1;
                }
                // format header - bold, yellow on black
                using (ExcelRange r = workSheet.Cells[startRowFrom, startBodyColumnFrom, startRowFrom, toColumn])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#00a65a"));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                }

                // format cells - add borders
                using (ExcelRange r = workSheet.Cells[startRowFrom + 1, startBodyColumnFrom, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                {
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                }

                //Fill background color if item is allowed to edit => except number index column
                if (isEdit == true)
                {
                    //fill color
                    foreach (var index in columnIsAllowedToEdit)
                    {
                        ExcelRange cellToFill = workSheet.Cells[startRowFrom + 1, (index + 1), startRowFrom + dataTable.Rows.Count, (index + 1)];
                        cellToFill.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cellToFill.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                    }
                }
                else
                {
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        //Add new => number index column is allowed to edit => fill color
                        ExcelRange indexToFill = workSheet.Cells[startRowFrom + 1, 1];
                        indexToFill.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        indexToFill.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                        foreach (var index in columnIsAllowedToEdit)
                        {
                            int columnType = i + 1;
                            ExcelRange cellToFill = workSheet.Cells[startRowFrom + 1, (index + 1)];
                            cellToFill.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cellToFill.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                        }
                    }
                }



                //format datetime
                if (columnIsDateTime != null && columnIsDateTime.Count > 0)
                {
                    foreach (var index in columnIsDateTime)
                    {
                        ExcelRange cellToFill = workSheet.Cells[startRowFrom + 1, (index + 1), startRowFrom + dataTable.Rows.Count, (index + 1)];
                        cellToFill.Style.Numberformat.Format = "DD/MM/YYYY";
                    }
                }

                //currency
                if (columnIsCurrency != null && columnIsCurrency.Count > 0)
                {
                    foreach (var index in columnIsCurrency)
                    {
                        ExcelRange cellToFill = workSheet.Cells[startRowFrom + 1, (index + 1), startRowFrom + dataTable.Rows.Count, (index + 1)];
                        cellToFill.Style.Numberformat.Format = "#,##0";
                    }
                }

                //boolean
                if (columnIsBoolean != null && columnIsBoolean.Count > 0)
                {
                    foreach (var index in columnIsBoolean)
                    {
                        ExcelRange cellToFill = workSheet.Cells[startRowFrom + 1, (index + 1), startRowFrom + dataTable.Rows.Count, (index + 1)];
                        foreach (var item in cellToFill)
                        {
                            switch ((bool?)item.Value)
                            {
                                case true:
                                    item.Value = "X";
                                    break;
                                case false:
                                    item.Value = "";
                                    break;
                                default:
                                    item.Value = "";
                                    break;
                            }
                        }
                        cellToFill.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                }

                //dropdownlist
                if (columnIsDropdownlist != null && columnIsDropdownlist.Count > 0)
                {
                    List<ExcelRange> excelFormulaLst = new List<ExcelRange>();
                    List<ExcelRange> matchIdFormulaLst = new List<ExcelRange>();
                    List<ExcelRange> matchFormulaLst = new List<ExcelRange>();
                    #region Data Validation
                    int indexColumn = 2;
                    //Type Guid
                    if (dropdownModels != null && dropdownModels.Count > 0)
                    {
                        ExcelRange excelFormula;
                        ExcelRange matchIdFormula;
                        ExcelRange matchFormula;
                        foreach (var dropdownList in dropdownModels)
                        {
                            int indexRow = startRowFrom + 1;
                            foreach (var item in dropdownList)
                            {
                                //Id
                                ExcelRange cellId = workSheet.Cells[indexRow, (dataTable.Columns.Count + indexColumn)];
                                cellId.Value = item.Id;

                                //Name
                                ExcelRange cellName = workSheet.Cells[indexRow, (dataTable.Columns.Count + indexColumn + 1)];
                                cellName.Value = item.Name;
                                indexRow++;
                            }
                            //index of dropdown data (display name)
                            excelFormula = workSheet.Cells[startRowFrom + 4, (dataTable.Columns.Count + indexColumn + 1), indexRow + 2, (dataTable.Columns.Count + indexColumn + 1)];
                            excelFormulaLst.Add(excelFormula);
                            //index of id
                            matchIdFormula = workSheet.Cells[startRowFrom + 1, (dataTable.Columns.Count + indexColumn), indexRow - 1, (dataTable.Columns.Count + indexColumn)];
                            matchIdFormulaLst.Add(matchIdFormula);
                            //index of name
                            matchFormula = workSheet.Cells[startRowFrom + 1, (dataTable.Columns.Count + indexColumn + 1), indexRow - 1, (dataTable.Columns.Count + indexColumn + 1)];
                            matchFormulaLst.Add(matchFormula);
                            indexColumn += 3;
                        }
                    }
                    //Type int
                    if (dropdownIdTypeIntModels != null && dropdownIdTypeIntModels.Count > 0)
                    {
                        ExcelRange excelFormula;
                        ExcelRange matchIdFormula;
                        ExcelRange matchFormula;
                        foreach (var dropdownList in dropdownIdTypeIntModels)
                        {
                            int indexRow = startRowFrom + 1;
                            foreach (var item in dropdownList)
                            {
                                //Id
                                ExcelRange cellId = workSheet.Cells[indexRow, (dataTable.Columns.Count + indexColumn)];
                                cellId.Value = item.Id;

                                //Name
                                ExcelRange cellName = workSheet.Cells[indexRow, (dataTable.Columns.Count + indexColumn + 1)];
                                cellName.Value = item.Name;
                                indexRow++;
                            }
                            //index of dropdown data (display name)
                            excelFormula = workSheet.Cells[startRowFrom + 4, (dataTable.Columns.Count + indexColumn + 1), indexRow + 2, (dataTable.Columns.Count + indexColumn + 1)];
                            excelFormulaLst.Add(excelFormula);
                            //index of id
                            matchIdFormula = workSheet.Cells[startRowFrom + 1, (dataTable.Columns.Count + indexColumn), indexRow - 1, (dataTable.Columns.Count + indexColumn)];
                            matchIdFormulaLst.Add(matchIdFormula);
                            //index of name
                            matchFormula = workSheet.Cells[startRowFrom + 1, (dataTable.Columns.Count + indexColumn + 1), indexRow - 1, (dataTable.Columns.Count + indexColumn + 1)];
                            matchFormulaLst.Add(matchFormula);
                            indexColumn += 3;
                        }
                    }
                    //Type string
                    if (dropdownIdTypeStringModels != null && dropdownIdTypeStringModels.Count > 0)
                    {
                        ExcelRange excelFormula;
                        ExcelRange matchIdFormula;
                        ExcelRange matchFormula;
                        foreach (var dropdownList in dropdownIdTypeStringModels)
                        {
                            int indexRow = startRowFrom + 1;
                            foreach (var item in dropdownList)
                            {
                                //Id
                                ExcelRange cellId = workSheet.Cells[indexRow, (dataTable.Columns.Count + indexColumn)];
                                cellId.Value = item.Id;

                                //Name
                                ExcelRange cellName = workSheet.Cells[indexRow, (dataTable.Columns.Count + indexColumn + 1)];
                                cellName.Value = item.Name;
                                indexRow++;
                            }
                            //index of dropdown data (display name)
                            excelFormula = workSheet.Cells[startRowFrom + 4, (dataTable.Columns.Count + indexColumn + 1), indexRow + 2, (dataTable.Columns.Count + indexColumn + 1)];
                            excelFormulaLst.Add(excelFormula);
                            //index of id
                            matchIdFormula = workSheet.Cells[startRowFrom + 1, (dataTable.Columns.Count + indexColumn), indexRow - 1, (dataTable.Columns.Count + indexColumn)];
                            matchIdFormulaLst.Add(matchIdFormula);
                            //index of name
                            matchFormula = workSheet.Cells[startRowFrom + 1, (dataTable.Columns.Count + indexColumn + 1), indexRow - 1, (dataTable.Columns.Count + indexColumn + 1)];
                            matchFormulaLst.Add(matchFormula);
                            indexColumn += 3;
                        }
                    }
                    #endregion
                    //iterate through two collections of the same length
                    var smallestUpperBound = Math.Min(columnIsDropdownlist.Count, excelFormulaLst.Count);
                    for (var index = 0; index < smallestUpperBound; index++)
                    {
                        // Do something with collection1[index] and collection2[index]
                        //Dropdownlist
                        ExcelRange dropdownColumn = workSheet.SelectedRange[startRowFrom + 1, (columnIsDropdownlist[index] + 1), startRowFrom + dataTable.Rows.Count, (columnIsDropdownlist[index] + 1)];
                        if (isEdit == false)
                        {
                            dropdownColumn = workSheet.SelectedRange[startRowFrom + 1, (columnIsDropdownlist[index] + 1), 500, (columnIsDropdownlist[index] + 1)];
                        }
                        var listResult = dropdownColumn.DataValidation.AddListDataValidation();

                        //Get data for dropdownlist
                        //MUST turn range to absolute $$ => apply for all row in table
                        //If not absolute, next row will leave the last data row before
                        var itemNameBefore = excelFormulaLst[index];
                        //start
                        var sItemNameRow = itemNameBefore.Start.Row.ToString();
                        var sItemNameCol = itemNameBefore.Start.Column.ToString();
                        var sItemNameFromRC = ExcelCellBase.TranslateFromR1C1("R" + sItemNameRow + "C" + sItemNameCol, 0, 0);
                        //end
                        var eItemNameRow = itemNameBefore.End.Row.ToString();
                        var eItemNameCol = itemNameBefore.End.Column.ToString();
                        var eItemNameFromRC = ExcelCellBase.TranslateFromR1C1("R" + eItemNameRow + "C" + eItemNameCol, 0, 0);
                        //matchName final
                        var itemNameEnd = string.Format("{0}:{1}", sItemNameFromRC, eItemNameFromRC);

                        listResult.Formula.ExcelFormula = itemNameEnd;
                        listResult.ShowErrorMessage = true;
                        listResult.Error = LanguageResource.DataValidationError;
                    }

                    var smallestUpperBoundMatch = Math.Min(matchIdFormulaLst.Count, matchFormulaLst.Count);
                    int matchIndex = 1;
                    for (var index = 0; index < smallestUpperBound; index++)
                    {
                        //INDEX, MATCH
                        var indexMatch = workSheet.SelectedRange[(startRowFrom + 1), (dataTable.Columns.Count + matchIndex), (startRowFrom + dataTable.Rows.Count), (dataTable.Columns.Count + matchIndex)];
                        if (isEdit == false)
                        {
                            indexMatch = workSheet.SelectedRange[(startRowFrom + 1), (dataTable.Columns.Count + matchIndex), 500, (dataTable.Columns.Count + matchIndex)];
                        }
                        #region MatchId with absolute $$
                        var matchIdBefore = matchIdFormulaLst[index];
                        //start
                        var sMatchIdRow = matchIdBefore.Start.Row.ToString();
                        var sMatchIdCol = matchIdBefore.Start.Column.ToString();
                        var sMatchIdFromRC = ExcelCellBase.TranslateFromR1C1("R" + sMatchIdRow + "C" + sMatchIdCol, 0, 0);
                        //end
                        var eMatchIdRow = matchIdBefore.End.Row.ToString();
                        var eMatchIdCol = matchIdBefore.End.Column.ToString();
                        var eMatchIdFromRC = ExcelCellBase.TranslateFromR1C1("R" + eMatchIdRow + "C" + eMatchIdCol, 0, 0);
                        //matchId final
                        var matchIdEnd = string.Format("{0}:{1}", sMatchIdFromRC, eMatchIdFromRC);
                        #endregion

                        #region MatchName with absolute $$
                        var matchNameBefore = matchFormulaLst[index];
                        //start
                        var sMatchNameRow = matchNameBefore.Start.Row.ToString();
                        var sMatchNameCol = matchNameBefore.Start.Column.ToString();
                        var sMatchNameFromRC = ExcelCellBase.TranslateFromR1C1("R" + sMatchNameRow + "C" + sMatchNameCol, 0, 0);
                        //end
                        var eMatchNameRow = matchNameBefore.End.Row.ToString();
                        var eMatchNameCol = matchNameBefore.End.Column.ToString();
                        var eMatchNameFromRC = ExcelCellBase.TranslateFromR1C1("R" + eMatchNameRow + "C" + eMatchNameCol, 0, 0);
                        //matchName final
                        var matchNameEnd = string.Format("{0}:{1}", sMatchNameFromRC, eMatchNameFromRC);
                        #endregion

                        var executeIndex = workSheet.Cells[startRowFrom + 1, (columnIsDropdownlist[index] + 1)];
                        indexMatch.Formula = "IFERROR(INDEX(" + matchIdEnd + ",MATCH(" + executeIndex + "," + matchNameEnd + ",0)),\"\")";
                        matchIndex += 3;
                    }
                }

                if (isHeadingHasValue == true)
                {
                    int headerIndex = 1;
                    int headerColumnIndex = 1;
                    int colunmWarning = 2;
                    foreach (var itemHeading in heading)
                    {
                        workSheet.Cells[headerIndex, headerColumnIndex].Value = itemHeading.Content;

                        if (itemHeading.isWarning == true)
                        {
                            ExcelRange headerCell = workSheet.Cells[headerIndex, headerColumnIndex, headerIndex, colunmWarning];
                            headerCell.Merge = true;
                            //headerCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            headerCell.Style.Font.Color.SetColor(System.Drawing.Color.Red);

                            if (itemHeading.isHasBorder == true)
                            {
                                ExcelRange headerBorderCell = workSheet.Cells[headerIndex, colunmWarning + 1];
                                headerBorderCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                headerBorderCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                headerBorderCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                headerBorderCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                headerBorderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                headerBorderCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                            }
                        }
                        else
                        {
                            if (itemHeading.isCode == false)
                            {
                                ExcelRange headerCell = workSheet.Cells[headerIndex, headerColumnIndex, headerIndex, dataTable.Columns.Count];
                                headerCell.Merge = true;
                                headerCell.Style.Font.Size = 20;
                                headerCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }

                        }
                        headerIndex++;
                        workSheet.InsertRow(headerIndex, itemHeading.RowsToIgnore);
                        headerIndex += itemHeading.RowsToIgnore;
                    }

                    //workSheet.Cells[headerIndex, headerIndex].Value = heading;
                    //ExcelRange headerCell = workSheet.Cells[headerIndex, headerIndex, headerIndex, dataTable.Columns.Count];
                    //headerCell.Merge = true;
                    //headerCell.Style.Font.Size = 20;
                    //headerCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                //Font Family
                var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                allCells.Style.Font.Name = "Times New Roman";

                result = package.GetAsByteArray();
            }

            return result;
        }

        //List<T> data: List of model need to select
        //List<ExcelTemplate> ColumnsToTake: Use this template to initialize necessary attributes
        //bool showSlno: Show serial number or not. Exp: No. 1, 2, 3,...
        public static byte[] ExportExcel<T>(List<T> data, List<ExcelTemplate> ColumnsToTake, List<ExcelHeadingTemplate> Heading, bool showSlno = false)
        {
            return ExportExcel<T>(ListToDataTable<T>(data), ColumnsToTake, Heading, showSlno);
        }
    }
}
