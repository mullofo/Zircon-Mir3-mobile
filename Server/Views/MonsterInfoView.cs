using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class MonsterInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MonsterInfoView()        //怪物信息视图
        {
            InitializeComponent();

            MonsterInfoGridControl.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;

            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;

            MonsterImageComboBox.Items.AddEnum<MonsterImage>();
            StatComboBox.Items.AddEnum<Stat>();
            StatImageComboBox.Items.AddEnum<MirAnimation>();
            EffectImageComboBox.Items.AddEnum<LibraryFile>();
            MagicDirImageComboBox.Items.AddEnum<MagicDir>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(MonsterInfoGridView);
            SMain.SetUpView(MonsterInfoStatsGridView);
            SMain.SetUpView(DropsGridView);
            SMain.SetUpView(RespawnsGridView);
            //SMain.SetUpView(MonsterCustomInfoGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<MonsterInfo>.ExportExcel(true);

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
                Helpers.HelperExcel<MonsterInfo>.ImportExcel(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        /*private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            ExportImportHelp.ExportExcel(this.Text, MonsterInfoGridView);
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)  //导入
        {
            try
            {
                DataTable dt = null;
                ExportImportHelp.ImportExcel(MonsterInfoGridView, ref dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MonsterInfo ItemInfo = MonsterInfoGridView.GetRow(i) as MonsterInfo;
                        DataRow DataRow = dt.Rows[i];
                        ItemInfo.Image = ExportImportHelp.GetEnumName<MonsterImage>(Convert.ToString(DataRow["Image"]));
                        //ItemInfo.Image = (MonsterImage)Enum.Parse(typeof(MonsterImage), Convert.ToString(DataRow["Image"]));
                        ItemInfo.File = (LibraryFile)Enum.Parse(typeof(LibraryFile), Convert.ToString(DataRow["File"]));
                        ItemInfo.AttackSound = (SoundIndex)Enum.Parse(typeof(SoundIndex), Convert.ToString(DataRow["AttackSound"]));
                        ItemInfo.StruckSound = (SoundIndex)Enum.Parse(typeof(SoundIndex), Convert.ToString(DataRow["StruckSound"]));
                        ItemInfo.DieSound = (SoundIndex)Enum.Parse(typeof(SoundIndex), Convert.ToString(DataRow["DieSound"]));
                        ItemInfo.Flag = (MonsterFlag)Enum.Parse(typeof(MonsterFlag), Convert.ToString(DataRow["Flag"]));
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");

            }

        }*/
        int[] SelectRows = null;
        private void MonsterInfoGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C)
            {
                SelectRows = this.MonsterInfoGridView.GetSelectedRows();

                e.Handled = true;
                List<MonsterInfo> MonsterInfoList = new List<MonsterInfo>();
                IList<MonsterInfoCopy> MonsterInfoCopyList = new List<MonsterInfoCopy>();
                foreach (int RowIndex in SelectRows)
                {
                    MonsterInfo MonsterInfo = MonsterInfoGridView.GetRow(RowIndex) as MonsterInfo;
                    MonsterInfo.Respawns.ToList().ForEach(o => o.StrMapFileName = o.MapID.ToString());          //用于对应新对象中的StrMonsterName字段
                    MonsterInfo.Respawns.ToList().ForEach(o => o.StrMapRegionDescition = o.Region == null ? "" : o.Region.Description);    //用于对应新对象中的StrMonsterName字段
                    MonsterInfo.Drops.ToList().ForEach(o => o.StrItemName = o.Item.ItemName);                      //用于对应新对象中的StrMonsterName字段
                    MonsterInfo.Events.ToList().ForEach(o => o.StrEventDescription = o.MonsterEvent.Description);  //用于对应新对象中的StrMonsterName字段

                    MonsterInfoList.Add(MonsterInfo);
                }

                MonsterInfoCopyList = TypeUtil.CopyList<MonsterInfo, MonsterInfoCopy>(MonsterInfoList);
                for (int i = 0; i < MonsterInfoCopyList.Count; i++)
                {
                    MonsterInfoCopyList[i].MonsterInfoStats = TypeUtil.CopyList<MonsterInfoStat, MonsterInfoStatCopy>(MonsterInfoList.FirstOrDefault(o => o.MonsterName == MonsterInfoCopyList[i].MonsterName).MonsterInfoStats);
                    MonsterInfoCopyList[i].Respawns = TypeUtil.CopyList<RespawnInfo, RespawnInfoCopy>(MonsterInfoList.FirstOrDefault(o => o.MonsterName == MonsterInfoCopyList[i].MonsterName).Respawns);
                    MonsterInfoCopyList[i].Drops = TypeUtil.CopyList<DropInfo, DropInfoCopy>(MonsterInfoList.FirstOrDefault(o => o.MonsterName == MonsterInfoCopyList[i].MonsterName).Drops);
                    MonsterInfoCopyList[i].Events = TypeUtil.CopyList<MonsterEventTarget, EventTargetCopy>(MonsterInfoList.FirstOrDefault(o => o.MonsterName == MonsterInfoCopyList[i].MonsterName).Events);
                }
                try
                {
                    string strJson = JsonHelper.SerializeObject(MonsterInfoCopyList);
                    Clipboard.SetDataObject(strJson, true);
                }
                catch (System.Exception ex)
                {
                    XtraMessageBox.Show("复制内容错误," + ex.Message, "错误");
                    return;
                }
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
            {// 注意：QuestDetails对象是无法复制的
                if (SelectRows != null)
                {
                    foreach (int RowIndex in SelectRows)
                    {
                        MonsterInfo MonsterInfo = MonsterInfoGridView.GetRow(RowIndex) as MonsterInfo;
                        MonsterInfo MonsterInfoTemp = SMain.Session.GetCollection<MonsterInfo>().CreateNewObject();
                        MonsterInfoTemp.MonsterName = MonsterInfo.MonsterName;
                        MonsterInfoTemp.Image = MonsterInfo.Image;
                        MonsterInfoTemp.File = MonsterInfo.File;
                        MonsterInfoTemp.AttackSound = MonsterInfo.AttackSound;
                        MonsterInfoTemp.StruckSound = MonsterInfo.StruckSound;
                        MonsterInfoTemp.DieSound = MonsterInfo.DieSound;
                        MonsterInfoTemp.BodyShape = MonsterInfo.BodyShape;
                        MonsterInfoTemp.AI = MonsterInfo.AI;
                        MonsterInfoTemp.Level = MonsterInfo.Level;
                        MonsterInfoTemp.ViewRange = MonsterInfo.ViewRange;
                        MonsterInfoTemp.CoolEye = MonsterInfo.CoolEye;
                        MonsterInfoTemp.Experience = MonsterInfo.Experience;
                        MonsterInfoTemp.Undead = MonsterInfo.Undead;
                        MonsterInfoTemp.CanPush = MonsterInfo.CanPush;
                        MonsterInfoTemp.CanTame = MonsterInfo.CanTame;
                        MonsterInfoTemp.AttackDelay = MonsterInfo.AttackDelay;
                        MonsterInfoTemp.MoveDelay = MonsterInfo.MoveDelay;
                        MonsterInfoTemp.IsBoss = MonsterInfo.IsBoss;
                        MonsterInfoTemp.Flag = MonsterInfo.Flag;
                        MonsterInfoTemp.CallFightPet = MonsterInfo.CallFightPet;

                        foreach (MonsterInfoStat MonsterInfoStat in MonsterInfo.MonsterInfoStats)
                        {
                            MonsterInfoStat MonsterInfoStatTemp = MonsterInfoTemp.MonsterInfoStats.AddNew();
                            MonsterInfoStatTemp.Monster = MonsterInfoTemp;
                            MonsterInfoStatTemp.Stat = MonsterInfoStat.Stat;
                            MonsterInfoStatTemp.Amount = MonsterInfoStat.Amount;
                        }
                        foreach (RespawnInfo RespawnInfo in MonsterInfo.Respawns)
                        {
                            RespawnInfo MespawnInfoTemp = MonsterInfoTemp.Respawns.AddNew();
                            MespawnInfoTemp.Monster = MonsterInfoTemp;
                            MespawnInfoTemp.Region = RespawnInfo.Region;
                            MespawnInfoTemp.MapID = RespawnInfo.MapID;
                            MespawnInfoTemp.MapX = RespawnInfo.MapX;
                            MespawnInfoTemp.MapY = RespawnInfo.MapY;
                            MespawnInfoTemp.EventSpawn = RespawnInfo.EventSpawn;
                            MespawnInfoTemp.Delay = RespawnInfo.Delay;
                            MespawnInfoTemp.Count = RespawnInfo.Count;
                            MespawnInfoTemp.DropSet = RespawnInfo.DropSet;
                            MespawnInfoTemp.Announce = RespawnInfo.Announce;
                            MespawnInfoTemp.EasterEventChance = RespawnInfo.EasterEventChance;
                        }

                        foreach (DropInfo DropInfo in MonsterInfo.Drops)
                        {
                            DropInfo DropInfoTemp = MonsterInfoTemp.Drops.AddNew();
                            DropInfoTemp.Monster = MonsterInfoTemp;
                            DropInfoTemp.Item = DropInfo.Item;
                            DropInfoTemp.Chance = DropInfo.Chance;
                            DropInfoTemp.Amount = DropInfo.Amount;
                            DropInfoTemp.DropSet = DropInfo.DropSet;
                            DropInfoTemp.PartOnly = DropInfo.PartOnly;
                            DropInfoTemp.EasterEvent = DropInfo.EasterEvent;
                        }

                        foreach (MonsterEventTarget EventTarget in MonsterInfo.Events)
                        {
                            MonsterEventTarget monsterEventTargetTemp = MonsterInfoTemp.Events.AddNew();
                            monsterEventTargetTemp.Monster = MonsterInfoTemp;
                            monsterEventTargetTemp.MonsterEvent = EventTarget.MonsterEvent;
                            monsterEventTargetTemp.DropSet = EventTarget.DropSet;
                            monsterEventTargetTemp.Value = EventTarget.Value;
                        }

                        foreach (QuestTaskMonsterDetails QuestTaskMonsterDetails in MonsterInfo.QuestDetails)
                        {
                            QuestTaskMonsterDetails QuestTaskMonsterDetailsTemp = MonsterInfoTemp.QuestDetails.AddNew();
                            QuestTaskMonsterDetailsTemp.Monster = MonsterInfoTemp;
                            QuestTaskMonsterDetailsTemp.Task = QuestTaskMonsterDetails.Task;
                            QuestTaskMonsterDetailsTemp.Map = QuestTaskMonsterDetails.Map;
                            QuestTaskMonsterDetailsTemp.Chance = QuestTaskMonsterDetails.Chance;
                            QuestTaskMonsterDetailsTemp.Amount = QuestTaskMonsterDetails.Amount;
                            QuestTaskMonsterDetailsTemp.DropSet = QuestTaskMonsterDetails.DropSet;
                            QuestTaskMonsterDetailsTemp.QuerTaskIndex = QuestTaskMonsterDetails.QuerTaskIndex;
                            QuestTaskMonsterDetailsTemp.MonsterName = QuestTaskMonsterDetails.MonsterName;
                            QuestTaskMonsterDetailsTemp.MapName = QuestTaskMonsterDetails.MapName;
                        }
                    }
                }
                else
                {
                    IDataObject iData = Clipboard.GetDataObject();
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        string strJson = (string)iData.GetData(DataFormats.Text);
                        List<MonsterInfoCopy> MonsterInfoCopyList = null;
                        try
                        {
                            MonsterInfoCopyList = JsonHelper.DeserializeJsonToList<MonsterInfoCopy>(strJson);
                        }
                        catch (System.Exception)
                        {
                            XtraMessageBox.Show("复制内容错误，请确认复制来源理否正确！", "错误");
                            return;
                        }
                        IList<MapRegion> MapRegionList = SMain.Session.GetCollection<MapRegion>().Binding;
                        IList<MonsterEventInfo> EventInfoList = SMain.Session.GetCollection<MonsterEventInfo>().Binding;
                        IList<ItemInfo> ItemInfoList = SMain.Session.GetCollection<ItemInfo>().Binding;

                        for (int i = 0; i < MonsterInfoCopyList.Count; i++)
                        {
                            MonsterInfoCopy MonsterInfo = MonsterInfoCopyList[i];
                            MonsterInfo MonsterInfoTemp = SMain.Session.GetCollection<MonsterInfo>().CreateNewObject();

                            MonsterInfoTemp.MonsterName = MonsterInfo.MonsterName;
                            MonsterInfoTemp.Image = MonsterInfo.Image;
                            MonsterInfoTemp.File = MonsterInfo.File;
                            MonsterInfoTemp.AttackSound = MonsterInfo.AttackSound;
                            MonsterInfoTemp.StruckSound = MonsterInfo.StruckSound;
                            MonsterInfoTemp.DieSound = MonsterInfo.DieSound;
                            MonsterInfoTemp.BodyShape = MonsterInfo.BodyShape;
                            MonsterInfoTemp.AI = MonsterInfo.AI;
                            MonsterInfoTemp.Level = MonsterInfo.Level;
                            MonsterInfoTemp.ViewRange = MonsterInfo.ViewRange;
                            MonsterInfoTemp.CoolEye = MonsterInfo.CoolEye;
                            MonsterInfoTemp.Experience = MonsterInfo.Experience;
                            MonsterInfoTemp.Undead = MonsterInfo.Undead;
                            MonsterInfoTemp.CanPush = MonsterInfo.CanPush;
                            MonsterInfoTemp.CanTame = MonsterInfo.CanTame;
                            MonsterInfoTemp.AttackDelay = MonsterInfo.AttackDelay;
                            MonsterInfoTemp.MoveDelay = MonsterInfo.MoveDelay;
                            MonsterInfoTemp.IsBoss = MonsterInfo.IsBoss;
                            MonsterInfoTemp.Flag = MonsterInfo.Flag;
                            MonsterInfoTemp.CallFightPet = MonsterInfo.CallFightPet;

                            foreach (MonsterInfoStatCopy MonsterInfoStatCopy in MonsterInfo.MonsterInfoStats)
                            {
                                MonsterInfoStat MonsterInfoStatTemp = MonsterInfoTemp.MonsterInfoStats.AddNew();
                                MonsterInfoStatTemp.Monster = MonsterInfoTemp;
                                MonsterInfoStatTemp.Stat = MonsterInfoStatCopy.Stat;
                                MonsterInfoStatTemp.Amount = MonsterInfoStatCopy.Amount;
                            }
                            foreach (RespawnInfoCopy RespawnInfoCopy in MonsterInfo.Respawns)
                            {
                                RespawnInfo MespawnInfoTemp = MonsterInfoTemp.Respawns.AddNew();
                                MespawnInfoTemp.Monster = MonsterInfoTemp;
                                MespawnInfoTemp.Region = MapRegionList.FirstOrDefault(o => o.Map.FileName == RespawnInfoCopy.StrMapFileName && o.Description == RespawnInfoCopy.StrMapFileName);
                                MespawnInfoTemp.EventSpawn = RespawnInfoCopy.EventSpawn;
                                MespawnInfoTemp.Delay = RespawnInfoCopy.Delay;
                                MespawnInfoTemp.Count = RespawnInfoCopy.Count;
                                MespawnInfoTemp.DropSet = RespawnInfoCopy.DropSet;
                                MespawnInfoTemp.Announce = RespawnInfoCopy.Announce;
                                MespawnInfoTemp.EasterEventChance = RespawnInfoCopy.EasterEventChance;
                            }

                            foreach (DropInfoCopy DropInfoCopy in MonsterInfo.Drops)
                            {
                                DropInfo DropInfoTemp = MonsterInfoTemp.Drops.AddNew();
                                DropInfoTemp.Monster = MonsterInfoTemp;
                                DropInfoTemp.Item = ItemInfoList.FirstOrDefault(o => o.ItemName == DropInfoCopy.StrItemName);
                                DropInfoTemp.Chance = DropInfoCopy.Chance;
                                DropInfoTemp.Amount = DropInfoCopy.Amount;
                                DropInfoTemp.DropSet = DropInfoCopy.DropSet;
                                DropInfoTemp.PartOnly = DropInfoCopy.PartOnly;
                                DropInfoTemp.EasterEvent = DropInfoCopy.EasterEvent;
                            }

                            foreach (EventTargetCopy EventTargetCopy in MonsterInfo.Events)
                            {
                                MonsterEventTarget monsterEventTargetTemp = MonsterInfoTemp.Events.AddNew();
                                monsterEventTargetTemp.Monster = MonsterInfoTemp;
                                monsterEventTargetTemp.MonsterEvent = EventInfoList.FirstOrDefault(o => o.Description == EventTargetCopy.StrEventDescription);
                                monsterEventTargetTemp.DropSet = EventTargetCopy.DropSet;
                                monsterEventTargetTemp.Value = EventTargetCopy.Value;
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
    }

    public class MonsterInfoCopy
    {
        public string MonsterName
        {
            get { return _MonsterName; }
            set
            {
                if (_MonsterName == value) return;

                var oldValue = _MonsterName;
                _MonsterName = value;
            }
        }
        private string _MonsterName;

        public MonsterImage Image
        {
            get { return _Image; }
            set
            {
                if (_Image == value) return;

                var oldValue = _Image;
                _Image = value;
            }
        }
        private MonsterImage _Image;

        public LibraryFile File
        {
            get { return _File; }
            set
            {
                if (_File == value) return;

                var oldValue = _File;
                _File = value;
            }
        }

        private LibraryFile _File;

        public SoundIndex AttackSound
        {
            get { return _AttackSound; }
            set
            {
                if (_AttackSound == value) return;

                var oldValue = _AttackSound;
                _AttackSound = value;
            }
        }
        private SoundIndex _AttackSound;

        public SoundIndex StruckSound
        {
            get { return _StruckSound; }
            set
            {
                if (_StruckSound == value) return;

                var oldValue = _StruckSound;
                _StruckSound = value;
            }
        }
        private SoundIndex _StruckSound;

        public SoundIndex DieSound
        {
            get { return _DieSound; }
            set
            {
                if (_DieSound == value) return;

                var oldValue = _DieSound;
                _DieSound = value;
            }
        }
        private SoundIndex _DieSound;

        public int BodyShape
        {
            get { return _BodyShape; }
            set
            {
                if (_BodyShape == value) return;

                var oldValue = _BodyShape;
                _BodyShape = value;
            }
        }
        private int _BodyShape;

        public int AI
        {
            get { return _AI; }
            set
            {
                if (_AI == value) return;

                var oldValue = _AI;
                _AI = value;
            }
        }
        private int _AI;

        public int Level
        {
            get { return _Level; }
            set
            {
                if (_Level == value) return;

                var oldValue = _Level;
                _Level = value;
            }
        }
        private int _Level;

        public int ViewRange
        {
            get { return _ViewRange; }
            set
            {
                if (_ViewRange == value) return;

                var oldValue = _ViewRange;
                _ViewRange = value;
            }
        }
        private int _ViewRange;

        public int CoolEye
        {
            get { return _CoolEye; }
            set
            {
                if (_CoolEye == value) return;

                var oldValue = _CoolEye;
                _CoolEye = value;
            }
        }
        private int _CoolEye;

        public decimal Experience
        {
            get { return _Experience; }
            set
            {
                if (_Experience == value) return;

                var oldValue = _Experience;
                _Experience = value;
            }
        }
        private decimal _Experience;

        public bool Undead
        {
            get { return _Undead; }
            set
            {
                if (_Undead == value) return;

                var oldValue = _Undead;
                _Undead = value;
            }
        }
        private bool _Undead;


        public bool CanPush
        {
            get { return _CanPush; }
            set
            {
                if (_CanPush == value) return;

                var oldValue = _CanPush;
                _CanPush = value;
            }
        }
        private bool _CanPush;

        public bool CanTame
        {
            get { return _CanTame; }
            set
            {
                if (_CanTame == value) return;

                var oldValue = _CanTame;
                _CanTame = value;
            }
        }
        private bool _CanTame;

        public int AttackDelay
        {
            get { return _AttackDelay; }
            set
            {
                if (_AttackDelay == value) return;

                var oldValue = _AttackDelay;
                _AttackDelay = value;
            }
        }
        private int _AttackDelay;

        public int MoveDelay
        {
            get { return _MoveDelay; }
            set
            {
                if (_MoveDelay == value) return;

                var oldValue = _MoveDelay;
                _MoveDelay = value;
            }
        }
        private int _MoveDelay;

        public bool IsBoss
        {
            get { return _IsBoss; }
            set
            {
                if (_IsBoss == value) return;

                var oldValue = _IsBoss;
                _IsBoss = value;
            }
        }
        private bool _IsBoss;

        public MonsterFlag Flag
        {
            get { return _Flag; }
            set
            {
                if (_Flag == value) return;

                var oldValue = _Flag;
                _Flag = value;
            }
        }
        private MonsterFlag _Flag;

        public bool CallFightPet
        {
            get { return _CallFightPet; }
            set
            {
                if (_CallFightPet == value) return;

                var oldValue = _CallFightPet;
                _CallFightPet = value;
            }
        }
        private bool _CallFightPet;

        public IList<MonsterInfoStatCopy> MonsterInfoStats { get; set; }

        public IList<RespawnInfoCopy> Respawns { get; set; }

        public IList<DropInfoCopy> Drops { get; set; }

        public IList<MonsterCustomInfoCopy> MonsterCustomInfos { get; set; }

        public IList<EventTargetCopy> Events { get; set; }

        //public List<QuestTaskMonsterDetailsCopy> QuestDetails { get; set; }

    }

    public class MonsterInfoStatCopy
    {
        public Stat Stat
        {
            get { return _Stat; }
            set
            {
                if (_Stat == value) return;

                var oldValue = _Stat;
                _Stat = value;
            }
        }
        private Stat _Stat;

        public int Amount
        {
            get { return _Amount; }
            set
            {
                if (_Amount == value) return;

                var oldValue = _Amount;
                _Amount = value;
            }
        }
        private int _Amount;
    }

    public class RespawnInfoCopy
    {
        //public MapRegion Region
        //{
        //    get { return _Region; }
        //    set
        //    {
        //        if (_Region == value) return;

        //        var oldValue = _Region;
        //        _Region = value;


        //    }
        //}
        //private MapRegion _Region;

        /// <summary>
        /// 地图ID
        /// </summary>
        public int MapID
        {
            get { return _MapId; }
            set
            {
                if (_MapId == value) return;

                var oldValue = _MapId;
                _MapId = value;
            }
        }
        private int _MapId;
        /// <summary>
        /// 地图X坐标
        /// </summary>
        public int MapX
        {
            get { return _MapX; }
            set
            {
                if (_MapX == value) return;

                var oldValue = _MapX;
                _MapX = value;
            }
        }
        private int _MapX;
        /// <summary>
        /// 地图Y坐标
        /// </summary>
        public int MapY
        {
            get { return _MapY; }
            set
            {
                if (_MapY == value) return;

                var oldValue = _MapY;
                _MapY = value;
            }
        }
        private int _MapY;

        public bool EventSpawn
        {
            get { return _EventSpawn; }
            set
            {
                if (_EventSpawn == value) return;

                var oldValue = _EventSpawn;
                _EventSpawn = value;
            }
        }
        private bool _EventSpawn;


        public int Delay
        {
            get { return _Delay; }
            set
            {
                if (_Delay == value) return;

                var oldValue = _Delay;
                _Delay = value;
            }
        }
        private int _Delay;

        public int Count
        {
            get { return _Count; }
            set
            {
                if (_Count == value) return;

                var oldValue = _Count;
                _Count = value;
            }
        }
        private int _Count;

        public int DropSet
        {
            get { return _DropSet; }
            set
            {
                if (_DropSet == value) return;

                var oldValue = _DropSet;
                _DropSet = value;
            }
        }
        private int _DropSet;

        public bool Announce
        {
            get { return _Announce; }
            set
            {
                if (_Announce == value) return;

                var oldValue = _Announce;
                _Announce = value;
            }
        }
        private bool _Announce;

        public int EasterEventChance
        {
            get { return _EasterEventChance; }
            set
            {
                if (_EasterEventChance == value) return;

                var oldValue = _EasterEventChance;
                _EasterEventChance = value;
            }
        }
        private int _EasterEventChance;

        public string StrMapFileName { get; set; }
        public string StrMapRegionDescition { get; set; }
    }

    public class MonsterCustomInfoCopy
    {
        public MirAnimation Animation
        {
            get { return _Animation; }
            set
            {
                if (_Animation == value) return;

                var oldValue = _Animation;
                _Animation = value;
            }
        }
        private MirAnimation _Animation;

        public int Origin
        {
            get { return _Origin; }
            set
            {
                if (_Origin == value) return;

                var oldValue = _Origin;
                _Origin = value;
            }
        }
        private int _Origin;

        public int Frame
        {
            get { return _Frame; }
            set
            {
                if (_Frame == value) return;

                var oldValue = _Frame;
                _Frame = value;
            }
        }
        private int _Frame;

        public int Format
        {
            get { return _Format; }
            set
            {
                if (_Format == value) return;

                var oldValue = _Format;
                _Format = value;
            }
        }
        private int _Format;

        public int Loop
        {
            get { return _Loop; }
            set
            {
                if (_Loop == value) return;

                var oldValue = _Loop;
                _Loop = value;
            }
        }
        private int _Loop;

        public bool CanReversed
        {
            get { return _CanReversed; }
            set
            {
                if (_CanReversed == value) return;

                var oldValue = _CanReversed;
                _CanReversed = value;
            }
        }
        private bool _CanReversed;

        public bool CanStaticSpeed
        {
            get { return _CanStaticSpeed; }
            set
            {
                if (_CanStaticSpeed == value) return;

                var oldValue = _CanStaticSpeed;
                _CanStaticSpeed = value;
            }
        }
        private bool _CanStaticSpeed;

        public MirAction Action
        {
            get { return _Action; }
            set
            {
                if (_Action == value) return;

                var oldValue = _Action;
                _Action = value;
            }
        }
        private MirAction _Action;

        public LibraryFile Effect
        {
            get { return _Effect; }
            set
            {
                if (_Effect == value) return;

                var oldValue = _Effect;
                _Effect = value;
            }
        }

        private LibraryFile _Effect;

        public int StartIndex
        {
            get { return _StartIndex; }
            set
            {
                if (_StartIndex == value) return;

                var oldValue = _StartIndex;
                _StartIndex = value;
            }
        }
        private int _StartIndex;

        public int FrameCount
        {
            get { return _FrameCount; }
            set
            {
                if (_FrameCount == value) return;

                var oldValue = _FrameCount;
                _FrameCount = value;
            }
        }
        private int _FrameCount;

        public int FrameDelay
        {
            get { return _FrameDelay; }
            set
            {
                if (_FrameDelay == value) return;

                var oldValue = _FrameDelay;
                _FrameDelay = value;
            }
        }
        private int _FrameDelay;

        public int StartLight
        {
            get { return _StartLight; }
            set
            {
                if (_StartLight == value) return;

                var oldValue = _StartLight;
                _StartLight = value;
            }
        }
        private int _StartLight;

        public int EndLight
        {
            get { return _EndLight; }
            set
            {
                if (_EndLight == value) return;

                var oldValue = _EndLight;
                _EndLight = value;
            }
        }
        private int _EndLight;

        public MagicDir MagicDir
        {
            get { return _MagicDir; }
            set
            {
                if (_MagicDir == value) return;

                var oldValue = _MagicDir;
                _MagicDir = value;
            }
        }
        private MagicDir _MagicDir;
    }


    public class EventTargetCopy
    {
        public MonsterEventInfo MonsterEvent
        {
            get { return _monsterEvent; }
            set
            {
                if (_monsterEvent == value) return;

                var oldValue = _monsterEvent;
                _monsterEvent = value;
            }
        }
        private MonsterEventInfo _monsterEvent;

        public int DropSet
        {
            get { return _DropSet; }
            set
            {
                if (_DropSet == value) return;

                var oldValue = _DropSet;
                _DropSet = value;
            }
        }
        private int _DropSet;

        public int Value
        {
            get { return _Value; }
            set
            {
                if (_Value == value) return;

                var oldValue = _Value;
                _Value = value;
            }
        }
        private int _Value;

        public string StrEventDescription { get; set; }
    }

    public class QuestTaskMonsterDetailsCopy
    {
        public QuestTask Task
        {
            get { return _Task; }
            set
            {
                if (_Task == value) return;

                var oldValue = _Task;
                _Task = value;
            }
        }
        private QuestTask _Task;

        //Can Be null
        public MapInfo Map
        {
            get { return _Map; }
            set
            {
                if (_Map == value) return;

                var oldValue = _Map;
                _Map = value;
            }
        }
        private MapInfo _Map;

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

        public int Amount
        {
            get { return _Amount; }
            set
            {
                if (_Amount == value) return;

                var oldValue = _Amount;
                _Amount = value;
            }
        }
        private int _Amount;

        //Can be 0
        public int DropSet
        {
            get { return _DropSet; }
            set
            {
                if (_DropSet == value) return;

                var oldValue = _DropSet;
                _DropSet = value;
            }
        }
        private int _DropSet;

        public int QuerTaskIndex
        {
            get { return _QuerTaskIndex; }
            set
            {
                if (_QuerTaskIndex == value) return;

                var oldValue = _QuerTaskIndex;
                _QuerTaskIndex = value;
            }
        }
        private int _QuerTaskIndex;

        public string MonsterName
        {
            get { return _MonsterName; }
            set
            {
                if (_MonsterName == value) return;

                var oldValue = _MonsterName;
                _MonsterName = value;
            }
        }
        private string _MonsterName;

        public string MapName
        {
            get { return _MapName; }
            set
            {
                if (_MapName == value) return;

                var oldValue = _MapName;
                _MapName = value;


            }
        }
        private string _MapName;
    }
}