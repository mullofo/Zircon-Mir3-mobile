using MirDB;
using System;
using System.Collections.Generic;

namespace Library.MirDB
{
    /// <summary>
    /// 数据库集合
    /// </summary>
    public abstract class ADBCollection
    {
        /// <summary>
        /// INT计数
        /// </summary>
        public abstract int Count { get; }
        /// <summary>
        /// 内部数据库映射
        /// </summary>
        internal DBMapping Mapping { get; set; }
        /// <summary>
        /// 内部客户端数据库映射
        /// </summary>
        internal DBMapping ClientMapping { get; set; }
        /// <summary>
        /// 是否系统数据库
        /// </summary>
        internal bool IsSystemData { get; set; }
        /// <summary>
        /// 是否客户端数据库
        /// </summary>
        internal bool IsClientSystemData { get; set; }
        /// <summary>
        /// 内部网络
        /// </summary>
        internal Session Session { get; set; }
        /// <summary>
        /// 内部类型
        /// </summary>
        internal Type Type { get; set; }
        /// <summary>
        /// 只读布尔值
        /// </summary>
        internal bool ReadOnly { get; set; }
        /// <summary>
        /// 引发属性更改布尔值
        /// </summary>
        public bool RaisePropertyChanges { get; set; }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mapping"></param>
        internal abstract void Load(byte[] data, DBMapping mapping);
        /// <summary>
        /// 保存对象
        /// </summary>
        internal abstract void SaveObjects();

#if SERVER || ServerTool
        #region MYSQL
        /// <summary>
        /// MySql加载对象
        /// </summary>
        /// <param name="mapping"></param>
        internal abstract void LoadSQL();
        /// <summary>
        /// MySql初始化所有对象
        /// </summary>
        internal abstract void InitObjectToSQL();
        /// <summary>
        /// MySql初始化所有外键
        /// </summary>
        internal abstract void InitForeignKey();
        /// <summary>
        /// MySql保存新建及修改的对象
        /// </summary>
        internal abstract void SaveSQLParam();
        /// <summary>
        /// MySql数据转换到Z版二进制文件
        /// </summary>
        internal abstract void SqlToFile();
        /// <summary>
        /// MySql删除对象
        /// </summary>
        internal abstract void SaveObjectsSQL();
        #endregion
#endif
        /// <summary>
        /// 获取保存数据
        /// </summary>
        /// <returns></returns>
        internal abstract byte[] GetSaveData();
        /// <summary>
        /// 获取保存客户端数据
        /// </summary>
        /// <returns></returns>
        internal abstract byte[] GetClientSaveData();
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ob"></param>
        internal abstract void Delete(DBObject ob);
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <returns></returns>
        internal abstract DBObject CreateObject();
        /// <summary>
        /// 从一个对象黏贴到新对象
        /// </summary>
        /// <param name="oldOb"></param>
        /// <param name="subClass"></param>
        /// <returns></returns>
        internal abstract void PasteObject(DBObject oldOb, DBObject newOb, Dictionary<DBObject, DBObject> subClass);
        /// <summary>
        /// 从一个对象二进制流黏贴到新对象
        /// </summary>
        /// <param name="br"></param>
        /// <param name="subClass"></param>
        /// <returns></returns>
        internal abstract void PasteObject(System.IO.BinaryReader br, DBMapping mapping, DBObject newOb, Dictionary<DBObject, int> subClass);
        /// <summary>
        /// 按索引获取对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal abstract DBObject GetObjectByIndex(int index);
        /// <summary>
        /// 按字段名获取对象
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected internal abstract DBObject GetObjectbyFieldName(string fieldName, object value);
        /// <summary>
        /// 加载
        /// </summary>
        internal abstract void OnLoaded();

        internal bool IsDisposed { get; private set; }

        public event EventHandler Disposing;

        public void Dispose()
        {
            Dispose(!IsDisposed);
            GC.SuppressFinalize(this);
        }

        ~ADBCollection()
        {
            Dispose(disposing: false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposing?.Invoke(this, EventArgs.Empty);
                Mapping = null;
                ClientMapping = null;
                IsSystemData = false;
                IsClientSystemData = false;
                Session = null;
                Type = null;
                ReadOnly = false;
                RaisePropertyChanges = false;
                IsDisposed = true;
                Disposing = null;
            }
        }
    }
}
