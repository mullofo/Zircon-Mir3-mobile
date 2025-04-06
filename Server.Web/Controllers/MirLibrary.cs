using Library;

namespace ServerWeb.Server.Controllers
{
    /// <summary>
    /// 库文件
    /// </summary>
    public sealed class MirLibrary : IDisposable
    {
        public readonly object LoadLocker = new object();
        //版本信息，注意只能20个字节，VersionString 19字节, LibVersion 1字节
        public string VersionString = "BlackDragon Version";
        public const byte LibVersion = 3;
        //加密相关
        public bool Crypt;
        public byte[] Key = new byte[8];
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName;
        /// <summary>
        /// 文件流
        /// </summary>
        private FileStream _FStream;
        /// <summary>
        /// 二进制阅读器
        /// </summary>
        private BinaryReader _BReader;

        public bool Loaded, Loading;
        /// <summary>
        /// 图片
        /// </summary>
        public MirImage[] Images;

        public byte[] Header;


        /// <summary>
        /// 库文件
        /// </summary>
        /// <param name="fileName"></param>
        public MirLibrary(string fileName)
        {
            _FStream = File.OpenRead(fileName);
            _BReader = new BinaryReader(_FStream);
            FileName = fileName;

            //头缓存进内存
            ReadLibrary();
        }
        /// <summary>
        /// 读取库文件
        /// </summary>
        public void ReadLibrary()
        {
            try
            {
                lock (LoadLocker)
                {
                    if (Loading) return;
                    Loading = true;

                    if (_BReader == null)
                    {
                        Loaded = true;
                        return;
                    }

                    //读取版本信息
                    var currentString = System.Text.Encoding.Default.GetString(_BReader.ReadBytes(19)).TrimEnd('\0');
                    var byt = _BReader.ReadByte();
                    //最高位代表是否加密
                    Crypt = Convert.ToBoolean(byt & 0x80);
                    var currentVersion = byt & 0x7F;
                    //兼容Zircon格式
                    bool isZirconVersion = currentString != VersionString;

                    byte[] buffer = null;
                    if (isZirconVersion)
                    {
                        _BReader.BaseStream.Seek(0, SeekOrigin.Begin);
                        buffer = _BReader.ReadBytes(_BReader.ReadInt32());
                    }
                    else if (currentVersion > 3)
                    {
                        //MessageBox.Show("版本错误, 应为黑龙专用版本: " + LibVersion.ToString() + " 发现版本: " + currentVersion.ToString() + ".", "打开失败", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    else
                    {
                        if (currentVersion == 1 || !Crypt)
                            buffer = _BReader.ReadBytes(_BReader.ReadInt32());
                        else
                        {
                            int bufferLength = _BReader.ReadInt32(); //长度作为加密随机盐值
                            byte[] temp = _BReader.ReadBytes(bufferLength);
                            Key = Decode(_BReader.ReadBytes(8), System.Text.Encoding.ASCII.GetBytes(VersionString), 8);  //8个字节为key
                            buffer = Decode(temp, Key, (byte)(bufferLength % 0xFF));
                        }
                    }

                    if (buffer == null) return;
                    using (MemoryStream mstream = new MemoryStream(buffer))
                    using (BinaryReader reader = new BinaryReader(mstream))
                    {
                        Images = new MirImage[reader.ReadInt32()];

                        for (int i = 0; i < Images.Length; i++)
                        {
                            //判断图片格式
                            byte imageTextureType = reader.ReadByte();
                            if (imageTextureType == 0) continue;
                            Images[i] = new MirImage(reader, currentVersion, isZirconVersion) { ImageTextureType = imageTextureType, };
                        }
                    }

                    //头数据缓存进内存 微端使用
                    long headerLength = _BReader.BaseStream.Position;
                    _BReader.BaseStream.Seek(0, SeekOrigin.Begin);
                    using (MemoryStream ms = new MemoryStream())
                    using (BinaryWriter writer = new BinaryWriter(ms))
                    {
                        writer.Write(_BReader.BaseStream.Length); //文件总长度
                        writer.Write((int)headerLength);  //文件头长度
                        writer.Write(_BReader.ReadBytes((int)headerLength)); //文件头数组
                        Header = ms.ToArray();
                    }

                    Loaded = true;
                }
            }
            catch
            { }
        }

        //解密数据
        private byte[] Decode(byte[] content, byte[] key, byte salt)
        {
            byte dkey = 0;
            for (int i = 0; i < key.Length; i++)
            {
                dkey ^= key[i];
            }

            byte tempsalt = salt;
            byte[] result = new byte[content.Length];
            //从后向前解密，因为我们初始值的盐值是0，所以这里当第一字节解码时盐值也应该是0
            for (int i = content.Length - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    salt = tempsalt;
                }
                else
                {
                    salt = content[i - 1];
                }
                //密文^dkey^salt 就的到原文
                result[i] = (byte)(content[i] ^ dkey ^ salt);
            }
            return result;
        }

        public byte[] GetLibraryHeader()
        {
            if (!Loaded) ReadLibrary();

            //while (!Loaded)
            //    Thread.Sleep(1);

            return Header;
        }
        /// <summary>
        /// 创建图像
        /// </summary>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public byte[] GetImage(int index)
        {
            if (!CheckImage(index)) return null;

            MirImage image = Images[index];

            if (!image.ImageValid) image.CreateImage(_BReader);

            if (image.Image == null) return null;

            return image.Image;
        }
        /// <summary>
        /// 检查图片
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool CheckImage(int index)
        {
            if (!Loaded) ReadLibrary();

            //while (!Loaded)
            //    Thread.Sleep(1);

            return index >= 0 && index < Images.Length && Images[index] != null;
        }

        #region IDisposable Support

        public bool IsDisposed { get; private set; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposed = true;

                foreach (MirImage image in Images)
                    image.Dispose();


                Images = null;


                _FStream?.Dispose();
                _FStream = null;

                _BReader?.Dispose();
                _BReader = null;

                Loading = false;
                Loaded = false;
            }

        }

        ~MirLibrary()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /// <summary>
    /// 图片
    /// </summary>
    public sealed class MirImage : IDisposable
    {
        /// <summary>
        /// 坐标
        /// </summary>
        public int Position;

        public byte ImageTextureType;
        /// <summary>
        /// 有效图像
        /// </summary>
        public bool ImageValid { get; private set; }
        /// <summary>
        /// 图像数据大小
        /// </summary>
        public int ImageDataSize;

        public byte ShadowTextureType;
        /// <summary>
        /// 阴影数据大小
        /// </summary>
        public int ShadowDataSize;

        public byte OverlayTextureType;
        /// <summary>
        /// 叠加数据大小
        /// </summary>
        public int OverlayDataSize;

        public byte[] Image;
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireTime;

        /// <summary>
        /// 图片二进制阅读器
        /// </summary>
        /// <param name="reader"></param>
        public MirImage(BinaryReader reader, int version, bool isZirconVersion = false)
        {
            //IsZirconVersion = isZirconVersion;
            //Version = version;

            Position = reader.ReadInt32();

            //如果黑龙版
            if (!isZirconVersion)
            {
                ImageDataSize = reader.ReadInt32();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadByte();
                ShadowTextureType = reader.ReadByte();
                OverlayTextureType = reader.ReadByte();

                if (ShadowTextureType > 0)
                {
                    ShadowDataSize = reader.ReadInt32();
                    reader.ReadInt16();
                    reader.ReadInt16();
                }

                if (OverlayTextureType > 0)
                {
                    OverlayDataSize = reader.ReadInt32();
                    if (version > 1)
                    {
                        reader.ReadInt16();
                        reader.ReadInt16();
                    }
                }
            }
            //如果zircon版
            else
            {
                int Width = reader.ReadInt16();
                int Height = reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();

                reader.ReadByte();
                int ShadowWidth = reader.ReadInt16();
                int ShadowHeight = reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();

                int OverlayWidth = reader.ReadInt16();
                int OverlayHeight = reader.ReadInt16();

                int w = Width + (4 - Width % 4) % 4;
                int h = Height + (4 - Height % 4) % 4;
                ImageDataSize = w * h / 2;

                w = ShadowWidth + (4 - ShadowWidth % 4) % 4;
                h = ShadowHeight + (4 - ShadowHeight % 4) % 4;
                ShadowDataSize = w * h / 2;

                w = OverlayWidth + (4 - OverlayWidth % 4) % 4;
                h = OverlayHeight + (4 - OverlayHeight % 4) % 4;
                OverlayDataSize = w * h / 2;

                ImageTextureType = 1;
                ShadowTextureType = 1;
                OverlayTextureType = 1;
            }
        }

        /// <summary>
        /// 处理纹理
        /// </summary>
        public unsafe void DisposeTexture()
        {
            Image = null;

            ImageValid = false;

            ExpireTime = DateTime.MinValue;
        }
        /// <summary>
        /// 创建图像
        /// </summary>
        /// <param name="reader"></param>
        public void CreateImage(BinaryReader reader)
        {
            if (Position == 0) return;

            try
            {
                lock (reader)
                {
                    int length = ImageDataSize + ShadowDataSize + OverlayDataSize;
                    reader.BaseStream.Seek(Position, SeekOrigin.Begin);
                    using (MemoryStream ms = new MemoryStream())
                    using (BinaryWriter writer = new BinaryWriter(ms))
                    {
                        writer.Write(Position);
                        writer.Write(length);
                        writer.Write(reader.ReadBytes(length));
                        Image = ms.ToArray();
                    }
                    ImageValid = true;
                    ExpireTime = Time.Now + TimeSpan.FromMinutes(10);
                }
            }
            catch
            {
                Image = null;
            }
        }


        #region IDisposable Support

        public bool IsDisposed { get; private set; }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposed = true;

                Position = 0;

            }
        }

        public void Dispose()
        {
            Dispose(!IsDisposed);
            GC.SuppressFinalize(this);
        }
        ~MirImage()
        {
            Dispose(false);
        }

        #endregion

    }


}
