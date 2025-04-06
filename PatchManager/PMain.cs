using AndroidXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace PatchManager
{
    /// <summary>
    /// 主要信息
    /// </summary>
    public partial class PMain : Form
    {
        /// <summary>
        /// 列表文件名
        /// </summary>
        public const string PListFileName = "PList.Bin";
        private const string APKListFileName = "APKVersion.bin";
        /// <summary>
        /// 客户端补丁路径
        /// </summary>
        public const string ClientPath = ".\\";
        /// <summary>
        /// 客户端文件名
        /// </summary>
        public const string ClientFileName = "Mir3Game.exe";
        /// <summary>
        /// 最后一次检查速度
        /// </summary>
        public DateTime LastSpeedCheck;
        /// <summary>
        /// 总上传量
        /// </summary>
        public long TotalUpload;
        /// <summary>
        /// 总进度
        /// </summary>
        public long TotalProgress;
        /// <summary>
        /// 当前进度
        /// </summary>
        public long CurrentProgress;
        /// <summary>
        /// 最终上传过程
        /// </summary>
        public long LastUploadProcess;
        /// <summary>
        /// 是否错误
        /// </summary>
        public bool Error;

        private bool Mobile;
        private PatchInformation ApkPatchInfo;
        private PatchInformation BaseZipPatchInfo;
        private string CurrentAPKVersion;
        private string LiveAPKVersion;

        private bool IsMobPath;
        public string CleanClientPath => IsMobPath ? Config.MobCleanClientPath : Config.PcCleanClientPath;
        public string FtpHost => IsMobPath ? Config.MobFtpHost : Config.PcFtpHost;
        public bool FtpUseLogin => IsMobPath ? Config.MobFtpUseLogin : Config.PcFtpUseLogin;
        public string Username => IsMobPath ? Config.MobUsername : Config.PcUsername;
        public string Password => IsMobPath ? Config.MobPassword : Config.PcPassword;

        public PMain()
        {
            //服务管理器.安全协议
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                                   SecurityProtocolType.Tls11 |
                                                   SecurityProtocolType.Tls12;
            InitializeComponent();
        }
        /// <summary>
        /// 上传端游补丁按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pcUploadPatchButton_Click(object sender, EventArgs e)
        {
            IsMobPath = false;
            Error = false;
            CreatePatch();
        }

        /// <summary>
        /// 上传手游补丁按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mobUploadPatchButton_Click(object sender, EventArgs e)
        {
            IsMobPath = true;
            Error = false;
            if (Mobile)
            {
                if (ApkPatchInfo != null && CurrentAPKVersion != null)
                {
                    if (BaseZipPatchInfo != null)
                    {
                        CreatePatch();
                    }
                    else
                        MessageBox.Show("未选择BaseZip基础包！！！");
                }
                else
                    MessageBox.Show("未选择APK文件或APK文件版本验证错误！！！");

            }
            else
                CreatePatch();
        }

        /// <summary>
        /// 创建补丁
        /// </summary>
        private async void CreatePatch()
        {
            InterfaceLock(false);

            Progress<string> progress = new Progress<string>(s => StatusLabel.Text = s);

            List<PatchInformation> currentVersion = await Task.Run(() => CreateVersion(progress));
            List<PatchInformation> liveVersion = await Task.Run(() => GetPatchInformation(progress));

            List<PatchInformation> patch = await Task.Run(() => CalculatePatch(currentVersion, liveVersion, progress));

            Task task = Task.Run(() => UploadPatch(patch, progress));

            while (!task.IsCompleted)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                CreateSizeLabel();
            }

            if (Directory.Exists(ClientPath + "Patch\\"))
                Directory.Delete(ClientPath + "Patch\\", true);

            if (!Error)
                SaveVersion(currentVersion);

            //是否上传APK文件
            if (IsMobPath && Mobile)
            {
                if (BaseZipPatchInfo != null && CurrentAPKVersion != null)
                {
                    if (BaseZipPatchInfo != null)
                    {
                        currentVersion = new List<PatchInformation> { ApkPatchInfo, BaseZipPatchInfo };

                        liveVersion = await Task.Run(() => GetPatchInformation(progress, isAPKinfo: true));

                        patch = await Task.Run(() => CalculatePatch(currentVersion, liveVersion, progress, isAPKinfo: true));

                        task = Task.Run(() => UploadPatch(patch, progress, isAPKinfo: true));

                        while (!task.IsCompleted)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                            CreateSizeLabel();
                        }

                        if (!Error)
                            SaveVersion(currentVersion, true);
                    }
                    else
                        MessageBox.Show("未选择BaseZip基础包！！！");
                }
                else
                    MessageBox.Show("未选择APK文件或APK文件版本验证错误！！！");

            }

            InterfaceLock(true);

            StatusLabel.Text = "完成.";
            UploadSizeLabel.Text = "完成.";
            UploadSpeedLabel.Text = "完成.";
        }

        /// <summary>
        /// 接口锁
        /// </summary>
        /// <param name="enabled"></param>
        private void InterfaceLock(bool enabled)
        {
            pcClientPathTextBox.Enabled = enabled;
            pcFtpHostTextBox.Enabled = enabled;
            pcFtpUseLoginCheckBox.Enabled = enabled;
            pcFtpAccountTextBox.Enabled = enabled;
            pcFtpPassTextBox.Enabled = enabled;
            pcUploadPatchButton.Enabled = enabled;

            mobClientPathTextBox.Enabled = enabled;
            mobFtpHostTextBox.Enabled = enabled;
            mobFtpUseLoginCheckBox.Enabled = enabled;
            mobFtpAccountTextBox.Enabled = enabled;
            mobFtpPassTextBox.Enabled = enabled;
            mobileCheckBox.Enabled = enabled;
            openAPKButton.Enabled = enabled;
            openBaseZipButton.Enabled = enabled;
            mobUploadPatchButton.Enabled = enabled;
        }
        /// <summary>
        /// 创建版本效验
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        private List<PatchInformation> CreateVersion(IProgress<string> progress)
        {
            try
            {
                string[] files = Directory.GetFiles(CleanClientPath, "*.*", SearchOption.AllDirectories);

                PatchInformation[] list = new PatchInformation[files.Length];
                ParallelOptions po = new ParallelOptions { MaxDegreeOfParallelism = 8 };
                int count = 0;
                Parallel.For(0, files.Length, po, i =>
                {
                    list[i] = new PatchInformation(files[i], files[i].Remove(0, CleanClientPath.Length));
                    progress.Report($"创建版本: 文件 {Interlocked.Increment(ref count)} / {files.Length}");
                });

                return list.ToList();
            }
            catch (Exception ex)
            {
                progress.Report(ex.Message);
            }

            return null;
        }
        /// <summary>
        /// 获取补丁信息
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        private List<PatchInformation> GetPatchInformation(IProgress<string> progress, bool isAPKinfo = false)
        {
            try
            {
                progress.Report("下载补丁信息");

                using (WebClient client = new WebClient())
                {
                    if (FtpUseLogin)
                        client.Credentials = new NetworkCredential(Username, Password);

                    using (MemoryStream mStream = new MemoryStream(client.DownloadData(FtpHost + (isAPKinfo ? APKListFileName : PListFileName))))
                    using (BinaryReader reader = new BinaryReader(mStream))
                    {
                        List<PatchInformation> list = new List<PatchInformation>();

                        if (isAPKinfo)
                            LiveAPKVersion = reader.ReadString();

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
        /// <summary>
        /// 计算是否匹配 效验
        /// </summary>
        /// <param name="current"></param>
        /// <param name="live"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        private List<PatchInformation> CalculatePatch(List<PatchInformation> current, List<PatchInformation> live, IProgress<string> progress, bool isAPKinfo = false)
        {
            List<PatchInformation> patch = new List<PatchInformation>();

            if (current == null) return patch;

            if (isAPKinfo)
            {
                foreach (PatchInformation pi in current)
                {
                    PatchInformation lFile = live?.FirstOrDefault(x => x.FileName == pi.FileName);

                    if (lFile != null && IsMatch(lFile.CheckSum, pi.CheckSum))
                        continue;

                    Interlocked.Add(ref TotalUpload, pi.CompressedLength);
                    patch.Add(pi);
                }
            }
            else
            {
                ParallelOptions po = new ParallelOptions { MaxDegreeOfParallelism = 8 };
                int count = 0;
                Parallel.For(0, current.Count, po, i =>
                {
                    PatchInformation file = current[i];
                    PatchInformation lFile = live?.FirstOrDefault(x => x.FileName == file.FileName);

                    if (lFile != null && IsMatch(lFile.CheckSum, file.CheckSum))
                    {
                        file.CompressedLength = lFile.CompressedLength;
                        return;
                    }

                    if (!Directory.Exists(ClientPath + "Patch\\"))
                        Directory.CreateDirectory(ClientPath + "Patch\\");

                    string webFileName = file.FileName.Replace("\\", "-") + ".gz";
                    file.CompressedLength = Compress(file.FullFileName, $"{ClientPath}Patch\\{webFileName}");

                    Interlocked.Add(ref TotalUpload, file.CompressedLength);

                    lock (patch)
                        patch.Add(file);

                    progress.Report($"创建文件: {Interlocked.Increment(ref count)} / {current.Count}");

                });
            }

            return patch;
        }
        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="offSet"></param>
        /// <returns></returns>
        public static bool IsMatch(byte[] a, byte[] b, long offSet = 0)
        {
            if (b == null || a == null || b.Length + offSet > a.Length || offSet < 0) return false;

            for (int i = 0; i < b.Length; i++)
                if (a[offSet + i] != b[i])
                    return false;

            return true;
        }
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destFile"></param>
        /// <returns></returns>
        private static long Compress(string sourceFile, string destFile)
        {
            using (FileStream tofile = File.Create(destFile))
            using (FileStream fromfile = File.OpenRead(sourceFile))
            {
                using (System.IO.Compression.GZipStream gStream = new System.IO.Compression.GZipStream(tofile, System.IO.Compression.CompressionMode.Compress, true))
                    fromfile.CopyTo(gStream);

                return tofile.Length;
            }
        }

        /// <summary>
        /// 上传补丁
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="progress"></param>
        private void UploadPatch(List<PatchInformation> patch, IProgress<string> progress, bool isAPKinfo = false)
        {
            for (int i = 0; i < patch.Count; i++)
            {
                PatchInformation file = patch[i];

                progress.Report($"上传文件: {i + 1} / {patch.Count}");

                if (!Upload(file, progress, isAPKinfo))
                {
                    Error = true;
                    MessageBox.Show($"补丁上传出错：{file.FileName}");
                    return;
                }

            }
        }

        /// <summary>
        /// 主文件负载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PMain_Load(object sender, EventArgs e)
        {
#if !DEBUG

            mobUploadPatchButton.Enabled = false;
            mobUploadPatchButton.Visible = false;
#endif

            pcClientPathTextBox.Text = Config.PcCleanClientPath.EndsWith(@"\") ? Config.PcCleanClientPath : Config.PcCleanClientPath + @"\";
            pcFtpHostTextBox.Text = Config.PcFtpHost.EndsWith("/") ? Config.PcFtpHost : Config.PcFtpHost + "/";
            pcFtpUseLoginCheckBox.Checked = Config.PcFtpUseLogin;
            pcFtpAccountTextBox.Text = Config.PcUsername;
            pcFtpPassTextBox.Text = Config.PcPassword;

            mobClientPathTextBox.Text = Config.MobCleanClientPath.EndsWith(@"\") ? Config.MobCleanClientPath : Config.MobCleanClientPath + @"\";
            mobFtpHostTextBox.Text = Config.MobFtpHost.EndsWith("/") ? Config.MobFtpHost : Config.MobFtpHost + "/";
            mobFtpUseLoginCheckBox.Checked = Config.MobFtpUseLogin;
            mobFtpAccountTextBox.Text = Config.MobUsername;
            mobFtpPassTextBox.Text = Config.MobPassword;
        }
        /// <summary>
        /// 完整客户端路径更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pcClientPathTextBox_TextChanged(object sender, EventArgs e)
        {
            Config.PcCleanClientPath = pcClientPathTextBox.Text.EndsWith(@"\") ? pcClientPathTextBox.Text : pcClientPathTextBox.Text + @"\";
        }
        /// <summary>
        /// 上传链接地址更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pcFtpHostTextBox_TextChanged(object sender, EventArgs e)
        {
            Config.PcFtpHost = pcFtpHostTextBox.Text.EndsWith("/") ? pcFtpHostTextBox.Text : pcFtpHostTextBox.Text + "/";
        }
        /// <summary>
        /// 登录是否保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pcFtpUseLoginCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Config.PcFtpUseLogin = (bool)pcFtpUseLoginCheckBox.Checked;
        }
        /// <summary>
        /// 登录账号更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pcFtpAccountTextBox_TextChanged(object sender, EventArgs e)
        {
            Config.PcUsername = (string)pcFtpAccountTextBox.Text;
        }
        /// <summary>
        /// 登录密码更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pcFtpPassTextBox_TextChanged(object sender, EventArgs e)
        {
            Config.PcPassword = (string)pcFtpPassTextBox.Text;
        }


        private void mobClientPathTextBox_TextChanged(object sender, EventArgs e)
        {
            Config.MobCleanClientPath = mobClientPathTextBox.Text.EndsWith(@"\")? mobClientPathTextBox.Text : mobClientPathTextBox.Text + @"\";
        }

        private void mobFtpHostTextBox_TextChanged(object sender, EventArgs e)
        {
            Config.MobFtpHost = mobFtpHostTextBox.Text.EndsWith("/") ? mobFtpHostTextBox.Text : mobFtpHostTextBox.Text + "/";
        }

        private void mobFtpUseLoginCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Config.MobFtpUseLogin = mobFtpUseLoginCheckBox.Checked;
        }

        private void mobFtpAccountTextBox_TextChanged(object sender, EventArgs e)
        {
            Config.MobUsername = mobFtpAccountTextBox.Text;
        }

        private void mobFtpPassTextBox_TextChanged(object sender, EventArgs e)
        {
            Config.MobPassword = mobFtpPassTextBox.Text;
        }
        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="file"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        private bool Upload(PatchInformation file, IProgress<string> progress, bool isAPKinfo = false)
        {
            string webFileName = isAPKinfo ? file.FileName : file.FileName.Replace("\\", "-") + ".gz";

            try
            {
                using (WebClient client = new WebClient())
                {
                    if (FtpUseLogin) client.Credentials = new NetworkCredential(Username, Password);

                    bool uploading = true;
                    client.UploadProgressChanged += (o, e) => CurrentProgress = e.BytesSent;
                    client.UploadFileCompleted += (o, e) =>
                    {
                        if (!isAPKinfo)
                            File.Delete($"{ClientPath}Patch\\{webFileName}");
                        uploading = false;
                    };

                    if (!isAPKinfo && !Directory.Exists(ClientPath + "Patch\\"))
                        Directory.CreateDirectory(ClientPath + "Patch\\");

                    if (isAPKinfo)
                        client.UploadFileAsync(new Uri(FtpHost + webFileName), file.FullFileName);
                    else
                        client.UploadFileAsync(new Uri(FtpHost + webFileName), $"{ClientPath}Patch\\{webFileName}");

                    while (uploading)
                        Task.Delay(TimeSpan.FromMilliseconds(1));

                    CurrentProgress = 0;
                    TotalProgress += file.CompressedLength;
                }

                return true;
            }
            catch (Exception ex)
            {
                progress.Report(ex.Message);
            }

            return false;
        }
        /// <summary>
        /// 创建尺寸大小标签
        /// </summary>
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

            if (TotalUpload > GB)
                text.Append($" / {TotalUpload / GB:#,##0.0}GB");
            else if (TotalUpload > MB)
                text.Append($" / {TotalUpload / MB:#,##0.0}MB");
            else if (TotalUpload > KB)
                text.Append($" / {TotalUpload / KB:#,##0}KB");
            else
                text.Append($" / {TotalUpload:#,##0}B");

            UploadSizeLabel.Text = text.ToString();

            if (TotalUpload > 0)
                TotalProgressBar.Value = Math.Max(0, Math.Min(100, (int)(progress * 100 / TotalUpload)));

            long speed = (progress - LastUploadProcess) * TimeSpan.TicksPerSecond / (DateTime.Now.Ticks - LastSpeedCheck.Ticks); //May cause errors?
            LastUploadProcess = progress;

            if (speed > GB)
                UploadSpeedLabel.Text = $"{speed / GB:#,##0.0}GBps";
            else if (speed > MB)
                UploadSpeedLabel.Text = $"{speed / MB:#,##0.0}MBps";
            else if (speed > KB)
                UploadSpeedLabel.Text = $"{speed / KB:#,##0}KBps";
            else
                UploadSpeedLabel.Text = $"{speed:#,##0}Bps";

            LastSpeedCheck = DateTime.Now;
        }
        /// <summary>
        /// 保存版本
        /// </summary>
        /// <param name="current"></param>
        private void SaveVersion(List<PatchInformation> current, bool isAPKinfo = false)
        {
            using (MemoryStream mStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(mStream))
            {
                if (isAPKinfo)
                    writer.Write(CurrentAPKVersion);

                foreach (PatchInformation info in current)
                    info.Save(writer);

                using (WebClient client = new WebClient())
                {
                    if (FtpUseLogin)
                        client.Credentials = new NetworkCredential(Username, Password);

                    client.UploadData(new Uri(FtpHost + (isAPKinfo ? APKListFileName : PListFileName)), mStream.ToArray());
                }
            }
        }

        private void mobileCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Mobile = mobileCheckBox.Checked;
        }

        private void openAPKButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Android APK|*.apk";
            if (openDialog.ShowDialog() != DialogResult.OK) return;

            apkFIleTextBox.Text = openDialog.FileName;
            CurrentAPKVersion = null;
            apkVersionValueLabel.Text = "";

            using (var zip = Ionic.Zip.ZipFile.Read(openDialog.FileName))
            {
                using (var zipStream = zip["AndroidManifest.xml"].OpenReader())
                {
                    byte[] mainfest = new byte[zipStream.Length];
                    zipStream.Read(mainfest, 0, mainfest.Length);

                    using (var stream = new MemoryStream(mainfest))
                    {
                        var reader = new AndroidXmlReader(stream);
                        List<AndroidInfo> androidInfos = new List<AndroidInfo>();

                        while (reader.Read())
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    {
                                        AndroidInfo info = new AndroidInfo();
                                        androidInfos.Add(info);
                                        info.Name = reader.Name;
                                        info.Settings = new List<AndroidSetting>();
                                        for (int i = 0; i < reader.AttributeCount; i++)
                                        {
                                            reader.MoveToAttribute(i);

                                            AndroidSetting setting = new AndroidSetting() { Name = reader.Name, Value = reader.Value };
                                            info.Settings.Add(setting);
                                        }
                                        reader.MoveToElement();
                                        break;
                                    }
                            }
                        }

                        StringBuilder builder = new StringBuilder();
                        var main = androidInfos.FirstOrDefault(x => x.Name == "manifest");
                        if (main != null)
                        {
                            var versingName = main.Settings.FirstOrDefault(x => x.Name == "android:versionName");
                            if (versingName != null)
                            {
                                builder.Append(string.Format("{0}", versingName.Value));
                                var versionCode = main.Settings.FirstOrDefault(x => x.Name == "android:versionCode");
                                if (versionCode != null)
                                {
                                    builder.Append(string.Format(".{0}", versionCode.Value));
                                    using (FileStream fs = File.OpenRead(openDialog.FileName))
                                    {
                                        ApkPatchInfo = new PatchInformation(openDialog.FileName, Path.GetFileName(openDialog.FileName)) { CompressedLength = fs.Length };
                                    }

                                    CurrentAPKVersion = builder.ToString();
                                    apkVersionValueLabel.Text = CurrentAPKVersion;
                                }
                            }

                        }
                    }
                }
            }
        }

        private void openBaseZipButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "手游基础包|*.zip";
            if (openDialog.ShowDialog() != DialogResult.OK) return;

            baseZipTextBox.Text = openDialog.FileName;

            using (FileStream stream = File.OpenRead(openDialog.FileName))
            {
                BaseZipPatchInfo = new PatchInformation(openDialog.FileName, Path.GetFileName(openDialog.FileName)) { CompressedLength = stream.Length };
            }
        }
    }
}
