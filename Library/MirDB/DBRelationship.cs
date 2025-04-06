using MirDB;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Library.MirDB
{
    /// <summary>
    /// 数据库关系
    /// </summary>
    internal class DBRelationship
    {
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// 链接目标
        /// </summary>
        public Dictionary<int, DBRelationshipTargets> LinkTargets = new Dictionary<int, DBRelationshipTargets>();

        public DBRelationship(Type type)
        {
            Type = type;
        }
        /// <summary>
        /// 使用密匙
        /// </summary>
        /// <param name="session"></param>
        public void ConsumeKeys(Session session)
        {

            foreach (KeyValuePair<int, DBRelationshipTargets> pair in LinkTargets)
            {
                DBObject linkOb = session.GetObject(Type, pair.Key);

                foreach (KeyValuePair<PropertyInfo, ConcurrentQueue<DBObject>> target in pair.Value.PropertyTargets)
                {
                    while (!target.Value.IsEmpty)
                    {
                        if (!target.Value.TryDequeue(out DBObject targetOb)) continue;

                        target.Key.SetValue(targetOb, linkOb);
                    }
                }
                pair.Value.PropertyTargets.Clear();
            }
            LinkTargets.Clear();
        }
    }
    /// <summary>
    /// 数据库关系目标
    /// </summary>
    internal class DBRelationshipTargets
    {
        public Dictionary<PropertyInfo, ConcurrentQueue<DBObject>> PropertyTargets = new Dictionary<PropertyInfo, ConcurrentQueue<DBObject>>();
    }
}
