using System;
using System.ComponentModel;
using System.Reflection;

namespace MirDB
{
    /// <summary>
    /// 数据库绑定列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DBBindingList<T> : BindingList<T> where T : DBObject, new()
    {
        /// <summary>
        /// 会话
        /// </summary>
        private readonly Session Session;
        /// <summary>
        /// 数据库父级对象
        /// </summary>
        private readonly DBObject Parent;
        /// <summary>
        /// 属性信息
        /// </summary>
        private readonly PropertyInfo Property;
        /// <summary>
        /// 关联链接
        /// </summary>
        private readonly Association Link;
        /// <summary>
        /// 数据库绑定列表
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="property"></param>
        public DBBindingList(DBObject parent, PropertyInfo property)
        {
            Session = parent.Collection.Session;
            Parent = parent;
            Property = property;
            Link = property.GetCustomAttribute<Association>();

            RaiseListChangedEvents = parent.Collection.RaisePropertyChanges;
        }
        /// <summary>
        /// 覆盖添加新的
        /// </summary>
        /// <param name="e"></param>
        protected override void OnAddingNew(AddingNewEventArgs e)
        {
            base.OnAddingNew(e);

            e.NewObject = Session.CreateObject<T>();
        }
        /// <summary>
        /// 替代作废插入项
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, T item)
        {
            if (Items.Contains(item)) return;

            base.InsertItem(index, item);

            CreateLink(item);
        }
        /// <summary>
        /// 覆盖作废删除项
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            T ob = Items[index];

            base.RemoveItem(index);

            RemoveLink(ob);
        }
        /// <summary>
        /// 创建链接
        /// </summary>
        /// <param name="ob"></param>
        public void CreateLink(T ob)
        {
            if (ob == null || Link == null) return;

            if (!DBObject.RelationDictionary.TryGetValue(Property, out var best))
            {
                PropertyInfo[] properties = ob.GetType().GetProperties();
                foreach (PropertyInfo p in properties)
                {
                    Association obLink = p.GetCustomAttribute<Association>();

                    if (obLink == null || obLink.Identity != Link.Identity || p.PropertyType != Property.DeclaringType) continue;

                    best = p;

                    if (p != Property) break;
                }
                DBObject.RelationDictionary[Property] = best;
            }

            if (best != null)
            {
                best.SetValue(ob, Parent);
                return;
            }

            throw new ArgumentException($"找不到关联 {Parent.ThisType.Name}, 链接: {Link.Identity ?? "Empty"} -> {ob.GetType()}");
        }
        /// <summary>
        /// 删除链接
        /// </summary>
        /// <param name="ob"></param>
        public void RemoveLink(T ob)
        {
            if (ob == null || Link == null) return;

            if (!DBObject.RelationDictionary.TryGetValue(Property, out var best))
            {
                PropertyInfo[] properties = ob.GetType().GetProperties();
                foreach (PropertyInfo p in properties)
                {
                    Association obLink = p.GetCustomAttribute<Association>();

                    if (obLink == null || obLink.Identity != Link.Identity || p.PropertyType != Property.DeclaringType) continue;

                    best = p;

                    if (p != Property) break;
                }
                DBObject.RelationDictionary[Property] = best;
            }

            if (best != null)
            {
                best.SetValue(ob, null);
                return;
            }

            throw new ArgumentException($"找不到关联 {Parent.ThisType.Name}, 链接: {Link.Identity ?? "Empty"} -> {ob.GetType()}");
        }
    }
}
