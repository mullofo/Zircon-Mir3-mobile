using MirDB;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Server.Envir;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static MirDB.Association;

namespace Server.Helpers
{
    public class HelperExcel<T>
         where T : DBObject, new()
    {
        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="inSub">是否含关联表</param>
        public static void ExportExcel(bool inSub = false)
        {
            var type = typeof(T);

            string strPath = Path.Combine(Config.ExportPath, type.Name);
            ExportExcel(inSub, strPath);

            MessageBox.Show($"导出成功!", "提示");
        }
        public static void ExportExcel(bool inSub, string strPath)
        {
            if (inSub)
            {
                var type = typeof(T);

                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                var infos = type.GetProperties();
                var types = infos.Where(p => p.PropertyType.Name.IndexOf("DBBindingList") != -1).Select(p => p.PropertyType.GetGenericArguments()[0]);
                foreach (var item in types)
                {
                    var exportType = typeof(HelperExcel<>).MakeGenericType(item);
                    exportType.GetMethod("ExportExcel", new Type[] { typeof(bool), typeof(string) }).Invoke(null, new object[] { true, strPath });
                }
                ExportExcel(strPath);
            }
            else
            {
                ExportExcel(Config.ExportPath);
            }
        }

        //各对象值,如MapInfo.1=比奇城
        static Dictionary<string, string> Keys = new Dictionary<string, string>();
        //各对象的描述字段，如MapInfo=Description
        static Dictionary<string, string> Descriptions = new Dictionary<string, string>();
        /// <summary>
        /// 导出excel
        /// </summary>
        public static void ExportExcel(string strPath)
        {
            if (string.IsNullOrEmpty(strPath))
            {
                strPath = Config.ExportPath;
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);
            }

            IList<T> entitys = SMain.Session.GetCollection<T>().Binding;
            if (entitys?.Count == 0)
            {
                return;
            }
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet");
            IRow Title = null;
            IRow rows = null;
            Type entityType = entitys[0].GetType();

            string fileName = entityType.Name;    //fileName=数据库集的名字
            string strFullPath = Path.Combine(strPath, fileName + ".xlsx");   //完整路径=数据库集的名字加xlsx

            PropertyInfo[] entityProperties = entityType.GetProperties();  //属性信息 实体属性=实体类型.获取属性

            Title = sheet.CreateRow(0);   //标题= 创建工作表 创建行
            List<string> Ignores = new List<string>();  //忽略其他绑定列表
            var fileIndex = 1;
            var temp = 0;
            for (int k = 0; k < entityProperties.Length; k++)
            {
                var ignore = entityProperties[k].GetCustomAttribute(typeof(IgnoreProperty));  //获取自定义属性(忽略属性)
                if (ignore != null || entityProperties[k].PropertyType.Name.IndexOf("DBBindingList") != -1)//排除属性
                {
                    Ignores.Add(entityProperties[k].Name);
                    temp++;
                    continue;
                }
                Title.CreateCell(k - temp).SetCellValue(entityProperties[k].Name);
            }
            var rowindex = 0;
            for (int i = 0; i < entitys.Count; i++)
            {
                temp = 0;
                rows = sheet.CreateRow(rowindex + 1);
                object entity = entitys[i];
                for (int j = 0; j < entityProperties.Length; j++)
                {
                    var ignore = entityProperties[j].GetCustomAttribute(typeof(IgnoreProperty));
                    if (ignore != null || entityProperties[j].PropertyType.Name.IndexOf("DBBindingList") != -1)//排除属性
                    {
                        temp++;
                        continue;
                    }
                    var v = entityProperties[j].GetValue(entity) ?? "";
                    if (string.IsNullOrEmpty(v.ToString()))
                    {
                        rows.CreateCell(j - temp).SetCellValue("");
                    }
                    else if (entityProperties[j].PropertyType.IsArray
                        || entityProperties[j].PropertyType.IsGenericType
                        || entityProperties[j].PropertyType == typeof(BitArray))
                    {
                        var json = JsonConvert.SerializeObject(v, new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            ContractResolver = new JsonPropertyContractResolver(Ignores)
                        });

                        if (json.Length > 30000)
                        {
                            var txtFile = Path.Combine(strPath, $"{fileName}_{i} _{j}.txt");
                            using (var txt = new FileStream(txtFile, FileMode.Create))
                            {
                                using (var sw = new StreamWriter(txt, Encoding.UTF8))
                                {
                                    sw.Write(json);
                                }
                            }
                            rows.CreateCell(j - temp).SetCellValue(txtFile);
                        }
                        else
                        {
                            rows.CreateCell(j - temp).SetCellValue(json);

                        }
                    }
                    else if (entityProperties[j].PropertyType.Namespace.StartsWith("Library.SystemModels") && entityProperties[j].PropertyType.IsClass)
                    {
                        var index = string.Empty;
                        var vType = v.GetType().GetProperties().FirstOrDefault(p => p.Name == "Index");
                        if (vType != null)
                        {
                            index = vType.GetValue(v).ToString();
                        }

                        var session = SMain.Session.GetType();
                        var cllectionMethod = session.GetMethod("GetCollection", new Type[] { });
                        var collection = cllectionMethod.MakeGenericMethod(entityProperties[j].PropertyType).Invoke(SMain.Session, null);
                        var list = collection.GetType().GetField("Binding").GetValue(collection) as IEnumerable<object>;
                        var key = entityProperties[j].PropertyType.Name + "." + index;
                        if (Keys.TryGetValue(entityProperties[j].PropertyType.Name + "." + index, out string desc))
                        {
                            index += "|" + desc;
                        }
                        else
                        {
                            //取得index值，以及对应描述值
                            foreach (var item in list)
                            {
                                if (!Descriptions.TryGetValue(item.GetType().Name, out string descProp))
                                {
                                    foreach (var prop in item.GetType().GetProperties())
                                    {
                                        if (prop.GetCustomAttribute<ExportAttribute>() != null)
                                        {
                                            Descriptions[item.GetType().Name] = prop.Name;
                                            descProp = prop.Name;
                                            break;
                                        }
                                    }
                                }
                                if (string.IsNullOrEmpty(descProp))
                                {
                                    break;
                                }
                                var index2 = item.GetType().GetProperty("Index").GetValue(item, null).ToString();
                                if (index != index2)
                                {
                                    continue;
                                }
                                desc = item.GetType().GetProperty(descProp).GetValue(item, null).ToString();
                                Keys[key] = desc;
                                index += "|" + desc;
                                break;
                            }
                        }


                        rows.CreateCell(j - temp).SetCellValue(index);

                    }
                    else
                    {
                        rows.CreateCell(j - temp).SetCellValue(v.ToString());
                    }
                }

                if (i >= 30000 && i % 30000 == 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        workbook.Write(ms);
                        using (FileStream fs = new FileStream(strFullPath, FileMode.Create, FileAccess.Write))
                        {
                            byte[] data = ms.ToArray();
                            fs.Write(data, 0, data.Length);
                            fs.Flush();
                        }

                    }
                    workbook.Close();
                    GC.Collect();

                    if (i == entitys.Count - 1)
                    {
                        return;
                    }
                    strFullPath = Path.Combine(strPath, fileName + fileIndex + ".xlsx");
                    workbook = new XSSFWorkbook();
                    sheet = workbook.CreateSheet("Sheet");
                    rowindex = 0;
                    fileIndex++;
                }
                rowindex++;
            }

            using (MemoryStream ms = new MemoryStream())  //内存流
            {
                workbook.Write(ms);
                using (FileStream fs = new FileStream(strFullPath, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }

        /// <summary>
        /// 导入excel
        /// </summary>
        /// 
        /// <param name="inSub">是否含关联表</param>
        public static void ImportExcel(bool inSub = false)
        {
            var type = typeof(T);
            IList<Type> types = new List<Type>();
            GetImportTypes(type, types);
            string msg = string.Empty;
            if (inSub)
            {
                string strPath = Path.Combine(Config.ExportPath, type.Name);
                if (!Directory.Exists(strPath))
                    return;

                msg += ImportExcel(Path.Combine(strPath, typeof(T).Name + ".xlsx")) + "\r\n";//导入基础表
                foreach (var item in types)//导入关联表
                {
                    var exportType = typeof(HelperExcel<>).MakeGenericType(item);
                    var result = exportType.GetMethod("ImportExcel", new Type[] { typeof(string) }).Invoke(null, new object[] { Path.Combine(strPath, item.Name + ".xlsx") });
                    msg += Convert.ToString(result) + "\r\n";
                }
                foreach (var item in types)
                {
                    var exportType = typeof(HelperExcel<>).MakeGenericType(item);
                    exportType.GetMethod("BindlingList").Invoke(null, null);
                }
                BindlingList();

                MessageBox.Show(msg, "提示");
            }
            else
            {

                msg = ImportExcel(Path.Combine(Config.ExportPath, typeof(T).Name + ".xlsx"));
                MessageBox.Show(msg, "提示");
            }
        }

        private static void GetImportTypes(Type type, IList<Type> result)
        {
            var infos = type.GetProperties();
            var bindings = infos.Where(p => p.PropertyType.Name.IndexOf("DBBindingList") != -1);
            var types = bindings.Select(p => p.PropertyType.GetGenericArguments()[0]);
            foreach (var item in types)
            {
                result.Add(item);
                GetImportTypes(item, result);
            }
        }


        /// <summary>
        /// 添加从表值BindingList
        /// </summary>
        /// <param name="bindings"></param>
        public static void BindlingList()
        {
            var infos = typeof(T).GetProperties();
            var bindings = infos.Where(p => p.PropertyType.Name.IndexOf("DBBindingList") != -1);

            IList<T> entitys = SMain.Session.GetCollection<T>().Binding;

            var session = SMain.Session.GetType();
            var cllectionMethod = session.GetMethod("GetCollection", new Type[] { });
            foreach (var item in entitys)
            {
                var index = item.Index;
                foreach (var binding in bindings)
                {
                    //DBObject parent, PropertyInfo property
                    var list = Activator.CreateInstance(binding.PropertyType, item, binding);
                    var collection = cllectionMethod.MakeGenericMethod(binding.PropertyType.GetGenericArguments()[0]).Invoke(SMain.Session, null);
                    var fromList = collection.GetType().GetField("Binding").GetValue(collection) as IEnumerable<DBObject>;
                    foreach (var from in fromList)
                    {
                        var modelType = from.GetType().GetProperties().First(p => p.PropertyType == item.GetType());
                        var model = modelType.GetValue(from);
                        if (modelType == null || model == null || modelType.PropertyType.GetProperty("Index").GetValue(model).ToString() != index.ToString())
                        {
                            continue;
                        }
                        list.GetType().GetMethod("Add").Invoke(list, new object[] { from });//此处

                    }
                    binding.SetValue(item, list);
                }
            }
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="file">导入文件</param>
        /// <returns>List<T></returns>
        public static string ImportExcel(string path)
        {
            if (!File.Exists(path))
            {
                return $"文件{path},不存在!";
            }
            int delCount = 0;
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            SMain.Session.GetCollection<T>().Binding.Clear();
            List<T> list = new List<T> { };

            stream.Seek(0, SeekOrigin.Begin);

            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet sheet = workbook.GetSheetAt(0);
            IRow cellNum = sheet.GetRow(0);
            var propertys = typeof(T).GetProperties();
            string value = null;
            int num = cellNum.LastCellNum;
            List<string> titles = new List<string>();
            for (int i = 0; i < num; i++)
            {
                titles.Add(sheet.GetRow(0).GetCell(i).ToString());
            }

            int offSet = 1;
            int maxIndex = -1;
            for (int i = offSet; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                var index = Convert.ToInt32(row.GetCell(titles.IndexOf("Index")).ToString());
                if (index > maxIndex)
                {
                    maxIndex = index;
                }
                var tObj = SMain.Session.GetCollection<T>().Binding.FirstOrDefault(p => p.Index == index);
                T obj = tObj ?? SMain.Session.GetCollection<T>().CreateNewObject();
                for (int j = 0; j < propertys.Length; j++)
                {
                    if (titles.IndexOf(propertys[j].Name) == -1)
                    {
                        continue;
                    }
                    value = row.GetCell(titles.IndexOf(propertys[j].Name))?.ToString() ?? "";

                    string str = (propertys[j].PropertyType).FullName;
                    if (string.IsNullOrEmpty(value))
                    {
                        propertys[j].SetValue(obj, null, null);
                    }
                    else if (propertys[j].PropertyType == typeof(string))
                    {
                        propertys[j].SetValue(obj, value, null);
                    }
                    else if (propertys[j].PropertyType == typeof(DateTime))
                    {
                        DateTime pdt = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                        propertys[j].SetValue(obj, pdt, null);
                    }
                    else if (str.IndexOf("System.DateTime") != -1)
                    {
                        if (!DateTime.TryParse(value, out DateTime date))
                        {
                            propertys[j].SetValue(obj, null, null);
                        }
                        else
                        {
                            DateTime pdt = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                            propertys[j].SetValue(obj, pdt, null);
                        }
                    }
                    else if (propertys[j].PropertyType == typeof(bool))
                    {
                        bool pb = Convert.ToBoolean(value);
                        propertys[j].SetValue(obj, pb, null);
                    }
                    else if (str == "System.Int16")
                    {
                        short pi16 = Convert.ToInt16(value);
                        propertys[j].SetValue(obj, pi16, null);
                    }
                    else if (str == "System.Int32")
                    {
                        int pi32 = Convert.ToInt32(value);
                        propertys[j].SetValue(obj, pi32, null);
                    }
                    else if (str == "System.Int64")
                    {
                        long pi64 = Convert.ToInt64(value);
                        propertys[j].SetValue(obj, pi64, null);
                    }
                    else if (str == "System.Byte")
                    {
                        byte pb = Convert.ToByte(value);
                        propertys[j].SetValue(obj, pb, null);
                    }
                    else if (str == "System.Byte")
                    {
                        byte pb = Convert.ToByte(value);
                        propertys[j].SetValue(obj, pb, null);
                    }
                    else if (propertys[j].PropertyType == typeof(decimal))
                    {
                        decimal.TryParse(value, out decimal v);
                        propertys[j].SetValue(obj, v, null);
                    }
                    else if (propertys[j].PropertyType == typeof(float))
                    {
                        float.TryParse(value, out float v);
                        propertys[j].SetValue(obj, v, null);
                    }
                    else if (propertys[j].PropertyType.IsValueType &&
                          propertys[j].PropertyType.IsGenericType &&
                          propertys[j].PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) &&
                          propertys[j].PropertyType.GetGenericArguments()[0].IsEnum
                          )
                    {
                        var enumValue = Enum.Parse(propertys[j].PropertyType.GetGenericArguments()[0], value);
                        propertys[j].SetValue(obj, enumValue, null);
                    }
                    else if (propertys[j].PropertyType.IsEnum)
                    {
                        var enumValue = Enum.Parse(propertys[j].PropertyType, value);
                        propertys[j].SetValue(obj, enumValue, null);
                    }
                    else if (propertys[j].PropertyType == typeof(BitArray))
                    {
                        var v = JsonConvert.DeserializeObject<bool[]>(GetCellValue(value));
                        propertys[j].SetValue(obj, new BitArray(v), null);
                    }
                    else if (propertys[j].PropertyType.IsArray || propertys[j].PropertyType.IsGenericType)
                    {
                        Type locType = typeof(JsonConvert);//获取类
                        MethodInfo methoed = locType.GetMethod("DeserializeObject", new Type[] { typeof(string), typeof(Type) });//获取方法
                        var arry = methoed.Invoke(locType, new object[] { GetCellValue(value), propertys[j].PropertyType });
                        propertys[j].SetValue(obj, arry, null);
                    }
                    else if (propertys[j].PropertyType.Namespace.StartsWith("Library.SystemModels"))
                    {
                        var reg = new Regex(@"^[0-9]{1,}\|");
                        if (reg.IsMatch(value))
                        {
                            value = reg.Match(value).Value.Replace("|", "");
                        }
                        var v = SMain.Session.GetObject(propertys[j].PropertyType, "Index", Convert.ToInt32(value));
                        propertys[j].SetValue(obj, v, null);
                    }
                    //else
                    //{
                    //    action?.Invoke(propertys[j], obj, GetCellValue(value));
                    //}
                }

                list.Add(obj);
            }

            var delList = SMain.Session.GetCollection<T>().Binding.Where(p => p.Index > maxIndex).ToList();
            if (delList.Count > 0)
            {
                foreach (var item in delList)
                {
                    SMain.Session.GetCollection<T>().Binding.Remove(item);

                }
            }
            delCount = delList.Count;

            stream.Close();
            stream.Dispose();

            return $"{typeof(T).Name}导入成功:{list.Count}条,删除{delCount}条!";
        }
        /// <summary>
        /// 获取单元格值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string GetCellValue(string str)
        {
            var result = str;
            if (str.IndexOf(".txt") != -1)//TODO 正则名字+.txt
            {
                using (StreamReader reader = new StreamReader(str))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }
    }
}
