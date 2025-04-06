using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryEditor
{
    /// <summary>
    /// 传奇正传素材
    /// </summary>
    public class CQZZLibrary
    {
        public MImage[] Images;

        private readonly string _fileName;

        private int[] _indexList;
        private int _version = 0;

        private BinaryReader _bReader;
        private FileStream _fStream;
        private string _MainExtention = ".dat";
        private string _IndexExtention = ".idx";

        private bool _initialized;


        public CQZZLibrary(string name)
        {
            _fileName = Path.ChangeExtension(name, null);
            Initialize();
        }

        public void Initialize()
        {
            _initialized = true;

            if (!File.Exists(_fileName + _IndexExtention)) return;
            if (!File.Exists(_fileName + _MainExtention)) return;

            _fStream = new FileStream(_fileName + _MainExtention, FileMode.Open, FileAccess.Read);
            _bReader = new BinaryReader(_fStream);
            LoadImageInfo();
        }

        private void LoadImageInfo()
        {
            _fStream.Seek(0, SeekOrigin.Begin);
            _bReader.ReadInt32(); //0x15 ???
            _fStream.Seek(4, SeekOrigin.Current);
            _bReader.ReadInt32(); //0x1600 ???
            _fStream.Seek(4, SeekOrigin.Current);
            _bReader.ReadInt32(); //0x3558 ???version??

            LoadIndexFile();
        }

        private void LoadIndexFile()
        {
            FileStream stream = null;

            try
            {
                stream = new FileStream(_fileName + _IndexExtention, FileMode.Open, FileAccess.Read);
                stream.Seek(0, SeekOrigin.Begin);
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    reader.ReadInt32(); //0x15 ???
                    stream.Seek(4, SeekOrigin.Current);
                    int count = reader.ReadInt32(); //图片数量
                    int groupcount = reader.ReadInt32(); //图片分组数量
                    reader.ReadInt32(); //0x3558 ???version??
                    reader.ReadByte(); // ??

                    _indexList = new int[count];
                    Images = new MImage[count];

                    LMain.form1.toolStripProgressBar.Value = 0;
                    LMain.form1.toolStripProgressBar.Maximum = count;
                    for (int i = 0; i < count; i++)
                    {
                        var unknow1 = reader.ReadInt32(); //???
                        int compresssize = reader.ReadInt32(); //压缩大小
                        _indexList[i] = reader.ReadInt32(); //数据索引位
                        var width = reader.ReadUInt16(); //width
                        var height = reader.ReadUInt16(); //height

                        CheckImage(i, compresssize);
                        LMain.form1.toolStripProgressBar.Value = i;
                    }

                    LMain.form1.toolStripProgressBar.Value = 0;
                }
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }

        private void CheckImage(int index, int size)
        {
            if (!_initialized) Initialize();
            if (Images == null || index < 0 || index >= Images.Length) return;
            if (Images[index] == null)
            {
                _fStream.Position = _indexList[index];
                Images[index] = new MImage(_bReader, size) { index = index };
            }

            if (Images[index].Image == null)
            {
                _fStream.Seek(_indexList[index] + 17, SeekOrigin.Begin);
                Images[index].CreateTexture(_bReader);
            }
        }
        public void Dispose()
        {
            Images = null;
            GC.Collect();
            GC.SuppressFinalize(this);
        }

        public void ToMLibrary(byte imageTextureType, byte shadowTextureType, byte overlayTextureType, bool crypt)
        {
            string fileName = Path.ChangeExtension(_fileName, ".Zl");

            if (File.Exists(fileName))
                File.Delete(fileName);

            BlackDragonLibrary library = new BlackDragonLibrary(fileName);
            //library.Save();

            library.Images.AddRange(Enumerable.Repeat(new BlackDragonLibrary.MImage(), Images.Length));

            string fname = Path.GetFileNameWithoutExtension(_fileName);

            ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 8 };

            try
            {
                Parallel.For(0, Images.Length, options, i =>
                    {
                        MImage image = Images[i];
                        //MImage shadowimage = shadowLibrary != null && i < shadowLibrary.Images.Length ? shadowLibrary.Images[i] : null;
                        //bitmap的宽高已经是4的倍数
                        //if (shadowimage != null && shadowimage.Image != null)
                        //    library.Images[i] = new BlackDragonLibrary.MImage(image.Image, shadowimage.Image, image.MaskImage, imageTextureType, shadowTextureType, overlayTextureType)
                        //    {
                        //        Width = image.Width,
                        //        Height = image.Height,
                        //        OffSetX = image.OffSetX,
                        //        OffSetY = image.OffSetY,
                        //        ShadowOffSetX = shadowimage.OffSetX,
                        //        ShadowOffSetY = shadowimage.OffSetY,

                        //        ShadowWidth = shadowimage.Width,
                        //        ShadowHeight = shadowimage.Height,
                        //        OverlayWidth = image.Width,
                        //        OverlayHeight = image.Height,
                        //    };
                        //else
                        library.Images[i] = new BlackDragonLibrary.MImage(image.Image, null, image.MaskImage, imageTextureType, shadowTextureType, overlayTextureType)
                        {
                            Width = image.Width,
                            Height = image.Height,
                            OffSetX = image.OffSetX,
                            OffSetY = image.OffSetY,
                            ShadowOffSetX = image.ShadowOffSetX,
                            ShadowOffSetY = image.ShadowOffSetY,
                            OverlayWidth = image.Width,
                            OverlayHeight = image.Height,
                        };
                    });
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                if (crypt)
                {
                    library.Key = new byte[8];
                    System.Text.Encoding.ASCII.GetBytes(CryptForm.form1.keyTextBox.Text, 0, CryptForm.form1.keyTextBox.Text.Length, library.Key, 0);
                    library.Crypt = true;
                }
                library.Save(fileName);
            }

            // Operation finished.
            // System.Windows.Forms.MessageBox.Show("Converted " + fileName + " successfully.",
            //    "Wemade Information",
            //        System.Windows.Forms.MessageBoxButtons.OK,
            //            System.Windows.Forms.MessageBoxIcon.Information,
            //                System.Windows.Forms.MessageBoxDefaultButton.Button1);
        }

        public class MImage
        {
            public int index;
            public readonly short Width, Height, OffSetX, OffSetY, ShadowOffSetX, ShadowOffSetY;
            public Bitmap Image;
            public byte nType;
            public int nSize;
            //public bool boHasShadow;
            //public bool HasMask;
            public Bitmap MaskImage;

            private int convert16bitTo32bit(int color, byte alpha = 255)
            {
                byte red = (byte)((color & 0xf800) >> 8);
                byte green = (byte)((color & 0x07e0) >> 3);
                byte blue = (byte)((color & 0x001f) << 3);
                return ((red << 0x10) | (green << 0x8) | blue) | (alpha << 24);
            }

            public MImage(BinaryReader reader, int size)
            {
                if (reader.BaseStream.Position == 0) return;

                Width = reader.ReadInt16();
                Height = reader.ReadInt16();
                OffSetX = reader.ReadInt16();
                OffSetY = reader.ReadInt16();
                int compresssize = reader.ReadInt32();
                int decompresssize = reader.ReadInt32();
                nType = reader.ReadByte(); //??

                nSize = size;


            }

            public unsafe void CreateTexture(BinaryReader reader)
            {
                if (Width == 0 || Height == 0) return;
                Image = new Bitmap(Width, Height);
                //MaskImage = new Bitmap(1, 1);

                BitmapData data = Image.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                byte[] bytes = new byte[0];
                //byte[] maskbytes = new byte[0];

                bytes = Ionic.Zlib.ZlibStream.UncompressBuffer(reader.ReadBytes(nSize));

                if (bytes.Length <= 1)
                {
                    Image.UnlockBits(data);
                    Image.Dispose();
                    Image = null;
                    MaskImage.Dispose();
                    return;
                }

                int index = 0;
                int* scan0 = (int*)data.Scan0;
                {
                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            byte alpha = nType == 4 ? bytes[index++] : (byte)255;
                            scan0[y * Width + x] = convert16bitTo32bit(bytes[index++] + (bytes[index++] << 8), alpha);
                        }
                    }
                }
                Image.UnlockBits(data);

                //if (HasMask)
                //{
                //    BitmapData Maskdata = MaskImage.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                //    int* maskscan0 = (int*)Maskdata.Scan0;
                //    {
                //        for (int y = Height - 1; y >= 0; y--)
                //        {
                //            for (int x = 0; x < Width; x++)
                //            {
                //                if (nType == 3)
                //                {
                //                    maskscan0[y * Width + x] = BitConverter.ToInt32(maskbytes, index);
                //                    index += 4;
                //                    continue;
                //                }

                //                maskscan0[y * Width + x] = convert16bitTo32bit(maskbytes[index++] + (maskbytes[index++] << 8));
                //            }
                //        }
                //    }
                //    MaskImage.UnlockBits(Maskdata);
                //}
            }
        }
    }
}