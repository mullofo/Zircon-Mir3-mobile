using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Vestris.ResourceLib;

namespace LauncherSet
{
    public partial class LMain : Form
    {

        public string strInitFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\LauncherSet.ini";
        //private INIFileHeLp INIFileHeLp = null;
        private string strImagePath = Environment.CurrentDirectory + "\\Image\\";
        public LMain()
        {
            InitializeComponent();
            ConfigReader.Load();
            //this.pictureBox2.Image = Image.FromStream(System.Net.WebRequest.Create(http://www.xyhhxx.com/images/logo/logo.gif).GetResponse().GetResponseStream());

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ConfigReader.Save();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                /*
                INIFileHeLp = new INIFileHeLp(strInitFilePath);
                string strTitle = INIFileHeLp.IniReadValue("LauncherSet", "Title");
                string strUpdateAddress = INIFileHeLp.IniReadValue("LauncherSet", "UpdateAddress");
                string strBackImage = INIFileHeLp.IniReadValue("LauncherSet", "BackImage");
                string strPatchRemark = INIFileHeLp.IniReadValue("LauncherSet", "PatchRemark");
                string strPatchAddress = INIFileHeLp.IniReadValue("LauncherSet", "PatchAdress");
                */
                string strTitle = Config.Title;
                string strUpdateAddress = Config.Host;
                string strBackImage = Config.BackImage;
                string strWebAddress = Config.WebAddress;
                string strPatchRemark = Config.PatchRemark;
                string strPatchAddress = Config.PatchAdress;
                txtTitle.Text = strTitle;
                txtUpdateAddress.Text = strUpdateAddress;
                //pictureBackImage.Image = Image.FromFile(strBackImage);
                strFileName = Config.BackImage;
                try
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile(strBackImage);
                    System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                    img.Dispose();
                    pictureBackImage.Image = bmp;
                }
                catch (System.Exception)
                {
                    //MessageBox.Show(ex.ToString());
                }

                txtWebAddress.Text = strWebAddress;
                txtPatchRemark.Text = strPatchRemark;
                txtPatchAdress.Text = strPatchAddress;
                clientStyle.SelectedIndex = 0;
                strDestFileName = strBackImage;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (
                clientStyle.SelectedIndex == -1 ||
                string.IsNullOrEmpty(this.txtTitle.Text) ||
                string.IsNullOrEmpty(this.txtUpdateAddress.Text) ||
                string.IsNullOrEmpty(this.txtPatchRemark.Text) ||
                string.IsNullOrEmpty(this.txtPatchAdress.Text)
                )
            {
                MessageBox.Show("不能有空！");
                return;
            }
            if (!string.IsNullOrEmpty(strFileName) && !File.Exists(strFileName))
            {
                MessageBox.Show("设置的图片不存在");
                return;
            }


            byte[] exeFile = Properties.Resources.Launcher;
            byte[] titlebytes = Encoding.UTF8.GetBytes(this.txtTitle.Text);
            byte[] cleintStylebytes = Encoding.UTF8.GetBytes(this.clientStyle.SelectedIndex.ToString());
            byte[] webAddress = Encoding.UTF8.GetBytes(this.txtWebAddress.Text);
            byte[] updateUrlbytes = Encoding.UTF8.GetBytes(this.txtUpdateAddress.Text);
            byte[] patchRemarkbytes = Encoding.UTF8.GetBytes(this.txtPatchRemark.Text);
            byte[] patchAdress = Encoding.UTF8.GetBytes(this.txtPatchAdress.Text);

            var outPath = $@".\Launcher\{this.txtTitle.Text}.exe";
            if (!Directory.Exists(@".\Launcher"))
                Directory.CreateDirectory(@".\Launcher");
            using (var file = new FileStream(outPath, FileMode.Create))
            {
                file.Write(exeFile, 0, exeFile.Length);
            }

            //修改图标
            IconFile iconFile = new IconFile(@".\Resources\Mir3.ico");
            IconDirectoryResource iconDirectoryResource = new IconDirectoryResource(iconFile);
            iconDirectoryResource.SaveTo(outPath);

            //追加自定义信息
            var exeBytes = File.ReadAllBytes(outPath);
            FileStream fsObj = new FileStream(outPath, FileMode.Create);
            fsObj.Write(exeBytes, 0, exeBytes.Length);

            fsObj.Write(BitConverter.GetBytes(titlebytes.Length), 0, 4);
            fsObj.Write(titlebytes, 0, titlebytes.Length);

            fsObj.Write(BitConverter.GetBytes(cleintStylebytes.Length), 0, 4);
            fsObj.Write(cleintStylebytes, 0, cleintStylebytes.Length);

            fsObj.Write(BitConverter.GetBytes(updateUrlbytes.Length), 0, 4);
            fsObj.Write(updateUrlbytes, 0, updateUrlbytes.Length);


            fsObj.Write(BitConverter.GetBytes(webAddress.Length), 0, 4);
            fsObj.Write(webAddress, 0, webAddress.Length);

            fsObj.Write(BitConverter.GetBytes(patchRemarkbytes.Length), 0, 4);
            fsObj.Write(patchRemarkbytes, 0, patchRemarkbytes.Length);

            fsObj.Write(BitConverter.GetBytes(patchAdress.Length), 0, 4);
            fsObj.Write(patchAdress, 0, patchAdress.Length);
            if (string.IsNullOrEmpty(strFileName))//如果为空则为默认图片 launcher 要做对应修改了！
            {
                int count = 0;
                fsObj.Write(BitConverter.GetBytes(count), 0, 4);
            }
            else
            {
                FileStream fsImage = new FileStream(strFileName, FileMode.Open);
                BinaryReader br = new BinaryReader(fsImage);
                byte[] imagebytes = br.ReadBytes((int)fsImage.Length);
                fsObj.Write(BitConverter.GetBytes(imagebytes.Length), 0, 4);
                fsObj.Write(imagebytes, 0, imagebytes.Length);
                br.Close();
                fsImage.Close();

            }
            fsObj.Write(BitConverter.GetBytes(exeBytes.Length), 0, 4);
            int verifi = 0x1267101A;
            fsObj.Write(BitConverter.GetBytes(verifi), 0, 4);
            fsObj.Close();
            fsObj.Dispose();


            Config.BackImage = strFileName;
            Config.WebAddress = this.txtWebAddress.Text;
            Config.Host = this.txtUpdateAddress.Text;
            Config.PatchRemark = this.txtPatchRemark.Text;
            Config.PatchAdress = this.txtPatchAdress.Text;
            Config.Title = this.txtTitle.Text;
            /*
            try
            {              
                INIFileHeLp.IniWriteValue("LauncherSet", "Title", this.txtTitle.Text);
                INIFileHeLp.IniWriteValue("LauncherSet", "UpdateAddress", this.txtUpdateAddress.Text);
                //将图片保存到软件目录 

                if (!Directory.Exists(strImagePath))
                    Directory.CreateDirectory(strImagePath);
                if (File.Exists(strDestFileName))
                    File.Delete(strDestFileName);
                pictureBackImage.Image.Save(strDestFileName);            
                INIFileHeLp.IniWriteValue("LauncherSet", "BackImage", strDestFileName);
                INIFileHeLp.IniWriteValue("LauncherSet", "PatchRemark", this.txtPatchRemark.Text);
                INIFileHeLp.IniWriteValue("LauncherSet", "PatchAdress", this.txtPatchAdress.Text);
                MessageBox.Show("操作成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            */

        }


        private string strDestFileName = "";
        private string strFileName = "";
        private void btnSelectPicture_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "JPG文件|*.jpg|BMP文件|*.BMP|所有文件|*.*";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.FilterIndex = 1;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    strFileName = openFileDialog.FileName;
                    FileStream fileStream = new FileStream(strFileName, FileMode.Open, FileAccess.Read);//打开文件
                    byte[] dataImage = new byte[fileStream.Length];//自定义第五个数据位 updateUrl
                    fileStream.Read(dataImage, 0, (int)fileStream.Length);//读出 updateUrl
                    MemoryStream ms = new MemoryStream(dataImage);
                    pictureBackImage.Image = Image.FromStream(ms);
                    ms.Close();
                    fileStream.Close();
                    string strNewFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(strFileName);
                    strDestFileName = strImagePath + strNewFileName;

                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("上传题目图片失败,原因为" + ex.ToString());
            }
        }
    }
}
