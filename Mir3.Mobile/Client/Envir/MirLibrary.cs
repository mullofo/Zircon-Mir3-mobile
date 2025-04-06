using Client.Extentions;
using Client.Helpers;
using Library;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Texture = MonoGame.Extended.Texture;

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
        public string FullPathName;
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

        private byte HeadFileStat;
        private DateTime NextSyncTime;

        /// <summary>
        /// 库文件
        /// </summary>
        /// <param name="fileName"></param>
        public MirLibrary(string fileName)
        {
            FileName = fileName;
            FullPathName = CEnvir.MobileClientPath + fileName;
            if (File.Exists(FullPathName))
            {
                _FStream = File.OpenRead(FullPathName);
                _BReader = new BinaryReader(_FStream);
            }
        }
        /// <summary>
        /// 读取库文件
        /// </summary>
        public void ReadLibrary()
        {
            lock (LoadLocker)
            {
                // 1为有微端异步任务；2为正在get数据；3为get数据成功(会创建号文件)
                if (Loading || HeadFileStat == 1 || HeadFileStat == 2) return;
                try
                {
                    if (!File.Exists(FullPathName) && HeadFileStat == 0 && Time.Now > NextSyncTime && LibraryHelper.MicroServerActive)
                    {
                        //限制并发量
                        if (LibraryHelper.Semaphore.CurrentCount > 0)
                        {
                            LibraryHelper.Semaphore.WaitAsync();
                            HeadFileStat = 1;
                            Task.Run(async () =>
                            {
                                try
                                {
                                    if (HeadFileStat != 1) return;

                                    HeadFileStat = 2;
                                    bool returned = await LibraryHelper.GetHeaderAsync(FileName);
                                    if (returned)
                                        HeadFileStat = 3;
                                    else
                                    {
                                        //如果get数据不成功，状态置为0 并且1秒后在get
                                        HeadFileStat = 0;
                                        NextSyncTime = Time.Now.AddSeconds(1);
                                    }
                                }
                                finally
                                {
                                    LibraryHelper.Semaphore.Release();
                                }
                            });
                        }
                        return;
                    }

                    if (!File.Exists(FullPathName)) return;
                    if (_FStream == null)
                        _FStream = File.OpenRead(FullPathName);
                    if (_FStream != null)
                        _BReader = new BinaryReader(_FStream);
                    Loading = true;
                }
                catch (Exception ex2)
                {
                    CEnvir.SaveError(ex2.ToString());
                }
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
                    Images[i] = new MirImage(reader, currentVersion, isZirconVersion) { ImageTextureType = imageTextureType };
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
                    if (!image.ImageValid) image.CreateImage(_BReader, FileName, index);
                    texture = image.Image;
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateImage(_BReader, FileName, index);
                    texture = image.Shadow;
                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateImage(_BReader, FileName, index);
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

            //while (!Loaded)
            //{
            //    if (HeadFileStat == 1 || HeadFileStat == 2) break;
            //    Thread.Sleep(1);
            //}
            if (HeadFileStat == 1 || HeadFileStat == 2) return false;

            if (Images == null) return false;

            return index >= 0 && index < Images.Length && Images[index] != null && Loaded;
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
        public void Draw(int index, float x, float y, Color colour, Rectangle area, float opacity, ImageType type, byte shadow = 0, float zoomRate = 1F, int uiOffsetX = 0)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            Texture texture;

            float oldOpacity = DXManager.Opacity;
            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_BReader, FileName, index);
                    texture = image.Image;
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateImage(_BReader, FileName, index);
                    texture = image.Shadow;

                    if (texture == null)
                    {
                        if (!image.ImageValid) image.CreateImage(_BReader, FileName, index);
                        texture = image.Image;

                        switch (image.ShadowType)
                        {
                            case 177:
                            case 176:
                            case 49:
                                Matrix m = Matrix.CreateScale(1F, 0.5f, 0);

                                m.M21 = -0.50F;
                                DXManager.Sprite.Transform = m * Matrix.CreateTranslation((x + image.Height / 2) * zoomRate + uiOffsetX, y * zoomRate, 0);

                                DXManager.Sprite.samplerState = null;
                                DXManager.SetOpacity(opacity);

                                if (zoomRate == 1F)
                                    DXManager.Sprite.Draw(texture, Vector2.Zero, Vector2.Zero, Color.Black);
                                else
                                    DXManager.Sprite.DrawZoom(texture, Vector2.Zero, Color.Black, zoomRate);
                                CEnvir.DPSCounter++;

                                DXManager.SetOpacity(oldOpacity);
                                DXManager.Sprite.Transform = Matrix.Identity;
                                DXManager.Sprite.samplerState = SamplerState.PointClamp;

                                image.ExpireTime = Time.Now + Config.CacheDuration;
                                break;
                            case 50:
                                DXManager.SetOpacity(opacity);
                                if (zoomRate == 1F)
                                    DXManager.Sprite.Draw(texture, Vector2.Zero, new Vector2(x, y), Color.Black);
                                else
                                    DXManager.Sprite.DrawZoom(texture, new Vector2(x * zoomRate + uiOffsetX, y * zoomRate), Color.Black, zoomRate);
                                CEnvir.DPSCounter++;
                                DXManager.SetOpacity(oldOpacity);

                                image.ExpireTime = Time.Now + Config.CacheDuration;
                                break;
                        }
                        return;
                    }
                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateImage(_BReader, FileName, index);
                    texture = image.Overlay;
                    break;
                default:
                    return;
            }

            if (texture == null) return;

            DXManager.SetOpacity(opacity);

            if (zoomRate == 1F)
                DXManager.Sprite.Draw(texture, area, Vector2.Zero, new Vector2(x, y), colour);
            else
                DXManager.Sprite.DrawZoom(texture, area, Vector2.Zero, new Vector2(x * zoomRate + uiOffsetX, y * zoomRate), colour, zoomRate);

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
        public void Draw(int index, float x, float y, Color colour, bool useOffSet, float opacity, ImageType type, float zoomRate = 1F, int uiOffsetX = 0)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            Texture texture;

            float oldOpacity = DXManager.Opacity;
            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_BReader, FileName, index);
                    texture = image.Image;
                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateImage(_BReader, FileName, index);
                    texture = image.Shadow;

                    if (useOffSet)
                    {
                        x += image.ShadowOffSetX;
                        y += image.ShadowOffSetY;
                    }

                    if (texture == null)
                    {
                        if (!image.ImageValid) image.CreateImage(_BReader, FileName, index);
                        texture = image.Image;

                        switch (image.ShadowType)  //改正NPC、怪物等阴影部分
                        {
                            case 177:
                            case 176:
                            case 49:
                                Matrix m = Matrix.CreateScale(1F, 0.5f, 0);

                                m.M21 = -0.50F;
                                DXManager.Sprite.Transform = m * Matrix.CreateTranslation((x + image.Height / 2) * zoomRate + uiOffsetX, y * zoomRate, 0);

                                DXManager.Sprite.samplerState = null;
                                DXManager.SetOpacity(opacity);
                                if (zoomRate == 1F)
                                    DXManager.Sprite.Draw(texture, Vector2.Zero, Vector2.Zero, Color.Black);
                                else
                                    DXManager.Sprite.DrawZoom(texture, Vector2.Zero, Color.Black, zoomRate);
                                CEnvir.DPSCounter++;

                                DXManager.SetOpacity(oldOpacity);
                                DXManager.Sprite.Transform = Matrix.Identity;
                                DXManager.Sprite.samplerState = SamplerState.PointClamp;

                                image.ExpireTime = Time.Now + Config.CacheDuration;
                                break;
                            case 50:
                                DXManager.SetOpacity(opacity);
                                if (zoomRate == 1F)
                                    DXManager.Sprite.Draw(texture, Vector2.Zero, new Vector2(x, y), Color.Black);
                                else
                                    DXManager.Sprite.DrawZoom(texture, new Vector2(x * zoomRate + uiOffsetX, y * zoomRate), Color.Black, zoomRate);
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
                    if (!image.OverlayValid) image.CreateImage(_BReader, FileName, index);
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
            if (zoomRate == 1F)
                DXManager.Sprite.Draw(texture, Vector2.Zero, new Vector2(x, y), colour);
            else
                DXManager.Sprite.DrawZoom(texture, new Vector2(x * zoomRate + uiOffsetX, y * zoomRate), colour, zoomRate);
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
        public void DrawBlend(int index, float x, float y, Color colour, bool useOffSet, float rate, ImageType type, byte shadow = 0, BlendType blendType = BlendType.NORMAL, float zoomRate = 1F, int uiOffsetX = 0)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            Texture texture;

            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_BReader, FileName, index);
                    texture = image.Image;
                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                case ImageType.Shadow:
                    return;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateImage(_BReader, FileName, index);
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

            DXManager.SetBlend(true, rate, blendtype: blendType);

            if (rate != 1F)
                colour = Color.FromArgb((int)(colour.A * rate), (int)(colour.R * rate), (int)(colour.G * rate), (int)(colour.B * rate));
            if (zoomRate == 1F)
                DXManager.Sprite.Draw(texture, Vector2.Zero, new Vector2(x, y), colour);
            else
                DXManager.Sprite.DrawZoom(texture, new Vector2(x * zoomRate + uiOffsetX, y * zoomRate), colour, zoomRate);
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
        /// <param name="RendState"></param>
        public void DrawBlend(int index, float x, float y, Color colour, float sizex, float sizey, float rate, ImageType type, BlendType RendState = BlendType.NONE)
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
                        image.CreateImage(_BReader, FileName, index);
                    }
                    texture = image.Image;
                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid)
                    {
                        image.CreateImage(_BReader, FileName, index);
                    }
                    texture = image.Overlay;
                    break;
            }
            if (texture == null) return;

            bool oldBlend = DXManager.Blending;
            float oldRate = DXManager.BlendRate;
            float scalx = sizex / (float)image.Width;
            float scaly = sizey / (float)image.Height;
            DXManager.Sprite.Transform = Matrix.CreateScale(scalx, scaly, 0f);
            DXManager.SetBlend(true, rate, blendtype: RendState);
            //if (zoomRate == 1F)
            DXManager.Sprite.Draw(texture, Vector2.Zero, new Vector2((x - sizex / 3f) / scalx, (y - sizey / 3f) / scaly), colour);
            //else
            //    DXManager.Sprite.DrawZoom(texture, new Vector2(((x - sizex / 3f) / scalx) * zoomRate + uiOffsetX, ((y - sizey / 3f) / scaly) * zoomRate), colour, zoomRate);

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

                HeadFileStat = 0;
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
        /// 文件名
        /// </summary>
        private byte ImageFileStat;
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
        public BitArray ImageData;
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
        private DateTime NextSyncTime;

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

            return ImageData.Get(p.X + p.Y * w);

            //bool visible = false;
            //switch (ImageTextureType)
            //{
            //    case 32:
            //        int offset = (p.X + p.Y * w) * 4;
            //        int pix = ImageData[offset + 3] << 24 | ImageData[offset + 2] << 16 | ImageData[offset + 1] << 8 | ImageData[offset];
            //        visible = pix != 0;
            //        break;
            //    default:
            //        int x = (p.X - p.X % 4) / 4;
            //        int y = (p.Y - p.Y % 4) / 4;
            //        int index = (y * (w / 4) + x) * 8;

            //        int col0 = ImageData[index + 1] << 8 | ImageData[index], col1 = ImageData[index + 3] << 8 | ImageData[index + 2];

            //        if (col0 == 0 && col1 == 0) return false;

            //        if (!acurrate || col1 < col0) return true;

            //        x = p.X % 4;
            //        y = p.Y % 4;
            //        x *= 2;
            //        visible = (ImageData[index + 4 + y] & 1 << x) >> x != 1 || (ImageData[index + 4 + y] & 1 << x + 1) >> x + 1 != 1; ;
            //        break;
            //}

            //return visible;
        }

        public void InitVisiblePixel(byte[] buffer)
        {
            int w = Width + (4 - Width % 4) % 4;
            int h = Height + (4 - Height % 4) % 4;

            if (ImageData == null)
                ImageData = new BitArray(w * h);

            //for (int y = 0; y < h; y++)
            //{
            //    for (int x = 0; x < w; x++)
            //    {
            //        int pix = buffer[x * y * 4 + 2] << 24 | buffer[x * y * 4 + 1] << 16 | buffer[x * y * 4 + 0] << 8 | buffer[3];
            //        ImageData.Set(i / 4, pix != 0);
            //    }
            //}
            for (int i = 0; i < h * w * 4; i += 4)
            {
                int pix = buffer[i + 2] << 24 | buffer[i + 1] << 16 | buffer[i] << 8 | buffer[i + 3];
                ImageData.Set(i / 4, pix != 0);
            }
        }

        /// <summary>
        /// 处理纹理
        /// </summary>
        public unsafe void DisposeTexture()
        {
            if (Image != null && !Image.Disposed)
                Image.Dispose();

            if (Shadow != null && !Shadow.Disposed)
                Shadow.Dispose();

            if (Overlay != null && !Overlay.Disposed)
                Overlay.Dispose();

            ImageData = null;
            //ShadowData = null;
            //OverlayData = null;

            Image = null;
            Shadow = null;
            Overlay = null;

            ImageValid = false;
            ImageFileStat = 0;
            ShadowValid = false;
            OverlayValid = false;

            ExpireTime = DateTime.MinValue;

            DXManager.TextureList.Remove(this);
        }
        /// <summary>
        /// 创建图像
        /// </summary>
        /// <param name="reader"></param>
        public void CreateImage(BinaryReader reader, string fileName, int index)
        {
            if (Position == 0) return;
            if (Width == 1 && Height == 1) return;
            //如果 1为有微端异步任务；2为正在get数据；3为get数据成功(会直接创建好所有纹理) 返回
            if (ImageFileStat == 1 || ImageFileStat == 2 || ImageFileStat == 3) return;

            //DateTime now = Time.Now;
            lock (reader)
            {
                reader.BaseStream.Seek(Position, SeekOrigin.Begin);
                byte[] buffer = reader.ReadBytes(ImageDataSize);
                var empty = buffer[0] == 0;
                if (empty)//如果是空数据流
                {
                    if (ImageFileStat == 0 && Time.Now > NextSyncTime && LibraryHelper.MicroServerActive)  // 0为初始状态
                    {
                        //限制并发量
                        if (LibraryHelper.Semaphore.CurrentCount > 0)
                        {
                            LibraryHelper.Semaphore.WaitAsync();
                            ImageFileStat = 1;  // 1为开始异步任务
                            Task.Run(async () =>
                            {
                                try
                                {
                                    if (ImageFileStat != 1) return;

                                    ImageFileStat = 2; // 2为开始http get数据

                                    buffer = await LibraryHelper.GetImageAsync(fileName, index, ImageDataSize + ShadowDataSize + OverlayDataSize);
                                    if (buffer?.Length > 0)
                                    {
                                        using (MemoryStream ms = new MemoryStream(buffer))
                                        using (BinaryReader br = new BinaryReader(ms))
                                        {
                                            ImageSetData(br.ReadBytes(ImageDataSize));
                                            if (ShadowDataSize > 0)
                                                ShadowSetData(br.ReadBytes(ShadowDataSize));
                                            else
                                                ShadowValid = true;
                                            if (OverlayDataSize > 0)
                                                OverlaySetData(br.ReadBytes(OverlayDataSize));
                                            else
                                                OverlayValid = true;
                                        }

                                        if (ImageValid)
                                            ImageFileStat = 3; // 3为get数据成功
                                        else
                                        {
                                            //如果get数据不成功，状态置为0 并且1秒后在get
                                            ImageFileStat = 0; //如果微端没有获取到数据 置为0 下次请求会重新获取
                                            NextSyncTime = Time.Now.AddSeconds(1);
                                        }
                                    }
                                    else
                                    {
                                        //如果get数据不成功，状态置为0 并且1秒后在get
                                        ImageFileStat = 0; //如果微端没有获取到数据 置为0 下次请求会重新获取
                                        NextSyncTime = Time.Now.AddSeconds(1);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CEnvir.SaveError(fileName + "  " + index.ToString() + "\r\n" + ex.ToString());
                                }
                                finally
                                {
                                    LibraryHelper.Semaphore.Release();
                                }

                            });
                        }

                    }
                    return;
                }

                try
                {
                    ImageSetData(buffer);
                    if (ShadowDataSize > 0)
                        ShadowSetData(reader.ReadBytes(ShadowDataSize));
                    else
                        ShadowValid = true;
                    if (OverlayDataSize > 0)
                        OverlaySetData(reader.ReadBytes(OverlayDataSize));
                    else
                        OverlayValid = true;
                }
                catch (Exception ex)
                {
                    CEnvir.SaveError(fileName + "  " + index.ToString() + "\r\n" + ex.ToString());
                }

                buffer = null;
            }
            //CEnvir.ImageDelayCounter += (Time.Now - now).Ticks;
        }
        private void ImageSetData(byte[] buffer)
        {
            int w = Width + (4 - Width % 4) % 4;
            int h = Height + (4 - Height % 4) % 4;

            if (w == 0 || h == 0) return;

            SurfaceFormat format = ImageTextureType == 1 ? SurfaceFormat.Dxt1 : ImageTextureType == 3 ? SurfaceFormat.Dxt3 : ImageTextureType == 5 ? SurfaceFormat.Dxt5 : SurfaceFormat.Color;
            Image = new Texture(DXManager.Device, w, h, 1, Usage.None, SurfaceFormat.Color, Pool.Managed);

            if (!IsZirconVersion)
                buffer = Decompress(buffer);

            switch (format)
            {
                case SurfaceFormat.Dxt1:
                    buffer = DxtUtil.DecompressDxt1(buffer, w, h);
                    break;
                case SurfaceFormat.Dxt3:
                    buffer = DxtUtil.DecompressDxt3(buffer, w, h);
                    break;
                case SurfaceFormat.Dxt5:
                    buffer = DxtUtil.DecompressDxt5(buffer, w, h);
                    break;
                    //case SurfaceFormat.Color:
                    //    for (int i = 0; i < h * w * 4; i += 4)
                    //    {
                    //        if (buffer[i + 3] == 0)
                    //        {
                    //            buffer[i] = 0; //透明化处理
                    //            buffer[i + 1] = 0; //透明化处理
                    //            buffer[i + 2] = 0; //透明化处理
                    //            continue;
                    //        }
                    //        //反转红色/蓝色
                    //        byte b = buffer[i];
                    //        buffer[i] = buffer[i + 2];
                    //        buffer[i + 2] = b;

                    //    }
                    //    break;
            }
            Image.SetData(buffer);

            InitVisiblePixel(buffer);

            ImageValid = true;
            ExpireTime = CEnvir.Now + Config.CacheDuration;
            DXManager.TextureList.Add(this);

        }
        /// <summary>
        /// 创建阴影
        /// </summary>
        //public void CreateShadow(BinaryReader reader)
        //{
        //    if (Position == 0 || ShadowDataSize == 0) return;

        //    if (!ImageValid)
        //        CreateImage(reader);

        //    //如果 1为有微端异步任务；2为正在get数据；3为get数据成功(会直接创建好所有纹理) 返回
        //    if (ImageFileStat == 1 || ImageFileStat == 2 || ImageFileStat == 3) return;

        //    lock (reader)
        //    {
        //        reader.BaseStream.Seek(Position + ImageDataSize, SeekOrigin.Begin);
        //        byte[] buffer = reader.ReadBytes(ShadowDataSize);

        //        ShadowSetData(buffer);
        //        buffer = null;
        //    } 
        //}
        private void ShadowSetData(byte[] buffer)
        {
            int w = ShadowWidth + (4 - ShadowWidth % 4) % 4;
            int h = ShadowHeight + (4 - ShadowHeight % 4) % 4;

            if (w == 0 || h == 0) return;

            SurfaceFormat format = ShadowTextureType == 1 ? SurfaceFormat.Dxt1 : ShadowTextureType == 3 ? SurfaceFormat.Dxt3 : ShadowTextureType == 5 ? SurfaceFormat.Dxt5 : SurfaceFormat.Color;
            Shadow = new Texture(DXManager.Device, w, h, 1, Usage.None, SurfaceFormat.Color, Pool.Managed);

            if (!IsZirconVersion)
                buffer = Decompress(buffer);

            switch (format)
            {
                case SurfaceFormat.Dxt1:
                    buffer = DxtUtil.DecompressDxt1(buffer, w, h);
                    break;
                case SurfaceFormat.Dxt3:
                    buffer = DxtUtil.DecompressDxt3(buffer, w, h);
                    break;
                case SurfaceFormat.Dxt5:
                    buffer = DxtUtil.DecompressDxt5(buffer, w, h);
                    break;
                    //case SurfaceFormat.Color:
                    //    for (int i = 0; i < h * w * 4; i += 4)
                    //    {
                    //        //反转红色/蓝色
                    //        byte b = buffer[i];
                    //        buffer[i] = buffer[i + 2];
                    //        buffer[i + 2] = b;
                    //    }
                    //    break;
            }
            Shadow.SetData(buffer);

            ShadowValid = true;
        }
        /// <summary>
        /// 创建叠加
        /// </summary>
        //public void CreateOverlay(BinaryReader reader)
        //{
        //    if (Position == 0 || OverlayDataSize == 0) return;

        //    if (!ImageValid)
        //        CreateImage(reader);

        //    //如果 1为有微端异步任务；2为正在get数据；3为get数据成功(会直接创建好所有纹理) 返回
        //    if (ImageFileStat == 1 || ImageFileStat == 2 || ImageFileStat == 3) return;

        //    lock (reader)
        //    {
        //        reader.BaseStream.Seek(Position + ImageDataSize + ShadowDataSize, SeekOrigin.Begin);
        //        byte[] buffer = reader.ReadBytes(OverlayDataSize);

        //        OverlaySetData(buffer);
        //        buffer = null;
        //    }
        //}

        private void OverlaySetData(byte[] buffer)
        {
            int w = OverlayWidth + (4 - OverlayWidth % 4) % 4;
            int h = OverlayHeight + (4 - OverlayHeight % 4) % 4;

            if (w == 0 || h == 0) return;

            SurfaceFormat format = OverlayTextureType == 1 ? SurfaceFormat.Dxt1 : OverlayTextureType == 3 ? SurfaceFormat.Dxt3 : OverlayTextureType == 5 ? SurfaceFormat.Dxt5 : SurfaceFormat.Color;
            Overlay = new Texture(DXManager.Device, w, h, 1, Usage.None, SurfaceFormat.Color, Pool.Managed);

            if (!IsZirconVersion)
                buffer = Decompress(buffer);

            switch (format)
            {
                case SurfaceFormat.Dxt1:
                    buffer = DxtUtil.DecompressDxt1(buffer, w, h);
                    break;
                case SurfaceFormat.Dxt3:
                    buffer = DxtUtil.DecompressDxt3(buffer, w, h);
                    break;
                case SurfaceFormat.Dxt5:
                    buffer = DxtUtil.DecompressDxt5(buffer, w, h);
                    break;
                    //case SurfaceFormat.Color:
                    //    for (int i = 0; i < h * w * 4; i += 4)
                    //    {
                    //        //反转红色/蓝色
                    //        byte b = buffer[i];
                    //        buffer[i] = buffer[i + 2];
                    //        buffer[i + 2] = b;
                    //    }
                    //    break;
            }
            Overlay.SetData(buffer);

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
