using Library;
using Library.SystemModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Server
{
    public static class LangEx
    {
        public static Dictionary<string, string> Langs = new Dictionary<string, string>();
        /// <summary>
        /// 初始化国际化KEY
        /// </summary>
        public static void Init()
        {
            var list = SEnvir.LangInfoList.Binding;
            foreach (var item in list)
            {
                var language = item.LangType;
                var key = $"{language}.{item.Type}.{item.Key.ToUpper()}";
                var lang = item.Value?.Replace("\\n", "\n");
                if (!Langs.ContainsKey(key))
                {
                    Langs.Add(key, lang);
                }
                else
                {
#if DEBUG
                    SEnvir.Log($"[警告] 国际化重复KEY:{key}");
#endif
                }
            }
        }
        public static string Lang<T>(this T target, Language language, Expression<Func<T, string>> memberLambda) where T : MirDB.DBObject
        {
            if (memberLambda.Body is MemberExpression memberSelectorExpression)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    var v = property.GetValue(target);
                    if (v == null)
                    {
                        return null;
                    }
                    var result = (string)v;
                    if (target.GetType().GetProperty(property.Name).GetCustomAttribute(typeof(MirDB.Association.LangAttribute), false) == null)
                    {
                        return result;
                    }
                    var key = $"{language}.{target.GetType().Name}.{property.Name}.{result.ToUpper()}";
                    if (Langs.TryGetValue(key, out string lang))
                    {
                        return lang;
                    }
                    return lang ?? result;
                }
            }
            return string.Empty;
        }
        public static string Lang(this Enum target, Language language)
        {
            var result = target.ToString();
            var key = $"{language}.{target.GetType().FullName}.{result.ToUpper()}";
            if (Langs.TryGetValue(key, out string lang))
            {
                return lang;
            }

            return lang ?? result;
        }

        public static string Lang(this string target, Language language, params object[] values)
        {
            var key = $"{language}.{LangInfo.ServerKey}.{target.Trim().ToUpper()}";
            if (!Langs.TryGetValue(key, out string lang))
            {
                lang = lang ?? target;
            }
            lang = values == null || values.Length == 0 ? lang : string.Format(lang, values);
            return lang;
        }

        public static string Lang(this TimeSpan time, Language language, bool details, bool small = false)
        {
            string textD = null;
            string textH = null;
            string textM = null;
            string textS = null;

            if (time.Days >= 1) textD = "System.Days".Lang(language, time.Days);

            if (time.Hours >= 1) textH = "System.Hours".Lang(language, time.Hours);

            if (time.Minutes >= 1) textM = "System.Minutes".Lang(language, time.Minutes);

            if (time.Seconds >= 1) textS = "System.Seconds".Lang(language, time.Seconds);
            else if (time.TotalSeconds > 1 && time.Seconds > 0) textS = "System.LessSecond".Lang(language);//"不到一秒钟";

            if (!details)
                return textD ?? textH ?? textM ?? textS;

            if (textD != null)
                return textD + " " + (textH ?? textM ?? textS);

            if (textH != null)
                return textH + " " + (textM ?? textS);

            if (textM != null)
                return textM + " " + textS;

            return textS?.Replace("\\n", "\n") ?? string.Empty;
        }
    }

}