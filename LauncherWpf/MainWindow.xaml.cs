using IWshRuntimeLibrary;
using Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string PatcherFileName = @".\Patcher.exe";

        public const string PListFileName = "PList.Bin";
        public const string ClientPath = ".\\";

        public DateTime LastSpeedCheck;
        public long TotalDownload, TotalProgress, CurrentProgress, LastDownloadProcess;
        public bool NeedUpdate;

        public static bool HasError;
        private string Host;

        public const string DXInstallerPath = @".\游戏运行必须软件包\DirectX Redist\DXSETUP.exe";
        public const string DotNetInstallerPath = @".\游戏运行必须软件包\Microsoft .NET Framework 4.8_4.8.3928.0.exe";

        public WebWindow _webWindow = new WebWindow();
        CheckWindow _checkWindow = new CheckWindow();

        ClientStyle _clientStyle;
        public MainWindow()
        {
            ConfigReader.Load();                                            //配置读取器.加载
            InitializeComponent();
            Init();
            App.CurrentWindow = this;
            LocationChanged += MainWindow_LocationChanged;
            border1.Visibility = System.Windows.Visibility.Hidden;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitialWebWindow();
            MoveWebWindow();
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            MoveWebWindow();
        }

        private void InitialWebWindow()
        {
            _webWindow.WindowStyle = System.Windows.WindowStyle.None;
            _webWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            _webWindow.ResizeMode = System.Windows.ResizeMode.NoResize;
            _webWindow.ShowInTaskbar = false;
            //_webWindow.webBrowser1.Navigate(new Uri("http://www.mir3.red"));

            _webWindow.Owner = this;
            _webWindow.Show();


            _checkWindow.WindowStyle = System.Windows.WindowStyle.None;
            _checkWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            _checkWindow.ResizeMode = System.Windows.ResizeMode.NoResize;
            _checkWindow.ShowInTaskbar = false;
            _checkWindow.Owner = _webWindow;
            _checkWindow.Show();
            _checkWindow.Visibility = Visibility.Hidden;
        }

        private void MoveWebWindow()
        {
            _webWindow.Left = Left + border1.Margin.Left;
            _webWindow.Top = Top + border1.Margin.Top + 39;
            _webWindow.Width = border1.Width;
            _webWindow.Height = border1.Height;


            _checkWindow.Left = Left + border1.Margin.Left;
            _checkWindow.Top = Top + border1.Margin.Top + 39;
            _checkWindow.Width = border1.Width;
            _checkWindow.Height = border1.Height;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
            _webWindow.Visibility = Visibility.Hidden;
            _checkWindow.Visibility = Visibility.Hidden;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ConfigReader.Save();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            RegisterEventAsync();
        }
        public bool CreateDesktopShortcut(string FileName, string ExePath)
        {
            try
            {
                string deskTop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";
                if (System.IO.File.Exists(deskTop + FileName + ".lnk"))  //
                {
                    System.IO.File.Delete(deskTop + FileName + ".lnk");//删除原来的桌面快捷键方式
                }
                WshShell shell = new WshShell();

                //快捷键方式创建的位置、名称
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(deskTop + FileName + ".lnk");
                shortcut.TargetPath = ExePath; //目标文件
                                               //该属性指定应用程序的工作目录，当用户没有指定一个具体的目录时，快捷方式的目标应用程序将使用该属性所指定的目录来装载或保存文件。
                shortcut.WorkingDirectory = System.Environment.CurrentDirectory;
                shortcut.WindowStyle = 1; //目标应用程序的窗口状态分为普通、最大化、最小化【1,3,7】
                shortcut.Description = FileName; //描述
                //var iconPath = ExePath.Substring(0, ExePath.LastIndexOf("\\")) + "\\Mir3.ico";
                //if (System.IO.File.Exists(iconPath))
                //{
                //    shortcut.IconLocation = iconPath;
                //    //Icon = new BitmapImage(new Uri(iconPath));
                //}
                shortcut.Arguments = "";
                //shortcut.Hotkey = "CTRL+ALT+F11"; // 快捷键
                shortcut.Save(); //必须调用保存快捷才成创建成功

                return true;
            }
            catch (Exception)

            {
                return false;
            }
        }
        private void Init()
        {
            string exePath = this.GetType().Assembly.Location;//获得自身程序的完整路径包括文件名
            //string str = Application.ExecutablePath;另外一种获取方式
            FileStream fileStream = new FileStream(exePath, FileMode.Open, FileAccess.Read);//打开文件
            fileStream.Seek(-4, SeekOrigin.End);//读出末尾的字节数 自定义为校验位
            byte[] bytes = new byte[4];
            fileStream.Read(bytes, 0, 4);
            int verifi = System.BitConverter.ToInt32(bytes, 0);
            if (verifi == 0x1267101A)
            {
                fileStream.Seek(-8, SeekOrigin.End);//读取自定义数据开始的位置的信息
                fileStream.Read(bytes, 0, 4);
                UInt32 offset = BitConverter.ToUInt32(bytes, 0);
                fileStream.Seek(offset, SeekOrigin.Begin);//来到偏移

                var title = ReadNextStream(fileStream);

                var clientStyle = ReadNextStream(fileStream);
                if (!Enum.TryParse(clientStyle, out _clientStyle))
                {
                    _clientStyle = ClientStyle.All;
                }
                if (_clientStyle != ClientStyle.All)
                {
                    labelGameType.Visibility = Visibility.Hidden;
                    dropGameType.Visibility = Visibility.Hidden;
                }
                else
                {
                    labelGameType.Visibility = Visibility.Visible;
                    dropGameType.Visibility = Visibility.Visible;
                }

                var updateUrl = ReadNextStream(fileStream);

                var webAdress = ReadNextStream(fileStream);

                var explainUrl = ReadNextStream(fileStream);

                var explainText = ReadNextStream(fileStream);

                var imageBytes = ReadNextStreamBytes(fileStream);

                if (string.IsNullOrEmpty(webAdress))
                {
                    if (imageBytes != null)
                        imgMain.Source = GetBitmapImage(imageBytes);
                }
                else
                {
                    _webWindow.AllowsTransparency = false;
                    _webWindow.webBrowser1.Navigate(new System.Uri(webAdress));
                }

                txtTitle.Content = title;
                Title = title;
                App.TrayIcon.Text = title;
                linkUpdateLog.NavigateUri = new Uri(explainUrl);
                linkUpdateText.Text = explainText;
                Host = updateUrl;

            }
            else
            {
                Host = Config.Host;
                _webWindow.webBrowser1.Navigate(new System.Uri("http://www.lomcn.cn"));
            }

            fileStream.Close();
            fileStream.Dispose();
            CheckPatch(false);
            CreateDesktopShortcut(Title, this.GetType().Assembly.Location);  //这里需要改成对接登录器设置名   
        }


        private string ReadNextStream(FileStream fileStream)
        {
            byte[] bytes = new byte[4];
            fileStream.Read(bytes, 0, 4);//读出4个字节
            var dataLength = BitConverter.ToInt32(bytes, 0);
            if (dataLength > 0)
            {
                byte[] datas = new byte[dataLength + 1];
                fileStream.Read(datas, 0, dataLength);
                var str = System.Text.Encoding.UTF8.GetString(datas).Replace('\0', ' ').Trim();//读出数据并转换为字符串
                return str;
            }
            return string.Empty;

        }

        private byte[] ReadNextStreamBytes(FileStream fileStream)
        {
            byte[] bytes = new byte[4];
            fileStream.Read(bytes, 0, 4);//读出4个字节
            var dataLength = BitConverter.ToInt32(bytes, 0);
            if (dataLength > 0)
            {
                byte[] datas = new byte[dataLength + 1];
                fileStream.Read(datas, 0, dataLength);
                return datas;
            }
            return null;
        }

        #region 更新素材
        private async void CheckPatch(bool repair)
        {
            HasError = false;
            btnRepair.IsEnabled = false;
            btnStart.IsEnabled = false;
            TotalDownload = 0;
            TotalProgress = 0;
            CurrentProgress = 0;
            totalProgress.Width = 0;
            currentProgress.Width = 0;
            LastSpeedCheck = Time.Now;
            NeedUpdate = false;

            Progress<string> progress = new Progress<string>(s => loadText.Content = s);

            List<PatchInformation> liveVersion = await Task.Run(() => GetPatchInformation(progress));

            if (liveVersion == null)
            {
                loadText.Content = "无法连接到更新服务.";
                btnRepair.IsEnabled = true;
                btnStart.IsEnabled = true;
                return;
            }

            List<PatchInformation> currentVersion = repair ? null : await Task.Run(() => LoadVersion(progress));
            List<PatchInformation> patch = await Task.Run(() => CalculatePatch(liveVersion, currentVersion, progress));

            loadText.Content = "开始更新";
            CreateSizeLabel();

            Task task = Task.Run(() => DownloadPatch(patch, progress));

            while (!task.IsCompleted)
            {
                CreateSizeLabel();

                await Task.Delay(100);
            }

            CreateSizeLabel();

            SaveVersion(liveVersion);

            loadText.Content = "完成";
            //speedText.Content = "";
            downloadText.Content = "";

            if (Directory.Exists(ClientPath + "Patch\\"))
                Directory.Delete(ClientPath + "Patch\\", true);

            if (NeedUpdate)   //需要更新
            {
                //文件.写入所有字节(程序.修补程序文件名，属性.资源.修补器）；
                //File.WriteAllBytes(Program.PatcherFileName, Properties.Resources.Patcher);
                //进程.开始(程序.修补程序文件名，$“\”{ 应用程序.可执行路径}.tmp\“\”{ 应用程序.可执行路径}\"");
                var path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, AppDomain.CurrentDomain.SetupInformation.ApplicationName);
                Process.Start(PatcherFileName, $"\"{path}.tmp\" \"{path}\"");
                Environment.Exit(0);  //环境.退出
            }

            /*try
            {
                if (File.Exists(Program.PatcherFileName))
                    File.Delete(Program.PatcherFileName);
            }
            catch (Exception) {}*/

            btnRepair.IsEnabled = true;
            btnStart.IsEnabled = true;
        }
        private void CreateSizeLabel()
        {
            const decimal KB = 1024;
            const decimal MB = KB * 1024;
            const decimal GB = MB * 1024;

            long progress = TotalProgress + CurrentProgress;

            StringBuilder text = new StringBuilder();

            if (progress > GB)
                text.Append($"{progress / GB:#,##0.0}GB");
            else if (progress > MB)
                text.Append($"{progress / MB:#,##0.0}MB");
            else if (progress > KB)
                text.Append($"{progress / KB:#,##0}KB");
            else
                text.Append($"{progress:#,##0}B");

            if (TotalDownload > GB)
                text.Append($" / {TotalDownload / GB:#,##0.0}GB");
            else if (TotalDownload > MB)
                text.Append($" / {TotalDownload / MB:#,##0.0}MB");
            else if (TotalDownload > KB)
                text.Append($" / {TotalDownload / KB:#,##0}KB");
            else
                text.Append($" / {TotalDownload:#,##0}B");

            var downloadSizeStr = text.ToString();

            if (TotalDownload > 0)
                totalProgress.Width = progress * 1F / TotalDownload * totalProgress.MaxWidth;

            long speed = (progress - LastDownloadProcess) * TimeSpan.TicksPerSecond / (Time.Now.Ticks - LastSpeedCheck.Ticks); //May cause errors?
            LastDownloadProcess = progress;

            /*if (speed > GB)
            {
                speedText.Content = $"{speed / GB:#,##0.0} GB/S";

            }
            else if (speed > MB)
            {
                speedText.Content = $"{speed / MB:#,##0.0} MB/s";
            }
            else if (speed > KB)
            {
                speedText.Content = $"{speed / KB:#,##0} KB/s";
            }
            else
            {
                speedText.Content = $"{speed:#,##0} Byte/s";

            }*/
            downloadText.Content = downloadSizeStr;
            LastSpeedCheck = Time.Now;
        }

        private List<PatchInformation> LoadVersion(IProgress<string> progress)
        {
            List<PatchInformation> list = new List<PatchInformation>();
            try
            {
                if (System.IO.File.Exists(ClientPath + "Version.bin"))
                {
                    using (MemoryStream mStream = new MemoryStream(System.IO.File.ReadAllBytes(ClientPath + "Version.bin")))
                    using (BinaryReader reader = new BinaryReader(mStream))
                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                            list.Add(new PatchInformation(reader));

                    progress.Report("正在计算补丁.");
                    return list;
                }
                progress.Report("缺少版本信息, 正在运行修复");
            }
            catch (Exception ex)
            {
                progress.Report(ex.Message);
            }

            return null;
        }
        private List<PatchInformation> GetPatchInformation(IProgress<string> progress)
        {
            try
            {
                progress.Report("下载补丁信息");

                using (WebClient client = new WebClient())
                {
                    if (Config.UseLogin)
                        client.Credentials = new NetworkCredential(Config.Username, Config.Password);
                    using (MemoryStream mStream = new MemoryStream(client.DownloadData(Path.Combine(Host, PListFileName))))
                    using (BinaryReader reader = new BinaryReader(mStream))
                    {
                        List<PatchInformation> list = new List<PatchInformation>();

                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                            list.Add(new PatchInformation(reader));

                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                progress.Report(ex.Message);
            }

            return null;
        }
        private List<PatchInformation> CalculatePatch(IReadOnlyList<PatchInformation> list, List<PatchInformation> current, IProgress<string> progress)
        {
            List<PatchInformation> patch = new List<PatchInformation>();

            if (list == null) return patch;

            for (int i = 0; i < list.Count; i++)
            {
                progress.Report($"检查文件: {i + 1} / {list.Count}");

                PatchInformation file = list[i];
                if (current != null && current.Any(x => x.FileName == file.FileName && IsMatch(x.CheckSum, file.CheckSum))) continue;

                if (System.IO.File.Exists(ClientPath + file.FileName))
                {
                    byte[] CheckSum;
                    using (MD5 md5 = MD5.Create())
                    {
                        using (FileStream stream = System.IO.File.OpenRead(ClientPath + file.FileName))
                            CheckSum = md5.ComputeHash(stream);
                    }

                    if (IsMatch(CheckSum, file.CheckSum))
                        continue;
                }
                patch.Add(file);
                TotalDownload += file.CompressedLength;
            }

            return patch;
        }
        public bool IsMatch(byte[] a, byte[] b, long offSet = 0)
        {
            if (b == null || a == null || b.Length + offSet > a.Length || offSet < 0) return false;

            for (int i = 0; i < b.Length; i++)
                if (a[offSet + i] != b[i])
                    return false;

            return true;
        }

        private void SaveVersion(List<PatchInformation> version)
        {
            using (FileStream fStream = System.IO.File.Create(ClientPath + "Version.bin"))
            using (BinaryWriter writer = new BinaryWriter(fStream))
            {
                foreach (PatchInformation info in version)
                    info.Save(writer);
            }
        }
        private void DownloadPatch(List<PatchInformation> patch, IProgress<string> progress)
        {
            List<Task> tasks = new List<Task>();

            foreach (PatchInformation file in patch)
            {
                if (!Download(file)) continue;

                tasks.Add(Task.Run(() => Extract(file)));
            }

            if (tasks.Count == 0) return;

            progress.Report("已下载, 正在提取.");

            Task.WaitAll(tasks.ToArray());
        }

        private bool Download(PatchInformation file)
        {
            var fileName = file.FileName.Replace("\\", "-");
            string webFileName = fileName + ".gz";

            try
            {
                using (WebClient client = new WebClient())
                {
                    if (Config.UseLogin) client.Credentials = new NetworkCredential(Config.Username, Config.Password);

                    bool downloading = true;
                    int downloads = 0;
                    client.DownloadProgressChanged += (o, e) =>
                    {
                        CurrentProgress = e.BytesReceived;
                        Dispatcher.Invoke(() =>
                        {
                            downloads++;
                            loadText.Content = $"下载[{fileName}]文件中{new string('.', downloads * 2)}";
                            if (downloads % 3 == 0)
                            {
                                downloads = 0;
                            }
                            currentProgress.Width = (e.ProgressPercentage / 100f) * currentProgress.MaxWidth;
                        });
                    };
                    client.DownloadFileCompleted += (o, e) => downloading = false;

                    if (!Directory.Exists(ClientPath + "Patch\\"))
                        Directory.CreateDirectory(ClientPath + "Patch\\");
                    client.DownloadFileAsync(new Uri(Path.Combine(Host, webFileName)), $"{ClientPath}Patch\\{webFileName}");

                    while (downloading)
                        Thread.Sleep(1);

                    CurrentProgress = 0;
                    Dispatcher.Invoke(() =>
                    {
                        currentProgress.Width = 0;
                    });
                    TotalProgress += file.CompressedLength;
                }

                return true;
            }
            catch (Exception)
            {
                file.CheckSum = new byte[8];
            }

            return false;
        }
        private void Extract(PatchInformation file)
        {
            var fileName = file.FileName.Replace("\\", "-");
            Dispatcher.Invoke(() =>
            {
                loadText.Content = $"解压[{fileName}]文件...";
                Thread.Sleep(100);
            });
            string webFileName = fileName + ".gz";

            try
            {
                string toPath = ClientPath + file.FileName;

                if (AppDomain.CurrentDomain.SetupInformation.ApplicationName.EndsWith(file.FileName, StringComparison.OrdinalIgnoreCase))
                {
                    toPath += ".tmp";
                    NeedUpdate = true;
                }


                if (System.IO.File.Exists(toPath)) System.IO.File.Delete(toPath);

                Decompress($"{ClientPath}Patch\\{webFileName}", toPath);

            }
            catch (UnauthorizedAccessException ex)
            {
                file.CheckSum = new byte[8];

                if (HasError) return;
                HasError = true;
                MessageBox.Show(ex.Message + "\n\n文件可能正在使用中, 请确保游戏已关闭.", "文件错误", MessageBoxButton.OK);
            }
            catch
            {
                file.CheckSum = new byte[8];
            }
        }
        private static void Decompress(string sourceFile, string destFile)
        {
            if (!Directory.Exists(Path.GetDirectoryName(destFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(destFile));

            using (FileStream tofile = System.IO.File.Create(destFile))
            using (FileStream fromfile = System.IO.File.OpenRead(sourceFile))
            using (GZipStream gStream = new GZipStream(fromfile, CompressionMode.Decompress))
            {
                gStream.CopyTo(tofile);
            }
            System.IO.File.Delete(sourceFile);
        }
        #endregion

        public static BitmapImage GetBitmapImage(byte[] imageBytes)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(imageBytes);
            bitmapImage.EndInit();
            return bitmapImage;
        }

        private void RegisterEventAsync()
        {
            mainBody.MouseDown += MainBody_MouseDown;
            btnMin.MouseUp += BtnMin_MouseUp;
            btnClose.MouseUp += BtnClose_MouseUp;
            linkUpdateLog.Click += LinkUpdateLog_Click;
            btnRepair.Click += (s, e) => CheckPatch(true);
            btnStart.MouseUp += BtnStart_MouseUp;
            ComponentDetection.Click += ComponentDetection_Click; ;
        }

        private void ComponentDetection_Click(object sender, RoutedEventArgs e)
        {
            buzy.Visibility = Visibility.Visible;
            _checkWindow.Visibility = Visibility.Visible;
            _checkWindow.Check(() => buzy.Visibility = Visibility.Hidden);

        }

        private void BtnStart_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Visibility = Visibility.Hidden;
                _webWindow.Visibility = Visibility.Hidden;
                _checkWindow.Visibility = Visibility.Hidden;
                if (_clientStyle == ClientStyle.All)
                {
                    Process.Start(ClientPath + (dropGameType.SelectedIndex == 0 ? "Mir3Game.exe" : "Mir3.exe"));
                }
                else
                {
                    Process.Start(ClientPath + (_clientStyle == ClientStyle.Korea ? "Mir3Game.exe" : "Mir3.exe"));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LinkUpdateLog_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkUpdateLog.NavigateUri.AbsoluteUri);
        }

        private void BtnClose_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void BtnMin_MouseUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MainBody_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
