using Library;
using System;
using System.Reflection;

namespace Client.Scenes.Configs
{
    public static class CConfigReader
    {
        public static void Load(string characterName, int charaterIndex)
        {
            Type[] types = typeof(CConfigReader).Assembly.GetTypes();

            foreach (Type type in types)
            {
                CConfigPath config = type.GetCustomAttribute<CConfigPath>();

                if (config == null) continue;

                object ob = null;
                if (!type.IsAbstract || !type.IsSealed)
                    ConfigReader.ConfigObjects[type] = ob = Activator.CreateInstance(type);

                ConfigReader.ReadConfig(type, string.Format(config.Path, characterName, charaterIndex), ob);
            }
        }
        public static void Save(string characterName, int charaterIndex)
        {
            Type[] types = typeof(CConfigReader).Assembly.GetTypes();

            foreach (Type type in types)
            {

                CConfigPath config = type.GetCustomAttribute<CConfigPath>();

                if (config == null) continue;

                object ob = null;

                if (!type.IsAbstract || !type.IsSealed)
                    ob = ConfigReader.ConfigObjects[type];

                ConfigReader.SaveConfig(type, string.Format(config.Path, characterName, charaterIndex), ob);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CConfigPath : Attribute   //配置路径属性
    {
        public string Path { get; set; }

        public CConfigPath(string path)
        {
            Path = path;
        }
    }
}
