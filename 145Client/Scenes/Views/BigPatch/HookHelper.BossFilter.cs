using Library;
using Library.SystemModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Client.Scenes.Views
{
    public partial class HookHelper
    {
        private static string bossFilterFile = $@".\Data\Saved\BossFilter.cfg";
        public static List<string> BossFilterLines;
        public static void LoadBossFilterConfig()
        {
            if (!Directory.Exists(Path.GetDirectoryName(bossFilterFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(bossFilterFile));

            //如果不存在过滤文件，创建文件
            if (!File.Exists(bossFilterFile))
                CreateBossFilterFile();
            else
            {
                //读取文件，如果文件内容为空，重新创建文件
                BossFilterLines = File.ReadAllLines(bossFilterFile).ToList();
                if (BossFilterLines == null || BossFilterLines.Count == 0)
                    CreateBossFilterFile();
            }
        }

        public static void BossFilterInitialize()
        {
            GameScene.Game.BigPatchBox.MonBoss.BossFilter.FileName = bossFilterFile;
            GameScene.Game.BigPatchBox.MonBoss.Initialize();
        }


        /// <summary>
        /// 创建boss过滤文件
        /// </summary>
        /// <param name="file"></param>
        public static void CreateBossFilterFile()
        {
            BossFilterLines = new List<string>();
            foreach (MonsterInfo info in Globals.MonsterInfoList.Binding)
            {
                string name = info.MonsterName.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                if (info.IsBoss && !BossFilterLines.Exists(d => d == name))
                {
                    BossFilterLines.Add($"{name}, True,");
                }
            }

            if (BossFilterLines.Count == 0) return;

            File.WriteAllLines(bossFilterFile, BossFilterLines);

        }
    }
}