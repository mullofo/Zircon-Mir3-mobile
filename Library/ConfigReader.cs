﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
#if Mobile
using Client.Envir;
#endif

namespace Library
{
    public class MagicHelper
    {
        public MagicType TypeID { get; set; }
        public string Name { get; set; }
        public SpellKey Key { get; set; }
        public bool LockPlayer { get; set; }
        public bool LockMonster { get; set; }
        public int Amulet { get; set; }
        public object obj { get; set; }
    };
    public static class ConfigReader   //配置读取
    {
        //正则表达式
        private static readonly Regex HeaderRegex = new Regex(@"^\[(?<Header>.+)\]$", RegexOptions.Compiled);
        private static readonly Regex EntryRegex = new Regex(@"^(?<Key>.*?)=(?<Value>.*)$", RegexOptions.Compiled);
        private static readonly Regex ColorRegex = new Regex(@"\[A:\s?(?<A>[0-9]{1,3}),\s?R:\s?(?<R>[0-9]{1,3}),\s?G:\s?(?<G>[0-9]{1,3}),\s?B:\s?(?<B>[0-9]{1,3})\]", RegexOptions.Compiled);
        private static readonly Regex MagicRegex = new Regex(@"\[N:\s?(?<N>[A-Za-z0-9\u4e00-\u9fa5]{0,16}),\s?T:\s?(?<T>[0-9]{1,3}),\s?K:\s?(?<K>[0-9]{1,3}),\s?P:\s?(?<P>[0-9]{1,3}),\s?M:\s?(?<M>[0-9]{1,3}),\s?A:\s?(?<A>[0-9-]{1,3})\]", RegexOptions.Compiled);

        public static readonly Dictionary<Type, object> ConfigObjects = new Dictionary<Type, object>();

        private static readonly Dictionary<Type, Dictionary<string, Dictionary<string, string>>> ConfigContents = new Dictionary<Type, Dictionary<string, Dictionary<string, string>>>();

        public static void Load()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type type in types)
            {
                ConfigPath config = type.GetCustomAttribute<ConfigPath>();

                if (config == null) continue;

                object ob = null;
                if (!type.IsAbstract || !type.IsSealed)
                    ConfigObjects[type] = ob = Activator.CreateInstance(type);

                ReadConfig(type, config.Path, ob);
            }
        }
        public static void Save()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type type in types)
            {

                ConfigPath config = type.GetCustomAttribute<ConfigPath>();

                if (config == null) continue;

                object ob = null;

                if (!type.IsAbstract || !type.IsSealed)
                    ob = ConfigObjects[type];

                SaveConfig(type, config.Path, ob);
            }
        }

        public static void ReadConfig(Type type, string path, object ob)  //读取配置
        {
            if (!File.Exists(path)) return;

            PropertyInfo[] properties = type.GetProperties();

            Dictionary<string, Dictionary<string, string>> contents = ConfigContents[type] = new Dictionary<string, Dictionary<string, string>>();

            string[] lines = File.ReadAllLines(path);

            Dictionary<string, string> section = null;

            foreach (string line in lines)
            {
                Match match = HeaderRegex.Match(line);
                if (match.Success)
                {
                    section = new Dictionary<string, string>();
                    contents[match.Groups["Header"].Value] = section;
                    continue;
                }

                if (section == null) continue;

                match = EntryRegex.Match(line);

                if (!match.Success) continue;

                section[match.Groups["Key"].Value] = match.Groups["Value"].Value;
            }

            string lastSection = null;

            foreach (PropertyInfo property in properties)
            {
                ConfigSection config = property.GetCustomAttribute<ConfigSection>();

                if (config != null) lastSection = config.Section;

                if (lastSection == null) continue;

                MethodInfo method = typeof(ConfigReader).GetMethod("Read", new[] { typeof(Type), typeof(string), typeof(string), property.PropertyType });

                property.SetValue(ob, method.Invoke(null, new[] { type, lastSection, property.Name, property.GetValue(ob) }));
            }
        }
        public static void SaveConfig(Type type, string path, object ob)   //保存设置
        {
            PropertyInfo[] properties = type.GetProperties();
            Dictionary<string, Dictionary<string, string>> contents = ConfigContents[type] = new Dictionary<string, Dictionary<string, string>>();

            string lastSection = null;

            foreach (PropertyInfo property in properties)
            {
                ConfigSection config = property.GetCustomAttribute<ConfigSection>();

                if (config != null) lastSection = config.Section;

                if (lastSection == null) continue;

                MethodInfo method = typeof(ConfigReader).GetMethod("Write", new[] { typeof(Type), typeof(string), typeof(string), property.PropertyType });

                method.Invoke(ob, new[] { type, lastSection, property.Name, property.GetValue(ob) });
            }

            List<string> lines = new List<string>();

            foreach (KeyValuePair<string, Dictionary<string, string>> header in contents)
            {
                lines.Add($"[{header.Key}]");

                foreach (KeyValuePair<string, string> entries in header.Value)
                    lines.Add($"{entries.Key}={entries.Value}");

                lines.Add(string.Empty);
            }

            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            File.WriteAllLines(path, lines, Encoding.Unicode);
        }

        private static bool TryGetEntry(Type type, string section, string key, out string value)  //尝试获得条目
        {
            value = null;
            Dictionary<string, Dictionary<string, string>> contents;
            Dictionary<string, string> entries;

            if (!ConfigContents.TryGetValue(type, out contents))
                ConfigContents[type] = contents = new Dictionary<string, Dictionary<string, string>>();

            if (contents.TryGetValue(section, out entries))
                return entries.TryGetValue(key, out value);

            entries = new Dictionary<string, string>();
            contents[section] = entries;

            return false;
        }

        #region Reads读取类型
        public static Boolean Read(Type type, string section, string key, Boolean value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                Boolean result;

                if (Boolean.TryParse(entry, out result))
                    return result;
            }

            ConfigContents[type][section][key] = value.ToString();

            return value;
        }

        public static Byte Read(Type type, string section, string key, Byte value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                Byte result;

                if (Byte.TryParse(entry, out result))
                    return result;
            }

            ConfigContents[type][section][key] = value.ToString();

            return value;
        }
        public static Int16 Read(Type type, string section, string key, Int16 value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                Int16 result;

                if (Int16.TryParse(entry, out result))
                    return result;
            }

            ConfigContents[type][section][key] = value.ToString();

            return value;
        }
        public static Int32 Read(Type type, string section, string key, Int32 value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                Int32 result;

                if (Int32.TryParse(entry, out result))
                    return result;
            }

            ConfigContents[type][section][key] = value.ToString();

            return value;
        }
        public static Int64 Read(Type type, string section, string key, Int64 value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                Int64 result;

                if (Int64.TryParse(entry, out result))
                    return result;
            }

            ConfigContents[type][section][key] = value.ToString();

            return value;
        }

        public static SByte Read(Type type, string section, string key, SByte value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                SByte result;

                if (SByte.TryParse(entry, out result))
                    return result;
            }

            ConfigContents[type][section][key] = value.ToString();

            return value;
        }
        public static UInt16 Read(Type type, string section, string key, UInt16 value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                UInt16 result;

                if (UInt16.TryParse(entry, out result))
                    return result;
            }

            ConfigContents[type][section][key] = value.ToString();

            return value;
        }
        public static UInt32 Read(Type type, string section, string key, UInt32 value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                UInt32 result;

                if (UInt32.TryParse(entry, out result))
                    return result;
            }

            ConfigContents[type][section][key] = value.ToString();

            return value;
        }
        public static UInt64 Read(Type type, string section, string key, UInt64 value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                UInt64 result;

                if (UInt64.TryParse(entry, out result))
                    return result;
            }

            ConfigContents[type][section][key] = value.ToString();

            return value;
        }

        public static Single Read(Type type, string section, string key, Single value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                Single result;

                if (Single.TryParse(entry, out result))
                    return result;
            }

            ConfigContents[type][section][key] = value.ToString(CultureInfo.InvariantCulture);

            return value;
        }
        public static Double Read(Type type, string section, string key, Double value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                Double result;

                if (Double.TryParse(entry, out result))
                    return result;
            }

            ConfigContents[type][section][key] = value.ToString(CultureInfo.InvariantCulture);

            return value;
        }
        public static Decimal Read(Type type, string section, string key, Decimal value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                Decimal result;

                if (Decimal.TryParse(entry, out result))
                    return result;
            }

            ConfigContents[type][section][key] = value.ToString(CultureInfo.InvariantCulture);

            return value;
        }

        public static Char Read(Type type, string section, string key, Char value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                Char result;

                if (Char.TryParse(entry, out result))
                    return result;
            }

            ConfigContents[type][section][key] = value.ToString();

            return value;
        }
        public static String Read(Type type, string section, string key, String value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
                return entry;

            ConfigContents[type][section][key] = value;

            return value;
        }

        public static Point Read(Type type, string section, string key, Point value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                string[] data = entry.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                int x, y;

                if (data.Length == 2 && int.TryParse(data[0], out x) && int.TryParse(data[1], out y))
                    return new Point(x, y);
            }

            ConfigContents[type][section][key] = $"{value.X}, {value.Y}";

            return value;
        }
        public static Size Read(Type type, string section, string key, Size value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                string[] data = entry.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                int width, height;

                if (data.Length == 2 && int.TryParse(data[0], out width) && int.TryParse(data[1], out height))
                    return new Size(width, height);
            }

            ConfigContents[type][section][key] = $"{value.Width}, {value.Height}";

            return value;
        }
        public static SizeF Read(Type type, string section, string key, SizeF value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                string[] data = entry.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                float width, height;

                if (data.Length == 2 && float.TryParse(data[0], out width) && float.TryParse(data[1], out height))
                    return new SizeF(width, height);
            }

            ConfigContents[type][section][key] = $"{value.Width}, {value.Height}";

            return value;
        }

        public static TimeSpan Read(Type type, string section, string key, TimeSpan value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                TimeSpan result;

                if (TimeSpan.TryParse(entry, out result))
                    return result;
            }

            ConfigContents[type][section][key] = value.ToString();

            return value;
        }
        public static DateTime Read(Type type, string section, string key, DateTime value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                DateTime result;

                if (DateTime.TryParse(entry, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                    return result;
            }

            ConfigContents[type][section][key] = value.ToString(CultureInfo.InvariantCulture);

            return value;
        }

        public static Color Read(Type type, string section, string key, Color value)
        {
            string entry;


            if (TryGetEntry(type, section, key, out entry))
            {
                Match match = ColorRegex.Match(entry);
                if (match.Success)
                {
                    int a = int.Parse(match.Groups["A"].Value);
                    int r = int.Parse(match.Groups["R"].Value);
                    int g = int.Parse(match.Groups["G"].Value);
                    int b = int.Parse(match.Groups["B"].Value);

                    return Color.FromArgb(
                        Math.Min(Byte.MaxValue, Math.Max(Byte.MinValue, a)),
                        Math.Min(Byte.MaxValue, Math.Max(Byte.MinValue, r)),
                        Math.Min(Byte.MaxValue, Math.Max(Byte.MinValue, g)),
                        Math.Min(Byte.MaxValue, Math.Max(Byte.MinValue, b)));
                }
            }

            ConfigContents[type][section][key] = $"[A:{value.A}, R:{value.R}, G:{value.G}, B:{value.B}]";

            return value;
        }
        public static MagicType Read(Type type, string section, string key, MagicType value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                UInt16 result;

                if (UInt16.TryParse(entry, out result))
                    return (MagicType)result;
            }

            ConfigContents[type][section][key] = value.ToString();

            return value;
        }
        public static List<MagicHelper> Read(Type type, string section, string key, List<MagicHelper> value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                char[] separater = { '|' };
                List<MagicHelper> result = new List<MagicHelper>();
                string[] entrys = entry.Split(separater, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < entrys.Length; i++)
                {
                    MagicHelper m = new MagicHelper();
                    Match match = MagicRegex.Match(entrys[i]);
                    if (match.Success)
                    {
                        m.Name = match.Groups['N'].Value;
                        m.TypeID = (MagicType)int.Parse(match.Groups["T"].Value);
                        m.Key = (SpellKey)int.Parse(match.Groups["K"].Value);
                        m.LockPlayer = int.Parse(match.Groups["P"].Value) > 0;
                        m.LockMonster = int.Parse(match.Groups["M"].Value) > 0;
                        m.Amulet = int.Parse(match.Groups["A"].Value);
                        result.Add(m);
                    }
                }
                return result;
            }

            ConfigContents[type][section][key] = value.ToString();

            return value;
        }
        public static List<string> Read(Type type, string section, string key, List<string> value)
        {
            string entry;

            if (TryGetEntry(type, section, key, out entry))
            {
                string[] separater = { "\\r\\n" };
                List<string> result = new List<string>();
                string[] entrys = entry.Split(separater, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < entrys.Length; i++)
                {
                    string str = entrys[i].Replace("\\\\", "\\");
                    result.Add(str);
                }
                return result;
            }

            ConfigContents[type][section][key] = value.ToString();

            return value;
        }
        #endregion

        #region Writes 写入
        public static void Write(Type type, string section, string key, Boolean value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = value.ToString();
        }

        public static void Write(Type type, string section, string key, Byte value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = value.ToString();
        }
        public static void Write(Type type, string section, string key, Int16 value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = value.ToString();
        }
        public static void Write(Type type, string section, string key, Int32 value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = value.ToString();
        }
        public static void Write(Type type, string section, string key, Int64 value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = value.ToString();
        }

        public static void Write(Type type, string section, string key, SByte value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = value.ToString();
        }
        public static void Write(Type type, string section, string key, UInt16 value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = value.ToString();
        }
        public static void Write(Type type, string section, string key, UInt32 value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = value.ToString();
        }
        public static void Write(Type type, string section, string key, UInt64 value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = value.ToString();
        }

        public static void Write(Type type, string section, string key, Single value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = value.ToString(CultureInfo.InvariantCulture);
        }
        public static void Write(Type type, string section, string key, Double value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = value.ToString(CultureInfo.InvariantCulture);
        }
        public static void Write(Type type, string section, string key, Decimal value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = value.ToString(CultureInfo.InvariantCulture);
        }

        public static void Write(Type type, string section, string key, Char value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = value.ToString();
        }
        public static void Write(Type type, string section, string key, String value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = value;
        }

        public static void Write(Type type, string section, string key, Point value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = $"{value.X}, {value.Y}";
        }
        public static void Write(Type type, string section, string key, Size value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = $"{value.Width}, {value.Height}";
        }

        public static void Write(Type type, string section, string key, TimeSpan value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = value.ToString();
        }
        public static void Write(Type type, string section, string key, DateTime value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = value.ToString(CultureInfo.InvariantCulture);
        }
        public static void Write(Type type, string section, string key, Color value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = $"[A:{value.A}, R:{value.R}, G:{value.G}, B:{value.B}]";
        }
        public static void Write(Type type, string section, string key, MagicType value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            ConfigContents[type][section][key] = ((UInt16)value).ToString();
        }
        public static void Write(Type type, string section, string key, List<MagicHelper> value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            string result = null;
            for (int i = 0; i < value.Count; i++)
            {
                if (result != null) result += "|";
                else { result = ""; }
                result += $"[N:{value[i].Name},T:{(int)(value[i].TypeID)},K:{(int)(value[i].Key)},P:{(value[i].LockPlayer ? 1 : 0)},M:{(value[i].LockMonster ? 1 : 0)},A:{value[i].Amulet}]";
            }
            ConfigContents[type][section][key] = result;
        }
        public static void Write(Type type, string section, string key, List<string> value)
        {
            if (!ConfigContents[type].ContainsKey(section)) ConfigContents[type][section] = new Dictionary<string, string>();

            string result = null;
            for (int i = 0; i < value.Count; i++)
            {
                if (value[i] == null) continue;
                if (value[i].Length == 0) continue;
                if (result != null) result += "\\r\\n";
                else { result = ""; }
                string str = value[i].Replace("\\", "\\\\");
                result += str;
            }
            if (result == null) result = "";
            ConfigContents[type][section][key] = result;
        }
        #endregion
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigPath : Attribute   //配置路径属性
    {
        public string Path { get; set; }

        public ConfigPath(string path)
        {
#if Mobile
            Path = System.IO.Path.Combine(CEnvir.MobileClientPath, path);
#else
            Path = path;
#endif

        }
    }

    public class ConfigNetwork : Attribute  //配置网路属性
    {
        public string Network { get; set; }

        public ConfigNetwork(string network)
        {
            Network = network;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigSection : Attribute   //配置部分属性
    {
        public string Section { get; set; }

        public ConfigSection(string section)
        {
            Section = section;
        }
    }
}
