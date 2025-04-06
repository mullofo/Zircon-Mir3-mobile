using Client.Controls;
using Client.Envir;
using Client.Scenes;
using Library;
using Sentry;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主要入口点
        /// </summary>
        [STAThread]
        static void Main()
        {
            //客户端多开限制
            Mutex mtx = null, mtx1 = null;
            if (Mutex.TryOpenExisting(@"heilong_main1", out mtx))
            {
                mtx.Close();
                if (Mutex.TryOpenExisting(@"heilong_main2", out mtx1))
                {
                    mtx1.Close();
                    Environment.Exit(0);
                }
                else
                    mtx1 = new Mutex(false, @"heilong_main2");
            }
            else

                mtx = new Mutex(false, @"heilong_main1");


            var x = SystemInformation.MouseWheelScrollDelta;
            Application.EnableVisualStyles();  //应用程序 启用视觉样式
            Application.SetCompatibleTextRenderingDefault(false); //应用程序 设置兼容文本呈现默认值

            ConfigReader.Load();  //读取所有配置并加载

            string sPath = @".\Data\Saved";  //判断Saved文件夹不存在 自动创建
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }

            //如果启用了Sentry
            if (Config.SentryEnabled)
            {
                //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

                using (SentrySdk.Init(o =>
                {
                    o.Dsn = Config.SentryClientDSN2;
                    o.SampleRate = 0.1f;
                    o.AddInAppInclude("Library.");
                    o.AttachStacktrace = true;
                    o.SendDefaultPii = true;
                }))
                {
                    Init();
                }
            }
            else
            {
                // 不开启Sentry 使用try-catch捕获异常
                try
                {
                    Init();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private static void Init()
        {
            if (!Directory.Exists(@".\ChatLogs\")) Directory.CreateDirectory(@".\ChatLogs\"); //创建 聊天记录
            if (!Directory.Exists(@".\SysLogs\")) Directory.CreateDirectory(@".\SysLogs\");  //创建 系统日志

            foreach (KeyValuePair<LibraryFile, string> pair in Libraries.LibraryList)  //遍历 LibraryFile
            {
                if (!File.Exists(@".\" + pair.Value)) continue;

                CEnvir.LibraryList[pair.Key] = new MirLibrary(@".\" + pair.Value);
            }

            //等收到服务端数据库信息时再加载
            //CEnvir.LoadDatabase(); //加载数据库

            CEnvir.Target = new TargetForm();  //新目标表单
            DXManager.Create();  //DX管理器创建
            DXSoundManager.Create();  //DX声效创建

            DXControl.ActiveScene = new StartScene(Config.IntroSceneSize);  //控制活动场景 = 新的登录场景大小 （登录界面显示）

            RenderLoop.Run(CEnvir.Target, CEnvir.GameLoop);  //运行 游戏循环

            DXControl.ActiveScene?.Dispose(); //不确定是否应该调用一下

            ConfigReader.Save();  //读取配置并 保存
            CEnvir.Session?.Save(true, MirDB.SessionMode.Client);
            CEnvir.Unload();
            DXManager.Unload();
            DXSoundManager.Unload();
        }
    }
}
