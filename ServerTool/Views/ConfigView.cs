using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using Reactor = License;

namespace Server.Views
{
    public partial class ConfigView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ConfigView()          //设置界面视图
        {
            InitializeComponent();

            MysteryShipRegionIndexEdit.Properties.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            LairRegionIndexEdit.Properties.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;

            PenetraliumKeyAEdit.Properties.DataSource = SMain.Session.GetCollection<QuestInfo>().Binding;
            PenetraliumKeyBEdit.Properties.DataSource = SMain.Session.GetCollection<QuestInfo>().Binding;
            PenetraliumKeyCEdit.Properties.DataSource = SMain.Session.GetCollection<QuestInfo>().Binding;

            RightDeliverEdit.Properties.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            ErrorDeliverEdit.Properties.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;

            gridControl_GoldBuySet.DataSource = SMain.Session.GetCollection<GameGoldSet>().Binding;

            PlayerExpGridControl.DataSource = SMain.Session.GetCollection<PlayerExpInfo>().Binding;   //经验列表
            WeaponExpGridControl.DataSource = SMain.Session.GetCollection<WeaponExpInfo>().Binding;   //武器经验列表
            AccessoryExpGridControl.DataSource = SMain.Session.GetCollection<AccessoryExpInfo>().Binding;  //首饰经验列表
            CraftExpGridControl.DataSource = SMain.Session.GetCollection<CraftExpInfo>().Binding;     //制作经验列表
            GuildLeveLExpGridControl.DataSource = SMain.Session.GetCollection<GuildLevelExp>().Binding;     //行会经验列表
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LoadSettings();

            LoadLicenseInfo();

            SMain.SetUpView(PlayerExpGridView);
            SMain.SetUpView(WeaponExpGridView);
            SMain.SetUpView(AccessoryExpGridView);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            SaveSettings();
        }

        public void LoadSettings()
        {
            MySqlEnableToggleSwitch.IsOn = Config.MySqlEnable;
            MySqlUserEdit.EditValue = Config.MySqlUser;
            MySqlPassEdit.EditValue = Config.MySqlPassword;
            MySqlIPEdit.EditValue = Config.MySqlIP;
            MySqlPortEdit.EditValue = Config.MySqlPort;
            EncryptDBToggleSwitch.IsOn = Config.EnableDBEncryption;
            EncryptDBPassword.EditValue = Config.DBPassword;

            MysteryShipRegionIndexEdit.EditValue = Config.MysteryShipRegionIndex;
            LairRegionIndexEdit.EditValue = Config.LairRegionIndex;
            PenetraliumKeyAEdit.EditValue = Config.PenetraliumKeyA;
            PenetraliumKeyBEdit.EditValue = Config.PenetraliumKeyB;
            PenetraliumKeyCEdit.EditValue = Config.PenetraliumKeyC;
            RightDeliverEdit.EditValue = Config.RightDeliver;
            ErrorDeliverEdit.EditValue = Config.ErrorDeliver;
        }
        public void SaveSettings()
        {
            Config.MySqlEnable = (bool)MySqlEnableToggleSwitch.IsOn;
            Config.MySqlUser = (string)MySqlUserEdit.EditValue;
            Config.MySqlPassword = (string)MySqlPassEdit.EditValue;
            Config.MySqlIP = (string)MySqlIPEdit.EditValue;
            Config.MySqlPort = (string)MySqlPortEdit.EditValue;
            Config.EnableDBEncryption = EncryptDBToggleSwitch.IsOn;
            Config.DBPassword = (string)EncryptDBPassword.EditValue;

            Config.MysteryShipRegionIndex = (int)MysteryShipRegionIndexEdit.EditValue;
            Config.LairRegionIndex = (int)LairRegionIndexEdit.EditValue;
            Config.LairRegionIndex = (int)LairRegionIndexEdit.EditValue;
            Config.PenetraliumKeyA = (int)PenetraliumKeyAEdit.EditValue;
            Config.PenetraliumKeyB = (int)PenetraliumKeyBEdit.EditValue;
            Config.PenetraliumKeyC = (int)PenetraliumKeyCEdit.EditValue;
            Config.RightDeliver = (int)RightDeliverEdit.EditValue;
            Config.ErrorDeliver = (int)ErrorDeliverEdit.EditValue;

            ConfigReader.Save();
        }

        public void LoadLicenseInfo()
        {
            HardwareIDTextBox.Text = Reactor.Status.HardwareID;
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveSettings();
        }
        private void ReloadButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadSettings();
        }

        private void CheckVersionButton_Click(object sender, EventArgs e)
        {
            byte[] old = Config.ClientHash;
            byte[] old1 = Config.ClientHash1;

            Config.LoadVersion();

            if (Functions.IsMatch(old, Config.ClientHash) || Functions.IsMatch(old1, Config.ClientHash1)) return;
        }
        private void VersionPathEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (OpenDialog.ShowDialog() != DialogResult.OK) return;
        }
        private void VersionPath1Edit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (Open1Dialog.ShowDialog() != DialogResult.OK) return;
        }
        private void MapPathEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (FolderDialog.ShowDialog() != DialogResult.OK) return;
        }

        private void ClientPathEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (FolderDialog.ShowDialog() != DialogResult.OK) return;
        }

        private void EncryptDBToggleSwitch_Toggled(object sender, System.EventArgs e)
        {
            if (Config.EnableDBEncryption == EncryptDBToggleSwitch.IsOn) return;
            Config.EnableDBEncryption = EncryptDBToggleSwitch.IsOn;

            SMain.Session.XOREncrypt = EncryptDBToggleSwitch.IsOn;
            string key = (string)EncryptDBPassword.EditValue;
            SMain.Session.XORKey = string.IsNullOrEmpty(key) ? Config.DBPassword : key;

        }
        private void EncryptDBPassword_EditValueChanged(object sender, System.EventArgs e)
        {
            SMain.Session.XOREncrypt = EncryptDBToggleSwitch.IsOn;
            string key = (string)EncryptDBPassword.EditValue;
            SMain.Session.XORKey = string.IsNullOrEmpty(key) ? Config.DBPassword : key;
        }

        private void MySqlEnableToggleSwitch_EditValueChanged(object sender, EventArgs e)
        {
            if (SMain.Session == null) return;
            if (SMain.Session.IsMySql != MySqlEnableToggleSwitch.IsOn)
                MySqlEnableDescLabelControl.Visible = true;
            else
                MySqlEnableDescLabelControl.Visible = false;
        }
    }
}