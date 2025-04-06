using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using Library.MirDB;
#if SERVER || ServerTool
using MySqlConnector;
#endif
#if SERVER
using Server.DBModels;
#endif
using System.Collections;

namespace MirDB
{
    /// <summary>
    /// 数据库集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class DBCollection<T> : ADBCollection, IEnumerable<T>, IEnumerable where T : DBObject, new()
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 绑定序号
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index] => Binding[index];
        /// <summary>
        /// 绑定计数
        /// </summary>
        public override int Count => Binding.Count;
        /// <summary>
        /// 绑定
        /// </summary>
        public IList<T> Binding;
        //private SortedDictionary<int, T> Dictionary = new SortedDictionary<int, T>(); //For Obtaining Keys.
        /// <summary>
        /// 版本是否有效  说明：代码里面的表是否匹配源数据的表，包括表和字段的匹配
        /// </summary>
        private bool VersionValid;
        /// <summary>
        /// 保存列表
        /// </summary>
        private List<T> SaveList;
        /// <summary>
        /// 客户端保存列表
        /// </summary>
        private List<T> ClientSaveList;
#if SERVER || ServerTool
        #region MySql
        /// <summary>
        /// 删除列表
        /// </summary>
        private List<int> DeleteList = new List<int>();
        private Hashtable SQLParam;
        private string DBName => IsSystemData ? "SystemDB" : "UserDB";
        private string connectionString;
        #endregion
#endif

        public DBCollection(Session session)
        {
            Type = typeof(T);
            Mapping = new DBMapping(session.Assemblies, Type);

            IsSystemData = Type.GetCustomAttribute<UserObject>() == null;
            IsClientSystemData = Type.GetCustomAttribute<ServerOnly>() == null;
            ClientMapping = new DBMapping(session.Assemblies, Type, IsSystemData);

            RaisePropertyChanges = IsSystemData;
            Session = session;

            if (IsSystemData)
                ReadOnly = Session.Mode != SessionMode.ServerTool;
            else
                ReadOnly = Session.Mode == SessionMode.ServerTool;

            if (IsSystemData)
            {
                BindingList<T> binding = new BindingList<T>
                {
                    RaiseListChangedEvents = RaisePropertyChanges
                };
                binding.AddingNew += (o, e) => e.NewObject = CreateNew();
                Binding = binding;
            }
            else
                Binding = new List<T>();

#if SERVER || ServerTool
            if (!session.IsMySql) return;
            connectionString = string.Format("Database={0};Server={1};Port={2};Uid={3};Pwd={4};AllowLoadLocalInfile=true;",
                DBName, session.MySqlParms[2], session.MySqlParms[3], session.MySqlParms[0], session.MySqlParms[1]);
#endif
        }
        /// <summary>
        /// 新建
        /// </summary>
        /// <returns></returns>
        private T CreateNew()
        {
            T ob = new T
            {
                Index = ++Index,
                Collection = this,
            };
            ob.OnCreated();

            return ob;
        }
        /// <summary>
        /// 创建新对象
        /// </summary>
        /// <returns></returns>
        public T CreateNewObject()
        {
            T ob = CreateNew();

            Binding.Add(ob);

            return ob;
        }
        /// <summary>
        /// 删除好友
        /// </summary>
        /// <param name="ob"></param>
        // 这个实现方法不太好
        public void DeleteFriends(DBObject ob)
        {
            Delete(ob);
        }
        /// <summary>
        /// 内部重写数据库对象按索引获取对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal override DBObject GetObjectByIndex(int index)
        {
            index = FastFind(index);

            if (index >= 0) return Binding[index];

            return null;
        }

        public T GetTObjectByIndex(int index)
        {
            index = FastFind(index);

            if (index >= 0) return Binding[index];

            return null;
        }

        /// <summary>
        /// 内部重写数据库对象 按字段名获取对象
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected internal override DBObject GetObjectbyFieldName(string fieldName, object value)
        {
            PropertyInfo info = Type.GetProperty(fieldName);

            if (info == null) return null;

            foreach (T ob in Binding)
            {
                if (info.GetValue(ob).Equals(value))
                    return ob;
            }

            return null;
        }

        const decimal KB = 1024;
        const decimal MB = KB * 1024;
        const decimal GB = MB * 1024;
        /// <summary>
        /// 内部覆盖无效的加载
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mapping"></param>
        internal override void Load(byte[] data, DBMapping mapping)
        {
            //代码里面的表或字段是否匹配源数据
            VersionValid = mapping.IsMatch(Mapping);

            using (MemoryStream mStream = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(mStream))
            {
                Index = reader.ReadInt32();

                int count = reader.ReadInt32();

                //#if SERVER
                //                if (!IsSystemData)
                //                {
                //                    string size = "0";
                //                    if (data.Length > GB)
                //                        size = string.Format(@"{0:#,##0.0}GB", data.Length / GB);
                //                    else if (data.Length > MB)
                //                        size = string.Format(@"{0:#,##0.0}MB", data.Length / MB);
                //                    else if (data.Length > KB)
                //                        size = string.Format(@"{0:#,##0}KB", data.Length / KB);
                //                    else
                //                        size = string.Format(@"{0:#,##0}B", data.Length);
                //                    Session.Output?.Invoke(null, $"正在加载表:{mapping.Type.Name} 大小:{size} 行数:{count}");
                //                }
                //#endif

                for (int i = 0; i < count; i++)
                {
                    T ob = new T { Collection = this };
                    Binding.Add(ob);

                    ob.RawData = reader.ReadBytes(reader.ReadInt32());
                    ob.Load(mapping);
                    //只取最大Index
                    if (ob.Index > Index)
                        Index = ob.Index;
                }
            }
        }

        /// <summary>
        /// 内部覆盖无效保存对象
        /// </summary>
        internal override void SaveObjects()
        {
            if (ReadOnly || SaveList != null) return;

            SaveList = new List<T>(Binding.Count);
            foreach (T ob in Binding)
            {
#if SERVER
                if (ob.GetType() == typeof(UserItem))
                {
                    var item = (ob as UserItem);
                    if (item.IsDeleted || item.CanBuyback && item.IsOwnerless) continue;
                }
#endif
                if (ob.IsTemporary) continue;
                //有变动的表或表的此行数据有修改，此行重新序列化
                if (!VersionValid || ob.IsModified)
                    ob.Save();
                //只取最大Index
                if (ob.Index > Index)
                    Index = ob.Index;

                ob.ClientRawData = null;
                SaveList.Add(ob);
            }
            //只有servertool工具才能保存客户端数据库
            if (Session.Mode == SessionMode.ServerTool && IsClientSystemData && IsSystemData)
            {
                ClientSaveList = new List<T>(Binding.Count);
                foreach (T ob in Binding)
                {
                    if (!ob.IsTemporary && !ob.ServerOnly)
                    {
                        if (!VersionValid || ob.IsModified || ob.ClientRawData == null)
                            ob.ClientSave();
                        ClientSaveList.Add(ob);
                    }
                }
            }

            VersionValid = true;
            return;
        }


        /// <summary>
        /// 从一个已有对象黏贴到新对象
        /// </summary>
        /// <param name="oldOb"></param>
        /// <param name="newOb"></param>
        /// <param name="subClass"></param>
        /// <returns></returns>
        internal override void PasteObject(DBObject oldOb, DBObject newOb, Dictionary<DBObject, DBObject> subClass)
        {
            subClass.Add(oldOb, newOb);

            //循环数据属性赋值到新数据
            foreach (DBValue dbValue in newOb.Collection.Mapping.Properties)
            {
                if (dbValue.PropertyName == "Index") continue;

                object value = dbValue.Property.GetValue(oldOb);
                if (value == null) continue;
                //如果字段关联了子类
                if (dbValue.Property.PropertyType.IsSubclassOf(typeof(DBObject)))
                {
                    if (dbValue.Property.GetValue(newOb) != null) continue;
                    //跟老数据关联的子类都要跟复制的新数据关联
                    foreach (KeyValuePair<DBObject, DBObject> pair in subClass)
                    {
                        if (pair.Key.ThisType == dbValue.Property.PropertyType)
                        {
                            if (pair.Key.Index == ((DBObject)value).Index)
                                value = pair.Value;
                        }
                    }
                }
                dbValue.Property.SetValue(newOb, value);
            }

            //关联的BindingList赋值
            PropertyInfo[] properties = newOb.ThisType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            foreach (PropertyInfo property in properties)
            {
                Association link = property.GetCustomAttribute<Association>();

                if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(DBBindingList<>)) continue;

                IBindingList oldList = (IBindingList)property.GetValue(oldOb);
                IBindingList newList = (IBindingList)property.GetValue(newOb);

                if (link != null)
                {
                    for (int i = oldList.Count - 1; i >= 0; i--)
                    {
                        DBObject listOldOb = (DBObject)oldList[i];
                        PasteObject(listOldOb, (DBObject)newList.AddNew(), subClass);
                    }
                }
            }
        }

        /// <summary>
        /// 从一个对象二进制流黏贴到新对象
        /// </summary>
        /// <param name="br"></param>
        /// <param name="sourceMapping"></param>
        /// <param name="newOb"></param>
        /// <param name="subClass"></param>
        /// <returns></returns>
        internal override void PasteObject(BinaryReader br, DBMapping sourceMapping, DBObject newOb, Dictionary<DBObject, int> subClass)
        {
            newOb.RawData = br.ReadBytes(br.ReadInt32());
            newOb.PasteObject(sourceMapping, subClass);

            //循环源数据属性赋值到新数据
            foreach (DBValue dbValue in newOb.Collection.Mapping.Properties)
            {
                //如果字段关联了子类
                if (dbValue.Property.PropertyType.IsSubclassOf(typeof(DBObject)))
                {
                    //跟老数据关联的子类都要跟复制的新数据关联
                    foreach (KeyValuePair<DBObject, int> pair in subClass)
                    {
                        if (pair.Key.ThisType == dbValue.Property.PropertyType)
                        {
                            object value = dbValue.Property.GetValue(newOb);
                            if (value == null) continue;
                            if (pair.Value == ((DBObject)value).Index)
                                dbValue.Property.SetValue(newOb, pair.Key);
                        }
                    }
                }
            }

            //关联的BindingList赋值
            PropertyInfo[] properties = newOb.ThisType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            int bindlistcount = br.ReadInt32();
            for (int x = 0; x < bindlistcount; x++)
            {
                DBMapping bindmapping = new DBMapping(Session.Assemblies, br);
                ADBCollection bindvalue = Session.GetCollection(bindmapping.Type);
                foreach (PropertyInfo property in properties)
                {
                    Association link = property.GetCustomAttribute<Association>();
                    if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(DBBindingList<>)) continue;
                    if (property.PropertyType.GenericTypeArguments[0].Name != bindmapping.Type.Name) continue;

                    if (link != null)
                    {
                        IBindingList list = (IBindingList)property.GetValue(newOb);
                        int bindcount = br.ReadInt32();
                        for (int y = 0; y < bindcount; y++)
                        {
                            bindvalue.PasteObject(br, bindmapping, (DBObject)list.AddNew(), subClass);
                        }
                    }
                }
            }
        }
#if SERVER || ServerTool
        #region MYSQL
        /// <summary>
        /// MySql加载对象
        /// </summary>
        internal override void LoadSQL()
        {
            DataTable dt = QueryTable(Mapping.Type.Name);
            //如果代码里面的表 数据库不存在  即为新表
            if (dt == null)
            {
                VersionValid = false;
                return;
            }
            else
            {
                VersionValid = true;
                //如果代码里面表的字段与数据库不一致，即有变动的表 VersionValid=false
                if (dt.Columns.Count != Mapping.Properties.Count)
                    VersionValid = false;
                foreach (DBValue dbValue in Mapping.Properties)
                {
                    if (!dt.Columns.Contains(dbValue.PropertyName))
                    {
                        VersionValid = false;
                        dbValue.IsNew = true;
                    }
                }
            }
            if (!VersionValid)
                Session.Output?.Invoke(null, $"警告：{DBName}.{Type.Name} 表不存在或字段有变动，请使用服务端工具重新执行初始化。");

            Index = GetMaxID("Index", Mapping.Type.Name);
            int count = dt.Rows.Count;

            for (int i = 0; i < count; i++)
            {
                T ob = new T { Collection = this };
                Binding.Add(ob);

                ob.LoadSQL(i, dt);
            }
            dt.Clear();
        }

        /// <summary>
        /// MySql初始化所有对象
        /// </summary>
        internal override void InitObjectToSQL()
        {
            //判断表是否存在 不存在直接建表，建完表后将此表的数据全部导入
            bool exist = TabExists(Type.Name);
            if (exist)
            {
                //如果代码里的字段有改动，即不匹配源数据的表，就重新初始化
                if (!VersionValid)
                {
                    List<string> sqlStrings = new List<string>() { $"DROP TABLE IF EXISTS {DBName}.{Type.Name}", };
                    if (ExecuteSqlTran(sqlStrings))
                        InitObjectToSQL();
                }
                return;
            }

            //拼接建表语句
            StringBuilder createTableSqlText = new StringBuilder();
            foreach (DBValue dbValue in Mapping.Properties)
            {
                if (dbValue.Property == null) continue;
                if (dbValue.PropertyName == "Index") continue;

                string colname = $"`{dbValue.PropertyName}`";
                //自定义类字段为int类型
                if (dbValue.Property.PropertyType.IsSubclassOf(typeof(DBObject)))
                    createTableSqlText.AppendLine(colname + " INT(10) ,");
                else
                    createTableSqlText.AppendLine(colname + dbValue.ReadTypeToColumnSQL() + ",");
            }
            createTableSqlText.Remove(createTableSqlText.ToString().LastIndexOf(','), 1);

            //数据库创建表
            if (ExecuteSql($"CREATE TABLE IF NOT EXISTS {DBName}.{Type.Name} (\r\n" +
                               $"`Index` INT(10) PRIMARY KEY ,\r\n" +
                               createTableSqlText.ToString() + ");"))
                Session.Output?.Invoke(null, $"{DBName}.{Type.Name} 表创建完成。");
            else
            {
                Session.Output?.Invoke(null, $"错误：{DBName}.{Type.Name} 表创建错误！！！！");
                return;
            }

            //新建表批量数据插入
            //获取表结构
            DataTable dt = QueryTable(Type.Name);
            foreach (T ob in Binding)
            {
                if (ob.IsTemporary) continue;

                ob.SaveDataTable(dt);
            }
            //整张表数据一次性插入数据库
            if (dt.Rows.Count > 0)
            {
                if (SqlBulkCopyByDatatable(Type.Name, dt))
                {
                    Session.Output?.Invoke(null, $"{DBName}.{Type.Name} 表已插入{dt.Rows.Count}条数据。");
                    dt.Clear();
                }
                else
                {
                    Session.Output?.Invoke(null, $"错误：{DBName}.{Type.Name} 表插入数据错误！！！！");
                    dt.Clear();
                    return;
                }
            }

            //这个标记为false是为了初始化外键，代表新表没有创建外键的。
            VersionValid = false;
        }
        /// <summary>
        /// MySql初始化所有外键
        /// </summary>
        internal override void InitForeignKey()
        {
            //有外键的返回
            if (VersionValid) return;

            bool exist = TabExists(Type.Name);
            if (!exist) return;

            StringBuilder sqlText = new StringBuilder();
            bool hasSubclass = false;
            foreach (DBValue dbValue in Mapping.Properties)
            {
                //关联了自定义类的字段建立外键约束
                if (!dbValue.Property.PropertyType.IsSubclassOf(typeof(DBObject))) continue;
                string dbName;
                if (dbValue.Property.PropertyType.GetCustomAttribute<UserObject>() == null)
                    dbName = "SystemDB";
                else
                    dbName = "UserDB";
                hasSubclass = true;
                sqlText.AppendLine($"ADD FOREIGN KEY(`{dbValue.PropertyName}`) REFERENCES `{dbName}`.`{dbValue.Property.PropertyType.Name}`(`Index`) ON DELETE SET NULL ON UPDATE CASCADE,");
            }

            if (!hasSubclass)
            {
                VersionValid = true;
                return;
            }
            sqlText.Remove(sqlText.ToString().LastIndexOf(','), 1);

            if (ExecuteSql($"ALTER TABLE {DBName}.{Type.Name} {sqlText};"))
                Session.Output?.Invoke(null, $"{DBName}.{Type.Name} 表外键已创建完成。");
            else
                return;

            VersionValid = true;
        }
        /// <summary>
        /// MySql保存新建及修改的对象
        /// </summary>
        internal override void SaveSQLParam()
        {
            if (ReadOnly || (SQLParam != null && SQLParam.Count > 0)) return;

            if (!VersionValid)
            {
                //InitObjectToSQL();
                Session.Output?.Invoke(null, $"错误：{DBName}.{Type.Name} 表 程序与数据库结构不同步，请关闭引擎，用工具重新同步下数据库，以免数据丢失。");
                //return;
            }

            SQLParam = new Hashtable();

            //拼接insert update语句
            foreach (T ob in Binding)
            {
                if (ob.IsTemporary) continue;

                if (ob.IsNewCreated || ob.IsModified)
                {
                    //拼接insert和update语句及MySqlParameter
                    MySqlParameter[] sqlParameter = new MySqlParameter[Mapping.Properties.Count - 1];
                    StringBuilder columnName = new StringBuilder();
                    StringBuilder valueName = new StringBuilder();
                    StringBuilder updateSql = new StringBuilder();

                    for (int i = 0; i < Mapping.Properties.Count; i++)
                    {
                        if (Mapping.Properties[i].PropertyName == "Index") continue;
                        if (Mapping.Properties[i].IsNew) continue;

                        object value;
                        if (Mapping.Properties[i].Property.PropertyType.IsSubclassOf(typeof(DBObject)))
                        {
                            DBObject linkOb = (DBObject)Mapping.Properties[i].Property.GetValue(ob);
                            value = Mapping.Properties[i].WriteValueSQL(linkOb?.Index ?? 0);
                            //没有关联类的字段，数据库统一保存为null，防止外键约束报错。
                            if ((int)value == 0)
                                value = DBNull.Value;
                        }
                        else
                            value = Mapping.Properties[i].WriteValueSQL(Mapping.Properties[i].Property.GetValue(ob));

                        sqlParameter[i] = new MySqlParameter($"@{Mapping.Properties[i].PropertyName}", value);

                        string col = $"`{Mapping.Properties[i].PropertyName}`";
                        string val = $"@{Mapping.Properties[i].PropertyName}";

                        columnName.Append(col + ",");
                        valueName.Append(val + ",");
                        updateSql.Append(col + "=" + val + ",");
                    }
                    columnName.Remove(columnName.ToString().LastIndexOf(','), 1);
                    valueName.Remove(valueName.ToString().LastIndexOf(','), 1);
                    updateSql.Remove(updateSql.ToString().LastIndexOf(','), 1);

                    //insert一行数据
                    if (ob.IsNewCreated)
                        SQLParam.Add($"INSERT INTO {DBName}.{Type.Name} (`Index`,{columnName}) VALUES({ob.Index},{valueName});", sqlParameter);
                    //update一行数据
                    else if (ob.IsModified)
                        SQLParam.Add($"UPDATE {DBName}.{Type.Name} SET {updateSql} WHERE `Index`={ob.Index};", sqlParameter);

                    ob.OnSaved();
                }
                //只取最大Index
                if (ob.Index > Index)
                    Index = ob.Index;
            }

            //执行delete 因为DeleteList在引擎是实时变动，所以放到引擎主线程去执行
            if (DeleteList.Count > 0)
            {
                StringBuilder deleteSql = new StringBuilder();
                foreach (int i in DeleteList)
                {
                    deleteSql.Append($"'{i}',");
                }
                deleteSql.Remove(deleteSql.ToString().LastIndexOf(','), 1);

                SQLParam.Add($"DELETE FROM {DBName}.{Type.Name} WHERE `Index` IN ({deleteSql});", null);
                DeleteList.Clear();
            }

            //VersionValid = true;
        }
        /// <summary>
        /// MySql删除对象
        /// </summary>
        internal override void SaveObjectsSQL()
        {
            if (SQLParam == null) return;
            try
            {
                //insert update delete执行
                if (SQLParam.Count > 0)
                {
                    if (ExecuteSqlTran(SQLParam))
                        SQLParam = null;
                }
                else
                    SQLParam = null;
            }
            catch (Exception ex)
            {
                Session.Output?.Invoke(null, ex.ToString());
            }
        }
        /// <summary>
        /// MySql数据转换到Z版二进制文件
        /// </summary>
        internal override void SqlToFile()
        {
            if (ReadOnly || SaveList != null) return;

            SaveList = new List<T>(Binding.Count);

            foreach (T ob in Binding)
            {
                if (ob.IsTemporary) continue;

                using (MemoryStream mem = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(mem))
                    {
                        foreach (DBValue dbValue in Mapping.Properties)
                        {
                            if (dbValue.Property.PropertyType.IsSubclassOf(typeof(DBObject)))
                            {
                                DBObject linkOb = (DBObject)dbValue.Property.GetValue(ob);
                                dbValue.WriteValue(linkOb?.Index ?? 0, bw);
                            }
                            else
                                dbValue.WriteValue(dbValue.Property.GetValue(ob), bw);
                        }
                        ob.RawData = mem.ToArray();
                    }
                }
                ob.ClientRawData = null;
                SaveList.Add(ob);
            }

            //只有servertool工具才能保存客户端数据库
            if (Session.Mode == SessionMode.ServerTool && IsClientSystemData && IsSystemData)
            {
                ClientSaveList = new List<T>(Binding.Count);
                foreach (T ob in Binding)
                {
                    if (!ob.IsTemporary && !ob.ServerOnly)
                    {
                        using (MemoryStream mem = new MemoryStream())
                        {
                            using (BinaryWriter bw = new BinaryWriter(mem))
                            {
                                foreach (DBValue dbValue in Mapping.Properties)
                                {
                                    if (dbValue.Property.PropertyType.IsSubclassOf(typeof(DBObject)))
                                    {
                                        DBObject linkOb = (DBObject)dbValue.Property.GetValue(ob);
                                        dbValue.WriteValue(linkOb?.Index ?? 0, bw);
                                    }
                                    else
                                        dbValue.WriteValue(dbValue.Property.GetValue(ob), bw);
                                }
                                ob.ClientRawData = mem.ToArray();
                            }
                        }
                        ClientSaveList.Add(ob);
                    }
                }
            }
        }
        #endregion
#endif
        /// <summary>
        /// 内部覆盖字节[]获取保存数据
        /// </summary>
        /// <returns></returns>
        internal override byte[] GetSaveData()
        {
            using (MemoryStream mStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(mStream))
            {
                writer.Write(Index);
                writer.Write(SaveList.Count);

                foreach (T ob in SaveList)
                {
                    writer.Write(ob.RawData.Length);
                    writer.Write(ob.RawData);
                }

                mStream.Seek(4, SeekOrigin.Begin);

                SaveList = null;
                return mStream.ToArray();
            }
        }
        /// <summary>
        /// 内部覆盖字节[]获取客户端保存数据
        /// </summary>
        /// <returns></returns>
        internal override byte[] GetClientSaveData()
        {
            using (MemoryStream mStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(mStream))
            {
                writer.Write(Index);
                writer.Write(ClientSaveList.Count);

                foreach (T ob in ClientSaveList)
                {
                    writer.Write(ob.ClientRawData.Length);
                    writer.Write(ob.ClientRawData);
                }

                mStream.Seek(4, SeekOrigin.Begin);

                ClientSaveList = null;
                return mStream.ToArray();
            }
        }
        /// <summary>
        /// 内部替代作废删除（数据库对象）
        /// </summary>
        /// <param name="ob"></param>
        internal override void Delete(DBObject ob)
        {
            /* for (int i = Binding.Count - 1; i >= 0; i--)
             {
                 if (Binding[i] != ob) continue;

                 Binding.RemoveAt(i);
                 break;
             }*/


            int index = FastFind(ob.Index);

            if (index >= 0)
            {
                Binding.RemoveAt(index);
#if SERVER || ServerTool
                DeleteList.Add(ob.Index);
#endif
            }
        }
        /// <summary>
        /// 私有智能快速查找INT索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int FastFind(int index)
        {
            int pos = 0;

            if (Binding.Count == 0) return -1;

            int shift = Binding.Count / 2;

            bool? dir = null;

            while (true)
            {
                shift = Math.Max(1, shift);

                int cur = Binding[pos].Index;
                if (cur == index) return pos;

                if (cur > index)
                {
                    pos -= shift;
                    shift /= 2;

                    if (pos <= -1) break;

                    if (shift == 0)
                    {
                        if (dir.HasValue && dir.Value) break;

                        dir = false;
                    }
                }
                else
                {
                    //Increase pos

                    pos += shift;
                    shift /= 2;

                    if (pos >= Binding.Count) break;

                    if (shift == 0)
                    {
                        if (dir.HasValue && !dir.Value) break;

                        dir = true;
                    }
                }

            }

            return -1;
        }
        /// <summary>
        /// 内部覆盖数据库对象  创建对象
        /// </summary>
        /// <returns></returns>
        internal override DBObject CreateObject()
        {
            return CreateNewObject();
        }
        /// <summary>
        /// 内部覆盖无效时加载
        /// </summary>
        internal override void OnLoaded()
        {
            foreach (T ob in Binding)
                ob.OnLoaded();
        }
#if SERVER || ServerTool
        #region MySql
        /// <summary>
        /// 查询表是否存在
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns>存在:True 不存在:False</returns>
        public bool TabExists(string TableName)
        {
            string strsql = "show tables like " + "'" + TableName + "'";
            object obj = GetSingle(strsql);
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 查询表指定字段的最大值 需要该字段为数字类型
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <param name="TableName">表名</param>
        /// <returns>Int</returns>
        public int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(`" + FieldName + "`) from " + TableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        /// <summary>
        /// 执行表查询，返回DataTable
        /// </summary>
        /// <param name="TableName">需要查询的表名</param>
        /// <returns>表存在返回DataTable，表不存在返回null</returns>
        public DataTable QueryTable(string TableName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataTable dt = new DataTable();
                try
                {
                    connection.Open();

                    string SQLString = "select * from " + TableName;
                    MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                    command.Fill(dt);
                    dt.TableName = TableName;
                }
                catch (MySqlException ex)
                {
                    connection.Close();
                    Session.Output?.Invoke(null, $"错误：{ex.Message} [{TableName}]");
                    return null;
                }
                return dt;
            }
        }
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>执行成功:True 执行失败:False</returns>
        public bool ExecuteSql(string SQLString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (MySqlException e)
                    {
                        connection.Close();
                        Session.Output?.Invoke(null, $"错误：{e.Message}\r\n[{SQLString}]");
                        return false;
                    }
                }
            }
        }
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>执行成功:True 执行失败:False</returns>
        public bool ExecuteSql(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = connection.CreateCommand())
                {
                    try
                    {
                        connection.Open();
                        PrepareCommand(cmd, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return true;
                    }
                    catch (MySqlException e)
                    {
                        connection.Close();
                        Session.Output?.Invoke(null, $"错误：{e.Message}\r\n[{SQLString}]");
                        return false;
                    }
                }
            }
        }
        /// <summary>
        /// DataTable批量插入表
        /// </summary>
        /// <param name="TableName">目标表</param>
        /// <param name="dt">源数据</param>
        /// <returns>执行成功:True 执行失败:False</returns>
        public bool SqlBulkCopyByDatatable(string TableName, DataTable dt)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlBulkCopy sqlbulkcopy = new MySqlBulkCopy(connection);
                    sqlbulkcopy.DestinationTableName = TableName;
                    sqlbulkcopy.WriteToServer(dt);
                    return true;
                }
                catch (MySqlException ex)
                {
                    connection.Close();
                    Session.Output?.Invoke(null, $"错误：{ex.Message} [{TableName}]");
                    return false;
                }
            }
        }
        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>	
        /// <returns>Commit返回True，Rollback返回False</returns>
        private bool ExecuteSqlTran(List<String> SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction tx = conn.BeginTransaction())
                {
                    using (MySqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        try
                        {
                            cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 0;";
                            cmd.ExecuteNonQuery();
                            for (int n = 0; n < SQLStringList.Count; n++)
                            {
                                string strsql = SQLStringList[n];
                                if (strsql.Trim().Length > 1)
                                {
                                    cmd.CommandText = strsql;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 1;";
                            cmd.ExecuteNonQuery();
                            tx.Commit();
                            return true;
                        }
                        catch (MySqlException e)
                        {
                            tx.Rollback();
                            conn.Close();
                            Session.Output?.Invoke(null, $"错误：{e.Message}\r\n[{cmd.CommandText}]");
                            return false;
                        }
                    }

                }

            }
        }
        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        /// <returns>Commit返回True，Rollback返回False</returns>
        public bool ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    using (MySqlCommand cmd = conn.CreateCommand())
                    {
                        try
                        {
                            string text = "SET FOREIGN_KEY_CHECKS = 0;";
                            PrepareCommand(cmd, trans, text, null);
                            cmd.ExecuteNonQuery();
                            //循环
                            foreach (DictionaryEntry myDE in SQLStringList)
                            {
                                string cmdText = myDE.Key.ToString();
                                MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Value;
                                if (cmdParms == null) continue;
                                PrepareCommand(cmd, trans, cmdText, cmdParms);
                                int val = cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                            text = "SET FOREIGN_KEY_CHECKS = 1;";
                            PrepareCommand(cmd, trans, text, null);
                            cmd.ExecuteNonQuery();
                            trans.Commit();
                            return true;
                        }
                        catch (MySqlException e)
                        {
                            trans.Rollback();
                            conn.Close();
                            Session.Output?.Invoke(null, $"错误：{e.Message}\r\n[{cmd.CommandText}]");
                            return false;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (MySqlException e)
                    {
                        connection.Close();
                        Session.Output?.Invoke(null, $"错误：{e.Message}\r\n[{SQLString}]");
                        return null;
                    }
                }
            }
        }
        private void PrepareCommand(MySqlCommand cmd, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
        {
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (MySqlParameter parameter in cmdParms)
                {
                    if (parameter == null) continue;
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }
        #endregion
#endif

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Index = 0;
                for (int i = Binding.Count - 1; i >= 0; i--)
                {
                    Binding[i].Dispose();
                }
                VersionValid = false;
                SaveList?.Clear();
                SaveList = null;
                ClientSaveList?.Clear();
                ClientSaveList = null;
            }
            base.Dispose(disposing);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Binding.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
