using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Server.Scripts
{
    public class CryptionHelper
    {
        static string _iv = ToDES("BlackDragon");
        static string _key = ToDES(Envir.Config.ScriptKey);

        /// <summary>
        /// 将一个不是8位字符串转成8位！！
        /// </summary>
        /// <param name="str"></param>
        public static string ToDES(string str)
        {
            if (str.Length <= 8)
            {
                str = str.ToLower().PadRight(8, 'a');//在字符串右边加a加满8位！！！
                return str;
            }
            else
            {
                str = str.Remove(8);//把第九位以后的字符删完！
                return str;
            }
        }

        /// <summary>
        /// 对文件内容进行DES加密
        /// </summary>
        /// <param name="sourceFile">待加密的文件绝对路径</param>
        /// <param name="destFile">加密后的文件保存的绝对路径</param>
        public static void EncryptFile(string sourceFile, string destFile)
        {
            if (Check(sourceFile))
            {
                return;
            }
            //判断当前的文件是否存在
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("指定的文件路径不存在！", sourceFile);
            //将字符串中的所有字符编码为一个字节序列
            byte[] tmpKey = Encoding.Default.GetBytes(_key);
            //将字符串中的所有字符编码为一个字节序列
            byte[] tmpIV = Encoding.Default.GetBytes(_iv);
            //Crypto:加密 
            //DES:数据加密标准（data encryption standard）  将一些数据加密到内存，然后解密数据
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //读取这个文件的所有内容 内容是字节
            byte[] btFile = File.ReadAllBytes(sourceFile);
            //文件流 使用指定的路径 创建模式和读/写权限初始化文件流的实例
            using (FileStream fs = new FileStream(destFile, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    //定义的加密流对象  CreateEncryptor：用指定的秘钥和初始化向量 创建对称数据加密标准加密对象
                    using (CryptoStream cs = new CryptoStream(fs, des.CreateEncryptor(tmpKey, tmpIV), CryptoStreamMode.Write))
                    {
                        cs.Write(btFile, 0, btFile.Length);
                        cs.FlushFinalBlock();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    fs.Close();
                }
            }
        }
        /// <summary>
        /// 对文件内容进行DES加密，加密后覆盖掉原来的文件
        /// </summary>
        /// <param name="sourceFile">待加密的文件的绝对路径</param>
        static public void EncryptFile(string sourceFile)
        {
            EncryptFile(sourceFile, sourceFile);
        }

        /// <summary>
        /// 对文件内容进行DES解密
        /// </summary>
        /// <param name="sourceFile">待解密的文件绝对路径</param>
        /// <param name="destFile">解密后的文件保存的绝对路径</param>
        public static void DecryptFile(string sourceFile, string destFile)
        {
            if (!Check(sourceFile))
            {
                return;
            }

            //判断文件是否存在
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("指定的文件路径不存在！", sourceFile);

            byte[] btKey = Encoding.Default.GetBytes(_key);
            byte[] btIV = Encoding.Default.GetBytes(_iv);
            //将一些数据加密到内存，然后解密数据
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] btFile = File.ReadAllBytes(sourceFile);

            using (FileStream fs = new FileStream(destFile, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    //CreateDecryptor(tmpIV, tmpIV):用指定的秘钥btKey,和初始化向量
                    //CryptoStream:加密流 fs:目标数据流 
                    // 返回结果: 
                    //     对称 System.Security.Cryptography.DES 解密器对象。
                    using (CryptoStream cs = new CryptoStream(fs, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(btFile, 0, btFile.Length);
                        cs.FlushFinalBlock();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    fs.Close();
                }
            }
        }


        /// <summary>
        /// 对文件内容进行DES解密，解密后覆盖掉原来的文件
        /// </summary>
        /// <param name="sourceFile">待解密的文件的绝对路径</param>
        public static void DecryptFile(string sourceFile)
        {
            DecryptFile(sourceFile, sourceFile);
        }

        /// <summary>
        /// 解密文件返回内容
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        public static string[] DecryptFilelines(string sourceFile)
        {
            sourceFile = GetRuntimeDirectory(sourceFile);
            if (!Check(sourceFile))
            {
                return File.ReadAllLines(sourceFile);
            }

            List<string> list = new List<string>();
            //判断文件是否存在
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("指定的文件路径不存在！", sourceFile);

            byte[] btKey = Encoding.Default.GetBytes(_key);
            byte[] btIV = Encoding.Default.GetBytes(_iv);
            //将一些数据加密到内存，然后解密数据
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] btFile = File.ReadAllBytes(sourceFile);

            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    //CreateDecryptor(tmpIV, tmpIV):用指定的秘钥btKey,和初始化向量
                    //CryptoStream:加密流 fs:目标数据流 
                    // 返回结果: 
                    //     对称 System.Security.Cryptography.DES 解密器对象。
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(btFile, 0, btFile.Length);
                        cs.FlushFinalBlock();

                    }


                    using (var mss = new MemoryStream(ms.ToArray()))
                    {
                        using (StreamReader sr = new StreamReader(mss))
                        {
                            while (true)
                            {
                                var aLine = sr.ReadLine();
                                if (aLine != null)
                                {
                                    list.Add(aLine);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {

                    ms.Close();

                }
            }
            return list.ToArray();
        }

        public static string DecryptFileStr(string sourceFile)
        {
            sourceFile = GetRuntimeDirectory(sourceFile);
            if (!Check(sourceFile))
            {
                return File.ReadAllText(sourceFile);
            }

            var str = string.Empty;
            //判断文件是否存在
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("指定的文件路径不存在！", sourceFile);

            byte[] btKey = Encoding.Default.GetBytes(_key);
            byte[] btIV = Encoding.Default.GetBytes(_iv);
            //将一些数据加密到内存，然后解密数据
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] btFile = File.ReadAllBytes(sourceFile);

            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    //CreateDecryptor(tmpIV, tmpIV):用指定的秘钥btKey,和初始化向量
                    //CryptoStream:加密流 fs:目标数据流 
                    // 返回结果: 
                    //     对称 System.Security.Cryptography.DES 解密器对象。
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(btFile, 0, btFile.Length);
                        cs.FlushFinalBlock();

                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    str = Encoding.Default.GetString(ms.ToArray());
                    ms.Close();

                }
            }
            return str;
        }

        /// <summary>
        /// 验证是否已加密
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        public static bool Check(string sourceFile)
        {
            string fileclass = "";
            using (FileStream fs = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(fs);
                for (int i = 0; i < 2; i++)
                {
                    fileclass += reader.ReadByte().ToString();
                }
            }

            return fileclass == "179225" || fileclass == "116119";

        }

        public static string GetRuntimeDirectory(string path)
        {
            if (IsLinuxRunTime())
                return GetLinuxDirectory(path);
            if (IsWindowRunTime())
                return GetWindowDirectory(path);
            return path;
        }

        public static bool IsWindowRunTime()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public static bool IsLinuxRunTime()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        public static string GetLinuxDirectory(string path)
        {
            return path.Replace("\\", "/");
        }
        public static string GetWindowDirectory(string path)
        {
            return path.Replace("/", "\\");
        }

    }
}
