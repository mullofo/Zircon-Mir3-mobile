using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Launcher
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static NotifyIcon TrayIcon { get; set; }

        public static MainWindow CurrentWindow { get; set; }
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            RemoveTrayIcon();
            AddTrayIcon();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            if (IsRunning())
            {
                MessageBox.Show("程序已经打开！");
                Shutdown();
                return;
            }

            base.OnStartup(e);
        }
        private void AddTrayIcon()
        {
            if (TrayIcon != null)
            {
                return;
            }

            TrayIcon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location),
                Text = "传奇3"
            };
            TrayIcon.Visible = true;
            TrayIcon.Click += (s, e) =>
            {
                CurrentWindow._webWindow.Visibility = Visibility.Visible;
                CurrentWindow.Visibility = Visibility.Visible;
            };

            ContextMenu menu = new ContextMenu();

            MenuItem closeItem = new MenuItem();
            closeItem.Text = "退出";
            closeItem.Click += new EventHandler(delegate { this.Shutdown(); });
            menu.MenuItems.Add(closeItem);

            TrayIcon.ContextMenu = menu;
        }

        private void RemoveTrayIcon()
        {
            if (TrayIcon != null)
            {
                TrayIcon.Visible = false;
                TrayIcon.Dispose();
                TrayIcon = null;
            }
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            RemoveTrayIcon();
        }


        public static bool IsRunning()
        {
            Process current = default(Process);
            current = System.Diagnostics.Process.GetCurrentProcess();
            Process[] processes = null;
            processes = System.Diagnostics.Process.GetProcessesByName(current.ProcessName);

            Process process = default(Process);

            foreach (Process tempLoopVar_process in processes)
            {
                process = tempLoopVar_process;

                if (process.Id != current.Id)
                {
                    if (System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        return true;

                    }

                }
            }
            return false;

        }
    }
}
