using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using AndroidX.Core.Content;
using Client.Envir;
using Library;
using Microsoft.Xna.Framework;
using Mir3.Mobile;
using System;
using System.IO;
using System.IO.Compression;
using Config = Client.Envir.Config;

namespace Mir3.Droid
{
    public class DrodNative : INative
    {
        public string SafeCode => MainActivity.Main.GetVersionName();

        public IUI UI => MainActivity.Main;

        public byte[] GetFileBytes(string path)
        {
            byte[] data;
            using (System.IO.Stream stream = Game.Activity.Assets!.Open(path))
            {
                MemoryStream memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                memoryStream.Seek(0L, SeekOrigin.Begin);
                stream.Close();
                data = memoryStream.ToArray();
            }
            return data;
        }

        public Stream GetFileStream(string path)
        {
            var stream = Game.Activity.Assets!.Open(path);
            return stream;
        }

        public void HideInputField()
        {
            MainActivity.HideInputField();
        }

        public void InitData()
        {
            var code = MainActivity.Main.GetVersionCode();
            var name = MainActivity.Main.GetVersionName();
#if DEBUG
#else
            if (Config.VersionCode != code || Config.VersionName != name)
#endif
            {
                using (var stream = GetFileStream("Data.zip"))
                {
                    using (var arc = new ZipArchive(stream))
                    {
                        try
                        {
                            arc.ExtractToDirectory(CEnvir.MobileClientPath, true);
                        }
                        finally
                        {
                            ConfigReader.Load();
                            Config.VersionCode = code;
                            Config.VersionName = name;
                            ConfigReader.Save();
                        }
                    }
                }

                //DataAdd.zip 不打包了，放到更新服务器上了
                //if (!Directory.Exists(Path.Combine(CEnvir.MobileClientPath, "Data", "Map Data")))
                //{
                //    using (var stream = GetFileStream("DataAdd.zip"))
                //    {
                //        using (var arc = new ZipArchive(stream))
                //        {
                //            try
                //            {
                //                arc.ExtractToDirectory(CEnvir.MobileClientPath, true);
                //            }
                //            catch
                //            {
                //                CEnvir.SaveError("Data-add 解压失败");
                //            }

                //        }
                //    }
                //}
            }
#if DEBUG
            Config.DebugLabel = true;
#endif
        }
        public bool IsPad => Game1.Game.Graphics.PreferredBackBufferWidth / Game1.Game.Graphics.PreferredBackBufferHeight < 1.5F;

        public void Initialize()
        {
            //手机目录
            CEnvir.MobileClientPath = Application.Context.GetExternalFilesDir(null)!.AbsolutePath + "/";

            //根据手机分辨率和dpi设置客户端缩放比率
            DisplayMetrics displayMetrics = new DisplayMetrics();
            MainActivity.Main.WindowManager!.DefaultDisplay!.GetMetrics(displayMetrics);
            CEnvir.Target = new System.Drawing.Size(Game1.Game.Graphics.PreferredBackBufferWidth, Game1.Game.Graphics.PreferredBackBufferHeight);
            CEnvir.Density = displayMetrics.Density;
            CEnvir.DeviceHeight = Game1.Game.Graphics.PreferredBackBufferHeight / displayMetrics.Ydpi;
            //屏幕比例 <1.5 就 4:3 比例
            if (Game1.Game.Graphics.PreferredBackBufferWidth / Game1.Game.Graphics.PreferredBackBufferHeight < 1.5F)
            {
                CEnvir.DevicePercent = 4;
                Config.GameSize = new System.Drawing.Size(1024, 768);
            }
            //屏幕比例 >=1.5 就 16:9 比例
            else
            {
                CEnvir.DevicePercent = 16;
                Config.GameSize = new System.Drawing.Size(960, 540);
            }
        }

        public void OpenUrl(string url)
        {
            //System.Diagnostics.Process.Start(url);
            MainActivity.Main.StartWebView(url);
        }

        public void ShowInputField(string text)
        {
            MainActivity.ShowInputField(text);
        }

        public bool UpdateAPKVersion(string filename)
        {
            try
            {
                string apkfile = Path.Combine(Path.Combine(CEnvir.MobileClientPath, "Patchs"), filename);
                Activity activity = Game.Activity;
                Java.IO.File file = new Java.IO.File(apkfile);
                bool exist = file.Exists();
                if (!exist || file.Length() <= 0)
                {
                    CEnvir.SaveError("APK文件不存在或者大小为0");
                    return false;
                }
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    Android.Net.Uri uriForFile = FileProvider.GetUriForFile(activity, activity.ApplicationInfo.PackageName + ".fileProvider", file);
                    Intent intent = new Intent(Intent.ActionInstallPackage);
                    intent.SetData(uriForFile);
                    intent.SetFlags(ActivityFlags.GrantReadUriPermission);
                    activity.StartActivity(intent);
                }
                else
                {
                    Android.Net.Uri data = Android.Net.Uri.FromFile(file);
                    Intent intent2 = new Intent(Intent.ActionView);
                    intent2.SetDataAndType(data, "application/vnd.android.package-archive");
                    intent2.SetFlags(ActivityFlags.NewTask);
                    activity.StartActivity(intent2);
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

