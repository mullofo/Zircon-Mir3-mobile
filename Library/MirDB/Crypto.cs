using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Library.MirDB.Crypto
{
    //todo 目前只有AES，RSA待添加

    /// <summary>
    /// 加密
    /// </summary>
    public static class Crypto
    {
        /// <summary>
        /// 对称加密
        /// </summary>
        public static class AesUtils
        {
            /// <summary>
            /// 密匙大小
            /// </summary>
            public const int KeySize = 256;
            /// <summary>
            /// 密匙大小字节
            /// </summary>
            public const int KeySizeBytes = 256 / 8;
            /// <summary>
            /// 字区大小
            /// </summary>
            public const int BlockSize = 128;
            /// <summary>
            /// 字区大小字节
            /// </summary>
            public const int BlockSizeBytes = 128 / 8;
            /// <summary>
            /// 默认缓冲区大小
            /// </summary>
            public const int DefaultBufferSize = 8 * 1024;

            /// <summary>
            /// 对称算法
            /// </summary>
            /// <returns></returns>
            public static SymmetricAlgorithm CreateSymmetricAlgorithm()
            {
                var aes = Aes.Create();
                aes.KeySize = KeySize;
                aes.BlockSize = BlockSize;
                aes.Mode = CipherMode.CBC;//不要使用ECB模式,不安全
                aes.Padding = PaddingMode.PKCS7;
                return aes;
            }

            /// <summary>
            /// 将一个stream的数据完整复制到另一个
            /// buffer作为中转
            /// </summary>
            public static long CopyTo(Stream input, Stream output, byte[] buffer)
            {
                if (buffer == null)
                    throw new ArgumentNullException(nameof(buffer));

                if (input == null)
                    throw new ArgumentNullException(nameof(input));

                if (output == null)
                    throw new ArgumentNullException(nameof(output));

                if (buffer.Length == 0)
                    throw new ArgumentException("buffer大小为0");

                long total = 0;
                int read;
                // 使用未加密的数据库，或者密钥不对，这里会使程序崩溃
                try
                {
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, read);
                        total += read;
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException("数据库秘钥错误", e);
                }
                return total;
            }

            //完整读取stream内容 存入byte[]并返回
            public static byte[] ReadFully(Stream input, int bufferSize)
            {
                if (bufferSize < 1)
                    throw new ArgumentOutOfRangeException(nameof(bufferSize));

                byte[] buffer = new byte[bufferSize];
                if (buffer == null)
                    throw new ArgumentNullException(nameof(buffer));

                if (input == null)
                    throw new ArgumentNullException(nameof(input));

                if (buffer.Length == 0)
                    throw new ArgumentException("buffer大小为0");

                // We could do all our own work here, but using MemoryStream is easier
                // and likely to be just as efficient.
                using (var tempStream = new MemoryStream())
                {
                    CopyTo(input, tempStream, buffer);
                    // No need to copy the buffer if it's the right size
                    if (tempStream.Length == tempStream.GetBuffer().Length)
                    {
                        return tempStream.GetBuffer();
                    }
                    // Okay, make a copy that's the right size
                    return tempStream.ToArray();
                }
            }

            /// <summary>
            /// 创建密匙
            /// </summary>
            /// <returns></returns>
            public static byte[] CreateKey()
            {
                using (var aes = CreateSymmetricAlgorithm())
                {
                    return aes.Key;
                }
            }
            /// <summary>
            /// 创建增值
            /// </summary>
            /// <returns></returns>
            public static byte[] CreateIv()
            {
                using (var aes = CreateSymmetricAlgorithm())
                {
                    return aes.IV;
                }
            }
            /// <summary>
            /// 创建密匙与增值
            /// </summary>
            /// <param name="cryptKey"></param>
            /// <param name="iv"></param>
            public static void CreateKeyAndIv(out byte[] cryptKey, out byte[] iv)
            {
                using (var aes = CreateSymmetricAlgorithm())
                {
                    cryptKey = aes.Key;
                    iv = aes.IV;
                }
            }
            /// <summary>
            /// 创建加密身份验证密匙与增值
            /// </summary>
            /// <param name="cryptKey"></param>
            /// <param name="authKey"></param>
            /// <param name="iv"></param>
            public static void CreateCryptAuthKeysAndIv(out byte[] cryptKey, out byte[] authKey, out byte[] iv)
            {
                using (var aes = CreateSymmetricAlgorithm())
                {
                    cryptKey = aes.Key;
                    iv = aes.IV;
                }
                using (var aes = CreateSymmetricAlgorithm())
                {
                    authKey = aes.Key;
                }
            }
            /// <summary>
            /// 加密
            /// </summary>
            /// <param name="text"></param>
            /// <param name="cryptKey"></param>
            /// <param name="iv"></param>
            /// <returns></returns>
            public static string Encrypt(string text, byte[] cryptKey, byte[] iv)
            {
                var encBytes = Encrypt(System.Text.Encoding.UTF8.GetBytes(text), cryptKey, iv);
                return Convert.ToBase64String(encBytes);
            }

            public static byte[] Encrypt(byte[] bytesToEncrypt, byte[] cryptKey, byte[] iv)
            {
                using (var aes = CreateSymmetricAlgorithm())
                using (var encrypter = aes.CreateEncryptor(cryptKey, iv))
                using (var cipherStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(cipherStream, encrypter, CryptoStreamMode.Write))
                    using (var binaryWriter = new BinaryWriter(cryptoStream))
                    {
                        binaryWriter.Write(bytesToEncrypt);
                    }
                    return cipherStream.ToArray();
                }
            }

            /// <summary>
            /// 解密
            /// </summary>
            /// <param name="encryptedBase64"></param>
            /// <param name="cryptKey"></param>
            /// <param name="iv"></param>
            /// <returns></returns>
            public static string Decrypt(string encryptedBase64, byte[] cryptKey, byte[] iv)
            {
                var bytes = Decrypt(Convert.FromBase64String(encryptedBase64), cryptKey, iv);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }

            public static byte[] Decrypt(byte[] encryptedBytes, byte[] cryptKey, byte[] iv)
            {
                using (var aes = CreateSymmetricAlgorithm())
                using (var decryptor = aes.CreateDecryptor(cryptKey, iv))

                using (var ms = new MemoryStream(encryptedBytes, 0, encryptedBytes.Length, writable: true, publiclyVisible: true))
                using (var cryptStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    return ReadFully(cryptStream, DefaultBufferSize);
                }
            }
        }

        // XOR加密
        // 进行一次XOR 拿到加密过的byte[]
        // 再进行一次XOR 就拿到原文了
        public static class XORUtils
        {
            public static string Base64Encode(string original)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(original);
                return Convert.ToBase64String(bytes);
            }

            public static string Base64Decode(string base64)
            {
                byte[] bytes = System.Convert.FromBase64String(base64);
                return Encoding.UTF8.GetString(bytes);
            }

            public static byte[] XOR(byte[] payload, string XORKey)
            {
                byte[] xorStuff = new byte[payload.Length];
                string base64Key = Base64Encode(XORKey);
                char[] bXORKey = base64Key.ToCharArray();
                for (int i = 0; i < payload.Length; i++)
                {
                    xorStuff[i] = (byte)(payload[i] ^ bXORKey[i % bXORKey.Length]);
                }
                return xorStuff;
            }
        }

        public static class MD5Utils
        {
            public static string GetFileMD5(string filename)
            {
                if (!File.Exists(filename)) return string.Empty;
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filename))
                    {
                        var hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
        }
    }
}
