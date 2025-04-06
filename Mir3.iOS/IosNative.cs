using System;
using System.IO;
using System.IO.Compression;
using Client.Controllers;
using Client.Envir;
using Foundation;
using Library;
using Mir3.iOS;
using Mir3.Mobile;
using UIKit;

namespace Client
{
    public class IosNative : INative
    {
        public string SafeCode => "version";//TODO

        public IUI UI => Program.Activity;

        public byte[] GetFileBytes(string path)
        {
            return File.ReadAllBytes(Path.Combine("Assets", path));
        }

        public Stream GetFileStream(string path)
        {
            var stream = GetFileBytes(path);
            Stream result = new MemoryStream(stream);
            return result;
        }

        public void HideInputField()
        {
            UI.HidePopWindow();
        }

        public void InitData()
        {
            var code = (NSString)NSBundle.MainBundle.InfoDictionary.ObjectForKey(new NSString("CFBundleVersion")); 
            var name = (NSString)NSBundle.MainBundle.InfoDictionary.ObjectForKey(new NSString("CFBundleShortVersionString"));
            var version = long.Parse(code.ToString().Replace(".", ""));
           
            if (Config.VersionCode != version || Config.VersionName != name.ToString())
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
                            Config.VersionCode = version;
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
            //#if DEBUG
            //            Config.DebugLabel = true;
            //#endif
        }

        public bool IsPad => UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom == UIKit.UIUserInterfaceIdiom.Pad;

        public void Initialize()
        {
            CEnvir.MobileClientPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/";
            CEnvir.Target = new System.Drawing.Size(Game1.Game.Graphics.PreferredBackBufferWidth, Game1.Game.Graphics.PreferredBackBufferHeight);
            CEnvir.Density = (float)UIScreen.MainScreen.Scale;
            CEnvir.DeviceHeight = Game1.Game.Graphics.PreferredBackBufferHeight / GetDpi();
            //PAD
            if (IsPad)
            {
                CEnvir.DevicePercent = 4;
                Config.GameSize = new System.Drawing.Size(1024, 768);
            }
            //IOS
            else
            {
                CEnvir.DevicePercent = 16;
                Config.GameSize = new System.Drawing.Size(960, 540);
            }
        }

        public void OpenUrl(string url)
        {
            var nsUrl = new NSUrl(url);
            UIApplication.SharedApplication.OpenUrl(nsUrl);
        }

        public void ShowInputField(string text)
        {
            UI.Push(new InputController(UI,text));
        }

        private static float GetDpi()
        {
            var scale = UIScreen.MainScreen.Scale;
            nfloat dpi = 0;
            if (UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
            {

                dpi = 132 * scale;

            }
            else if (UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {

                dpi = 163 * scale;
            }
            else
            {

                dpi = 160 * scale;

            }
            return (float)dpi;
        }
    }
}

