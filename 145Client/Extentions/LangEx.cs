using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Client.Extentions
{
    public static class LangEx
    {
        public static Dictionary<string, string> Langs = new Dictionary<string, string>();

        /// <summary>
        /// 初始化国际化KEY
        /// </summary>
        public static void Init()
        {
            var list = Globals.LangInfoList.Binding;
            foreach (var item in list)
            {
                var language = item.LangType;
                var key = $"{language}.{item.Type}.{item.Key.ToUpper()}";
                var lang = item.Value?.Replace("\\n", "\n");
                if (!Langs.ContainsKey(key))
                {
                    Langs.Add(key, lang);
                }
            }
        }

        public static string Lang<T>(this T target, Expression<Func<T, string>> memberLambda) where T : MirDB.DBObject
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
                    if (!Enum.TryParse(Envir.Config.Language, out Language type))
                    {
                        type = Language.SimplifiedChinese;
                    }
                    var key = $"{type}.{target.GetType().Name}.{property.Name}.{result.ToUpper()}";
                    if (Langs.TryGetValue(key, out string lang))
                    {
                        return lang;
                    }

                    return lang ?? result;
                }
            }
            return string.Empty;
        }
        public static string Lang(this Enum target)
        {
            if (!Enum.TryParse(Envir.Config.Language, out Language type))
            {
                type = Language.SimplifiedChinese;
            }
            var result = target.ToString();

            var key = $"{type}.{target.GetType().FullName}.{result.ToUpper()}";
            if (Langs.TryGetValue(key, out string lang))
            {
                return lang;
            }
            return lang ?? result;
        }

        public static string Lang(this string target, params object[] values)
        {
            if (Globals.LangInfoList == null) return target;

            if (!Enum.TryParse(Envir.Config.Language, out Language type))
            {
                type = Language.SimplifiedChinese;
            }
            var key = $"{type}.{LangInfo.ClientKey}.{target.Trim().ToUpper()}";
            if (!Langs.TryGetValue(key, out string lang))
            {
                lang = lang ?? target;
            }
            lang = values == null || values.Length == 0 ? lang : string.Format(lang, values);
            return lang;
        }

        public static string Lang(this TimeSpan time, bool details)
        {
            string textD = null;
            string textH = null;
            string textM = null;
            string textS = null;

            if (time.Days >= 1) textD = "System.Days".Lang(time.Days);

            if (time.Hours >= 1) textH = "System.Hours".Lang(time.Hours);

            if (time.Minutes >= 1) textM = "System.Minutes".Lang(time.Minutes);

            if (time.Seconds >= 1) textS = "System.Seconds".Lang(time.Seconds);
            else if (time.TotalSeconds > 1 && time.Seconds > 0) textS = "System.LessSecond".Lang();//"不到一秒钟";

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
