using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace PatchManager
{
    /// <summary>
    /// 补丁信息
    /// </summary>
    public sealed class PatchInformation
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        public string FullFileName { get; set; }
        /// <summary>
        /// 压缩长度
        /// </summary>
        public long CompressedLength { get; set; }
        /// <summary>
        /// 求和效验
        /// </summary>
        public byte[] CheckSum { get; set; }
        /// <summary>
        /// 补丁信息
        /// </summary>
        public PatchInformation() { }
        /// <summary>
        /// 补丁信息 文件名
        /// </summary>
        /// <param name="fileName"></param>
        public PatchInformation(string fullFileName, string fileName)
        {
            FullFileName = fullFileName;
            FileName = fileName;

            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(fullFileName))
                    CheckSum = md5.ComputeHash(stream);
            }
        }
        /// <summary>
        /// 补丁信息 二进制阅读器
        /// </summary>
        /// <param name="reader"></param>
        public PatchInformation(BinaryReader reader)
        {
            FileName = reader.ReadString();
            CompressedLength = reader.ReadInt64();

            CheckSum = reader.ReadBytes(reader.ReadInt32());
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="writer"></param>
        public void Save(BinaryWriter writer)
        {
            writer.Write(FileName);
            writer.Write(CompressedLength);
            writer.Write(CheckSum.Length);
            writer.Write(CheckSum);
        }
    }

    /// <summary>
    /// android应用程序信息
    /// </summary>
    public class AndroidInfo
    {
        public string Name { get; set; }

        public List<AndroidSetting> Settings { get; set; }
    }

    /// <summary>
    /// 设置
    /// </summary>
    public class AndroidSetting
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
