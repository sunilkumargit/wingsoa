/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using Ranet.Olap.Core.Data;
using Ranet.Olap.Core.Providers;

namespace Ranet.Olap.Core
{
    /// <summary>
    /// Предоставляет возможность экспорта сводной таблицы в формате Excel.
    /// </summary>
    public static class ExportHelper
    {
        private const string spreadsheetNamespace = "urn:schemas-microsoft-com:office:spreadsheet";
        private const string officeNamespace = "urn:schemas-microsoft-com:office:office";
        private const string excelNamespace = "urn:schemas-microsoft-com:office:excel";
        private const string htmlNamespace = "http://www.w3.org/2000/xmlns/";
        private const string axisStyle = "AxisStyle";
        private const string defaultStyle = "DefaultStyle";
        private const string tableStyle = "TableStyle";

        static private readonly Font fontTahoma8 = new Font("Tahoma", 8);
        static private PivotDataProvider pivotDataProvider;

        static private XmlDocument document;
        static private XmlElement worksheet;
        static private XmlElement styles;
        static private XmlElement currentRow;
        static private XmlElement table;
        private static XmlElement workbook;
       

       
        /// <summary>
        /// Экспорт в Microsoft Excel.
        /// </summary>
        /// <param name="provider">Provider.</param>
        /// <returns>Возращает строку с xml в формате "Spreadsheet".</returns>
        static public string ExportToExcel(PivotDataProvider provider)
        {
            pivotDataProvider = provider;
            document = new XmlDocument();
            
            WriteHeaders();
            
            //<DocumentProperties>
            WriteDocumentProperties();
            
            //<Styles>
            WriteDefaultStyles();

            string cubeName = provider.Provider.CellSet_Description.CubeName;
            //<Worksheet>
            worksheet = document.CreateElement("s", "Worksheet", spreadsheetNamespace);
            worksheet.SetAttribute("Protected", spreadsheetNamespace, true.GetHashCode().ToString());
            worksheet.SetAttribute("Name", spreadsheetNamespace, cubeName);
            workbook.AppendChild(worksheet);

            //<Table>
            table = document.CreateElement("s", "Table", spreadsheetNamespace);
            table.SetAttribute("StyleID", spreadsheetNamespace, tableStyle);
            worksheet.AppendChild(table);

            //Количество строк в области колонок:
            int colRowsCount = 0;
            if(pivotDataProvider.Provider.CellSet_Description.Axes.Count > 0 &&
               pivotDataProvider.Provider.CellSet_Description.Axes[0].Positions.Count > 0)
            {
                colRowsCount = pivotDataProvider.Provider.CellSet_Description.Axes[0].Positions[0].Members.Count;
            }
            //количество колонок в области строк:
            int rowColumnsCount = 0;
            if (pivotDataProvider.Provider.CellSet_Description.Axes.Count > 1 &&
                pivotDataProvider.Provider.CellSet_Description.Axes[1].Positions.Count > 0)
            {
                rowColumnsCount = pivotDataProvider.Provider.CellSet_Description.Axes[1].Positions[0].Members.Count;
            }

            //количество колонок в области колонок:
            int colColumnsCount = pivotDataProvider.ColumnsArea.ColumnsCount;
            
            //Задать ширину колонок:
            SetColumnsWidth(rowColumnsCount, colColumnsCount);

            WriteFilters();
            WriteColumns(rowColumnsCount + 1);
            WriteRows(colColumnsCount);

            //<WorksheetOptions>
            SetWorksheetOptions();
            
            return document.OuterXml.Replace("amp;#", "#");
        }

        /// <summary>
        /// Записывает заголовок Xml документа.
        /// </summary>
        static private void WriteHeaders()
        {
            //Заголовки:
            document.AppendChild(document.CreateXmlDeclaration("1.0", null, null));
            document.AppendChild(document.CreateProcessingInstruction("mso-application", "progid='Excel.Sheet'"));
            workbook = document.CreateElement("s", "Workbook", spreadsheetNamespace);
            XmlAttribute ns1 = document.CreateAttribute("xmlns:x", htmlNamespace);
            ns1.Value = excelNamespace;
            workbook.SetAttributeNode(ns1);
            XmlAttribute ns2 = document.CreateAttribute("xmlns:o", htmlNamespace);
            ns2.Value = officeNamespace;
            workbook.SetAttributeNode(ns2);
            document.AppendChild(workbook);
        }

        /// <summary>
        /// Записывает свойства документа Excel.
        /// </summary>
        static private void WriteDocumentProperties()
        {
            XmlElement properties = document.CreateElement("o", "DocumentProperties", officeNamespace);
            XmlElement author = document.CreateElement("o", "Author", officeNamespace);
            author.InnerText = "Pivot Table";
            properties.AppendChild(author);
            workbook.AppendChild(properties);
        }

        /// <summary>
        /// Добавляет стили по умолчанию для ячеек.
        /// </summary>
        static private void WriteDefaultStyles()
        {
            styles = document.CreateElement("s", "Styles", spreadsheetNamespace);
            workbook.AppendChild(styles);
            string clr = Color.LightGray.Name;
            AddStyle(axisStyle, clr, null, true, true, fontTahoma8);
            AddStyle(defaultStyle, null, null, true, true, fontTahoma8);
            AddStyle(tableStyle, null, null, false, false, null);
        }

        /// <summary>
        /// Задает ширину колонок для таблицы в Excel.
        /// </summary>
        /// <param name="rowColumnsCount">Количество колонок в области строк.</param>
        /// <param name="colColumnsCount">Количество колонок в области колонок.</param>
        static private void SetColumnsWidth(int rowColumnsCount, int colColumnsCount)
        {
            for (int i = 0; i < rowColumnsCount; i++)
            {
                XmlElement rcol = document.CreateElement("s", "Column", spreadsheetNamespace);
                table.AppendChild(rcol);
                rcol.SetAttribute("AutoFitWidth", spreadsheetNamespace, "0");
                rcol.SetAttribute("Width", spreadsheetNamespace, "200");
            }
            for (int i = 0; i < colColumnsCount; i++)
            {
                XmlElement сcol = document.CreateElement("s", "Column", spreadsheetNamespace);
                table.AppendChild(сcol);
                сcol.SetAttribute("AutoFitWidth", spreadsheetNamespace, "0");
                сcol.SetAttribute("Width", spreadsheetNamespace, "80");
            }
        }

        /// <summary>
        /// Задает параметры для страницы Excel.
        /// </summary>
        static private void SetWorksheetOptions()
        {
            XmlElement worksheetOptions = document.CreateElement("x", "WorksheetOptions", excelNamespace);
            worksheet.AppendChild(worksheetOptions);
            XmlElement protectObjects = document.CreateElement("x", "ProtectObjects", excelNamespace);
            protectObjects.InnerText = "False";
            worksheetOptions.AppendChild(protectObjects);
            XmlElement protectScenarios = document.CreateElement("x", "ProtectScenarios", excelNamespace);
            protectScenarios.InnerText = "False";
            worksheetOptions.AppendChild(protectScenarios);
            List<string> proprties = new List<string>
                                         {
                                             "Unsynced",
                                             "Selected",
                                             "AllowFormatCells",
                                             "AllowSizeCols",
                                             "AllowSizeRows",
                                             "AllowInsertCols",
                                             "AllowInsertRows",
                                             "AllowInsertHyperlinks",
                                             "AllowDeleteCols",
                                             "AllowDeleteRows",
                                             "AllowSort",
                                             "AllowFilter",
                                             "AllowUsePivotTables"
                                         };
                                                        
            foreach (string proprty in proprties)
            {
                XmlElement propertyElement = document.CreateElement("x", proprty, excelNamespace);
                propertyElement.InnerText = "True";
                worksheetOptions.AppendChild(propertyElement);
            }

   
        }

        /// <summary>
        /// Записывает ячейку.
        /// </summary>
        /// <param name="value">Значение ячейки.</param>
        /// <param name="type">Тип значения в виде строки.</param>
        /// <param name="styleId">ID стиля для ячейки.</param>
        /// <param name="indexColumn">Индекс ячейки в строке.</param>
        /// <param name="ticked">true, если нужно преде значением добавить ticked(').</param>
        /// <returns>Возращает XmlElement с ячейкой (cell).</returns>
        static private XmlElement WriteCell(string value, string type, string styleId, int indexColumn, bool ticked)
        {
            XmlElement cell = document.CreateElement("s", "Cell", spreadsheetNamespace);
            cell.SetAttribute("Index", spreadsheetNamespace, indexColumn.ToString());
            if (!string.IsNullOrEmpty(styleId))
                cell.SetAttribute("StyleID", spreadsheetNamespace, styleId);
            currentRow.AppendChild(cell);
            XmlElement data = document.CreateElement("s", "Data", spreadsheetNamespace);
            data.SetAttribute("Type", spreadsheetNamespace, type);
            data.SetAttribute("Ticked", excelNamespace, ticked.GetHashCode().ToString());
            data.InnerText = value;
            cell.AppendChild(data);

            return cell;
        }

        /// <summary>
        /// Добавляет фильтры.
        /// </summary>
        static private void WriteFilters()
        {
            int mergeLevel = pivotDataProvider.RowsArea.ColumnsCount +
                             pivotDataProvider.ColumnsArea.ColumnsCount - 2;
            
            for(int i = 2; i< pivotDataProvider.Provider.CellSet_Description.Axes.Count; i++)
            {
                string hierarchy = pivotDataProvider.Provider.CellSet_Description.Axes[i].Members[pivotDataProvider.Provider.CellSet_Description.Axes[i].Positions[0].Members[0].Id].HierarchyUniqueName;
                string level = pivotDataProvider.Provider.CellSet_Description.Axes[i].Members[pivotDataProvider.Provider.CellSet_Description.Axes[i].Positions[0].Members[0].Id].Caption;

                currentRow = document.CreateElement("s", "Row", spreadsheetNamespace);
                table.AppendChild(currentRow);
                WriteCell(hierarchy, "String", axisStyle, 1, true);
                XmlElement levelElement = WriteCell(level, "String", defaultStyle, 2, true);
                levelElement.SetAttribute("MergeAcross", spreadsheetNamespace, mergeLevel.ToString());
                    
            }
            currentRow = document.CreateElement("s", "Row", spreadsheetNamespace);
            table.AppendChild(currentRow);
        }

        /// <summary>
        /// Записывает область колонок используя CellSetDataProvider по позициям (Positions).
        /// </summary>
        /// <param name="startColumn">Колонка с которой начинать запись ячеек.</param>
        static private void WriteColumnsByPositions(int startColumn)
        {
            if (pivotDataProvider.Provider.CellSet_Description.Axes[0] != null)
            {
                int positionsCount = pivotDataProvider.Provider.CellSet_Description.Axes[0].Positions.Count;
                if (positionsCount < 0)
                    return;
                int membersCount = pivotDataProvider.Provider.CellSet_Description.Axes[0].Positions[0].Members.Count;

                for (int memberIndex = 0; memberIndex < membersCount; memberIndex++)
                {
                    //<Row>
                    currentRow = document.CreateElement("s", "Row", spreadsheetNamespace);
                    table.AppendChild(currentRow);

                    int indexColumn = startColumn;
                    foreach (PositionData position in pivotDataProvider.Provider.CellSet_Description.Axes[0].Positions)
                    {
                        MemberData member = pivotDataProvider.Provider.CellSet_Description.Axes[0].Members[position.Members[memberIndex].Id];

                        string value = member.Caption;
                        WriteCell(value, "String", axisStyle, indexColumn, true);
                        indexColumn++;
                    }
                }
            }
        }

        /// <summary>
        /// Записывает область колонок.
        /// </summary>
        /// <param name="startColumn">Колонка, с которой начинать запись ячеек.</param>
        static private void WriteColumns(int startColumn)
        {
            int rowsCount = pivotDataProvider.ColumnsArea.RowsCount;
            for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
            {
                int drill;
                pivotDataProvider.ColumnsArea.DrillDepth.TryGetValue(rowIndex, out drill);
                
                currentRow = document.CreateElement("s", "Row", spreadsheetNamespace);
                currentRow.SetAttribute("AutoFitHeight", spreadsheetNamespace, 1.ToString());
                currentRow.SetAttribute("Height", spreadsheetNamespace, (12 + 12*drill).ToString());
                table.AppendChild(currentRow);
                WriteColumnsAreaRow(pivotDataProvider.ColumnsArea.Members, startColumn, rowIndex);
            }
        }

        /// <summary>
        /// Записывает строку из области колонок.
        /// </summary>
        /// <param name="members"></param>
        /// <param name="startColumn">Начальная колонка.</param>
        /// <param name="rowIndex">Номер записываемой строки.</param>
        static void WriteColumnsAreaRow(List<PivotMemberItem> members, int startColumn, int rowIndex)
        {
            foreach (PivotMemberItem memberItem in members)
            {
                if (memberItem.RowIndex == rowIndex)
                {
                    int childrenCount = memberItem.ChildrenSize;
                    //GetAllChildrenCount(memberItem, out childrenCount, 0);

                    string s = string.Empty;
                    //if (memberItem.PivotDrillDepth > 0)
                    //{
                    //    s = s + "&#10;";
                    //}
                    for (int i = 0; i < memberItem.PivotDrillDepth; i++)
                    {
                        s = s + "&#10;";
                    }
                    string caption = s + memberItem.Member.Caption;
                    int mergeAcross = (childrenCount == 0 ? 0 : (childrenCount - 1));
                    XmlElement cell = WriteCell(caption, "String", axisStyle, memberItem.ColumnIndex + startColumn, true);
                    cell.SetAttribute("MergeAcross", spreadsheetNamespace, mergeAcross.ToString());
                }
                WriteColumnsAreaRow(memberItem.Children, startColumn, rowIndex);
                WriteColumnsAreaRow(memberItem.DrillDownChildren, startColumn, rowIndex);
            }
        }

        ///// <summary>
        ///// Получает количечство всех дочерних элементов (включая дочерних дочерних :))
        ///// </summary>
        ///// <param name="memberItem"></param>
        ///// <param name="count"></param>
        ///// <param name="curent"></param>
        //static private void GetAllChildrenCount(PivotMemberItem memberItem, out int count, int curent)
        //{
        //    count = curent;
        //    if (memberItem.Children.Count > 0)
        //    {
        //        curent = curent + memberItem.Children.Count;
        //        foreach (PivotMemberItem v in memberItem.Children)
        //        {
        //            GetAllChildrenCount(v, out count, curent);
        //        }
        //    }
        //}

        /// <summary>
        /// Записывает область строк используя CellSetDataProvider по позициям (Positions).
        /// </summary>
        /// <param name="columnsCount">Количество колонок в области значений.</param>
        static private void WriteRowsByPositions(int columnsCount)
        {
            if (pivotDataProvider.Provider.CellSet_Description.Axes[1] != null)
            {
                //строка в pivotDataProvider.Provider.CellSet_Description.Cells
                int cellRowNumber = 0;
                foreach (PositionData position in pivotDataProvider.Provider.CellSet_Description.Axes[1].Positions)
                {
                    //<Row>
                    currentRow = document.CreateElement("s", "Row", spreadsheetNamespace);
                    table.AppendChild(currentRow);
                    int indexCell = 1;
                    foreach (PositionMemberData pos_member in position.Members)
                    {
                        MemberData member = pivotDataProvider.Provider.CellSet_Description.Axes[1].Members[pos_member.Id];
                        string value = member.Caption;
                        WriteCell(value, "String", axisStyle, indexCell, true);
                        indexCell++;
                    }
                    WriteDataRow(cellRowNumber, columnsCount, indexCell);
                    cellRowNumber++;
                }
            }
        }

        /// <summary>
        /// Записывает область строк и значания построчно.
        /// </summary>
        /// <param name="columnsCount">Количество колонок в области значений.</param>
        static private void WriteRows(int columnsCount)
        {
            for (int cellRowNumber = 0; cellRowNumber < pivotDataProvider.RowsArea.RowsCount; cellRowNumber++)
            {
                //<Row>
                currentRow = document.CreateElement("s", "Row", spreadsheetNamespace);
                table.AppendChild(currentRow);
               
                WriteRowMembers(pivotDataProvider.RowsArea.Members, cellRowNumber);
                WriteDataRow(cellRowNumber, columnsCount, pivotDataProvider.RowsArea.ColumnsCount + 1);
            }
        }

        /// <summary>
        /// Записывает строку из области строк и строку из области значений.
        /// </summary>
        /// <param name="members"></param>
        /// <param name="row">Номер строки.</param>
        static private void WriteRowMembers(List<PivotMemberItem> members, int row)
        {
           
            foreach (PivotMemberItem memberItem in members)
            {
                if (row == memberItem.RowIndex)
                {
                    int childrenCount = memberItem.ChildrenSize;
                    //GetAllChildrenCount(memberItem, out childrenCount, 0);
                    string s = string.Empty;
                    for(int i =0; i<memberItem.PivotDrillDepth; i++)
                    {
                        s = s + "       ";
                    }
                    
                    string caption = s + memberItem.Member.Caption;
                    int mergeDown = (childrenCount == 0 ? 0 : (childrenCount - 1));
                    XmlElement cell = WriteCell(caption, "String", axisStyle, memberItem.ColumnIndex + 1, true);
                    cell.SetAttribute("MergeDown", spreadsheetNamespace, mergeDown.ToString());
                }
                WriteRowMembers(memberItem.Children, row);
                WriteRowMembers(memberItem.DrillDownChildren, row);
            }
        }

        /// <summary>
        /// Получает Формат строки по DisplayValue
        /// </summary>
        /// <param name="displayValue"></param>
        /// <returns></returns>
        private static string GetNumberFormat(string displayValue)
        {
            if (String.IsNullOrEmpty(displayValue))
                return String.Empty;
            string strFormat = String.Empty;//"#, ";
            bool isDr = false;
            int j = 0;
            if (displayValue[0] == '-')
                j = 1;
            for (int i = j; i < displayValue.Length; i++)
            {
                if (displayValue[i] == '.')
                {
                    isDr = true;
                }
                if (Char.IsNumber(displayValue[i]))
                {
                    if (isDr)
                        strFormat = strFormat + "0";
                    else
                        strFormat = strFormat + "#";
                }
                else
                {
                    if (displayValue[i] == '$')
                        strFormat = strFormat + "\\$";
                    else
                        strFormat = strFormat + displayValue[i];
                }
            }
            return strFormat;
        }

        /// <summary>
        /// Добавить строку со значениями.
        /// </summary>
        /// <param name="row">Номер строки.</param>
        /// <param name="columnsCount">Количество колонок в области значений.</param>
        /// <param name="startColumn">Начальная колонка, для области значений.</param>
        static private void WriteDataRow(int row, int columnsCount, int startColumn)
        {
           
            int indexColumn = startColumn;
            for (int col = 0; col < columnsCount; col++)
            {
                CellData cell = pivotDataProvider.Provider.CellSet_Description.GetCellDescription(col, row);
                CellValueData valueData = cell.Value;
                
                string color = string.Empty;
                object backColor = valueData.GetPropertyValue("BACK_COLOR");
                if (backColor != null)
                {
                    int colorARGB = Convert.ToInt32(backColor);
                    int colorOLE = ColorTranslator.ToOle(Color.FromArgb(colorARGB));
                    color = "#" + Color.FromArgb(colorOLE).Name;
                }

                string value = string.Empty;
                string type = "String";
                if (valueData.Value != null)
                {
                    type = GetTypeForExcel(valueData.Value);
                    value = valueData.Value.ToString();
                    value = value.Replace(',', '.');
                }

                string numberFormate = string.Empty;
                if (valueData.GetPropertyValue("FORMAT_STRING") != null)
                {
                    numberFormate = valueData.GetPropertyValue("FORMAT_STRING").ToString();
                }
                if (numberFormate == "Currency" || (String.IsNullOrEmpty(numberFormate) && type =="Number"))
                    numberFormate = GetNumberFormat(valueData.DisplayValue);

                string styleId = defaultStyle;
                if (valueData.Value != null)
                    styleId = FindStyle(color, numberFormate, !cell.Value.CanUpdate);
               

                
                if (string.IsNullOrEmpty(styleId))
                {
                    styleId = "Style" +row + col;
                    AddStyle(styleId, color, numberFormate, !cell.Value.CanUpdate, true, fontTahoma8);
                }

                WriteCell(value, type, styleId, indexColumn, false);
                indexColumn++;
            }
        }

        static private string GetTypeForExcel(object obj)
        {
            string typeStr = "Number";
            Type objType = obj.GetType();
            if (objType == typeof(string))
                typeStr = "String";
            if (objType == typeof(DateTime))
                typeStr = "DateTime";
            if (objType == typeof(bool))
                typeStr = "Boolean";
            return typeStr;
        }

        /// <summary>
        /// Ищет стиль по параметрам.
        /// </summary>
        /// <param name="color">Цвет.</param>
        /// <param name="numberFormate">Формат строки.</param>
        /// <param name="isProtect">true, если ячейка защищенная.</param>
        /// <returns>Id стиля.</returns>
        static private string FindStyle(string color, string numberFormate, bool isProtect)
        {
            string styleId = null;
            foreach (XmlElement style in styles.ChildNodes)
            {
                bool formatFlag = false;
                bool protectedFlag = false;
                bool colorFlag = false;
                foreach (XmlElement property in style.ChildNodes)
                {
                    string value;
                    if (property.Name == "s:Interior")
                    {
                        value = property.GetAttribute("Color");
                        if (color == value)
                            colorFlag = true;
                        else
                            break;
                    }
                    if (property.Name == "s:Protection")
                    {
                        value = property.GetAttribute("Protected");
                        if (isProtect.GetHashCode().ToString() == value)
                            protectedFlag = true;
                        else
                            break;
                    }
                    if (property.Name == "s:NumberFormat")
                    {
                        value = property.GetAttribute("Format");
                        if (numberFormate == value)
                            formatFlag = true;
                        else
                            break;
                    }
                }
                if (colorFlag && protectedFlag && formatFlag)
                {
                    styleId = style.GetAttribute("ID");
                    return styleId;
                }
            }
            return styleId;
        }

        /// <summary>
        /// Добовляет стиль.
        /// </summary>
        /// <param name="styleId">ID стиля.</param>
        /// <param name="backcolor">Цвет фоня для ячейки.</param>
        /// <param name="numberFormat">Формат строки.</param>
        /// <param name="protect">true, если ячейка защищенная.</param>
        /// <param name="borderPresence">Присутсвие рамки.</param>
        /// <param name="font">Шрифт.</param>
        static private void AddStyle(string styleId, string backcolor, string numberFormat, bool protect, bool borderPresence, Font font)
        {
            XmlElement style = document.CreateElement("s", "Style", spreadsheetNamespace);
            style.SetAttribute("ID", spreadsheetNamespace, styleId);
            styles.AppendChild(style);

            XmlElement interiorStyle = document.CreateElement("s", "Interior", spreadsheetNamespace);
            if (!string.IsNullOrEmpty(backcolor))
            {
                interiorStyle.SetAttribute("Color", spreadsheetNamespace, backcolor);
                interiorStyle.SetAttribute("Pattern", spreadsheetNamespace, "Solid");
            }
            style.AppendChild(interiorStyle);


            XmlElement protection = document.CreateElement("s", "Protection", spreadsheetNamespace);
            protection.SetAttribute("Protected", spreadsheetNamespace, protect.GetHashCode().ToString());
            style.AppendChild(protection);

            XmlElement numberFormatElement = document.CreateElement("s", "NumberFormat", spreadsheetNamespace);
            if (!string.IsNullOrEmpty(numberFormat))
                numberFormatElement.SetAttribute("Format", spreadsheetNamespace, numberFormat);
            style.AppendChild(numberFormatElement);

            XmlElement alignmentElement = document.CreateElement("s", "Alignment", spreadsheetNamespace);
            alignmentElement.SetAttribute("WrapText", spreadsheetNamespace, true.GetHashCode().ToString());
            alignmentElement.SetAttribute("Vertical", spreadsheetNamespace, "Top");
            style.AppendChild(alignmentElement);
            
            XmlElement bordersElement = document.CreateElement("s", "Borders", spreadsheetNamespace);
            style.AppendChild(bordersElement);
            if (borderPresence)
            {
                List<string> sides = new List<string> {"Bottom", "Left", "Right", "Top"};
                foreach (string s in sides)
                {
                    XmlElement borderElement = document.CreateElement("s", "Border", spreadsheetNamespace);
                    borderElement.SetAttribute("Position", spreadsheetNamespace, s);
                    borderElement.SetAttribute("LineStyle", spreadsheetNamespace, "Continuous");
                    borderElement.SetAttribute("Weight", spreadsheetNamespace, "1");
                    bordersElement.AppendChild(borderElement);
                }
            }

            if (font != null)
            {
                XmlElement fontElement = document.CreateElement("s", "Font", spreadsheetNamespace);
                style.AppendChild(fontElement);
                fontElement.SetAttribute("FontName", spreadsheetNamespace, font.Name);
                fontElement.SetAttribute("Size", spreadsheetNamespace, font.Size.ToString());
            }
        }
    }
}
