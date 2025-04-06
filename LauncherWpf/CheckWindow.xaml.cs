using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Launcher
{
    /// <summary>
    /// CheckWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CheckWindow : Window
    {
        public CheckWindow()
        {
            InitializeComponent();
        }



        #region 检测组件是否已经安装


        public void Check(Action callback)
        {
            checkFrame.Visibility = Visibility.Visible;
            int delay = 500;
            Task.Run(async () =>
            {
                bool osOK = false, dotNetOK = false, dxOK = false;
                await Task.Delay(delay);
                SetText(osLabel, "开始检测...");
                await Task.Delay(delay);
                SetText(osLabel, "检测中...");
                if (ComponentUtils.DotNetUtil.IsCompatible())
                {
                    SetText(osLabel, "OK");
                    osOK = true;
                }
                else
                {
                    SetText(osLabel, "未通过");
                }

                await Task.Delay(delay);
                SetText(dxLabel, "开始检测...");
                await Task.Delay(delay);
                SetText(dxLabel, "检测中...");
                if (ComponentUtils.DXUtil.IsCompatible())
                {
                    SetText(dxLabel, "OK");
                    dxOK = true;
                }
                else
                {
                    SetText(dxLabel, "未通过");
                }

                await Task.Delay(delay);
                SetText(netLabel, "开始检测...");
                await Task.Delay(delay);
                SetText(netLabel, "检测中...");
                if (ComponentUtils.DotNetUtil.IsCompatible())
                {
                    SetText(netLabel, "OK");
                    dotNetOK = true;
                }
                else
                {
                    SetText(netLabel, "未通过");
                }

                if (osOK && dotNetOK && dxOK)
                {
                    await Task.Delay(delay);
                    Dispatcher.Invoke(() =>
                    {
                        checkFrame.Visibility = Visibility.Hidden;
                    });
                }

                if (callback != null)
                {
                    Dispatcher.Invoke(callback);
                }
            });

        }

        private void SetText(Label label, string value)
        {
            Dispatcher.Invoke(() =>
            {
                label.Content = value;
            });
        }

        #endregion
    }
}
