using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

// 作者  杨伟
namespace Library
{
    public class ExportImportHelp
    {
        public static void ExportExcel(string strFilename, GridView gridivew)
        {
            string strPath = Convert.ToString(ConfigurationManager.AppSettings["ExportExcelPath"]);
            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);
            string strFullPath = strPath + strFilename + ".xlsx";
            gridivew.ExportToXlsx(strFullPath);
            System.Diagnostics.Process.Start(strFullPath);
        }


        public static void ImportExcel(GridView gridivew, ref DataTable dt)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Excel文件(*.xlsx;*.xls)()|*.xlsx;*.xls";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string strPath = dialog.FileName;

                    dt = ReadExcelToTable(strPath);

                    OpenWaitDialog("正在导入数据");
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dt.Columns[i].ColumnName = dt.Columns[i].ColumnName.Replace(" ", "").Trim();
                    }


                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (i + 1 > gridivew.RowCount)
                            break;


                        DataRow DataRow = dt.Rows[i];
                        for (int k = 0; k < dt.Columns.Count; k++)
                        {
                            Type type = null;
                            try
                            {

                                type = gridivew.VisibleColumns[k].ColumnType;
                                string strColumnName = gridivew.VisibleColumns[k].FieldName;
                                if (type.FullName.ToLower().Contains("system."))
                                    gridivew.SetRowCellValue(i, strColumnName, DataRow[strColumnName]);
                                //gridivew.SetRowCellValue(i, dt.Columns[k].ColumnName, DataRow[dt.Columns[k].ColumnName]);
                                #region
                                //Type type = gridivew.Columns[k].ColumnType;
                                //if (!type.FullName.ToLower().Contains("system."))
                                //{//如果不是系统类型，则肯定是自己类型的类型(如枚举)
                                //    try
                                //    {

                                //        type.GetProperty(dt.Columns[k].ColumnName).SetValue(ItemInfo, GetEnumName<T>(Convert.ToString(DataRow[dt.Columns[k].ColumnName])), null);
                                //    }
                                //    catch (System.Exception ex)
                                //    {
                                //        try
                                //        {
                                //            type.GetProperty(dt.Columns[k].ColumnName).SetValue(ItemInfo, (T)Enum.Parse(typeof(T), Convert.ToString(DataRow[dt.Columns[k].ColumnName])), null);
                                //        }
                                //        catch (System.Exception exException)
                                //        {

                                //        }

                                //    }
                                //}
                                //else
                                //{
                                //    gridivew.SetRowCellValue(i, dt.Columns[k].ColumnName, DataRow[dt.Columns[k].ColumnName]);
                                //}
                                #endregion
                            }
                            catch (System.Exception)
                            {

                            }
                        }



                        //ItemInfo.ItemType = this.GetEnumName<ItemType>(Convert.ToString(DataRow["ItemType"]));
                        //ItemInfo.RequiredClass = this.GetEnumName<RequiredClass>(Convert.ToString(DataRow["RequiredClass"]));
                        //ItemInfo.RequiredGender = this.GetEnumName<RequiredGender>(Convert.ToString(DataRow["RequiredGender"]));
                        //ItemInfo.RequiredType = this.GetEnumName<RequiredType>(Convert.ToString(DataRow["RequiredType"]));
                        //ItemInfo.Effect = (ItemEffect)Enum.Parse(typeof(ItemEffect), Convert.ToString(DataRow["Effect"]));
                        // ItemInfo.Rarity = this.GetEnumName<Rarity>(Convert.ToString(DataRow["Rarity"]));


                    }


                    if (gridivew.RowCount > dt.Rows.Count)
                    {//如果gridview里行数>excel里的行数，则要删除gridview里多余的行数
                        for (int i = dt.Rows.Count; i < gridivew.RowCount; i++)
                        {
                            gridivew.OptionsSelection.MultiSelect = true;
                            gridivew.SelectRow(i);
                        }
                        int[] selRow = gridivew.GetSelectedRows();
                        gridivew.DeleteSelectedRows();
                    }

                    if (gridivew.RowCount < dt.Rows.Count)
                    {//如果gridview里行数<excel里的行数，则要添加行
                        int StartCount = gridivew.RowCount;
                        for (int i = gridivew.RowCount; i < dt.Rows.Count; i++)
                        {
                            gridivew.AddNewRow();
                            DataRow DataRow = dt.Rows[i];
                            for (int k = 0; k < dt.Columns.Count; k++)
                            {
                                Type type = null;
                                try
                                {
                                    type = gridivew.VisibleColumns[k].ColumnType;
                                    string strColumnName = gridivew.VisibleColumns[k].FieldName;
                                    if (type.FullName.ToLower().Contains("system."))
                                        gridivew.SetRowCellValue(i, strColumnName, DataRow[strColumnName]);

                                }
                                catch (System.Exception)
                                {

                                }
                            }
                            //this.ItemInfoGridView.SetRowCellValue(i, "ItemName", dt.Rows[i]["ItemName"]);
                            gridivew.UpdateCurrentRow();
                        }

                        for (int i = StartCount; i < dt.Rows.Count; i++)
                        {
                            DataRow DataRow = dt.Rows[i];
                            for (int k = 0; k < dt.Columns.Count; k++)
                            {
                                Type type = null;
                                try
                                {
                                    type = gridivew.VisibleColumns[k].ColumnType;
                                    string strColumnName = gridivew.VisibleColumns[k].FieldName;
                                    if (type.FullName.ToLower().Contains("system."))
                                        gridivew.SetRowCellValue(i, strColumnName, DataRow[strColumnName]);

                                }
                                catch (System.Exception)
                                {

                                }
                            }

                            //gridivew.SetRowCellValue(i, "ItemName", dt.Rows[i]["ItemName"]);
                            gridivew.UpdateCurrentRow();
                        }
                    }

                }
            }

            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
            finally
            {
                CloseWaitDialog();
            }
        }



        public static DataTable ReadExcelToTable(string strPath)
        {
            string connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + strPath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1';";
            //string connstring = "Provider=Microsoft.JET.OLEDB.4.0;Data Source=" + strPath + ";Extended Properties='Excel8.0;HDR=YES;IMEX=1';";
            using (OleDbConnection conn = new OleDbConnection(connstring))
            {
                conn.Open();
                DataTable sheetsName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" }); //得到所有sheet的名字  
                string firstSheetName = "Sheet$";// sheetsName.Rows[0][2].ToString();
                string sql = string.Format("SELECT * FROM [{0}]", firstSheetName);

                OleDbDataAdapter ada = new OleDbDataAdapter(sql, connstring);
                DataSet set = new DataSet();
                ada.Fill(set);
                return set.Tables[0];
            }
        }





        public static T GetEnumName<T>(string description)
        {
            Type _type = typeof(T);
            foreach (FieldInfo field in _type.GetFields())
            {
                DescriptionAttribute[] _curDesc = GetDescriptAttr(field);
                if (_curDesc != null && _curDesc.Length > 0)
                {
                    if (_curDesc[0].Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException(string.Format("{0} 未能找到对应的枚举.", description), "Description");

        }

        public static object GetEnumName(string description, Type _type)
        {

            foreach (FieldInfo field in _type.GetFields())
            {
                DescriptionAttribute[] _curDesc = GetDescriptAttr(field);
                if (_curDesc != null && _curDesc.Length > 0)
                {
                    if (_curDesc[0].Description == description)
                        return field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return field.GetValue(null);
                }
            }
            throw new ArgumentException(string.Format("{0} 未能找到对应的枚举.", description), "Description");

        }

        public static DescriptionAttribute[] GetDescriptAttr(FieldInfo fieldInfo)
        {
            if (fieldInfo != null)
            {
                return (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            }
            return null;
        }




        private static WaitDialogForm wdf = null;
        private static void OpenWaitDialog(string caption)
        {
            wdf = new WaitDialogForm(caption + "...", "请等待...");

        }
        private static void CloseWaitDialog()
        {
            if (wdf != null)
            {
                wdf.Close();
            }


        }
    }
}
