using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using Library;
using Server.Envir;
using System;
using System.Runtime;
using System.Windows.Forms;

namespace Server
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ConfigReader.Load();  //配置读取

            try
            {
                Init();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private static void Init()
        {
            Config.LoadVersion();  //读取版本
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;  //延迟模式持续低更新

            BonusSkins.Register();   //服务端界面皮肤登记
            SkinManager.EnableFormSkins();  //启用视窗效果
            UserLookAndFeel.Default.SetSkinStyle("DevExpress Style");  //用户外观设置

            Application.Run(new SMain());  //应用程序 运行
            ConfigReader.Save();  //配置保存
        }
    }
}
