using System;
using System.Collections.Concurrent;

namespace Server.Envir
{
    public static class SEnvir
    {
        /// <summary>
        /// 并发队列 显示日志
        /// </summary>
        public static ConcurrentQueue<string> DisplayLogs = new ConcurrentQueue<string>();

        public static DateTime Now => DateTime.Now;
        public static DateTime DiyShowViewerLoopTime;
        public static bool LicenseValid;

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="log"></param>
        /// <param name="hardLog">是否追加至日志文件</param>
        public static void Log(string log)
        {
            log = string.Format("[{0:F}]: {1}", DateTime.Now, log);

            DisplayLogs.Enqueue(log);
        }
    }
}
