using System.IO;

namespace Mir3.Mobile
{
    public interface INative
    {
        /// <summary>
        /// 隐藏输入框
        /// </summary>
        void HideInputField();
        /// <summary>
        /// 显示输入框
        /// </summary>
        /// <param name="text"></param>
        void ShowInputField(string text);

        /// <summary>
        /// 打开网页
        /// </summary>
        /// <param name="url"></param>
        void OpenUrl(string url);

        /// <summary>
        /// 获取文件字节
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        byte[] GetFileBytes(string path);

        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Stream GetFileStream(string path);

        /// <summary>
        /// 是否平板
        /// </summary>
        bool IsPad { get; }

        /// <summary>
        /// 初始化
        /// 根据手机分辨率和dpi设置客户端缩放比率
        /// </summary>
        void Initialize();

        /// <summary>
        /// 解压Data.zip
        /// </summary>
        void InitData();

#if ANDROID
        /// <summary>
        /// 更新apk
        /// </summary>
        bool UpdateAPKVersion(string filename);
#endif

        /// <summary>
        /// web调用素材安全码
        /// </summary>
        string SafeCode { get; }

        IUI UI { get; }
    }
}

