using Client.Envir;
using Mir3.Mobile;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Patch
{
    public class Update
    {

        const string PatcherFileName = "Patcher.exe";
        const string PListFileName = "PList.Bin";
        const string Patchs = "Patchs";
        string ClientPath => CEnvir.MobileClientPath;
        bool _needUpdate = false;

        public PatchViewModel Vm { get; private set; }
        public bool AllCompleted { get; set; } = false;
        public Update(PatchViewModel vm)
        {
            Vm = vm;
        }

        public async Task CheckPatchAsync(bool repair)
        {
            //第一步
            // 1. Android环境会判断apk版本，然后更新apk包
            // 2. 在没有Map目录的时候 先更新基础包
            var pkginfo = await GetAPKVersion();
            var currentpkginfo = await LoadAPKVersion();
#if ANDROID
            //判断APK版本是否匹配，不匹配更新apk
            if ((pkginfo != null && string.Format("{0}.{1}", Config.VersionName, Config.VersionCode) != pkginfo.APKVersion))
            {
                Vm.Update();
                var apktask = Task.Run(() =>
                {
                    if (!DownloadFile(pkginfo.APKFileName, pkginfo.APKCompressedLength)) return;
                });
                while (!apktask.IsCompleted)
                {
                    Vm.Update();

                    await Task.Delay(100);
                }
                Vm.Update();

                //更新apk
                if (!Game1.Native.UpdateAPKVersion(pkginfo.APKFileName))
                    Vm.LoadText = "APK文件校验错误，请重新更新游戏...";
                return;//有版本更新不让进游戏，必须要更新游戏
            }
#endif

            //所有平台判断版本是否匹配，不匹配不让进游戏
            if (pkginfo != null && string.Format("{0}.{1}", Config.VersionName, Config.VersionCode) != pkginfo.APKVersion)
            {
                Vm.LoadText = "版本不匹配，请重新更新游戏...";
                return;
            }

            //基础包，没有Map目录就下载解压DataAdd.zip
            if ((!Directory.Exists(Path.Combine(CEnvir.MobileClientPath, "Map"))) ||
                (pkginfo != null && currentpkginfo != null && !IsMatch(pkginfo.BaseZipCheckSum, currentpkginfo.BaseZipCheckSum)))
            {
                Vm.Update();
                var maptask = Task.Run(() =>
                {
                    List<Task> tasks = new List<Task>();

                    if (DownloadFile(pkginfo.BaseZipFileName, pkginfo.BaseZipCompressedLength))
                        tasks.Add(Task.Run(() =>
                        {
                            string file = Path.Combine(Path.Combine(ClientPath, Patchs), pkginfo.BaseZipFileName);
                            using (var stream = File.OpenRead(file))
                            {
                                using (var arc = new ZipArchive(stream))
                                {
                                    try
                                    {
                                        Vm.LoadText = "正在解压基础包.";
                                        arc.ExtractToDirectory(ClientPath, true);
                                    }
                                    catch
                                    {
                                        CEnvir.SaveError("基础包解压失败");
                                        Vm.LoadText = "基础包 解压失败.";
                                    }

                                }
                            }
                            File.Delete(file);
                        }));

                    if (tasks.Count == 0) return;

                    Task.WaitAll(tasks.ToArray());
                });
                while (!maptask.IsCompleted)
                {
                    Vm.Update();

                    await Task.Delay(100);
                }
                Vm.Update();
            }

            if (pkginfo != null)
                SaveAPKVersion(pkginfo);

            //如果基础包解压不成功，不让进游戏
            if (!Directory.Exists(Path.Combine(CEnvir.MobileClientPath, "Map")))
            {
                Vm.LoadText = "缺失基础包，无法进入游戏.";
                return;
            }

            //第二步更新补丁文件
            var liveVersion = await GetPatchInformation();
            if (liveVersion == null)
            {
                Vm.LoadText = "无法连接到更新服务.";
                AllCompleted = true; //补丁文件没更新，可以进游戏
                return;
            }

            var currentVersion = repair ? null : await LoadVersion();
            var patch = CalculatePatch(liveVersion, currentVersion);
            Vm.Update();
            var task = Task.Run(() => DownloadPatch(patch));
            while (!task.IsCompleted)
            {
                Vm.Update();

                await Task.Delay(100);
            }
            Vm.Update();
            SaveVersion(liveVersion);
            Vm.LoadText = "完成";
            Vm.CurrentWidth = Vm.MaxWidth;
            Vm.TotalWidth = Vm.MaxWidth;
            AllCompleted = true;
        }

        private bool Download(PatchInformation file)
        {
            var fileName = file.FileName.Replace("\\", "-");
            string webFileName = fileName + ".gz";
            var clientPath = Path.Combine(ClientPath, Patchs);
            try
            {

                using (WebClient client = new WebClient())
                {
                    if (Config.UseLogin) client.Credentials = new NetworkCredential(Config.UserName, Config.Password);

                    bool downloading = true;
                    int downloads = 0;
                    client.DownloadProgressChanged += (o, e) =>
                    {
                        Vm.CurrentProgress = e.BytesReceived;
                        downloads++;
                        if (downloads > 3)
                        {
                            downloads = 0;
                        }
                        Vm.LoadText = $"下载[{fileName}]文件中{new string('.', downloads * 2)}";

                        Vm.CurrentWidth = (int)((e.ProgressPercentage / 100f) * Vm.MaxWidth);
                    };
                    client.DownloadFileCompleted += (o, e) => downloading = false;

                    if (!Directory.Exists(clientPath))
                        Directory.CreateDirectory(clientPath);
                    client.DownloadFileTaskAsync(new Uri(Path.Combine(Config.Host, webFileName)), Path.Combine(clientPath, webFileName));

                    while (downloading)
                        Thread.Sleep(1);

                    Vm.CurrentProgress = 0;
                    Vm.TotalProgress += file.CompressedLength;
                }

                return true;
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.Message);
                file.CheckSum = new byte[8];
            }

            return false;
        }

        private void DownloadPatch(List<PatchInformation> patch)
        {
            List<Task> tasks = new List<Task>();

            foreach (PatchInformation file in patch)
            {
                if (!Download(file)) continue;

                tasks.Add(Task.Run(() => Extract(file)));
            }

            if (tasks.Count == 0) return;

            Task.WaitAll(tasks.ToArray());
        }

        private async Task<List<PatchInformation>> LoadVersion()
        {
            var list = new List<PatchInformation>();
            try
            {
                var path = Path.Combine(ClientPath, "Version.bin");
                if (File.Exists(path))
                {
                    var bytes = await File.ReadAllBytesAsync(path);
                    using (MemoryStream mStream = new MemoryStream(bytes))
                    using (BinaryReader reader = new BinaryReader(mStream))
                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            var pinfo = new PatchInformation(reader);

                            if (!File.Exists(pinfo.FileName)) continue;
                            list.Add(pinfo);
                        }

                    return list;
                }
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.Message);
                return null;
            }

            return null;
        }

        private async Task<List<PatchInformation>> GetPatchInformation()
        {
            try
            {

                //using (var client = new HttpClient())
                //{
                //    if (Config.UseLogin)
                //    {
                //        var authToken = Encoding.ASCII.GetBytes($"{Config.UserName}:{Config.Password}");
                //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
                //    }
                //    var bytes = await client.GetByteArrayAsync(Path.Combine(Config.Host, PListFileName));
                //    using (MemoryStream mStream = new MemoryStream(bytes))
                //    using (BinaryReader reader = new BinaryReader(mStream))
                //    {
                //        List<PatchInformation> list = new List<PatchInformation>();

                //        while (reader.BaseStream.Position < reader.BaseStream.Length)
                //            list.Add(new PatchInformation(reader));

                //        return list;
                //    }
                //}

                using (WebClient client = new WebClient())
                {
                    if (Config.UseLogin)
                        client.Credentials = new NetworkCredential(Config.UserName, Config.Password);
                    var bytes = await client.DownloadDataTaskAsync(Path.Combine(Config.Host, PListFileName));
                    using (MemoryStream mStream = new MemoryStream(bytes))
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
                CEnvir.SaveError(ex.Message);
                return null;
            }
        }

        private List<PatchInformation> CalculatePatch(IReadOnlyList<PatchInformation> list, List<PatchInformation> current)
        {
            List<PatchInformation> patch = new List<PatchInformation>();

            if (list == null) return patch;

            for (int i = 0; i < list.Count; i++)
            {

                PatchInformation file = list[i];
                if (current != null && current.Any(x => x.FileName.Replace("\\", "/") == file.FileName.Replace("\\", "/") && IsMatch(x.CheckSum, file.CheckSum))) continue;

                if (System.IO.File.Exists(Path.Combine(ClientPath, file.FileName.Replace("\\", "/"))))
                {
                    byte[] CheckSum;
                    using (MD5 md5 = MD5.Create())
                    {
                        using (FileStream stream = System.IO.File.OpenRead(Path.Combine(ClientPath, file.FileName.Replace("\\", "/"))))
                            CheckSum = md5.ComputeHash(stream);
                    }

                    if (IsMatch(CheckSum, file.CheckSum))
                        continue;
                }
                patch.Add(file);
                Vm.TotalDownload += file.CompressedLength;
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

        private void Extract(PatchInformation file)
        {
            var fileName = file.FileName.Replace("\\", "-");
            Vm.LoadText = $"解压[{fileName}]文件...";
            string webFileName = fileName + ".gz";

            try
            {
                string toPath = Path.Combine(ClientPath, file.FileName.Replace("\\", "/"));

                if (PatcherFileName.EndsWith(file.FileName, StringComparison.OrdinalIgnoreCase))
                {
                    toPath += ".tmp";
                    _needUpdate = true;
                }


                if (System.IO.File.Exists(toPath)) System.IO.File.Delete(toPath);

                Decompress(Path.Combine(ClientPath, Patchs, webFileName), toPath);

            }
            catch (UnauthorizedAccessException)
            {
                file.CheckSum = new byte[8];
                Vm.LoadText = "文件可能正在使用中, 请确保游戏已关闭.";
            }
            catch
            {
                file.CheckSum = new byte[8];
            }
            finally
            {
                Vm.LoadText = "完成";
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
            File.Delete(sourceFile);
        }

        private void SaveVersion(List<PatchInformation> version)
        {
            using (FileStream fStream = System.IO.File.Create(Path.Combine(ClientPath, "Version.bin")))
            using (BinaryWriter writer = new BinaryWriter(fStream))
            {
                foreach (PatchInformation info in version)
                    info.Save(writer);
            }
        }

        /// <summary>
        /// 验证apk版本和dataadd.zip文件
        /// </summary>
        private async Task<PKGInformation> GetAPKVersion()
        {
            // APKVersion.bin 定义
            // 1.2.5.3                   --apk文件版本
            // xjmir3prod.apk 203766483  --apk文件名和字节数
            // DataAdd.zip 128176838     --基础包文件名和字节数
            //
            try
            {
                using (WebClient client = new WebClient())
                {
                    if (Config.UseLogin)
                        client.Credentials = new NetworkCredential(Config.UserName, Config.Password);
                    var bytes = await client.DownloadDataTaskAsync(Path.Combine(Config.Host, "APKVersion.bin"));
                    using (MemoryStream mStream = new MemoryStream(bytes))
                    using (BinaryReader reader = new BinaryReader(mStream))
                    {

                        return new PKGInformation()
                        {
                            APKVersion = reader.ReadString(),
                            APKFileName = reader.ReadString(),
                            APKCompressedLength = reader.ReadInt64(),
                            APKCheckSum = reader.ReadBytes(reader.ReadInt32()),
                            BaseZipFileName = reader.ReadString(),
                            BaseZipCompressedLength = reader.ReadInt64(),
                            BaseZipCheckSum = reader.ReadBytes(reader.ReadInt32()),
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 加载本地APKVersion
        /// </summary>
        private async Task<PKGInformation> LoadAPKVersion()
        {
            try
            {
                var path = Path.Combine(ClientPath, "APKVersion.bin");
                if (File.Exists(path))
                {
                    var bytes = await File.ReadAllBytesAsync(path);
                    using (MemoryStream mStream = new MemoryStream(bytes))
                    using (BinaryReader reader = new BinaryReader(mStream))
                    {
                        return new PKGInformation()
                        {
                            APKVersion = reader.ReadString(),
                            APKFileName = reader.ReadString(),
                            APKCompressedLength = reader.ReadInt64(),
                            APKCheckSum = reader.ReadBytes(reader.ReadInt32()),
                            BaseZipFileName = reader.ReadString(),
                            BaseZipCompressedLength = reader.ReadInt64(),
                            BaseZipCheckSum = reader.ReadBytes(reader.ReadInt32()),
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.Message);
                return null;
            }

            return null;
        }
        private void SaveAPKVersion(PKGInformation version)
        {
            using (FileStream fStream = File.Create(Path.Combine(ClientPath, "APKVersion.bin")))
            using (BinaryWriter writer = new BinaryWriter(fStream))
            {
                writer.Write(version.APKVersion);
                writer.Write(version.APKFileName);
                writer.Write(version.APKCompressedLength);
                writer.Write(version.APKCheckSum.Length);
                writer.Write(version.APKCheckSum);
                writer.Write(version.BaseZipFileName);
                writer.Write(version.BaseZipCompressedLength);
                writer.Write(version.BaseZipCheckSum.Length);
                writer.Write(version.BaseZipCheckSum);
            }
        }
        /// <summary>
        /// 下载单个文件
        /// </summary>
        private bool DownloadFile(string filename, long length)
        {
            var clientPath = Path.Combine(ClientPath, Patchs);
            Vm.TotalDownload += length;
            try
            {

                using (WebClient client = new WebClient())
                {
                    if (Config.UseLogin) client.Credentials = new NetworkCredential(Config.UserName, Config.Password);

                    bool downloading = true;
                    int downloads = 0;
                    client.DownloadProgressChanged += (o, e) =>
                    {
                        Vm.CurrentProgress = e.BytesReceived;
                        downloads++;
                        if (downloads > 3)
                        {
                            downloads = 0;
                        }
                        Vm.LoadText = $"下载[{filename}]文件中{new string('.', downloads * 2)}";

                        Vm.CurrentWidth = (int)((e.ProgressPercentage / 100f) * Vm.MaxWidth);
                    };
                    client.DownloadFileCompleted += (o, e) => downloading = false;

                    if (!Directory.Exists(clientPath))
                        Directory.CreateDirectory(clientPath);
                    client.DownloadFileTaskAsync(new Uri(Path.Combine(Config.Host, filename)), Path.Combine(clientPath, filename));

                    while (downloading)
                        Thread.Sleep(1);

                    Vm.CurrentProgress = 0;
                    Vm.TotalProgress += length;
                }
                return true;
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.Message);
                return false;
            }
        }


    }
}