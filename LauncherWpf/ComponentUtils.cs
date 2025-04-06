using Microsoft.Win32;
using System;

namespace Launcher
{
    public static class ComponentUtils
    {

        public static class OSUtil
        {
            public static bool IsCompatible()
            {
                //bool passed = RuntimeInformation.OSArchitecture == Architecture.X64;
                return Environment.Is64BitOperatingSystem;
            }
        }
        public static class DotNetUtil
        {
            public enum DotNetRelease
            {
                NOTFOUND,
                NET45,
                NET451,
                NET452,
                NET46,
                NET461,
                NET462,
                NET47,
                NET471,
                NET472,
                NET48,
            }

            public static bool IsCompatible(DotNetRelease req = DotNetRelease.NET461)
            {
                DotNetRelease r = GetRelease();
                if (r < req)
                {
                    return false;
                }
                return true;
            }

            public static DotNetRelease GetRelease(int release = default(int))
            {
                int r = release != default(int) ? release : GetVersion();
                if (r >= 528040) return DotNetRelease.NET48;
                if (r >= 461808) return DotNetRelease.NET472;
                if (r >= 461308) return DotNetRelease.NET471;
                if (r >= 460798) return DotNetRelease.NET47;
                if (r >= 394802) return DotNetRelease.NET462;
                if (r >= 394254) return DotNetRelease.NET461;
                if (r >= 393295) return DotNetRelease.NET46;
                if (r >= 379893) return DotNetRelease.NET452;
                if (r >= 378675) return DotNetRelease.NET451;
                if (r >= 378389) return DotNetRelease.NET45;
                return DotNetRelease.NOTFOUND;
            }

            public static int GetVersion()
            {
                int release = 0;
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                    .OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
                {
                    if (key != null) release = Convert.ToInt32(key.GetValue("Release"));
                    else return 0;
                }
                return release;
            }

            public static void SilentInstallDotNet(string path)
            {
                string strCmdText = $"{path} /q /norestart";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);
            }
        }

        public static class DXUtil
        {
            public static int GetVersion()
            {
                int version = 0;
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DirectX"))
                {
                    if (key != null)
                    {
                        string versionStr = key.GetValue("Version") as string;
                        if (!string.IsNullOrEmpty(versionStr))
                        {
                            var versionComponents = versionStr.Split('.');
                            if (versionComponents.Length > 1)
                            {
                                if (int.TryParse(versionComponents[1], out int directXLevel))
                                {
                                    version = directXLevel;
                                }
                            }
                        }
                    }
                }

                return version;
            }

            public static bool IsCompatible(int req = 9)
            {
                int r = GetVersion();
                if (r < req)
                {
                    return false;
                }
                return true;
            }

            public static void SilentInstallDX(string path)
            {
                string strCmdText = $"{path} /silent";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);
            }
        }
    }
}