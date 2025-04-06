using ManagedSquish;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LibraryEditor
{
    public enum ImageType
    {
        Image,
        Shadow,
        Overlay,
    }

    /// <summary>
    /// 黑龙专用Library(兼容Zircon)
    /// Uses ARGB32 Images
    /// </summary>
    public sealed class BlackDragonLibrary
    {
        public string FileName;
        public string _fileName;
        //版本信息，注意只能20个字节，VersionString 19字节, LibVersion 1字节
        public string VersionString = "BlackDragon Version";
        public const byte LibVersion = 3;
        //加密相关
        public bool Crypt;
        public byte[] Key = new byte[8];

        private FileStream _fStream;
        private BinaryReader _bReader;

        public List<MImage> Images;

        public bool MoblieLib;

        public BlackDragonLibrary(string fileName, bool moblie = false)
        {
            FileName = fileName;
            MoblieLib = moblie;
            _fileName = Path.ChangeExtension(fileName, null);
            Images = new List<MImage>();
            if (!File.Exists(fileName))
                return;

            _fStream = File.OpenRead(fileName);
            _bReader = new BinaryReader(_fStream);

            ReadLibrary();
            Close();
        }
        public void ReadLibrary()
        {
            if (_bReader == null)
                return;

            //读取版本信息
            var currentString = System.Text.Encoding.Default.GetString(_bReader.ReadBytes(19)).TrimEnd('\0');
            var byt = _bReader.ReadByte();
            //最高位代表是否加密
            Crypt = Convert.ToBoolean(byt & 0x80);
            var currentVersion = byt & 0x7F;
            //兼容Zircon格式
            bool isZirconVersion = currentString != VersionString;

            byte[] buffer = null;
            if (isZirconVersion)
            {
                _bReader.BaseStream.Seek(0, SeekOrigin.Begin);
                buffer = _bReader.ReadBytes(_bReader.ReadInt32());
            }
            else if (currentVersion > 3)
            {
                MessageBox.Show("版本错误, 应为黑龙专用版本: " + LibVersion.ToString() + " 发现版本: " + currentVersion.ToString() + ".", "打开失败", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            else
            {
                if (currentVersion == 1 || !Crypt)
                    buffer = _bReader.ReadBytes(_bReader.ReadInt32());
                else
                {
                    int bufferLength = _bReader.ReadInt32(); //长度作为加密随机盐值
                    byte[] temp = _bReader.ReadBytes(bufferLength);
                    Key = Decode(_bReader.ReadBytes(8), System.Text.Encoding.ASCII.GetBytes(VersionString), 8);  //8个字节为key
#if RELEASE
                    if (new CryptForm().ShowDialog() != DialogResult.OK) return;
                    if (System.Text.Encoding.ASCII.GetString(Key).TrimEnd('\0') != CryptForm.form1.keyTextBox.Text)
                    {
                        MessageBox.Show("密钥错误！！！", "错误", MessageBoxButtons.OK);
                        return;
                    }
#endif
                    buffer = Decode(temp, Key, (byte)(bufferLength % 0xFF));
                }
            }

            using (MemoryStream mstream = new MemoryStream(buffer))  //读取图片头索引数据
            using (BinaryReader reader = new BinaryReader(mstream))
            {
                //读取图片数量
                int count = reader.ReadInt32();

                //初始化Images数组
                for (int i = 0; i < count; i++)
                    Images.Add(null);

                LMain.form1.toolStripProgressBar.Value = 0;
                LMain.form1.toolStripProgressBar.Maximum = Images.Count;
                //读取图片头信息
                for (int i = 0; i < Images.Count; i++)
                {
                    //判断图片格式
                    byte imageTextureType = reader.ReadByte();
                    if (imageTextureType == 0) continue;

                    Images[i] = new MImage(reader, currentVersion, isZirconVersion) { ImageTextureType = imageTextureType, };

                    if (Images[i].Width == 1 && Images[i].Height == 1)
                        Images[i] = null;
                    //把图片原始数据load进内存供实时加载
                    else
                        Images[i].LoadImageData(_bReader, MoblieLib);

                    LMain.form1.toolStripProgressBar.Value = i;
                }

                LMain.form1.toolStripProgressBar.Value = 0;
            }
        }

        //加密过程
        private byte[] Encode(byte[] content, byte[] key, byte salt)
        {
            //初始值给一个0，因为0^ 任何值都等于任何值
            byte dkey = 0;
            for (int i = 0; i < key.Length; i++)
            {
                dkey ^= key[i];
            }

            //byte salt = 12;  //随机盐值
            //创建Byte数组来存储加密后的ASCII码
            byte[] result = new byte[content.Length];
            for (int i = 0; i < content.Length; i++)
            {

                salt = (byte)(content[i] ^ dkey ^ salt);
                //原文对应ASCII码加密时除第一个使用初始化值0，其他的使用的都是原文对应的ASCII码来做盐值
                result[i] = salt;
            }
            return result;
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
        public void Close()
        {
            if (_fStream != null)
                _fStream.Dispose();
            if (_bReader != null)
                _bReader.Dispose();
        }

        public void Dispose()
        {
            Images?.Clear();
            Images = null;
            GC.Collect();
            GC.SuppressFinalize(this);
        }

        public MImage CreateImage(int index, ImageType type)
        {
            if (!CheckImage(index)) return null;

            MImage image = Images[index];
            Bitmap bmp;

            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage();
                    bmp = image.Image;
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateShadow();
                    bmp = image.ShadowImage;
                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateOverlay();
                    bmp = image.OverlayImage;
                    break;
                default:
                    return null;
            }

            if (bmp == null) return null;

            return image;
        }

        private bool CheckImage(int index)
        {
            return index >= 0 && index < Images.Count && Images[index] != null;
        }

        //获取大图图像，解码大图
        public MImage GetImage(int index)
        {
            if (index < 0 || index >= Images.Count)
                return null;

            MImage image = Images[index];
            //解码大图
            if (image != null)
            {
                CreateImage(index, ImageType.Image);
                CreateImage(index, ImageType.Shadow);
                CreateImage(index, ImageType.Overlay);
            }

            return image;
        }

        public Bitmap GetPreview(int index, ImageType type)
        {
            if (index < 0 || index >= Images.Count)
                return new Bitmap(1, 1);

            MImage image = GetImage(index);

            switch (type)
            {
                case ImageType.Image:
                    if (image == null || image.Image == null)
                        return new Bitmap(1, 1);

                    if (image.Preview == null)
                        image.CreatePreview();

                    return image.Preview;
                case ImageType.Shadow:
                    if (image == null || image.ShadowImage == null)
                        return new Bitmap(1, 1);

                    if (image.ShadowPreview == null)
                        image.CreateShadowPreview();

                    return image.ShadowPreview;
                case ImageType.Overlay:
                    if (image == null || image.OverlayImage == null)
                        return new Bitmap(1, 1);

                    if (image.OverlayPreview == null)
                        image.CreateOverlayPreview();

                    return image.OverlayPreview;
            }

            return new Bitmap(1, 1);
        }

        public void RemoveBlanks(bool safe = false)
        {
            for (int i = Images.Count - 1; i >= 0; i--)
            {
                if (Images[i] == null)
                {
                    if (!safe)
                        RemoveImage(i);
                    else if (Images[i].OffSetX == 0 && Images[i].OffSetY == 0)
                        RemoveImage(i);
                }
            }
        }

        public void RemoveImage(int index)
        {
            if (Images == null || Images.Count <= 1)
            {
                Images = new List<MImage>();
                return;
            }

            Images.RemoveAt(index);
        }

        public void AddImage(Bitmap image, Bitmap shadowImage, Bitmap overlayImage, byte imageTextureType, byte shadowTextureType, byte overlayTextureType,
                             short width, short height, short offSetX, short offSetY, short shadowOffSetX, short shadowOffSetY, short shadow,
                             short shadowWidth, short shadowHeight, short overlayWidth, short overlayHeight)
        {
            MImage mImage = new MImage(image, shadowImage, overlayImage, imageTextureType, shadowTextureType, overlayTextureType)
            {
                Width = width,
                Height = height,
                OffSetX = offSetX,
                OffSetY = offSetY,
                ShadowOffSetX = shadowOffSetX,
                ShadowOffSetY = shadowOffSetY,
                Shadow = (byte)shadow,

                ShadowWidth = shadowWidth,
                ShadowHeight = shadowHeight,
                OverlayWidth = overlayWidth,
                OverlayHeight = overlayHeight,
            };

            Images.Add(mImage);
        }

        public void ReplaceImage(int index, Bitmap image, Bitmap shadowImage, Bitmap overlayImage, byte imageTextureType, byte shadowTextureType, byte overlayTextureType,
                             short width, short height, short offSetX, short offSetY, short shadowOffSetX, short shadowOffSetY, short shadow,
                             short shadowWidth, short shadowHeight, short overlayWidth, short overlayHeight)
        {
            MImage mImage = new MImage(image, shadowImage, overlayImage, imageTextureType, shadowTextureType, overlayTextureType)
            {
                Width = width,
                Height = height,
                OffSetX = offSetX,
                OffSetY = offSetY,
                ShadowOffSetX = shadowOffSetX,
                ShadowOffSetY = shadowOffSetY,
                Shadow = (byte)shadow,

                ShadowWidth = shadowWidth,
                ShadowHeight = shadowHeight,
                OverlayWidth = overlayWidth,
                OverlayHeight = overlayHeight,
            };

            Images[index] = mImage;
        }

        public void InsertImage(int index, Bitmap image, Bitmap shadowImage, Bitmap overlayImage, byte imageTextureType, byte shadowTextureType, byte overlayTextureType,
                             short width, short height, short offSetX, short offSetY, short shadowOffSetX, short shadowOffSetY, short shadow,
                             short shadowWidth, short shadowHeight, short overlayWidth, short overlayHeight)
        {
            MImage mImage = new MImage(image, shadowImage, overlayImage, imageTextureType, shadowTextureType, overlayTextureType)
            {
                Width = width,
                Height = height,
                OffSetX = offSetX,
                OffSetY = offSetY,
                ShadowOffSetX = shadowOffSetX,
                ShadowOffSetY = shadowOffSetY,
                Shadow = (byte)shadow,

                ShadowWidth = shadowWidth,
                ShadowHeight = shadowHeight,
                OverlayWidth = overlayWidth,
                OverlayHeight = overlayHeight,
            };

            Images.Insert(index, mImage);
        }

        public void Save(string path)
        {
            //4为总图片数占用字节，Images.Count为所有图片的图片格式占用位
            int headerSize = 4 + Images.Count;

            //遍历所有图片计算素材文件表头总大小
            foreach (MImage image in Images)
            {
                if (image == null || image.FBytes.Length == 0) continue;

                headerSize += 23;

                if (image.ShadowDataSize > 0)
                    headerSize += 8;

                if (image.OverlayDataSize > 0)
                    headerSize += 8;
            }
            //再多移4位表头数据大小占用位  20为版本头大小  加密多加8位密钥大小
            int position = headerSize + 4 + 20 + 8;
            //遍历所有图片计算素材文件图片数据的偏移量
            foreach (MImage image in Images)
            {
                if (image == null || image.FBytes.Length == 0) continue;

                image.Position = position;

                position += image.DataSize;
            }


            using (MemoryStream buffer = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(buffer))
            {
                //写入version信息  注意验证是否16字节，超过16字节读取会出错
                writer.Write(System.Text.Encoding.Default.GetBytes(VersionString));
                byte libversion = LibVersion;
                if (Crypt)
                    libversion = LibVersion | 0x80;
                else
                    libversion = LibVersion & 0x7F;
                writer.Write(libversion);

                //写入表头数据大小
                writer.Write(headerSize);

                using (MemoryStream buffer1 = new MemoryStream())
                using (BinaryWriter writer1 = new BinaryWriter(buffer1))
                {
                    //写入图片数量
                    writer1.Write(Images.Count);
                    //写入所有图片表头数据
                    foreach (MImage image in Images)
                    {
                        if (image == null || image.FBytes.Length == 0)
                        {
                            writer1.Write(byte.MinValue);
                            continue;
                        }

                        writer1.Write(image.ImageTextureType);
                        image.SaveHeader(writer1);
                    }

                    if (Crypt)
                        writer.Write(Encode(buffer1.ToArray(), Key, (byte)(headerSize % 0xFF)));
                    else
                        writer.Write(buffer1.ToArray());
                }

                //写入密钥
                byte[] test = Encode(Key, System.Text.Encoding.ASCII.GetBytes(VersionString), 8);
                writer.Write(Encode(Key, System.Text.Encoding.ASCII.GetBytes(VersionString), 8));

                //写入所有图片数据
                foreach (MImage image in Images)
                {
                    if (image == null || image.FBytes.Length == 0) continue;

                    if (image.FBytes != null)
                        writer.Write(image.FBytes);

                    if (image.ShadowFBytes != null)
                        writer.Write(image.ShadowFBytes);

                    if (image.OverlayFBytes != null)
                        writer.Write(image.OverlayFBytes);
                }
                //写入文件
                File.WriteAllBytes(path, buffer.ToArray());
            }
        }

        public sealed class MImage : IDisposable
        {
            //public const int HeaderSize = 33;
            public bool IsZirconVersion;
            public int Version;

            public int DataSize => (FBytes?.Length ?? 0) + (ShadowFBytes?.Length ?? 0) + (OverlayFBytes?.Length ?? 0);

            public int Position;

            #region Texture

            public short Width;
            public short Height;
            public short OffSetX;
            public short OffSetY;
            public byte Shadow;
            public byte ImageTextureType;
            public Bitmap Image, Preview;
            public bool ImageValid { get; private set; }
            public unsafe byte* ImageData;
            public int ImageDataSize;
            public byte[] FBytes;
            #endregion

            #region Shadow
            public short ShadowWidth;
            public short ShadowHeight;
            public short ShadowOffSetX;
            public short ShadowOffSetY;
            public byte ShadowTextureType;
            public Bitmap ShadowImage, ShadowPreview;
            public bool ShadowValid { get; private set; }
            public unsafe byte* ShadowData;
            public byte[] ShadowFBytes;
            public int ShadowDataSize;
            #endregion

            #region Overlay
            public short OverlayWidth;
            public short OverlayHeight;
            public byte OverlayTextureType;
            public Bitmap OverlayImage, OverlayPreview;
            public bool OverlayValid { get; private set; }
            public unsafe byte* OverlayData;
            public byte[] OverlayFBytes;
            public int OverlayDataSize;
            #endregion


            //public DateTime ExpireTime;

            public MImage()
            {
            }

            public MImage(BinaryReader reader, int version, bool isZirconVersion = false)
            {
                IsZirconVersion = isZirconVersion;
                Version = version;

                Position = reader.ReadInt32();

                //如果黑龙版
                if (!IsZirconVersion)
                {
                    ImageDataSize = reader.ReadInt32();
                    Width = reader.ReadInt16();
                    Height = reader.ReadInt16();
                    OffSetX = reader.ReadInt16();
                    OffSetY = reader.ReadInt16();
                    ShadowOffSetX = reader.ReadInt16();
                    ShadowOffSetY = reader.ReadInt16();
                    Shadow = reader.ReadByte();
                    ShadowTextureType = reader.ReadByte();
                    OverlayTextureType = reader.ReadByte();

                    if (ShadowTextureType > 0)
                    {
                        ShadowDataSize = reader.ReadInt32();
                        ShadowWidth = reader.ReadInt16();
                        ShadowHeight = reader.ReadInt16();
                    }

                    if (OverlayTextureType > 0)
                    {
                        OverlayDataSize = reader.ReadInt32();
                        if (Version > 1)
                        {
                            OverlayWidth = reader.ReadInt16();
                            OverlayHeight = reader.ReadInt16();
                        }
                        else
                        {
                            OverlayWidth = Width;
                            OverlayHeight = Height;
                        }
                    }
                }
                //如果zircon版
                else
                {
                    Width = reader.ReadInt16();
                    Height = reader.ReadInt16();
                    OffSetX = reader.ReadInt16();
                    OffSetY = reader.ReadInt16();

                    Shadow = reader.ReadByte();
                    ShadowWidth = reader.ReadInt16();
                    ShadowHeight = reader.ReadInt16();
                    ShadowOffSetX = reader.ReadInt16();
                    ShadowOffSetY = reader.ReadInt16();

                    OverlayWidth = reader.ReadInt16();
                    OverlayHeight = reader.ReadInt16();
                    int w = Width + (4 - Width % 4) % 4;
                    int h = Height + (4 - Height % 4) % 4;
                    ImageDataSize = w * h / 2;
                    w = ShadowWidth + (4 - ShadowWidth % 4) % 4;
                    h = ShadowHeight + (4 - ShadowHeight % 4) % 4;
                    ShadowDataSize = w * h / 2;
                    w = OverlayWidth + (4 - OverlayWidth % 4) % 4;
                    h = OverlayHeight + (4 - OverlayHeight % 4) % 4;
                    OverlayDataSize = w * h / 2;
                }
            }

            public unsafe MImage(Bitmap image, byte imageTextureType)
            {
                if (image == null || (image.Width == 1 || image.Height == 1))
                {
                    FBytes = new byte[0];
                    ImageTextureType = 0;
                    ShadowTextureType = 0;
                    OverlayTextureType = 0;
                    return;
                }

                Position = -1;
                ImageTextureType = imageTextureType;

                Image = image;
                FBytes = ConvertBitmapToArray(image, ImageTextureType);
                ImageDataSize = FBytes.Length;
                ShadowDataSize = 0;
                OverlayDataSize = 0;
                ShadowTextureType = 0;
                OverlayTextureType = 0;
            }

            public unsafe MImage(Bitmap image, Bitmap shadow, Bitmap overlay, byte imageTextureType, byte shadowTextureType, byte overlayTextureType)
            {
                if (image == null || (image.Width == 1 || image.Height == 1))
                {
                    FBytes = new byte[0];
                    ImageTextureType = 0;
                    ShadowTextureType = 0;
                    OverlayTextureType = 0;
                    return;
                }

                Position = -1;
                ImageTextureType = imageTextureType;

                Image = image;
                FBytes = ConvertBitmapToArray(image, ImageTextureType);
                ImageDataSize = FBytes.Length;

                //影子 统一用dxt1格式
                if (shadow != null && (shadow.Width > 1 && shadow.Height > 1))
                {
                    ShadowTextureType = shadowTextureType;

                    ShadowImage = shadow;
                    ShadowFBytes = ConvertBitmapToArray(shadow, ShadowTextureType);   //影子和覆盖层统一用DXT1格式
                    ShadowDataSize = ShadowFBytes.Length;
                }
                else
                    ShadowTextureType = 0;

                //覆盖 统一用dxt1格式
                if (overlay != null && (overlay.Width > 1 && overlay.Height > 1))
                {
                    OverlayTextureType = overlayTextureType;

                    OverlayImage = overlay;
                    OverlayFBytes = ConvertBitmapToArray(overlay, OverlayTextureType);   //影子和覆盖层统一用DXT1格式
                    OverlayDataSize = OverlayFBytes.Length;
                }
                else
                    OverlayTextureType = 0;
            }

            //WTL1to1ZL
            public unsafe MImage(byte[] fBytes, byte[] shadowFBytes, byte[] overlayFBytes, byte imageTextureType, byte shadowTextureType, byte overlayTextureType)
            {
                if (fBytes == null)
                {
                    FBytes = new byte[0];
                    ImageTextureType = 0;
                    ShadowTextureType = 0;
                    OverlayTextureType = 0;
                    return;
                }

                Position = -1;
                ImageTextureType = imageTextureType;

                FBytes = fBytes;
                ImageDataSize = FBytes.Length;

                //影子
                if (shadowFBytes != null)
                {
                    ShadowTextureType = shadowTextureType;

                    ShadowFBytes = shadowFBytes;   //影子和覆盖层统一用DXT1格式
                    ShadowDataSize = ShadowFBytes.Length;
                }
                else
                    ShadowTextureType = 0;

                //覆盖
                if (overlayFBytes != null)
                {
                    OverlayTextureType = overlayTextureType;

                    OverlayFBytes = overlayFBytes;
                    OverlayDataSize = OverlayFBytes.Length;
                }
                else
                    OverlayTextureType = 0;
            }

            public void LoadImageData(BinaryReader reader, bool mobile)
            {
                if (Position == 0) return;
                if (Width == 1 && Height == 1) return;
                reader.BaseStream.Seek(Position, SeekOrigin.Begin);

                //如果是zircon版(zircon图片格式是DXT1，没有gzip压缩，所以要转gzip压缩)
                if (IsZirconVersion)
                {
                    ImageTextureType = 1;
                    FBytes = Compress(reader.ReadBytes(ImageDataSize));
                    ImageDataSize = FBytes.Length;
                    if (ShadowDataSize > 0)
                    {
                        ShadowTextureType = 1;
                        ShadowFBytes = Compress(reader.ReadBytes(ShadowDataSize));
                        ShadowDataSize = ShadowFBytes.Length;
                    }

                    if (OverlayDataSize > 0)
                    {
                        OverlayTextureType = 1;
                        OverlayFBytes = Compress(reader.ReadBytes(OverlayDataSize));
                        OverlayDataSize = OverlayFBytes.Length;
                    }
                }
                //黑龙版
                else
                {
                    FBytes = reader.ReadBytes(ImageDataSize);
                    if (ShadowDataSize > 0)
                        ShadowFBytes = reader.ReadBytes(ShadowDataSize);
                    if (OverlayDataSize > 0)
                        OverlayFBytes = reader.ReadBytes(OverlayDataSize);

                    if (!mobile) return;
                    //如果是转换手游素材 原有的ARGB32统一转ABGR32
                    if (ImageDataSize > 0 && ImageTextureType == 32)
                    {
                        byte[] imageData = Decompress(FBytes);
                        for (int i = 0; i < imageData.Length; i += 4)
                        {
                            if (imageData[i + 3] == 0) //如果alpha通道为全透明，那么颜色置为0
                            {
                                imageData[i] = 0;
                                imageData[i + 1] = 0;
                                imageData[i + 2] = 0;
                            }
                            //反转红色/蓝色
                            byte b = imageData[i];
                            imageData[i] = imageData[i + 2];
                            imageData[i + 2] = b;
                        }

                        FBytes = Compress(imageData);
                        ImageDataSize = FBytes.Length;
                        ImageTextureType = 33;
                    }
                    if (ShadowDataSize > 0 && ShadowTextureType == 32)
                    {
                        byte[] shadowData = Decompress(ShadowFBytes);
                        for (int i = 0; i < shadowData.Length; i += 4)
                        {
                            if (shadowData[i + 3] == 0) //如果alpha通道为全透明，那么颜色置为0
                            {
                                shadowData[i] = 0;
                                shadowData[i + 1] = 0;
                                shadowData[i + 2] = 0;
                            }
                            //反转红色/蓝色
                            byte b = shadowData[i];
                            shadowData[i] = shadowData[i + 2];
                            shadowData[i + 2] = b;
                        }

                        ShadowFBytes = Compress(shadowData);
                        ShadowDataSize = ShadowFBytes.Length;
                        ShadowTextureType = 33;
                    }
                    if (OverlayDataSize > 0 && OverlayTextureType == 32)
                    {
                        byte[] overlayData = Decompress(OverlayFBytes);
                        for (int i = 0; i < overlayData.Length; i += 4)
                        {
                            if (overlayData[i + 3] == 0) //如果alpha通道为全透明，那么颜色置为0
                            {
                                overlayData[i] = 0;
                                overlayData[i + 1] = 0;
                                overlayData[i + 2] = 0;
                            }
                            //反转红色/蓝色
                            byte b = overlayData[i];
                            overlayData[i] = overlayData[i + 2];
                            overlayData[i + 2] = b;
                        }

                        OverlayFBytes = Compress(overlayData);
                        OverlayDataSize = OverlayFBytes.Length;
                        OverlayTextureType = 33;
                    }

                }
            }

            public unsafe void CreateImage()
            {
                if (Position == 0) return;
                if (Width == 1 && Height == 1) return;

                int w = Width + (4 - Width % 4) % 4;
                int h = Height + (4 - Height % 4) % 4;

                if (w == 0 || h == 0) return;

                Image = new Bitmap(w, h);
                BitmapData data = Image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                //reader.BaseStream.Seek(Position, SeekOrigin.Begin);
                //FBytes = reader.ReadBytes(ImageDataSize);

                byte[] decompressdata = Decompress(FBytes);

                if (ImageTextureType == 1 || ImageTextureType == 3 || ImageTextureType == 5)
                {
                    fixed (byte* source = decompressdata)
                        Squish.DecompressImage(data.Scan0, w, h, (IntPtr)source, ImageTextureType == 1 ? SquishFlags.Dxt1 : ImageTextureType == 3 ? SquishFlags.Dxt3 : SquishFlags.Dxt5);

                    byte* dest = (byte*)data.Scan0;

                    for (int i = 0; i < h * w * 4; i += 4)
                    {
                        //反转红色/蓝色
                        byte b = dest[i];
                        dest[i] = dest[i + 2];
                        dest[i + 2] = b;
                    }
                }
                else if (ImageTextureType == 32)
                    Marshal.Copy(decompressdata, 0, data.Scan0, decompressdata.Length);
                else if (ImageTextureType == 33)
                {
                    Marshal.Copy(decompressdata, 0, data.Scan0, decompressdata.Length);
                    byte* dest = (byte*)data.Scan0;
                    for (int i = 0; i < h * w * 4; i += 4)
                    {
                        //反转红色/蓝色
                        byte b = dest[i];
                        dest[i] = dest[i + 2];
                        dest[i + 2] = b;
                    }
                }

                decompressdata = null;

                Image.UnlockBits(data);
                ImageValid = true;
            }
            public unsafe void CreateShadow()
            {
                if (ShadowDataSize == 0) return;
                if (Position == 0) return;
                if (Width == 1 && Height == 1) return;

                if (!ImageValid)
                    CreateImage();

                int w = ShadowWidth + (4 - ShadowWidth % 4) % 4;
                int h = ShadowHeight + (4 - ShadowHeight % 4) % 4;

                if (w == 0 || h == 0) return;

                ShadowImage = new Bitmap(w, h);
                BitmapData data = ShadowImage.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                //reader.BaseStream.Seek(Position + ImageDataSize, SeekOrigin.Begin);
                //ShadowFBytes = reader.ReadBytes(ShadowDataSize);

                byte[] decompressdata = Decompress(ShadowFBytes);

                if (ShadowTextureType == 1 || ShadowTextureType == 3 || ShadowTextureType == 5)
                {
                    fixed (byte* source = decompressdata)
                        Squish.DecompressImage(data.Scan0, w, h, (IntPtr)source, ShadowTextureType == 1 ? SquishFlags.Dxt1 : ShadowTextureType == 3 ? SquishFlags.Dxt3 : SquishFlags.Dxt5);

                    byte* dest = (byte*)data.Scan0;

                    for (int i = 0; i < h * w * 4; i += 4)
                    {
                        //反转红色/蓝色
                        byte b = dest[i];
                        dest[i] = dest[i + 2];
                        dest[i + 2] = b;
                    }
                }
                else if (ShadowTextureType == 32)
                    Marshal.Copy(decompressdata, 0, data.Scan0, decompressdata.Length);
                else if (ShadowTextureType == 33)
                {
                    Marshal.Copy(decompressdata, 0, data.Scan0, decompressdata.Length);
                    byte* dest = (byte*)data.Scan0;
                    for (int i = 0; i < h * w * 4; i += 4)
                    {
                        //反转红色/蓝色
                        byte b = dest[i];
                        dest[i] = dest[i + 2];
                        dest[i + 2] = b;
                    }
                }

                decompressdata = null;

                ShadowImage.UnlockBits(data);
                ShadowValid = true;
            }
            public unsafe void CreateOverlay()
            {
                if (OverlayDataSize == 0) return;
                if (Position == 0) return;
                if (Width == 1 && Height == 1) return;

                if (!ImageValid)
                    CreateImage();

                int w = OverlayWidth + (4 - OverlayWidth % 4) % 4;
                int h = OverlayHeight + (4 - OverlayHeight % 4) % 4;

                if (w == 0 || h == 0) return;

                OverlayImage = new Bitmap(w, h);
                BitmapData data = OverlayImage.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                //reader.BaseStream.Seek(Position + ImageDataSize + ShadowDataSize, SeekOrigin.Begin);
                //OverlayFBytes = reader.ReadBytes(OverlayDataSize);

                byte[] decompressdata = Decompress(OverlayFBytes);

                if (OverlayTextureType == 1 || OverlayTextureType == 3 || OverlayTextureType == 5)
                {
                    fixed (byte* source = decompressdata)
                        Squish.DecompressImage(data.Scan0, w, h, (IntPtr)source, OverlayTextureType == 1 ? SquishFlags.Dxt1 : OverlayTextureType == 3 ? SquishFlags.Dxt3 : SquishFlags.Dxt5);

                    byte* dest = (byte*)data.Scan0;

                    for (int i = 0; i < h * w * 4; i += 4)
                    {
                        //反转红色/蓝色
                        byte b = dest[i];
                        dest[i] = dest[i + 2];
                        dest[i + 2] = b;
                    }
                }
                else if (OverlayTextureType == 32)
                    Marshal.Copy(decompressdata, 0, data.Scan0, decompressdata.Length);
                else if (OverlayTextureType == 33)
                {
                    Marshal.Copy(decompressdata, 0, data.Scan0, decompressdata.Length);
                    byte* dest = (byte*)data.Scan0;
                    for (int i = 0; i < h * w * 4; i += 4)
                    {
                        //反转红色/蓝色
                        byte b = dest[i];
                        dest[i] = dest[i + 2];
                        dest[i + 2] = b;
                    }
                }

                decompressdata = null;

                OverlayImage.UnlockBits(data);
                OverlayValid = true;
            }

            public void CreatePreview()
            {
                if (Image == null)
                {
                    Preview = new Bitmap(1, 1);
                    return;
                }

                Preview = new Bitmap(64, 64);

                using (Graphics g = Graphics.FromImage(Preview))
                {
                    g.InterpolationMode = InterpolationMode.Low;//HighQualityBicubic
                    g.Clear(Color.Transparent);
                    //按比例缩放 
                    int w = 64, h = 64;
                    if (Height > 64 || Width > 64)
                    {
                        if ((Width * 64) > (Height * 64))
                        {
                            //w = 64;
                            h = (64 * Height) / Width;
                        }
                        else
                        {
                            //h = 64;
                            w = (Width * 64) / Height;
                        }
                    }
                    else
                    {
                        w = Width;
                        h = Height;
                    }
                    g.DrawImage(Image, new Rectangle((64 - w) / 2, (64 - h) / 2, w, h), new Rectangle(0, 0, Width, Height), GraphicsUnit.Pixel);

                    g.Save();
                }
            }

            public void CreateShadowPreview()
            {
                if (ShadowImage == null)
                {
                    ShadowPreview = new Bitmap(1, 1);
                    return;
                }

                ShadowPreview = new Bitmap(64, 64);

                using (Graphics g = Graphics.FromImage(ShadowPreview))
                {
                    g.InterpolationMode = InterpolationMode.Low;//HighQualityBicubic
                    g.Clear(Color.Transparent);
                    //按比例缩放 
                    int w = 64, h = 64;
                    if (ShadowHeight > 64 || ShadowWidth > 64)
                    {
                        if ((ShadowWidth * 64) > (ShadowHeight * 64))
                        {
                            //w = 64;
                            h = (64 * ShadowHeight) / ShadowWidth;
                        }
                        else
                        {
                            //h = 64;
                            w = (ShadowWidth * 64) / ShadowHeight;
                        }
                    }
                    else
                    {
                        w = ShadowWidth;
                        h = ShadowHeight;
                    }
                    g.DrawImage(ShadowImage, new Rectangle((64 - w) / 2, (64 - h) / 2, w, h), new Rectangle(0, 0, ShadowWidth, ShadowHeight), GraphicsUnit.Pixel);

                    g.Save();
                }
            }

            public void CreateOverlayPreview()
            {
                if (OverlayImage == null)
                {
                    OverlayPreview = new Bitmap(1, 1);
                    return;
                }

                OverlayPreview = new Bitmap(64, 64);

                using (Graphics g = Graphics.FromImage(OverlayPreview))
                {
                    g.InterpolationMode = InterpolationMode.Low;//HighQualityBicubic
                    g.Clear(Color.Transparent);
                    //按比例缩放 
                    int w = 64, h = 64;
                    if (OverlayHeight > 64 || OverlayWidth > 64)
                    {
                        if ((OverlayWidth * 64) > (OverlayHeight * 64))
                        {
                            //w = 64;
                            h = (64 * OverlayHeight) / OverlayWidth;
                        }
                        else
                        {
                            //h = 64;
                            w = (OverlayWidth * 64) / OverlayHeight;
                        }
                    }
                    else
                    {
                        w = OverlayWidth;
                        h = OverlayHeight;
                    }
                    g.DrawImage(OverlayImage, new Rectangle((64 - w) / 2, (64 - h) / 2, w, h), new Rectangle(0, 0, OverlayWidth, OverlayHeight), GraphicsUnit.Pixel);

                    g.Save();
                }
            }

            private unsafe byte[] ConvertBitmapToArray(Bitmap input, byte textureType)
            {
                //转换4的倍数大小
                int w = input.Width + (4 - input.Width % 4) % 4;
                int h = input.Height + (4 - input.Height % 4) % 4;
                BitmapData data = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly,
                                                 PixelFormat.Format32bppArgb);
                byte[] pixels = new byte[w * h * 4];
                for (int i = 0; i < input.Height; i++)
                    Marshal.Copy(data.Scan0 + i * input.Width * 4, pixels, i * w * 4, input.Width * 4);
                input.UnlockBits(data);

                //纯黑色转透明 就是不带透明度的纯黑色
                //for (int i = 0; i < pixels.Length; i += 4)
                //{
                //    if (pixels[i] == 0 && pixels[i + 1] == 0 && pixels[i + 2] == 0 && pixels[i + 3] == 255)
                //        pixels[i + 3] = 0;
                //}

                //DXT1或DXT5
                if (textureType == 1 || textureType == 3 || textureType == 5)
                {
                    for (int i = 0; i < pixels.Length; i += 4)
                    {
                        //反转红色/蓝色
                        byte b = pixels[i];
                        pixels[i] = pixels[i + 2];
                        pixels[i + 2] = b;

                        //透明色转不透明色
                        //if (pixels[i + 3] > 0)
                        //{
                        //    byte r = pixels[i];
                        //    byte g = pixels[i + 1];
                        //    b = pixels[i + 2];
                        //    byte a = pixels[i + 3];

                        //    pixels[i] = (byte)(r * (a / 255.0) + 255 - a);
                        //    pixels[i + 1] = (byte)(g * (a / 255.0) + 255 - a);
                        //    pixels[i + 2] = (byte)(b * (a / 255.0) + 255 - a);
                        //    pixels[i + 3] = 255;
                        //}
                    }

                    int count = Squish.GetStorageRequirements(w, h, textureType == 1 ? SquishFlags.Dxt1 : textureType == 3 ? SquishFlags.Dxt3 : SquishFlags.Dxt5);

                    byte[] dxtDatas = new byte[count];
                    fixed (byte* dest = dxtDatas)
                    fixed (byte* source = pixels)
                    {
                        Squish.CompressImage((IntPtr)source, w, h, (IntPtr)dest, textureType == 1 ? SquishFlags.Dxt1 : textureType == 3 ? SquishFlags.Dxt3 : SquishFlags.Dxt5);
                    }
                    //返回压缩数据
                    return Compress(dxtDatas);
                }
                else if (textureType == 32)  //ARGB32
                {
                    for (int i = 0; i < pixels.Length; i += 4)
                    {
                        if (pixels[i + 3] == 0) //如果alpha通道为全透明，那么颜色置为0
                        {
                            pixels[i] = 0;
                            pixels[i + 1] = 0;
                            pixels[i + 2] = 0;
                        }
                    }
                }
                else if (textureType == 33) //ABGR32
                {
                    for (int i = 0; i < pixels.Length; i += 4)
                    {
                        if (pixels[i + 3] == 0) //如果alpha通道为全透明，那么颜色置为0
                        {
                            pixels[i] = 0;
                            pixels[i + 1] = 0;
                            pixels[i + 2] = 0;
                        }
                        //反转红色/蓝色
                        byte b = pixels[i];
                        pixels[i] = pixels[i + 2];
                        pixels[i + 2] = b;
                    }
                }

                //ARGB32 ABGR32
                return Compress(pixels);
            }

            public static byte[] Compress(byte[] raw)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    using (DeflateStream gzip = new DeflateStream(memory,
                    CompressionMode.Compress, true))
                    {
                        gzip.Write(raw, 0, raw.Length);
                    }
                    return memory.ToArray();
                }
            }

            static byte[] Decompress(byte[] gzip)
            {
                // Create a GZIP stream with decompression mode.
                // ... Then create a buffer and write into while reading from the GZIP stream.
                using (DeflateStream stream = new DeflateStream(new MemoryStream(gzip), CompressionMode.Decompress))
                {
                    const int size = 4096;
                    byte[] buffer = new byte[size];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        return memory.ToArray();
                    }
                }
            }

            public void SaveHeader(BinaryWriter writer)
            {
                writer.Write(Position);
                writer.Write(ImageDataSize);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(OffSetX);
                writer.Write(OffSetY);
                writer.Write(ShadowOffSetX);
                writer.Write(ShadowOffSetY);
                writer.Write(Shadow);
                writer.Write(ShadowTextureType);
                writer.Write(OverlayTextureType);

                if (ShadowTextureType > 0)
                {
                    writer.Write(ShadowDataSize);
                    writer.Write(ShadowWidth);
                    writer.Write(ShadowHeight);
                }

                if (OverlayTextureType > 0)
                {
                    writer.Write(OverlayDataSize);
                    writer.Write(OverlayWidth);
                    writer.Write(OverlayHeight);
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
                    ImageDataSize = 0;
                    ShadowDataSize = 0;
                    OverlayDataSize = 0;

                    Width = 0;
                    Height = 0;
                    OffSetX = 0;
                    OffSetY = 0;
                    Shadow = 0;

                    ShadowWidth = 0;
                    ShadowHeight = 0;
                    ShadowOffSetX = 0;
                    ShadowOffSetY = 0;

                    ImageTextureType = 0;
                    ShadowTextureType = 0;
                    OverlayTextureType = 0;

                    OverlayWidth = 0;
                    OverlayHeight = 0;
                }

            }

            public void Dispose()
            {
                Dispose(!IsDisposed);
                GC.SuppressFinalize(this);
            }
            ~MImage()
            {
                Dispose(false);
            }

            #endregion

        }
    }
}
