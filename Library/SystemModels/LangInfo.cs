using MirDB;
using System;
using System.Collections.Generic;
using static MirDB.Association;

namespace Library.SystemModels
{
    public class LangInfo : DBObject
    {

        private string _type;
        /// <summary>
        /// 类型
        /// </summary>
        public string Type
        {
            get { return _type; }
            set
            {
                if (_type == value) return;

                var oldValue = _type;
                _type = value;

                OnChanged(oldValue, value, "Type");
            }
        }

        private Language _langType;
        /// <summary>
        /// 语言类型
        /// </summary>
        public Language LangType
        {
            get { return _langType; }
            set
            {
                if (_langType == value) return;

                var oldValue = _langType;
                _langType = value;

                OnChanged(oldValue, value, "LangType");
            }
        }

        private string _key;
        /// <summary>
        /// 值
        /// </summary>
        public string Key
        {
            get { return _key; }
            set
            {
                if (_key == value) return;

                var oldValue = _key;
                _key = value;

                OnChanged(oldValue, value, "Key");
            }
        }

        private string _value;
        /// <summary>
        /// 值
        /// </summary>
        public string Value
        {
            get { return _value; }
            set
            {
                if (_value == value) return;

                var oldValue = _value;
                _value = value;

                OnChanged(oldValue, value, "Value");
            }
        }
        [IgnoreProperty]
        public static string ClientKey => "LangClient";
        [IgnoreProperty]
        public static string ServerKey => "LangServer";

        /// <summary>
        /// 获取需要国际化的字段
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetLangs()
        {
            var result = new Dictionary<string, string>();
            var assembly = typeof(LangInfo).Assembly;
            var types = assembly.GetExportedTypes();
            foreach (var type in types)
            {
                var attrs = type.GetCustomAttributes(typeof(LangAttribute), false);
                if (attrs == null || attrs.Length == 0) continue;
                if (type.IsClass)
                {
                    foreach (var prop in type.GetProperties())
                    {
                        var lang = prop.GetCustomAttributes(typeof(LangAttribute), false);
                        if (lang == null || lang.Length == 0) continue;
                        var desc = ((LangAttribute)lang[0]).Descrption ?? prop.Name;
                        result.Add($"{type.Name}.{prop.Name}", desc);
                    }
                }
                else if (type.IsEnum)
                {
                    var lang = type.GetCustomAttributes(typeof(LangAttribute), false);
                    if (lang == null || lang.Length == 0) continue;
                    var desc = ((LangAttribute)lang[0]).Descrption ?? type.Name;
                    result.Add($"{type.FullName}", desc);
                }

            }
            //其它常量
            result.Add(ClientKey, "客户端");
            result.Add(ServerKey, "服务端");
            return result;
        }

        public static Type GetType(string path)
        {
            var assembly = typeof(LangInfo).Assembly;
            return assembly.GetType(path);
        }
        public static List<string> GetEnumValue(string path)
        {
            var result = new List<string>();
            var assembly = typeof(LangInfo).Assembly;
            var type = assembly.GetType(path);
            if (!type.IsEnum) return result;
            foreach (var temp in Enum.GetNames(type))
            {
                result.Add(temp);
            }
            return result;
        }
    }
}
