using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryEditor
{
    public partial class LMain : Form
    {
        private readonly Dictionary<int, int> _indexList = new Dictionary<int, int>();
        public static BlackDragonLibrary _library;
        private BlackDragonLibrary.MImage _selectedImage, _exportImage;
        private Image _originalImage;
        public static LMain form1; //其他类调用窗体控件
        public delegate void DelegateProgressBar(int value, int maxvalue);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        public LMain()
        {
            InitializeComponent();
            form1 = this; //其他类调用窗体控件
            SendMessage(PreviewListView.Handle, 4149, 0, 5242946); //80 x 66
            PreviewListView.Size = splitContainer1.Panel2.Size;

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            if (Program.openFileWith.Length > 0 &&
                File.Exists(Program.openFileWith))
            {
                OpenLibraryDialog.FileName = Program.openFileWith;
                _library = new BlackDragonLibrary(OpenLibraryDialog.FileName);
                PreviewListView.VirtualListSize = _library.Images.Count;

                // Show .Lib path in application title.
                this.Text = OpenLibraryDialog.FileName.ToString();

                PreviewListView.SelectedIndices.Clear();

                if (PreviewListView.Items.Count > 0)
                    PreviewListView.Items[0].Selected = true;

                radioButtonImage.Enabled = true;
                radioButtonShadow.Enabled = true;
                radioButtonOverlay.Enabled = true;
            }
        }
        //
        public void setProgressBar(int value, int maxvlue)
        {
            if (toolStripProgressBar.ProgressBar.InvokeRequired) //控件是否跨线程？如果是，则执行括号里代码
            {
                DelegateProgressBar Callback = new DelegateProgressBar(setProgressBar); //实例化委托对象
                toolStripProgressBar.ProgressBar.Invoke(Callback, value, maxvlue); //重新调用setProgressBar函数
            }
            else //否则，即是本线程的控件，控件直接操作
            {
                toolStripProgressBar.Value = value;
                toolStripProgressBar.Maximum = maxvlue;
            }
        }
        //文件拖出
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            return; //尚未添加

            /*string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (Path.GetExtension(files[0]).ToUpper() == ".WIL" ||
                Path.GetExtension(files[0]).ToUpper() == ".WZL" ||
                Path.GetExtension(files[0]).ToUpper() == ".MIZ")
            {
                try
                {
                    ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 8 };
                    Parallel.For(0, files.Length, options, i =>
                    {
                        if (Path.GetExtension(files[i]) == ".wtl")
                        {
                            WTLLibrary WTLlib = new WTLLibrary(files[i]);
                            WTLlib.ToMLibrary();
                        }
                        else
                        {
                            WeMadeLibrary WILlib = new WeMadeLibrary(files[i]);
                            WILlib.ToMLibrary();
                        }
                        toolStripProgressBar.Value++;
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                toolStripProgressBar.Value = 0;

                MessageBox.Show(
                    string.Format("已成功转换 {0} {1}",
                    (OpenWeMadeDialog.FileNames.Length).ToString(),
                    (OpenWeMadeDialog.FileNames.Length > 1) ? "libraries" : "library"));
            }
            else if (Path.GetExtension(files[0]).ToUpper() == ".LIB")
            {
                ClearInterface();
                ImageList.Images.Clear();
                PreviewListView.Items.Clear();
                _indexList.Clear();

                if (_library != null) _library.Close();
                //_library = new MLibraryV2(files[0]);
                //PreviewListView.VirtualListSize = _library.Images.Count;
                PreviewListView.RedrawItems(0, PreviewListView.Items.Count - 1, true);

                // Show .Lib path in application title.
                this.Text = files[0].ToString();
            }
            else
            {
                return;
            }*/
        }
        //文件拖入
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        // Don't let the splitter go out of sight on resizing.
        private void LMain_Resize(object sender, EventArgs e)
        {
            if (splitContainer1.SplitterDistance <= this.Height - 150) return;
            if (this.Height - 150 > 0)
            {
                splitContainer1.SplitterDistance = this.Height - 150;
            }
        }
        //图像按钮事件
        private void radioButtonImage_CheckedChanged(object sender, EventArgs e)
        {
            int index = PreviewListView.SelectedIndices[0];
            ImageList.Images.Clear();
            PreviewListView.Items.Clear();
            _indexList.Clear();

            PreviewListView.VirtualListSize = 0;
            PreviewListView.VirtualListSize = _library.Images.Count;

            OffSetXTextBox.Enabled = true;
            OffSetYTextBox.Enabled = true;
            ShadowOffSetXTextBox.Enabled = true;
            ShadowOffSetYTextBox.Enabled = true;
            ShadowTextBox.Enabled = true;
            ImportButton.Enabled = true;
            DeleteButton.Enabled = true;

            PreviewListView.Items[index].Selected = true;
            PreviewListView.Items[index].EnsureVisible();
        }
        //影子按钮事件
        private void radioButtonShadow_CheckedChanged(object sender, EventArgs e)
        {
            int index = PreviewListView.SelectedIndices[0];
            ImageList.Images.Clear();
            PreviewListView.Items.Clear();
            _indexList.Clear();

            PreviewListView.VirtualListSize = 0;
            PreviewListView.VirtualListSize = _library.Images.Count;

            OffSetXTextBox.Enabled = false;
            OffSetYTextBox.Enabled = false;
            ShadowOffSetXTextBox.Enabled = false;
            ShadowOffSetYTextBox.Enabled = false;
            ShadowTextBox.Enabled = false;
            ImportButton.Enabled = true;
            DeleteButton.Enabled = true;

            PreviewListView.Items[index].Selected = true;
            PreviewListView.Items[index].EnsureVisible();
        }
        //覆盖按钮事件
        private void radioButtonOverlay_CheckedChanged(object sender, EventArgs e)
        {
            int index = PreviewListView.SelectedIndices[0];
            ImageList.Images.Clear();
            PreviewListView.Items.Clear();
            _indexList.Clear();

            PreviewListView.VirtualListSize = 0;
            PreviewListView.VirtualListSize = _library.Images.Count;

            OffSetXTextBox.Enabled = false;
            OffSetYTextBox.Enabled = false;
            ShadowOffSetXTextBox.Enabled = false;
            ShadowOffSetYTextBox.Enabled = false;
            ShadowTextBox.Enabled = false;
            ImportButton.Enabled = true;
            DeleteButton.Enabled = true;

            PreviewListView.Items[index].Selected = true;
            PreviewListView.Items[index].EnsureVisible();
        }
        //内存使用timer
        private void MemUsagetimer_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = string.Format("物理内存: {0}   虚拟内存: {1}",
                                        ProcessMonitor.GetPhysicalMemoryUsage(true),
                                        ProcessMonitor.GetPagedMemorySize64(true));
        }

        private void ClearInterface()
        {
            _selectedImage = null;
            ImageBox.Image = null;
            ZoomTrackBar.Value = 1;

            WidthLabel.Text = "<空>";
            HeightLabel.Text = "<空>";
            OffSetXTextBox.Text = string.Empty;
            OffSetYTextBox.Text = string.Empty;
            ShadowOffSetXTextBox.Text = string.Empty;
            ShadowOffSetYTextBox.Text = string.Empty;
            ShadowTextBox.Text = string.Empty;

            OffSetXTextBox.BackColor = SystemColors.Window;
            OffSetYTextBox.BackColor = SystemColors.Window;
            ShadowOffSetXTextBox.BackColor = SystemColors.Window;
            ShadowOffSetYTextBox.BackColor = SystemColors.Window;
            ShadowTextBox.BackColor = SystemColors.Window;
        }

        #region 工具栏
        //新建
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SaveLibraryDialog.ShowDialog() != DialogResult.OK) return;

            if (_library != null) _library.Close();
            _library = new BlackDragonLibrary(SaveLibraryDialog.FileName);
            PreviewListView.VirtualListSize = 1;
            _library.Save(SaveLibraryDialog.FileName);
        }
        //打开
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenLibraryDialog.ShowDialog() != DialogResult.OK) return;
            //MessageBox.Show(OpenLibraryDialog.FileName);
            ClearInterface();
            ImageList.Images.Clear();
            PreviewListView.Items.Clear();
            PreviewListView.VirtualListSize = 0;
            _indexList.Clear();

            if (_library != null)
            {
                _library.Close();
                //增加内存回收
                _library.Dispose();
            }
            _library = new BlackDragonLibrary(OpenLibraryDialog.FileName);

            PreviewListView.VirtualListSize = _library.Images.Count;

            // Show .Lib path in application title.
            this.Text = OpenLibraryDialog.FileName.ToString();

            PreviewListView.SelectedIndices.Clear();

            if (PreviewListView.Items.Count > 0)
                PreviewListView.Items[0].Selected = true;

            radioButtonImage.Enabled = true;
            radioButtonShadow.Enabled = true;
            radioButtonOverlay.Enabled = true;
        }
        //保存
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            _library.Save(_library.FileName);
            MessageBox.Show("保存成功");
        }
        //另存为
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (SaveLibraryDialog.ShowDialog() != DialogResult.OK) return;

            _library._fileName = SaveLibraryDialog.FileName;
            _library.Save(_library._fileName);
            MessageBox.Show("保存成功");
        }
        //关闭
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //To Dxt1
        private void toolStripMenuItemDXT1_Click(object sender, EventArgs e)
        {
            CovertToLibrary(1);
        }
        //To ABGR32 手游格式
        private void toolStripMenuItemABGR32_Click(object sender, EventArgs e)
        {
            //CovertToLibrary(5);

            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "WeMade|*.Zl";
            openDialog.Multiselect = true;

            if (openDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 1 };
                Parallel.For(0, openDialog.FileNames.Length, options, i =>
                {
                    BlackDragonLibrary lib = new BlackDragonLibrary(openDialog.FileNames[i], moblie: true);
                    lib.Save(openDialog.FileNames[i]);
                    lib.Dispose();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            MessageBox.Show(string.Format("已成功转换 {0} {1}",
                (openDialog.FileNames.Length).ToString(),
                (openDialog.FileNames.Length > 1) ? "libraries" : "library"));
        }
        //To ARGB32
        private void toolStripMenuItemARGB32_Click(object sender, EventArgs e)
        {
            CovertToLibrary(32);
        }
        //WTL 1to1 ZL
        private void toolStripMenuItemWTL_1to1_ZL_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "WeMade|*.Wtl";
            openDialog.Multiselect = true;

            if (openDialog.ShowDialog() != DialogResult.OK) return;

            //加密相关
            bool crypt;
            if (new CryptForm().ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(CryptForm.form1.keyTextBox.Text))
                crypt = false;
            else
                crypt = true;

            //toolStripProgressBar.Maximum = OpenWeMadeDialog.FileNames.Length;
            //toolStripProgressBar.Value = 0;
            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 1 };
                Parallel.For(0, openDialog.FileNames.Length, options, i =>
                {
                    WTL1to1ZL WTLlib = new WTL1to1ZL(openDialog.FileNames[i]);
                    WTLlib.ToMLibrary(crypt);
                    WTLlib.Dispose();
                    //toolStripProgressBar.Value++;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            //toolStripProgressBar.Value = 0;

            MessageBox.Show(string.Format("已成功转换 {0} {1}",
                (openDialog.FileNames.Length).ToString(),
                (openDialog.FileNames.Length > 1) ? "libraries" : "library"));
        }

        //转换素材函数，转换影子和覆盖层默认用DXT1格式
        private void CovertToLibrary(byte imageTextureType, byte shadowTextureType = 1, byte overlayTextureType = 1)
        {
            if (OpenWeMadeDialog.ShowDialog() != DialogResult.OK) return;

            //加密相关
            bool crypt;
            if (new CryptForm().ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(CryptForm.form1.keyTextBox.Text))
                crypt = false;
            else
                crypt = true;

            //toolStripProgressBar.Maximum = OpenWeMadeDialog.FileNames.Length;
            //toolStripProgressBar.Value = 0;

            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 1 };
                Parallel.For(0, OpenWeMadeDialog.FileNames.Length, options, i =>
                {
                    if (Path.GetExtension(OpenWeMadeDialog.FileNames[i]) == ".wtl")
                    {
                        WTLLibrary WTLlib = new WTLLibrary(OpenWeMadeDialog.FileNames[i]);
                        WTLlib.ToMLibrary(imageTextureType, shadowTextureType, overlayTextureType, crypt);
                        WTLlib.Dispose();
                    }
                    else if (Path.GetExtension(OpenWeMadeDialog.FileNames[i]) == ".Lib")
                    {
                        FileStream stream = new FileStream(OpenWeMadeDialog.FileNames[i], FileMode.Open, FileAccess.ReadWrite);
                        BinaryReader reader = new BinaryReader(stream);
                        int CurrentVersion = reader.ReadInt32();
                        stream.Close();
                        stream.Dispose();
                        reader.Dispose();
                        if (CurrentVersion == 1)
                        {
                            MLibraryV1 v1Lib = new MLibraryV1(OpenWeMadeDialog.FileNames[i]);
                            v1Lib.ToMLibrary(imageTextureType, shadowTextureType, overlayTextureType, crypt);
                            v1Lib.Dispose();
                        }
                        else
                        {
                            MLibraryV2 v2Lib = new MLibraryV2(OpenWeMadeDialog.FileNames[i]);
                            v2Lib.ToMLibrary(imageTextureType, shadowTextureType, overlayTextureType, crypt);
                            v2Lib.Dispose();
                        }
                    }
                    else if (Path.GetExtension(OpenWeMadeDialog.FileNames[i]) == ".ZL")
                    {
                        ZirconLibrary ZlLib = new ZirconLibrary(OpenWeMadeDialog.FileNames[i]);
                        ZlLib.ToMLibrary(imageTextureType, shadowTextureType, overlayTextureType, crypt);
                        ZlLib.Dispose();
                    }
                    else if (Path.GetExtension(OpenWeMadeDialog.FileNames[i]).ToUpper() == ".DAT" || Path.GetExtension(OpenWeMadeDialog.FileNames[i]).ToUpper() == ".IDX")
                    {
                        CQZZLibrary WILlib = new CQZZLibrary(OpenWeMadeDialog.FileNames[i]);
                        WILlib.ToMLibrary(imageTextureType, shadowTextureType, overlayTextureType, crypt);
                        WILlib.Dispose();
                    }
                    else
                    {
                        WeMadeLibrary WILlib = new WeMadeLibrary(OpenWeMadeDialog.FileNames[i]);
                        WILlib.ToMLibrary(imageTextureType, shadowTextureType, overlayTextureType, crypt);
                        WILlib.Dispose();
                    }
                    //toolStripProgressBar.Value++;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            //toolStripProgressBar.Value = 0;

            MessageBox.Show(string.Format("已成功转换 {0} {1}",
                (OpenWeMadeDialog.FileNames.Length).ToString(),
                (OpenWeMadeDialog.FileNames.Length > 1) ? "libraries" : "library"));
        }

        private void copyToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (PreviewListView.SelectedIndices.Count == 0) return;
            //if (SaveLibraryDialog.ShowDialog() != DialogResult.OK) return;

            //BlackDragonLibrary tempLibrary = new BlackDragonLibrary(SaveLibraryDialog.FileName);

            //List<int> copyList = new List<int>();

            //for (int i = 0; i < PreviewListView.SelectedIndices.Count; i++)
            //    copyList.Add(PreviewListView.SelectedIndices[i]);

            //copyList.Sort();

            //for (int i = 0; i < copyList.Count; i++)
            //{
            //    BlackDragonLibrary.MImage image = _library.GetImage(copyList[i]);
            //    tempLibrary.AddImage(image.Image, image.OffSetX, image.OffSetY);
            //}

            //tempLibrary.Save(SaveLibraryDialog.FileName);
        }
        //删除空图
        private void removeBlanksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确实要删除空白图片吗?",
                "删除空白图片",
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            _library.RemoveBlanks();
            ImageList.Images.Clear();
            _indexList.Clear();
            PreviewListView.VirtualListSize = _library.Images.Count;
        }

        private void safeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确实要删除空白图片吗?",
                "删除空白图片", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            _library.RemoveBlanks(true);
            ImageList.Images.Clear();
            _indexList.Clear();
            PreviewListView.VirtualListSize = _library.Images.Count;
        }

        private void countBlanksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //OpenLibraryDialog.Multiselect = true;

            //if (OpenLibraryDialog.ShowDialog() != DialogResult.OK)
            //{
            //    OpenLibraryDialog.Multiselect = false;
            //    return;
            //}

            //OpenLibraryDialog.Multiselect = false;

            //MLibraryV2.Load = false;

            //int count = 0;

            //for (int i = 0; i < OpenLibraryDialog.FileNames.Length; i++)
            //{
            //    MLibraryV2 library = new MLibraryV2(OpenLibraryDialog.FileNames[i]);

            //    for (int x = 0; x < library.Count; x++)
            //    {
            //        if (library.Images[x].Length <= 8)
            //            count++;
            //    }

            //    library.Close();
            //}

            //MLibraryV2.Load = true;
            //MessageBox.Show(count.ToString());

            int index = PreviewListView.SelectedIndices[0] + 1;
            _library?.Images?.Insert(index, null);
            ImageList.Images.Clear();
            _indexList.Clear();
            PreviewListView.VirtualListSize = _library.Images.Count;
        }

        private void encodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library._fileName == null) return;
            if (_library.Images == null) return;
            if (_library.Crypt)
            {
                MessageBox.Show("文件已经加密，请先取消加密。。。", "错误", MessageBoxButtons.OK);
                return;
            }
            if (new CryptForm().ShowDialog() != DialogResult.OK) return;
            if (string.IsNullOrEmpty(CryptForm.form1.keyTextBox.Text)) return;

            _library.Key = new byte[8];
            System.Text.Encoding.ASCII.GetBytes(CryptForm.form1.keyTextBox.Text, 0, CryptForm.form1.keyTextBox.Text.Length, _library.Key, 0);
            _library.Crypt = true;
            _library.Save(_library.FileName);
        }

        private void decodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library._fileName == null) return;
            if (_library.Images == null) return;
            if (!_library.Crypt)
            {
                MessageBox.Show("文件未加密", "提示", MessageBoxButtons.OK);
                return;
            }
            if (new CryptForm().ShowDialog() != DialogResult.OK) return;

            if (System.Text.Encoding.ASCII.GetString(_library.Key).TrimEnd('\0') == CryptForm.form1.keyTextBox.Text)
            {
                _library.Key = new byte[8];
                _library.Crypt = false;
                _library.Save(_library.FileName);
            }
            else
                MessageBox.Show("密钥错误！！！", "错误", MessageBoxButtons.OK);
        }
        #endregion

        #region 按钮
        //添加图像
        //private void AddButton_Click(object sender, EventArgs e)
        //{
        //    //if (_library == null) return;
        //    //if (_library._fileName == null) return;

        //    //if (ImportImageDialog.ShowDialog() != DialogResult.OK) return;

        //    //List<string> fileNames = new List<string>(ImportImageDialog.FileNames);

        //    ////fileNames.Sort();
        //    //toolStripProgressBar.Value = 0;
        //    //toolStripProgressBar.Maximum = fileNames.Count;

        //    //for (int i = 0; i < fileNames.Count; i++)
        //    //{
        //    //    string fileName = fileNames[i];
        //    //    Bitmap image;

        //    //    try
        //    //    {
        //    //        image = new Bitmap(fileName);
        //    //    }
        //    //    catch
        //    //    {
        //    //        continue;
        //    //    }

        //    //    fileName = Path.Combine(Path.GetDirectoryName(fileName), "Placements", Path.GetFileNameWithoutExtension(fileName));
        //    //    fileName = Path.ChangeExtension(fileName, ".txt");

        //    //    short x = 0;
        //    //    short y = 0;

        //    //    if (File.Exists(fileName))
        //    //    {
        //    //        string[] placements = File.ReadAllLines(fileName);

        //    //        if (placements.Length > 0)
        //    //            short.TryParse(placements[0], out x);
        //    //        if (placements.Length > 1)
        //    //            short.TryParse(placements[1], out y);
        //    //    }

        //    //    _library.AddImage(image, x, y);
        //    //    toolStripProgressBar.Value++;
        //    //    //image.Dispose();
        //    //}

        //    //PreviewListView.VirtualListSize = _library.Images.Count;
        //    //toolStripProgressBar.Value = 0;
        //}
        ////插入图像
        //private void InsertImageButton_Click(object sender, EventArgs e)
        //{
        //    //if (_library == null) return;
        //    //if (_library.FileName == null) return;
        //    //if (PreviewListView.SelectedIndices.Count == 0) return;
        //    //if (ImportImageDialog.ShowDialog() != DialogResult.OK) return;

        //    //List<string> fileNames = new List<string>(ImportImageDialog.FileNames);

        //    ////fileNames.Sort();

        //    //int index = PreviewListView.SelectedIndices[0];

        //    //toolStripProgressBar.Value = 0;
        //    //toolStripProgressBar.Maximum = fileNames.Count;

        //    //for (int i = fileNames.Count - 1; i >= 0; i--)
        //    //{
        //    //    string fileName = fileNames[i];

        //    //    Bitmap image;

        //    //    try
        //    //    {
        //    //        image = new Bitmap(fileName);
        //    //    }
        //    //    catch
        //    //    {
        //    //        continue;
        //    //    }

        //    //    fileName = Path.Combine(Path.GetDirectoryName(fileName), "Placements", Path.GetFileNameWithoutExtension(fileName));
        //    //    fileName = Path.ChangeExtension(fileName, ".txt");

        //    //    short x = 0;
        //    //    short y = 0;

        //    //    if (File.Exists(fileName))
        //    //    {
        //    //        string[] placements = File.ReadAllLines(fileName);

        //    //        if (placements.Length > 0)
        //    //            short.TryParse(placements[0], out x);
        //    //        if (placements.Length > 1)
        //    //            short.TryParse(placements[1], out y);
        //    //    }

        //    //    _library.InsertImage(index, image, x, y);

        //    //    toolStripProgressBar.Value++;
        //    //}

        //    //ImageList.Images.Clear();
        //    //_indexList.Clear();
        //    //PreviewListView.VirtualListSize = _library.Images.Count;
        //    //toolStripProgressBar.Value = 0;
        //    //_library.Save(_library._fileName);
        //}
        ////替换图像
        //private void buttonReplace_Click(object sender, EventArgs e)
        //{
        //    //if (_library == null) return;
        //    //if (_library.FileName == null) return;
        //    //if (PreviewListView.SelectedIndices.Count == 0) return;

        //    //OpenFileDialog ofd = new OpenFileDialog();
        //    //ofd.ShowDialog();

        //    //if (ofd.FileName == "") return;

        //    //Bitmap newBmp = new Bitmap(ofd.FileName);

        //    //ImageList.Images.Clear();
        //    //_indexList.Clear();
        //    //_library.ReplaceImage(PreviewListView.SelectedIndices[0], newBmp, 0, 0);
        //    //PreviewListView.VirtualListSize = _library.Images.Count;

        //    //try
        //    //{
        //    //    PreviewListView.RedrawItems(0, PreviewListView.Items.Count - 1, true);
        //    //    ImageBox.Image = _library.Images[PreviewListView.SelectedIndices[0]].Image;
        //    //}
        //    //catch (Exception)
        //    //{
        //    //    return;
        //    //}
        //}
        //导入图像
        private void ImportReplace_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library._fileName == null) return;
            if (PreviewListView.SelectedIndices.Count == 0) return;
            //打开文件窗口
            if (ImportImageDialog.ShowDialog() != DialogResult.OK) return;
            //打开AddImageForm窗口
            if (new AddImageForm().ShowDialog() != DialogResult.OK) return;

            List<string> fileNames = new List<string>(ImportImageDialog.FileNames);

            byte imageTextureType = AddImageForm.form1.ImageTextureType;
            byte shadowTextureType = AddImageForm.form1.ShadowTextureType;
            byte overlayTextureType = AddImageForm.form1.OverlayTextureType;

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = fileNames.Count;

            for (int i = 0; i < fileNames.Count; i++)
            {
                string fileName = fileNames[i];

                Bitmap image = null;
                Bitmap shadowimage = null;
                Bitmap maskimage = null;
                short width = 0, height = 0, offSetX = 0, offSetY = 0, shadowOffSetX = 0, shadowOffSetY = 0, shadow = 0;

                #region 图像层
                if (File.Exists(fileName))
                {
                    //加载图像
                    try
                    {
                        image = new Bitmap(fileName);
                        if (image.Width == 1 && image.Height == 1) image = null;
                    }
                    catch
                    {
                        continue;
                    }
                    //转换图像坐标文件名
                    string placmentFileName = Path.Combine(Path.GetDirectoryName(fileName), "Placements", Path.GetFileNameWithoutExtension(fileName));
                    placmentFileName = Path.ChangeExtension(placmentFileName, ".txt");

                    if (File.Exists(placmentFileName))
                    {
                        string[] placements = File.ReadAllLines(placmentFileName);

                        if (placements.Length > 0)
                            short.TryParse(placements[0], out width);
                        if (placements.Length > 1)
                            short.TryParse(placements[1], out height);
                        if (placements.Length > 2)
                            short.TryParse(placements[0], out offSetX);
                        if (placements.Length > 3)
                            short.TryParse(placements[1], out offSetY);
                        if (placements.Length > 4)
                            short.TryParse(placements[2], out shadowOffSetX);
                        if (placements.Length > 5)
                            short.TryParse(placements[3], out shadowOffSetY);
                        if (placements.Length > 6)
                            short.TryParse(placements[4], out shadow);
                    }
                }
                #endregion

                #region 影子层
                string shadowFileName = Path.Combine(Path.GetDirectoryName(fileName), "Shadow", Path.GetFileName(fileName));
                if (File.Exists(shadowFileName))
                {
                    //加载影子层图像
                    try
                    {
                        shadowimage = new Bitmap(shadowFileName);
                        if (shadowimage.Width == 1 && shadowimage.Height == 1) shadowimage = null;
                    }
                    catch
                    {
                        continue;
                    }
                    //转换影子层坐标文件
                    //string placmentFileName = Path.Combine(Path.GetDirectoryName(shadowFileName), "Placements", Path.GetFileNameWithoutExtension(shadowFileName));
                    //placmentFileName = Path.ChangeExtension(placmentFileName, ".txt");

                    //if (File.Exists(placmentFileName))
                    //{
                    //    string[] placements = File.ReadAllLines(placmentFileName);

                    //    if (placements.Length > 0)
                    //        short.TryParse(placements[0], out shadowOffSetX);
                    //    if (placements.Length > 1)
                    //        short.TryParse(placements[1], out shadowOffSetY);
                    //}
                }
                #endregion

                #region 覆盖层
                string overlayFileName = Path.Combine(Path.GetDirectoryName(fileName), "Overlay", Path.GetFileName(fileName));
                if (File.Exists(overlayFileName))
                {
                    //加载覆盖层图像
                    try
                    {
                        maskimage = new Bitmap(overlayFileName);
                        if (maskimage.Width == 1 && maskimage.Height == 1) maskimage = null;
                    }
                    catch
                    {
                        continue;
                    }
                    //转换覆盖层坐标文件
                    //string placmentFileName = Path.Combine(Path.GetDirectoryName(overlayFileName), "Placements", Path.GetFileNameWithoutExtension(overlayFileName));
                    //placmentFileName = Path.ChangeExtension(placmentFileName, ".txt");

                    //if (File.Exists(placmentFileName))
                    //{
                    //    string[] placements = File.ReadAllLines(placmentFileName);

                    //    if (placements.Length > 0)
                    //        short.TryParse(placements[0], out overlaywidth);
                    //    if (placements.Length > 1)
                    //        short.TryParse(placements[1], out overlayheight);
                    //    if (placements.Length > 2)
                    //        short.TryParse(placements[2], out overlayoffSetX);
                    //    if (placements.Length > 3)
                    //        short.TryParse(placements[3], out overlayoffSetY);
                    //    if (placements.Length > 4)
                    //        short.TryParse(placements[4], out overlayshadowOffSetX);
                    //    if (placements.Length > 5)
                    //        short.TryParse(placements[5], out overlayshadowOffSetY);
                    //    if (placements.Length > 6)
                    //        short.TryParse(placements[6], out overlayshadowtype);
                    //}
                }
                #endregion

                //尾部追加图片
                if (AddImageForm.form1.radioButtonAdd.Checked)
                {
                    if (image == null)
                        _library.Images.Add(null);
                    else
                        _library.AddImage(image, shadowimage, maskimage, imageTextureType, shadowTextureType, overlayTextureType,
                                      (short)(width == 0? image.Width : width), (short)(height == 0? image.Height : height), offSetX, offSetY, shadowOffSetX, shadowOffSetY, shadow,
                                      (short)(shadowimage?.Width ?? 0), (short)(shadowimage?.Height ?? 0), (short)(maskimage?.Width ?? 0), (short)(maskimage?.Height ?? 0));

                    if (i == fileNames.Count - 1)
                        PreviewListView.VirtualListSize = _library.Images.Count;
                }
                //插入图片
                else if (AddImageForm.form1.radioButtonInsert.Checked)
                {
                    int index = PreviewListView.SelectedIndices[0] + i;

                    if (image == null)
                        _library.Images.Insert(index, null);
                    else
                        _library.InsertImage(index, image, shadowimage, maskimage, imageTextureType, shadowTextureType, overlayTextureType,
                                      (short)(width == 0 ? image.Width : width), (short)(height == 0 ? image.Height : height), offSetX, offSetY, shadowOffSetX, shadowOffSetY, shadow,
                                      (short)(shadowimage?.Width ?? 0), (short)(shadowimage?.Height ?? 0), (short)(maskimage?.Width ?? 0), (short)(maskimage?.Height ?? 0));

                    if (i == fileNames.Count - 1)
                    {
                        ImageList.Images.Clear();
                        _indexList.Clear();
                        PreviewListView.VirtualListSize = _library.Images.Count;
                    }
                }
                //替换图片
                else if (AddImageForm.form1.radioButtonReplace.Checked)
                {
                    int index = PreviewListView.SelectedIndices[0] + i;

                    if (index < _library.Images.Count)
                    {
                        if (image == null)
                            _library.Images[index] = null;
                        else
                            _library.ReplaceImage(index, image, shadowimage, maskimage, imageTextureType, shadowTextureType, overlayTextureType,
                                          (short)(width == 0 ? image.Width : width), (short)(height == 0 ? image.Height : height), offSetX, offSetY, shadowOffSetX, shadowOffSetY, shadow,
                                          (short)(shadowimage?.Width ?? 0), (short)(shadowimage?.Height ?? 0), (short)(maskimage?.Width ?? 0), (short)(maskimage?.Height ?? 0));
                    }

                    if (i == fileNames.Count - 1)
                    {
                        ImageList.Images.Clear();
                        _indexList.Clear();
                        PreviewListView.VirtualListSize = _library.Images.Count;
                        try
                        {
                            int count = index >= _library.Images.Count ? _library.Images.Count - 1 : index;
                            PreviewListView.RedrawItems(PreviewListView.SelectedIndices[0], count, true);
                            ImageBox.Image = _library.Images[PreviewListView.SelectedIndices[0]].Image;
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    }
                }
                toolStripProgressBar.Value++;
                if (image != null)
                    image.Dispose();
                if (shadowimage != null)
                    shadowimage.Dispose();
                if (maskimage != null)
                    maskimage.Dispose();
            }
            toolStripProgressBar.Value = 0;
        }
        //删除图像
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library.FileName == null) return;
            if (PreviewListView.SelectedIndices.Count == 0) return;

            if (MessageBox.Show("是否确实要删除所选图片?",
                "删除选定.",
                MessageBoxButtons.YesNoCancel) != DialogResult.Yes) return;

            List<int> removeList = new List<int>();

            for (int i = 0; i < PreviewListView.SelectedIndices.Count; i++)
                removeList.Add(PreviewListView.SelectedIndices[i]);

            removeList.Sort();

            for (int i = removeList.Count - 1; i >= 0; i--)
                _library.RemoveImage(removeList[i]);

            ImageList.Images.Clear();
            _indexList.Clear();
            PreviewListView.VirtualListSize -= removeList.Count;
        }
        //导出PNG图像
        private void ExportPNGButton_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library.FileName == null) return;
            if (PreviewListView.SelectedIndices.Count == 0) return;

            string _fileName = Path.GetFileName(OpenLibraryDialog.FileName);
            string _newName = _fileName.Remove(_fileName.IndexOf('.'));
            string _folder = Application.StartupPath + "\\Exported\\" + _newName + "\\";

            Bitmap blank = new Bitmap(1, 1);

            // Create the folder if it doesn't exist.
            if (!Directory.Exists(_folder + "/Placements/"))
                Directory.CreateDirectory(_folder + "/Placements/");

            ListView.SelectedIndexCollection _col = PreviewListView.SelectedIndices;

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = _col.Count;

            for (int i = _col[0]; i < (_col[0] + _col.Count); i++)
            {
                _exportImage = _library.GetImage(i);
                if (_exportImage?.Image == null)         //输出PNG保存格式
                {
                    blank.Save(_folder + i.ToString() + ".png", ImageFormat.Png);
                }
                else
                {
                    _exportImage.Image.Save(_folder + i.ToString() + ".png", ImageFormat.Png);

                    if (_exportImage?.ShadowImage != null)
                    {
                        if (!Directory.Exists(_folder + "/Shadow/Placements/"))
                            Directory.CreateDirectory(_folder + "/Shadow/Placements/");
                        _exportImage.ShadowImage.Save(_folder + "/Shadow/" + i.ToString() + ".png", ImageFormat.Png);
                    }

                    if (_exportImage?.OverlayImage != null)
                    {
                        if (!Directory.Exists(_folder + "/Overlay/"))
                            Directory.CreateDirectory(_folder + "/Overlay/");
                        _exportImage.OverlayImage.Save(_folder + "/Overlay/" + i.ToString() + ".png", ImageFormat.Png);
                    }
                }

                toolStripProgressBar.Value++;

                int width = _exportImage?.Width ?? 0;
                int height = _exportImage?.Height ?? 0;
                int offSetX = _exportImage?.OffSetX ?? 0;
                int offSetY = _exportImage?.OffSetY ?? 0;
                int shadowOffSetX = _exportImage?.ShadowOffSetX ?? 0;
                int shadowOffSetY = _exportImage?.ShadowOffSetY ?? 0;
                int shadow = _exportImage?.Shadow ?? 0;

                File.WriteAllLines(_folder + "/Placements/" + i.ToString() + ".txt", new string[]
                {
                    width.ToString(),
                    height.ToString(),
                    offSetX.ToString(),
                    offSetY.ToString(),
                    shadowOffSetX.ToString(),
                    shadowOffSetY.ToString(),
                    shadow.ToString(),
                });

                //if (_exportImage.ShadowImage != null)
                //{
                //    int shadowOffSetX = _exportImage?.ShadowOffSetX ?? 0;
                //    int shadowOffSetY = _exportImage?.ShadowOffSetY ?? 0;

                //    File.WriteAllLines(_folder + "/Shadow/Placements/" + i.ToString() + ".txt", new string[]
                //    {
                //        shadowOffSetX.ToString(),
                //        shadowOffSetY.ToString(),
                //    });
                //}

                //覆盖层不需要坐标，用图像层坐标就可以
                //if (_exportImage.OverlayImage != null)
                //{
                //    int overlaywidth = _exportImage?.OverlayWidth ?? 0;
                //    int overlayheight = _exportImage?.OverlayHeight ?? 0;
                //    int overlayoffSetX = _exportImage?.OverlayOffSetX ?? 0;
                //    int overlayoffSetY = _exportImage?.OverlayOffSetY ?? 0;
                //    int overlayshadowOffSetX = _exportImage?.OverlayShadowOffSetX ?? 0;
                //    int overlayshadowOffSetY = _exportImage?.OverlayShadowOffSetY ?? 0;
                //    int overlayshadowType = _exportImage?.OverlayShadowType ?? 0;

                //    File.WriteAllLines(_folder + "/Overlay/Placements/" + i.ToString() + ".txt", new string[]
                //    {
                //        overlaywidth.ToString(),
                //        overlayheight.ToString(),
                //        overlayoffSetX.ToString(),
                //        overlayoffSetY.ToString(),
                //        overlayshadowOffSetX.ToString(),
                //        overlayshadowOffSetY.ToString(),
                //        overlayshadowType.ToString(),
                //    });
                //}
            }

            toolStripProgressBar.Value = 0;
            MessageBox.Show("保存到 " + _folder + "...", "图片保存", MessageBoxButtons.OK);
        }
        //导出BMP图像
        private void ExportBMPButton_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library.FileName == null) return;
            if (PreviewListView.SelectedIndices.Count == 0) return;

            string _fileName = Path.GetFileName(OpenLibraryDialog.FileName);
            string _newName = _fileName.Remove(_fileName.IndexOf('.'));
            string _folder = Application.StartupPath + "\\Exported\\" + _newName + "\\";

            Bitmap blank = new Bitmap(1, 1);

            // Create the folder if it doesn't exist.
            if (!Directory.Exists(_folder + "/Placements/"))
                Directory.CreateDirectory(_folder + "/Placements/");

            ListView.SelectedIndexCollection _col = PreviewListView.SelectedIndices;

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = _col.Count;

            for (int i = _col[0]; i < (_col[0] + _col.Count); i++)
            {
                _exportImage = _library.GetImage(i);
                if (_exportImage?.Image == null)         //输出PNG保存格式
                {
                    blank.Save(_folder + i.ToString() + ".bmp", ImageFormat.Bmp);
                }
                else
                {
                    _exportImage.Image.Save(_folder + i.ToString() + ".bmp", ImageFormat.Bmp);

                    if (_exportImage?.ShadowImage != null)
                    {
                        if (!Directory.Exists(_folder + "/Shadow/Placements/"))
                            Directory.CreateDirectory(_folder + "/Shadow/Placements/");
                        _exportImage.ShadowImage.Save(_folder + "/Shadow/" + i.ToString() + ".bmp", ImageFormat.Bmp);
                    }

                    if (_exportImage?.OverlayImage != null)
                    {
                        if (!Directory.Exists(_folder + "/Overlay/"))
                            Directory.CreateDirectory(_folder + "/Overlay/");
                        _exportImage.OverlayImage.Save(_folder + "/Overlay/" + i.ToString() + ".bmp", ImageFormat.Bmp);
                    }
                }

                toolStripProgressBar.Value++;

                int width = _exportImage?.Width ?? 0;
                int height = _exportImage?.Height ?? 0;
                int offSetX = _exportImage?.OffSetX ?? 0;
                int offSetY = _exportImage?.OffSetY ?? 0;
                int shadowOffSetX = _exportImage?.ShadowOffSetX ?? 0;
                int shadowOffSetY = _exportImage?.ShadowOffSetY ?? 0;
                int shadow = _exportImage?.Shadow ?? 0;

                File.WriteAllLines(_folder + "/Placements/" + i.ToString() + ".txt", new string[]
                {
                    width.ToString(),
                    height.ToString(),
                    offSetX.ToString(),
                    offSetY.ToString(),
                    shadowOffSetX.ToString(),
                    shadowOffSetY.ToString(),
                    shadow.ToString(),
                });

                //if (_exportImage.ShadowImage != null)
                //{
                //    int shadowOffSetX = _exportImage?.ShadowOffSetX ?? 0;
                //    int shadowOffSetY = _exportImage?.ShadowOffSetY ?? 0;

                //    File.WriteAllLines(_folder + "/Shadow/Placements/" + i.ToString() + ".txt", new string[]
                //    {
                //        shadowOffSetX.ToString(),
                //        shadowOffSetY.ToString(),
                //    });
                //}

                //覆盖层不需要坐标，用图像层坐标就可以
                //if (_exportImage.OverlayImage != null)
                //{
                //    int overlaywidth = _exportImage?.OverlayWidth ?? 0;
                //    int overlayheight = _exportImage?.OverlayHeight ?? 0;
                //    int overlayoffSetX = _exportImage?.OverlayOffSetX ?? 0;
                //    int overlayoffSetY = _exportImage?.OverlayOffSetY ?? 0;
                //    int overlayshadowOffSetX = _exportImage?.OverlayShadowOffSetX ?? 0;
                //    int overlayshadowOffSetY = _exportImage?.OverlayShadowOffSetY ?? 0;
                //    int overlayshadowType = _exportImage?.OverlayShadowType ?? 0;

                //    File.WriteAllLines(_folder + "/Overlay/Placements/" + i.ToString() + ".txt", new string[]
                //    {
                //        overlaywidth.ToString(),
                //        overlayheight.ToString(),
                //        overlayoffSetX.ToString(),
                //        overlayoffSetY.ToString(),
                //        overlayshadowOffSetX.ToString(),
                //        overlayshadowOffSetY.ToString(),
                //        overlayshadowType.ToString(),
                //    });
                //}
            }

            toolStripProgressBar.Value = 0;
            MessageBox.Show("保存到 " + _folder + "...", "图片保存", MessageBoxButtons.OK);
        }
        //OffSetX输入框修改事件
        private void OffSetXTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox control = sender as TextBox;

            if (control == null || !control.Focused) return;

            short temp;

            if (!short.TryParse(control.Text, out temp) || _library == null)
            {
                control.BackColor = Color.Red;
                return;
            }

            control.BackColor = SystemColors.Window;

            for (int i = 0; i < PreviewListView.SelectedIndices.Count; i++)
            {
                BlackDragonLibrary.MImage image = _library.GetImage(PreviewListView.SelectedIndices[i]);
                if (radioButtonImage.Checked)
                {
                    if (image?.Image == null) continue;
                    image.OffSetX = temp;
                }
            }
        }
        //OffSetY输入框修改事件
        private void OffSetYTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox control = sender as TextBox;

            if (control == null || !control.Focused) return;

            short temp;

            if (!short.TryParse(control.Text, out temp) || _library == null)
            {
                control.BackColor = Color.Red;
                return;
            }

            control.BackColor = SystemColors.Window;

            for (int i = 0; i < PreviewListView.SelectedIndices.Count; i++)
            {
                BlackDragonLibrary.MImage image = _library.GetImage(PreviewListView.SelectedIndices[i]);
                if (radioButtonImage.Checked)
                {
                    if (image?.Image == null) continue;
                    image.OffSetY = temp;
                }
            }
        }
        private void ShadowOffSetXTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox control = sender as TextBox;

            if (control == null || !control.Focused) return;

            short temp;

            if (!short.TryParse(control.Text, out temp) || _library == null)
            {
                control.BackColor = Color.Red;
                return;
            }

            control.BackColor = SystemColors.Window;

            for (int i = 0; i < PreviewListView.SelectedIndices.Count; i++)
            {
                BlackDragonLibrary.MImage image = _library.GetImage(PreviewListView.SelectedIndices[i]);
                if (image?.Image == null) continue;
                image.ShadowOffSetX = temp;
            }
        }

        private void ShadowOffSetYTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox control = sender as TextBox;

            if (control == null || !control.Focused) return;

            short temp;

            if (!short.TryParse(control.Text, out temp) || _library == null)
            {
                control.BackColor = Color.Red;
                return;
            }

            control.BackColor = SystemColors.Window;

            for (int i = 0; i < PreviewListView.SelectedIndices.Count; i++)
            {
                BlackDragonLibrary.MImage image = _library.GetImage(PreviewListView.SelectedIndices[i]);
                if (image?.Image == null) continue;
                image.ShadowOffSetY = temp;
            }
        }

        private void ShadowTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox control = sender as TextBox;

            if (control == null || !control.Focused) return;

            byte temp;

            if (!byte.TryParse(control.Text, out temp) || _library == null)
            {
                control.BackColor = Color.Red;
                return;
            }

            control.BackColor = SystemColors.Window;

            for (int i = 0; i < PreviewListView.SelectedIndices.Count; i++)
            {
                BlackDragonLibrary.MImage image = _library.GetImage(PreviewListView.SelectedIndices[i]);
                if (radioButtonImage.Checked)
                {
                    if (image?.Image == null) continue;
                    image.Shadow = temp;
                }
            }
        }

        // Resize the image(Zoom).
        private Image ImageBoxZoom(Image image, Size size)
        {
            _originalImage = _selectedImage.Image;
            Bitmap _bmp = new Bitmap(_originalImage, Convert.ToInt32(_originalImage.Width * size.Width), Convert.ToInt32(_originalImage.Height * size.Height));
            Graphics _gfx = Graphics.FromImage(_bmp);
            return _bmp;
        }

        //图像缩放放大
        private void ZoomTrackBar_Scroll(object sender, EventArgs e)
        {
            if (ImageBox.Image == null)
            {
                ZoomTrackBar.Value = 1;
            }
            if (ZoomTrackBar.Value > 0)
            {
                try
                {
                    PreviewListView.Items[(int)nudJump.Value].EnsureVisible();

                    Bitmap _newBMP = new Bitmap(_selectedImage.Width * ZoomTrackBar.Value, _selectedImage.Height * ZoomTrackBar.Value);
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(_newBMP))
                    {
                        if (checkBoxPreventAntiAliasing.Checked == true)
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.CompositingMode = CompositingMode.SourceCopy;
                        }

                        if (checkBoxQuality.Checked == true)
                        {
                            g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        }

                        g.DrawImage(_selectedImage.Image, new Rectangle(0, 0, _newBMP.Width, _newBMP.Height));
                    }
                    ImageBox.Image = _newBMP;

                    toolStripStatusLabel.ForeColor = SystemColors.ControlText;
                    toolStripStatusLabel.Text = "选择图像: " + string.Format("{0} / {1}",
                        PreviewListView.SelectedIndices[0].ToString(),
                        (PreviewListView.Items.Count - 1).ToString());
                }
                catch
                {
                    return;
                }
            }
        }

        //pictureBox背景颜色切换
        private void pictureBox_Click(object sender, EventArgs e)
        {
            if (panel.BackColor == Color.Black)
            {
                panel.BackColor = Color.GhostWhite;
            }
            else
            {
                panel.BackColor = Color.Black;
            }
        }
        //有按钮 下一个图像
        private void buttonSkipNext_Click(object sender, EventArgs e)
        {
            nextImageToolStripMenuItem_Click(null, null);
        }
        //左按钮 上一个图像
        private void buttonSkipPrevious_Click(object sender, EventArgs e)
        {
            previousImageToolStripMenuItem_Click(null, null);
        }
        //左按钮调用的函数
        private void previousImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (PreviewListView.Visible && PreviewListView.Items.Count > 0)
                {
                    int index = PreviewListView.SelectedIndices[0];
                    index = index - 1;
                    PreviewListView.SelectedIndices.Clear();
                    this.PreviewListView.Items[index].Selected = true;
                    PreviewListView.Items[index].EnsureVisible();

                    if (_selectedImage == null || _selectedImage.Height == 1 && _selectedImage.Width == 1 && PreviewListView.SelectedIndices[0] != 0)
                    {
                        previousImageToolStripMenuItem_Click(null, null);
                    }
                }
            }
            catch (Exception)
            {
                PreviewListView.SelectedIndices.Clear();
                this.PreviewListView.Items[PreviewListView.Items.Count - 1].Selected = true;
            }
        }
        //右按钮调用的函数
        private void nextImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (PreviewListView.Visible && PreviewListView.Items.Count > 0)
                {
                    int index = PreviewListView.SelectedIndices[0];
                    index = index + 1;
                    PreviewListView.SelectedIndices.Clear();
                    this.PreviewListView.Items[index].Selected = true;
                    PreviewListView.Items[index].EnsureVisible();

                    if (_selectedImage == null || _selectedImage.Height == 1 && _selectedImage.Width == 1 && PreviewListView.SelectedIndices[0] != 0)
                    {
                        nextImageToolStripMenuItem_Click(null, null);
                    }
                }
            }
            catch (Exception)
            {
                PreviewListView.SelectedIndices.Clear();
                this.PreviewListView.Items[0].Selected = true;
            }
        }
        // Move Left and Right through images.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left)
            {
                previousImageToolStripMenuItem_Click(null, null);
                return true;
            }

            if (keyData == Keys.Right)
            {
                nextImageToolStripMenuItem_Click(null, null);
                return true;
            }

            if (keyData == Keys.Up) //Not 100% accurate but works for now.
            {
                double d = Math.Floor((double)(PreviewListView.Width / 67));
                int index = PreviewListView.SelectedIndices[0] - (int)d;

                PreviewListView.SelectedIndices.Clear();
                if (index < 0)
                    index = 0;

                this.PreviewListView.Items[index].Selected = true;

                return true;
            }

            if (keyData == Keys.Down) //Not 100% accurate but works for now.
            {
                double d = Math.Floor((double)(PreviewListView.Width / 67));
                int index = PreviewListView.SelectedIndices[0] + (int)d;

                PreviewListView.SelectedIndices.Clear();
                if (index > PreviewListView.Items.Count - 1)
                    index = PreviewListView.Items.Count - 1;

                this.PreviewListView.Items[index].Selected = true;

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        //不模糊
        private void checkBoxQuality_CheckedChanged(object sender, EventArgs e)
        {
            ZoomTrackBar_Scroll(null, null);
        }
        //不抗锯齿
        private void checkBoxPreventAntiAliasing_CheckedChanged(object sender, EventArgs e)
        {
            ZoomTrackBar_Scroll(null, null);
        }
        //图片编号输入框上下翻按钮触发事件
        private void nudJump_ValueChanged(object sender, EventArgs e)
        {
            if (PreviewListView.Items.Count - 1 >= nudJump.Value)
            {
                PreviewListView.SelectedIndices.Clear();
                PreviewListView.Items[(int)nudJump.Value].Selected = true;
                PreviewListView.Items[(int)nudJump.Value].EnsureVisible();
            }
        }
        //图片编号输入框输入事件
        private void nudJump_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //Enter key is down.
                if (PreviewListView.Items.Count - 1 >= nudJump.Value)
                {
                    PreviewListView.SelectedIndices.Clear();
                    PreviewListView.Items[(int)nudJump.Value].Selected = true;
                    PreviewListView.Items[(int)nudJump.Value].EnsureVisible();
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        #endregion

        #region 缩略图

        private void PreviewListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PreviewListView.SelectedIndices.Count == 0)
            {
                ClearInterface();
                return;
            }

            _selectedImage = _library.GetImage(PreviewListView.SelectedIndices[0]);

            if (_selectedImage == null)
            {
                toolStripStatusLabel.ForeColor = SystemColors.ControlText;
                toolStripStatusLabel.Text = "选定图片: " + string.Format("{0} / {1}",
                PreviewListView.SelectedIndices[0].ToString(),
                (PreviewListView.Items.Count - 1).ToString());
                toolStripStatusLabel2.Text = "格式:";
                ClearInterface();
                return;
            }

            if (radioButtonImage.Checked)
            {
                WidthLabel.Text = _selectedImage.Width.ToString();
                HeightLabel.Text = _selectedImage.Height.ToString();
                OffSetXTextBox.Text = _selectedImage.OffSetX.ToString();
                OffSetYTextBox.Text = _selectedImage.OffSetY.ToString();
                ShadowOffSetXTextBox.Text = _selectedImage.ShadowOffSetX.ToString();
                ShadowOffSetYTextBox.Text = _selectedImage.ShadowOffSetY.ToString();
                ShadowTextBox.Text = _selectedImage.Shadow.ToString();

                if (contrayCheckBox.Checked)
                    ImageBox.Image = PContray(_selectedImage.Image);
                else
                    ImageBox.Image = _selectedImage.Image;
            }
            else if (radioButtonShadow.Checked)
            {
                WidthLabel.Text = _selectedImage.ShadowWidth.ToString();
                HeightLabel.Text = _selectedImage.ShadowHeight.ToString();
                OffSetXTextBox.Text = _selectedImage.OffSetX.ToString();
                OffSetYTextBox.Text = _selectedImage.OffSetY.ToString();
                ShadowOffSetXTextBox.Text = _selectedImage.ShadowOffSetX.ToString();
                ShadowOffSetYTextBox.Text = _selectedImage.ShadowOffSetY.ToString();
                ShadowTextBox.Text = string.Empty;

                ShadowOffSetXTextBox.Enabled = _selectedImage.ShadowValid;
                ShadowOffSetYTextBox.Enabled = _selectedImage.ShadowValid;

                ImageBox.Image = _selectedImage.ShadowImage;
            }
            if (radioButtonOverlay.Checked)
            {
                WidthLabel.Text = _selectedImage.ShadowWidth.ToString();
                HeightLabel.Text = _selectedImage.ShadowHeight.ToString();
                OffSetXTextBox.Text = _selectedImage.OffSetX.ToString();
                OffSetYTextBox.Text = _selectedImage.OffSetY.ToString();
                ShadowOffSetXTextBox.Text = _selectedImage.ShadowOffSetX.ToString();
                ShadowOffSetYTextBox.Text = _selectedImage.ShadowOffSetY.ToString();
                ShadowTextBox.Text = string.Empty;

                ImageBox.Image = _selectedImage.OverlayImage;
            }

            // Keep track of what image/s are selected.
            if (PreviewListView.SelectedIndices.Count > 1)
            {
                toolStripStatusLabel.ForeColor = Color.Red;
                toolStripStatusLabel.Text = "选择多个图片.";
                toolStripStatusLabel2.ForeColor = Color.Red;
                toolStripStatusLabel2.Text = "格式:";
            }
            else
            {
                toolStripStatusLabel.ForeColor = SystemColors.ControlText;
                toolStripStatusLabel.Text = "选定图片: " + string.Format("{0} / {1}",
                PreviewListView.SelectedIndices[0].ToString(),
                (PreviewListView.Items.Count - 1).ToString());
            }

            toolStripStatusLabel2.Text = _selectedImage.ImageTextureType == 1 ? "格式: DXT1" : _selectedImage.ImageTextureType == 3 ? "格式: DXT3" : _selectedImage.ImageTextureType == 5 ? "格式: DXT5" : _selectedImage.ImageTextureType == 32 ? "格式: ARGB32" : "格式: ABGR32";

            nudJump.Value = PreviewListView.SelectedIndices[0];
        }

        private void PreviewListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            int index;

            if (_indexList.TryGetValue(e.ItemIndex, out index))
            {
                e.Item = new ListViewItem { ImageIndex = index, Text = e.ItemIndex.ToString() };
                return;
            }

            _indexList.Add(e.ItemIndex, ImageList.Images.Count);
            if (radioButtonImage.Checked)
                ImageList.Images.Add(_library.GetPreview(e.ItemIndex, ImageType.Image));
            else if (radioButtonShadow.Checked)
                ImageList.Images.Add(_library.GetPreview(e.ItemIndex, ImageType.Shadow));
            else if (radioButtonOverlay.Checked)
                ImageList.Images.Add(_library.GetPreview(e.ItemIndex, ImageType.Overlay));
            e.Item = new ListViewItem { ImageIndex = index, Text = e.ItemIndex.ToString() };
        }

        private void PreviewListView_VirtualItemsSelectionRangeChanged(object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e)
        {
            // Keep track of what image/s are selected.
            ListView.SelectedIndexCollection _col = PreviewListView.SelectedIndices;

            if (_col.Count > 1)
            {
                toolStripStatusLabel.ForeColor = Color.Red;
                toolStripStatusLabel.Text = "选择多个图片.";
            }
        }

        //定义图像反色函数
        private Bitmap PContray(Bitmap a)
        {
            int w = a.Width;
            int h = a.Height;
            Bitmap dstBitmap = new Bitmap(a.Width, a.Height, PixelFormat.Format32bppArgb);
            BitmapData srcData = a.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData dstData = dstBitmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* pIn = (byte*)srcData.Scan0.ToPointer();
                byte* pOut = (byte*)dstData.Scan0.ToPointer();
                byte* p;
                int stride = srcData.Stride;
                int r, g, b;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        p = pIn;
                        r = p[2];
                        g = p[1];
                        b = p[0];
                        pOut[3] = p[3];
                        pOut[2] = (byte)(255 - r);
                        pOut[1] = (byte)(255 - g);
                        pOut[0] = (byte)(255 - b);
                        pIn += 4;
                        pOut += 4;
                    }
                    pIn += srcData.Stride - w * 4;
                    pOut += srcData.Stride - w * 4;
                }
                a.UnlockBits(srcData);
                dstBitmap.UnlockBits(dstData);
                return dstBitmap;
            }
        }

        #endregion

        private const int HowDeepToScan = 6;

        /*public static void ProcessDir(string sourceDir, int recursionLvl, string outputDir)
        {
            if (recursionLvl <= HowDeepToScan)
            {
                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles(sourceDir);
                foreach (string fileName in fileEntries)
                {
                    if (Directory.Exists(outputDir) != true) Directory.CreateDirectory(outputDir);
                    MLibraryv0 OldLibrary = new MLibraryv0(fileName);
                    MLibraryV2 NewLibrary = new MLibraryV2(outputDir + Path.GetFileName(fileName)) { Images = new List<MLibraryV2.MImage>(), IndexList = new List<int>(), Count = OldLibrary.Images.Count }; ;
                    for (int i = 0; i < OldLibrary.Images.Count; i++)
                        NewLibrary.Images.Add(null);
                    for (int j = 0; j < OldLibrary.Images.Count; j++)
                    {
                        MLibraryv0.MImage oldimage = OldLibrary.GetMImage(j);
                        NewLibrary.Images[j] = new MLibraryV2.MImage(oldimage.FBytes, oldimage.Width, oldimage.Height) { X = oldimage.X, Y = oldimage.Y };
                    }
                    NewLibrary.Save();
                    for (int i = 0; i < NewLibrary.Images.Count; i++)
                    {
                        if (NewLibrary.Images[i].Preview != null)
                            NewLibrary.Images[i].Preview.Dispose();
                        if (NewLibrary.Images[i].Image != null)
                            NewLibrary.Images[i].Image.Dispose();
                        if (NewLibrary.Images[i].MaskImage != null)
                            NewLibrary.Images[i].MaskImage.Dispose();
                    }
                    for (int i = 0; i < OldLibrary.Images.Count; i++)
                    {
                        if (OldLibrary.Images[i].Preview != null)
                            OldLibrary.Images[i].Preview.Dispose();
                        if (OldLibrary.Images[i].Image != null)
                            OldLibrary.Images[i].Image.Dispose();
                    }
                    NewLibrary.Images.Clear();
                    NewLibrary.IndexList.Clear();
                    OldLibrary.Images.Clear();
                    OldLibrary.IndexList.Clear();
                    NewLibrary.Close();
                    OldLibrary.Close();
                    NewLibrary = null;
                    OldLibrary = null;
                }

                // Recurse into subdirectories of this directory.
                string[] subdirEntries = Directory.GetDirectories(sourceDir);
                foreach (string subdir in subdirEntries)
                {
                    // Do not iterate through re-parse points.
                    if (Path.GetFileName(Path.GetFullPath(subdir).TrimEnd(Path.DirectorySeparatorChar)) == Path.GetFileName(Path.GetFullPath(outputDir).TrimEnd(Path.DirectorySeparatorChar))) continue;
                    if ((File.GetAttributes(subdir) &
                         FileAttributes.ReparsePoint) !=
                             FileAttributes.ReparsePoint)
                        ProcessDir(subdir, recursionLvl + 1, outputDir + " \\" + Path.GetFileName(Path.GetFullPath(subdir).TrimEnd(Path.DirectorySeparatorChar)) + "\\");
                }
            }
        }*/

    }
}