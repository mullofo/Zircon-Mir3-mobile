using Library;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Client.Envir
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

        public List<int> breakPoints = new List<int>();

        /// <summary>
        /// 库文件
        /// </summary>
        /// <param name="fileName"></param>
        public MirLibrary(string fileName)
        {
            _FStream = File.OpenRead(fileName);
            _BReader = new BinaryReader(_FStream);
            FileName = fileName;
        }
        /// <summary>
        /// 读取库文件
        /// </summary>
        public void ReadLibrary()
        {
            lock (LoadLocker)
            {
                if (Loading) return;
                Loading = true;
            }

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
                MessageBox.Show("版本错误, 应为黑龙专用版本: " + LibVersion.ToString() + " 发现版本: " + currentVersion.ToString() + ".", "打开失败", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                breakPoints.Clear();
                bool breaking = false; ;

                for (int i = 0; i < Images.Length; i++)
                {
                    //判断图片格式
                    byte imageTextureType = reader.ReadByte();
                    if (imageTextureType == 0)
                    {
                        if (!breaking)
                        {
                            breakPoints.Add(i);
                        }
                        breaking = true;
                        continue;
                    }
                    breaking = false;
                    Images[i] = new MirImage(reader, currentVersion, isZirconVersion) { ImageTextureType = imageTextureType, };
                }
            }
            Loaded = true;
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

        public int GetFrameCount(int index)
        {
            for (int i = 0; i < breakPoints.Count; i++)
            {
                if (index < breakPoints[i])
                {
                    return breakPoints[i] - index;
                }
            }
            return 1;
        }

        /// <summary>
        /// 获得尺寸
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Size GetSize(int index)
        {
            if (!CheckImage(index)) return Size.Empty;

            return new Size(Images[index].Width, Images[index].Height);
        }
        /// <summary>
        /// 获得偏移量
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Point GetOffSet(int index)
        {
            if (!CheckImage(index)) return Point.Empty;

            return new Point(Images[index].OffSetX, Images[index].OffSetY);
        }
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MirImage GetImage(int index)
        {
            if (!CheckImage(index)) return null;

            return Images[index];
        }
        /// <summary>
        /// 创建图像
        /// </summary>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public MirImage CreateImage(int index, ImageType type)
        {
            if (!CheckImage(index)) return null;

            MirImage image = Images[index];

            Texture texture;

            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_BReader);
                    texture = image.Image;
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateShadow(_BReader);
                    texture = image.Shadow;
                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateOverlay(_BReader);
                    texture = image.Overlay;
                    break;
                default:
                    return null;
            }

            if (texture == null) return null;

            return image;
        }
        /// <summary>
        /// 检查图片
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool CheckImage(int index)
        {
            if (!Loaded) ReadLibrary();

            while (!Loaded)
                Thread.Sleep(1);

            return index >= 0 && index < Images.Length && Images[index] != null;
        }
        /// <summary>
        /// 可见像素
        /// </summary>
        /// <param name="index"></param>
        /// <param name="location"></param>
        /// <param name="accurate"></param>
        /// <param name="offSet"></param>
        /// <returns></returns>
        public bool VisiblePixel(int index, Point location, bool accurate = true, bool offSet = false)
        {
            if (!CheckImage(index)) return false;

            MirImage image = Images[index];

            if (offSet)
                location = new Point(location.X - image.OffSetX, location.Y - image.OffSetY);

            return image.VisiblePixel(location, accurate);
        }
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="index"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="colour"></param>
        /// <param name="area"></param>
        /// <param name="opacity"></param>
        /// <param name="type"></param>
        /// <param name="shadow"></param>
        public void Draw(int index, float x, float y, Color colour, Rectangle area, float opacity, ImageType type, byte shadow = 0)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            Texture texture;

            float oldOpacity = DXManager.Opacity;
            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_BReader);
                    texture = image.Image;
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateShadow(_BReader);
                    texture = image.Shadow;

                    if (texture == null)
                    {
                        if (!image.ImageValid) image.CreateImage(_BReader);
                        texture = image.Image;

                        switch (image.ShadowType)
                        {
                            case 177:
                            case 176:
                            case 49:
                                Matrix m = Matrix.Scaling(1F, 0.5f, 0);

                                m.M21 = -0.50F;
                                DXManager.Sprite.Transform = m * Matrix.Translation(x + image.Height / 2, y, 0);

                                DXManager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
                                DXManager.SetOpacity(opacity);

                                DXManager.Sprite.Draw(texture, Vector3.Zero, Vector3.Zero, Color.Black);
                                CEnvir.DPSCounter++;

                                DXManager.SetOpacity(oldOpacity);
                                DXManager.Sprite.Transform = Matrix.Identity;
                                DXManager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);

                                image.ExpireTime = Time.Now + Config.CacheDuration;
                                break;
                            case 50:
                                DXManager.SetOpacity(opacity);

                                DXManager.Sprite.Draw(texture, Vector3.Zero, new Vector3(x, y, 0), Color.Black);
                                CEnvir.DPSCounter++;
                                DXManager.SetOpacity(oldOpacity);

                                image.ExpireTime = Time.Now + Config.CacheDuration;
                                break;
                        }
                        return;
                    }
                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateOverlay(_BReader);
                    texture = image.Overlay;
                    break;
                default:
                    return;
            }

            if (texture == null) return;

            DXManager.SetOpacity(opacity);

            DXManager.Sprite.Draw(texture, area, Vector3.Zero, new Vector3(x, y, 0), colour);
            CEnvir.DPSCounter++;
            DXManager.SetOpacity(oldOpacity);

            image.ExpireTime = Time.Now + Config.CacheDuration;
        }
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="index"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="colour"></param>
        /// <param name="useOffSet"></param>
        /// <param name="opacity"></param>
        /// <param name="type"></param>
        public void Draw(int index, float x, float y, Color colour, bool useOffSet, float opacity, ImageType type)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            Texture texture;

            float oldOpacity = DXManager.Opacity;
            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_BReader);
                    texture = image.Image;
                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateShadow(_BReader);
                    texture = image.Shadow;

                    if (useOffSet)
                    {
                        x += image.ShadowOffSetX;
                        y += image.ShadowOffSetY;
                    }

                    if (texture == null)
                    {
                        if (!image.ImageValid) image.CreateImage(_BReader);
                        texture = image.Image;

                        switch (image.ShadowType)  //改正NPC、怪物等阴影部分
                        {
                            case 177:
                            case 176:
                            case 49:
                                Matrix m = Matrix.Scaling(1F, 0.5f, 0);

                                m.M21 = -0.50F;
                                DXManager.Sprite.Transform = m * Matrix.Translation(x + image.Height / 2, y, 0);

                                DXManager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
                                DXManager.SetOpacity(opacity);

                                DXManager.Sprite.Draw(texture, Vector3.Zero, Vector3.Zero, Color.Black);
                                CEnvir.DPSCounter++;

                                DXManager.SetOpacity(oldOpacity);
                                DXManager.Sprite.Transform = Matrix.Identity;
                                DXManager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);

                                image.ExpireTime = Time.Now + Config.CacheDuration;
                                break;
                            case 50:
                                DXManager.SetOpacity(opacity);

                                DXManager.Sprite.Draw(texture, Vector3.Zero, new Vector3(x, y, 0), Color.Black);
                                CEnvir.DPSCounter++;
                                DXManager.SetOpacity(oldOpacity);

                                image.ExpireTime = Time.Now + Config.CacheDuration;
                                break;

                                /*default:
                                    Matrix m = Matrix.Scaling(1F, 0.5f, 0);

                                    m.M21 = -0.50F;
                                    DXManager.Sprite.Transform = m * Matrix.Translation(x + image.Height / 2, y, 0);  //要根据不同类型调整Y的坐标

                                    DXManager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
                                    if (oldOpacity != 0.5F) DXManager.SetOpacity(0.5F);

                                    DXManager.Sprite.Draw(texture, Vector3.Zero, Vector3.Zero, Color.Black);
                                    CEnvir.DPSCounter++;

                                    DXManager.SetOpacity(oldOpacity);
                                    DXManager.Sprite.Transform = Matrix.Identity;
                                    DXManager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);

                                    image.ExpireTime = Time.Now + Config.CacheDuration;
                                    break;*/
                        }
                        return;
                    }
                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateOverlay(_BReader);
                    texture = image.Overlay;

                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                default:
                    return;
            }

            if (texture == null) return;

            DXManager.SetOpacity(opacity);

            DXManager.Sprite.Draw(texture, Vector3.Zero, new Vector3(x, y, 0), colour);
            CEnvir.DPSCounter++;

            DXManager.SetOpacity(oldOpacity);

            image.ExpireTime = Time.Now + Config.CacheDuration;
        }
        /// <summary>
        /// 混合绘制
        /// </summary>
        /// <param name="index"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="colour"></param>
        /// <param name="useOffSet"></param>
        /// <param name="rate"></param>
        /// <param name="type"></param>
        /// <param name="shadow"></param>
        public void DrawBlend(int index, float x, float y, Color colour, bool useOffSet, float rate, ImageType type, byte shadow = 0, BlendType blendType = BlendType.NORMAL)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            Texture texture;

            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_BReader);
                    texture = image.Image;
                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                case ImageType.Shadow:
                    return;
                /*     if (!image.ShadowValid) image.CreateShadow(_BReader);
                     texture = image.Shadow;

                     if (useOffSet)
                     {
                         x += image.ShadowOffSetX;
                         y += image.ShadowOffSetY;
                     }
                     break;*/
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateOverlay(_BReader);
                    texture = image.Overlay;

                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                default:
                    return;
            }
            if (texture == null) return;

            bool oldBlend = DXManager.Blending;
            float oldRate = DXManager.BlendRate;

            DXManager.SetBlend(true, rate, blendType);

            if (rate != 1F)
                colour = Color.FromArgb((int)(colour.A * rate), (int)(colour.R * rate), (int)(colour.G * rate), (int)(colour.B * rate));
            DXManager.Sprite.Draw(texture, Vector3.Zero, new Vector3(x, y, 0), colour);
            CEnvir.DPSCounter++;

            DXManager.SetBlend(oldBlend, oldRate);

            image.ExpireTime = Time.Now + Config.CacheDuration;
        }

        #region 粒子相关
        /// <summary>
        /// 粒子混合绘制
        /// </summary>
        /// <param name="index"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="colour"></param>
        /// <param name="sizex"></param>
        /// <param name="sizey"></param>
        /// <param name="rate"></param>
        /// <param name="type"></param>
        /// <param name="blendType"></param>
        public void DrawBlend(int index, float x, float y, Color colour, float sizex, float sizey, float rate, ImageType type, BlendType blendType = BlendType.NORMAL)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];
            Texture texture;
            switch (type)
            {
                default:
                    return;
                case ImageType.Shadow:
                    return;
                case ImageType.Image:
                    if (!image.ImageValid)
                    {
                        image.CreateImage(_BReader);
                    }
                    texture = image.Image;
                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid)
                    {
                        image.CreateOverlay(_BReader);
                    }
                    texture = image.Overlay;
                    break;
            }
            if (texture == null) return;

            bool oldBlend = DXManager.Blending;
            float oldRate = DXManager.BlendRate;
            float scalx = sizex / (float)image.Width;
            float scaly = sizey / (float)image.Height;
            DXManager.SetBlend(true, rate, blendType);
            DXManager.Sprite.Transform = Matrix.Scaling(scalx, scaly, 0f);
            DXManager.Sprite.Draw(texture, Vector3.Zero, new Vector3((x - sizex / 3f) / scalx, (y - sizey / 3f) / scaly, 0f), colour);
            //DXManager.Sprite.Draw(texture, Vector3.Zero, new Vector3(x / scalx, y / scaly, 0f), colour);
            CEnvir.DPSCounter++;
            DXManager.SetBlend(oldBlend, oldRate);
            DXManager.Sprite.Transform = Matrix.Identity;
            image.ExpireTime = Time.Now + Config.CacheDuration;
        }
        #endregion

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
        public bool IsZirconVersion;
        public int Version;

        #region Texture
        /// <summary>
        /// 宽
        /// </summary>
        public short Width;
        /// <summary>
        /// 高
        /// </summary>
        public short Height;
        /// <summary>
        /// X偏移量
        /// </summary>
        public short OffSetX;
        /// <summary>
        /// Y偏移量
        /// </summary>
        public short OffSetY;
        /// <summary>
        /// 阴影类型
        /// </summary>
        public byte ShadowType;
        /// <summary>
        /// 纹理图像
        /// </summary>
        public Texture Image;
        public byte ImageTextureType;
        /// <summary>
        /// 有效图像
        /// </summary>
        public bool ImageValid { get; private set; }
        /// <summary>
        /// 图像数据
        /// </summary>
        public unsafe byte* ImageData;
        /// <summary>
        /// 图像数据大小
        /// </summary>
        public int ImageDataSize;
        #endregion

        #region Shadow 阴影
        /// <summary>
        /// 阴影宽度
        /// </summary>
        public short ShadowWidth;
        /// <summary>
        /// 阴影高度
        /// </summary>
        public short ShadowHeight;
        /// <summary>
        /// 阴影X偏移量
        /// </summary>
        public short ShadowOffSetX;
        /// <summary>
        /// 阴影Y偏移量
        /// </summary>
        public short ShadowOffSetY;
        /// <summary>
        /// 阴影纹理
        /// </summary>
        public Texture Shadow;
        public byte ShadowTextureType;
        /// <summary>
        /// 有效阴影
        /// </summary>
        public bool ShadowValid { get; private set; }
        /// <summary>
        /// 阴影数据
        /// </summary>
        //public unsafe byte* ShadowData;
        /// <summary>
        /// 阴影数据大小
        /// </summary>
        public int ShadowDataSize;
        #endregion

        #region Overlay 叠加
        /// <summary>
        /// 叠加宽度
        /// </summary>
        public short OverlayWidth;
        /// <summary>
        /// 叠加高度
        /// </summary>
        public short OverlayHeight;
        /// <summary>
        /// 叠加纹理
        /// </summary>
        public Texture Overlay;
        public byte OverlayTextureType;
        /// <summary>
        /// 有效叠加
        /// </summary>
        public bool OverlayValid { get; private set; }
        /// <summary>
        /// 叠加数据
        /// </summary>
        //public unsafe byte* OverlayData;
        /// <summary>
        /// 叠加数据大小
        /// </summary>
        public int OverlayDataSize;
        #endregion

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
            IsZirconVersion = isZirconVersion;
            Version = version;

            Position = reader.ReadInt32();

            //如果黑龙版
            if (!isZirconVersion)
            {
                ImageDataSize = reader.ReadInt32();
                Width = reader.ReadInt16();
                Height = reader.ReadInt16();
                OffSetX = reader.ReadInt16();
                OffSetY = reader.ReadInt16();
                ShadowOffSetX = reader.ReadInt16();
                ShadowOffSetY = reader.ReadInt16();
                ShadowType = reader.ReadByte();
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

                ShadowType = reader.ReadByte();
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

                ImageTextureType = 1;
                ShadowTextureType = 1;
                OverlayTextureType = 1;
            }
        }
        /// <summary>
        /// 可见像素
        /// </summary>
        /// <param name="p"></param>
        /// <param name="acurrate"></param>
        /// <returns></returns>
        public unsafe bool VisiblePixel(Point p, bool acurrate)
        {
            if (p.X < 0 || p.Y < 0 || !ImageValid || ImageData == null) return false;

            int w = Width + (4 - Width % 4) % 4;
            int h = Height + (4 - Height % 4) % 4;

            if (p.X >= w || p.Y >= h)
                return false;

            bool visible = false;
            switch (ImageTextureType)
            {
                case 32:
                case 33:
                    int offset = (p.X + p.Y * w) * 4;
                    int pix = ImageData[offset + 3] << 24 | ImageData[offset + 2] << 16 | ImageData[offset + 1] << 8 | ImageData[offset];
                    visible = pix != 0;
                    break;
                default:
                    int x = (p.X - p.X % 4) / 4;
                    int y = (p.Y - p.Y % 4) / 4;
                    int index = (y * (w / 4) + x) * 8;

                    int col0 = ImageData[index + 1] << 8 | ImageData[index], col1 = ImageData[index + 3] << 8 | ImageData[index + 2];

                    if (col0 == 0 && col1 == 0) return false;

                    if (!acurrate || col1 < col0) return true;

                    x = p.X % 4;
                    y = p.Y % 4;
                    x *= 2;
                    visible = (ImageData[index + 4 + y] & 1 << x) >> x != 1 || (ImageData[index + 4 + y] & 1 << x + 1) >> x + 1 != 1; ;
                    break;
            }

            return visible;
        }

        /// <summary>
        /// 处理纹理
        /// </summary>
        public unsafe void DisposeTexture()
        {
            if (Image != null && !Image.IsDisposed)
                Image.Dispose();

            if (Shadow != null && !Shadow.IsDisposed)
                Shadow.Dispose();

            if (Overlay != null && !Overlay.IsDisposed)
                Overlay.Dispose();

            ImageData = null;
            //ShadowData = null;
            //OverlayData = null;

            Image = null;
            Shadow = null;
            Overlay = null;

            ImageValid = false;
            ShadowValid = false;
            OverlayValid = false;

            ExpireTime = DateTime.MinValue;

            DXManager.TextureList.Remove(this);
        }
        /// <summary>
        /// 创建图像
        /// </summary>
        /// <param name="reader"></param>
        public unsafe void CreateImage(BinaryReader reader)
        {
            if (Position == 0) return;

            int w = Width + (4 - Width % 4) % 4;
            int h = Height + (4 - Height % 4) % 4;

            if (w == 0 || h == 0) return;

            Format format = ImageTextureType == 1 ? Format.Dxt1 : ImageTextureType == 3 ? Format.Dxt3 : ImageTextureType == 5 ? Format.Dxt5 : ImageTextureType == 32 ? Format.A8R8G8B8 : Format.A8B8G8R8;

            Image = new Texture(DXManager.Device, w, h, 1, Usage.None, format, Pool.Managed);
            DataRectangle rect = Image.LockRectangle(0, LockFlags.Discard);
            ImageData = (byte*)rect.DataPointer;

            byte[] buffer = null;
            lock (reader)
            {
                reader.BaseStream.Seek(Position, SeekOrigin.Begin);
                if (IsZirconVersion)
                    buffer = reader.ReadBytes(ImageDataSize);
                else
                    buffer = Decompress(reader.ReadBytes(ImageDataSize));
            }
            DataStream data = new DataStream(rect.DataPointer, buffer.Length, true, true);
            data.Write(buffer, 0, buffer.Length);

            Image.UnlockRectangle(0);
            data.Dispose();

            buffer = null;

            ImageValid = true;
            ExpireTime = CEnvir.Now + Config.CacheDuration;
            DXManager.TextureList.Add(this);
        }
        /// <summary>
        /// 创建阴影
        /// </summary>
        /// <param name="reader"></param>
        public unsafe void CreateShadow(BinaryReader reader)
        {
            if (Position == 0 || ShadowDataSize == 0) return;

            if (!ImageValid)
                CreateImage(reader);

            int w = ShadowWidth + (4 - ShadowWidth % 4) % 4;
            int h = ShadowHeight + (4 - ShadowHeight % 4) % 4;

            if (w == 0 || h == 0) return;

            Format format = ShadowTextureType == 1 ? Format.Dxt1 : ShadowTextureType == 3 ? Format.Dxt3 : ShadowTextureType == 5 ? Format.Dxt5 : ShadowTextureType == 32 ? Format.A8R8G8B8 : Format.A8B8G8R8;

            Shadow = new Texture(DXManager.Device, w, h, 1, Usage.None, format, Pool.Managed);
            DataRectangle rect = Shadow.LockRectangle(0, LockFlags.Discard);
            //ShadowData = (byte*)rect.DataPointer;

            byte[] buffer = null;
            lock (reader)
            {
                reader.BaseStream.Seek(Position + ImageDataSize, SeekOrigin.Begin);
                if (IsZirconVersion)
                    buffer = reader.ReadBytes(ShadowDataSize);
                else
                    buffer = Decompress(reader.ReadBytes(ShadowDataSize));
            }
            DataStream data = new DataStream(rect.DataPointer, buffer.Length, true, true);
            data.Write(buffer, 0, buffer.Length);

            Shadow.UnlockRectangle(0);
            data.Dispose();

            buffer = null;

            ShadowValid = true;
        }
        /// <summary>
        /// 创建叠加
        /// </summary>
        /// <param name="reader"></param>
        public unsafe void CreateOverlay(BinaryReader reader)
        {
            if (Position == 0 || OverlayDataSize == 0) return;

            if (!ImageValid)
                CreateImage(reader);

            int w = Width + (4 - Width % 4) % 4;
            int h = Height + (4 - Height % 4) % 4;

            if (w == 0 || h == 0) return;

            Format format = OverlayTextureType == 1 ? Format.Dxt1 : OverlayTextureType == 3 ? Format.Dxt3 : OverlayTextureType == 5 ? Format.Dxt5 : OverlayTextureType == 32 ? Format.A8R8G8B8 : Format.A8B8G8R8;

            Overlay = new Texture(DXManager.Device, w, h, 1, Usage.None, format, Pool.Managed);
            DataRectangle rect = Overlay.LockRectangle(0, LockFlags.Discard);
            //OverlayData = (byte*)rect.DataPointer;

            byte[] buffer = null;
            lock (reader)
            {
                reader.BaseStream.Seek(Position + ImageDataSize + ShadowDataSize, SeekOrigin.Begin);
                if (IsZirconVersion)
                    buffer = reader.ReadBytes(OverlayDataSize);
                else
                    buffer = Decompress(reader.ReadBytes(OverlayDataSize));
            }
            DataStream data = new DataStream(rect.DataPointer, buffer.Length, true, true);
            data.Write(buffer, 0, buffer.Length);

            Overlay.UnlockRectangle(0);
            data.Dispose();

            buffer = null;

            OverlayValid = true;
        }

        private static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (DeflateStream stream = new DeflateStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    stream.CopyTo(memory);
                    return memory.ToArray();
                }
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
                IsZirconVersion = false;

                Width = 0;
                Height = 0;
                OffSetX = 0;
                OffSetY = 0;

                ShadowWidth = 0;
                ShadowHeight = 0;
                ShadowOffSetX = 0;
                ShadowOffSetY = 0;

                //OverlayWidth = 0;
                //OverlayHeight = 0;
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

    /// <summary>
    /// 图片类型
    /// </summary>
    public enum ImageType
    {
        /// <summary>
        /// 图片
        /// </summary>
        Image,
        /// <summary>
        /// 影子
        /// </summary>
        Shadow,
        /// <summary>
        /// 叠加
        /// </summary>
        Overlay,
    }

}
