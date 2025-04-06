using Library;
using Library.Network;
using Microsoft.AspNetCore.Mvc;
using Server.Web.Controllers;
using System.Web;
using SFile = System.IO.File;
namespace ServerWeb.Server.Controllers
{
    [AuthorFilter]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GameController : ControllerBase
    {
        /// <summary>
        /// 用户名验证
        /// </summary>
        public static string Author { get; set; } = "BlackDragon";
        /// <summary>
        /// 移动端版本
        /// </summary>
        public static string AppVersion { get; set; } = "1.0.0";
        /// <summary>
        /// 库文件
        /// </summary>
        public static Dictionary<string, MirLibrary> LibraryList = new Dictionary<string, MirLibrary>();

        private readonly ILogger<GameController> _logger;
        IWebHostEnvironment _environment;
        public GameController(ILogger<GameController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _environment = env;
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        [HttpGet]
        [Route("/api/file/{path}/{name}")]
        public IActionResult GetFile(string path, string name)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(name)) return new NotFoundResult();

            string fullPath = Path.Combine(Globals.ResoucePath, HttpUtility.UrlDecode(path).Replace("_", "/"), HttpUtility.UrlDecode(name)).Replace("\\", "/"); ;
            if (!SFile.Exists(fullPath)) return new NotFoundResult();

            // 读取文件信息
            FileInfo fileMetaInfo = new FileInfo(fullPath);
            // 创建文件读取流
            FileStream fileStream = SFile.OpenRead(fullPath);
            // 根据文件后缀获取文件的ContentType
            //var fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();
            // 返回文件contentType类型
            //var contentType = fileExtensionContentTypeProvider.Mappings[".png"];
            return File(fileStream, "application/octet-stream");
        }
        [HttpGet]
        [Route("/api/sound/{name}/{index?}")]
        public MicroSound? Sound(string name, int? index)
        {
            index = index ?? 1;
            var path = Path.Combine(Globals.ResoucePath, "Sound", name + ".wav");
            if (!SFile.Exists(path))
            {
                return null;
            }

            var bytes = SFile.ReadAllBytes(path);
            var len = 1024 * 1024;

            return new MicroSound
            {
                Bytes = bytes.Skip(len * (index.Value - 1)).Take(len).ToArray(),
                Max = bytes.Length % len == 0 ? bytes.Length / len : (bytes.Length / len + 1),
                Current = index.Value
            };
        }

        /// <summary>
        /// 返回资源文件信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/libimage/{path}/{name}/{index}")]
        public IActionResult? LibraryImage(string name, int index, string path)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(path)) return new NotFoundResult();

            path = HttpUtility.UrlDecode(path).Replace("_", "/");
            name = HttpUtility.UrlDecode(name);
            string fullPath = Path.Combine(Globals.ResoucePath, path, name).Replace("\\", "/"); ;
            if (!SFile.Exists(fullPath)) return new NotFoundResult();

            var key = Path.Combine(path, name).Replace("\\", "/");
            if (LibraryList.TryGetValue(key, out var library))
            {
                if (library != null)
                {
                    var image = library.GetImage(index);
                    if (image != null && image.Length > 0)
                        return File(image, "application/octet-stream");
                }
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// 返回头信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/libheader/{path}/{name}")]
        public IActionResult Library(string name, string path)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(path)) return new NotFoundResult();

            path = HttpUtility.UrlDecode(path).Replace("_", "/");
            name = HttpUtility.UrlDecode(name);
            string fullPath = Path.Combine(Globals.ResoucePath, path, name).Replace("\\", "/");
            if (!SFile.Exists(fullPath)) return new NotFoundResult();

            var key = Path.Combine(path, name).Replace("\\", "/");
            if (LibraryList.TryGetValue(key, out var library))
            {
                if (library != null)
                {
                    var libraryHeader = library.GetLibraryHeader();
                    if (libraryHeader != null && libraryHeader.Length > 0)
                        return File(libraryHeader, "application/octet-stream");
                }
            }
            return new NotFoundResult();
        }

    }
}
