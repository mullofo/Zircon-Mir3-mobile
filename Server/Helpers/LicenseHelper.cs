using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Server.Envir;
using Reactor = License;

namespace Server.Helpers
{
    public static class LicenseHelper
    {
        static LicenseHelper()
        {
            ReLoadLicense(null);
        }
        public static bool ReLoadLicense(string path)
        {
            //SEnvir.Log("重新加载授权");
            if (string.IsNullOrEmpty(path))
            {
                path = Config.LicenseFile;
            }

            if (!File.Exists(path))
            {
                return false;
            }

            //加载授权文件
            Reactor.Status.LoadLicense(path);
            //SEnvir.Log($"授权加载完毕");
            return true;
        }

        public static LicenseState CheckLicense()
        {
            if (Reactor.Status.Expired)
            {
                return LicenseState.Expired;
            }

            if (Reactor.Status.Hardware_Lock_Enabled && CurrentHardwareID != LicenseHardwareID)
            {
                return LicenseState.HardwareNotMatched;
            }

            if (Reactor.Status.Evaluation_Lock_Enabled &&
                Reactor.Status.Evaluation_Time_Current > Reactor.Status.Evaluation_Time)
            {
                return LicenseState.EvaluationExpired;
            }

            if (Reactor.Status.Number_Of_Uses_Lock_Enable &&
                Reactor.Status.Number_Of_Uses_Current > Reactor.Status.Number_Of_Uses)
            {
                return LicenseState.UsesExceeded;
            }
#if DEBUG
            return LicenseState.DebugMode;
#else
            return Reactor.Status.Licensed ? LicenseState.Licensed : LicenseState.Invalid;
#endif
        }

        public static bool IsLicensed => Reactor.Status.Licensed;
        public static string CurrentHardwareID => Reactor.Status.HardwareID;
        public static string LicenseHardwareID => Reactor.Status.License_HardwareID;
        public static bool IsEvaluation => Reactor.Status.Evaluation_Lock_Enabled;
        public static string EvaluationType =>
            Reactor.Status.Evaluation_Type == Reactor.EvaluationType.Trial_Days ? "按天试用" : "按运行时长试用";
        public static int EvaluationRemainingTime =>
            Reactor.Status.Evaluation_Time - Reactor.Status.Evaluation_Time_Current + 1;

        public static bool HardwareLockEnabled => Reactor.Status.Hardware_Lock_Enabled;
        public static bool HardwareMatch => CurrentHardwareID == LicenseHardwareID;


        public static Dictionary<string, string> GetAdditionalInfo()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!Reactor.Status.Licensed) return result;

            for (int i = 0; i < Reactor.Status.KeyValueList.Count; i++)
            {
                result[Reactor.Status.KeyValueList.GetKey(i).ToString()] =
                    Reactor.Status.KeyValueList.GetByIndex(i).ToString();
            }

            return result;
        }

        public static string InvalidateLicense()
        {
            return Reactor.Status.InvalidateLicense();
        }

        public static bool CheckConfirmationCode(string confirmation)
        {
            return Reactor.Status.CheckConfirmationCode(Reactor.Status.HardwareID,
                confirmation);
        }

        public static bool ReactivateLicense(string code)
        {
            return Reactor.Status.ReactivateLicense(code);
        }
    }
}

namespace Server
{

    /// <summary>
    /// 授权状态
    /// </summary>
    public enum LicenseState
    {
        [Description("无效")]
        Invalid = 0,
        [Description("找不到授权文件")]
        Missing,
        [Description("已过期")]
        Expired,
        [Description("硬件码不符")]
        HardwareNotMatched,
        [Description("试用到期")]
        EvaluationExpired,
        [Description("超过使用次数")]
        UsesExceeded,

        [Description("有效")]
        Licensed = 101,
        [Description("Debug模式无需授权")]
        DebugMode = 102
    }
}