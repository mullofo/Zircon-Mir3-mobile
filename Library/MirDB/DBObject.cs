using Library.MirDB;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;

namespace MirDB
{
    /// <summary>
    /// 数据对象 通知属性变更
    /// </summary>
    public abstract class DBObject : INotifyPropertyChanged, IDisposable
    {
        internal static ConcurrentDictionary<PropertyInfo, PropertyInfo> RelationDictionary = new ConcurrentDictionary<PropertyInfo, PropertyInfo>();
        /// <summary>
        /// 公共类索引 获取内部集合
        /// </summary>
        public int Index { get; internal set; }
        /// <summary>
        /// 是否服务端类型
        /// </summary>
        public bool ServerOnly
        {
            get
            {
                return _ServerOnly;
            }
            set
            {
                if (_ServerOnly != value)
                {
                    bool serverOnly = _ServerOnly;
                    _ServerOnly = value;
                    OnChanged(serverOnly, value, "ServerOnly");
                }
            }
        }
        private bool _ServerOnly;

        /// <summary>
        /// 创建时间
        /// </summary>
        [ServerOnly]
        public DateTime CreatTime
        {
            get
            {
                return _CreatTime;
            }
            set
            {
                if (_CreatTime != value)
                {
                    DateTime creatTime = _CreatTime;
                    _CreatTime = value;
                    OnChanged(creatTime, value, "CreatTime");
                }
            }
        }

        private DateTime _CreatTime;
        /// <summary>
        /// 内部数据库集合
        /// </summary>
        [IgnoreProperty]  //忽略属性
        internal ADBCollection Collection
        {
            get { return _Collection; }
            set
            {
                if (_Collection != null) return;

                _Collection = value;

                CreateBindings();
            }
        }
        private ADBCollection _Collection;

        /// <summary>
        /// 内部只读类型
        /// </summary>
        internal readonly Type ThisType;
        /// <summary>
        /// 是否加载
        /// </summary>
        [IgnoreProperty]
        protected internal bool IsLoaded { get; private set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        [IgnoreProperty]
        protected internal bool IsDeleted { get; private set; }
        /// <summary>
        /// 临时缓存
        /// </summary>
        [IgnoreProperty]
        public bool IsTemporary { get; set; }
        /// <summary>
        /// 是否数据有修改
        /// </summary>
        [IgnoreProperty]
        protected internal bool IsModified { get; private set; }
        /// <summary>
        /// 是否新创建的
        /// </summary>
        [IgnoreProperty]
        protected internal bool IsNewCreated { get; private set; }
        /// <summary>
        /// 保存内存流
        /// </summary>
        private MemoryStream SaveMemoryStream;
        /// <summary>
        /// 保存二进制写入程序
        /// </summary>
        private BinaryWriter SaveBinaryWriter;
        /// <summary>
        /// 原始数据
        /// </summary>
        protected internal byte[] RawData;
        /// <summary>
        /// 客户端原始数据
        /// </summary>
        protected internal byte[] ClientRawData;

        protected DBObject()
        {
            ThisType = GetType();
        }
        /// <summary>
        /// 内部负载(数据库映射)
        /// </summary>
        /// <param name="mapping"></param>
        internal void Load(DBMapping mapping)
        {
            IsLoaded = false;
            using (MemoryStream mStream = new MemoryStream(RawData))
            using (BinaryReader reader = new BinaryReader(mStream))
            {
                foreach (DBValue dbValue in mapping.Properties)
                {
                    object value = dbValue.ReadValue(reader);

                    if (dbValue.Property == null) continue;

                    if (dbValue.Property.PropertyType.IsSubclassOf(typeof(DBObject)))
                    {
                        if (!Collection.Session.Relationships.TryGetValue(dbValue.Property.PropertyType, out DBRelationship relationship))
                        {
                            lock (Collection.Session.Relationships)
                                if (!Collection.Session.Relationships.TryGetValue(dbValue.Property.PropertyType, out relationship))
                                    Collection.Session.Relationships[dbValue.Property.PropertyType] = relationship = new DBRelationship(dbValue.Property.PropertyType);
                        }

                        if (!relationship.LinkTargets.TryGetValue((int)value, out DBRelationshipTargets targets))
                        {
                            lock (relationship)
                                if (!relationship.LinkTargets.TryGetValue((int)value, out targets))
                                    relationship.LinkTargets[(int)value] = targets = new DBRelationshipTargets();
                        }

                        if (!targets.PropertyTargets.TryGetValue(dbValue.Property, out ConcurrentQueue<DBObject> list))
                        {
                            lock (targets)
                                if (!targets.PropertyTargets.TryGetValue(dbValue.Property, out list))
                                    targets.PropertyTargets[dbValue.Property] = list = new ConcurrentQueue<DBObject>();
                        }

                        list.Enqueue(this);

                        continue;
                    }

                    if (dbValue.PropertyType.IsEnum)
                    {
                        if (dbValue.PropertyType.GetEnumUnderlyingType() == dbValue.Property.PropertyType)
                        {
                            dbValue.Property.SetValue(this, value);
                            continue;
                        }
                    }
                    else if (dbValue.PropertyType == dbValue.Property.PropertyType)
                    {
                        dbValue.Property.SetValue(this, value);
                        continue;
                    }

                    try
                    {
                        dbValue.Property.SetValue(this, Convert.ChangeType(value, dbValue.PropertyType));
                    }
                    catch { }
                }
            }
        }
        /// <summary>
        /// 复制一个对象到数组
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] CopyObject()
        {
            Save();
            using (MemoryStream mem = new MemoryStream())
            using (BinaryWriter wr = new BinaryWriter(mem))
            {
                wr.Write(RawData.Length);
                wr.Write(RawData);

                using (MemoryStream ms = new MemoryStream())
                using (BinaryWriter br = new BinaryWriter(ms))
                {
                    int bindcount = 0;
                    PropertyInfo[] properties = ThisType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
                    foreach (PropertyInfo property in properties)
                    {
                        Association link = property.GetCustomAttribute<Association>();
                        if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(DBBindingList<>)) continue;

                        if (link != null)
                        {
                            IBindingList list = (IBindingList)property.GetValue(this);
                            if (list.Count <= 0) continue;
                            ((DBObject)list[0]).Collection.Mapping.Save(br);
                            int count = 0;
                            List<byte> data = new List<byte>();
                            for (int i = list.Count - 1; i >= 0; i--)
                            {
                                DBObject listOb = (DBObject)list[i];
                                data.AddRange(listOb.CopyObject());
                                count++;
                            }
                            br.Write(count);
                            br.Write(data.ToArray());

                            bindcount++;
                        }
                    }
                    if (bindcount > 0)
                    {
                        wr.Write(bindcount);
                        wr.Write(ms.ToArray());
                    }
                    else
                        wr.Write(0);
                }

                return mem.ToArray();
            }
        }
        /// <summary>
        /// 从对象二进制流赋值到新对象
        /// </summary>
        /// <param name="sourceMapping"></param>
        /// <param name="subClass"></param>
        /// <returns></returns>
        internal void PasteObject(DBMapping sourceMapping, Dictionary<DBObject, int> subClass)
        {
            using (MemoryStream mStream = new MemoryStream(RawData))
            using (BinaryReader reader = new BinaryReader(mStream))
            {
                foreach (DBValue dbValue in sourceMapping.Properties)
                {
                    object value = dbValue.ReadValue(reader);

                    if (dbValue.Property == null) continue;
                    if (dbValue.PropertyName == "Index")
                    {
                        subClass.Add(this, (int)value);
                        continue;
                    }

                    if (dbValue.Property.PropertyType.IsSubclassOf(typeof(DBObject)))
                    {
                        if (dbValue.Property.GetValue(this) != null) continue;
                        ADBCollection coll = Collection.Session.GetCollection(dbValue.Property.PropertyType);
                        dbValue.Property.SetValue(this, coll.GetObjectByIndex((int)value));
                        continue;
                    }

                    if (dbValue.PropertyType.IsEnum)
                    {
                        if (dbValue.PropertyType.GetEnumUnderlyingType() == dbValue.Property.PropertyType)
                        {
                            dbValue.Property.SetValue(this, value);
                            continue;
                        }
                    }
                    else if (dbValue.PropertyType == dbValue.Property.PropertyType)
                    {
                        dbValue.Property.SetValue(this, value);
                        continue;
                    }

                    try
                    {
                        dbValue.Property.SetValue(this, Convert.ChangeType(value, dbValue.PropertyType));
                    }
                    catch { }
                }
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        internal void Save()
        {
            if (IsTemporary) return;

            //处理流可能会导致线程上的GC收集

            if (SaveMemoryStream == null)
                SaveMemoryStream = new MemoryStream();

            if (SaveBinaryWriter == null)
                SaveBinaryWriter = new BinaryWriter(SaveMemoryStream);

            SaveMemoryStream.SetLength(0);

            foreach (DBValue dbValue in Collection.Mapping.Properties)
            {
                if (dbValue.Property.PropertyType.IsSubclassOf(typeof(DBObject)))
                {
                    DBObject linkOb = (DBObject)dbValue.Property.GetValue(this);
                    dbValue.WriteValue(linkOb?.Index ?? 0, SaveBinaryWriter);
                }
                else
                    dbValue.WriteValue(dbValue.Property.GetValue(this), SaveBinaryWriter);
            }

            RawData = SaveMemoryStream.ToArray();

            OnSaved();
        }
        /// <summary>
        /// 客户端保存
        /// </summary>
        internal void ClientSave()
        {
            if (IsTemporary) return;

            if (SaveMemoryStream == null)
                SaveMemoryStream = new MemoryStream();

            if (SaveBinaryWriter == null)
                SaveBinaryWriter = new BinaryWriter(SaveMemoryStream);

            SaveMemoryStream.SetLength(0);

            foreach (DBValue dbValue in Collection.ClientMapping.Properties)
            {
                if (dbValue.Property.PropertyType.IsSubclassOf(typeof(DBObject)))
                {
                    DBObject linkOb = (DBObject)dbValue.Property.GetValue(this);
                    dbValue.WriteValue(linkOb?.Index ?? 0, SaveBinaryWriter);
                }
                else
                    dbValue.WriteValue(dbValue.Property.GetValue(this), SaveBinaryWriter);
            }
            ClientRawData = SaveMemoryStream.ToArray();
        }


#if SERVER || ServerTool
        #region MYSQL
        internal void LoadSQL(int row, DataTable dt)
        {
            foreach (DBValue dbValue in Collection.Mapping.Properties)
            {
                //新增的字段跳过
                if (!dt.Columns.Contains(dbValue.PropertyName)) continue;
                if (dbValue.Property == null) continue;
                object value = dbValue.ReadValueSQL(dt.Rows[row][dbValue.PropertyName]);

                if (dbValue.Property.PropertyType.IsSubclassOf(typeof(DBObject)))
                {
                    //没有关联类的字段值统一设为0。
                    if (value == null)
                        value = 0;

                    if (!Collection.Session.Relationships.TryGetValue(dbValue.Property.PropertyType, out DBRelationship relationship))
                    {
                        lock (Collection.Session.Relationships)
                            if (!Collection.Session.Relationships.TryGetValue(dbValue.Property.PropertyType, out relationship))
                                Collection.Session.Relationships[dbValue.Property.PropertyType] = relationship = new DBRelationship(dbValue.Property.PropertyType);
                    }

                    if (!relationship.LinkTargets.TryGetValue((int)value, out DBRelationshipTargets targets))
                    {
                        lock (relationship)
                            if (!relationship.LinkTargets.TryGetValue((int)value, out targets))
                                relationship.LinkTargets[(int)value] = targets = new DBRelationshipTargets();
                    }

                    if (!targets.PropertyTargets.TryGetValue(dbValue.Property, out ConcurrentQueue<DBObject> list))
                    {
                        lock (targets)
                            if (!targets.PropertyTargets.TryGetValue(dbValue.Property, out list))
                                targets.PropertyTargets[dbValue.Property] = list = new ConcurrentQueue<DBObject>();
                    }

                    list.Enqueue(this);


                    continue;
                }

                if (dbValue.PropertyType.IsEnum)
                {
                    if (dbValue.PropertyType.GetEnumUnderlyingType() == dbValue.Property.PropertyType)
                    {
                        dbValue.Property.SetValue(this, value);
                        continue;
                    }
                }
                else if (dbValue.PropertyType == dbValue.Property.PropertyType)
                {
                    dbValue.Property.SetValue(this, value);
                    continue;
                }

                try
                {
                    dbValue.Property.SetValue(this, Convert.ChangeType(value, dbValue.PropertyType));
                }
                catch { }
            }
        }

        //行数据统一保存到DataTable,然后批量插入表
        internal void SaveDataTable(DataTable dt)
        {
            if (IsTemporary) return;

            //行赋值
            DataRow dr = dt.NewRow();
            foreach (DBValue dbValue in Collection.Mapping.Properties)
            {
                object value;
                if (dbValue.Property.PropertyType.IsSubclassOf(typeof(DBObject)))
                {
                    DBObject linkOb = (DBObject)dbValue.Property.GetValue(this);
                    value = dbValue.WriteValueSQL(linkOb?.Index ?? 0);
                    //没有关联类的字段，数据库统一保存为null，防止外键约束报错。
                    if ((int)value == 0)
                        value = DBNull.Value;
                }
                else
                    value = dbValue.WriteValueSQL(dbValue.Property.GetValue(this));

                dr[dbValue.PropertyName] = value;
            }
            dt.Rows.Add(dr);
        }
        #endregion
#endif
        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            if (Collection.ReadOnly) return;

            PropertyChanged = null;

            Collection.Session.Delete(this);
        }
        /// <summary>
        /// 快速删除
        /// </summary>
        public void FastDelete()
        {
            if (Collection.ReadOnly) return;

            Collection.Session.FastDelete(this);
        }

        /// <summary>
        /// 创建时
        /// </summary>
        protected virtual internal void OnCreated()
        {
            IsModified = true;
            IsLoaded = true;
            IsNewCreated = true;
            CreatTime = DateTime.Now;
        }
        /// <summary>
        /// 读取时
        /// </summary>
        protected virtual internal void OnLoaded()
        {
            IsLoaded = true;
        }
        /// <summary>
        /// 储存时
        /// </summary>
        protected virtual internal void OnSaved()
        {
            IsModified = false;
            IsNewCreated = false;
        }
        /// <summary>
        /// 删除时
        /// </summary>
        protected virtual internal void OnDeleted()
        {
            IsDeleted = true;
        }
        /// <summary>
        /// 绑定
        /// </summary>
        public void CreateBindings()
        {
            PropertyInfo[] properties = ThisType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(DBBindingList<>)) continue;

                property.SetValue(this, Activator.CreateInstance(property.PropertyType, this, property));
            }
        }
        /// <summary>
        /// 创建链接
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="info"></param>
        private void CreateLink(object ob, PropertyInfo info)
        {
            if (ob == null) return;

            Association link = null;
            if (!RelationDictionary.TryGetValue(info, out var best))
            {
                link = info.GetCustomAttribute<Association>();
                if (link == null) return;

                PropertyInfo[] properties = ob.GetType().GetProperties();
                foreach (PropertyInfo p in properties)
                {
                    Association obLink = p.GetCustomAttribute<Association>();

                    if (obLink == null || obLink.Identity != link.Identity) continue;

                    if (p.PropertyType == info.DeclaringType)
                    {
                        best = p;

                        if (p != info)
                            break;
                    }
                    else if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DBBindingList<>) &&
                        p.PropertyType.GetGenericArguments()[0] == info.DeclaringType) //Basically this type
                    {
                        best = p;

                        if (p != info)
                            break;
                    }
                }
                RelationDictionary[info] = best;
            }

            if (best != null)
            {
                if (best.PropertyType == info.DeclaringType)
                {
                    best.SetValue(ob, this);
                    return;
                }

                if (best.PropertyType.IsGenericType && best.PropertyType.GetGenericTypeDefinition() == typeof(DBBindingList<>) &&
                    best.PropertyType.GetGenericArguments()[0] == info.DeclaringType) //Basically this type
                {
                    ((IBindingList)best.GetValue(ob)).Add(this);
                    return;
                }
            }

            if (link == null)
                link = info.GetCustomAttribute<Association>();

            //TODO 这一行会引起调试缓慢
            //throw new ArgumentException($"找不到关联 {ThisType.Name}, 链接: {link.Identity ?? "Empty"} -> {info.PropertyType.Name}");
        }
        /// <summary>
        /// 删除链接
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="info"></param>
        private void RemoveLink(object ob, PropertyInfo info)
        {
            if (ob == null) return;

            Association link = null;
            if (!RelationDictionary.TryGetValue(info, out var best))
            {
                link = info.GetCustomAttribute<Association>();
                if (link == null) return;

                PropertyInfo[] properties = ob.GetType().GetProperties();
                foreach (PropertyInfo p in properties)
                {
                    Association obLink = p.GetCustomAttribute<Association>();

                    if (obLink == null || obLink.Identity != link.Identity) continue;

                    if (p.PropertyType == info.DeclaringType)
                    {
                        best = p;

                        if (p != info) break;
                    }

                    if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DBBindingList<>) &&
                        p.PropertyType.GetGenericArguments()[0] == info.DeclaringType) //Basically this type
                    {
                        best = p;

                        if (p != info) break;
                    }
                }
                RelationDictionary[info] = best;
            }

            if (best != null)
            {
                if (best.PropertyType == info.DeclaringType)
                {
                    best.SetValue(ob, null);
                    return;
                }

                if (best.PropertyType.IsGenericType && best.PropertyType.GetGenericTypeDefinition() == typeof(DBBindingList<>) &&
                    best.PropertyType.GetGenericArguments()[0] == info.DeclaringType) //Basically this type
                {
                    ((IBindingList)best.GetValue(ob)).Remove(this);
                    return;
                }
            }

            if (link == null)
                link = info.GetCustomAttribute<Association>();

            throw new ArgumentException($"找不到关联 {ThisType.Name}, 链接: {link.Identity ?? "Empty"} -> {info.PropertyType.Name}");
        }

        #region 属性更改时
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性更改是
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        protected virtual void OnChanged(object oldValue, object newValue, string propertyName)
        {
            try
            {
                if (Collection?.Session?.Relationships == null)
                    IsModified = true;

                if (oldValue is DBObject || newValue is DBObject)
                {
                    PropertyInfo info = ThisType.GetProperty(propertyName);

                    RemoveLink(oldValue, info);
                    CreateLink(newValue, info);
                }

                if (IsLoaded && Collection.RaisePropertyChanges)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (System.Exception)
            {

            }
        }
        #endregion


        [IgnoreProperty]
        protected bool IsDisposed { get; private set; }

        public event EventHandler Disposing;

        public void Dispose()
        {
            Dispose(!IsDisposed);
            GC.SuppressFinalize(this);
        }

        ~DBObject()
        {
            Dispose(disposing: false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposing?.Invoke(this, EventArgs.Empty);
                Index = 0;
                _Collection = null;
                IsLoaded = false;
                IsDeleted = false;
                IsTemporary = false;
                IsModified = false;
                SaveMemoryStream?.Dispose();
                SaveMemoryStream = null;
                SaveBinaryWriter?.Dispose();
                SaveBinaryWriter = null;
                RawData = null;
                ClientRawData = null;
                IsDisposed = true;
                Disposing = null;
            }
        }
    }
}
