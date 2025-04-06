using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class MapInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MapInfoView()       //地图信息视图
        {
            InitializeComponent();

            MapInfoGridControl.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            MapInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;

            LightComboBox.Items.AddEnum<LightSetting>();
            WeatherComboBox.Items.AddEnum<WeatherSetting>();
            DirectionImageComboBox.Items.AddEnum<MirDirection>();
            MapIconImageComboBox.Items.AddEnum<MapIcon>();
            StartClassImageComboBox.Items.AddEnum<RequiredClass>();
        }

        private void MapInfoView_Load(object sender, EventArgs e)
        {
            SMain.SetUpView(MapInfoGridView);
            SMain.SetUpView(GuardsGridView);
            SMain.SetUpView(RegionGridView);
            SMain.SetUpView(MiningGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        private void EditButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (MapViewer.CurrentViewer == null)
            {
                MapViewer.CurrentViewer = new MapViewer();
                MapViewer.CurrentViewer.Show();
            }

            MapViewer.CurrentViewer.BringToFront();

            GridView view = MapInfoGridControl.FocusedView as GridView;

            if (view == null) return;

            MapViewer.CurrentViewer.Save();

            MapViewer.CurrentViewer.MapRegion = view.GetFocusedRow() as MapRegion;
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<MapInfo>.ExportExcel(true);

            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)  //导入
        {
            try
            {
                Helpers.HelperExcel<MapInfo>.ImportExcel(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        /*private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            ExportImportHelp.ExportExcel(this.Text, MapInfoGridView);
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)  //导入
        {
            try
            {
                DataTable dt = null;
                ExportImportHelp.ImportExcel(MapInfoGridView, ref dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MapInfo ItemInfo = MapInfoGridView.GetRow(i) as MapInfo;
                        DataRow DataRow = dt.Rows[i];
                        ItemInfo.Light = ExportImportHelp.GetEnumName<LightSetting>(Convert.ToString(DataRow["Light"]));
                        ItemInfo.Weather = ExportImportHelp.GetEnumName<WeatherSetting>(Convert.ToString(DataRow["Weather"]));
                        ItemInfo.Fight = ExportImportHelp.GetEnumName<FightSetting>(Convert.ToString(DataRow["Fight"]));
                        ItemInfo.Music = (SoundIndex)Enum.Parse(typeof(SoundIndex), Convert.ToString(DataRow["Music"]));
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }*/
        int[] SelectRows = null;
        private void MapInfoGridView_KeyDown(object sender, KeyEventArgs e)
        {//处理复制事件（支持关联）
            try
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C)
                {
                    SelectRows = this.MapInfoGridView.GetSelectedRows();

                    e.Handled = true;
                    List<MapInfo> MapInfoList = new List<MapInfo>();
                    IList<MapInfoCopy> MapInfoCopyList = new List<MapInfoCopy>();
                    foreach (int RowIndex in SelectRows)
                    {
                        MapInfo MapInfo = MapInfoGridView.GetRow(RowIndex) as MapInfo;
                        MapInfo.Guards.ToList().ForEach(o => o.StrMonsterName = o.Monster.MonsterName);//用于对应新对象中的StrMonsterName字段
                        MapInfo.Mining.ToList().ForEach(o => o.StrItemName = o.Item.ItemName);//用于对应新对象中的StrItemName字段
                        MapInfoList.Add(MapInfo);
                    }

                    MapInfoCopyList = TypeUtil.CopyList<MapInfo, MapInfoCopy>(MapInfoList);
                    for (int i = 0; i < MapInfoCopyList.Count; i++)
                    {
                        MapInfoCopyList[i].Guards = TypeUtil.CopyList<GuardInfo, GuardInfoCopy>(MapInfoList.FirstOrDefault(o => o.FileName == MapInfoCopyList[i].FileName).Guards);
                        MapInfoCopyList[i].Mining = TypeUtil.CopyList<MineInfo, MineInfoCopy>(MapInfoList.FirstOrDefault(o => o.FileName == MapInfoCopyList[i].FileName).Mining);
                    }

                    string strJson = JsonHelper.SerializeObject(MapInfoCopyList);
                    Clipboard.SetDataObject(strJson, true);

                }
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
                {

                    if (SelectRows != null)
                    {
                        foreach (int RowIndex in SelectRows)
                        {
                            MapInfo MapInfo = MapInfoGridView.GetRow(RowIndex) as MapInfo;
                            MapInfo MapInfoTemp = SMain.Session.GetCollection<MapInfo>().CreateNewObject();

                            MapInfoTemp.FileName = MapInfo.FileName;
                            MapInfoTemp.Description = MapInfo.Description;
                            MapInfoTemp.MiniMap = MapInfo.MiniMap;
                            MapInfoTemp.Background = MapInfo.Background;
                            MapInfoTemp.Light = MapInfo.Light;
                            MapInfoTemp.Weather = MapInfo.Weather;
                            MapInfoTemp.Fight = MapInfo.Fight;
                            MapInfoTemp.AllowRT = MapInfo.AllowRT;
                            MapInfoTemp.SkillDelay = MapInfo.SkillDelay;
                            MapInfoTemp.CanHorse = MapInfo.CanHorse;
                            MapInfoTemp.AllowTT = MapInfo.AllowTT;
                            MapInfoTemp.DeathDrop = MapInfo.DeathDrop;
                            MapInfoTemp.CanMine = MapInfo.CanMine;
                            MapInfoTemp.CanMarriageRecall = MapInfo.CanMarriageRecall;
                            MapInfoTemp.AllowRecall = MapInfo.AllowRecall;
                            MapInfoTemp.IsDynamic = MapInfo.IsDynamic;
                            MapInfoTemp.MinimumLevel = MapInfo.MinimumLevel;
                            MapInfoTemp.MaximumLevel = MapInfo.MaximumLevel;
                            MapInfoTemp.ReconnectMap = MapInfo.ReconnectMap;
                            MapInfoTemp.Music = MapInfo.Music;
                            MapInfoTemp.MonsterHealth = MapInfo.MonsterHealth;
                            MapInfoTemp.MonsterDamage = MapInfo.MonsterDamage;
                            MapInfoTemp.DropRate = MapInfo.DropRate;
                            MapInfoTemp.ExperienceRate = MapInfo.ExperienceRate;
                            MapInfoTemp.GoldRate = MapInfo.GoldRate;
                            MapInfoTemp.MaxMonsterHealth = MapInfo.MaxMonsterHealth;
                            MapInfoTemp.MaxMonsterDamage = MapInfo.MaxMonsterDamage;
                            MapInfoTemp.MaxDropRate = MapInfo.MaxDropRate;
                            MapInfoTemp.MaxExperienceRate = MapInfo.MaxExperienceRate;
                            MapInfoTemp.MaxGoldRate = MapInfo.MaxGoldRate;
                            MapInfoTemp.HealthRate = MapInfo.HealthRate;
                            MapInfoTemp.BlackScorching = MapInfo.BlackScorching;
                            MapInfoTemp.Script = MapInfo.Script;

                            foreach (GuardInfo GuardInfo in MapInfo.Guards)
                            {
                                GuardInfo GuardInfoTemp = MapInfoTemp.Guards.AddNew();
                                GuardInfoTemp.Map = MapInfoTemp;
                                GuardInfoTemp.Monster = GuardInfo.Monster;
                                GuardInfoTemp.X = GuardInfo.X;
                                GuardInfoTemp.Y = GuardInfo.Y;
                                GuardInfoTemp.Direction = GuardInfo.Direction;
                            }

                            foreach (MapRegion MapRegion in MapInfo.Regions)
                            {
                                MapRegion MapRegionTemp = MapInfoTemp.Regions.AddNew();
                                MapRegionTemp.Map = MapInfoTemp;
                                MapRegionTemp.Description = MapRegion.Description;
                                MapRegionTemp.BitRegion = MapRegion.BitRegion;
                                MapRegionTemp.PointRegion = MapRegion.PointRegion;
                                MapRegionTemp.Size = MapRegion.Size;
                            }

                            foreach (MineInfo MineInfo in MapInfo.Mining)
                            {
                                MineInfo MineInfoTemp = MapInfoTemp.Mining.AddNew();
                                MineInfoTemp.Map = MapInfoTemp;
                                MineInfoTemp.Item = MineInfo.Item;
                                MineInfoTemp.Chance = MineInfo.Chance;
                            }
                        }
                    }
                    else
                    {
                        IDataObject iData = Clipboard.GetDataObject();
                        if (iData.GetDataPresent(DataFormats.Text))
                        {
                            string strJson = (string)iData.GetData(DataFormats.Text);
                            List<MapInfoCopy> MapInfoCopyList = null;
                            try
                            {
                                MapInfoCopyList = JsonHelper.DeserializeJsonToList<MapInfoCopy>(strJson);
                            }
                            catch (System.Exception)
                            {
                                XtraMessageBox.Show("复制内容错误，请确认复制来源理否正确！", "错误");
                                return;
                            }
                            IList<ItemInfo> ItemInfoList = SMain.Session.GetCollection<ItemInfo>().Binding;
                            IList<MonsterInfo> MonsterInfoList = SMain.Session.GetCollection<MonsterInfo>().Binding;

                            for (int i = 0; i < MapInfoCopyList.Count; i++)
                            {
                                MapInfoCopy MapInfo = MapInfoCopyList[i];
                                MapInfo MapInfoTemp = SMain.Session.GetCollection<MapInfo>().CreateNewObject();

                                MapInfoTemp.FileName = MapInfo.FileName;
                                MapInfoTemp.Description = MapInfo.Description;
                                MapInfoTemp.MiniMap = MapInfo.MiniMap;
                                MapInfoTemp.Background = MapInfo.Background;
                                MapInfoTemp.Light = MapInfo.Light;
                                MapInfoTemp.Weather = MapInfo.Weather;
                                MapInfoTemp.Fight = MapInfo.Fight;
                                MapInfoTemp.AllowRT = MapInfo.AllowRT;
                                MapInfoTemp.SkillDelay = MapInfo.SkillDelay;
                                MapInfoTemp.CanHorse = MapInfo.CanHorse;
                                MapInfoTemp.AllowTT = MapInfo.AllowTT;
                                MapInfoTemp.DeathDrop = MapInfo.DeathDrop;
                                MapInfoTemp.CanMine = MapInfo.CanMine;
                                MapInfoTemp.CanMarriageRecall = MapInfo.CanMarriageRecall;
                                MapInfoTemp.AllowRecall = MapInfo.AllowRecall;
                                MapInfoTemp.IsDynamic = MapInfo.IsDynamic;
                                MapInfoTemp.MinimumLevel = MapInfo.MinimumLevel;
                                MapInfoTemp.MaximumLevel = MapInfo.MaximumLevel;
                                //MapInfoTemp.ReconnectMap = MapInfo.ReconnectMap;
                                MapInfoTemp.Music = MapInfo.Music;
                                MapInfoTemp.MonsterHealth = MapInfo.MonsterHealth;
                                MapInfoTemp.MonsterDamage = MapInfo.MonsterDamage;
                                MapInfoTemp.DropRate = MapInfo.DropRate;
                                MapInfoTemp.ExperienceRate = MapInfo.ExperienceRate;
                                MapInfoTemp.GoldRate = MapInfo.GoldRate;
                                MapInfoTemp.MaxMonsterHealth = MapInfo.MaxMonsterHealth;
                                MapInfoTemp.MaxMonsterDamage = MapInfo.MaxMonsterDamage;
                                MapInfoTemp.MaxDropRate = MapInfo.MaxDropRate;
                                MapInfoTemp.MaxExperienceRate = MapInfo.MaxExperienceRate;
                                MapInfoTemp.MaxGoldRate = MapInfo.MaxGoldRate;
                                MapInfoTemp.HealthRate = MapInfo.HealthRate;
                                MapInfoTemp.BlackScorching = MapInfo.BlackScorching;
                                MapInfoTemp.Script = MapInfo.Script;

                                foreach (GuardInfoCopy GuardInfoCopy in MapInfo.Guards)
                                {
                                    GuardInfo GuardInfoTemp = MapInfoTemp.Guards.AddNew();
                                    GuardInfoTemp.Map = MapInfoTemp;
                                    GuardInfoTemp.Monster = MonsterInfoList.FirstOrDefault(o => o.MonsterName == GuardInfoCopy.StrMonsterName);
                                    GuardInfoTemp.X = GuardInfoCopy.X;
                                    GuardInfoTemp.Y = GuardInfoCopy.Y;
                                    GuardInfoTemp.Direction = GuardInfoCopy.Direction;
                                }


                                foreach (MineInfoCopy MineInfoCopy in MapInfo.Mining)
                                {
                                    MineInfo MineInfoTemp = MapInfoTemp.Mining.AddNew();
                                    MineInfoTemp.Map = MapInfoTemp;
                                    MineInfoTemp.Item = ItemInfoList.FirstOrDefault(o => o.ItemName == MineInfoCopy.StrItemName);
                                    MineInfoTemp.Chance = MineInfoCopy.Chance;
                                }

                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("目前剪贴板中数据不可转换为文本", "错误");
                            return;
                        }
                    }
                    XtraMessageBox.Show("复制成功，请到最后一行查看。", "提示");
                }
            }
            catch (System.Exception ex)
            {
                XtraMessageBox.Show(ex.ToString());
            }
        }


        public class MapInfoCopy
        {
            public string FileName
            {
                get { return _FileName; }
                set
                {
                    if (_FileName == value) return;

                    var oldValue = _FileName;
                    _FileName = value;
                }
            }
            private string _FileName;

            public string Description
            {
                get { return _Description; }
                set
                {
                    if (_Description == value) return;

                    var oldValue = _Description;
                    _Description = value;
                }
            }
            private string _Description;

            public int MiniMap
            {
                get { return _MiniMap; }
                set
                {
                    if (_MiniMap == value) return;

                    var oldValue = _MiniMap;
                    _MiniMap = value;
                }
            }
            private int _MiniMap;

            public int Background         //地图背景最底层
            {
                get { return _Background; }
                set
                {
                    if (_Background == value) return;

                    var oldValue = _Background;
                    _Background = value;
                }
            }
            private int _Background;

            public LightSetting Light
            {
                get { return _Light; }
                set
                {
                    if (_Light == value) return;

                    var oldValue = _Light;
                    _Light = value;
                }
            }
            private LightSetting _Light;

            public WeatherSetting Weather  //天气
            {
                get { return _Weather; }
                set
                {
                    if (_Weather == value) return;

                    var oldValue = _Weather;
                    _Weather = value;
                }
            }
            private WeatherSetting _Weather;

            public FightSetting Fight
            {
                get { return _Fight; }
                set
                {
                    if (_Fight == value) return;

                    var oldValue = _Fight;
                    _Fight = value;
                }
            }
            private FightSetting _Fight;

            public bool AllowRT
            {
                get { return _AllowRT; }
                set
                {
                    if (_AllowRT == value) return;

                    var oldValue = _AllowRT;
                    _AllowRT = value;
                }
            }
            private bool _AllowRT;

            public int SkillDelay
            {
                get { return _SkillDelay; }
                set
                {
                    if (_SkillDelay == value) return;

                    var oldValue = _SkillDelay;
                    _SkillDelay = value;
                }
            }
            private int _SkillDelay;

            public bool CanHorse
            {
                get { return _CanHorse; }
                set
                {
                    if (_CanHorse == value) return;

                    var oldValue = _CanHorse;
                    _CanHorse = value;
                }
            }
            private bool _CanHorse;

            public bool AllowTT
            {
                get { return _AllowTT; }
                set
                {
                    if (_AllowTT == value) return;

                    var oldValue = _AllowTT;
                    _AllowTT = value;
                }
            }
            private bool _AllowTT;

            public bool DeathDrop
            {
                get { return _DeathDrop; }
                set
                {
                    if (_DeathDrop == value) return;

                    var oldValue = _DeathDrop;
                    _DeathDrop = value;
                }
            }
            private bool _DeathDrop;

            public bool CanMine
            {
                get { return _CanMine; }
                set
                {
                    if (_CanMine == value) return;

                    var oldValue = _CanMine;
                    _CanMine = value;
                }
            }
            private bool _CanMine;

            public bool CanMarriageRecall
            {
                get { return _CanMarriageRecall; }
                set
                {
                    if (_CanMarriageRecall == value) return;

                    var oldValue = _CanMarriageRecall;
                    _CanMarriageRecall = value;
                }
            }
            private bool _CanMarriageRecall;

            public bool AllowRecall
            {
                get => _AllowRecall;
                set
                {
                    if (_AllowRecall == value) return;

                    bool oldValue = _AllowRecall;
                    _AllowRecall = value;
                }
            }
            private bool _AllowRecall;

            //是否为副本地图
            public bool IsDynamic
            {
                get => _IsDynamic;
                set
                {
                    if (_IsDynamic == value) return;

                    bool oldValue = _IsDynamic;
                    _IsDynamic = value;
                }
            }
            private bool _IsDynamic;

            public int MinimumLevel
            {
                get { return _MinimumLevel; }
                set
                {
                    if (_MinimumLevel == value) return;

                    var oldValue = _MinimumLevel;
                    _MinimumLevel = value;
                }
            }
            private int _MinimumLevel;

            public int MaximumLevel
            {
                get { return _MaximumLevel; }
                set
                {
                    if (_MaximumLevel == value) return;

                    var oldValue = _MaximumLevel;
                    _MaximumLevel = value;
                }
            }
            private int _MaximumLevel;

            public SoundIndex Music
            {
                get { return _Music; }
                set
                {
                    if (_Music == value) return;

                    var oldValue = _Music;
                    _Music = value;
                }
            }
            private SoundIndex _Music;

            public int MonsterHealth
            {
                get { return _MonsterHealth; }
                set
                {
                    if (_MonsterHealth == value) return;

                    var oldValue = _MonsterHealth;
                    _MonsterHealth = value;
                }
            }
            private int _MonsterHealth;

            public int MonsterDamage
            {
                get { return _MonsterDamage; }
                set
                {
                    if (_MonsterDamage == value) return;

                    var oldValue = _MonsterDamage;
                    _MonsterDamage = value;
                }
            }
            private int _MonsterDamage;

            public int DropRate
            {
                get { return _DropRate; }
                set
                {
                    if (_DropRate == value) return;

                    var oldValue = _DropRate;
                    _DropRate = value;
                }
            }
            private int _DropRate;

            public int ExperienceRate
            {
                get { return _ExperienceRate; }
                set
                {
                    if (_ExperienceRate == value) return;

                    var oldValue = _ExperienceRate;
                    _ExperienceRate = value;
                }
            }
            private int _ExperienceRate;

            public int GoldRate
            {
                get { return _GoldRate; }
                set
                {
                    if (_GoldRate == value) return;

                    var oldValue = _GoldRate;
                    _GoldRate = value;
                }
            }
            private int _GoldRate;

            public int MaxMonsterHealth
            {
                get { return _MaxMonsterHealth; }
                set
                {
                    if (_MaxMonsterHealth == value) return;

                    var oldValue = _MaxMonsterHealth;
                    _MaxMonsterHealth = value;
                }
            }
            private int _MaxMonsterHealth;

            public int MaxMonsterDamage
            {
                get { return _MaxMonsterDamage; }
                set
                {
                    if (_MaxMonsterDamage == value) return;

                    var oldValue = _MaxMonsterDamage;
                    _MaxMonsterDamage = value;
                }
            }
            private int _MaxMonsterDamage;

            public int MaxDropRate
            {
                get { return _MaxDropRate; }
                set
                {
                    if (_MaxDropRate == value) return;

                    var oldValue = _MaxDropRate;
                    _MaxDropRate = value;
                }
            }
            private int _MaxDropRate;

            public int MaxExperienceRate
            {
                get { return _MaxExperienceRate; }
                set
                {
                    if (_MaxExperienceRate == value) return;

                    var oldValue = _MaxExperienceRate;
                    _MaxExperienceRate = value;
                }
            }
            private int _MaxExperienceRate;

            public int MaxGoldRate
            {
                get { return _MaxGoldRate; }
                set
                {
                    if (_MaxGoldRate == value) return;

                    var oldValue = _MaxGoldRate;
                    _MaxGoldRate = value;
                }
            }
            private int _MaxGoldRate;

            public int HealthRate                         //地图血值变化
            {
                get { return _HealthRate; }
                set
                {
                    if (_HealthRate == value) return;

                    var oldValue = _HealthRate;
                    _HealthRate = value;
                }
            }
            private int _HealthRate;

            public bool BlackScorching                    //地图黑炎属性设置
            {
                get { return _BlackScorching; }
                set
                {
                    if (_BlackScorching == value) return;

                    var oldValue = _BlackScorching;
                    _BlackScorching = value;
                }
            }
            private bool _BlackScorching;

            public string Script
            {
                get { return _Script; }
                set
                {
                    if (_Script == value) return;
                    var oldValue = _Script;
                    _Script = value;
                }
            }
            private string _Script;

            public IList<GuardInfoCopy> Guards { get; set; }

            public IList<MapRegionCopy> Regions { get; set; }

            public IList<MineInfoCopy> Mining { get; set; }
        }

        public class GuardInfoCopy
        {
            public MonsterInfo MonsterInfo
            {
                get { return _MonsterInfo; }
                set
                {
                    if (_MonsterInfo == value) return;

                    var oldValue = _MonsterInfo;
                    _MonsterInfo = value;
                }
            }
            private MonsterInfo _MonsterInfo;


            public string StrMonsterName
            {
                get { return _StrMonsterName; }
                set
                {
                    if (_StrMonsterName == value) return;

                    var oldValue = _StrMonsterName;
                    _StrMonsterName = value;
                }
            }
            private string _StrMonsterName;


            public int X
            {
                get { return _X; }
                set
                {
                    if (_X == value) return;

                    var oldValue = _X;
                    _X = value;
                }
            }
            private int _X;

            public int Y
            {
                get { return _Y; }
                set
                {
                    if (_Y == value) return;

                    var oldValue = _Y;
                    _Y = value;
                }
            }
            private int _Y;

            public MirDirection Direction
            {
                get { return _Direction; }
                set
                {
                    if (_Direction == value) return;

                    var oldValue = _Direction;
                    _Direction = value;
                }
            }
            private MirDirection _Direction;
        }

        public class MineInfoCopy
        {
            public ItemInfo ItemInfo
            {
                get { return _ItemInfo; }
                set
                {
                    if (_ItemInfo == value) return;

                    var oldValue = _ItemInfo;
                    _ItemInfo = value;


                }
            }
            private ItemInfo _ItemInfo;

            public string StrItemName
            {
                get { return _StrItemName; }
                set
                {
                    if (_StrItemName == value) return;

                    var oldValue = _StrItemName;
                    _StrItemName = value;

                }
            }
            private string _StrItemName;

            public int Chance
            {
                get { return _Chance; }
                set
                {
                    if (_Chance == value) return;

                    var oldValue = _Chance;
                    _Chance = value;


                }
            }
            private int _Chance;

        }

    }
}