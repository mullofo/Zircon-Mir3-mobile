using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MirDB
{
    /// <summary>
    /// 数据库映射
    /// </summary>
    public sealed class DBMapping
    {
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; }
        /// <summary>
        /// 列出<数据值>属性
        /// </summary>
        public List<DBValue> Properties { get; } = new List<DBValue>();

        public Assembly[] Assemblies { get; private set; }

        public bool ClientData { get; set; }
        /// <summary>
        /// 数据库映射类型
        /// </summary>
        /// <param name="type"></param>
        public DBMapping(Assembly[] assemblies, Type type, bool clientData = false)
        {
            Assemblies = assemblies;

            Type = type;
            ClientData = clientData;

            PropertyInfo[] properties = Type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);

            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttribute<IgnoreProperty>() != null) continue;
                if (ClientData && property.GetCustomAttribute<ServerOnly>() != null) continue;
                if (!DBValue.TypeList.ContainsValue(property.PropertyType) && !property.PropertyType.IsEnum && !property.PropertyType.IsSubclassOf(typeof(DBObject))) continue;

                Properties.Add(new DBValue(property));
            }
        }
        /// <summary>
        /// 数据库映射(二进制读取)
        /// </summary>
        /// <param name="reader"></param>
        public DBMapping(Assembly[] assemblies, BinaryReader reader)
        {
            Assemblies = assemblies;

            string typeName = reader.ReadString();
            Type = Assemblies.Select(x => x.GetType(typeName)).FirstOrDefault(x => x != null);

            if (Type == null)
            {
                typeName = typeName.Replace("Server.DBModels", "Library.SystemModels");
                Type = Assembly.GetEntryAssembly().GetType(typeName) ?? Assembly.GetCallingAssembly().GetType(typeName);
            }

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                Properties.Add(new DBValue(reader, Type));
        }
        /// <summary>
        /// 保存(二进制写入程序)
        /// </summary>
        /// <param name="writer"></param>
        public void Save(BinaryWriter writer)
        {
            writer.Write(Type.FullName);

            writer.Write(Properties.Count);

            foreach (DBValue value in Properties)
                value.Save(writer);
        }
        /// <summary>
        /// 匹配(数据库映射)
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public bool IsMatch(DBMapping mapping)
        {
            if (Properties.Count != mapping.Properties.Count) return false;

            for (int i = 0; i < Properties.Count; i++)
                if (!Properties[i].IsMatch(mapping.Properties[i])) return false;

            return true;
        }
    }
}
