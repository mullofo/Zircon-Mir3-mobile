using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;

namespace Server
{
    public class NPOIHelper
    {
        /// <summary>
        /// DataTable导出到Excel文件
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">保存位置</param>
        [Obsolete]
        public static void Export(DataTable dtSource, string strHeaderText, string strFileName)
        {
            using (MemoryStream ms = Export(dtSource, strHeaderText))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }

        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        [Obsolete]
        public static MemoryStream Export(DataTable dtSource, string strHeaderText)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();




            HSSFSheet sheet = workbook.CreateSheet(strHeaderText) as HSSFSheet;

            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "NPOI";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "文件作者信息"; //填加xls文件作者信息
                si.ApplicationName = "创建程序信息"; //填加xls文件创建程序信息
                si.LastAuthor = "最后保存者信息"; //填加xls文件最后保存者信息
                si.Comments = "作者信息"; //填加xls文件作者信息
                si.Title = "标题信息"; //填加xls文件标题信息

                si.Subject = "主题信息";//填加文件主题信息
                si.CreateDateTime = DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion

            HSSFCellStyle dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            HSSFDataFormat format = workbook.CreateDataFormat() as HSSFDataFormat;
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet() as HSSFSheet;
                    }

                    #region 表头及样式
                    //{
                    //    HSSFRow headerRow = sheet.CreateRow(0) as HSSFRow;
                    //    headerRow.HeightInPoints = 25;
                    //    headerRow.CreateCell(0).SetCellValue(strHeaderText);

                    //    HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                    //    headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    //    HSSFFont font = workbook.CreateFont() as  HSSFFont;
                    //    font.FontHeightInPoints = 20;
                    //    font.Boldweight = 700;
                    //    headStyle.SetFont(font);
                    //    headerRow.GetCell(0).CellStyle = headStyle;


                    //    sheet.AddMergedRegion(new  NPOI.SS.Util.Region(0, 0, 0, dtSource.Columns.Count - 1));
                    //    //headerRow.Dispose();
                    //}
                    #endregion


                    #region 列头及样式
                    {
                        HSSFRow headerRow = sheet.CreateRow(0) as HSSFRow;
                        HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                        HSSFFont font = workbook.CreateFont() as HSSFFont;
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                        }

                    }
                    #endregion

                    rowIndex = 1;
                }
                #endregion


                #region 填充内容
                HSSFRow dataRow = sheet.CreateRow(rowIndex) as HSSFRow;
                foreach (DataColumn column in dtSource.Columns)
                {
                    HSSFCell newCell = dataRow.CreateCell(column.Ordinal) as HSSFCell;

                    string drValue = row[column].ToString();

                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle;//格式化显示
                            break;
                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16"://整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal"://浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }

                }
                #endregion

                rowIndex++;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;


                //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet
                return ms;
            }
        }
        public static DataTable ToDataTable<T>(IList<T> vos)
        {
            Type voType = typeof(T);
            //构造数据表
            DataTable dt = new DataTable(voType.Name);
            PropertyInfo[] properties = voType.GetProperties();
            IDictionary<string, PropertyInfo> voProperties = new Dictionary<string, PropertyInfo>();
            //构造数据列
            foreach (PropertyInfo property in properties)
            {
                DataColumn col = new DataColumn(property.Name);
                col.DataType = property.PropertyType;
                col.Caption = property.Name;
                dt.Columns.Add(col);
                voProperties.Add(property.Name, property);
            }
            if (vos == null || vos.Count == 0)
            {
                return dt;
            }
            //读取记录数据
            foreach (object obj in vos)
            {
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo pro in voProperties.Values)
                {
                    dr[pro.Name] = pro.GetValue(obj, null);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }


        public static List<T> ToList<T>(DataTable dataTable) where T : class, new()
        {
            List<T> list = new List<T>();
            PropertyInfo[] properties = typeof(T).GetProperties();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    T t = Activator.CreateInstance<T>();
                    PropertyInfo[] array = properties;
                    for (int i = 0; i < array.Length; i++)
                    {
                        PropertyInfo propertyInfo = array[i];
                        try
                        {
                            if (dataTable.Columns.Contains(propertyInfo.Name) && dataRow[propertyInfo.Name] != null && propertyInfo.PropertyType.IsEnum)
                            {
                                propertyInfo.SetValue(t, Convert.ChangeType(Enum.Parse(propertyInfo.PropertyType, dataRow[propertyInfo.Name].ToString()), propertyInfo.PropertyType), null);
                            }
                            else
                            {
                                if (dataTable.Columns.Contains(propertyInfo.Name) && dataRow[propertyInfo.Name] != null)
                                {
                                    if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                                    {
                                        propertyInfo.SetValue(t, Convert.ChangeType(dataRow[propertyInfo.Name], Nullable.GetUnderlyingType(propertyInfo.PropertyType)), null);
                                    }
                                    else
                                    {
                                        propertyInfo.SetValue(t, Convert.ChangeType(dataRow[propertyInfo.Name], propertyInfo.PropertyType), null);
                                    }
                                }
                            }
                            goto IL_1A7;
                        }
                        catch
                        {
                            goto IL_1A7;
                        }
                    // break;
                    IL_1A7:;
                    }
                    list.Add(t);
                }
            }
            return list;
        }

        /// <summary>
        /// DataTable导出到Excel文件
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">保存位置</param>
        [Obsolete]
        public static void Export(Dictionary<string, DataTable> dic, string strFileName)
        {
            using (MemoryStream ms = Export(dic))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }

        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        [Obsolete]
        public static MemoryStream Export(Dictionary<string, DataTable> dic)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();

            foreach (string strSheetName in dic.Keys)
            {

                DataTable dtSource = dic[strSheetName];

                for (int i = 0; i < dtSource.Columns.Count; i++)
                {
                    DataColumn column = dtSource.Columns[i];
                    if (!column.DataType.FullName.ToLower().Contains("system."))
                    {
                        dtSource.Columns.RemoveAt(i);
                        i--;

                    }

                }





                HSSFSheet sheet = workbook.CreateSheet(strSheetName) as HSSFSheet;

                #region 右击文件 属性信息
                {
                    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                    dsi.Company = "NPOI";
                    workbook.DocumentSummaryInformation = dsi;

                    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                    si.Author = "文件作者信息"; //填加xls文件作者信息
                    si.ApplicationName = "创建程序信息"; //填加xls文件创建程序信息
                    si.LastAuthor = "最后保存者信息"; //填加xls文件最后保存者信息
                    si.Comments = "作者信息"; //填加xls文件作者信息
                    si.Title = "标题信息"; //填加xls文件标题信息

                    si.Subject = "主题信息";//填加文件主题信息
                    si.CreateDateTime = DateTime.Now;
                    workbook.SummaryInformation = si;
                }
                #endregion

                HSSFCellStyle dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                HSSFDataFormat format = workbook.CreateDataFormat() as HSSFDataFormat;
                dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

                //取得列宽
                int[] arrColWidth = new int[dtSource.Columns.Count];
                foreach (DataColumn item in dtSource.Columns)
                {
                    arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
                }
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                        if (intTemp > arrColWidth[j])
                        {
                            arrColWidth[j] = intTemp;
                        }
                    }
                }
                int rowIndex = 0;
                foreach (DataRow row in dtSource.Rows)
                {
                    #region 新建表，填充表头，填充列头，样式
                    if (rowIndex == 65535 || rowIndex == 0)
                    {
                        if (rowIndex != 0)
                        {
                            sheet = workbook.CreateSheet() as HSSFSheet;
                        }

                        #region 表头及样式
                        //{
                        //    HSSFRow headerRow = sheet.CreateRow(0) as HSSFRow;
                        //    headerRow.HeightInPoints = 25;
                        //    headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        //    HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        //    headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                        //    HSSFFont font = workbook.CreateFont() as  HSSFFont;
                        //    font.FontHeightInPoints = 20;
                        //    font.Boldweight = 700;
                        //    headStyle.SetFont(font);
                        //    headerRow.GetCell(0).CellStyle = headStyle;


                        //    sheet.AddMergedRegion(new  NPOI.SS.Util.Region(0, 0, 0, dtSource.Columns.Count - 1));
                        //    //headerRow.Dispose();
                        //}
                        #endregion


                        #region 列头及样式
                        {
                            HSSFRow headerRow = sheet.CreateRow(0) as HSSFRow;
                            HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                            headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                            HSSFFont font = workbook.CreateFont() as HSSFFont;
                            font.FontHeightInPoints = 10;
                            font.Boldweight = 700;
                            headStyle.SetFont(font);
                            foreach (DataColumn column in dtSource.Columns)
                            {
                                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                                headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                                //设置列宽
                                sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                            }

                        }
                        #endregion

                        rowIndex = 1;
                    }
                    #endregion


                    #region 填充内容
                    HSSFRow dataRow = sheet.CreateRow(rowIndex) as HSSFRow;
                    foreach (DataColumn column in dtSource.Columns)
                    {
                        HSSFCell newCell = dataRow.CreateCell(column.Ordinal) as HSSFCell;

                        string drValue = row[column].ToString();

                        switch (column.DataType.ToString())
                        {
                            case "System.String"://字符串类型
                                newCell.SetCellValue(drValue);
                                break;
                            case "System.DateTime"://日期类型
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(dateV);

                                newCell.CellStyle = dateStyle;//格式化显示
                                break;
                            case "System.Boolean"://布尔型
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                newCell.SetCellValue(boolV);
                                break;
                            case "System.Int16"://整型
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                newCell.SetCellValue(intV);
                                break;
                            case "System.Decimal"://浮点型
                            case "System.Double":
                                double doubV = 0;
                                double.TryParse(drValue, out doubV);
                                newCell.SetCellValue(doubV);
                                break;
                            case "System.DBNull"://空值处理
                                newCell.SetCellValue("");
                                break;
                            default:
                                newCell.SetCellValue("");
                                break;
                        }

                    }
                    #endregion

                    rowIndex++;
                }
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;


                //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet
                return ms;
            }
        }

        public static List<DataTable> Import(string strFileName)
        {


            List<DataTable> DataTableList = new List<DataTable>();

            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            for (int k = 0; k < hssfworkbook.NumberOfSheets; k++)
            {
                DataTable dt = new DataTable();
                HSSFSheet sheet = hssfworkbook.GetSheetAt(k) as HSSFSheet;

                dt.TableName = sheet.SheetName;
                System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

                HSSFRow headerRow = sheet.GetRow(0) as HSSFRow;
                int cellCount = headerRow.LastCellNum;

                for (int j = 0; j < cellCount; j++)
                {
                    HSSFCell cell = headerRow.GetCell(j) as HSSFCell;
                    dt.Columns.Add(cell.ToString());
                }

                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    HSSFRow row = sheet.GetRow(i) as HSSFRow;
                    DataRow dataRow = dt.NewRow();

                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                            dataRow[j] = row.GetCell(j).ToString();
                    }

                    dt.Rows.Add(dataRow);
                }

                DataTableList.Add(dt);
            }
            return DataTableList;
        }

    }
}