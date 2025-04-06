using Library;
using System;
using System.Windows.Forms;

namespace PatchManager
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主要入口点
        /// </summary>
        [STAThread]
        static void Main()
        {
            ConfigReader.Load();                                          //配置读取器.加载

            Application.EnableVisualStyles();                             //应用程序.启用视觉样式
            Application.SetCompatibleTextRenderingDefault(false);         //应用程序.设置兼容文本呈现默认值
            Application.Run(new PMain());                                 //运行

            ConfigReader.Save();                                          //配置读取器.保存
        }
    }
}
