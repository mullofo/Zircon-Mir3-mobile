using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryEditor
{
    public class WTL1to1ZL
    {
        private readonly string _fileName;

        public MImage[] Images;

        private BinaryReader _bReader;
        private int _count;
        private FileStream _fStream;
        private int[] _indexList;
        private bool _initialized;

        WTL1to1ZL shadowLibrary;

        public bool IsNewVersion { get; private set; }

        public WTL1to1ZL(string filename)
        {
            _fileName = Path.ChangeExtension(filename, null);
            Initialize();
            Close();
        }

        public void Initialize()
        {
            _initialized = true;
            if (!File.Exists(_fileName + ".wtl")) return;
            _fStream = new FileStream(_fileName + ".wtl", FileMode.Open, FileAccess.ReadWrite);
            _bReader = new BinaryReader(_fStream);
            LoadImageInfo();

            LMain.form1.toolStripProgressBar.Value = 0;
            LMain.form1.toolStripProgressBar.Maximum = _count;
            for (int i = 0; i < _count; i++)
            {
                CheckImage(i);
                LMain.form1.toolStripProgressBar.Value = i;
            }

            LMain.form1.toolStripProgressBar.Value = 0;
        }

        private void LoadImageInfo()
        {
            _fStream.Seek(2, SeekOrigin.Begin);
            var version = System.Text.Encoding.UTF8.GetString(_bReader.ReadBytes(20)).TrimEnd('\0');
            IsNewVersion = version == "ILIB v2.0-WEMADE";

            _fStream.Seek(28, SeekOrigin.Begin);
            _count = _bReader.ReadInt32();
            _indexList = new int[_count];
            Images = new MImage[_count];

            if (IsNewVersion)
            {
                _bReader.BaseStream.Seek(_fStream.Length - _count * 4, SeekOrigin.Begin);
            }

            for (int i = 0; i < _count; i++)
                _indexList[i] = _bReader.ReadInt32();
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
            Images = null;
            if (shadowLibrary != null)
            {
                shadowLibrary.Dispose();
                shadowLibrary = null;
            }

            GC.Collect();
            GC.SuppressFinalize(this);
        }

        public void CheckImage(int index)
        {
            if (!_initialized)
                Initialize();

            if (index < 0 || index >= Images.Length) return;
            if (_indexList[index] == 0)
            {
                Images[index] = new MImage(index, IsNewVersion);
                return;
            }
            MImage image = Images[index];

            if (image == null)
            {
                _fStream.Seek(_indexList[index], SeekOrigin.Begin);
                image = new MImage(index, IsNewVersion, _bReader);
                Images[index] = image;
                image.CreateTexture(_bReader);
            }
        }

        public void ToMLibrary(bool crypt)
        {
            string fileName = Path.ChangeExtension(_fileName, ".Zl");

            if (File.Exists(fileName))
                File.Delete(fileName);

            BlackDragonLibrary library = new BlackDragonLibrary(fileName);
            //library.Save();

            library.Images.AddRange(Enumerable.Repeat(new BlackDragonLibrary.MImage(), Images.Length));

            string fname = Path.GetFileNameWithoutExtension(_fileName);
            if (fname.StartsWith("Mon-"))
            {
                string shadowPath = Path.Combine(Path.GetDirectoryName(_fileName), fname.Replace("Mon-", "MonS-") + ".wtl");
                shadowLibrary = null;
                if (File.Exists(shadowPath))
                    shadowLibrary = new WTL1to1ZL(shadowPath);
            }


            ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 8 };

            //LMain.form1.setProgressBar(0, Images.Length);
            try
            {
                Parallel.For(0, Images.Length, options, i =>
                {
                    MImage image = Images[i];
                    MImage shadowimage = shadowLibrary != null && i < shadowLibrary.Images.Length ? shadowLibrary.Images[i] : null;
                    //bitmap的宽高已经是4的倍数
                    if (shadowimage != null && shadowimage._fBytes.Length > 0)
                        library.Images[i] = new BlackDragonLibrary.MImage(image._fBytes, shadowimage._fBytes, image._MaskfBytes, image.ImageTextureType, shadowimage.ImageTextureType, image.MaskTextureType)
                        {
                            Width = image.Width,
                            Height = image.Height,
                            OffSetX = image.X,
                            OffSetY = image.Y,
                            ShadowOffSetX = shadowimage.X,
                            ShadowOffSetY = shadowimage.Y,
                            Shadow = image.Shadow,

                            ShadowWidth = shadowimage.Width,
                            ShadowHeight = shadowimage.Height,
                            OverlayWidth = image.MaskWidth,
                            OverlayHeight = image.MaskHeight,
                        };
                    else
                        library.Images[i] = new BlackDragonLibrary.MImage(image._fBytes, null, image._MaskfBytes, image.ImageTextureType, 0, image.MaskTextureType)
                        {
                            Width = image.Width,
                            Height = image.Height,
                            OffSetX = image.X,
                            OffSetY = image.Y,
                            ShadowOffSetX = image.ShadowX,
                            ShadowOffSetY = image.ShadowY,
                            Shadow = image.Shadow,

                            OverlayWidth = image.MaskWidth,
                            OverlayHeight = image.MaskHeight,
                        };
                    //LMain.form1.setProgressBar(i, Images.Length);
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
            public readonly short Width, Height, X, Y, ShadowX, ShadowY;
            public readonly int Length;

            public int DataOffset { get; }

            public readonly byte Shadow;

            public int Index { get; }
            public bool IsNewVersion { get; }
            public byte ImageTextureType { get; }
            public byte MaskTextureType { get; }

            public byte[] _fBytes;
            //public Bitmap Image;

            public readonly Boolean HasMask;
            public short MaskWidth, MaskHeight, MaskX, MaskY;
            public int MaskLength;
            public byte[] _MaskfBytes;
            //public Bitmap MaskImage;

            public MImage(int index, bool isNewVersion)
            {
                Index = index;
                IsNewVersion = isNewVersion;
                _fBytes = new byte[0];
                _MaskfBytes = new byte[0];
            }

            public MImage(int index, bool isNewVersion, BinaryReader bReader)
            {
                Index = index;
                IsNewVersion = isNewVersion;
                Width = bReader.ReadInt16();
                Height = bReader.ReadInt16();
                X = bReader.ReadInt16();
                Y = bReader.ReadInt16();
                ShadowX = bReader.ReadInt16();
                ShadowY = bReader.ReadInt16();

                if (IsNewVersion)
                {
                    var imageU1 = bReader.ReadByte();
                    ImageTextureType = bReader.ReadByte();
                    var maskU1 = bReader.ReadByte();
                    MaskTextureType = bReader.ReadByte();

                    HasMask = MaskTextureType > 0;
                    Length = bReader.ReadInt32();
                    if (Length % 4 > 0) Length += 4 - (Length % 4);
                    DataOffset = (int)bReader.BaseStream.Position;
                }
                else
                {
                    Length = bReader.ReadByte() | bReader.ReadByte() << 8 | bReader.ReadByte() << 16;
                    Shadow = bReader.ReadByte();
                    HasMask = ((Shadow >> 7) == 1) ? true : false;
                    ImageTextureType = 1;
                    MaskTextureType = 1;
                }
            }

            public unsafe void CreateTexture(BinaryReader bReader)
            {
                _fBytes = ReadImage(bReader, Length, Width, ImageTextureType);
                if (HasMask)
                {
                    if (IsNewVersion)
                    {
                        MaskWidth = Width;
                        MaskHeight = Height;
                        MaskX = X;
                        MaskY = Y;
                        MaskLength = bReader.ReadInt32();
                    }
                    else
                    {
                        MaskWidth = bReader.ReadInt16();
                        MaskHeight = bReader.ReadInt16();
                        MaskX = bReader.ReadInt16();
                        MaskY = bReader.ReadInt16();
                        bReader.ReadInt16();//mask shadow x
                        bReader.ReadInt16();//mask shadow y
                        MaskLength = bReader.ReadByte() | bReader.ReadByte() << 8 | bReader.ReadByte() << 16;
                        bReader.ReadByte(); //mask shadow
                    }

                    _MaskfBytes = ReadImage(bReader, MaskLength, MaskWidth, MaskTextureType);
                }
            }

            private byte[] DecompressV1Texture(BinaryReader bReader, int imageLength, short outputWidth)
            {
                const int size = 8;
                int blockOffSet = 0;
                List<byte> countList = new List<byte>();
                List<byte> fBytesList = new List<byte>();
                int currentx = 0;
                int currenty = 0;

                int tWidth = 4;
                while (tWidth < Width)
                    tWidth *= 2;

                byte[] buffer = bReader.ReadBytes(imageLength);

                if (buffer.Length != imageLength) return null;

                while (blockOffSet < imageLength)
                {
                    countList.Clear();
                    for (int i = 0; i < 8; i++)
                        countList.Add(buffer[blockOffSet++]);

                    for (int i = 0; i < countList.Count; i++)
                    {
                        int count = countList[i];

                        if (i % 2 == 0)
                        {
                            if (currentx >= tWidth)
                            {
                                currentx -= tWidth;
                                currenty += 4;
                            }

                            for (int off = 0; off < count; off++)
                            {
                                if (currentx < Width + (4 - Width % 4) % 4 && currenty < Height + (4 - Height % 4) % 4)
                                {
                                    byte[] block = new byte[size] { 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff };
                                    fBytesList.AddRange(block);
                                }

                                currentx += 4;

                                if (currentx >= tWidth)
                                {
                                    currentx -= tWidth;
                                    currenty += 4;
                                }
                            }
                            continue;
                        }

                        for (int c = 0; c < count; c++)
                        {
                            if (blockOffSet >= buffer.Length)
                                break;

                            if (currentx < Width + (4 - Width % 4) % 4 && currenty < Height + (4 - Height % 4) % 4)
                            {
                                byte[] block = new byte[size];

                                Array.Copy(buffer, blockOffSet, block, 0, size);
                                blockOffSet += size;
                                fBytesList.AddRange(block);
                            }

                            currentx += 4;
                            if (currentx >= tWidth)
                            {
                                currentx -= tWidth;
                                currenty += 4;
                            }
                        }
                    }
                }

                return Ionic.Zlib.DeflateStream.CompressBuffer(fBytesList.ToArray());
            }


            private byte[] DecompressV2Texture(BinaryReader bReader, int imageLength, byte textureType)
            {
                var buffer = Ionic.Zlib.DeflateStream.UncompressBuffer(bReader.ReadBytes(imageLength));
                int size = textureType == 1 ? 8 : 16;
                int blockOffSet = 0;
                List<byte> fBytesList = new List<byte>();
                int currentx = 0;
                int currenty = 0;

                int tWidth = 4;
                while (tWidth < Width)
                    tWidth *= 2;

                while (blockOffSet < buffer.Length)
                {
                    if (blockOffSet >= buffer.Length)
                        break;

                    if (currentx >= tWidth)
                    {
                        currentx -= tWidth;
                        currenty += 4;
                    }

                    if (currentx < Width + (4 - Width % 4) % 4 && currenty < Height + (4 - Height % 4) % 4)
                    {
                        byte[] block = new byte[size];

                        Array.Copy(buffer, blockOffSet, block, 0, size);
                        fBytesList.AddRange(block);
                    }

                    currentx += 4;
                    if (currentx >= tWidth)
                    {
                        currentx -= tWidth;
                        currenty += 4;
                    }

                    blockOffSet += size;

                }
                return Ionic.Zlib.DeflateStream.CompressBuffer(fBytesList.ToArray());
            }

            public byte[] ReadImage(BinaryReader bReader, int imageLength, short outputWidth, byte textureType)
            {
                return IsNewVersion ? DecompressV2Texture(bReader, imageLength, textureType) : DecompressV1Texture(bReader, imageLength, outputWidth);
            }

        }

    }
}