using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Principal;

namespace Client.Envir
{
    public static class AntiCheat
    {

        static bool IsElevated
        {
            get
            {
                var id = WindowsIdentity.GetCurrent();
                return id.Owner != id.User;
            }
        }

        /// <summary>
        /// 计算文件的哈希值
        /// </summary>
        /// <param name="filename">文件完整路径</param>
        /// <param name="hashingAlgoType">哈希算法</param>
        /// <returns></returns>
        public static string GetChecksum(string filename, HashingAlgoTypes hashingAlgoType = HashingAlgoTypes.SHA256)
        {
            if (File.Exists(filename))
            {
                using var hasher = System.Security.Cryptography.HashAlgorithm.Create(hashingAlgoType.ToString());
                using var stream = System.IO.File.OpenRead(filename);
                if (hasher != null)
                {
                    var hash = hasher.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "");
                }
            }

            return null;
        }

        /// <summary>
        /// 无需管理员权限，可以拿到32bit和64bit
        /// </summary>
        /// <returns></returns>
        public static HashSet<string> GetAllProcessesPathsV1()
        {
            HashSet<string> result = new HashSet<string>();
            var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
            {
                var query = from p in Process.GetProcesses()
                    join mo in results.Cast<ManagementObject>()
                        on p.Id equals (int)(uint)mo["ProcessId"]
                    select new
                    {
                        //Process = p.ProcessName,
                        Path = (string)mo["ExecutablePath"],
                    };

                foreach (var item in query)
                {
                    if (item.Path != null)
                    {
                        result.Add(item.Path);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 需要管理员权限，只能拿到当前64bit
        /// </summary>
        /// <returns></returns>
        public static HashSet<string> GetAllProcessesPathsV2()
        {
            HashSet<string> result = new HashSet<string>();

            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    var path = process?.MainModule?.FileName;
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        result.Add(path);
                    }
                }
                catch
                {
                    // ignored
                }
            }

            return result;
        }

        public static List<string> ComputeAllHashes()
        {
            List<string> result = new List<string>();

            HashSet<string> pathList;

            //pathList = IsElevated ? GetAllProcessesPathsV2() : GetAllProcessesPathsV1();
            pathList = GetAllProcessesPathsV1();

            foreach (var path in pathList)
            {
                result.Add(GetChecksum(path));
            }

            return result;
        }
    }

    public enum HashingAlgoTypes
    {
        MD5,
        SHA1,
        SHA256,
        SHA384,
        SHA512
    }


}
