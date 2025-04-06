using Android.Bluetooth;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Java.Lang;
using System.IO;

namespace Mir3.Droid.Helpers
{
    public class VerifyDevice
    {

        /// <summary>
        /// 验证是否为模拟器
        /// </summary>
        /// <returns></returns>
        public static bool Verify()
        {
            //蓝牙、光感去除 模拟器可以模拟
            //return NotHasBlueTooth()
            //    || NotHasLightSensorManager()
            //    || CheckEmulator()
            //    || CheckIsNotRealPhone();
            return CheckIsNotRealPhone() || CheckEmulator();
        }

        /// <summary>
        /// 判断蓝牙是否有效
        /// </summary>
        /// <returns></returns>
        private static bool NotHasBlueTooth()
        {
            string name = string.Empty;
            try
            {
                var manager = MainActivity.Main.ApplicationContext.GetSystemService(Context.BluetoothService) as BluetoothManager;
                var adapter = manager.Adapter;
                name = adapter?.Name;
            }
            catch
            {
                return false;
            }
            return string.IsNullOrEmpty(name);
        }
        /// <summary>
        /// 依据是否存在 光传感器 来判断是否为模拟器
        /// </summary>
        /// <returns></returns>
        private static bool NotHasLightSensorManager()
        {
            try
            {
                var manager = MainActivity.Main.ApplicationContext.GetSystemService(Context.SensorService) as SensorManager;
                var senor = manager?.GetDefaultSensor(SensorType.Light);
                return senor == null;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 根据部分特征参数设备信息来判断是否为模拟器
        /// </summary>
        /// <returns></returns>
        private static bool CheckEmulator()
        {
            //厂商判断
            var emulator = false;
            var manufacturers = new string[] { "unknown", "genymotion", "yeshen", "mumu", "ldmnq" };//TODO加入更多模拟器厂商
            var manufacturer = Build.Manufacturer.ToLower();
            foreach (var manu in manufacturers)
            {
                if (manufacturer.Contains(manu))
                {
                    emulator = true;
                    break;
                }
            }

            return emulator
                || Build.Fingerprint.StartsWith("generic")
                || Build.Fingerprint.ToLower().Contains("emulator")
                || Build.Fingerprint.ToLower().Contains("vbox")
                || Build.Fingerprint.ToLower().Contains("test-keys")
                || Build.Model.Contains("google_sdk")
                || Build.Model.Contains("Emulator")
                || Build.Model.Contains("Android SDK built for x86")
                || Build.Model.Contains("Android SDK built for arm64")
                || (Build.Brand.StartsWith("generic") && Build.Device.StartsWith("generic"))
                || Build.Product == "google_sdk";
        }

        /// <summary>
        /// 根据CPU信息判断是否为真机
        /// </summary>
        /// <returns></returns>
        private static bool CheckIsNotRealPhone()
        {
            var cpu = string.Empty;
            try
            {
                var args = new string[] { "/system/bin/cat", "/proc/cpuinfo" };
                var processBuild = new ProcessBuilder(args);
                var process = processBuild.Start();
                var stream = process.InputStream;
                if (stream == null)
                    return true;
                using (stream)
                using (var read = new StreamReader(stream))
                {
                    cpu = read.ReadToEnd().ToLower();
                }
            }
            catch
            {
                return false;
            }

            return cpu.Contains("intel") || cpu.Contains("amd");
        }

    }
}

