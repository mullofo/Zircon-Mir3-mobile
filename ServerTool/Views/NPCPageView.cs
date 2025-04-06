using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using Library;
using Library.SystemModels;
using System;

namespace Server.Views
{
    public partial class NPCPageView : RibbonForm
    {
        public NPCPageView()        //NPC页面视图
        {
            InitializeComponent();

            //NPCPageGridControl.DataSource = SMain.Session.GetCollection<NPCPage>().Binding; 

            //PageLookUpEdit.DataSource = SMain.Session.GetCollection<NPCPage>().Binding;
            ItemInfoLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            MapLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;

            DialogTypeImageComboBox.Items.AddEnum<NPCDialogType>();
            CheckTypeImageComboBox.Items.AddEnum<NPCCheckType>();
            ActionTypeImageComboBox.Items.AddEnum<NPCActionType>();
            ItemTypeImageComboBox.Items.AddEnum<ItemType>();

        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.ServerTool);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(NPCPageGridView);
            SMain.SetUpView(ChecksGridView);
            SMain.SetUpView(ActionsGridView);
            SMain.SetUpView(ButtonsGridView);
            SMain.SetUpView(TypesGridView);
            SMain.SetUpView(GoodsGridView);
        }
    }
}