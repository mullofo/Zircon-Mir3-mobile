using Client.Controls;
using Client.Envir;
using Library;
using Library.Network;
using Mir3.Mobile;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Client.Helpers
{
    public class LibraryHelper
    {
        static readonly HttpClient httpClient = new HttpClient();

        public static bool MicroServerActive
        {
            get => _MicroServerActive;
            set
            {
                if (_MicroServerActive == value) return;

                _MicroServerActive = value;

                if (!value)
                {
                    if (Message == null || Message.IsDisposed)
                        Message = DXMessageBox.Show("微端服务连接失败，将影响游戏体验。\n15秒后将重新连接！", "错误");
                }
                else
                {
                    if (Message != null)
                    {
                        Message.TryDispose();
                        Message = null;
                    }
                }
            }
        }
        private static bool _MicroServerActive;

        private static DateTime RetryTime;
        private static int ExceptionCount;

        public static readonly SemaphoreSlim Semaphore;
        private const int LimitTask = 10;

        public static DXMessageBox Message = DXMessageBox.Show("微端服务连接失败，将影响游戏体验。\n15秒后将重新连接！", "错误");

        static LibraryHelper()
        {
            Semaphore = new SemaphoreSlim(LimitTask);

            httpClient.BaseAddress = new Uri($"http://{Config.MicroClientIP}:{Config.MicroClientPort}/api/");
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            var user = "BlackDragon";
            var safeCode = Game1.Native.SafeCode;

            httpClient.DefaultRequestHeaders.Add("User", HttpUtility.UrlEncode(user));
            httpClient.DefaultRequestHeaders.Add("Code", safeCode);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));

        }

        private static bool CheckServerOnline()
        {
            try
            {
                // 发送一个连接，判断微端服务是否启动
                using (HttpResponseMessage response = httpClient.GetAsync(@"file/Data/Ground.Zl", HttpCompletionOption.ResponseHeadersRead).Result)
                {
                    if (response.IsSuccessStatusCode)
                        return true;
                }
            }
            catch
            {

            }
            return false;
        }

        #region 合并资源
        /// <summary>
        /// 待合并列表
        /// </summary>
        static ConcurrentDictionary<string, List<MicroLibraryImage>> SaveImageList { get; set; } = new ConcurrentDictionary<string, List<MicroLibraryImage>>();

        static bool _updating = false;

        public static void Process()
        {
            //Console.WriteLine(Semaphore.CurrentCount);
            if (_updating)
            {
                //Debug.WriteLine("======= updating" + DateTime.Now.ToString("HH:mm:ss.fff"));
                return;
            }
            _updating = true;


            //重试连接微端
            if ((!MicroServerActive || ExceptionCount >= 5) && Time.Now > RetryTime)
            {
                ExceptionCount = 0;
                RetryTime = Time.Now + TimeSpan.FromSeconds(15);
                MicroServerActive = CheckServerOnline();
            }

            foreach (var x in SaveImageList)
            {
                if (System.IO.File.Exists(CEnvir.MobileClientPath + x.Key) && x.Value?.Count > 0)
                    using (FileStream stream = new FileStream(CEnvir.MobileClientPath + x.Key, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        for (int i = 0; i < x.Value.Count; i++)
                        {
                            var image = x.Value[i];
                            stream.Seek(image.Postion, SeekOrigin.Begin);
                            stream.Write(image.ImageData, 0, image.ImageData.Length);
                        }
                        x.Value.Clear();
                    }

            }
            //Debug.WriteLine($"=======End   {mir.MirImage.FileName} {mir.MirImage.Index} {DateTime.Now.ToString("HH:mm:ss.fff")}");

            _updating = false;
        }
        #endregion

        /// <summary>
        /// 获取Sound文件，并创建
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> GetSound(string fileName)
        {
            var realName = fileName;
            fileName = fileName.Replace("./", "");
            fileName = fileName.Replace(".\\", "");
            fileName = fileName.Replace("\\", "/");
            var name = Path.GetFileName(fileName); //获取文件名，包含扩展名
            var path = fileName.Replace("/" + name, ""); //获取路劲
            name = HttpUtility.UrlPathEncode(name);
            path = HttpUtility.UrlPathEncode(path.Replace('/', '_'));
            var api = $"file/{path}/{name}";

            try
            {
                return await DownloadFile(api, CEnvir.MobileClientPath + realName);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError($"素材：{api},下载资源异常,异常原因{ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// 获取ZL头文件，并创建
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<bool> GetHeaderAsync(string fileName)
        {
            var realName = fileName;
            fileName = fileName.Replace("./", "");
            fileName = fileName.Replace(".\\", "");
            fileName = fileName.Replace("\\", "/");
            var name = Path.GetFileName(fileName); //获取文件名，包含扩展名
            var path = fileName.Replace("/" + name, ""); //获取路劲
            name = HttpUtility.UrlPathEncode(name);
            path = HttpUtility.UrlPathEncode(path.Replace('/', '_'));
            var api = $"libheader/{path}/{name}";
            try
            {
                var result = await HttpClientGet(api);
                if (result != null && result.Length > 0)
                {
                    using (result)
                    using (BinaryReader br = new BinaryReader(result))
                    {
                        MicroLibraryHeader zl = new MicroLibraryHeader(br);
                        if (zl == null || zl.HeaderBytes?.Length == 0)
                        {
                            CEnvir.SaveError($"素材：{api},下载资源失败,原因：服务端返回字节数异常");
                            return false;
                        }

                        var fullname = CEnvir.MobileClientPath + realName;
                        var dir = Path.GetDirectoryName(fullname);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        using (FileStream stream = System.IO.File.Create(fullname))
                        {
                            stream.SetLength(zl.TotalLength);
                            stream.Write(zl.HeaderBytes, 0, zl.HeaderBytes.Length);
                        }
                        return true;
                    }
                }
                else
                    CEnvir.SaveError($"素材：{api},下载资源失败,原因：服务端返回字节数为0");
            }
            catch (Exception ex)
            {
                CEnvir.SaveError($"素材：{api},下载资源异常,异常原因{ex.Message}");
            }

            return false;
        }
        /// <summary>
        /// 获取zl中index资源
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static async Task<byte[]> GetImageAsync(string fileName, int index, int length)
        {
            var realName = fileName;
            fileName = fileName.Replace("./", "");
            fileName = fileName.Replace(".\\", "");
            fileName = fileName.Replace("\\", "/");
            var name = Path.GetFileName(fileName); //获取文件名，包含扩展名
            var path = fileName.Replace("/" + name, ""); //获取路劲
            name = HttpUtility.UrlPathEncode(name);
            path = HttpUtility.UrlPathEncode(path.Replace('/', '_'));
            var api = $"libimage/{path}/{name}/{index}";
            try
            {
                var result = await HttpClientGet(api);
                if (result != null && result.Length > 0)
                {
                    using (result)
                    using (BinaryReader br = new BinaryReader(result))
                    {
                        MicroLibraryImage zl = new MicroLibraryImage(br);
                        if (zl == null || zl.ImageData?.Length != length)
                        {
                            CEnvir.SaveError($"素材：{api},下载资源失败,原因：服务端素材有更新，不匹配。");
                            return null;
                        }

                        if (!SaveImageList.TryGetValue(realName, out var list))
                        {
                            list = new List<MicroLibraryImage>() { zl };
                            SaveImageList[realName] = list;
                        }
                        else
                            SaveImageList[realName].Add(zl);

                        return zl.ImageData;
                    }
                }
                else
                    CEnvir.SaveError($"素材：{api},下载资源失败,原因：服务端返回字节数为0");
            }
            catch (Exception ex)
            {
                CEnvir.SaveError($"素材：{api},下载资源异常,异常原因{ex.Message}");
            }

            return null;
        }

        #region call api

        private static async Task<MemoryStream> HttpClientGet(string url)
        {
            try
            {
                // 发送GET请求，并等待响应
                using (HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    //response.EnsureSuccessStatusCode();
                    //var code = response.StatusCode;
                    // 判断请求是否成功,如果失败则返回 false
                    if (!response.IsSuccessStatusCode)
                    {
                        CEnvir.SaveError($"素材：{url},下载资源异常,异常原因{response.StatusCode}");
                        return null;
                    }

                    // 获取响应流
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        // 获取响应内容长度
                        long totalBytes = response.Content.Headers.ContentLength ?? -1;
                        MemoryStream ms = new MemoryStream();
                        await contentStream.CopyToAsync(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        if (ms.Length == totalBytes)
                            return ms;
                        else
                            CEnvir.SaveError($"素材：{url},下载资源失败,原因：服务端返回字节数异常");
                    }
                }
            }
            catch (Exception ex)
            {
                CEnvir.SaveError($"素材：{url},下载资源异常,异常原因{ex.Message}");
                ExceptionCount++;
            }
            return null;
        }

        public static async Task<bool> DownloadFile(string url, string destFullName)
        {
            try
            {
                // 发送GET请求，并等待响应
                using (HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    //response.EnsureSuccessStatusCode();
                    //var code = response.StatusCode;
                    // 判断请求是否成功,如果失败则返回 false
                    if (!response.IsSuccessStatusCode)
                    {
                        CEnvir.SaveError($"素材：{url},下载资源异常,异常原因{response.StatusCode}");
                        return false;
                    }

                    // 获取响应流
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        // 获取响应内容长度
                        //long totalBytes = response.Content.Headers.ContentLength ?? -1;
                        var dir = Path.GetDirectoryName(destFullName);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        // 创建一个文件流，并将响应内容写入文件流
                        using (FileStream fileStream = new FileStream(destFullName, FileMode.Create, FileAccess.Write/*, FileShare.None, 65536, true*/))
                        {
                            //分块下载 速度慢 可以统计流量信息
                            //long totalBytes = response.Content.Headers.ContentLength ?? -1;
                            //long totalDownloadedBytes = 0;
                            //// 创建一个缓冲区，大小为64KB
                            //byte[] buffer = new byte[65536];
                            //int bytesRead;
                            //// 循环异步读取响应流的内容，直到读取完毕
                            //while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            //{
                            //    // 将读取到的内容写入文件流，并调用进度回调函数
                            //    await fileStream.WriteAsync(buffer, 0, bytesRead);
                            //    totalDownloadedBytes += bytesRead;
                            //}

                            //直接下载，速度快 不考虑统计流量信息
                            await contentStream.CopyToAsync(fileStream);
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                CEnvir.SaveError($"素材：{url},下载资源异常,异常原因{ex.Message}");
                ExceptionCount++;
            }

            return false;
        }

        #endregion
    }

}