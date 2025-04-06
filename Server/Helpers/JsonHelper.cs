using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Server

{
    /// <summary>
    /// JSON解析器
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 将对象序列化为JSON格式
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns>json字符串</returns>
        public static string SerializeObject(object o)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string json = JsonConvert.SerializeObject(o);
            return json;
        }

        /// <summary>
        /// 解析JSON字符串生成对象实体
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串(eg.{"ID":"112","Name":"石子儿"})</param>
        /// <returns>对象实体</returns>
        public static T DeserializeJsonToObject<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
            T t = o as T;
            return t;
        }

        /// <summary>
        /// 解析JSON数组生成对象实体集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"石子儿"}])</param>
        /// <returns>对象实体集合</returns>
        public static List<T> DeserializeJsonToList<T>(string json) where T : class
        {
            //JsonSerializerSettings settings = new JsonSerializerSettings();
            //settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //List<T> lista = JsonConvert.DeserializeObject<T>(json, settings) as List<T>;
            //return lista;


            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = o as List<T>;
            return list;
        }

        /// <summary>
        /// 反序列化JSON到给定的匿名对象.
        /// </summary>
        /// <typeparam name="T">匿名对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <param name="anonymousTypeObject">匿名对象</param>
        /// <returns>匿名对象</returns>
        public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
        {
            T t = JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
            return t;
        }
    }

    public class TypeUtil
    {
        public static T Copy<F, T>(F obj) where T : class
        {
            T result;
            try
            {
                result = Copy2<F, T>(obj);
            }
            catch
            {
                result = Copy2<F, T>(obj);
            }
            return result;
        }
        public static IList<T> CopyList<F, T>(IList<F> objs) where T : class
        {
            IList<T> result;
            if (objs == null)
            {
                result = null;
            }
            else
            {
                List<T> list = new List<T>();
                foreach (F current in objs)
                {
                    list.Add(Copy<F, T>(current));
                }
                result = list;
            }
            return result;
        }

        private static T Copy2<F, T>(F obj) where T : class
        {
            T result;
            if (obj == null)
            {
                result = default(T);
            }
            else
            {
                PropertyInfo[] properties = typeof(T).GetProperties();
                PropertyInfo[] properties2 = typeof(F).GetProperties();
                object obj2 = Activator.CreateInstance(typeof(T));
                PropertyInfo[] array = properties2;
                for (int i = 0; i < array.Length; i++)
                {
                    PropertyInfo p = array[i];
                    object value = p.GetValue(obj, null);
                    PropertyInfo propertyInfo = properties.FirstOrDefault((PropertyInfo c) => c.Name == p.Name);
                    if (propertyInfo != null && p.PropertyType == propertyInfo.PropertyType)
                    {
                        propertyInfo.SetValue(obj2, value, null);
                    }

                }
                result = (obj2 as T);
            }
            return result;
        }
    }
}
