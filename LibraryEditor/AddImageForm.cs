using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace LibraryEditor
{
    public partial class AddImageForm : Form
    {
        public static AddImageForm form1; //其他方法可以调用窗体控件
        public Bitmap newImage;
        public Bitmap newShadowImage;
        public Bitmap newMaskImage;
        private List<string> FileNames = new List<string>(LMain.form1.ImportImageDialog.FileNames);
        public byte ImageTextureType;
        public byte ShadowTextureType;
        public byte OverlayTextureType;
        private int CurrentNum
        {
            get { return _CurrentNum; }
            set
            {
                if (value >= FileNames.Count || value < 0) return;
                _CurrentNum = value;
            }
        }
        private int _CurrentNum;
        public AddImageForm()
        {
            InitializeComponent();
            form1 = this; //其他方法可以调用窗体控件

            radioButtonReplace.Checked = true;
            imageRadioButtonARGB32.Checked = true;
            shadowRadioButtonDXT1.Checked = true;
            overlayRadioButtonDXT1.Checked = true;
            textBox1.Text = Path.GetFileName(FileNames[CurrentNum]);
            this.Text = FileNames[CurrentNum];

            //form标题

            //加载预览图
            CreatePreview(FileNames[CurrentNum]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CurrentNum--;
            CreatePreview(FileNames[CurrentNum]);
            textBox1.Text = Path.GetFileName(FileNames[CurrentNum]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CurrentNum++;
            CreatePreview(FileNames[CurrentNum]);
            textBox1.Text = Path.GetFileName(FileNames[CurrentNum]);
        }

        private void CreatePreview(string filename)
        {
            #region 图像层
            if (File.Exists(filename))
            {
                //加载图像
                newImage = new Bitmap(filename);

                //转换图像坐标文件名
                string placmentFileName = Path.Combine(Path.GetDirectoryName(filename), "Placements", Path.GetFileNameWithoutExtension(filename));
                placmentFileName = Path.ChangeExtension(placmentFileName, ".txt");

                short offSetX = 0, offSetY = 0, shadowOffSetX = 0, shadowOffSetY = 0, shadow = 0;
                //如果存在坐标文件
                if (File.Exists(placmentFileName))
                {
                    string[] placements = File.ReadAllLines(placmentFileName);

                    if (placements.Length > 0)
                        short.TryParse(placements[0], out offSetX);
                    if (placements.Length > 1)
                        short.TryParse(placements[1], out offSetY);
                    if (placements.Length > 2)
                        short.TryParse(placements[2], out shadowOffSetX);
                    if (placements.Length > 3)
                        short.TryParse(placements[3], out shadowOffSetY);
                    if (placements.Length > 4)
                        short.TryParse(placements[4], out shadow);
                }

                WidthLabel.Text = "宽: " + newImage.Width.ToString();
                HeightLabel.Text = "高: " + newImage.Height.ToString();
                OffSetXTextBox.Text = offSetX.ToString();
                OffSetYTextBox.Text = offSetY.ToString();
                ShadowXTextBox.Text = shadowOffSetX.ToString();
                ShadowYTextBox.Text = shadowOffSetY.ToString();
                ShadowTextBox.Text = shadow.ToString();

                imagePictureBox.Image = CreateImage(newImage, imagePictureBox.Width, imagePictureBox.Height);
            }
            #endregion

            #region 影子层
            //转换影子层文件名
            string shadowFileName = Path.Combine(Path.GetDirectoryName(filename), "Shadow", Path.GetFileName(filename));
            if (File.Exists(shadowFileName))
            {
                //加载图像
                newShadowImage = new Bitmap(shadowFileName);

                //转换影子层坐标文件
                //string placmentFileName = Path.Combine(Path.GetDirectoryName(shadowFileName), "Placements", Path.GetFileNameWithoutExtension(shadowFileName));
                //placmentFileName = Path.ChangeExtension(placmentFileName, ".txt");

                //short shadowOffSetX = 0;
                //short shadowOffSetY = 0;

                //if (File.Exists(placmentFileName))
                //{
                //    string[] placements = File.ReadAllLines(placmentFileName);

                //    if (placements.Length > 0)
                //        short.TryParse(placements[0], out shadowOffSetX);
                //    if (placements.Length > 1)
                //        short.TryParse(placements[1], out shadowOffSetY);
                //}

                //ShadowXTextBox.Text = shadowOffSetX.ToString();
                //ShadowYTextBox.Text = shadowOffSetY.ToString();

                overlayPictureBox.Image = CreateImage(newShadowImage, overlayPictureBox.Width, overlayPictureBox.Height);

                shadowGroupBox.Visible = true;
            }
            else
            {
                shadowGroupBox.Visible = false;
            }
            #endregion

            #region 覆盖层
            //转换覆盖层文件名
            string overlayFileName = Path.Combine(Path.GetDirectoryName(filename), "Overlay", Path.GetFileName(filename));
            if (File.Exists(overlayFileName))
            {
                //加载图像
                newMaskImage = new Bitmap(overlayFileName);

                //转换覆盖层坐标文件
                //string placmentFileName = Path.Combine(Path.GetDirectoryName(overlayFileName), "Placements", Path.GetFileNameWithoutExtension(overlayFileName));
                //placmentFileName = Path.ChangeExtension(placmentFileName, ".txt");

                //short maskx = 0;
                //short masky = 0;
                //short maskshadowx = 0;
                //short maskshadowy = 0;
                //short maskshadow = 0;

                //if (File.Exists(placmentFileName))
                //{
                //    string[] placements = File.ReadAllLines(placmentFileName);

                //    if (placements.Length > 0)
                //        short.TryParse(placements[0], out maskx);
                //    if (placements.Length > 1)
                //        short.TryParse(placements[1], out masky);
                //    if (placements.Length > 2)
                //        short.TryParse(placements[2], out maskshadowx);
                //    if (placements.Length > 3)
                //        short.TryParse(placements[3], out maskshadowy);
                //    if (placements.Length > 4)
                //        short.TryParse(placements[4], out maskshadow);
                //}

                overlayPictureBox.Image = CreateImage(newMaskImage, overlayPictureBox.Width, overlayPictureBox.Height);

                overlayGroupBox.Visible = true;
            }
            else
            {
                overlayGroupBox.Visible = false;
            }
            #endregion

            if (newImage != null)
                newImage.Dispose();
            if (newShadowImage != null)
                newShadowImage.Dispose();
            if (newMaskImage != null)
                newMaskImage.Dispose();
        }

        private Bitmap CreateImage(Bitmap image, int destWidth, int destHeight)
        {
            //按比例缩放 
            int width = 0, height = 0;
            int sourWidth = image.Width;
            int sourHeight = image.Height;
            if (sourHeight > destHeight || sourWidth > destWidth)
            {
                if ((sourWidth * destHeight) > (sourHeight * destWidth))
                {
                    width = destWidth;
                    height = (destWidth * sourHeight) / sourWidth;
                }
                else
                {
                    height = destHeight;
                    width = (sourWidth * destHeight) / sourHeight;
                }
            }
            else
            {
                width = sourWidth;
                height = sourHeight;
            }

            Bitmap Preview = new Bitmap(destWidth, destHeight);

            using (Graphics g = Graphics.FromImage(Preview))
            {
                g.InterpolationMode = InterpolationMode.Low;//HighQualityBicubic
                g.Clear(Color.Transparent);
                g.DrawImage(image, new Rectangle((destWidth - width) / 2, (destHeight - height) / 2, width, height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

                g.Save();
            }

            return Preview;
        }

        private void imageRadioButtonDXT1_CheckedChanged(object sender, EventArgs e)
        {
            ImageTextureType = 1;
        }

        private void imageRadioButtonDXT5_CheckedChanged(object sender, EventArgs e)
        {
            ImageTextureType = 5;
        }

        private void imageRadioButtonARGB32_CheckedChanged(object sender, EventArgs e)
        {
            ImageTextureType = 32;
        }

        private void imageRadioButtonABGR32_CheckedChanged(object sender, EventArgs e)
        {
            ImageTextureType = 33;
        }

        private void shadowRadioButtonDXT1_CheckedChanged(object sender, EventArgs e)
        {
            ShadowTextureType = 1;
        }

        private void shadowRadioButtonDXT5_CheckedChanged(object sender, EventArgs e)
        {
            ShadowTextureType = 5;
        }

        private void overlayRadioButtonARGB32_CheckedChanged(object sender, EventArgs e)
        {
            ShadowTextureType = 32;
        }

        private void shadowRadioButtonABGR32_CheckedChanged(object sender, EventArgs e)
        {
            ShadowTextureType = 33;
        }

        private void overlayRadioButtonDXT1_CheckedChanged(object sender, EventArgs e)
        {
            OverlayTextureType = 1;
        }

        private void overlayRadioButtonDXT5_CheckedChanged(object sender, EventArgs e)
        {
            OverlayTextureType = 5;
        }

        private void shadowRadioButtonARGB32_CheckedChanged(object sender, EventArgs e)
        {
            OverlayTextureType = 32;
        }

        private void overlayRadioButtonABGR32_CheckedChanged(object sender, EventArgs e)
        {
            OverlayTextureType = 33;
        }
    }
}