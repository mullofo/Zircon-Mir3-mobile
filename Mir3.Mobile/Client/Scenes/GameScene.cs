using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.Scenes.Configs;
using Client.Scenes.Views;
using Library;
using Library.SystemModels;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using C = Library.Network.ClientPackets;
using Color = System.Drawing.Color;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;
using Point = System.Drawing.Point;
using UserObject = Client.Models.UserObject;

//Cleaned
namespace Client.Scenes
{
    /// <summary>
    /// 游戏场景
    /// </summary>
    public sealed class GameScene : DXScene
    {
        #region Debug DXControls

        public DXControl DebugDxControl;

        #endregion

        #region Properties
        public bool Loaded { get; set; }
        public static GameScene Game;

#if Mobile
#else
        public DXImageControl GuildFlag;
#endif

        #region SelectedCell
        /// <summary>
        /// 选定道具单元格
        /// </summary>
        public DXItemCell SelectedCell
        {
            get => _SelectedCell;
            set
            {
                if (_SelectedCell == value) return;

                DXItemCell oldValue = _SelectedCell;
                if (oldValue != null) oldValue.Selected = false;

                _SelectedCell = value;
                if (value != null) value.Selected = true;

                OnSelectedCellChanged(oldValue, value);
            }
        }
        private DXItemCell _SelectedCell;
        public event EventHandler<EventArgs> SelectedCellChanged;
        public void OnSelectedCellChanged(DXItemCell oValue, DXItemCell nValue)
        {
            SelectedCellChanged?.Invoke(this, EventArgs.Empty);

            if (oValue != null)
            {
                //购买界面点击cell刷新背包出售价格
                if (oValue.GridType == GridType.Inventory && InventoryBox.InventoryType == InventoryType.BuySell)
                    InventoryBox.ChangePriceDes();
            }

            if (nValue != null)
            {
                //道具被选中，当前的道具标签不显示
                //if (ItemLabel != null && !ItemLabel.IsDisposed) ItemLabel.Dispose();
                //购买界面点击cell刷新背包出售价格
                if (_SelectedCell.GridType == GridType.Inventory && InventoryBox.InventoryType == InventoryType.BuySell)
                    InventoryBox.ChangePriceDes();
            }

        }
        #endregion

        #region User
        /// <summary>
        /// 设置远程仓库
        /// </summary>
        public bool OnRemoteStorge;
        /// <summary>
        /// 设置自动挂机页是否显示
        /// </summary>
        public bool OnAutoHookTab;
        /// <summary>
        /// 是否启用免蜡
        /// </summary>
        public bool OnBrightBox;
        /// <summary>
        /// 是否启用免助跑
        /// </summary>
        public bool OnRunCheck;
        /// <summary>
        /// 是否启用稳如泰山
        /// </summary>
        public bool OnRockCheck;
        /// <summary>
        /// 自身角色对象
        /// </summary>
        public UserObject User
        {
            get => _User;
            set
            {
                if (_User == value) return;

                _User = value;

                HookHelper.AutoPotionInitialize();
                //HookHelper.LoadPickFilterConfig(value);
                HookHelper.BossFilterInitialize();

                UserChanged();

                //SentrySdk.ConfigureScope(s =>
                //{
                //    s.SetTag("Player", _User?.Name ?? string.Empty);
                //});
            }
        }
        private UserObject _User;

        public uint WarWeaponID
        {
            get => _WarWeaponID;
            set
            {
                if (_WarWeaponID == value) return;

                _WarWeaponID = value;
            }
        }
        private uint _WarWeaponID;

        public Point WarWeaponLocation
        {
            get => _WarWeaponLocation;
            set
            {
                if (_WarWeaponLocation == value) return;

                _WarWeaponLocation = value;
            }
        }
        private Point _WarWeaponLocation;
        #endregion

        #region Observer
        /// <summary>
        /// 观察者
        /// </summary>
        public bool Observer
        {
            get => _Observer;
            set
            {
                if (_Observer == value) return;

                bool oldValue = _Observer;
                _Observer = value;

                OnObserverChanged(oldValue, value);
            }
        }
        private bool _Observer;
        public event EventHandler<EventArgs> ObserverChanged;
        public void OnObserverChanged(bool oValue, bool nValue)
        {
            ObserverChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        public bool GoldPickedUp;

        public MapObject MagicObject,            //施法目标
                         MouseObject,            //鼠标指向的目标
                         TargetObject,           //攻击目标    
                         FocusObject;           //焦点目标
        public DXControl ItemLabel, MagicLabel;

        #region MouseItem
        /// <summary>
        /// 鼠标指向道具
        /// </summary>
        public ClientUserItem MouseItem
        {
            get => _MouseItem;
            set
            {
                if (_MouseItem == value) return;

                ClientUserItem oldValue = _MouseItem;
                _MouseItem = value;

                OnMouseItemChanged(oldValue, value);
            }
        }
        private ClientUserItem _MouseItem;
        public event EventHandler<EventArgs> MouseItemChanged;
        public void OnMouseItemChanged(ClientUserItem oValue, ClientUserItem nValue)
        {
            MouseItemChanged?.Invoke(this, EventArgs.Empty);

            CreateItemLabel();
        }
        #endregion

        #region MouseMagic
        /// <summary>
        /// 鼠标指向魔法技能信息
        /// </summary>
        public MagicInfo MouseMagic
        {
            get => _MouseMagic;
            set
            {
                if (_MouseMagic == value) return;

                MagicInfo oldValue = _MouseMagic;
                _MouseMagic = value;

                OnMouseMagicChanged(oldValue, value);
            }
        }
        private MagicInfo _MouseMagic;
        public event EventHandler<EventArgs> MouseMagicChanged;
        public void OnMouseMagicChanged(MagicInfo oValue, MagicInfo nValue)
        {
            MouseMagicChanged?.Invoke(this, EventArgs.Empty);

            if (MagicLabel != null && !MagicLabel.IsDisposed) MagicLabel.Dispose();
            MagicLabel = null;
            CreateMagicLabel();
        }
        #endregion

        /// <summary>
        /// 地图管理
        /// </summary>
        public MapControl MapControl;
        /// <summary>
        /// 活动事件
        /// </summary>
        public ChatEventDialog ChatEventBox;
#if Mobile
        /// <summary>
        /// 主面板
        /// </summary>
        //public PhoneMainPanel MainPanel;
        /// <summary>
        /// 聊天框界面
        /// </summary>
        public PhoneChatDialog ChatBox;
        public ChatLDialog ChatLBox;
        public ChatRDialog ChatRBox;
        /// <summary>
        /// 药品快捷栏
        /// </summary>
        public PhoneBeltDialog BeltBox;
        public PhoneExpPanel ExpPanel;
        //public PhoneRightButtonsPanel RightButtonsPanel;
        //public PhoneLeftDownButtonsPanel LeftDownButtonsPanel;
        public PhoneLeftCentButtonsPanel LeftCentButtonsPanel;
        /// <summary>
        /// 左上角角色界面
        /// </summary>
        public PhoneUserFunctionControl UserFunctionControl;
        public PhoneLeftUpUserPanel LeftUpUserPanel;
        private PhoneDownCentPanel DownCentPanel;
        public EnemyMemberDialog EnemyMemberBox;
        /// <summary>
        /// 异界之门开启时间
        /// </summary>
        public DXLabel NetherworldGateLabel;
#else
        /// <summary>
        /// 主面板
        /// </summary>
        public MainPanel MainPanel;
        /// <summary>
        /// 聊天框界面
        /// </summary>
        public ChatDialog ChatBox;
        /// <summary>
        /// 药品快捷栏
        /// </summary>
        public BeltDialog BeltBox;
#endif
        /// <summary>
        /// 游戏设置
        /// </summary>
        public DXConfigWindow ConfigBox;
        /// <summary>
        /// 角色背包
        /// </summary>
        public InventoryDialog InventoryBox;
        /// <summary>
        /// 角色背包寄售查询界面
        /// </summary>
        public InventoryJueSeDialog InventoryJueSeBox;
        /// <summary>
        /// 角色界面
        /// </summary>
        public CharacterDialog CharacterBox;
        /// <summary>
        /// 离开窗界面
        /// </summary>
        public ExitDialog ExitBox;
        /// <summary>
        /// 消息文本框
        /// </summary>
        public MsgTipTextBox MsgTipTextBox;
        /// <summary>
        /// 选项卡
        /// </summary>
        public DXTabControl ChatTabBox;
        /// <summary>
        /// 聊天框传送道具信息列表
        /// </summary>
        public List<ClientUserItem> ChatItemList = new List<ClientUserItem>();
        /// <summary>
        /// 聊天信息框
        /// </summary>
        public ChatTextBox ChatTextBox;
        /// <summary>
        /// NPC对话框
        /// </summary>
        public NPCDialog NPCBox;
        /// <summary>
        /// NPC交易买界面
        /// </summary>
        public NPCGoodsDialog NPCGoodsBox;
        /// <summary>
        /// NPC交易卖界面
        /// </summary>
        public NPCSellDialog NPCSellBox;
        /// <summary>
        /// NPC交易一键出售界面
        /// </summary>
        public NPCRootSellDialog NPCRootSellBox;
        /// <summary>
        /// NPC修理界面
        /// </summary>
        public NPCRepairDialog NPCRepairBox;
        /// <summary>
        /// NPC特修界面
        /// </summary>
        public NPCSpecialRepairDialog NPCSpecialRepairBox;
        /// <summary>
        /// NPC精炼石合成界面
        /// </summary>
        public NPCRefinementStoneDialog NPCRefinementStoneBox;
        /// <summary>
        /// NPC精炼界面
        /// </summary>
        public NPCRefineDialog NPCRefineBox;
        /// <summary>
        /// NPC高级精炼界面
        /// </summary>
        public NPCRefineRetrieveDialog NPCRefineRetrieveBox;
        /// <summary>
        /// NPC任务列表框
        /// </summary>
        public NPCQuestListDialog NPCQuestListBox;
        /// <summary>
        /// NPC任务框
        /// </summary>
        public NPCQuestDialog NPCQuestBox;
        /// <summary>
        /// NPC宠物购买界面
        /// </summary>
        public NPCAdoptCompanionDialog NPCAdoptCompanionBox;
        /// <summary>
        /// NPC宠物寄存界面
        /// </summary>
        public NPCCompanionStorageDialog NPCCompanionStorageBox;
        /// <summary>
        /// NPC结婚戒指功能界面
        /// </summary>
        public NPCWeddingRingDialog NPCWeddingRingBox;
        /// <summary>
        /// NPC道具分解碎片界面
        /// </summary>
        public NPCItemFragmentDialog NPCItemFragmentBox;
        /// <summary>
        /// NPC首饰升级界面
        /// </summary>
        public NPCAccessoryUpgradeDialog NPCAccessoryUpgradeBox;
        /// <summary>
        /// NPC首饰熔炼界面
        /// </summary>
        public NPCAccessoryLevelDialog NPCAccessoryLevelBox;
        /// <summary>
        /// NPC首饰重置界面
        /// </summary>
        public NPCAccessoryResetDialog NPCAccessoryResetBox;
        /// <summary>
        /// NPC大师精炼界面
        /// </summary>
        public NPCMasterRefineDialog NPCMasterRefineBox;
        /// <summary>
        /// 技能书合成界面
        /// </summary>
        public NPCBookCombineDialog NPCBookCombineBox;
        /// <summary>
        /// 镶嵌打孔界面
        /// </summary>
        public NPCPerforationDialog NPCPerforationBox;
        /// <summary>
        /// 镶嵌附魔界面
        /// </summary>
        public NPCEnchantingDialog NPCEnchantingBox;
        /// <summary>
        /// 附魔合成界面
        /// </summary>
        public NPCEnchantmentSynthesisDialog NPCEnchantmentSynthesisBox;
        /// <summary>
        /// 古代墓碑
        /// </summary>
        public NPCAncientTombstoneDialog NPCAncientTombstoneBox;
        /// <summary>
        /// 新版首饰合成
        /// </summary>
        public NPCAccessoryCombineDialog NPCAccessoryCombineBox;
        /// <summary>
        /// NPC回购界面
        /// </summary>
        public NPCBuyBackDialog NPCBuyBackBox;
        /// <summary>
        /// 小地图界面
        /// </summary>
        public MiniMapDialog MiniMapBox;
        /// <summary>
        /// 大地图界面
        /// </summary>
        public BigMapDialog BigMapBox;
        /// <summary>
        /// 魔法技能界面
        /// </summary>
        public MagicDialog MagicBox;
        /// <summary>
        /// 魔法技能角色寄售查询界面
        /// </summary>
        public MagicJueSeDialog MagicJueSeBox;
        /// <summary>
        /// 组队界面
        /// </summary>
        public GroupDialog GroupBox;
        /// <summary>
        /// BUFF界面
        /// </summary>
        public BuffDialog BuffBox;
        /// <summary>
        /// 仓库界面
        /// </summary>
        public StorageDialog StorageBox;
        /// <summary>
        /// 大补贴游戏辅助功能
        /// </summary>
        public BigPatchDialog BigPatchBox;
        /// <summary>
        /// 查看玩家角色界面
        /// </summary>
        public InspectDialog InspectBox;
        /// <summary>
        /// 排行榜界面
        /// </summary>
        public RankingDialog RankingBox;
        /// <summary>
        /// 商城界面
        /// </summary>
        public MarketPlaceDialog MarketPlaceBox;
        /// <summary>
        /// 寄售界面
        /// </summary>
        public MarketSearchDialog MarketSearchBox;
        /// <summary>
        /// 委托界面
        /// </summary>
        public MarketConsignDialog MarketConsignBox;
        /// <summary>
        /// 元宝委托界面
        /// </summary>
        public GameGoldMarketConsignDialog GameGoldMarketConsignBox;
        /// <summary>
        /// 我的寄售
        /// </summary>
        public MyMarketDialog MyMarketBox;
        /// <summary>
        /// 账号寄售界面
        /// </summary>
        public AccountConsignmentDialog AccountConsignmentBox;
        /// <summary>
        /// 拍卖行
        /// </summary>
        public AuctionsDialog AuctionsBox;
        /// <summary>
        /// 拍卖行委托
        /// </summary>
        public AuctionsConsignDialog AuctionsConsignBox;
        /// <summary>
        /// 读取邮件界面
        /// </summary>
        public ReadMailDialog ReadMailBox;
        /// <summary>
        /// 交易窗界面
        /// </summary>
        public TradeDialog TradeBox;
        /// <summary>
        /// 行会界面
        /// </summary>
        public GuildDialog GuildBox;
        /// <summary>
        /// 行会管理成员界面
        /// </summary>
        public GuildMemberDialog GuildMemberBox;
        /// <summary>
        /// 任务界面
        /// </summary>
        public QuestDialog QuestBox;
        /// <summary>
        /// 任务跟踪框
        /// </summary>
        public QuestTrackerDialog QuestTrackerBox;
        /// <summary>
        /// NPC副本时间计时框
        /// </summary>
        public NPCReplicaDialog NPCReplicaBox;
        /// <summary>
        /// NPC副本顶部时间计时框
        /// </summary>
        public NPCTopTagDialog NPCTopTagBox;
        /// <summary>
        /// 宠物界面
        /// </summary>
        public CompanionDialog CompanionBox;
        /// <summary>
        /// 宠物捡取过滤功能界面
        /// </summary>
        public PickUpSettings PickUpSettingsS;
        /// <summary>
        /// 怪物信息框
        /// </summary>
        public MonsterDialog MonsterBox;
#if Mobile
        /// <summary>
        /// 魔法技能快捷栏
        /// </summary>
        public PhoneMagicBarControl MagicBarBox;
#else
        /// <summary>
        /// 魔法技能快捷栏
        /// </summary>
        public MagicBarDialog MagicBarBox;
#endif
        /// <summary>
        /// 编辑字符对话框
        /// </summary>
        public EditCharacterDialog EditCharacterBox;
        /// <summary>
        /// 财富查询功能界面
        /// </summary>
        public FortuneCheckerDialog FortuneCheckerBox;
        /// <summary>
        /// 爆率查询功能界面
        /// </summary>
        public RateQueryDiglog RateQueryBox;
        /// <summary>
        /// NPC武器工艺界面
        /// </summary>
        public NPCWeaponCraftWindow NPCWeaponCraftBox;
        /// <summary>
        /// 传奇宝箱功能界面
        /// </summary>
        public TreasureChest TreasureChestBox;
        /// <summary>
        /// 传奇宝箱开奖界面
        /// </summary>
        public LuckDraw LuckDrawBox;
        /// <summary>
        /// 居中置顶信息公告栏
        /// </summary>
        public ChatNoticeDialog ChatNoticeBox;
        /// <summary>
        /// 钓鱼界面
        /// </summary>
        public FishingDialog FishingBox;
        /// <summary>
        /// 钓鱼状态图标
        /// </summary>
        public FishingStatusIcon FishingStatusBox;
        /// <summary>
        /// NPC D菜单功能
        /// </summary>
        public NPCDKeyDialog NPCDKeyBox;
        /// <summary>
        /// 额外属性加点界面
        /// </summary>
        public AdditionalDialog AdditionalBox;
        /// <summary>
        /// 组队列表信息框
        /// </summary>
        public GroupMemberDialog GroupMemberBox;
        /// <summary>
        /// 制作功能面板
        /// </summary>
        public CraftDialog CraftBox;
        /// <summary>
        /// 制作进度功能界面
        /// </summary>
        public CraftResult CraftResultBox;
        /// <summary>
        /// 记忆传送功能界面
        /// </summary>
        public FixedPointDialog FixedPointBox;
        /// <summary>
        /// 泰山许愿池功能界面
        /// </summary>
        public VowDialog VowBox;
        /// <summary>
        /// 顶部快捷图标功能面板
        /// </summary>
        public ShortcutDialog ShortcutBox;
        /// <summary>
        /// 倒计时bar
        /// </summary>
        public CountdownBar CountdownBox;
        /// <summary>
        /// 新版武器升级
        /// </summary>
        public NPCWeaponUpgradeDialog NPCWeaponUpgradeBox;
        /// <summary>
        /// 顶部信息框
        /// </summary>
        public TopInfoDialog TopInfoBox;
        /// <summary>
        /// 交流面板
        /// </summary>
        public CommunicationDialog CommunicationBox;

        /// <summary>
        /// 奖金池界面
        /// </summary>
        public BonusPoolDialog BonusPoolBox;
        /// <summary>
        /// 奖金池排行版
        /// </summary>
        public BonusPoolVersionDialog BonusPoolVersionBox;
        /// <summary>
        /// 私聊名字记录框
        /// </summary>
        public ChatWhisperDialog LatestWhispers;
        /// <summary>
        /// 金币交易行
        /// </summary>
        public GoldTradingBusinessDialog GoldTradingBusinessBox;
        /// <summary>
        /// 攻城兵器界面
        /// </summary>
        public WarWeaponDialog WarWeaponBox;

        /// <summary>
        /// 屏幕中间安全区信息框
        /// 进出安全区提示
        /// </summary>
        public WarningObject TipWarning;
        /// <summary>
        /// 背包格子数量定义
        /// </summary>
        public ClientUserItem[] Inventory = new ClientUserItem[Globals.InventorySize];
        /// <summary>
        /// 碎片包格子数量定义
        /// </summary>
        public ClientUserItem[] PatchGrid = new ClientUserItem[1002];
        /// <summary>
        /// 人物装备格子数量定义
        /// </summary>
        public ClientUserItem[] Equipment = new ClientUserItem[Globals.EquipmentSize];
        /// <summary>
        /// 钓鱼装备格子数量定义
        /// </summary>
        public ClientUserItem[] FishingEquipment = new ClientUserItem[Globals.FishingEquipmentSize];

        /// <summary>
        /// 角色任务日志列表
        /// </summary>
        public List<ClientUserQuest> QuestLog = new List<ClientUserQuest>();
        /// <summary>
        /// 角色成就日志列表
        /// </summary>
        public List<ClientUserAchievement> AchievementLog = new List<ClientUserAchievement>();
        /// <summary>
        /// 行会站
        /// </summary>
        public HashSet<string> GuildWars = new HashSet<string>();
        /// <summary>
        /// 行会联盟
        /// </summary>
        public HashSet<string> GuildAlliances = new HashSet<string>();
        /// <summary>
        /// 行会攻城战
        /// </summary>
        public HashSet<CastleInfo> ConquestWars = new HashSet<CastleInfo>();
        /// <summary>
        /// 沙巴克城主人
        /// </summary>
        public Dictionary<CastleInfo, string> CastleOwners = new Dictionary<CastleInfo, string>();


        public SortedDictionary<uint, ClientObjectData> DataDictionary = new SortedDictionary<uint, ClientObjectData>();

        public Dictionary<ItemInfo, ClientFortuneInfo> FortuneDictionary = new Dictionary<ItemInfo, ClientFortuneInfo>();
        /// <summary>
        /// 移动框架
        /// </summary>
        public bool MoveFrame { get; set; }
        /// <summary>
        /// 移动时间
        /// </summary>
        private DateTime MoveTime;
        /// <summary>
        /// 输出时间
        /// </summary>
        private DateTime OutputTime;
        /// <summary>
        /// 道具刷新恢复时间
        /// </summary>
        private DateTime ItemRefreshTime;
        /// <summary>
        /// 目标开关时间
        /// </summary>
        private DateTime TargetSwitchTime;
        /// <summary>
        /// 能否跑
        /// </summary>
        public bool CanRun;
        /// <summary>
        /// 推能否助跑
        /// </summary>
        public bool CanPush;

        /// <summary>
        /// 跑不停开关
        /// </summary>
        public bool AutoRun
        {
            get => _AutoRun;
            set
            {
                if (_AutoRun == value) return;
                _AutoRun = value;

                ReceiveChat(value ? "GameScene.AutoRunOn".Lang() : "GameScene.AutoRunOff".Lang(), MessageType.Hint);
            }
        }
        private bool _AutoRun;

        #region StorageSize
        /// <summary>
        /// 仓库格子容量大小
        /// </summary>
        public int StorageSize
        {
            get { return _StorageSize; }
            set
            {
                if (_StorageSize == value) return;

                int oldValue = _StorageSize;
                _StorageSize = value;

                OnStorageSizeChanged(oldValue, value);
            }
        }
        private int _StorageSize;
        public void OnStorageSizeChanged(int oValue, int nValue)
        {
            StorageBox.RefreshStorage();
        }
        #endregion

        #region PatchGridSize
        /// <summary>
        /// 碎片包裹格子容量大小
        /// </summary>
        public int PatchGridSize
        {
            get { return _PatchGridSize; }
            set
            {
                if (_PatchGridSize == value) return;

                int oldValue = _PatchGridSize;
                _PatchGridSize = value;

                OnPatchGridSizeChanged(oldValue, value);
            }
        }
        private int _PatchGridSize;
        public void OnPatchGridSizeChanged(int oValue, int nValue)
        {
            InventoryBox.RefreshPatchGrid();
        }
        #endregion

        #region NPCID
        /// <summary>
        /// NPC的序号
        /// </summary>
        public uint NPCID
        {
            get => _NPCID;
            set
            {
                if (_NPCID == value) return;

                uint oldValue = _NPCID;
                _NPCID = value;

                OnNPCIDChanged(oldValue, value);
            }
        }
        private uint _NPCID;
        public void OnNPCIDChanged(uint oValue, uint nValue)
        {
            if (MapControl?.Objects == null || NPCQuestListBox == null) return;

            foreach (MapObject ob in MapControl.Objects)
            {
                if (ob.Race != ObjectType.NPC || ob.ObjectID != NPCID) continue;

                NPCQuestListBox.NPCInfo = ((NPCObject)ob).NPCInfo;
                return;
            }
            NPCQuestListBox.NPCInfo = null;
        }
        #endregion

        #region Companion
        /// <summary>
        /// 角色宠物
        /// </summary>
        public ClientUserCompanion Companion
        {
            get => _Companion;
            set
            {
                if (_Companion == value) return;

                _Companion = value;

                CompanionChanged();
            }
        }
        private ClientUserCompanion _Companion;
        #endregion

        /// <summary>
        /// 角色配偶
        /// </summary>
        public ClientPlayerInfo Partner
        {
            get => _Partner;
            set
            {
                if (_Partner == value) return;

                _Partner = value;

                MarriageChanged();
            }
        }
        private ClientPlayerInfo _Partner;
        /// <summary>
        /// 检查ID
        /// </summary>
        public uint InspectID;
        /// <summary>
        /// 捡取时间
        /// </summary>
        public DateTime PickUpTime;
        /// <summary>
        /// 角色道具时间
        /// </summary>
        public DateTime UseItemTime;
        /// <summary>
        /// NPC点击时间
        /// </summary>
        public DateTime NPCTime;
        /// <summary>
        /// 切换时间
        /// </summary>
        public DateTime ToggleTime;
        /// <summary>
        /// 检查时间
        /// </summary>
        public DateTime InspectTime;
        /// <summary>
        /// 道具时间
        /// </summary>
        public DateTime ItemTime = CEnvir.Now;
        /// <summary>
        /// 回生丸复活时间
        /// </summary>
        public DateTime ReincarnationPillTime;
        /// <summary>
        /// 道具复活时间
        /// </summary>
        public DateTime ItemReviveTime;
        /// <summary>
        /// 白天时间
        /// </summary>
        public float DayTime
        {
            get => _DayTime;
            set
            {
                if (_DayTime == value) return;

                _DayTime = value;
                MapControl.LLayer.UpdateLights();
            }
        }
        private float _DayTime;

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            SetDefaultLocations();

            foreach (DXWindow window in DXWindow.Windows)
                window.LoadSettings();

            //LoadChatTabs();
        }

        /// <summary>
        /// 自动打怪开关
        /// </summary>
        public bool AutoAttack
        {
            get => _AutoAttack;
            set
            {
                if (_AutoAttack == value) return;
                _AutoAttack = value;

                ReceiveChat(value ? "自动打怪 开".Lang() : "自动打怪 关".Lang(), MessageType.Hint);
            }
        }
        private bool _AutoAttack;

        /// <summary>
        /// 持续施法开关
        /// </summary>
        public bool ContinuouslyMagic
        {
            get => _ContinuouslyMagic;
            set
            {
                if (_ContinuouslyMagic == value) return;
                _ContinuouslyMagic = value;

                ReceiveChat(value ? "GameScene.ContinuouslyMagicOn".Lang() : "GameScene.ContinuouslyMagicOff".Lang(), MessageType.Hint);
            }
        }
        private bool _ContinuouslyMagic;
        public MagicType ContinuouslyMagicType
        {
            get => _ContinuouslyMagicType;
            set
            {
                if (_ContinuouslyMagicType == value) return;
                _ContinuouslyMagicType = value;
            }
        }
        private MagicType _ContinuouslyMagicType = MagicType.None;

        public MagicType LockMagicType
        {
            get => _LockMagicType;
            set
            {
                if (_LockMagicType == value) return;
                _LockMagicType = value;
            }
        }
        private MagicType _LockMagicType = MagicType.None;

#if Mobile
        public DXButton TownReviveButton;

        public DXStick VirtualWalkStick;
#endif
        #endregion

        /// <summary>
        /// 游戏场景(窗口界面)
        /// </summary>
        /// <param name="size"></param>
        public GameScene(Size size) : base(size)
        {
            //每个场景实例化之前都要设置下偏移
            //Location = Functions.ScalePointXOffset(new Point(0, 0), CEnvir.UIScale, (int)Math.Round(CEnvir.UI_Offset_X / ZoomRate));

            DrawTexture = false;

            Init();
            PreSceneEvent += () => DoExitGame();
        }

        public void Init()
        {
            Game = this;

            //foreach (NPCInfo info in Globals.NPCInfoList.Binding)
            //    info.CurrentIcon = QuestIcon.None;

            MapControl = new MapControl
            {
                Parent = this,
                Size = Size,
            };
            //MapControl.MouseWheel += (o, e) =>
            //{
            //    if (!ChatBox.DisplayArea.Contains(e.Location) || !ChatBox.Visible) return;
            //    ChatBox.ChatPanel_MouseWheel(ChatBox.ScrollBar, e);
            //};

#if Mobile
            //MainPanel = new PhoneMainPanel
            //{
            //    Parent = this
            //};

            ChatBox = new PhoneChatDialog
            {
                Parent = this,
                Visible = false,
            };
            ChatLBox = new ChatLDialog
            {
                Parent = this,
            };
            NetherworldGateLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                ForeColour = Color.Yellow,
                PassThrough = true,
            };
            NetherworldGateLabel.Size = new Size(ChatLBox.Size.Width, CEnvir.GetFontSize(NetherworldGateLabel.Font).Height);
            ChatRBox = new ChatRDialog
            {
                Parent = this,
            };

            BeltBox = new PhoneBeltDialog
            {
                Parent = this,
                Visible = true,
            };

            ExpPanel = new PhoneExpPanel
            {
                Parent = this,
            };

            //RightButtonsPanel = new PhoneRightButtonsPanel
            //{
            //    Parent = this,
            //};

            //LeftDownButtonsPanel = new PhoneLeftDownButtonsPanel
            //{
            //    Parent = this,
            //};

            LeftCentButtonsPanel = new PhoneLeftCentButtonsPanel
            {
                Parent = this,
            };

            GroupMemberBox = new GroupMemberDialog
            {
                Parent = this,
                Visible = false
            };

            UserFunctionControl = new PhoneUserFunctionControl
            {
                Parent = this,
                Visible = false,
            };

            LeftUpUserPanel = new PhoneLeftUpUserPanel
            {
                Parent = this,
            };

            DownCentPanel = new PhoneDownCentPanel
            {
                Parent = this,
            };

            TownReviveButton = new DXButton
            {
                Parent = this,
                Size = new Size(90, 25),
                ButtonType = ButtonType.Default,
                Label = { Text = "复活回城" },
                Visible = false,
            };
            TownReviveButton.TouchUp += (o, e) =>
            {
                if (Observer) return;

                if (!User.Dead)
                {
                    TownReviveButton.Visible = false;
                    return;
                }
                //沙巴克复活时间增加15秒  地图是沙巴克地图  处于攻城时间
                CastleInfo warCastle = ConquestWars.FirstOrDefault(x => x.Map == MapControl.MapInfo);
                if (CEnvir.Now < MapObject.User.WarReviveTime.AddSeconds(15) && MapControl.MapInfo.Index == 25 && warCastle != null)
                {
                    foreach (CastleInfo castle in ConquestWars)   //攻城时的颜色设置 遍历行会攻城站
                    {
                        if (castle.Map != MapControl.MapInfo) continue;   //如果攻城站地图信息不对 继续

                        if (CastleOwners.TryGetValue(castle, out string ownerGuild))
                        {
                            if (ownerGuild == Game.User.Title)
                            {
                                ReceiveChat($"死亡复活冷却延迟 {(MapObject.User.WarReviveTime.AddSeconds(15) - CEnvir.Now).TotalSeconds.ToString("0")} 秒。", MessageType.System);
                                return;
                            }
                        }
                    }
                }
                CEnvir.Enqueue(new C.TownRevive());
                TownReviveButton.Visible = false;
            };

            EnemyMemberBox = new EnemyMemberDialog
            {
                Parent = this,
                //Visible = false
            };
#else
            MainPanel = new MainPanel
            {
                Parent = this
            };

            GuildFlag = new DXImageControl
            {
                Parent = this,
                Visible = false,
                LibraryFile = LibraryFile.UI1,
                Movable = false,
                IsControl = false,
                Location = new Point(-10, Config.GameSize.Height - 240)
            };

            
            ChatBox = new ChatDialog
            {
                Parent = this,
            };
            
            BeltBox = new BeltDialog
            {
                Parent = this,
            };

            GroupMemberBox = new GroupMemberDialog
            {
                Parent = this,
                Visible = false
            };

#endif


            ConfigBox = new DXConfigWindow
            {
                Parent = this,
                Visible = false,
                NetworkTab = { Enabled = false, TabButton = { Visible = false } },
            };

            ExitBox = new ExitDialog
            {
                Parent = this,
                Visible = false,
            };

            InventoryBox = new InventoryDialog
            {
                Parent = this,
                Visible = false,
            };

            InventoryJueSeBox = new InventoryJueSeDialog
            {
                Parent = this,
                Visible = false,
            };

            CharacterBox = new CharacterDialog
            {
                Parent = this,
                Visible = false,
            };

            ChatTextBox = new ChatTextBox
            {
                Parent = ChatBox.BackGround,
                //Visible = false,
            };

            MsgTipTextBox = new MsgTipTextBox
            {
                Parent = this,
                Visible = false
            };
            NPCBox = new NPCDialog
            {
                Parent = this,
                Visible = false
            };
            NPCGoodsBox = new NPCGoodsDialog
            {
                Parent = this,
                Visible = false
            };

            NPCSellBox = new NPCSellDialog
            {
                Parent = this,
                Visible = false,
            };

            NPCRootSellBox = new NPCRootSellDialog
            {
                Parent = this,
                Visible = false,
            };

            NPCRepairBox = new NPCRepairDialog
            {
                Parent = this,
                Visible = false,
            };

            NPCSpecialRepairBox = new NPCSpecialRepairDialog
            {
                Parent = this,
                Visible = false,
            };

            NPCQuestListBox = new NPCQuestListDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCQuestBox = new NPCQuestDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCAdoptCompanionBox = new NPCAdoptCompanionDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCCompanionStorageBox = new NPCCompanionStorageDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCWeddingRingBox = new NPCWeddingRingDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCBuyBackBox = new NPCBuyBackDialog
            {
                Parent = this,
                Visible = false,
            };

            MiniMapBox = new MiniMapDialog
            {
                Parent = this,
                Visible = true,
            };
            MagicBox = new MagicDialog()
            {
                Parent = this,
                Visible = false,
            };
            MagicJueSeBox = new MagicJueSeDialog()
            {
                Parent = this,
                Visible = false,
            };
            GroupBox = new GroupDialog()
            {
                Parent = this,
                Visible = false,
            };

            BigMapBox = new BigMapDialog
            {
                Parent = this,
                Visible = false,
            };
            BuffBox = new BuffDialog
            {
                Parent = this,
                Visible = true,
            };
            StorageBox = new StorageDialog
            {
                Parent = this,
                Visible = false
            };
            BigPatchBox = new BigPatchDialog
            {
                Parent = this,
                Visible = false
            };
            NPCRefinementStoneBox = new NPCRefinementStoneDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCItemFragmentBox = new NPCItemFragmentDialog()
            {
                Parent = this,
                Visible = false,
            };
            NPCAccessoryUpgradeBox = new NPCAccessoryUpgradeDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCAccessoryLevelBox = new NPCAccessoryLevelDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCAccessoryResetBox = new NPCAccessoryResetDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCRefineBox = new NPCRefineDialog
            {
                Parent = this,
                Visible = false
            };
            NPCRefineRetrieveBox = new NPCRefineRetrieveDialog
            {
                Parent = this,
                Visible = false
            };
            NPCMasterRefineBox = new NPCMasterRefineDialog
            {
                Parent = this,
                Visible = false
            };

            InspectBox = new InspectDialog
            {
                Parent = this,
                Visible = false
            };
            RankingBox = new RankingDialog
            {
                Parent = this,
                Visible = false
            };

            MarketPlaceBox = new MarketPlaceDialog
            {
                Parent = this,
                Visible = false,
            };
            MarketSearchBox = new MarketSearchDialog
            {
                Parent = this,
                Visible = false,
            };
            MarketConsignBox = new MarketConsignDialog
            {
                Parent = this,
                Visible = false,
            };

            GameGoldMarketConsignBox = new GameGoldMarketConsignDialog
            {
                Parent = this,
                Visible = false,
            };

            MyMarketBox = new MyMarketDialog
            {
                Parent = this,
                Visible = false,
            };

            AccountConsignmentBox = new AccountConsignmentDialog
            {
                Parent = this,
                Visible = false,
            };

            AuctionsBox = new AuctionsDialog
            {
                Parent = this,
                Visible = false,
            };

            AuctionsConsignBox = new AuctionsConsignDialog
            {
                Parent = this,
                Visible = false,
            };

            EditCharacterBox = new EditCharacterDialog
            {
                Parent = this,
                Visible = false
            };

            CommunicationBox = new CommunicationDialog
            {
                Parent = this,
                Visible = false
            };

            ReadMailBox = new ReadMailDialog
            {
                Parent = this,
                Visible = false
            };
            CommunicationBox.CloseButton.MouseClick += (o, e) => { if (ReadMailBox.Visible) ReadMailBox.Visible = false; };

            TradeBox = new TradeDialog
            {
                Parent = this,
                Visible = false
            };
            GuildBox = new GuildDialog
            {
                Parent = this,
                Visible = false
            };
            GuildMemberBox = new GuildMemberDialog
            {
                Parent = this,
                Visible = false
            };

            QuestBox = new QuestDialog
            {
                Parent = this,
                Visible = false
            };
            QuestTrackerBox = new QuestTrackerDialog
            {
                Parent = this,
                Visible = false
            };
            NPCReplicaBox = new NPCReplicaDialog
            {
                Parent = this,
                Visible = false
            };
            NPCTopTagBox = new NPCTopTagDialog
            {
                Parent = this,
                Visible = false
            };

            CompanionBox = new CompanionDialog
            {
                Parent = this,
                Visible = false,
            };
            PickUpSettingsS = new PickUpSettings
            {
                Parent = this,
                Visible = false,
            };
            CompanionBox.CloseButton.MouseClick += (o, e) => { if (PickUpSettingsS.Visible) PickUpSettingsS.Visible = false; };

#if Mobile
            MagicBarBox = new PhoneMagicBarControl
            {
                Parent = this,
            };
#else
            MagicBarBox = new MagicBarDialog
            {
                Parent = this,
                Visible = false,
            };
#endif

            FortuneCheckerBox = new FortuneCheckerDialog
            {
                Parent = this,
                Visible = false,
            };

            RateQueryBox = new RateQueryDiglog
            {
                Parent = this,
                Visible = false,
            };

            NPCWeaponCraftBox = new NPCWeaponCraftWindow
            {
                Visible = false,
                Parent = this,
            };

            TreasureChestBox = new TreasureChest
            {
                Parent = this,
                Visible = false
            };

            LuckDrawBox = new LuckDraw
            {
                Parent = this,
                Visible = false
            };

            ChatNoticeBox = new ChatNoticeDialog
            {
                Parent = this,
                Visible = false
            };

            FishingBox = new FishingDialog
            {
                Parent = this,
                Visible = false
            };

            FishingStatusBox = new FishingStatusIcon
            {
                Parent = this,
                Visible = false
            };

            NPCDKeyBox = new NPCDKeyDialog
            {
                Parent = this,
                Visible = false
            };

            AdditionalBox = new AdditionalDialog
            {
                Parent = this,
                Visible = false
            };

            NPCBookCombineBox = new NPCBookCombineDialog
            {
                Parent = this,
                Visible = false
            };

            NPCPerforationBox = new NPCPerforationDialog
            {
                Parent = this,
                Visible = false
            };

            NPCEnchantingBox = new NPCEnchantingDialog
            {
                Parent = this,
                Visible = false
            };

            NPCEnchantmentSynthesisBox = new NPCEnchantmentSynthesisDialog
            {
                Parent = this,
                Visible = false
            };

            NPCWeaponUpgradeBox = new NPCWeaponUpgradeDialog
            {
                Parent = this,
                Visible = false
            };

            NPCAccessoryCombineBox = new NPCAccessoryCombineDialog
            {
                Parent = this,
                Visible = false
            };

            NPCAncientTombstoneBox = new NPCAncientTombstoneDialog
            {
                Parent = this,
                Visible = false
            };

            CraftBox = new CraftDialog
            {
                Parent = this,
                Visible = false
            };

            CraftResultBox = new CraftResult
            {
                Parent = this,
                Visible = false
            };

            MonsterBox = new MonsterDialog
            {
                Parent = this,
                Visible = false,
            };

            FixedPointBox = new FixedPointDialog
            {
                Parent = this,
                Visible = false,
            };

            VowBox = new VowDialog
            {
                Parent = this,
                Visible = false,
            };

            ShortcutBox = new ShortcutDialog
            {
                Parent = this,
                Visible = Config.ShortcutEnabled,
            };

            CountdownBox = new CountdownBar
            {
                Parent = this,
                Visible = false
            };

            TopInfoBox = new TopInfoDialog
            {
                Parent = this,
                Visible = false
            };

            BonusPoolBox = new BonusPoolDialog
            {
                Parent = this,
                Visible = true
            };

            BonusPoolVersionBox = new BonusPoolVersionDialog
            {
                Parent = this,
                Visible = false
            };

            LatestWhispers = new ChatWhisperDialog
            {
                Parent = this
            };

            GoldTradingBusinessBox = new GoldTradingBusinessDialog
            {
                Parent = this,
                Visible = false
            };


            WarWeaponBox = new WarWeaponDialog
            {
                Parent = this,
                Visible = false
            };
#if Mobile
            VirtualWalkStick = new DXStick
            {
                Parent = this
            };
            VirtualWalkStick.TouchDown += (s, e) =>
            {
                StickMode = StickMode.Walk;
            };
#endif
            ChatEventBox = new ChatEventDialog
            {
                Parent = this,
                Visible = false,
            };

            SetDefaultLocations();

            //LoadChatTabs();

            foreach (DXWindow window in DXWindow.Windows)
                window.LoadSettings();
        }

        #region Methods
        /// <summary>
        /// 横屏切换，适配刘海
        /// </summary>
        public void ChangeLandscape()
        {
            NPCBox.Location = new Point(NativeUI.ScreenOffset.X, 0);
            LeftUpUserPanel.Location = new Point(10 + NativeUI.ScreenOffset.X, 0);
            LeftCentButtonsPanel.Location = new Point(20 + NativeUI.ScreenOffset.X, LeftUpUserPanel.Location.Y + LeftUpUserPanel.Size.Height + 10);
            //LeftDownButtonsPanel.Location = new Point(20 + NativeUI.ScreenOffset.X, Size.Height - LeftDownButtonsPanel.Size.Height - ExpPanel.Size.Height - 10);
            UserFunctionControl.Location = new Point(LeftUpUserPanel.Location.X + 15, LeftUpUserPanel.Location.Y + 100);
            ChatLBox.Location = new Point(NativeUI.ScreenOffset.X == 0 ? 10 : NativeUI.ScreenOffset.X, Size.Height - ChatLBox.Size.Height - 50);
            EnemyMemberBox.Location = new Point(Size.Width - EnemyMemberBox.Size.Width - 24 - NativeUI.ScreenOffset.Y, 20);

            //RightButtonsPanel.Location = new Point(Size.Width - RightButtonsPanel.Size.Width - NativeUI.ScreenOffset.Y, Size.Height - RightButtonsPanel.Size.Height - ExpPanel.Size.Height);
            MagicBarBox.Location = new Point(Size.Width - MagicBarBox.Size.Width - NativeUI.ScreenOffset.Y, Size.Height - MagicBarBox.Size.Height - ExpPanel.Size.Height);
            BeltBox.Location = new Point(Size.Width - BeltBox.Size.Width - NativeUI.ScreenOffset.Y, MagicBarBox.Location.Y - BeltBox.Size.Height);

            BuffBox.Location = new Point(LeftUpUserPanel.Location.X + LeftUpUserPanel.Size.Width, LeftUpUserPanel.Location.Y + 25);
            DownCentPanel.Location = new Point(MagicBarBox.Location.X - DownCentPanel.Size.Width - 10, ExpPanel.Location.Y - DownCentPanel.Size.Height);
            GroupMemberBox.Location = new Point(LeftCentButtonsPanel.Location.X + LeftCentButtonsPanel.Size.Width, LeftCentButtonsPanel.Location.Y);

            VirtualWalkStick.Location = new Point(DownCentPanel.Location.X - VirtualWalkStick.Size.Width - 10, Size.Height - VirtualWalkStick.Size.Height - 10);

            MiniMapBox.Location = new Point(Size.Width - MiniMapBox.Size.Width - 50, 0);

            NetherworldGateLabel.Location = new Point(ChatLBox.Location.X, ExpPanel.Location.Y - NetherworldGateLabel.Size.Height - 10);
        }
        /// <summary>
        /// 设置窗体默认显示的位置
        /// </summary>
        private void SetDefaultLocations()
        {
            if (ConfigBox == null) return;

            ConfigBox.Location = new Point((Size.Width - ConfigBox.Size.Width) / 2, (Size.Height - ConfigBox.Size.Height) / 2);

            ExitBox.Location = new Point((Size.Width - ExitBox.Size.Width) / 2, (Size.Height - ExitBox.Size.Height) / 2);

            TradeBox.Location = new Point((Size.Width - TradeBox.Size.Width) / 2, (Size.Height - TradeBox.Size.Height) / 2);

            GuildBox.Location = new Point((Size.Width - GuildBox.Size.Width) / 2, (Size.Height - GuildBox.Size.Height) / 2);

            GuildMemberBox.Location = new Point((Size.Width - GuildMemberBox.Size.Width) / 2, (Size.Height - GuildMemberBox.Size.Height) / 2);

            InventoryJueSeBox.Location = new Point(Size.Width - InventoryJueSeBox.Size.Width, 0);

            MapControl.Size = Size;

            //UI主面板左边版面坐标
#if Mobile
            InventoryBox.Location = new Point(Size.Width / 2, 20);
            CharacterBox.Location = new Point(Size.Width / 2 - CharacterBox.Size.Width, 20);
            ExpPanel.Location = new Point(1, Size.Height - ExpPanel.Size.Height);
            ChatBox.Location = new Point((Size.Width - ChatBox.Size.Width) / 2, 20);
            //ChatTextBox.Size = new Size(286, 20);
            ChatTextBox.Location = new Point(20, 268);
            MagicBox.Location = new Point((Size.Width - MagicBox.Size.Width) / 2 - 50, 0);
            TownReviveButton.Location = new Point((Size.Width - TownReviveButton.Size.Width) / 2, (Size.Height - TownReviveButton.Size.Height) / 2 - 40);
            ChangeLandscape();
            ChatRBox.Location = new Point((Size.Width - ChatRBox.Size.Width) / 2 - 50, DownCentPanel.Location.Y - ChatRBox.Size.Height - 10);
            QuestBox.Location = new Point((Size.Width - QuestBox.Size.Width) / 2, (Size.Height - QuestBox.Size.Height) / 2);
            BigPatchBox.Location = new Point((Size.Width - BigPatchBox.Size.Width) / 2, 10);

#else
            NPCBox.Location = Point.Empty;
            InventoryBox.Location = new Point(Size.Width - InventoryBox.Size.Width, 0);
            CharacterBox.Location = Point.Empty;
            MainPanel.Location = new Point((Size.Width - MainPanel.Size.Width) / 2, Size.Height - MainPanel.Size.Height);
            ChatBox.Location = new Point((Size.Width - ChatBox.Size.Width) / 2 + 6, Size.Height - 320);
            MagicBox.Location = new Point(Size.Width - MagicBox.Size.Width, 0);
            ChatTextBox.Location = new Point((Size.Width - MainPanel.Size.Width) / 2 + 190, Size.Height - ChatTextBox.Size.Height + 20);
            //物品快捷栏显示坐标
            BeltBox.Location = new Point((Size.Width - BeltBox.Size.Width) / 2, MainPanel.Location.Y - BeltBox.Size.Height);
            GroupMemberBox.Location = new Point(0 - 10, (Size.Height - GroupMemberBox.Size.Height) / 6 - 100);
            MiniMapBox.Location = new Point(Size.Width - MiniMapBox.Size.Width - 20, 0);
#endif

            LatestWhispers.Location = new Point(ChatTextBox.Location.X, ChatTextBox.Location.Y - LatestWhispers.Size.Height - 4);

            MsgTipTextBox.Location = new Point((Size.Width - MsgTipTextBox.Size.Width) / 2, Convert.ToInt32(Math.Ceiling(Size.Height * 0.2)));


            NPCGoodsBox.Location = new Point(0, 0);

            NPCSellBox.Location = new Point(NPCGoodsBox.Size.Width, NPCBox.Size.Height);

            NPCRootSellBox.Location = new Point(0, NPCBox.Size.Height);

            NPCRepairBox.Location = new Point(0, NPCBox.Size.Height);

            NPCSpecialRepairBox.Location = new Point(0, NPCBox.Size.Height);

            NPCBuyBackBox.Location = new Point(0, NPCBox.Size.Height);


            QuestTrackerBox.Location = new Point(Size.Width - QuestTrackerBox.Size.Width, MiniMapBox.Size.Height + 10);

            NPCReplicaBox.Location = new Point(Size.Width - QuestTrackerBox.Size.Width, MiniMapBox.Size.Height + 200);

            NPCTopTagBox.Location = new Point((Size.Width - GroupBox.Size.Width) / 2, 0);

            MagicJueSeBox.Location = new Point(Size.Width - MagicJueSeBox.Size.Width, 0);

            GroupBox.Location = new Point((Size.Width - GroupBox.Size.Width) / 2, (Size.Height - GroupBox.Size.Height) / 2);

            StorageBox.Location = new Point(Size.Width - StorageBox.Size.Width - InventoryBox.Size.Width, 0);

            InspectBox.Location = new Point(CharacterBox.Size.Width, 0);

            RankingBox.Location = new Point((Size.Width - RankingBox.Size.Width) / 2, 10);

            MarketPlaceBox.Location = new Point((Size.Width - MarketPlaceBox.Size.Width) / 2, (Size.Height - MarketPlaceBox.Size.Height) / 2);

            MarketSearchBox.Location = new Point((Size.Width - MarketSearchBox.Size.Width) / 2, 20);

            MarketConsignBox.Location = new Point(50, (Size.Height - MarketConsignBox.Size.Height) / 4);

            GameGoldMarketConsignBox.Location = new Point(50, (Size.Height - GameGoldMarketConsignBox.Size.Height) / 4);

            MyMarketBox.Location = new Point((Size.Width - MyMarketBox.Size.Width) / 2, (Size.Height - MyMarketBox.Size.Height) / 2);

            AccountConsignmentBox.Location = new Point((Size.Width - AccountConsignmentBox.Size.Width) / 2, (Size.Height - AccountConsignmentBox.Size.Height) / 2);

            AuctionsBox.Location = new Point((Size.Width - AuctionsBox.Size.Width) / 2, (Size.Height - AuctionsBox.Size.Height) / 2);

            AuctionsConsignBox.Location = new Point((Size.Width - AuctionsConsignBox.Size.Width) / 2, (Size.Height - AuctionsConsignBox.Size.Height) / 2);

            CommunicationBox.Location = new Point((Size.Width - CommunicationBox.Size.Width) / 2, (Size.Height - CommunicationBox.Size.Height) / 2);

            ReadMailBox.Location = new Point(CommunicationBox.Size.Width - ReadMailBox.Size.Width, (Size.Height - CommunicationBox.Size.Height) / 2);

            CompanionBox.Location = new Point((Size.Width - CompanionBox.Size.Width) / 2, (Size.Height - CompanionBox.Size.Height) / 2);

            PickUpSettingsS.Location = new Point(CompanionBox.Size.Width - PickUpSettingsS.Size.Width, (Size.Height - CompanionBox.Size.Height) / 2);

            EditCharacterBox.Location = new Point((Size.Width - EditCharacterBox.Size.Width) / 2, (Size.Height - EditCharacterBox.Size.Height) / 2);

            FortuneCheckerBox.Location = new Point((Size.Width - FortuneCheckerBox.Size.Width) / 2, (Size.Height - FortuneCheckerBox.Size.Height) / 2);

            RateQueryBox.Location = new Point((Size.Width - RateQueryBox.Size.Width) / 2, (Size.Height - RateQueryBox.Size.Height) / 2);

            NPCWeaponCraftBox.Location = new Point((Size.Width - NPCWeaponCraftBox.Size.Width) / 2, (Size.Height - NPCWeaponCraftBox.Size.Height) / 2);

            NPCAccessoryCombineBox.Location = new Point((Size.Width - NPCAccessoryCombineBox.Size.Width) / 2, (Size.Height - NPCAccessoryCombineBox.Size.Height) / 2);

            TreasureChestBox.Location = new Point((Size.Width - TreasureChestBox.Size.Width) / 2, (Size.Height - TreasureChestBox.Size.Height) / 2);

            LuckDrawBox.Location = new Point((Size.Width - LuckDrawBox.Size.Width) / 2, (Size.Height - LuckDrawBox.Size.Height) / 2);

            ChatNoticeBox.Location = new Point((Size.Width - ChatNoticeBox.Size.Width) / 2, (Size.Height - ChatNoticeBox.Size.Height) / 6);

            FishingBox.Location = new Point((Size.Width - FishingBox.Size.Width) / 2, (Size.Height - FishingBox.Size.Height) / 2);

            NPCAdoptCompanionBox.Location = new Point((Size.Width - NPCAdoptCompanionBox.Size.Width) / 2, (Size.Height - NPCAdoptCompanionBox.Size.Height) / 2);

            NPCDKeyBox.Location = Point.Empty;

            AdditionalBox.Location = Point.Empty;

            NPCPerforationBox.Location = new Point((Size.Width - NPCPerforationBox.Size.Width) / 2, (Size.Height - NPCPerforationBox.Size.Height) / 2);

            NPCEnchantingBox.Location = new Point((Size.Width - NPCEnchantingBox.Size.Width) / 2, (Size.Height - NPCEnchantingBox.Size.Height) / 2);

            NPCEnchantmentSynthesisBox.Location = new Point((Size.Width - NPCEnchantmentSynthesisBox.Size.Width) / 2, (Size.Height - NPCEnchantmentSynthesisBox.Size.Height) / 2);

            NPCWeaponUpgradeBox.Location = new Point((Size.Width - NPCWeaponUpgradeBox.Size.Width) / 2, (Size.Height - NPCWeaponUpgradeBox.Size.Height) / 2);

            NPCAncientTombstoneBox.Location = new Point((Size.Width - NPCAncientTombstoneBox.Size.Width) / 2, (Size.Height - NPCAncientTombstoneBox.Size.Height) / 2);

            CraftBox.Location = new Point((Size.Width - CraftBox.Size.Width) / 2, (Size.Height - CraftBox.Size.Height) / 2);

            FixedPointBox.Location = new Point((Size.Width - FixedPointBox.Size.Width) / 4, (Size.Height - FixedPointBox.Size.Height) / 2);

            VowBox.Location = new Point((Size.Width - VowBox.Size.Width) / 2, (Size.Height - VowBox.Size.Height) / 2);

            ShortcutBox.Location = Point.Empty;

            FishingStatusBox.Location = new Point((Size.Width - FishingStatusBox.Size.Width) / 2 - 5, (Size.Height - FishingStatusBox.Size.Height) / 2 - 120);

            TopInfoBox.Location = new Point(0, 0);

            //BonusPoolBox.Location = new Point(Size.Width - BonusPoolBox.Size.Width, Size.Height - MainPanel.Size.Height + 36);

            BonusPoolVersionBox.Location = new Point((Size.Width - BonusPoolVersionBox.Size.Width) / 2, (Size.Height - BonusPoolVersionBox.Size.Height) / 2);

            GoldTradingBusinessBox.Location = new Point((Size.Width - GoldTradingBusinessBox.Size.Width) / 2, (Size.Height - GoldTradingBusinessBox.Size.Height) / 2);

            WarWeaponBox.Location = new Point(Size.Width - WarWeaponBox.Size.Width - 30, Size.Height - WarWeaponBox.Size.Height - 160);
        }

        //int MapAnimationCount = 1;  //地图动画计数
        /*public void SaveChatTabs()  //保存聊天记录
        {
            DBCollection<ChatTabControlSetting> controlSettings = CEnvir.Session.GetCollection<ChatTabControlSetting>();
            DBCollection<ChatTabPageSetting> pageSettings = CEnvir.Session.GetCollection<ChatTabPageSetting>();

            for (int i = controlSettings.Binding.Count - 1; i >= 0; i--)
                controlSettings.Binding[i].Delete();

            foreach (DXControl temp1 in Controls)
            {
                DXTabControl tabControl = temp1 as DXTabControl;

                if (tabControl == null) continue;

                ChatTabControlSetting cSetting = controlSettings.CreateNewObject();

                cSetting.Resolution = Config.GameSize;
                cSetting.Location = tabControl.Location;
                cSetting.Size = tabControl.Size;

                foreach (DXControl tempC in tabControl.Controls)
                {
                    ChatTab tab = tempC as ChatTab;
                    if (tab == null) continue;

                    ChatTabPageSetting pSetting = pageSettings.CreateNewObject();

                    pSetting.Parent = cSetting;

                    if (tabControl.SelectedTab == tab)
                        cSetting.SelectedPage = pSetting;

                    pSetting.Name = tab.Panel.NameTextBox.TextBox.Text;
                    pSetting.Transparent = tab.Panel.TransparentCheckBox.Checked;
                    pSetting.Alert = tab.Panel.AlertCheckBox.Checked;
                    pSetting.LocalChat = tab.Panel.LocalCheckBox.Checked;
                    pSetting.WhisperChat = tab.Panel.WhisperCheckBox.Checked;
                    pSetting.GroupChat = tab.Panel.GroupCheckBox.Checked;
                    pSetting.GuildChat = tab.Panel.GuildCheckBox.Checked;
                    pSetting.ShoutChat = tab.Panel.ShoutCheckBox.Checked;
                    pSetting.GlobalChat = tab.Panel.GlobalCheckBox.Checked;
                    pSetting.ObserverChat = tab.Panel.ObserverCheckBox.Checked;
                    pSetting.HintChat = tab.Panel.HintCheckBox.Checked;
                    pSetting.SystemChat = tab.Panel.SystemCheckBox.Checked;
                    pSetting.GainsChat = tab.Panel.GainsCheckBox.Checked;
                }
            }
        }

        public void LoadChatTabs()  //加载聊天选项卡
        {
            if (ConfigBox == null) return;

            for (int i = ChatTab.Tabs.Count - 1; i >= 0; i--)
                ChatTab.Tabs[i].Panel.RemoveButton.InvokeMouseClick();

            DBCollection<ChatTabControlSetting> controlSettings = CEnvir.Session.GetCollection<ChatTabControlSetting>();

            bool result = false;
            foreach (ChatTabControlSetting cSetting in controlSettings.Binding)
            {
                if (cSetting.Resolution != Config.GameSize) continue;

                result = true;

                DXTabControl tabControl = new DXTabControl
                {
                    Location = cSetting.Location,
                    Size = cSetting.Size,
                    Parent = this,
                };

                ChatTab selected = null;
                foreach (ChatTabPageSetting pSetting in cSetting.Controls)
                {
                    ChatTab tab = ChatOptionsBox.AddNewTab();

                    tab.Parent = tabControl;

                    tab.Panel.NameTextBox.TextBox.Text = pSetting.Name;
                    tab.Panel.TransparentCheckBox.Checked = pSetting.Transparent;
                    tab.Panel.AlertCheckBox.Checked = pSetting.Alert;
                    tab.Panel.LocalCheckBox.Checked = pSetting.LocalChat;
                    tab.Panel.WhisperCheckBox.Checked = pSetting.WhisperChat;
                    tab.Panel.GroupCheckBox.Checked = pSetting.GroupChat;
                    tab.Panel.GuildCheckBox.Checked = pSetting.GuildChat;
                    tab.Panel.ShoutCheckBox.Checked = pSetting.ShoutChat;
                    tab.Panel.GlobalCheckBox.Checked = pSetting.GlobalChat;
                    tab.Panel.ObserverCheckBox.Checked = pSetting.ObserverChat;

                    tab.Panel.HintCheckBox.Checked = pSetting.HintChat;
                    tab.Panel.SystemCheckBox.Checked = pSetting.SystemChat;
                    tab.Panel.GainsCheckBox.Checked = pSetting.GainsChat;

                    if (pSetting == cSetting.SelectedPage)
                        selected = tab;
                }

                tabControl.SelectedTab = selected;
            }

            if (result)
                Game.ReceiveChat("已加载聊天布局", MessageType.Announcement);
            else
                ChatOptionsBox.CreateDefaultWindows();
        }*/
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            #region 喝药
            HookHelper.AutoHP();
            HookHelper.AutoMP();
            #endregion

            if (!Observer) BigPatchBox?.UpdateAutoAssist();// 大补帖辅助部分

            if (CEnvir.Now >= MoveTime)
            {
                MoveTime = CEnvir.Now.AddMilliseconds(100);
                MapControl.Animation++;
                MoveFrame = true;
            }
            else
            {
                MoveFrame = false;
            }

            if (CEnvir.Ctrl && DXTextBox.ActiveTextBox == ChatTextBox.TextBox && !LatestWhispers.Visible)
            {
                LatestWhispers.Visible = true;
                LatestWhispers.Location = new Point(ChatTextBox.Location.X, ChatTextBox.Location.Y - LatestWhispers.Size.Height - 4);
            }
            if (!CEnvir.Ctrl && LatestWhispers.Visible)
            {
                LatestWhispers.Visible = false;
            }

#if Mobile
#else
            if (MouseControl == MapControl)
                MapControl.CheckCursor();
#endif

            if (MouseControl == MapControl)
            {
                if (CEnvir.Ctrl && MapObject.MouseObject?.Race == ObjectType.Item)
                    MouseItem = ((ItemObject)MapObject.MouseObject).Item;
                else
                    MouseItem = null;
            }

            //计时道具计时
            if (ItemTime.AddSeconds(1) <= CEnvir.Now)
            {
                TimeSpan ticks = CEnvir.Now - ItemTime;
                ItemTime = CEnvir.Now;

                //玩家不为空         玩家不在安全区 或 服务端设置安全区扣消耗限时物品时间
                if (User != null && !User.InSafeZone || CEnvir.ClientControl.InSafeZoneItemExpireCheck)
                {
                    foreach (ClientUserItem item in Equipment)
                    {
                        if ((item?.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;

                        if (CEnvir.ClientControl.InSafeZoneItemExpireCheck)  //服务端设置安全区扣消耗限时物品时间
                        {
                            TimeSpan remainTicks = item.ExpireTime - (item.ExpireDateTime - ItemTime); //根据过期日期换算出tick
                            item.ExpireTime -= remainTicks;
                        }
                        else
                            item.ExpireTime -= ticks;
                    }

                    foreach (ClientUserItem item in Inventory)
                    {
                        if ((item?.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;

                        if (CEnvir.ClientControl.InSafeZoneItemExpireCheck)  //服务端设置安全区扣消耗限时物品时间
                        {
                            TimeSpan remainTicks = item.ExpireTime - (item.ExpireDateTime - ItemTime); //根据过期日期换算出tick
                            item.ExpireTime -= remainTicks;
                            //System.Diagnostics.Debug.WriteLine(CEnvir.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "  " + item.ExpireTime + "  " + item.ExpireDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        }
                        else
                            item.ExpireTime -= ticks;
                    }

                    if (Companion != null)
                    {
                        foreach (ClientUserItem item in Companion.InventoryArray)
                        {
                            if ((item?.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;

                            if (CEnvir.ClientControl.InSafeZoneItemExpireCheck)  //服务端设置安全区扣消耗限时物品时间
                            {
                                TimeSpan remainTicks = item.ExpireTime - (item.ExpireDateTime - ItemTime); //根据过期日期换算出tick
                                item.ExpireTime -= remainTicks;
                            }
                            else
                                item.ExpireTime -= ticks;
                        }
                        foreach (ClientUserItem item in Companion.EquipmentArray)
                        {
                            if ((item?.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;

                            if (CEnvir.ClientControl.InSafeZoneItemExpireCheck)  //服务端设置安全区扣消耗限时物品时间
                            {
                                TimeSpan remainTicks = item.ExpireTime - (item.ExpireDateTime - ItemTime); //根据过期日期换算出tick
                                item.ExpireTime -= remainTicks;
                            }
                            else
                                item.ExpireTime -= ticks;
                        }
                    }
                }
            }

            if (MouseItem != null && CEnvir.Now > ItemRefreshTime)
            {
                CreateItemLabel();
            }

            MapControl.ProcessInput();

            foreach (MapObject ob in MapControl.Objects)
                ob?.Process();

            for (int i = MapControl.Effects.Count - 1; i >= 0; i--)
                MapControl.Effects[i].Process();

            if (ItemLabel != null && !ItemLabel.IsDisposed)
            {
                int x = CEnvir.MouseLocation.X + 15, y = CEnvir.MouseLocation.Y;

                if (x + ItemLabel.Size.Width > Size.Width + Location.X)
                    x = Size.Width - ItemLabel.Size.Width + Location.X;

                if (y + ItemLabel.Size.Height > Size.Height + Location.Y)
                    y = Size.Height - ItemLabel.Size.Height + Location.Y;

                if (x < Location.X)
                    x = Location.X;

                if (y <= Location.Y)
                    y = Location.Y;

                ItemLabel.Location = new Point(x, y);
            }

            if (MagicLabel != null && !MagicLabel.IsDisposed)
            {
                int x = CEnvir.MouseLocation.X + 15, y = CEnvir.MouseLocation.Y;

                if (x + MagicLabel.Size.Width > Size.Width + Location.X)
                    x = Size.Width - MagicLabel.Size.Width + Location.X;

                if (y + MagicLabel.Size.Height > Size.Height + Location.Y)
                    y = Size.Height - MagicLabel.Size.Height + Location.Y;

                if (x < Location.X)
                    x = Location.X;

                if (y <= Location.Y)
                    y = Location.Y;

                MagicLabel.Location = new Point(x, y);
            }

            MonsterObject mob = MouseObject as MonsterObject;
            if (BigPatchConfig.AndroidPlayer)
            {
                //没目标 或者目标已死亡 超过一定时间没打死怪 换目标
                if ((TargetObject == null) || TargetObject.Dead || (BigPatchConfig.ChkChangeTarget && CEnvir.Now > TargetSwitchTime))
                {
                    //新算法离自己最近的目标
                    TargetObject = Game.MapControl.Objects?.OrderBy(
                        x =>
                        (x.CurrentLocation.X - User.CurrentLocation.X) * (x.CurrentLocation.X - User.CurrentLocation.X) +
                        (x.CurrentLocation.Y - User.CurrentLocation.Y) * (x.CurrentLocation.Y - User.CurrentLocation.Y)).Where(
                        x =>
                        SelectMonster(x)).FirstOrDefault();

                    mob = TargetObject as MonsterObject;
                    if (mob != null) MapControl.AutoPath = false;
                    //新目标 刷新切换目标的时间
                    TargetSwitchTime = CEnvir.Now.AddSeconds(BigPatchConfig.TargetTimeRandom);
                }
                else
                {
                    mob = TargetObject as MonsterObject;
                }
                MouseObject = TargetObject;
            }

            if (MouseObject is MonsterObject)
            {
                if (mob != null && mob.CompanionObject == null)
                    MonsterBox.Object = mob;
            }
            else if (MouseObject is PlayerObject && MouseObject.ObjectID != User.ObjectID)
            {
                var player = MouseObject as PlayerObject;
                if (player != null)
                    MonsterBox.Object = player;
            }
            else
                MonsterBox.Object = null;

            if (TipWarning != null && CEnvir.Now - TipWarning.StartTime > TipWarning.Duration)
            {
                TipWarning.Label.Visible = false;
            }

#if Mobile
#else
            //如果武器为空 或 新版武器升级
            if (Game?.Equipment[(int)EquipmentSlot.Weapon] == null || CEnvir.ClientControl.NewWeaponUpgradeCheck)
            {
                Game.TopInfoBox.WeaponEXPLabel.Text = "";
                Game.TopInfoBox.EXPLabel.Location = new Point(Game.TopInfoBox.TimeLabel.Size.Width + 5, 2);
                Game.TopInfoBox.BagWeightLabel.Location = new Point(Game.TopInfoBox.TimeLabel.Size.Width + Game.TopInfoBox.EXPLabel.Size.Width + 10, 2);
                Game.TopInfoBox.ModeLabel.Location = new Point(Game.TopInfoBox.TimeLabel.Size.Width + Game.TopInfoBox.EXPLabel.Size.Width + Game.TopInfoBox.BagWeightLabel.Size.Width + 15, 2);
                Game.TopInfoBox.NetherworldGateLabel.Location = new Point(Game.TopInfoBox.TimeLabel.Location.X, 16);
                Game.TopInfoBox.JinamStoneGateLabel.Location = new Point(Game.TopInfoBox.NetherworldGateLabel.Size.Width + 10, 16);
            }
            else
            {
                //如果没开启新版武器升级 且 武器等级到17满级
                if (!CEnvir.ClientControl.NewWeaponUpgradeCheck && Game?.Equipment[(int)EquipmentSlot.Weapon]?.Level == 17)
                {
                    Game.TopInfoBox.WeaponEXPLabel.Text = "武器经验".Lang() + "  Max";
                    Game.TopInfoBox.EXPLabel.Location = new Point(Game.TopInfoBox.TimeLabel.Size.Width + 5, 2);
                    Game.TopInfoBox.WeaponEXPLabel.Location = new Point(Game.TopInfoBox.TimeLabel.Size.Width + Game.TopInfoBox.EXPLabel.Size.Width + 10, 2);
                    Game.TopInfoBox.BagWeightLabel.Location = new Point(Game.TopInfoBox.TimeLabel.Size.Width + Game.TopInfoBox.EXPLabel.Size.Width + Game.TopInfoBox.WeaponEXPLabel.Size.Width + 15, 2);
                    Game.TopInfoBox.ModeLabel.Location = new Point(Game.TopInfoBox.TimeLabel.Size.Width + Game.TopInfoBox.EXPLabel.Size.Width + Game.TopInfoBox.WeaponEXPLabel.Size.Width + Game.TopInfoBox.BagWeightLabel.Size.Width + 20, 2);
                    Game.TopInfoBox.NetherworldGateLabel.Location = new Point(Game.TopInfoBox.TimeLabel.Location.X, 16);
                    Game.TopInfoBox.JinamStoneGateLabel.Location = new Point(Game.TopInfoBox.NetherworldGateLabel.Size.Width + 10, 16);
                }
                else
                {
                    Game.TopInfoBox.WeaponEXPLabel.Text = $"武器经验".Lang() + $"  {Game?.Equipment[(int)EquipmentSlot.Weapon]?.Experience:###0}/{Globals.GameWeaponEXPInfoList[Game.Equipment[(int)EquipmentSlot.Weapon].Level].Exp:###0}";
                    Game.TopInfoBox.EXPLabel.Location = new Point(Game.TopInfoBox.TimeLabel.Size.Width + 5, 2);
                    Game.TopInfoBox.WeaponEXPLabel.Location = new Point(Game.TopInfoBox.TimeLabel.Size.Width + Game.TopInfoBox.EXPLabel.Size.Width + 10, 2);
                    Game.TopInfoBox.BagWeightLabel.Location = new Point(Game.TopInfoBox.TimeLabel.Size.Width + Game.TopInfoBox.EXPLabel.Size.Width + Game.TopInfoBox.WeaponEXPLabel.Size.Width + 15, 2);
                    Game.TopInfoBox.ModeLabel.Location = new Point(Game.TopInfoBox.TimeLabel.Size.Width + Game.TopInfoBox.EXPLabel.Size.Width + Game.TopInfoBox.WeaponEXPLabel.Size.Width + Game.TopInfoBox.BagWeightLabel.Size.Width + 20, 2);
                    Game.TopInfoBox.NetherworldGateLabel.Location = new Point(Game.TopInfoBox.TimeLabel.Location.X, 16);
                    Game.TopInfoBox.JinamStoneGateLabel.Location = new Point(Game.TopInfoBox.NetherworldGateLabel.Size.Width + 10, 16);
                }
            }
#endif
        }

        /// <summary>
        /// 选择怪物
        /// </summary>
        /// <returns></returns>
        private bool SelectMonster(MapObject ob)
        {
            if (ob.Race != ObjectType.Monster || ob.Dead) return false;
            if (TargetObject != null && TargetObject.ObjectID == ob.ObjectID) return false; //跳过同一个怪物
            //如果怪物超出了视野 不选择
            int dis = Functions.Distance(User.CurrentLocation, ob.CurrentLocation);
            if (dis > CEnvir.ClientControl.MaxViewRange - 1) return false;

            MonsterObject monob = ob as MonsterObject;
            if (monob.MonsterInfo == null) return false;
            if (monob.MonsterInfo.AI == 4) return false;   //栗子树返回
            if (!string.IsNullOrEmpty(ob.PetOwner)) return false;  //过滤掉宠物
            if (monob.MonsterInfo.AI < 0) return false;

            if (BigPatchConfig.AndroidLockRange)
            {
                if (monob.CurrentLocation.X < (int)(BigPatchConfig.AndroidCoord.X - BigPatchConfig.AndroidCoordRange) || monob.CurrentLocation.X > (int)(BigPatchConfig.AndroidCoord.X + BigPatchConfig.AndroidCoordRange)) return false;
                if (monob.CurrentLocation.Y < (int)(BigPatchConfig.AndroidCoord.Y - BigPatchConfig.AndroidCoordRange) || monob.CurrentLocation.Y > (int)(BigPatchConfig.AndroidCoord.Y + BigPatchConfig.AndroidCoordRange)) return false;
            }

            return true;
        }
        /// <summary>
        /// 更新对象数据时 大小地图
        /// </summary>
        /// <param name="data"></param>
        public void OnUpdateObjectData(ClientObjectData data)
        {
            //if (data.ObjectID == User.ObjectID)
            //{
            //    MainPanel.MapControl_MapInfoChanged(this, EventArgs.Empty);
            //}
            BigMapBox.Update(data);
            MiniMapBox.Update(data);
#if Mobile
#else
            ChatBox.Photo.Update(data);
#endif
            BigPatchBox.ChkRandom(data);
            WarWeaponBox.Map.Update(data);
        }

        /// <summary>
        /// 移除对象数据时 大小地图
        /// </summary>
        /// <param name="data"></param>
        public void OnRemoveObjectData(ClientObjectData data)
        {
            BigMapBox.Remove(data);
            MiniMapBox.Remove(data);
#if Mobile
#else
            ChatBox.Photo.Remove(data);
#endif
            WarWeaponBox.Map.Remove(data);
        }

        /// <summary>
        /// BUFF框随地图框位移
        /// </summary>
        /// <param name="e"></param>
        //public override void OnMouseMove(MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);
        //    //if (MiniMapBox.Visible == true)
        //    //{
        //    //    BuffBox.Location = new Point
        //    //    {
        //    //        X = MiniMapBox.Location.X - BuffBox.Size.Width,
        //    //        Y = BuffBox.Location.Y,
        //    //    };
        //    //    BuffBox.Visible = true;
        //    //}
        //    //else
        //    //{
        //    //    BuffBox.Location = new Point
        //    //    {
        //    //        X = Size.Width - BuffBox.Size.Width,
        //    //        Y = BuffBox.Location.Y,
        //    //    };
        //    //    BuffBox.Visible = true;
        //    //}
        //}

        /// <summary>
        /// 按下快捷键时
        /// </summary>
        /// <param name="e"></param>
        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled) return;

            switch (e.KeyCode)
            {
                case Keys.Escape:         //ESC键
                    MonsterBox.Object = null;
                    e.Handled = true;
                    if (Game.InventoryBox.Visible == true)
                    {
                        Game.InventoryBox.Close1Button.InvokeMouseClick();
                        if (!Config.EscapeCloseAll)
                            e.Handled = true;
                    }
                    //if (Game.BeltBox.Visible == true)
                    //{
                    //    Game.BeltBox.CloseBtn.InvokeMouseClick();
                    //    if (!Config.EscapeCloseAll)
                    //        e.Handled = true;
                    //}
                    if (Game.MagicBarBox.Visible == true)
                    {
                        Game.MagicBarBox.Visible = false;
                        if (!Config.EscapeCloseAll)
                            e.Handled = true;
                    }
                    if (Game.StorageBox.Visible == true)
                    {
                        Game.StorageBox.Visible = false;
                        if (!Config.EscapeCloseAll)
                            e.Handled = true;
                    }
                    if (Game.AuctionsBox.Visible == true)
                    {
                        Game.AuctionsBox.Visible = false;
                        if (!Config.EscapeCloseAll)
                            e.Handled = true;
                    }
                    if (Game.BonusPoolVersionBox.Visible == true)
                    {
                        Game.BonusPoolVersionBox.Visible = false;
                        if (!Config.EscapeCloseAll)
                            e.Handled = true;
                    }
                    if (Game.NPCBox.Visible == true)
                    {
                        Game.NPCBox.Visible = false;
                        if (!Config.EscapeCloseAll)
                            e.Handled = true;
                    }
                    break;
                case Keys.Space:   //空格
                case Keys.Enter:   //回车
                    if (ConfirmWindow != null)
                    {
                        ConfirmWindow.OkPress();
                    }
                    else if (!ChatBox.Visible)
                    {
                        if (!ChatTextBox.Visible)
                        {
                            ChatTextBox.Visible = true;
                        }
                        ChatTextBox.OpenChat();
                    }
                    break;
                case Keys.Tab:  //持续施法开关
                    if (Observer) break;
                    //ContinuouslyMagic = !ContinuouslyMagic;
                    break;
            }

            //键位定义
            foreach (KeyBindAction action in CEnvir.GetKeyAction(e.KeyCode))
            {
                switch (action)
                {
                    case KeyBindAction.ConfigWindow:
                        if (Observer) continue;
                        ConfigBox.Visible = !ConfigBox.Visible;
                        break;
                    case KeyBindAction.RankingWindow:
                        if (!CEnvir.ClientControl.RankingShowCheck) return;  //排行榜设置不显示就不设置快捷
                        RankingBox.Visible = !RankingBox.Visible && CEnvir.Connection != null;
                        break;
                    case KeyBindAction.CharacterWindow:
                        CharacterBox.Visible = !CharacterBox.Visible;
                        break;
                    case KeyBindAction.InventoryWindow:
                        InventoryBox.Visible = !InventoryBox.Visible;
                        break;
                    //case KeyBindAction.FortuneWindow:
                    //    if (!CEnvir.ClientControl.RateQueryShowCheck) return;  //爆率查询设置不显示就不设置快捷
                    //    RateQueryBox.Visible = !RateQueryBox.Visible;
                    //    //FortuneCheckerBox.Visible = !FortuneCheckerBox.Visible;
                    //    break;
                    case KeyBindAction.MagicWindow:
                        if (Observer) continue;
                        MagicBox.Visible = !MagicBox.Visible;
                        //MagicBarBox.Visible = MagicBox.Visible;
                        break;
                    case KeyBindAction.MagicBarWindow:
                        MagicBarBox.Visible = !MagicBarBox.Visible;
                        break;
                    case KeyBindAction.GameStoreWindow:
                        if (Observer) continue;
                        MarketPlaceBox.Visible = !MarketPlaceBox.Visible;
                        break;
                    //case KeyBindAction.CompanionWindow:
                    //    if (Globals.CompanionInfoList.Count == 0) return;
                    //    CompanionBox.Visible = !CompanionBox.Visible;
                    //    if (PickUpSettingsS.Visible)
                    //        PickUpSettingsS.Visible = false;
                    //    break;
                    case KeyBindAction.GroupWindow:
                        if (Observer) continue;
                        if (Game.MapControl.MapInfo.CanPlayName == true) continue;
                        GroupBox.Visible = !GroupBox.Visible;
                        break;
                    //case KeyBindAction.StorageWindow:
                    //    if (OnRemoteStorge)
                    //        StorageBox.Visible = !StorageBox.Visible;
                    //    break;
                    case KeyBindAction.GuildWindow:
                        if (Observer) continue;
                        GuildBox.Visible = !GuildBox.Visible;
                        break;
                    //case KeyBindAction.QuestLogWindow:
                    //    QuestBox.Visible = !QuestBox.Visible;
                    //    break;
                    //case KeyBindAction.QuestTrackerWindow:
                    //    if (Observer) continue;
                    //    QuestBox.CurrentTab.ShowTrackerBox.Checked = !QuestBox.CurrentTab.ShowTrackerBox.Checked;
                    //    break;
                    case KeyBindAction.BeltWindow:
                        BeltBox.Visible = !BeltBox.Visible;
                        break;
                    case KeyBindAction.ChatWindow:
#if Mobile
#else
                        if (ChatBox.ShrinkButton.Visible == false)
                        {
                            ChatBox.ExpendButton.InvokeMouseClick();
                        }
                        else
                        {
                            ChatBox.ShrinkButton.InvokeMouseClick();
                        }
#endif
                        break;
                    case KeyBindAction.MarketPlaceWindow:
                        if (Observer) continue;
                        GoldTradingBusinessBox.Visible = !GoldTradingBusinessBox.Visible;
                        break;
                    case KeyBindAction.MapMiniWindow:
#if Mobile
#else
                        if (MiniMapBox.ScaleSize != 1.0f && MiniMapBox.Visible == false)
                        {
                            MiniMapBox.Enlarge.InvokeMouseClick();
                            MiniMapBox.Visible = true;
                            MiniMapBox.UpdateBuffBoxLocation();
                        }
                        else
                        {
                            MiniMapBox.Visible = !MiniMapBox.Visible;
                            MiniMapBox.UpdateBuffBoxLocation();
                        }
#endif
                        break;
                    case KeyBindAction.MapMiniWindowSize:
#if Mobile
#else
                        MiniMapBox.Enlarge.InvokeMouseClick();
#endif
                        break;
                    case KeyBindAction.MapBigWindow:
#if Mobile
#else
                        if (MiniMapBox.Panorama.Visible == false)
                        {
                            MiniMapBox.Enlarge.InvokeMouseClick();
                        }
                        else
                        {
                            MiniMapBox.Panorama.InvokeMouseClick();
                        }
#endif
                        break;
                    case KeyBindAction.CommunicationBoxWindow:
                        if (Observer) continue;
                        CommunicationBox.Visible = !CommunicationBox.Visible;
                        break;
                    case KeyBindAction.ExitGameWindow:
                        DoExitGame();
                        break;
                    case KeyBindAction.ChangeAttackMode:
                        if (Observer) continue;
                        User.AttackMode = (AttackMode)(((int)User.AttackMode + 1) % 5);
                        CEnvir.Enqueue(new C.ChangeAttackMode { Mode = User.AttackMode });
                        break;
                    case KeyBindAction.ChangePetMode:
                        if (Observer) continue;

                        User.PetMode = (PetMode)(((int)User.PetMode + 1) % 5);
                        CEnvir.Enqueue(new C.ChangePetMode { Mode = User.PetMode });
                        break;
                    case KeyBindAction.GroupAllowSwitch:
                        if (Observer) continue;

                        GroupBox.AllowGroupButton.InvokeMouseClick();
                        break;
                    case KeyBindAction.GroupTarget:
                        if (Observer) continue;
                        if (Game.MapControl.MapInfo.CanPlayName == true) continue;

                        if (MouseObject == null || MouseObject.Race != ObjectType.Player) continue;

                        CEnvir.Enqueue(new C.GroupInvite { Name = MouseObject.Name });
                        break;
                    case KeyBindAction.TradeRequest:
                        if (Observer) continue;

                        CEnvir.Enqueue(new C.TradeRequest());
                        break;
                    case KeyBindAction.TradeAllowSwitch:
                        if (Observer) continue;

                        CEnvir.Enqueue(new C.Chat { Text = "@允许交易" });
                        break;
                    case KeyBindAction.ChangeChatMode:
                        if (Observer) continue;
                        ChatTextBox.ChatModeButton.InvokeMouseClick();
                        break;
                    case KeyBindAction.ItemPickUp:   //捡取设置
                        if (Observer) continue;

                        if (BigPatchConfig.ChkTabPick)
                        {
                            if (CEnvir.Now > PickUpTime)
                            {
                                CEnvir.Enqueue(new C.PickUp());
                                PickUpTime = CEnvir.Now.AddMilliseconds(250);
                            }
                        }
                        //Game.AutoPotionBox.AutoPickup();    //范围一键捡取
                        break;
                    case KeyBindAction.PartnerTeleport:
                        if (Observer) continue;

                        CEnvir.Enqueue(new C.MarriageTeleport());
                        break;
                    case KeyBindAction.MountToggle:
                        if (Observer) continue;

                        if (CEnvir.Now < User.NextActionTime || User.ActionQueue.Count > 0) return;
                        if (CEnvir.Now < User.ServerTime) return;
                        if (CEnvir.Now < MapObject.User.CombatTime.AddSeconds(10) && !Game.Observer && GameScene.Game.User.Horse == HorseType.None)
                        {
                            Game.ReceiveChat("战斗中无法骑马".Lang(), MessageType.System);
                            return;
                        }

                        User.ServerTime = CEnvir.Now.AddSeconds(5);
                        CEnvir.Enqueue(new C.Mount());
                        break;
                    case KeyBindAction.AutoRunToggle:
                        if (Observer) continue;

                        AutoRun = !AutoRun;
                        break;
                    case KeyBindAction.UseBelt01:
                        if (Observer) continue;

                        if (BeltBox.Grid.Grid.Length > 0)
                        {
                            if (SelectedCell != null)
                                SelectedCell.MoveItem(BeltBox.Grid.Grid[0]);
                            else
                            {
                                BeltBox.Grid.Grid[0].UseItem();
                                HookHelper.UseHpTime.AddSeconds(5);
                                HookHelper.UseMpTime.AddSeconds(5);
                            }
                        }
                        break;
                    case KeyBindAction.UseBelt02:
                        if (Observer) continue;

                        if (BeltBox.Grid.Grid.Length > 1)
                        {
                            if (SelectedCell != null)
                                SelectedCell.MoveItem(BeltBox.Grid.Grid[1]);
                            else
                            {
                                BeltBox.Grid.Grid[1].UseItem();
                                HookHelper.UseHpTime.AddSeconds(5);
                                HookHelper.UseMpTime.AddSeconds(5);
                            }
                        }
                        break;
                    case KeyBindAction.UseBelt03:
                        if (Observer) continue;

                        if (BeltBox.Grid.Grid.Length > 2)
                        {
                            if (SelectedCell != null)
                                SelectedCell.MoveItem(BeltBox.Grid.Grid[2]);
                            else
                            {
                                BeltBox.Grid.Grid[2].UseItem();
                                HookHelper.UseHpTime.AddSeconds(5);
                                HookHelper.UseMpTime.AddSeconds(5);
                            }
                        }
                        break;
                    case KeyBindAction.UseBelt04:
                        if (Observer) continue;

                        if (BeltBox.Grid.Grid.Length > 3)
                        {
                            if (SelectedCell != null)
                                SelectedCell.MoveItem(BeltBox.Grid.Grid[3]);
                            else
                            {
                                BeltBox.Grid.Grid[3].UseItem();
                                HookHelper.UseHpTime.AddSeconds(5);
                                HookHelper.UseMpTime.AddSeconds(5);
                            }
                        }
                        break;
                    case KeyBindAction.UseBelt05:
                        if (Observer) continue;

                        if (BeltBox.Grid.Grid.Length > 4)
                        {
                            if (SelectedCell != null)
                                SelectedCell.MoveItem(BeltBox.Grid.Grid[4]);
                            else
                            {
                                BeltBox.Grid.Grid[4].UseItem();
                                HookHelper.UseHpTime.AddSeconds(5);
                                HookHelper.UseMpTime.AddSeconds(5);
                            }
                        }
                        break;
                    case KeyBindAction.UseBelt06:
                        if (Observer) continue;

                        if (BeltBox.Grid.Grid.Length > 5)
                        {
                            if (SelectedCell != null)
                                SelectedCell.MoveItem(BeltBox.Grid.Grid[5]);
                            else
                            {
                                BeltBox.Grid.Grid[5].UseItem();
                                HookHelper.UseHpTime.AddSeconds(5);
                                HookHelper.UseMpTime.AddSeconds(5);
                            }
                        }
                        break;
                    case KeyBindAction.SpellSet01:
                        if (Observer) continue;
                        MagicBarBox.SpellSet = 1;
                        break;
                    case KeyBindAction.SpellSet02:
                        if (Observer) continue;
                        MagicBarBox.SpellSet = 2;
                        break;
                    case KeyBindAction.SpellSet03:
                        if (Observer) continue;
                        MagicBarBox.SpellSet = 3;
                        break;
                    case KeyBindAction.SpellSet04:
                        if (Observer) continue;
                        MagicBarBox.SpellSet = 4;
                        break;
                    case KeyBindAction.SpellUse01:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell01);
                        break;
                    case KeyBindAction.SpellUse02:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell02);
                        break;
                    case KeyBindAction.SpellUse03:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell03);
                        break;
                    case KeyBindAction.SpellUse04:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell04);
                        break;
                    case KeyBindAction.SpellUse05:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell05);
                        break;
                    case KeyBindAction.SpellUse06:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell06);
                        break;
                    case KeyBindAction.SpellUse07:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell07);
                        break;
                    case KeyBindAction.SpellUse08:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell08);
                        break;
                    case KeyBindAction.SpellUse09:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell09);
                        break;
                    case KeyBindAction.SpellUse10:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell10);
                        break;
                    case KeyBindAction.SpellUse11:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell11);
                        break;
                    case KeyBindAction.SpellUse12:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell12);
                        break;
                    case KeyBindAction.SpellUse13:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell13);
                        break;
                    case KeyBindAction.SpellUse14:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell14);
                        break;
                    case KeyBindAction.SpellUse15:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell15);
                        break;
                    case KeyBindAction.SpellUse16:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell16);
                        break;
                    case KeyBindAction.SpellUse17:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell17);
                        break;
                    case KeyBindAction.SpellUse18:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell18);
                        break;
                    case KeyBindAction.SpellUse19:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell19);
                        break;
                    case KeyBindAction.SpellUse20:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell20);
                        break;
                    case KeyBindAction.SpellUse21:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell21);
                        break;
                    case KeyBindAction.SpellUse22:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell22);
                        break;
                    case KeyBindAction.SpellUse23:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell23);
                        break;
                    case KeyBindAction.SpellUse24:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell24);
                        break;
                    /*case KeyBindAction.MenuBoxWindow:            //主菜单快捷
                        MenuBox.Visible = !MenuBox.Visible;
                        break;*/
                    case KeyBindAction.BigPatchBoxWindow:
                        if (Observer) continue;
                        if (null == BigPatchBox) break;
                        BigPatchBox.Visible = !BigPatchBox.Visible;
                        break;
                    case KeyBindAction.NPCDKeyWindow:
                        if (Observer) continue;
                        if (NPCBox.Visible == true)
                        {
                            NPCBox.Visible = false;
                            break;
                        }
                        if (NPCDKeyBox.Visible == true)
                        {
                            NPCDKeyBox.Visible = false;
                            break;
                        }
                        //发包 D键
                        CEnvir.Enqueue(new C.DKey { });
                        break;
                    //case KeyBindAction.CraftWindow:     //制作列表
                    //    if (Observer) continue;
                    //    CraftBox.Visible = !CraftBox.Visible;
                    //    break;
                    //case KeyBindAction.FixedPointWindow:   //记忆传送
                    //    if (!CEnvir.ClientControl.UseFixedPoint) return;  //记忆传送设置不显示就不设置快捷
                    //    if (Observer) continue;
                    //    FixedPointBox.Visible = !FixedPointBox.Visible;
                    //    break;
                    case KeyBindAction.BuffWindow:   //BUFF
                        if (Observer) continue;
                        BuffBox.Visible = !BuffBox.Visible;
                        break;
                    case KeyBindAction.TownReviveWindow:   //死亡复活
                        if (Observer) continue;
                        if (!User.Dead) continue;
                        //沙巴克复活时间增加15秒  地图是沙巴克地图  处于攻城时间
                        CastleInfo warCastle = Game.ConquestWars.FirstOrDefault(x => x.Map == Game.MapControl.MapInfo);
                        if (CEnvir.Now < MapObject.User.WarReviveTime.AddSeconds(15) && Game.MapControl.MapInfo.Index == 25 && warCastle != null)
                        {
                            foreach (CastleInfo castle in Game.ConquestWars)   //攻城时的颜色设置 遍历行会攻城站
                            {
                                if (castle.Map != Game.MapControl.MapInfo) continue;   //如果攻城站地图信息不对 继续

                                if (Game.CastleOwners.TryGetValue(castle, out string ownerGuild))
                                {
                                    if (ownerGuild == User.Title)
                                    {
                                        Game.ReceiveChat($"死亡复活冷却延迟 {(MapObject.User.WarReviveTime.AddSeconds(15) - CEnvir.Now).TotalSeconds.ToString("0")} 秒。", MessageType.System);
                                        return;
                                    }
                                    else
                                    {
                                        CEnvir.Enqueue(new C.TownRevive());
                                        return;
                                    }
                                }
                            }
                        }
                        CEnvir.Enqueue(new C.TownRevive());
                        break;
                    case KeyBindAction.AuctionsWindow:
                        if (Observer) continue;
                        AuctionsBox.Visible = !AuctionsBox.Visible;
                        break;
                    case KeyBindAction.BonusPoolWindow:
                        if (Observer) continue;
                        BonusPoolVersionBox.Visible = !BonusPoolVersionBox.Visible;
                        break;
                    case KeyBindAction.WarWeaponWindow:
                        if (Observer) continue;
                        if (Game.WarWeaponID != 0)
                            WarWeaponBox.Visible = !WarWeaponBox.Visible;
                        else
                            WarWeaponBox.Visible = false;
                        break;
                    case KeyBindAction.GroupFrameWindow:
                        if (Observer) continue;
                        //GroupMemberBox.Visible = !GroupMemberBox.Visible;
                        break;
                    default:
                        continue;
                }
                e.Handled = true;
            }
        }

        private void DoExitGame()
        {
            if (BigPatchConfig.ChkQuickSelect)
            {
                CEnvir.Enqueue(new C.Logout());
            }
            else
            {
                ExitBox.Visible = true;
                ExitBox.BringToFront();
            }
        }

        /// <summary>
        /// 物品信息创建道具标签
        /// </summary>
        public void CreateItemLabel()
        {
            //Type itemType;
            //Type weaponType;
            //MemberInfo[] infos;
            //DescriptionAttribute description;

            if (ItemLabel != null && !ItemLabel.IsDisposed) ItemLabel.Dispose();

            if (MouseItem?.Info == null /*|| MouseItem == SelectedCell?.Item*/) return;

            //if (SelectedCell != null && (SelectedCell.GridType == GridType.Inventory || SelectedCell.GridType == GridType.Storage)) return;

            ItemRefreshTime = CEnvir.Now.AddSeconds(1);  //道具刷新时间

            Stats stats = new Stats();
            Stats addedStats = new Stats();
            addedStats.Add(MouseItem.AddedStats);

            stats.Add(MouseItem.Info.Stats);
            stats.Add(MouseItem.AddedStats);

            ItemLabel = new DXControl
            {
                BackColour = Color.FromArgb(240, 15, 35, 50),   //道具背景颜色
                Border = true,
                BorderColour = Color.FromArgb(100, 90, 75),     //道具边框颜色
                DrawTexture = true,
                IsControl = false,
                IsVisible = true,
            };

            ItemInfo displayInfo = MouseItem.Info;

            if (MouseItem.Info.Effect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == MouseItem.AddedStats[Stat.ItemIndex]);

            #region 自定义前缀

            DXLabel triangle = new DXLabel
            {
                ForeColour = Color.Yellow,   //道具字体颜色
                Location = new Point(4, 5),
                Parent = ItemLabel,
                Text = "▼",
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
            };
            int itemNameOffset = triangle.DisplayArea.Right;
            if (!string.IsNullOrEmpty(MouseItem.CustomPrefixText))
            {
                DXLabel prefix = new DXLabel
                {
                    ForeColour = MouseItem.CustomPrefixColor == Color.Empty ? Color.Yellow : MouseItem.CustomPrefixColor,
                    Location = new Point(itemNameOffset - 2, 4),
                    Parent = ItemLabel,
                    Text = MouseItem.CustomPrefixText
                };
                itemNameOffset = prefix.DisplayArea.Right;
            }

            #endregion

            #region 道具名字
            DXLabel label = new DXLabel
            {
                ForeColour = Color.FromArgb(255, 255, 0),   //道具字体颜色
                Location = new Point(itemNameOffset, 4),
                Parent = ItemLabel,
                Text = displayInfo.Lang(p => p.ItemName)
            };

            //if (MouseItem.Info.ItemType == ItemType.Book && MouseItem.CurrentDurability >= 80)  //80及以上的书 金色字
            //    label.ForeColour = Color.Gold;

            if (MouseItem.Info.Effect == ItemEffect.ItemPart)
                label.Text += " - [" + "碎片".Lang() + "]";
            ItemLabel.Size = new Size(label.DisplayArea.Right + 4, label.DisplayArea.Bottom + 3);
            if (MouseItem.Refine)
                label.Text = "(*)" + label.Text;
            // 增加装备名字的颜色区分               测试用
            /*if (displayInfo.Rarity == Rarity.Elite)
            {
                label.ForeColour = Color.MediumPurple;
            }
            else if (displayInfo.Rarity == Rarity.Superior)
            {
                label.ForeColour = Color.MediumSpringGreen;
            }
            else
            {
                label.ForeColour = Color.White;
            }*/
            #endregion

            /*label = new DXLabel    //分割线          测试用
            {
                ForeColour = Color.FromArgb(100, 90, 75),
                BackColour = Color.FromArgb(100, 90, 75),
                Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                Parent = ItemLabel,
                AutoSize = false,
                Size = new Size(ItemLabel.Size.Width, 1)
            };*/

            bool needSpacer = false;
            //物品类型
            /*if (displayInfo.ItemType != ItemType.Nothing)
            {
                itemType = typeof(ItemType);
                infos = itemType.GetMember(displayInfo.ItemType.ToString());

                description = infos[0].GetCustomAttribute<DescriptionAttribute>();
                label = new DXLabel
                {
                    ForeColour = Color.White,   //类型 白色
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "物品类型:" + $"{description?.Description ?? displayInfo.ItemType.ToString()}",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                needSpacer = true;
            }

            //武器类型
            if (displayInfo.WeaponType != WeaponType.None)
            {
                weaponType = typeof(WeaponType);
                infos = weaponType.GetMember(displayInfo.WeaponType.ToString());

                description = infos[0].GetCustomAttribute<DescriptionAttribute>();
                label = new DXLabel
                {
                    ForeColour = Color.White,   //类型 白色
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "武器类型:" + $"{description?.Description ?? displayInfo.WeaponType.ToString()}",
                };
                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                needSpacer = true;
            }*/

            #region 持久 - 品质 - 成功率 -纯度
            if (MouseItem.Info.Durability > 0)
            {
                label = new DXLabel
                {
                    ForeColour = MouseItem.CurrentDurability == 0 ? Color.Red : Color.FromArgb(130, 255, 255),
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                };
                //如果物品类型不是消耗品 或者不是技能书 或者不是时装 才显示持久
                if (MouseItem.Info.ItemType != ItemType.Consumable && MouseItem.Info.ItemType != ItemType.Book && MouseItem.Info.ItemType != ItemType.Fashion && MouseItem.Info.ItemType != ItemType.Ore)
                {
                    label.Text = $"持久力".Lang() + $": {Math.Round(MouseItem.CurrentDurability / 1000M)}/{Math.Round(MouseItem.MaxDurability / 1000M)}";
                }

                //如果物品是武器并且是鱼竿 或 钓鱼的各类道具 或者是自动药剂 持久按正常值显示
                if (MouseItem.Info.ItemType == ItemType.Weapon && MouseItem.Info.Shape == 125 ||
                    MouseItem.Info.ItemType == ItemType.Weapon && MouseItem.Info.Shape == 126 ||
                    MouseItem.Info.ItemType == ItemType.Hook ||
                    MouseItem.Info.ItemType == ItemType.Float ||
                    MouseItem.Info.ItemType == ItemType.Bait ||
                    MouseItem.Info.ItemType == ItemType.Finder ||
                    MouseItem.Info.ItemType == ItemType.Reel ||
                    MouseItem.Info.ItemType == ItemType.Medicament)
                {
                    label.Text = $"持久力".Lang() + $": {Math.Round(MouseItem.CurrentDurability / 1M)}/{Math.Round(MouseItem.MaxDurability / 1M)}";
                }

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                needSpacer = true;
            }
            //重量
            if (MouseItem.Info.Weight > 0)
            {
                label = new DXLabel
                {
                    ForeColour = Color.White,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"重量".Lang() + $": {MouseItem.Info.Weight}",
                };

                switch (MouseItem.Info.ItemType)
                {
                    case ItemType.Weapon:
                    case ItemType.Shield:
                    case ItemType.Torch:
                        if (User.HandWeight - (Equipment[(int)EquipmentSlot.Weapon]?.Info?.Weight ?? 0) + (MouseItem.Info?.Weight ?? 0) > User.Stats[Stat.HandWeight])
                            label.ForeColour = Color.Red;
                        break;
                    case ItemType.Armour:
                    case ItemType.Fashion:
                    case ItemType.Helmet:
                    case ItemType.Necklace:
                    case ItemType.Bracelet:
                    case ItemType.Ring:
                    case ItemType.Shoes:
                    case ItemType.Medicament:
                    case ItemType.Poison:
                    case ItemType.Amulet:
                        if (User.WearWeight - (Equipment[(int)EquipmentSlot.Armour]?.Info?.Weight ?? 0) + (MouseItem.Info?.Weight ?? 0) > User.Stats[Stat.WearWeight])
                            label.ForeColour = Color.Red;
                        break;
                }

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                needSpacer = true;
            }

            if (needSpacer)
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            //数量
            if (MouseItem.Info.Effect == ItemEffect.Gold || MouseItem.Info.Effect == ItemEffect.Experience)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Yellow,
                    Location = new Point(ItemLabel.DisplayArea.Right, 4),
                    Parent = ItemLabel,
                    Text = $"数量".Lang() + $": {MouseItem.Count}"
                };
                ItemLabel.Size = new Size(label.DisplayArea.Right + 4, ItemLabel.Size.Height);
                return;
            }

            if (MouseItem.Info.Effect == ItemEffect.ItemPart)
            {
                label = new DXLabel
                {
                    ForeColour = Color.LightSeaGreen,
                    Location = new Point(ItemLabel.DisplayArea.Right, 4),
                    Parent = ItemLabel,
                    Text = $"碎片数量".Lang() + $": {MouseItem.Count}/{displayInfo.PartCount}",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4, ItemLabel.Size.Height);
            }
            else if (MouseItem.Info.StackSize > 1)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Yellow,
                    Location = new Point(ItemLabel.DisplayArea.Right, 4),
                    Parent = ItemLabel,
                    Text = $"数量".Lang() + $": {MouseItem.Count}/{MouseItem.Info.StackSize}"
                };
                ItemLabel.Size = new Size(label.DisplayArea.Right + 4, ItemLabel.Size.Height);
            }
            #endregion

            #region 属性
            switch (displayInfo.ItemType) //属性
            {
                case ItemType.Consumable:
                case ItemType.Scroll:
                    if (MouseItem.Info.Effect == ItemEffect.StatExtractor
                     || MouseItem.Info.Effect == ItemEffect.RefineExtractor
                     || MouseItem.Info.Effect == ItemEffect.GuildAllianceTreaty
                     || MouseItem.Info.Effect == ItemEffect.ArmourExtractor
                     || MouseItem.Info.Effect == ItemEffect.NecklaceExtractor
                     || MouseItem.Info.Effect == ItemEffect.BraceletRExtractor
                     || MouseItem.Info.Effect == ItemEffect.RingRExtractor
                     || MouseItem.Info.Effect == ItemEffect.ShoesExtractor
                     || MouseItem.Info.Effect == ItemEffect.HelmetExtractor
                     || MouseItem.Info.Effect == ItemEffect.StatExcellentExtractor)
                        EquipmentItemInfo();
                    else
                        CreatePotionLabel();
                    break;
                //case ItemType.Book:
                //    if (MouseItem.Info.Durability > 0 && MouseItem.Info.Shape > 0)  //技能书的持久大于0，并且技能书的Shape大于0  才显示成功率
                //    {
                //        label = new DXLabel
                //        {
                //            ForeColour = Color.White,
                //            Location = new Point(ItemLabel.DisplayArea.Right, 4),
                //            Parent = ItemLabel,
                //            Text = $"成功率".Lang() + $": {MouseItem.CurrentDurability}/{MouseItem.MaxDurability}",
                //        };

                //        ItemLabel.Size = new Size(label.DisplayArea.Right + 4, ItemLabel.Size.Height);
                //    }
                //    break;
                case ItemType.Meat:
                    if (MouseItem.Info.Durability > 0)
                    {
                        label = new DXLabel
                        {
                            ForeColour = MouseItem.CurrentDurability == 0 ? Color.Red : Color.White,
                            Location = new Point(ItemLabel.DisplayArea.Right, 4),
                            Parent = ItemLabel,
                            Text = $"品质".Lang() + $": {Math.Round(MouseItem.CurrentDurability / 1000M)}/{Math.Round(MouseItem.MaxDurability / 1000M)}",
                        };

                        ItemLabel.Size = new Size(label.DisplayArea.Right + 4, ItemLabel.Size.Height);
                    }
                    break;
                case ItemType.Ore:
                    if (MouseItem.Info.Durability > 0)
                    {
                        label = new DXLabel
                        {
                            ForeColour = MouseItem.CurrentDurability == 0 ? Color.Red : Color.White,
                            Location = new Point(ItemLabel.DisplayArea.Right, 4),
                            Parent = ItemLabel,
                            Text = $"纯度".Lang() + $": {Math.Round(MouseItem.CurrentDurability / 1000M)}",
                        };

                        ItemLabel.Size = new Size(label.DisplayArea.Right + 4, ItemLabel.Size.Height);
                    }
                    break;
                case ItemType.Gem:
                case ItemType.Orb:
                    EquipmentItemInfo("属性".Lang() + ": ");
                    break;

                default:
                    EquipmentItemInfo();
                    break;
            }

            #endregion

            #region 穿戴限制
            /*if (displayInfo.RequiredGender != RequiredGender.None)
            {
                Color colour = Color.White;
                switch (User.Gender)
                {
                    case MirGender.Male:
                        if (!displayInfo.RequiredGender.HasFlag(RequiredGender.Male))
                            colour = Color.Red;
                        break;
                    case MirGender.Female:
                        if (!displayInfo.RequiredGender.HasFlag(RequiredGender.Female))
                            colour = Color.Red;
                        break;
                }

                itemType = MouseItem.Info.RequiredGender.GetType();
                infos = itemType.GetMember(MouseItem.Info.RequiredGender.ToString());

                description = infos[0].GetCustomAttribute<DescriptionAttribute>();

                label = new DXLabel
                {
                    ForeColour = colour,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"要求性别: {description?.Description ?? MouseItem.Info.RequiredGender.ToString()}",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
            }*/


            if (displayInfo.RequiredClass != RequiredClass.All)
            {
                Color colour = Color.White;
                switch (User.Class)
                {
                    case MirClass.Warrior:
                        if (!MouseItem.Info.RequiredClass.HasFlag(RequiredClass.Warrior))
                            colour = Color.Red;
                        break;
                    case MirClass.Wizard:
                        if (!MouseItem.Info.RequiredClass.HasFlag(RequiredClass.Wizard))
                            colour = Color.Red;
                        break;
                    case MirClass.Taoist:
                        if (!MouseItem.Info.RequiredClass.HasFlag(RequiredClass.Taoist))
                            colour = Color.Red;
                        break;
                    case MirClass.Assassin:
                        if (!MouseItem.Info.RequiredClass.HasFlag(RequiredClass.Assassin))
                            colour = Color.Red;
                        break;
                }

                //itemType = displayInfo.RequiredClass.GetType();

                //infos = itemType.GetMember(displayInfo.RequiredClass.ToString());

                //description = infos[0].GetCustomAttribute<DescriptionAttribute>();

                label = new DXLabel
                {
                    ForeColour = colour,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"职业限制".Lang() + ":" + displayInfo.RequiredClass.Lang(),
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom + 3 > ItemLabel.Size.Height ? label.DisplayArea.Bottom + 3 : ItemLabel.Size.Height);
            }

            if (displayInfo.RequiredAmount > 0)
            {
                string text;
                Color colour = displayInfo.Rarity == Rarity.Common ? Color.White : Color.FromArgb(255, 165, 0);
                switch (displayInfo.RequiredType)
                {
                    case RequiredType.Level:
                        text = $"等级要求".Lang() + $": {MouseItem.Info.RequiredAmount}";
                        if (User.Level < MouseItem.Info.RequiredAmount && User.Stats[Stat.Rebirth] == 0)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxLevel:
                        text = $"最高等级".Lang() + $": {MouseItem.Info.RequiredAmount}";
                        if (User.Level > MouseItem.Info.RequiredAmount || User.Stats[Stat.Rebirth] > 0)
                            colour = Color.Red;
                        break;
                    case RequiredType.AC:
                        text = $"物理防御要求".Lang() + $": {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.MaxAC] < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MR:
                        text = $"魔法防御要求".Lang() + $": {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.MaxMR] < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.DC:
                        text = $"破坏要求".Lang() + $": {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.MaxDC] < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MC:
                        text = $"自然系魔法要求".Lang() + $": {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.MaxMC] < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.SC:
                        text = $"灵魂系魔法要求".Lang() + $": {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.MaxSC] < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.Health:
                        text = $"生命值要求".Lang() + $": {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.Health] < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.Mana:
                        text = $"魔法值要求".Lang() + $": {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.Mana] < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.CompanionLevel:
                        text = $"宠物等级".Lang() + $": {MouseItem.Info.RequiredAmount}";
                        if (Companion == null || Companion.Level < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxCompanionLevel:
                        text = $"宠物最大等级".Lang() + $": {MouseItem.Info.RequiredAmount}";
                        if (Companion == null || Companion.Level > MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.RebirthLevel:
                        text = $"转生等级".Lang() + $": {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.Rebirth] < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxRebirthLevel:
                        text = $"转生最大等级".Lang() + $": {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.Rebirth] > MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    default:
                        text = "未知类型需求".Lang();
                        break;
                }

                if (displayInfo.Rarity > Rarity.Common)
                    text += $" (" + displayInfo.Rarity.Lang() + $")";

                label = new DXLabel
                {
                    ForeColour = colour,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = text,
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom + 3 > ItemLabel.Size.Height ? label.DisplayArea.Bottom + 3 : ItemLabel.Size.Height);
            }
            else if (displayInfo.Rarity > Rarity.Common)
            {
                label = new DXLabel
                {
                    ForeColour = Color.FromArgb(150, 50, 200),
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = displayInfo.Rarity.Lang(),
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom + 3 > ItemLabel.Size.Height ? label.DisplayArea.Bottom + 3 : ItemLabel.Size.Height);
            }
            #endregion

            #region 其他信息
            bool spacer = false;
            long sale = MouseItem.Price(Math.Max(1, MouseItem.Count));
            /*if (sale > 0)
            {
                label = new DXLabel
                {
                    ForeColour = Color.LightGoldenrodYellow,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"售价: {sale}",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
            }
            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);*/

            if (MouseItem.Info.Durability > 0 && !MouseItem.Info.CanRepair && MouseItem.Info.StackSize == 1)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Red,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "无法修理".Lang(),
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }

            if (!MouseItem.Info.CanSell || (MouseItem.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Red,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "无法出售到商店".Lang(),
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }

            if (!MouseItem.Info.CanStore)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Red,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "仓库无法保存".Lang(),
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }

            if (!MouseItem.Info.CanTrade || (MouseItem.Flags & UserItemFlags.Bound) == UserItemFlags.Bound)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Red,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "私人物品/无法交易".Lang(),
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }

            if (!MouseItem.Info.CanDrop)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Red,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "无法丢弃".Lang(),
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }

            if (!MouseItem.Info.CanDeathDrop || (MouseItem.Flags & UserItemFlags.Bound) == UserItemFlags.Bound)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Red,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "死亡不会掉落".Lang(),
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }

            if ((MouseItem.Flags & UserItemFlags.Bound) == UserItemFlags.Bound)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Red,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "绑定物品".Lang(),
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }

            if ((MouseItem.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable)
            {
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                };

                switch (MouseItem.Info.ItemType)
                {
                    case ItemType.Book:
                        //label.ForeColour = Color.Yellow;
                        //label.Text = "不包含4级秘籍".Lang();
                        break;
                    default:
                        if (CEnvir.ClientControl.ShopNonRefinable)
                        {
                            label.ForeColour = Color.Red;
                            label.Text = "无法炼制或升级".Lang();
                        }
                        else
                        {
                            return;
                        }
                        break;
                }

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }
            //else if (MouseItem.Info.ItemType == ItemType.Book)
            //{
            //    label = new DXLabel
            //    {
            //        Location = new Point(4, ItemLabel.DisplayArea.Bottom),
            //        Parent = ItemLabel,
            //        ForeColour = Color.Green,
            //        Text = "包含高级秘籍".Lang(),
            //    };

            //    ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
            //        label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
            //    ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
            //    spacer = true;
            //}
            else if (MouseItem.Info.ItemType == ItemType.Orb || MouseItem.Info.ItemType == ItemType.Gem)
            {
                string info = "可以被镶嵌在".Lang() + ":" + Environment.NewLine;
                switch (MouseItem.Info.Shape)
                {
                    case 1:
                        info = info + "- " + "武器".Lang();
                        break;
                    case 2:
                        info = info + "- " + "衣服".Lang();
                        break;
                    case 3:
                        info = info + "- " + "头盔".Lang();
                        break;
                    case 4:
                        info = info + "- " + "项链".Lang();
                        break;
                    case 5:
                        info = info + "- " + "手镯".Lang();
                        break;
                    case 6:
                        info = info + "- " + "戒指".Lang();
                        break;
                    case 7:
                        info = info + "- " + "鞋子".Lang();
                        break;
                    case 8:
                        info = info + "- " + "盾牌".Lang();
                        break;
                    case 20:
                        info = info + "- " + "武器".Lang() + Environment.NewLine + "- " + "项链".Lang() + Environment.NewLine + "- " + "手镯".Lang() + Environment.NewLine + "- " + "戒指".Lang();
                        break;
                    case 21:
                        info = info + "- " + "衣服".Lang() + Environment.NewLine + "- " + "盾牌".Lang() + Environment.NewLine + "- " + "头盔".Lang() + Environment.NewLine + "- " + "鞋子".Lang();
                        break;
                    default:
                        info += "- " + "所有装备".Lang();
                        break;
                }
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    ForeColour = Color.White,
                    Text = info
                };
                ItemLabel.Size = new Size((label.DisplayArea.Right + 4 > ItemLabel.Size.Width) ? (label.DisplayArea.Right + 4) : ItemLabel.Size.Width, (label.DisplayArea.Bottom > ItemLabel.Size.Height) ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    ForeColour = Color.White,
                    Text = "按住Ctrl后左键点击装备进行镶嵌".Lang(),
                };
                ItemLabel.Size = new Size((label.DisplayArea.Right + 4 > ItemLabel.Size.Width) ? (label.DisplayArea.Right + 4) : ItemLabel.Size.Width, (label.DisplayArea.Bottom > ItemLabel.Size.Height) ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }
            #endregion

            #region 道具说明
            if (!string.IsNullOrEmpty(displayInfo.Description))   //道具说明
            {
                //string[] split = displayInfo.Lang(p => p.Description).Split(new char[] { '/' }, 10);   //增加道具说明文件换行 最多5行
                //string SS = "";

                label = new DXLabel
                {
                    ForeColour = Color.White,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    //Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                    Text = displayInfo.Lang(p => p.Description)
                };

                //for (int i = 0; i < split.Count(); i++)
                //{
                //    label.Text += (split.Count() > 0 && i < split.Count()) ? $"{split[i]}\n" : split[i];
                //}

                if (displayInfo.Effect == ItemEffect.FootBallWhistle)
                    label.ForeColour = Color.Red;

                if (displayInfo.Rarity == Rarity.Superior)
                    label.ForeColour = Color.FromArgb(138, 95, 15);

                if (displayInfo.Rarity == Rarity.Elite)
                    label.ForeColour = Color.FromArgb(108, 10, 157);

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }

            if (spacer)
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 3);
            #endregion

            #region 特殊修理
            if (MouseItem.Info.Durability > 0 && MouseItem.Info.CanRepair && MouseItem.Info.StackSize == 1 && MouseItem.Info.ItemType != ItemType.Book && !CEnvir.ClientControl.ItemCanRepairCheck)
            {
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                };

                if (CEnvir.Now >= MouseItem.NextSpecialRepair)
                {
                    label.Text = "可以特殊修理".Lang();
                    label.ForeColour = Color.LimeGreen;
                }
                else
                {
                    label.Text = $"下次特殊修理".Lang() + $" {(MouseItem.NextSpecialRepair - CEnvir.Now).Lang(true)}";
                    label.ForeColour = Color.Red;
                }

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
            }
            #endregion

            #region 装备幻化
            if (MouseItem.AddedStats[Stat.Illusion] > 0)
            {
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"幻化: {CEnvir.GetItemIllusionItemName(MouseItem)}" + "\n幻化时效: " + MouseItem.IllusionExpireDateTime.ToString(),
                    ForeColour = Color.Chocolate,
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
            }
            #endregion

            //有效期
            if ((MouseItem.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable)
            {
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"过期时间".Lang() + $": {MouseItem.ExpireTime.Lang(true)}",
                    ForeColour = Color.Chocolate,
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
            }

            if (stats[Stat.ItemReviveTime] > 0)
            {
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                };

                DateTime value = MouseItem.Info.Effect == ItemEffect.PillOfReincarnation ? ReincarnationPillTime : ItemReviveTime;

                if (CEnvir.Now >= value)
                {
                    label.Text = "复活准备就绪".Lang();
                    label.ForeColour = Color.LimeGreen;
                }
                else
                {
                    label.Text = $"复活等待冷却".Lang() + $" {(value - CEnvir.Now).Lang(true)}";
                    label.ForeColour = Color.Red;
                }

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
            }

            #region 宝石显示  未调整，不知道显示正不正确
            if (stats[Stat.EmptyGemSlot] + stats[Stat.UsedGemSlot] > 0)
                CreateGemInfo();
            #endregion

            #region 套装信息显示

            CreateSetItemInfo(CEnvir.Alt);
            #endregion

            #region 结婚戒指
            if ((MouseItem.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage)
            {
                label = new DXLabel
                {
                    ForeColour = Color.MediumOrchid,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "结婚戒指".Lang(),
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
            }
            #endregion

            #region 精炼信息
            if (NPCItemFragmentBox.IsVisible && MouseItem.CanFragment())
            {
                label = new DXLabel
                {
                    ForeColour = Color.MediumAquamarine,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"分解成本".Lang() + $": {MouseItem.FragmentCost():#,##0}",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                label = new DXLabel
                {
                    ForeColour = Color.MediumAquamarine,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"分解成本".Lang() + $": {(MouseItem.Info.Rarity == Rarity.Common ? "分解成本".Lang() : "分解成本".Lang() + " (II)")} x{MouseItem.FragmentCount():#,##0}",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
            }
            #endregion

            #region 重置时间
            if (CEnvir.Now < MouseItem.NextReset)
            {
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"可以重置使用".Lang() + $" {(MouseItem.NextReset - CEnvir.Now).Lang(true)}",
                    ForeColour = Color.Red,
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
            }

            if ((MouseItem.Flags & UserItemFlags.Locked) == UserItemFlags.Locked)
            {
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"GameScene.ItemLocked".Lang(),
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
            }
            #endregion

            #region 道具来源信息
            if (CEnvir.DisplayItemSource)
            {
                //判断是否GM制造
                string sourceName = MouseItem.SourceName;
                string owner = MouseItem.OriginalOwner;
                if (MouseItem.SourceName == "GM制造".Lang() && !CEnvir.DisplayGMItemSource)
                {
                    sourceName = "GM制造".Lang();
                    owner = GameScene.Game?.User.Name;
                }

                //物品来源
                label = new DXLabel
                {
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "物品来源".Lang() + "：" + sourceName,
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);


                //获得者
                label = new DXLabel
                {
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "获得者".Lang() + "：" + owner,
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                //来源地图
                string sourceMap = string.IsNullOrEmpty(MouseItem.SourceMap) ? "无" : MouseItem.SourceMap;
                label = new DXLabel
                {
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "来源地图".Lang() + "：" + sourceMap,
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                //来源时间
                label = new DXLabel
                {
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "时间".Lang() + "：" + MouseItem.CreationTime,
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            }

            #endregion

            #region 拍卖行信息

            if ((MouseItem.Flags & UserItemFlags.AutionItem) == UserItemFlags.AutionItem && MouseItem.LastPrice != 0)
            {
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"\n竞拍者:".Lang() + $"{MouseItem.LastName}" + $"\n当前价:" + $"{MouseItem.LastPrice / 100}",
                    ForeColour = Color.LimeGreen,
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
            }

            #endregion

            if (MouseItem.ExInfoOnly)
            {
                DXLabel dxlabel = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = MouseItem.ExInfo,
                    ForeColour = Color.Lime
                };
                ItemLabel.Size = new Size((dxlabel.DisplayArea.Right + 4 > ItemLabel.Size.Width) ? (dxlabel.DisplayArea.Right + 4) : ItemLabel.Size.Width, (dxlabel.DisplayArea.Bottom > ItemLabel.Size.Height) ? dxlabel.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
            }
        }

        /// <summary>
        /// 绘制道具属性
        /// </summary>
        /// <param name="prefix"></param>
        private void EquipmentItemInfo(string prefix = "")
        {
            Stats stats = new Stats();

            ItemInfo displayInfo = MouseItem.Info;

            if (MouseItem.Info.Effect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == MouseItem.AddedStats[Stat.ItemIndex]);

            stats.Add(displayInfo.Stats, displayInfo.ItemType != ItemType.Weapon);
            stats.Add(MouseItem.AddedStats, MouseItem.Info.ItemType != ItemType.Weapon);

            Stat weaponElementStat = Stat.None;
            if (displayInfo.ItemType == ItemType.Weapon)
            {
                weaponElementStat = MouseItem.AddedStats.GetWeaponElement();

                if (weaponElementStat == Stat.None)
                    weaponElementStat = displayInfo.Stats.GetWeaponElement();

                if (weaponElementStat != Stat.None)
                    stats[weaponElementStat] += MouseItem.AddedStats.GetWeaponElementValue() + displayInfo.Stats.GetWeaponElementValue();
            }

            DXLabel label;

            bool firstElement = stats.HasElementalWeakness();
            HashSet<Stat> statsDrawn = new HashSet<Stat>();

            if (MouseItem.FullItemStats != null)
            {
                foreach (FullItemStat fullItemStat in MouseItem.FullItemStats)
                {
                    if (fullItemStat.Stat == Stat.UsedGemSlot || fullItemStat.Stat == Stat.EmptyGemSlot)  //不绘制已镶嵌的个数
                    {
                        continue;
                    }

                    if (Globals.GemList.Contains(fullItemStat.StatSource))   //不绘制镶嵌带来的属性
                    {
                        continue;
                    }

                    if (statsDrawn.Contains(fullItemStat.Stat))   //画过的属性不再画
                    {
                        continue;
                    }

                    string text = stats.GetDisplay(fullItemStat.Stat);

                    //武器元素特殊处理
                    if (text == null && displayInfo.ItemType == ItemType.Weapon && fullItemStat.Stat == Stat.WeaponElement)
                    {
                        text = stats.GetDisplay(weaponElementStat);
                    }

                    ItemInfoStat itemstat = displayInfo.ItemStats.FirstOrDefault(x => x.Stat == fullItemStat.Stat && x.ShowHidden);
                    if (itemstat != null) continue;

                    if (text == null) continue;

                    string added = MouseItem.AddedStats.GetFormat(fullItemStat.Stat, true);

                    bool isGem = fullItemStat.Stat == Stat.GemOrbBrake || fullItemStat.Stat == Stat.GemOrbSuccess;
                    //淬炼
                    bool isOther = fullItemStat.StatSource == StatSource.Other;
                    if (isOther)
                    {
                        prefix = $"淬炼:\n";
                    }
                    label = new DXLabel
                    {
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                        Parent = ItemLabel,
                        Text = isGem ? (prefix + text) : text
                    };

                    //避免画两次武器元素
                    if (displayInfo.ItemType == ItemType.Weapon && fullItemStat.Stat == weaponElementStat)
                    {
                        statsDrawn.Add(Stat.WeaponElement);
                    }
                    statsDrawn.Add(fullItemStat.Stat);

                    //属性颜色显示
                    switch (fullItemStat.Stat)
                    {
                        case Stat.Health:
                        case Stat.Mana:
                            label.ForeColour = Color.FromArgb(148, 255, 206);
                            break;
                        case Stat.Luck:
                            label.ForeColour = Color.FromArgb(240, 240, 30);
                            break;
                        case Stat.Strength:
                            label.ForeColour = Color.FromArgb(148, 255, 206);
                            break;
                        case Stat.DropRate:
                        case Stat.ExperienceRate:
                        case Stat.SkillRate:
                        case Stat.GoldRate:
                        case Stat.MiningSuccessRate:
                            label.ForeColour = Color.FromArgb(240, 240, 30);
                            //if (added == null) break;
                            //label.Text += $" ({added})";
                            break;
                        case Stat.FireAttack:
                        case Stat.IceAttack:
                        case Stat.LightningAttack:
                        case Stat.WindAttack:
                        case Stat.HolyAttack:
                        case Stat.DarkAttack:
                        case Stat.PhantomAttack:
                        case Stat.WeaponElement:
                            label.ForeColour = Color.FromArgb(0, 255, 0);
                            break;
                        case Stat.FireResistance:
                        case Stat.IceResistance:
                        case Stat.LightningResistance:
                        case Stat.WindResistance:
                        case Stat.HolyResistance:
                        case Stat.DarkResistance:
                        case Stat.PhantomResistance:
                        case Stat.PhysicalResistance:
                            label.ForeColour = !firstElement ? Color.FromArgb(0, 255, 0) : Color.FromArgb(145, 255, 200);
                            firstElement = true;
                            break;
                        case Stat.Guild1:
                            label.Text += $"{MouseItem.Guild1Name}";
                            break;
                        case Stat.Guild2:
                            label.Text += $"{MouseItem.Guild2Name}";
                            break;
                        default:
                            if (MouseItem.AddedStats[fullItemStat.Stat] == 0) break;
                            label.Text += $"({added})";
                            label.ForeColour = Color.FromArgb(148, 255, 206);
                            break;
                    }
                    //如果淬炼属性覆盖原有颜色
                    if (isOther)
                    {
                        label.ForeColour = Color.FromArgb(30, 145, 255);
                    }
                    ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                        label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                }
            }
            else
            {
                foreach (KeyValuePair<Stat, int> pair in stats.Values)
                {
                    string text = stats.GetDisplay(pair.Key);

                    ItemInfoStat itemstat = displayInfo.ItemStats.FirstOrDefault(x => x.Stat == pair.Key && x.ShowHidden);
                    if (itemstat != null) continue;

                    if (text == null) continue;

                    string added = MouseItem.AddedStats.GetFormat(pair.Key);

                    label = new DXLabel
                    {
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                        Parent = ItemLabel,
                        Text = text
                    };

                    switch (pair.Key)
                    {
                        case Stat.Health:
                        case Stat.Mana:
                            label.ForeColour = Color.FromArgb(148, 255, 206);
                            break;
                        case Stat.Luck:
                            label.ForeColour = Color.FromArgb(240, 240, 30);
                            break;
                        case Stat.Strength:
                            label.ForeColour = Color.FromArgb(148, 255, 206);
                            break;
                        case Stat.DropRate:
                        case Stat.ExperienceRate:
                        case Stat.SkillRate:
                        case Stat.GoldRate:
                        case Stat.MiningSuccessRate:
                            label.ForeColour = Color.FromArgb(240, 240, 30);
                            //if (added == null) break;
                            //label.Text += $" ({added})";
                            break;
                        case Stat.FireAttack:
                        case Stat.IceAttack:
                        case Stat.LightningAttack:
                        case Stat.WindAttack:
                        case Stat.HolyAttack:
                        case Stat.DarkAttack:
                        case Stat.PhantomAttack:
                            label.ForeColour = Color.FromArgb(0, 255, 0);
                            break;
                        case Stat.FireResistance:
                        case Stat.IceResistance:
                        case Stat.LightningResistance:
                        case Stat.WindResistance:
                        case Stat.HolyResistance:
                        case Stat.DarkResistance:
                        case Stat.PhantomResistance:
                        case Stat.PhysicalResistance:
                            label.ForeColour = !firstElement ? Color.FromArgb(0, 255, 0) : Color.FromArgb(145, 255, 200);
                            firstElement = true;
                            break;
                        case Stat.Guild1:
                            label.Text += $"{MouseItem.Guild1Name}";
                            break;
                        case Stat.Guild2:
                            label.Text += $"{MouseItem.Guild2Name}";
                            break;
                        default:
                            if (MouseItem.AddedStats[pair.Key] == 0) break;
                            label.Text += $"   ({added})";
                            label.ForeColour = Color.FromArgb(148, 255, 206);
                            break;
                    }

                    ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                        label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                }
            }

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 3);

            //Type itemType = displayInfo.ItemType.GetType(); ;
            //MemberInfo[] infos = itemType.GetMember(displayInfo.ItemType.ToString());

            //DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();
            switch (displayInfo.ItemType)
            {
                case ItemType.Weapon:
                    if ((MouseItem.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) break;  //如果是不能精炼的装备
                    if (CEnvir.ClientControl.NewWeaponUpgradeCheck) break;   //如果开启新版武器升级

                    label = new DXLabel
                    {
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                        Parent = ItemLabel,
                        //Text = $"{displayInfo.ItemType.Lang()}" + "等级".Lang() + ": " + (MouseItem.Level < Globals.GameWeaponEXPInfoList.Count ? MouseItem.Level.ToString() : "Max")
                    };

                    if (MouseItem.Info.Index == 80360 || MouseItem.Info.Index == 80361 || MouseItem.Info.Index == 80362)
                        label.Text = $"{displayInfo.ItemType.Lang()}" + "等级".Lang() + ": " + (MouseItem.Level < 13 ? MouseItem.Level.ToString() : "Max");
                    else
                        label.Text = $"{displayInfo.ItemType.Lang()}" + "等级".Lang() + ": " + (MouseItem.Level < Globals.GameWeaponEXPInfoList.Count ? MouseItem.Level.ToString() : "Max");

                    ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                        label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                    if (MouseItem.Level < 13 || (!(MouseItem.Info.Index == 80360 || MouseItem.Info.Index == 80361 || MouseItem.Info.Index == 80362) && MouseItem.Level < Globals.GameWeaponEXPInfoList.Count))   //武器修炼值
                    {
                        label = new DXLabel
                        {
                            Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                            Parent = ItemLabel,
                        };

                        if ((MouseItem.Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable)
                        {
                            label.Text = "可修炼".Lang();
                            label.ForeColour = Color.LightGreen;
                        }
                        else
                        {
                            label.Text = $"{displayInfo.ItemType.Lang()}" + "修炼值".Lang() + $": {MouseItem.Experience / Globals.GameWeaponEXPInfoList[MouseItem.Level].Exp:0.##%}";
                            label.ForeColour = Color.White;
                        }

                        ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                            label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                    }
                    ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 3);
                    break;
                    //case ItemType.Necklace:
                    //case ItemType.Bracelet:
                    //case ItemType.Ring:
                    //    if ((MouseItem.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) break;

                    //if (MouseItem.Level < Globals.GameAccessoryEXPInfoList.Count && !CEnvir.ClientControl.JewelryLvShowsCheck || MouseItem.Level > 1)   //道具修炼值
                    //{
                    //    label = new DXLabel           //首饰等级
                    //    {
                    //        ForeColour = Color.White,
                    //        Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    //        Parent = ItemLabel,
                    //        Text = $"{displayInfo.ItemType.Lang()}" + "等级".Lang() + ": " + (MouseItem.Level < Globals.GameAccessoryEXPInfoList.Count ? MouseItem.Level.ToString() : "Max")
                    //    };

                    //    ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    //        label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                    //}

                    //if (MouseItem.Level < Globals.GameAccessoryEXPInfoList.Count && !CEnvir.ClientControl.JewelryExpShowsCheck)   //道具修炼值
                    //{
                    //    label = new DXLabel
                    //    {
                    //        Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    //        Parent = ItemLabel,
                    //    };

                    //    if ((MouseItem.Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable)
                    //    {
                    //        label.Text = "可修炼".Lang();
                    //        label.ForeColour = Color.LightGreen;
                    //    }
                    //    else
                    //    {
                    //        label.Text = $"{displayInfo.ItemType.Lang()}" + "修炼值".Lang() + $": {MouseItem.Experience / Globals.GameAccessoryEXPInfoList[MouseItem.Level].Exp:0.##%}";
                    //        label.ForeColour = Color.White;
                    //    }

                    //    ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    //        label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                    //}
                    //ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 3);
                    //break;
            }
        }
        /// <summary>
        /// 创建药水标签
        /// </summary>
        private void CreatePotionLabel()
        {
            if (MouseItem == null) return;

            Stats stats = new Stats();

            stats.Add(MouseItem.Info.Stats);

            DXLabel label;
            foreach (KeyValuePair<Stat, int> pair in stats.Values)
            {
                string text = stats.GetDisplay(pair.Key);

                if (text == null) continue;

                label = new DXLabel
                {
                    ForeColour = Color.White,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = text
                };

                switch (pair.Key)
                {
                    case Stat.Luck:
                    case Stat.DropRate:
                    case Stat.ExperienceRate:
                    case Stat.SkillRate:
                    case Stat.GoldRate:
                        label.ForeColour = Color.Yellow;
                        break;
                    case Stat.DeathDrops:
                        label.ForeColour = Color.OrangeRed;
                        break;
                }

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
            }
            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            if (MouseItem.Info.Durability > 0)
            {
                label = new DXLabel
                {
                    ForeColour = Color.White,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"GameScene.ItemCooling".Lang((MouseItem.Info.Durability / 1000M).ToString("#,##0.#"))
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
            }
        }
        /// <summary>
        /// 创建魔法技能标签
        /// </summary>
        private void CreateMagicLabel()
        {
            if (MouseMagic == null) return;

            MagicLabel = new DXControl
            {
                BackColour = Color.FromArgb(150, 175, 160, 100),       //(255, 0, 24, 48),
                Border = false,
                DrawTexture = true,
                IsControl = false,
                IsVisible = true,
            };

            DXLabel label = new DXLabel
            {
                ForeColour = Color.FromArgb(240, 210, 170),
                Location = new Point(2, 12),
                Parent = MagicLabel,
                Font = new Font(Config.FontName, CEnvir.FontSize(9.5F), FontStyle.Bold),
                Text = $"［{MouseMagic.Lang(p => p.Name)}］"
            };
            MagicLabel.Size = new Size(label.DisplayArea.Right + 4, label.DisplayArea.Bottom + 4);

            ClientUserMagic magic;

            string classtext = null;
            switch (MouseMagic.Class)
            {
                case MirClass.Warrior:
                    classtext = $"无";
                    break;
                case MirClass.Wizard:
                    classtext = $"自然系";
                    break;
                case MirClass.Taoist:
                    classtext = $"灵魂系";
                    break;
                case MirClass.Assassin:
                    classtext = $"无";
                    break;
            }

            label = new DXLabel
            {
                ForeColour = Color.White,
                Location = new Point(4, MagicLabel.DisplayArea.Bottom + 8),
                Parent = MagicLabel,
                Outline = false,
                Text = $"属性".Lang() + $" : " + classtext,
            };
            MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom);

            Type type = MouseMagic.School.GetType();
            MemberInfo[] infos = type.GetMember(MouseMagic.School.ToString());

            DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

            label = new DXLabel
            {
                ForeColour = Color.White,
                Location = new Point(4, MagicLabel.DisplayArea.Bottom + 4),
                Parent = MagicLabel,
                Outline = false,
                Text = $"元素".Lang() + $" : {description?.Description ?? MouseMagic.School.ToString()}",
            };
            MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom);

            label = new DXLabel
            {
                ForeColour = User.Level < MouseMagic.NeedLevel1 ? Color.Red : Color.White,
                Location = new Point(4, MagicLabel.DisplayArea.Bottom + 4),
                Parent = MagicLabel,
                Outline = false,
                Text = $"1等级可修炼级别".Lang() + $" : {MouseMagic.NeedLevel1}",
            };
            MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom);
            //width = label.DisplayArea.Right + 10;

            label = new DXLabel
            {
                ForeColour = Color.White,
                Location = new Point(4, MagicLabel.DisplayArea.Bottom + 4),
                Parent = MagicLabel,
                Outline = false,
                Text = $"-" + "所需修炼值".Lang() + $" : {MouseMagic.Experience1:###0}",
            };
            MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom);

            new DXLabel
            {
                ForeColour = User.Level < MouseMagic.NeedLevel2 ? Color.Red : Color.White,
                Location = new Point(4, MagicLabel.DisplayArea.Bottom + 4),
                Parent = MagicLabel,
                Outline = false,
                Text = $"2等级可修炼级别".Lang() + $" : {MouseMagic.NeedLevel2}",
            };

            MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom);

            label = new DXLabel
            {
                ForeColour = Color.White,
                Location = new Point(4, MagicLabel.DisplayArea.Bottom + 20),
                Parent = MagicLabel,
                Outline = false,
                Text = $"-" + "所需修炼值".Lang() + $" : {MouseMagic.Experience2:###0}",
            };
            MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom);

            new DXLabel
            {
                ForeColour = User.Level < MouseMagic.NeedLevel3 ? Color.Red : Color.White,
                Location = new Point(4, MagicLabel.DisplayArea.Bottom + 4),
                Parent = MagicLabel,
                Outline = false,
                Text = $"3等级可修炼级别".Lang() + $" : {MouseMagic.NeedLevel3}",
            };
            MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom);

            label = new DXLabel
            {
                ForeColour = Color.White,
                Location = new Point(4, MagicLabel.DisplayArea.Bottom + 20),
                Parent = MagicLabel,
                Outline = false,
                Text = $"-" + "所需修炼值".Lang() + $" : {MouseMagic.Experience3:###0}",
            };
            MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom);

            //label = new DXLabel
            //{
            //    ForeColour = magic?.Level < 3 ? Color.Red : Color.White,
            //    Location = new Point(4, MagicLabel.DisplayArea.Bottom),
            //    Parent = MagicLabel,
            //    Text = $"4级技能需要技能书".Lang(),
            //};
            //MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom);

            label = new DXLabel
            {
                AutoSize = false,
                ForeColour = Color.White,
                Location = new Point(4, MagicLabel.DisplayArea.Bottom + 4),
                Parent = MagicLabel,
                Outline = false,
                Text = "说明".Lang() + " : " + MouseMagic.Lang(p => p.Description),
            };
            label.Size = DXLabel.GetHeight(label, MagicLabel.Size.Width);
            MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom + 4);

            if (User.Magics.TryGetValue(MouseMagic, out magic))
            {
                string text = null;
                switch (magic.Level)
                {
                    case 0:
                        text = $"{magic.Experience}/{magic.Info.Experience1}";
                        break;
                    case 1:
                        text = $"{magic.Experience}/{magic.Info.Experience2}";
                        break;
                    case 2:
                        text = $"{magic.Experience}/{magic.Info.Experience3}";
                        break;
                    default:
                        text = $"Level : Max";
                        break;
                }

                label = new DXLabel
                {
                    ForeColour = Color.Red,
                    Location = new Point(4, MagicLabel.DisplayArea.Bottom + 4),
                    Parent = MagicLabel,
                    Outline = false,
                    Text = $"{text}",
                };
            }
            MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom + 12);
        }

        /// <summary>
        /// 套装属性显示标签
        /// </summary>
        private void CreateSetItemInfo(bool showParts = false)
        {
            int level;
            MirClass userClass;
            List<ItemInfo> itemsWearing;

            DXItemCell cell = MouseControl as DXItemCell;
            if (cell?.GridType == GridType.Inspect)
            {
                level = InspectBox.Level;
                userClass = InspectBox.Class;
                itemsWearing = InspectBox.Equipment.Where(x => x != null && x.CurrentDurability > 0).Select(y => y.Info).ToList();
            }
            else
            {
                level = User.Level;
                userClass = User.Class;
                itemsWearing = Equipment.Where(x => x != null && x.CurrentDurability > 0).Select(y => y.Info).ToList();
            }

            var details =
                Functions.GetAllItemSetDetails(MouseItem.Info, itemsWearing, false);

            foreach (var setDetail in details)
            {
                string setName = setDetail.Key;
                var activeGroups = setDetail.Value["active"];
                var inactiveGroups = setDetail.Value["inactive"];

                #region 套装名称

                DXLabel label = new DXLabel
                {
                    ForeColour = Color.LimeGreen,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"套装名称".Lang(),
                };

                //下面预留2格空位
                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom + 2);

                label = new DXLabel
                {
                    ForeColour = Color.LimeGreen,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"    {setName}"
                };

                //下面预留2格空位
                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom + 2);

                #endregion

                #region 套装搭配

                //画套装搭配
                label = new DXLabel
                {
                    ForeColour = Color.LimeGreen,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "套装搭配 - 按住Alt键查看详情".Lang(),
                };
                //下面预留2格空位
                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom + 2);

                #region 已激活的套装

                foreach (var activeGroup in activeGroups)
                {
                    label = new DXLabel
                    {
                        ForeColour = Color.LimeGreen,
                        Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                        Parent = ItemLabel,
                        Text = $"    {activeGroup.GroupName}"
                    };

                    if (showParts)
                    {
                        int requiredNum = activeGroup.RequiredNumItems;
                        int candidatesNum = activeGroup.SetGroupItems.Count;
                        string condition = requiredNum < candidatesNum ? $": (以下任意{requiredNum}件生效)" : $": (以下全部{requiredNum}件生效)";

                        switch (activeGroup.SetRequirement)
                        {
                            case ItemSetRequirementType.WithoutReplacement:
                                condition += " 不可重复触发";
                                break;
                            case ItemSetRequirementType.WithReplacement:
                                condition += " 可以重复触发";
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        // 显示触发条件
                        label.Text += condition;

                        //下面预留2格空位
                        ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                            label.DisplayArea.Bottom + 2);

                        foreach (var requiredItem in activeGroup.GetSetGroupItemInfoList())
                        {
                            label = new DXLabel
                            {
                                ForeColour = Color.LimeGreen,
                                Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                                Parent = ItemLabel,
                                Text = $"    {requiredItem.ItemName}"
                            };
                            //下面预留2格空位
                            ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                                label.DisplayArea.Bottom + 2);
                        }
                    }
                    else
                    {
                        //显示穿戴的件数
                        label.Text +=
                            $"({activeGroup.GetRequiredNumItem()}/{activeGroup.GetRequiredNumItem()}): ";

                        // 画属性
                        var groupStats = activeGroup.GetSetGroupStats(level, userClass);
                        foreach (KeyValuePair<Stat, int> pair in groupStats.Values)
                        {
                            string text = groupStats.GetDisplay(pair.Key);

                            if (text == null) continue;
                            label.Text += "\n    " + text;
                        }
                    }

                    //下面预留2格空位
                    ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                        label.DisplayArea.Bottom + 2);
                }

                #endregion

                #region 未激活的套装

                foreach (var inactiveGroup in inactiveGroups)
                {
                    label = new DXLabel
                    {
                        ForeColour = Color.Gray,
                        Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                        Parent = ItemLabel,
                        Text = $"    {inactiveGroup.GroupName}"
                    };

                    if (showParts)
                    {
                        int requiredNum = inactiveGroup.RequiredNumItems;
                        int candidatesNum = inactiveGroup.SetGroupItems.Count;
                        string condition = requiredNum < candidatesNum ? $": (以下任意{requiredNum}件生效)" : $": (以下全部{requiredNum}件生效)";

                        switch (inactiveGroup.SetRequirement)
                        {
                            case ItemSetRequirementType.WithoutReplacement:
                                condition += " 不可重复触发";
                                break;
                            case ItemSetRequirementType.WithReplacement:
                                condition += " 可以重复触发";
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        // 显示触发条件
                        label.Text += condition;

                        //下面预留2格空位
                        ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                            label.DisplayArea.Bottom + 2);

                        foreach (var requiredItem in inactiveGroup.GetSetGroupItemInfoList())
                        {
                            label = new DXLabel
                            {
                                ForeColour = Color.Gray,
                                Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                                Parent = ItemLabel,
                                Text = $"    {requiredItem.ItemName}"
                            };
                            //下面预留2格空位
                            ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                                label.DisplayArea.Bottom + 2);
                        }
                    }
                    else
                    {
                        //显示穿戴的件数
                        label.Text +=
                            $"({inactiveGroup.GetNumItemsWearing(itemsWearing)}/{inactiveGroup.GetRequiredNumItem()}): ";

                        // 画属性
                        var groupStats = inactiveGroup.GetSetGroupStats(level, userClass);
                        foreach (KeyValuePair<Stat, int> pair in groupStats.Values)
                        {
                            string text = groupStats.GetDisplay(pair.Key);

                            if (text == null) continue;
                            label.Text += "\n    " + text;
                        }
                    }

                    //下面预留2格空位
                    ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                        label.DisplayArea.Bottom + 4);
                }

                #endregion

                //label = new DXLabel
                //{
                //    ForeColour = Color.LimeGreen,
                //    Location = new Point(LeftOffset, ItemLabel.DisplayArea.Bottom),
                //    Parent = ItemLabel,
                //    Text = $"套装描述".Lang()
                //};

                ////下面预留2格空位
                //ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                //    label.DisplayArea.Bottom + 2);

                if (!string.IsNullOrEmpty(Globals.SetInfoList.Binding.FirstOrDefault(x => x.SetName == setName)?.SetDescription))
                {
                    label = new DXLabel
                    {
                        ForeColour = Color.LimeGreen,
                        Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                        Parent = ItemLabel,
                        Text = $"    {Globals.SetInfoList.Binding.FirstOrDefault(x => x.SetName == setName)?.SetDescription}"
                    };

                    //下面预留4格空位
                    ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                        label.DisplayArea.Bottom + 4);
                }

                #endregion

            }
        }

        /// <summary>
        /// 镶嵌属性显示标签
        /// </summary>
        private void CreateGemInfo()
        {
            if (MouseItem.FullItemStats == null)
                return;

            Dictionary<StatSource, List<FullItemStat>> gemList = new Dictionary<StatSource, List<FullItemStat>>();

            foreach (FullItemStat gemStat in MouseItem.FullItemStats)
            {
                if (!Globals.GemList.Contains(gemStat.StatSource))
                    continue;
                if (!gemList.ContainsKey(gemStat.StatSource))
                {
                    gemList.Add(gemStat.StatSource, new List<FullItemStat>());
                }
                gemList[gemStat.StatSource].Add(gemStat);
            }

            foreach (KeyValuePair<StatSource, List<FullItemStat>> kvp in gemList)
            {
                if (kvp.Value.Count < 1)
                    continue;

                Stats gemStats = new Stats();

                ItemInfo gemInfo = Globals.ItemInfoList.Binding.First(x => x.ItemName == kvp.Value[0]?.SourceName);

                DXImageControl gemImage = new DXImageControl
                {
                    LibraryFile = LibraryFile.Ground,
                    Index = gemInfo.Image,
                    Parent = ItemLabel,
                    IsControl = false,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom + 4),
                };
                int xOffset = 16 - gemImage.Size.Width;
                int yOffset = 12 - gemImage.Size.Height;
                DXLabel gemText = new DXLabel
                {
                    ForeColour = Color.LimeGreen,
                    Location = new Point(gemImage.DisplayArea.Right + xOffset, gemImage.DisplayArea.Top),
                    Parent = ItemLabel,
                    Text = ""
                };

                foreach (FullItemStat gemStat in kvp.Value)
                {
                    gemStats[gemStat.Stat] += gemStat.Amount;
                }
                foreach (FullItemStat gemStat in kvp.Value)
                {
                    gemText.Text += $"{gemStats.GetDisplay(gemStat.Stat)} ";
                }
                ItemLabel.Size = new Size(gemText.DisplayArea.Right + 4 > ItemLabel.Size.Width ? gemText.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                gemText.DisplayArea.Bottom > ItemLabel.Size.Height ? gemText.DisplayArea.Bottom : ItemLabel.Size.Height);
            }

            //画空位
            foreach (FullItemStat fullItemStat in MouseItem.FullItemStats)
            {
                if (fullItemStat.Stat == Stat.EmptyGemSlot)
                {
                    for (int i = 0; i < fullItemStat.Amount; i++)
                    {
                        DXLabel slotText = new DXLabel
                        {
                            ForeColour = Color.LightGray,
                            Location = new Point(4, ItemLabel.DisplayArea.Bottom + 4),
                            Parent = ItemLabel,
                            Text = "空闲的附魔石孔位".Lang()
                        };
                        ItemLabel.Size = new Size(slotText.DisplayArea.Right + 4 > ItemLabel.Size.Width ? slotText.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    slotText.DisplayArea.Bottom + 4 > ItemLabel.Size.Height ? slotText.DisplayArea.Bottom : ItemLabel.Size.Height + 4);
                    }
                }
            }
            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
        }

        /// <summary>
        /// 魔法技能键位
        /// </summary>
        /// <param name="key"></param>
        public void UseMagic(SpellKey key)
        {
            //放魔法时自动下马
            if (BigPatchConfig.ChkDismountToFireMagic)
                if (User.Horse != HorseType.None)
                {
                    if (CEnvir.Now < User.NextActionTime || User.ActionQueue.Count > 0) return;
                    if (CEnvir.Now < User.ServerTime) return;

                    User.ServerTime = CEnvir.Now.AddSeconds(5);
                    CEnvir.Enqueue(new C.Mount());
                }

            if (Game.Observer || User == null || User.Horse != HorseType.None || MagicBarBox == null || key == SpellKey.None) return;

            ClientUserMagic magic = null;

            foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in User.Magics)
            {
                switch (MagicBarBox.SpellSet)
                {
                    case 1:
                        if (pair.Value.Set1Key == key)
                            magic = pair.Value;
                        break;
                    case 2:
                        if (pair.Value.Set2Key == key)
                            magic = pair.Value;
                        break;
                    case 3:
                        if (pair.Value.Set3Key == key)
                            magic = pair.Value;
                        break;
                    case 4:
                        if (pair.Value.Set4Key == key)
                            magic = pair.Value;
                        break;
                }

                if (magic != null) break;
            }

            //持续施法，切换魔法
            if (magic != null && CanContinuouslyMagic(magic.Info.Magic))
                ContinuouslyMagicType = magic.Info.Magic;

            UseMagic(magic);
        }

        /// <summary>
        /// 可以攻击的目标
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        private bool CanAttackTarget(MapObject ob)
        {
            if (ob == null || ob.Dead) return false;

            switch (ob.Race)
            {
                case ObjectType.Player:
                    if (ob == Game._User) return false;
                    return true;
                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)ob;

                    if (mob.MonsterInfo.AI < 0) return false;

                    return true;

                default:
                    return false;
            }
        }
        /// <summary>
        /// 可以持续释放的魔法
        /// </summary>
        public bool CanContinuouslyMagic(MagicType type)
        {
            switch (type)
            {
                //需要持续魔法的列表
                case MagicType.FireBall:
                case MagicType.LightningBall:
                case MagicType.IceBolt:
                case MagicType.GustBlast:
                case MagicType.AdamantineFireBall:
                case MagicType.ThunderBolt:
                case MagicType.IceBlades:
                case MagicType.Cyclone:
                case MagicType.FireStorm:
                case MagicType.LightningWave:
                case MagicType.IceStorm:
                case MagicType.DragonTornado:
                case MagicType.ChainLightning:
                case MagicType.MeteorShower:

                case MagicType.EvilSlayer:
                case MagicType.GreaterEvilSlayer:
                case MagicType.ExplosiveTalisman:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 绘制后打开
        /// </summary>
        protected override void OnAfterDraw()
        {
            base.OnAfterDraw();

            int image = -1;
            Color color = Color.Empty;

            /*if (SelectedCell?.Item != null)
            {
                ItemInfo info = SelectedCell.Item.Info;

                if (info.Effect == ItemEffect.ItemPart)
                    info = Globals.ItemInfoList.Binding.First(x => x.Index == SelectedCell.Item.AddedStats[Stat.ItemIndex]);

                image = info.Image;
                color = SelectedCell.Item.Colour;
            }
            else*/
            if (GoldPickedUp)  //捡取金币
                image = 124;

            MirLibrary library;

            if (image >= 0 && CEnvir.LibraryList.TryGetValue(LibraryFile.Inventory, out library))
            {
                Size imageSize = library.GetSize(image);
                Point p = new Point(CEnvir.MouseLocation.X - imageSize.Width / 2, CEnvir.MouseLocation.Y - imageSize.Height / 2);

                if (p.X + imageSize.Width >= Size.Width + Location.X)
                    p.X = Size.Width - imageSize.Width + Location.X;

                if (p.Y + imageSize.Height >= Size.Height + Location.Y)
                    p.Y = Size.Height - imageSize.Height + Location.Y;

                if (p.X < Location.X)
                    p.X = Location.X;

                if (p.Y <= Location.Y)
                    p.Y = Location.Y;

                library.Draw(image, p.X, p.Y, Color.White, false, 1f, ImageType.Image, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

                if (color != Color.Empty)
                    library.Draw(image, p.X, p.Y, color, false, 1f, ImageType.Overlay, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
            }

            if (ItemLabel != null && !ItemLabel.IsDisposed)
                ItemLabel.Draw();

            if (MagicLabel != null && !MagicLabel.IsDisposed)
                MagicLabel.Draw();
        }

#if Mobile
#else
        public void UpdateGuildFlag(int index, Color color)
        {
            //if (index == -1) return;

            if (index >= 0)
            {
                GuildFlag.Visible = true;
                GuildFlag.Index = 1690 + index;
                GuildFlag.AfterDraw += (o, e) =>
                {
                    MirLibrary library;
                    if (!CEnvir.LibraryList.TryGetValue(LibraryFile.UI1, out library)) return;
                    library.Draw(1690 + index, GuildFlag.Location.X, GuildFlag.Location.Y, color, false, 1f, ImageType.Overlay);
                };
            }
            else
            {
                GuildFlag.Visible = false;
            }
        }
#endif

        /// <summary>
        /// 位移 方向 位置
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="location"></param>
        public void Displacement(MirDirection direction, Point location)
        {
            //if (MapObject.User.Direction == direction && MapObject.User.CurrentLocation == location) return;

            MapObject.User.ServerTime = DateTime.MinValue;
            MapObject.User.SetAction(new ObjectAction(MirAction.Standing, direction, location));
            MapObject.User.NextActionTime = CEnvir.Now.AddMilliseconds(300);
        }

        /// <summary>
        /// 填充道具
        /// </summary>
        /// <param name="items"></param>
        public void FillItems(List<ClientUserItem> items)
        {
            foreach (ClientUserItem item in items)
            {
                if (item.Slot >= Globals.FishingOffSet)
                {
                    //写入 钓鱼装备的box
                    int idx = item.Slot - Globals.FishingOffSet;
                    if (FishingBox.FishingGrid.Length <= idx) continue;

                    FishingBox.FishingGrid[idx].Item = item;
                    continue;
                }

                if (item.Slot >= Globals.PatchOffSet)
                {
                    //写入 碎片的box
                    int idx = item.Slot - Globals.PatchOffSet;
                    if (InventoryBox.PatchGrid.Grid.Length <= idx) continue;

                    InventoryBox.PatchGrid.Grid[idx].Item = item;
                    continue;
                }

                if (item.Slot >= Globals.EquipmentOffSet)
                {
                    int idx = item.Slot - Globals.EquipmentOffSet;
                    if (CharacterBox.Grid.Length <= idx) continue;

                    CharacterBox.Grid[idx].Item = item;
                    continue;
                }

                if (InventoryBox.Grid.Grid.Length <= item.Slot) continue;

                InventoryBox.Grid.Grid[item.Slot].Item = item;
            }
        }

        /// <summary>
        /// 刷新道具
        /// </summary>
        /// <param name="items"></param>
        /// <param name="type"></param>
        public void RefreshItems(List<ClientUserItem> items, GridType type)
        {
            foreach (ClientUserItem item in items)
            {
                if (item?.Slot >= Globals.PatchOffSet)
                {
                    InventoryBox.PatchGrid.Grid[item.Slot - Globals.PatchOffSet].Item = item;
                    continue;
                }
                InventoryBox.Grid.Grid[item.Slot].Item = item;
            }
            if (type == GridType.PatchGrid)
            {
                for (var i = items.Count; i < InventoryBox.PatchGrid.Grid.Length; i++)
                {
                    InventoryBox.PatchGrid.Grid[i].Item = null;
                }
            }
            else
            {
                for (var i = items.Count; i < InventoryBox.Grid.Grid.Length; i++)
                {
                    InventoryBox.Grid.Grid[i].Item = null;
                }
            }

            InventoryBox.RefreshGrid.Enabled = true;
        }

        /// <summary>
        /// 增加道具
        /// </summary>
        /// <param name="items"></param>
        public void AddItems(List<ClientUserItem> items)
        {
            foreach (ClientUserItem item in items)
            {
                if (item.Info.Effect == ItemEffect.Experience) continue;
                if ((item.Flags & UserItemFlags.QuestItem) == UserItemFlags.QuestItem) continue;

                item.Info.PlaySound();
                if (item.Info.Effect == ItemEffect.Gold)        //道具类金币增加
                {
                    User.Gold += item.Count;
                    continue;
                }
                else if (item.Info.Effect == ItemEffect.GameGold)   //道具类元宝增加
                {
                    User.GameGold += (int)item.Count;
                    continue;
                }
                else if (item.Info.Effect == ItemEffect.Prestige)    //道具类声望增加
                {
                    User.Prestige += (int)item.Count;
                    continue;
                }

                if (item.Info.Effect == ItemEffect.Contribute)    //道具类贡献增加
                {
                    User.Contribute += (int)item.Count;
                    continue;
                }

                var grid = item.Slot >= Globals.PatchOffSet ? InventoryBox.PatchGrid : InventoryBox.Grid;
                var slot = item.Slot;
                if (slot >= Globals.FishingOffSet)
                {
                    slot -= Globals.FishingOffSet;
                }
                if (slot >= Globals.PatchOffSet)
                {
                    slot -= Globals.PatchOffSet;
                }
                else if (slot >= Globals.EquipmentOffSet)
                {
                    slot -= Globals.EquipmentOffSet;
                }
                if (slot >= 0 && slot < grid.Grid.Length)
                {
                    //服务端已经算好了slot
                    if (grid.Grid[slot].Item != null) //堆叠
                    {
                        //更新数目即可
                        grid.Grid[slot].Item.Count += item.Count;
                    }
                    else
                    {
                        grid.Grid[slot].Item = item;
                    }
                    grid.Grid[slot].RefreshItem();
                }
            }
        }

        /// <summary>
        /// 增加宠物道具
        /// </summary>
        /// <param name="items"></param>
        public void AddCompanionItems(List<ClientUserItem> items)
        {
            foreach (ClientUserItem item in items)
            {
                if (item.Info.Effect == ItemEffect.Experience) continue;
                if ((item.Flags & UserItemFlags.QuestItem) == UserItemFlags.QuestItem) continue;

                if (item.Info.Effect == ItemEffect.Gold)
                {
                    User.Gold += item.Count;
                    DXSoundManager.Play(SoundIndex.GoldGained);
                    continue;
                }

                bool handled = false;
                if (item.Info.StackSize > 1 && (item.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable)
                {
                    foreach (DXItemCell cell in CompanionBox.InventoryGrid.Grid)
                    {
                        if (cell.Item == null || cell.Item.Info != item.Info || cell.Item.Count >= cell.Item.Info.StackSize) continue;

                        if ((cell.Item.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                        if ((cell.Item.Flags & UserItemFlags.Bound) != (item.Flags & UserItemFlags.Bound)) continue;
                        if ((cell.Item.Flags & UserItemFlags.Worthless) != (item.Flags & UserItemFlags.Worthless)) continue;
                        if ((cell.Item.Flags & UserItemFlags.NonRefinable) != (item.Flags & UserItemFlags.NonRefinable)) continue;
                        if (!cell.Item.AddedStats.Compare(item.AddedStats)) continue;

                        if (cell.Item.Count + item.Count <= item.Info.StackSize)
                        {
                            cell.Item.Count += item.Count;
                            cell.RefreshItem();
                            handled = true;
                            break;
                        }

                        //把现有的堆叠满
                        item.Count -= item.Info.StackSize - cell.Item.Count;
                        cell.Item.Count = item.Info.StackSize;
                        cell.RefreshItem();
                    }
                    if (handled) continue;
                }

                for (int i = 0; i < CompanionBox.InventoryGrid.Grid.Length; i++)
                {
                    if (CompanionBox.InventoryGrid.Grid[i].Item != null) continue;

                    CompanionBox.InventoryGrid.Grid[i].Item = item;
                    item.Slot = i;
                    break;
                }
            }
        }

        /// <summary>
        /// 可以使用的道具 佩戴需求
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CanUseItem(ClientUserItem item)
        {
            switch (User.Gender)  //性别
            {
                case MirGender.Male:
                    if (!item.Info.RequiredGender.HasFlag(RequiredGender.Male))
                        return false;
                    break;
                case MirGender.Female:
                    if (!item.Info.RequiredGender.HasFlag(RequiredGender.Female))
                        return false;
                    break;
            }

            switch (User.Class)   //职业
            {
                case MirClass.Warrior:
                    if (!item.Info.RequiredClass.HasFlag(RequiredClass.Warrior))
                        return false;
                    break;
                case MirClass.Wizard:
                    if (!item.Info.RequiredClass.HasFlag(RequiredClass.Wizard))
                        return false;
                    break;
                case MirClass.Taoist:
                    if (!item.Info.RequiredClass.HasFlag(RequiredClass.Taoist))
                        return false;
                    break;
                case MirClass.Assassin:
                    if (!item.Info.RequiredClass.HasFlag(RequiredClass.Assassin))
                        return false;
                    break;
            }
            switch (item.Info.RequiredType)   //属性要求类型
            {
                case RequiredType.Level:
                    if (User.Level < item.Info.RequiredAmount && User.Stats[Stat.Rebirth] == 0) return false;
                    break;
                case RequiredType.MaxLevel:
                    if (User.Level > item.Info.RequiredAmount || User.Stats[Stat.Rebirth] > 0) return false;
                    break;
                case RequiredType.AC:
                    if (User.Stats[Stat.MaxAC] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.MR:
                    if (User.Stats[Stat.MaxMR] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.DC:
                    if (User.Stats[Stat.MaxDC] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.MC:
                    if (User.Stats[Stat.MaxMC] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.SC:
                    if (User.Stats[Stat.MaxSC] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.Health:
                    if (User.Stats[Stat.Health] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.Mana:
                    if (User.Stats[Stat.Mana] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.Accuracy:
                    if (User.Stats[Stat.Accuracy] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.Agility:
                    if (User.Stats[Stat.Agility] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.CompanionLevel:
                    if (Companion == null || Companion.Level < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.MaxCompanionLevel:
                    if (Companion == null || Companion.Level > item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.RebirthLevel:
                    if (User.Stats[Stat.Rebirth] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.MaxRebirthLevel:
                    if (User.Stats[Stat.Rebirth] > item.Info.RequiredAmount) return false;
                    break;
            }

            switch (item.Info.ItemType)   //道具类型
            {
                case ItemType.Book:
                    MagicInfo magic = Globals.MagicInfoList.Binding.FirstOrDefault(x => x.Index == item.Info.Shape);
                    if (magic == null) return false;
                    if (User.Magics.ContainsKey(magic) && (User.Magics[magic].Level < 3 || (item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable)) return false;
                    break;
                case ItemType.Consumable:
                    switch (item.Info.Shape)
                    {
                        case 1: //道具Buff

                            ClientBuffInfo buff = User.Buffs.FirstOrDefault(x => x.Type == BuffType.ItemBuff && x.ItemIndex == item.Info.Index);

                            if (buff != null && buff.RemainingTime == TimeSpan.MaxValue) return false;
                            break;
                    }
                    break;
            }
            return true;
        }

        /// <summary>
        /// 可穿戴道具
        /// </summary>
        /// <param name="item"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool CanWearItem(ClientUserItem item, EquipmentSlot slot)
        {
            if (!CanUseItem(item)) return false;

            switch (slot)
            {
                case EquipmentSlot.Weapon:
                case EquipmentSlot.Torch:
                case EquipmentSlot.Shield:
                    if (User.HandWeight - (Equipment[(int)slot]?.Info?.Weight ?? 0) + item.Weight > User.Stats[Stat.HandWeight])
                    {
                        ReceiveChat($"GameScene.CanWearItemHandWeight".Lang(item.Info.Lang(p => p.ItemName)), MessageType.System);
                        return false;
                    }
                    break;
                default:
                    if (User.WearWeight - (Equipment[(int)slot]?.Info?.Weight ?? 0) + item.Weight > User.Stats[Stat.WearWeight])
                    {
                        ReceiveChat($"GameScene.CanWearItemWearWeight".Lang(item.Info.Lang(p => p.ItemName)), MessageType.System);
                        return false;
                    }
                    break;
            }
            return true;
        }

        /// <summary>
        /// 宠物可穿戴道具
        /// </summary>
        /// <param name="item"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool CanCompanionWearItem(ClientUserItem item, CompanionSlot slot)
        {
            if (Companion == null) return false;

            if (!CanCompanionUseItem(item.Info)) return false;

            return true;
        }
        /// <summary>
        /// 宠物可穿戴道具限制
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool CanCompanionUseItem(ItemInfo info)
        {
            switch (info.RequiredType)
            {
                case RequiredType.CompanionLevel:
                    if (Companion == null || Companion.Level < info.RequiredAmount) return false;
                    break;
                case RequiredType.MaxCompanionLevel:
                    if (Companion == null || Companion.Level > info.RequiredAmount) return false;
                    break;
            }
            return true;
        }
        /// <summary>
        /// 钓鱼可穿戴道具
        /// </summary>
        /// <param name="item"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool CanFishingWearItem(ClientUserItem item, FishingSlot slot)
        {
            if (!Game.User.HasFishingRod) return false;

            return true;
        }

        /// <summary>
        /// 包裹刷新
        /// </summary>
        /// <param name="items"></param>
        public void SortFillItems(List<ClientUserItem> items)
        {
            for (int i = 0; i < Globals.InventorySize; i++)
            {
                InventoryBox.Grid.Grid[i].Item = null;
            }
            foreach (ClientUserItem item in items)  //清空原包裹后 重新赋值
            {
                InventoryBox.Grid.Grid[item.Slot].Item = item;
            }
        }

        /// <summary>
        /// 返回人物包裹中某物品的数量
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public long CountItemInventory(ItemInfo info)
        {
            return InventoryBox.Grid.Grid.Sum(c => c.Item?.Info?.Index == info.Index ? c.Item.Count : 0);
        }

        /// <summary>
        /// 返回宠物包裹中某物品的数量
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public long CountItemPetInventory(ItemInfo info)
        {
            return CompanionBox.InventoryGrid.Grid.Sum(c => c.Item?.Info?.Index == info.Index ? c.Item.Count : 0);
        }

        /// <summary>
        /// 返回人物仓库中某物品的数量
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public long CountItemStorage(ItemInfo info)
        {
            return StorageBox.Grid.Grid.Sum(c => c.Item?.Info?.Index == info.Index ? c.Item.Count : 0);
        }

        /// <summary>
        /// 返回人物所有某物品的数量
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public long CountItemAll(ItemInfo info)
        {
            return CountItemInventory(info) + CountItemPetInventory(info) + CountItemStorage(info);
        }

        /// <summary>
        /// 制作等级经验更新
        /// </summary>
        public void CraftStatsChanged()
        {
            if (User == null) return;
            int currCraftLevel = User.CraftLevel;
            CharacterBox.CraftLevelLabel.Text = currCraftLevel.ToString();

            CharacterBox.CraftExpLabel.Text = User.CraftExp - Globals.CraftExpDict[Math.Max(currCraftLevel - 1, 0)] + "/" +
                                              (Globals.CraftExpDict[Math.Min(Globals.CraftExpDict.Count - 1, currCraftLevel)] -
                                               Globals.CraftExpDict[Math.Max(currCraftLevel - 1, 0)]);
        }

        /// <summary>
        /// 制造物品书签和进度更新
        /// </summary>
        public void CraftBookmarkChanged()
        {
            if (User == null) return;
        }

        /// <summary>
        /// 开始制造
        /// </summary>
        /// <param name="item"></param>
        /// <param name="completeTime"></param>
        public void CraftStarted(CraftItemInfo item, DateTime completeTime)
        {
            User.CraftFinishTime = completeTime;
            User.CraftingItem = item;
        }

        /// <summary>
        /// 用户更新
        /// </summary>
        public void UserChanged()
        {
            LevelChanged();
            ClassChanged();
            StatsChanged();
            ExperienceChanged();
            HealthChanged();
            ManaChanged();
            GoldChanged();
            SafeZoneChanged();
            AttackModeChanged();
            PetModeChanged();
            CraftStatsChanged();

            BigPatchBox?.UserChanged();
            MagicBarBox?.UpdateIcons();
            TradeBox.CloseButton.Enabled = !Observer;
            TradeBox.ConfirmButton.Visible = !Observer;

            NPCBox.CloseButton.Enabled = !Observer;
            NPCGoodsBox.CloseButton.Enabled = !Observer;
            NPCRefineBox.CloseButton.Enabled = !Observer;
            NPCRepairBox.CloseButton.Enabled = !Observer;
            NPCSpecialRepairBox.CloseButton.Enabled = !Observer;
            NPCSellBox.CloseButton.Enabled = !Observer;
            NPCRootSellBox.CloseButton.Enabled = !Observer;
            NPCRefineRetrieveBox.CloseButton.Enabled = !Observer;
        }
        /// <summary>
        /// 等级更新
        /// </summary>
        public void LevelChanged()
        {
            if (User == null) return;

            User.MaxExperience = User.Level < Globals.GamePlayEXPInfoList.Count ? Globals.GamePlayEXPInfoList[User.Level].Exp : 0;   //等级经验
            //MainPanel.LevelLabel.Text = User.Level.ToString();   //UI面板等级显示
            CharacterBox.LevelLabel.Text = User.Level.ToString(); //角色面板等级显示

#if Mobile
            LeftUpUserPanel.LevelLabel.Text = User.Level.ToString(); //UI面板等级显示
#else
            ChatBox.LevelLabel.Text = User.Level.ToString(); //UI面板等级显示
#endif

            foreach (NPCGoodsCell cell in NPCGoodsBox.Cells)
                cell.UpdateColours();

            foreach (KeyValuePair<MagicInfo, MagicCell> pair in MagicBox.Magics)
                pair.Value.Refresh();

            CheckNewQuests();
        }
        /// <summary>
        /// 角色职业更新
        /// </summary>
        public void ClassChanged()
        {
            if (User == null) return;

            //Type itemType = User.Class.GetType(); ;
            //MemberInfo[] infos = itemType.GetMember(User.Class.ToString());

            //DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();
            //MainPanel.ClassLabel.Text = $"{description?.Description ?? User.Class.ToString()}";

            foreach (NPCGoodsCell cell in NPCGoodsBox.Cells)
                cell.UpdateColours();

#if Mobile
#else
            MainPanel.DCLabel.Visible = (User.Class == MirClass.Warrior || User.Class == MirClass.Assassin);
            MainPanel.MCLabel.Visible = User.Class == MirClass.Wizard;
            MainPanel.SCLabel.Visible = User.Class == MirClass.Taoist;
#endif

            //MagicBox?.CreateTabs();
        }
        /// <summary>
        /// 角色属性更新
        /// </summary>
        public void StatsChanged()
        {
            if (User.Stats == null) return;

            User.Light = Math.Max(3, User.Stats[Stat.Light]);

#if Mobile
#else
            MainPanel.ACLabel.Text = User.Stats.GetFormat(Stat.MaxAC);
            MainPanel.MRLabel.Text = User.Stats.GetFormat(Stat.MaxMR);
            MainPanel.DCLabel.Text = User.Stats.GetFormat(Stat.MaxDC);
            MainPanel.MCLabel.Text = User.Stats.GetFormat(Stat.MaxMC);
            MainPanel.SCLabel.Text = User.Stats.GetFormat(Stat.MaxSC);
#endif

            HealthChanged();
            ManaChanged();

            foreach (NPCGoodsCell cell in NPCGoodsBox.Cells)
                cell.UpdateColours();

            CharacterBox.UpdateStats();   //人物属性刷新
            AdditionalBox.UpdateStats();  //额外属性刷新
        }
        /// <summary>
        /// 角色经验值更新
        /// </summary>
        public void ExperienceChanged()
        {
            if (User == null) return;

            //MainPanel.ExperienceLabel.Text = User.MaxExperience > 0 ? $"{User.Experience/User.MaxExperience: #,##0.00%}" : $"{User.Experience: #,##0#}";  //UI面板经验值显示

            CharacterBox.ExceptionLabel.Text = User.MaxExperience > 0 ? $"{User.Experience / User.MaxExperience: ###0%}" : $"{User.Experience: ###0}";  //人物面板经验值显示
#if Mobile
            ExpPanel.ExpLabel.Text = User.MaxExperience > 0 ? $"{User.Experience:###0}/{User.MaxExperience:###0}" + $" {User.Experience / User.MaxExperience: ###0.##%}" : $"{User.Experience: ###0%}";
#else
            MainPanel.ExperienceBar.Hint = $"GameScene.ExperienceChanged".Lang(User.Experience.ToString("###0"), User.MaxExperience.ToString("###0"));
            Game.TopInfoBox.EXPLabel.Text = $"经验".Lang() + $"  {User.Experience:###0}/{User.MaxExperience:###0}";
#endif

        }
        /// <summary>
        /// 角色血量更新
        /// </summary>
        public void HealthChanged()
        {
            if (User == null) return;

#if Mobile
            LeftUpUserPanel.HealthBarLabel.Text = $"{Math.Max(0, User.CurrentHP)}/{User.Stats[Stat.Health]}";
#else
            MainPanel.HealthBar.Hint = $"(" + "体力".Lang() + $"){Math.Max(0, User.CurrentHP)} / {User.Stats[Stat.Health]}";
            MainPanel.HealthLabel.Text = $"{Math.Max(0, User.CurrentHP)}/{User.Stats[Stat.Health]}";
#endif
            CharacterBox.HealthLabel.Text = $"{Math.Max(0, User.CurrentHP)}/{User.Stats[Stat.Health]}";
            if (Game.BigPatchBox != null && Game.BigPatchBox.AutoPotionPage.chkHPSynchro.Checked)
            {
                Game.BigPatchBox.AutoPotionPage.NumberMindHP.ValueTextBox.Value = User.Stats[Stat.Health];
            };
        }
        /// <summary>
        /// 角色蓝值更新
        /// </summary>
        public void ManaChanged()
        {
            if (User == null) return;

#if Mobile
            LeftUpUserPanel.ManaBarLabel.Text = $"{Math.Max(0, User.CurrentMP)}/{User.Stats[Stat.Mana]}";
#else
            MainPanel.ManaBar.Hint = $"(" + "法力".Lang() + $"){Math.Max(0, User.CurrentMP)} / {User.Stats[Stat.Mana]}";
            MainPanel.ManaLabel.Text = $"{Math.Max(0, User.CurrentMP)}/{User.Stats[Stat.Mana]}";
#endif
            CharacterBox.ManaLabel.Text = $"{Math.Max(0, User.CurrentMP)}/{User.Stats[Stat.Mana]}";
            if (Game.BigPatchBox != null && Game.BigPatchBox.AutoPotionPage.chkMPSynchro.Checked)
            {
                Game.BigPatchBox.AutoPotionPage.NumberMindMP.ValueTextBox.Value = User.Stats[Stat.Mana];
            };
        }
        /// <summary>
        /// 攻击模式更新
        /// </summary>
        public void AttackModeChanged()
        {
            if (User == null) return;

            //Type type = typeof(AttackMode);

            //MemberInfo[] infos = type.GetMember(User.AttackMode.ToString());

            //DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

            ConfigBox.AttackModeLabel.Text = $"攻击模式".Lang() + $"{User.AttackMode.Lang()}";

#if Mobile
            LeftUpUserPanel?.AttackModeChanged();
#else
            MainPanel.AttackModeLabel.Text = User.AttackMode.Lang();
#endif
        }
        /// <summary>
        /// 宠物攻击模式更新
        /// </summary>
        public void PetModeChanged()
        {
            if (User == null) return;

            //Type type = typeof(PetMode);

            //MemberInfo[] infos = type.GetMember(User.PetMode.ToString());

            //DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

#if Mobile
            LeftUpUserPanel?.PetModeChanged();
#else
            MainPanel.PetModeLabel.Text = User.PetMode.Lang();
#endif
        }
        /// <summary>
        /// 自动挂机时间更新
        /// </summary>
        public void AutoTimeChanged()
        {
            if (User == null) return;
            BigPatchBox?.OnTimerChanged(User.AutoTime);
        }
        /// <summary>
        /// 金币元宝等属性更新
        /// </summary>
        public void GoldChanged()
        {
            if (User == null) return;

            InventoryBox.GoldLabel.Text = User.Gold.ToString("#,##0");            //金币
            InventoryBox.GameGoldLabel.Text = (Convert.ToDecimal(User.GameGold) / 100).ToString("[###0.00] " + "赞助币".Lang());    //元宝
            MarketPlaceBox.GameGoldBox.Text = (Convert.ToDecimal(User.GameGold) / 100).ToString("###0.00");          //元宝
            MarketPlaceBox.HuntGoldBox.Value = User.HuntGold;          //赏金
            GoldTradingBusinessBox.GoldBox.Text = User.Gold.ToString("#,##0");            //金币
            GoldTradingBusinessBox.GameGoldBox.Text = (Convert.ToDecimal(User.GameGold) / 100).ToString("###0.00");    //元宝

            //CharacterBox.PrestigeLabel.Text = User.Prestige.ToString("#,##0");    //声望
            //CharacterBox.ContributeLabel.Text = User.Contribute.ToString("#,##0");   //贡献
            NPCAdoptCompanionBox.RefreshUnlockButton();

#if Mobile
#else
            Game.TopInfoBox.ModeLabel.Text = $"金币".Lang() + $"  {User.Gold.ToString("###0")}";
#endif

            foreach (NPCGoodsCell cell in NPCGoodsBox.Cells)
                cell.UpdateColours();
        }

        /// <summary>
        /// 安全区更新
        /// </summary>
        public void SafeZoneChanged()
        {
            if (User == null) return;
            User.MovingOffSetChanged();
        }

        /// <summary>
        /// 发送屏幕正中显示的安全区进出提示
        /// </summary>
        public void SendTipWarning(string text)
        {
            if (TipWarning == null)
            {
                TipWarning = new WarningObject(text, Color.White, this);
                TipWarning.Label.Location = new Point(Size.Width / 2 - TipWarning.Label.Size.Width / 2, 100);
                TipWarning.Label.Size = new Size(TipWarning.Label.Size.Width, TipWarning.Label.Size.Height);
            }
            else
            {
                TipWarning.Label.Visible = true;
                TipWarning.Label.Text = text;
                TipWarning.StartTime = CEnvir.Now;
            }
        }

        /// <summary>
        /// 重量信息更新
        /// </summary>
        public void WeightChanged()
        {
            if (User == null) return;

            InventoryBox.WeightLabel.Text = $"重量".Lang() + $":  {User.BagWeight} / {User.Stats[Stat.BagWeight]}";      //背包负重显示
            InventoryBox.WeightLabel.ForeColour = User.BagWeight > User.Stats[Stat.BagWeight] ? Color.Red : Color.FromArgb(200, 200, 255);

            CharacterBox.PackLabel.Text = $"{User.BagWeight}/{User.Stats[Stat.BagWeight]}";     //人物界面负重显示
            CharacterBox.PackLabel.ForeColour = User.BagWeight > User.Stats[Stat.BagWeight] ? Color.Red : Color.White;

            CharacterBox.WearWeightLabel.Text = $"{User.WearWeight}/{User.Stats[Stat.WearWeight]}";   //人物界面穿戴显示
            CharacterBox.WearWeightLabel.ForeColour = User.WearWeight > User.Stats[Stat.WearWeight] ? Color.Red : Color.White;

            CharacterBox.HandWeightLabel.Text = $"{User.HandWeight}/{User.Stats[Stat.HandWeight]}";   //人物界面腕力显示
            CharacterBox.HandWeightLabel.ForeColour = User.HandWeight > User.Stats[Stat.HandWeight] ? Color.Red : Color.White;

#if Mobile
#else
            MainPanel.BagWeightBar.Hint = $"(" + "负重".Lang() + $"){User.BagWeight} / {User.Stats[Stat.BagWeight]}";      //UI主面板背包负重显示
            Game.TopInfoBox.BagWeightLabel.Text = $"负重".Lang() + $"  {User.BagWeight}/{User.Stats[Stat.BagWeight]} (" + "剩".Lang() + $"{User.Stats[Stat.BagWeight] - User.BagWeight})";
#endif
        }
        /// <summary>
        /// 宠物信息更新
        /// </summary>
        public void CompanionChanged()
        {
            NPCCompanionStorageBox.UpdateScrollBar();

            CompanionBox.CompanionChanged();
            PickUpSettingsS.CompanionChanged();
        }
        /// <summary>
        /// 婚姻状态更新
        /// </summary>
        public void MarriageChanged()
        {
            CharacterBox.MarriageIconLabel.Visible = CharacterBox.MarriageLabel.Visible = CharacterBox.MarriageIcon.Visible = !string.IsNullOrEmpty(Partner?.Name);
            CharacterBox.MarriageLabel.Text = CharacterBox.MarriageIcon.Hint = Partner?.Name;
        }
        /// <summary>
        /// 接收聊天信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public void ReceiveChat(string message, MessageType type)
        {
            if (type == MessageType.RollNotice)   //系统正中公告栏
                ChatNoticeBox.ShowNotice(message);

            if (Config.LogChat)
            {
                switch (type)
                {
                    case MessageType.Normal:        //普通文字
                    case MessageType.Shout:         //区域聊天
                    case MessageType.Group:         //组队聊天
                    case MessageType.Global:        //世界聊天
                    case MessageType.WhisperIn:     //私聊信息
                    case MessageType.GMWhisperIn:   //GM私聊信息
                    case MessageType.WhisperOut:    //收到私聊信息
                    case MessageType.ObserverChat:  //观察者聊天
                    case MessageType.Guild:         //行会聊天
                        CEnvir.ChatLog.Enqueue($"[{Time.Now:F}]: {message}");
                        break;
                    case MessageType.System:        //系统信息
                    case MessageType.Hint:          //提示信息
                    case MessageType.Announcement:  //公告信息
                    case MessageType.Combat:        //战斗信息提示
                    case MessageType.Notice:        //告示
                    case MessageType.RollNotice:    //中央滚动告示
                    case MessageType.ItemTips:      //极品物品提示
                    case MessageType.BossTips:      //boss提示
                    case MessageType.UseItem:       //道具使用提示
                    case MessageType.DurabilityTips: //持久提示
                        CEnvir.SystemLog.Enqueue($"[{Time.Now:F}]: {message}");
                        break;
                }

            }
            ChatBox.ReceiveChat(message, type);
            ChatLBox.ReceiveChat(message, type);
            ChatRBox.ReceiveChat(message, type);
        }

        /// <summary>
        /// 可以接受的任务类型信息
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        public bool CanAccept(QuestInfo quest)
        {
            if (quest.QuestType != QuestType.Repeatable &&    //任务不是重复任务
                quest.QuestType != QuestType.Daily &&         //任务不是每日万事通任务
                quest.QuestType != QuestType.Hidden &&        //任务不是奇遇任务
                QuestLog.Any(x => x.Quest == quest)) return false;   //角色任务列表里的任务不等任务里的  那么无法接任务

            foreach (QuestRequirement requirement in quest.Requirements)  //遍历任务要求判断
            {
                switch (requirement.Requirement)
                {
                    case QuestRequirementType.MinLevel:
                        if (User.Level < requirement.IntParameter1) return false;
                        break;
                    case QuestRequirementType.MaxLevel:
                        if (User.Level > requirement.IntParameter1) return false;
                        break;
                    case QuestRequirementType.NotAccepted:
                        if (QuestLog.Any(x => x.Quest == requirement.QuestParameter)) return false;

                        break;
                    case QuestRequirementType.HaveCompleted:
                        if (QuestLog.Any(x => x.Quest == requirement.QuestParameter && x.Completed)) break;

                        return false;
                    case QuestRequirementType.HaveNotCompleted:
                        if (QuestLog.Any(x => x.Quest == requirement.QuestParameter && x.Completed)) return false;

                        break;
                    case QuestRequirementType.Class:
                        switch (User.Class)
                        {
                            case MirClass.Warrior:
                                if ((requirement.Class & RequiredClass.Warrior) != RequiredClass.Warrior) return false;

                                break;
                            case MirClass.Wizard:
                                if ((requirement.Class & RequiredClass.Wizard) != RequiredClass.Wizard) return false;
                                break;
                            case MirClass.Taoist:
                                if ((requirement.Class & RequiredClass.Taoist) != RequiredClass.Taoist) return false;
                                break;
                            case MirClass.Assassin:
                                if ((requirement.Class & RequiredClass.Assassin) != RequiredClass.Assassin) return false;
                                break;
                        }
                        break;
                }
            }

            //任务是可重复任务  且 重复任务次数小于等于0 就无法接任务
            if (quest.QuestType == QuestType.Repeatable && User.RepeatableQuestRemains <= 0)
            {
                return false;
            }

            //任务是万事通任务  且 每日万事通任务次数小于等于0 就无法接任务
            // ||QuestLog.Any(x => x.Quest == quest && !x.Completed)
            if (quest.QuestType == QuestType.Daily && User.DailyQuestRemains <= 0)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 任务变化更新
        /// </summary>
        /// <param name="quest"></param>
        public void QuestChanged(ClientUserQuest quest)
        {
            CheckNewQuests();

            QuestBox.QuestChanged(quest);
        }
        /// <summary>
        /// 检查新任务
        /// </summary>
        public void CheckNewQuests()
        {
            QuestBox.PopulateQuests();

            QuestTrackerBox.PopulateQuests();

            NPCQuestListBox.UpdateQuestDisplay();

            UpdateQuestIcons();
        }
        /// <summary>
        /// 任务进行中
        /// </summary>
        /// <param name="info"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public bool HasQuest(MonsterInfo info, MapInfo map)
        {
            foreach (QuestTaskMonsterDetails detail in info.QuestDetails)
            {
                if (detail.Map != null && detail.Map != map) continue;

                QuestInfo quest = QuestBox.CurrentTab.Quests.FirstOrDefault(x => x == detail.Task?.Quest);

                if (quest == null) continue;

                ClientUserQuest userQuest = QuestLog.First(x => x.Quest == quest);

                if (userQuest.IsComplete) continue;

                ClientUserQuestTask UserTask = userQuest.Tasks.FirstOrDefault(x => x.Task == detail.Task);

                if (UserTask != null && UserTask.Completed) continue;

                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取任务信息文本
        /// </summary>
        /// <param name="questInfo"></param>
        /// <param name="userQuest"></param>
        /// <param name="isLog"></param>
        /// <returns></returns>
        public string GetQuestText(QuestInfo questInfo, ClientUserQuest userQuest, bool isLog)
        {
            string text;

            if (userQuest == null)
                text = questInfo.AcceptText;          //可接
            else if (userQuest.Completed)
                text = questInfo.ArchiveText;         //完成
            else if (userQuest.IsComplete && !isLog)
                text = questInfo.CompletedText;       //完成
            else
                text = questInfo.ProgressText;        //当前

            text = text.Replace("[PLAYERNAME]", User.Name);
            if (questInfo.StartNPC != null)
                text = text.Replace("[STARTNAME]", questInfo.StartNPC.NPCName);
            if (questInfo.FinishNPC != null)
                text = text.Replace("[FINISHNAME]", questInfo.FinishNPC.NPCName);

            return text;

        }
        /// <summary>
        /// 获取任务要求文本
        /// </summary>
        /// <param name="questInfo"></param>
        /// <param name="userQuest"></param>
        /// <returns></returns>
        public void GetTaskText(DXListView listView, QuestInfo questInfo, ClientUserQuest userQuest)
        {
            if (listView.ItemCount > 0) listView.RemoveAll();
            uint rows = 0;
            foreach (QuestTask task in questInfo.Tasks)
            {
                StringBuilder builder = new StringBuilder();
                ClientUserQuestTask userTask = userQuest?.Tasks.FirstOrDefault(x => x.Task == task);

                bool needComma = false;
                switch (task.Task)
                {
                    case QuestTaskType.KillMonster:
                        for (int i = 0; i < task.MonsterDetails.Count; i++)
                        {
                            QuestTaskMonsterDetails monster = task.MonsterDetails[i];
                            if (monster == null) continue;
                            if (i > 3)
                            {
                                builder.Append("...");
                                break;
                            }

                            if (needComma)
                                builder.Append(" " + "或".Lang() + " ");

                            needComma = true;

                            builder.Append(monster.Monster?.MonsterName ?? string.Empty);

                            if (!string.IsNullOrEmpty(monster.Map?.Description))
                                builder.Append(" (" + monster.Map.Description + ")");
                        }
                        break;
                    case QuestTaskType.GainItem:
                        builder.Append(task.ItemParameter?.Lang(p => p.ItemName));
                        for (int i = 0; i < task.MonsterDetails.Count; i++)
                        {
                            QuestTaskMonsterDetails monster = task.MonsterDetails[i];
                            if (monster == null) continue;
                            if (i > 2)
                            {
                                builder.Append("...");
                                break;
                            }

                            if (needComma)
                                builder.Append(" " + "或".Lang() + " ");

                            needComma = true;

                            builder.Append(" (" + (monster.Monster?.MonsterName ?? string.Empty) + ")");

                        }
                        break;
                }

                string colum3;
                if (userQuest != null)
                {
                    if (userTask != null && userTask.Completed)
                        colum3 = ("完成".Lang());
                    else
                        colum3 = ($"{userTask?.Amount ?? 0}/{task.Amount}");
                }
                else
                    colum3 = ($"0/{task.Amount}");

                rows = listView.InsertItem(rows, (rows + 1).ToString());
                listView.SetItem(rows, 1, builder.ToString());
                listView.SetItem(rows, 2, colum3);
                rows++;
            }
            if (listView.ItemCount > 0) listView.VScrollBar.Value = 1;
        }

        /// <summary>
        /// 获取任务信息
        /// </summary>
        /// <param name="task"></param>
        /// <param name="userQuest"></param>
        /// <returns></returns>
        public string[] GetTrackerText(QuestTask task, ClientUserQuest userQuest)
        {
            string[] texts = new string[2];

            ClientUserQuestTask userTask = userQuest?.Tasks.FirstOrDefault(x => x.Task == task);

            switch (task.Task)
            {
                case QuestTaskType.KillMonster:
                    texts[0] = task.MonsterDetails.Count > 2 ? task.MonsterDetails[0].Monster.MonsterName + "…" : string.Join("、", task.MonsterDetails.Select(p => p.Monster.MonsterName).ToArray());
                    break;
                case QuestTaskType.GainItem:
                    texts[0] = task.ItemParameter?.Lang(p => p.ItemName);
                    break;
            }

            if (userQuest != null)
            {
                texts[1] = $"{userTask?.Amount ?? 0}/{task.Amount}";
            }

            return texts;
        }
        /// <summary>
        /// 更新任务NPC头顶动画
        /// </summary>
        public void UpdateQuestIcons()
        {
            foreach (NPCInfo info in Globals.NPCInfoList.Binding)      //循环 NPC信息列表
                info.CurrentIcon = QuestIcon.None;                     //当前任务图标为空

            foreach (QuestInfo quest in QuestBox.AvailableTab.Quests)  //循环 可接任务项
            {
                if (quest.StartNPC == null) continue;                    //如果起始任务NPC等空 跳出

                quest.StartNPC.CurrentIcon |= QuestIcon.NewQuest;      //创建新任务图标
            }

            if (User.RepeatableQuestRemains > 0)    //判断当前重复任务次数
            {
                foreach (QuestInfo quest in Globals.QuestInfoList.Binding.Where(x => x.QuestType == QuestType.Repeatable))
                {
                    if (quest.StartItem == null || quest.StartNPC == null) continue;   //如果起始任务NPC 或 道具接任务 等空 跳出

                    quest.StartNPC.CurrentIcon |= QuestIcon.NewQuest;   //创建新任务图标
                }
            }

            if (User.DailyQuestRemains > 0)   //判断当前每日任务次数
            {
                foreach (QuestInfo quest in Globals.QuestInfoList.Binding.Where(x => x.QuestType == QuestType.Daily))  //循环 万事通任务
                {
                    if (quest.StartNPC == null) continue;                        //如果起始任务NPC等空 跳出

                    quest.StartNPC.CurrentIcon |= QuestIcon.NewQuest;          //创建新任务图标
                }
            }

            foreach (QuestInfo quest in Globals.QuestInfoList.Binding.Where(x => x.QuestType == QuestType.Hidden))   //循环  奇遇任务
            {
                if (quest.StartItem == null || quest.StartNPC == null) continue;   //如果起始任务NPC 或  道具接任务 等空 跳出

                quest.StartNPC.CurrentIcon |= QuestIcon.NewQuest;    //创建新任务图标
            }

            bool completed = false;   //完成判断开关

            foreach (QuestInfo quest in QuestBox.CurrentTab.Quests)      //循环 当前任务页  任务
            {
                ClientUserQuest userQuest = QuestLog.First(x => x.Quest == quest);

                if (userQuest.IsComplete)   //如果角色任务已完成
                {
                    quest.FinishNPC.CurrentIcon |= QuestIcon.QuestComplete;   //当前结束NPC的图标 显示 已完成
                    completed = true;    //完成开关开启
                }
                else
                {
                    if (quest.QuestType == QuestType.Daily)    //如果是每日任务
                    {
                        if (quest.StartNPC != null)            //接任务NPC不为空
                            quest.StartNPC.CurrentIcon = QuestIcon.QuestIncomplete;   //当前接NPC任务显示 进行中图标
                    }
                    quest.FinishNPC.CurrentIcon |= QuestIcon.QuestIncomplete;   //当前结束NPC显示 任务进行中
                }
            }

#if Mobile
#else
            MainPanel.AvailableQuestIcon.Visible = QuestBox.AvailableTab.Quests.Count > 0;
            MainPanel.CompletedQuestIcon.Visible = completed;
#endif

            foreach (NPCInfo info in Globals.NPCInfoList.Binding)   //循环  NPC信息列表
            {
                BigMapBox.Update(info);                             //大地图更新任务信息
                MiniMapBox.Update(info);                            //小地图更新任务信息
#if Mobile
#else
                ChatBox.Photo.Update(info);
#endif
                WarWeaponBox.Map.Update(info);
            }

            foreach (MapObject ob in MapControl.Objects)           //循环 地图对象
                ob.UpdateQuests();                                 //更新任务地图对象

            foreach (ClientObjectData data in DataDictionary.Values)   //循环客户端数据库 数据资料
            {
                BigMapBox.Update(data);                                //大地图更新数据
                MiniMapBox.Update(data);                               //小地图更新数据
#if Mobile
#else
                ChatBox.Photo.Update(data);
#endif
                WarWeaponBox.Map.Update(data);
            }

            foreach (MapObject ob in MapControl.Objects)
            {
                if (CEnvir.UpdatedNPCLooks.TryGetValue(ob.ObjectID, out var p))
                {
                    NPCObject npc = ((NPCObject)ob);
                    npc.UpdateNPCLook(p.NPCName, p.NameColor, p.OverlayColor, p.Library, p.ImageIndex);
                    if (p.UpdateNPCIcon)
                    {
                        npc.UpdateNPCIcon(p.Icon);
                    }
                }
            }
        }
        /// <summary>
        /// 获取NPC图标控制
        /// </summary>
        /// <param name="NPC"></param>
        /// <returns></returns>
        public DXControl GetNPCControl(NPCInfo NPC)
        {
            int icon = 0;
            Color colour = Color.White;

            if ((NPC.CurrentIcon & QuestIcon.QuestComplete) == QuestIcon.QuestComplete) //任务完成
            {
                icon = 98;
                colour = Color.Yellow;

            }
            else if ((NPC.CurrentIcon & QuestIcon.NewQuest) == QuestIcon.NewQuest)   //新任务
            {
                icon = 97;
                colour = Color.Yellow;
            }
            else if ((NPC.CurrentIcon & QuestIcon.QuestIncomplete) == QuestIcon.QuestIncomplete)  //任务进行
            {
                icon = 98;
                colour = Color.White;
            }

            if (icon > 0)
            {
                DXImageControl image = new DXImageControl
                {
                    LibraryFile = LibraryFile.Interface,
                    Index = icon,
                    ForeColour = colour,
                    Hint = NPC.NPCName,
                    Tag = NPC.CurrentIcon,
                };
                image.OpacityChanged += (o, e) => image.ImageOpacity = image.Opacity;

                return image;
            }

            return new DXControl
            {
                Size = new Size(3, 3),
                DrawTexture = true,
                Hint = NPC.NPCName,
                BackColour = Color.Yellow,
                Tag = NPC.CurrentIcon,
            };
        }

        /// <summary>
        /// 联盟判断
        /// </summary>
        /// <param name="objectID"></param>
        /// <returns></returns>
        public bool IsAlly(uint objectID)
        {
            if (User.ObjectID == objectID) return true;

            if (Partner != null && Partner.ObjectID == objectID) return true;   //配偶

            foreach (ClientPlayerInfo member in GroupBox.Members)  //组队
                if (member.ObjectID == objectID) return true;

            if (GuildBox.GuildInfo != null)            //行会
                foreach (ClientGuildMemberInfo member in GuildBox.GuildInfo.Members)
                    if (member.ObjectID == objectID) return true;

            return false;
        }

        /// <summary>
        /// 使用魔法（魔法类型）
        /// </summary>
        /// <param name="type"></param>
        public void UseMagic(MagicType type)
        {
            if (Game.Observer || User == null || User.Horse != HorseType.None || MagicBarBox == null || type == MagicType.None) return;

            ClientUserMagic magic = null;
            foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in User.Magics)
            {
                if (pair.Value.Info.Magic == type)
                {
                    magic = pair.Value;
                    break;
                }
            }
            if (magic == null) return;
            if (CEnvir.Now < magic.NextCast) return;
            UseMagic(magic);
        }

        /// <summary>
        /// 自动装备符
        /// </summary>
        /// <param name="magic"></param>
        /// <returns></returns>
        private MagicHelper TakeAmulet(ClientUserMagic magic)
        {
            if (User.Class != MirClass.Taoist)
            {
                return null;
            }
            // Amulet 符的位置
            MagicHelper helper = null;
            for (int i = 0; i < BigPatchConfig.magics.Count; i++)
            {
                if (BigPatchConfig.magics[i].TypeID == magic.Info.Magic)
                {
                    helper = BigPatchConfig.magics[i];
                    break;
                }
            }
            bool isNot = false;
            switch (magic.Info.Magic) //增加不使用符的技能判断
            {
                case MagicType.Heal:
                case MagicType.PoisonDust:
                case MagicType.MassHeal:
                case MagicType.LifeSteal:
                case MagicType.GreaterPoisonDust:
                    isNot = true;
                    break;
            }

            //自动换符
            if (!BigPatchConfig.AutoAmulet || isNot) return helper;

            if (null == helper) return helper;
            int needShape = -1;

            needShape = helper.Amulet;
            int shape = -1;

            if (CharacterBox.Grid[(int)EquipmentSlot.Amulet].Item?.Info?.ItemType == ItemType.Amulet)
            {
                shape = CharacterBox.Grid[(int)EquipmentSlot.Amulet].Item?.Info.Shape ?? -1;
            }

            if (magic.Info.Magic == MagicType.Resurrection)
            {
                needShape = 0;
            }

            long count = CharacterBox.Grid[(int)EquipmentSlot.Amulet].Item?.Count ?? 0;

            if (needShape == -1 && shape != -1 && count >= 25) return helper; //回生已佩戴

            if (shape != -1 && needShape == shape && count >= 25) return helper; //已经有不需要戴

            for (int i = 0; i < Inventory.Length; i++)
            {
                if ((Inventory[i]?.Info.ItemType ?? 0) == ItemType.Amulet)//是符
                {
                    //包里找到了一个符
                    ItemInfo info = Inventory[i]?.Info;
                    if (info.Shape == needShape || needShape == -1 && info.Shape != 0)//指定的符 或者 任意符 
                    {
                        //找到了想要的符
                        shape = info.Shape;
                        DXItemCell[] grid = InventoryBox.Grid.Grid;
                        CharacterBox.Grid[(int)EquipmentSlot.Amulet].ToEquipment(grid[i]);////到装备单元格
                        break;
                    }
                }
            }

            if (shape == -1)
            {
                ReceiveChat($"GameScene.AutoAmulet".Lang(), MessageType.Hint);
            }
            return helper;
        }
        /// <summary>
        /// 使用魔法技能
        /// </summary>
        /// <param name="magic">技能</param>
        public void UseMagic(ClientUserMagic magic)
        {
            if (magic == null || User.Level < magic.Info.NeedLevel1) return;  //如果技能等空 或 角色级别小于技能等级 跳出

            MapObject target = null;   //目标等空
            MagicHelper helper = TakeAmulet(magic);  //自动符

            // 攻城期间 部分技能无法在旗帜附近使用
            // 瞬息移动Teleportation /乾坤大挪移Interchange /斗转星移Beckon /移行换位GeoManipulation /鹰击DanceOfSwallow /妙影无踪Transparency
            if (!CEnvir.AllowTeleportMagicNearFlag)
            {
                // 技能处于限制列表内
                if (Functions.IsTeleportMagic(magic.Info.Magic))
                {
                    // 是否处于攻城期间
                    CastleInfo warCastle =
                        GameScene.Game.ConquestWars.FirstOrDefault(x => x.Map == GameScene.Game.MapControl.MapInfo);
                    if (warCastle != null)
                    {
                        if (CEnvir.WarFlagsDict.TryGetValue(warCastle, out Point flagLocation))
                        {
                            //处于限制范围之内？
                            if (flagLocation != Point.Empty)
                            {
                                int distance = Functions.Distance(Game.User.CurrentLocation, flagLocation);
                                if (distance <= CEnvir.TeleportMagicRadiusRange)
                                {
                                    // Cannot use this magic near the flag
                                    ReceiveChat("旗帜附近无法使用此技能".Lang(Config.Language), MessageType.System);
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            //其他类型的技能限制
            if (CEnvir.MapMagicRestrictions.TryGetValue(GameScene.Game.MapControl.MapInfo.Description,
                out var restrictedMagics))
            {
                if (restrictedMagics.Contains(magic.Info.Magic))
                {
                    // Cannot use this magic on this map
                    ReceiveChat("本地图禁止使用此技能".Lang(Config.Language), MessageType.System);
                    return;
                }
            }

            switch (magic.Info.Magic)
            {
                case MagicType.Swordsmanship:
                case MagicType.SpiritSword:
                case MagicType.VineTreeDance:
                case MagicType.WillowDance:
                    return;
                case MagicType.Thrusting:
                    if (CEnvir.Now < ToggleTime) return;
                    ToggleTime = CEnvir.Now.AddSeconds(1);
                    CEnvir.Enqueue(new C.MagicToggle { Magic = magic.Info.Magic, CanUse = !User.CanThrusting });
                    return;
                case MagicType.HalfMoon:
                    if (CEnvir.Now < ToggleTime) return;
                    ToggleTime = CEnvir.Now.AddSeconds(1);
                    CEnvir.Enqueue(new C.MagicToggle { Magic = magic.Info.Magic, CanUse = !User.CanHalfMoon });
                    return;
                case MagicType.DestructiveSurge:
                    if (CEnvir.Now < ToggleTime) return;
                    ToggleTime = CEnvir.Now.AddSeconds(1);
                    CEnvir.Enqueue(new C.MagicToggle { Magic = magic.Info.Magic, CanUse = !User.CanDestructiveBlow });
                    return;
                case MagicType.FlamingSword:
                case MagicType.MaelstromBlade:
                    {
                        if (CEnvir.Now < magic.NextCast || magic.Cost > User.CurrentMP) return;
                        magic.NextCast = CEnvir.Now.AddSeconds(0.5D);
                        CEnvir.Enqueue(new C.MagicToggle { Magic = magic.Info.Magic });
                        return;
                    }
                case MagicType.DragonRise:
                    {
                        if (CEnvir.Now < magic.NextCast || magic.Cost > User.CurrentMP) return;
                        magic.NextCast = CEnvir.Now.AddSeconds(0.5D);

                        //有目标
                        if (CanAttackTarget(MagicObject))
                            target = MagicObject;

                        if (CanAttackTarget(MouseObject))
                        {
                            target = MouseObject;

                            if (MouseObject.Race == ObjectType.Monster && ((MonsterObject)MouseObject).MonsterInfo.AI >= 0)
                            {
                                MapObject.MagicObject = target;
                            }
                            else if (MouseObject.Race == ObjectType.Player)
                            {
                                MapObject.MagicObject = target;
                            }
                            else
                            {
                                MapObject.MagicObject = null;
                            }
                        }
                        CEnvir.Enqueue(new C.MagicToggle { Magic = magic.Info.Magic });

                        return;
                    }
                case MagicType.BladeStorm:
                case MagicType.DemonicRecovery:
                    if (CEnvir.Now < magic.NextCast || magic.Cost > User.CurrentMP) return;
                    magic.NextCast = CEnvir.Now.AddSeconds(0.5D);
                    CEnvir.Enqueue(new C.MagicToggle { Magic = magic.Info.Magic });
                    return;
                case MagicType.FlameSplash:
                    if (CEnvir.Now < ToggleTime) return;
                    ToggleTime = CEnvir.Now.AddSeconds(1);
                    CEnvir.Enqueue(new C.MagicToggle { Magic = magic.Info.Magic, CanUse = !User.CanFlameSplash });
                    return;
                case MagicType.FullBloom:
                case MagicType.WhiteLotus:
                case MagicType.RedLotus:
                case MagicType.SweetBrier:
                    if (CEnvir.Now < ToggleTime || CEnvir.Now < magic.NextCast) return;

                    if (User.AttackMagic != magic.Info.Magic)
                    {
                        ReceiveChat($"{magic.Info.Name} " + "准备就绪".Lang(), MessageType.Combat);
                        int attackDelay = (int)(CEnvir.ClientControl.GlobalsAttackDelay - MapObject.User.Stats[Stat.AttackSpeed] / 10.0 * CEnvir.ClientControl.GlobalsASpeedRate);
                        attackDelay = Math.Max(100, attackDelay);

                        ToggleTime = CEnvir.Now + TimeSpan.FromMilliseconds(attackDelay + 200);

                        User.AttackMagic = magic.Info.Magic;
                    }
                    return;
                case MagicType.Endurance:
                    if (CEnvir.Now < magic.NextCast || magic.Cost > User.CurrentMP) return;
                    magic.NextCast = CEnvir.Now.AddSeconds(0.5D);
                    CEnvir.Enqueue(new C.MagicToggle { Magic = magic.Info.Magic });
                    return;
                case MagicType.Karma:
                    if (CEnvir.Now < ToggleTime || CEnvir.Now < magic.NextCast || User.Buffs.All(x => x.Type != BuffType.Cloak)) return;

                    if (User.AttackMagic != magic.Info.Magic)
                    {
                        ReceiveChat($"{magic.Info.Name} " + "准备就绪".Lang(), MessageType.Combat);
                        ToggleTime = CEnvir.Now + TimeSpan.FromMilliseconds(500);

                        User.AttackMagic = magic.Info.Magic;
                    }
                    return;

            }

            if (CEnvir.Now < User.NextMagicTime || User.Dead || User.Buffs.Any(x => x.Type == BuffType.DragonRepulse || x.Type == BuffType.FrostBite) ||
                (User.Poison & PoisonType.Paralysis) == PoisonType.Paralysis ||
                (User.Poison & PoisonType.StunnedStrike) == PoisonType.StunnedStrike ||
                (User.Poison & PoisonType.ThousandBlades) == PoisonType.ThousandBlades ||
                (User.Poison & PoisonType.Silenced) == PoisonType.Silenced || (User.Buffs.Any(x => x.Type == BuffType.ElementalHurricane) && magic.Info.Magic != MagicType.ElementalHurricane)) return;

            if (CEnvir.Now < magic.NextCast)
            {
                if (CEnvir.Now >= OutputTime)
                {
                    OutputTime = CEnvir.Now.AddSeconds(1);
                    ReceiveChat($"GameScene.NextMagic".Lang(magic.Info.Name), MessageType.Hint);
                }
                return;
            }

            switch (magic.Info.Magic)
            {
                case MagicType.Cloak:
                    if (User.VisibleBuffs.Contains(BuffType.Cloak)) break;
                    if (CEnvir.Now < User.CombatTime.AddSeconds(10))
                    {
                        if (CEnvir.Now >= OutputTime)
                        {
                            OutputTime = CEnvir.Now.AddSeconds(1);
                            ReceiveChat($"GameScene.NotAvailable".Lang(magic.Info.Name), MessageType.Hint);
                        }
                        return;
                    }

                    if (User.Stats[Stat.Health] * magic.Cost / 1000 >= User.CurrentHP || User.CurrentHP < User.Stats[Stat.Health] / 10)
                    {
                        if (CEnvir.Now >= OutputTime)
                        {
                            OutputTime = CEnvir.Now.AddSeconds(1);
                            ReceiveChat($"GameScene.CurrentHP".Lang(magic.Info.Name), MessageType.Hint);
                        }
                        return;
                    }
                    break;
                case MagicType.DarkConversion:
                    if (User.VisibleBuffs.Contains(BuffType.DarkConversion)) break;

                    if (magic.Cost > User.CurrentMP)
                    {
                        if (CEnvir.Now >= OutputTime)
                        {
                            OutputTime = CEnvir.Now.AddSeconds(1);
                            ReceiveChat($"GameScene.CurrentMP".Lang(magic.Info.Name), MessageType.Hint);
                        }
                        return;
                    }
                    break;
                case MagicType.DragonRepulse:
                    if (User.Stats[Stat.Health] * magic.Cost / 1000 >= User.CurrentHP || User.CurrentHP < User.Stats[Stat.Health] / 10)
                    {
                        if (CEnvir.Now >= OutputTime)
                        {
                            OutputTime = CEnvir.Now.AddSeconds(1);
                            ReceiveChat($"GameScene.CurrentHP".Lang(magic.Info.Name), MessageType.Hint);
                        }
                        return;
                    }
                    if (User.Stats[Stat.Mana] * magic.Cost / 1000 >= User.CurrentMP || User.CurrentMP < User.Stats[Stat.Mana] / 10)
                    {
                        if (CEnvir.Now >= OutputTime)
                        {
                            OutputTime = CEnvir.Now.AddSeconds(1);
                            ReceiveChat($"GameScene.CurrentMP".Lang(magic.Info.Name), MessageType.Hint);
                        }
                        return;
                    }
                    break;
                case MagicType.ElementalHurricane:
                    int cost = magic.Cost;
                    if (MapObject.User.VisibleBuffs.Contains(BuffType.ElementalHurricane))
                        cost = 0;

                    if (cost > User.CurrentMP)
                    {
                        if (CEnvir.Now >= OutputTime)
                        {
                            OutputTime = CEnvir.Now.AddSeconds(1);
                            ReceiveChat($"GameScene.CurrentMP".Lang(magic.Info.Name), MessageType.Hint);
                        }
                        return;
                    }
                    break;
                case MagicType.Concentration:
                    if (User.VisibleBuffs.Contains(BuffType.Concentration)) return;

                    if (magic.Cost > User.CurrentMP)
                    {
                        if (CEnvir.Now >= OutputTime)
                        {
                            OutputTime = CEnvir.Now.AddSeconds(1);
                            ReceiveChat($"GameScene.CurrentMP".Lang(magic.Info.Name), MessageType.Hint);
                        }
                        return;
                    }
                    break;
                default:

                    if (magic.Cost > User.CurrentMP)
                    {
                        if (CEnvir.Now >= OutputTime)
                        {
                            OutputTime = CEnvir.Now.AddSeconds(1);
                            ReceiveChat($"GameScene.CurrentMP".Lang(magic.Info.Name), MessageType.Hint);
                        }
                        return;
                    }
                    break;
            }
            MirDirection direction = MapControl.MouseDirection();

            switch (magic.Info.Magic)
            {
                case MagicType.ShoulderDash:
                    if (CEnvir.Now < User.ServerTime) return;   //如果时间小于当前角色的服务器时间 跳出
                    if ((User.Poison & PoisonType.WraithGrip) == PoisonType.WraithGrip) return; //如果角色毒状态等石化 跳开

                    User.ServerTime = CEnvir.Now.AddSeconds(5);  //角色服务器时间等5秒
                    User.NextMagicTime = CEnvir.Now + TimeSpan.FromMilliseconds(CEnvir.ClientControl.GlobalsMagicDelay);  //角色的下一次释法时间等于系统时间加技能延迟
                    //发包服务端 魔法 方向 攻击类型 技能类型
                    CEnvir.Enqueue(new C.Magic { Direction = direction, Action = MirAction.Spell, Type = magic.Info.Magic });
                    return;
                case MagicType.ReigningStep:
                    if (CEnvir.Now < User.ServerTime) return;   //如果时间小于当前角色的服务器时间 跳出
                    if ((User.Poison & PoisonType.WraithGrip) == PoisonType.WraithGrip) return; //如果角色毒状态等石化 跳开

                    User.ServerTime = CEnvir.Now.AddSeconds(5);  //角色服务器时间等5秒
                    User.NextMagicTime = CEnvir.Now + TimeSpan.FromMilliseconds(CEnvir.ClientControl.GlobalsMagicDelay);  //角色的下一次释法时间等于系统时间加技能延迟
                    //发包服务端 魔法 方向 攻击类型 技能类型
                    CEnvir.Enqueue(new C.Magic { Direction = direction, Action = MirAction.Spell, Type = magic.Info.Magic });
                    return;
                case MagicType.DanceOfSwallow:
                    if (CEnvir.Now < User.ServerTime) return;
                    if (CanAttackTarget(MouseObject))
                        target = MouseObject;

                    //if (target == null) //TODO
                    //target = GetCloseesTarget();

                    if (target == null) return;

                    if (!Functions.InRange(target.CurrentLocation, User.CurrentLocation, Globals.MagicRange))
                    {
                        if (CEnvir.Now < OutputTime) return;
                        OutputTime = CEnvir.Now.AddSeconds(1);
                        ReceiveChat($"GameScene.MagicRange".Lang(magic.Info.Name), MessageType.Hint);
                        return;
                    }

                    User.ServerTime = CEnvir.Now.AddSeconds(5);
                    User.NextMagicTime = CEnvir.Now + TimeSpan.FromMilliseconds(CEnvir.ClientControl.GlobalsMagicDelay);

                    MapObject.TargetObject = target;
                    MapObject.MagicObject = target;

                    CEnvir.Enqueue(new C.Magic { Action = MirAction.Spell, Type = magic.Info.Magic, Target = target.ObjectID });
                    return;
                //这里的魔法是可以锁定的 魔法锁定
                case MagicType.FireStorm:
                case MagicType.LightningWave:
                case MagicType.DragonTornado:
                case MagicType.IceStorm:
                case MagicType.IceRain:
                case MagicType.ChainLightning:
                case MagicType.FireBall:
                case MagicType.IceBolt:
                case MagicType.LightningBall:
                case MagicType.GustBlast:
                case MagicType.ElectricShock:
                case MagicType.AdamantineFireBall:
                case MagicType.ThunderBolt:
                case MagicType.IceBlades:
                case MagicType.Cyclone:
                case MagicType.ExpelUndead:
                case MagicType.Heal:
                case MagicType.ExplosiveTalisman:
                case MagicType.EvilSlayer:
                case MagicType.GreaterEvilSlayer:
                case MagicType.ImprovedExplosiveTalisman:
                case MagicType.Infection:
                case MagicType.Neutralize:
                case MagicType.GreaterHolyStrike:
                case MagicType.PoisonDust:
                    if (BigPatchConfig.AutoPoisonDust && magic.Info.Magic == MagicType.PoisonDust)
                    {
                        int shape = CharacterBox.Grid[(int)EquipmentSlot.Poison].Item?.Info.Shape ?? -1;
                        var needShape = -1;
                        switch (helper.Amulet)
                        {
                            case 0:
                                needShape = 1;
                                break;
                            case 1:
                                needShape = 0;
                                break;
                        }
                        if (needShape != shape || shape == -1)
                        {
                            for (int i = 0; i < Inventory.Length; i++)
                            {
                                if ((Inventory[i]?.Info.ItemType ?? 0) == ItemType.Poison && (Inventory[i]?.Info.Shape ?? -1) != shape && needShape == -1
                                    || (Inventory[i]?.Info.ItemType ?? 0) == ItemType.Poison && (Inventory[i]?.Info.Shape ?? -1) == needShape
                                    )
                                {
                                    DXItemCell[] grid = InventoryBox.Grid.Grid;
                                    CharacterBox.Grid[(int)EquipmentSlot.Poison].ToEquipment(grid[i]);
                                    break;
                                }
                            }
                            shape = CharacterBox.Grid[(int)EquipmentSlot.Poison].Item?.Info.Shape ?? -1;
                            if (shape == -1)
                            {
                                ReceiveChat($"GameScene.AutoPoison".Lang(), MessageType.Hint);
                            }
                        }
                    }
                    //有目标
                    if (CanAttackTarget(MagicObject))
                        target = MagicObject;

                    //如果大补帖不锁定怪物，目标是怪物， 解除锁定
                    if (helper?.LockMonster == false && target != null)
                    {
                        if (target.Race == ObjectType.Monster)
                            target = null;
                    }
                    //如果大补帖不锁定人，目标是人， 解除锁定
                    if (helper?.LockPlayer == false && target != null)
                    {
                        if (target.Race == ObjectType.Player)
                            target = null;
                    }

                    //如果持续魔法 锁怪
                    if (ContinuouslyMagic)
                        target = MagicObject;

                    if (CanAttackTarget(MouseObject))
                    {
                        target = MouseObject;
                        MapObject.MagicObject = target;
                    }

                    //如果挂机，锁死目标
                    if (BigPatchConfig.AndroidPlayer)
                    {
                        if (CanAttackTarget(TargetObject))
                        {
                            target = TargetObject;
                            MapObject.MagicObject = target;
                            break;
                        }
                    }

                    //    if (target == null) //TODO
                    //        ;//target = GetCloseesTarget();
                    break;


                case MagicType.MassHeal:
                case MagicType.MagicResistance:
                case MagicType.Resilience:
                case MagicType.ElementalSuperiority:
                case MagicType.BloodLust:
                case MagicType.Purification:
                case MagicType.MassInvisibility:
                    //如果目标是人 且 没死亡
                    if (MagicObject != null && MagicObject.Race == ObjectType.Player)
                        target = MagicObject;

                    //如果大补帖不锁定怪物，目标是怪物， 解除锁定
                    if (helper?.LockMonster == false && target != null)
                    {
                        if (target.Race == ObjectType.Monster)
                            target = null;
                    }
                    //如果大补帖不锁定人，目标是人， 解除锁定
                    if (helper?.LockPlayer == false && target != null)
                    {
                        if (target.Race == ObjectType.Player)
                            target = null;
                    }

                    if (MouseObject != null && MouseObject.Race == ObjectType.Player)
                    {
                        target = MouseObject;
                        MapObject.MagicObject = target;
                    }

                    if (magic.Info.Magic == MagicType.Heal)
                        target ??= User;
                    else if (magic.Info.Magic == MagicType.Purification)
                    {
                        if (target == null || target.Race != ObjectType.Player)
                        {
                            target = User;
                        }
                    }
                    else if (magic.Info.Magic == MagicType.MassInvisibility || magic.Info.Magic == MagicType.MassHeal || magic.Info.Magic == MagicType.MagicResistance || magic.Info.Magic == MagicType.Resilience || magic.Info.Magic == MagicType.ElementalSuperiority || magic.Info.Magic == MagicType.BloodLust)
                    {
                        if (target == null && !Functions.InRange(MapControl.MapLocation, User.CurrentLocation, Globals.MagicRange))
                        {
                            if (CEnvir.Now < OutputTime) return;
                            OutputTime = CEnvir.Now.AddSeconds(1);
                            ReceiveChat($"GameScene.MagicRange".Lang(magic.Info.Name), MessageType.Hint);
                            return;
                        }
                    }
                    break;
                case MagicType.Resurrection:
                    if (MagicObject != null && MagicObject != User && MagicObject.Dead && MagicObject.Race == ObjectType.Player)
                        target = MagicObject;

                    //如果大补帖不锁定怪物，目标是怪物， 解除锁定
                    if (helper?.LockMonster == false && target != null)
                    {
                        if (target.Race == ObjectType.Monster)
                            target = null;
                    }
                    //如果大补帖不锁定人，目标是人， 解除锁定
                    if (helper?.LockPlayer == false && target != null)
                    {
                        if (target.Race == ObjectType.Player)
                            target = null;
                    }

                    if (MouseObject != null && MouseObject != User && MouseObject.Dead && MouseObject.Race == ObjectType.Player)
                    {
                        target = MouseObject;
                        MapObject.MagicObject = target;
                    }
                    if (target == null || target == User || !target.Dead || target.Race != ObjectType.Player) return;
                    break;

                case MagicType.WraithGrip:
                case MagicType.HellFire:
                case MagicType.Abyss:
                    if (CanAttackTarget(MouseObject))
                        target = MouseObject;

                    //    if (target == null) //TODO
                    //        ;//target = GetCloseesTarget();
                    break;
                case MagicType.Interchange:
                case MagicType.Beckon:
                    if (CanAttackTarget(MouseObject))
                        target = MouseObject;

                    //    if (target == null) //TODO
                    //        ;//target = GetCloseesTarget();
                    break;
                case MagicType.ThousandBlades:
                    if (CanAttackTarget(MouseObject) && Functions.InRange(MouseObject.CurrentLocation, User.CurrentLocation, 1))
                        target = MouseObject;
                    break;
                case MagicType.MassBeckon:
                    break;

                case MagicType.CelestialLight:
                    //if (User.Buffs.Any(x => x.Type == BuffType.CelestialLight)) return;
                    break;
                case MagicType.Defiance:
                    if (User.Buffs.Any(x => x.Type == BuffType.Defiance) || User.Buffs.Any(x => x.Type == BuffType.Might)) return;
                    direction = MirDirection.Down;
                    break;
                case MagicType.Invincibility:
                    direction = MirDirection.Down;
                    break;
                case MagicType.Might:
                    if (User.Buffs.Any(x => x.Type == BuffType.Might) || User.Buffs.Any(x => x.Type == BuffType.Defiance)) return;
                    direction = MirDirection.Down;
                    break;
                case MagicType.ReflectDamage:
                    if (User.Buffs.Any(x => x.Type == BuffType.ReflectDamage)) return;
                    direction = MirDirection.Down;
                    break;
                case MagicType.Fetter:
                    direction = MirDirection.Down;
                    break;
                case MagicType.Renounce:
                    if (User.Buffs.Any(x => x.Type == BuffType.Renounce)) return;
                    break;
                case MagicType.StrengthOfFaith:
                    if (User.Buffs.Any(x => x.Type == BuffType.StrengthOfFaith)) return;
                    break;
                case MagicType.MagicShield:
                    if (User.Buffs.Any(x => x.Type == BuffType.SuperiorMagicShield)) return;
                    break;
                case MagicType.SuperiorMagicShield:
                    if (User.Buffs.Any(x => x.Type == BuffType.SuperiorMagicShield)) return;
                    break;
                case MagicType.FrostBite:
                    if (User.Buffs.Any(x => x.Type == BuffType.FrostBite)) return;
                    break;
                case MagicType.ScortchedEarth:
                case MagicType.LightningBeam:
                case MagicType.FrozenEarth:
                case MagicType.BlowEarth:
                case MagicType.GreaterFrozenEarth:
                case MagicType.ThunderStrike:
                case MagicType.ElementalHurricane:
                case MagicType.TaoistCombatKick:
                case MagicType.ThunderKick:
                case MagicType.SummonDemonicCreature:
                case MagicType.DemonExplosion:
                case MagicType.DarkSoulPrison:
                case MagicType.PoisonousCloud:
                case MagicType.Cloak:
                case MagicType.SummonPuppet:
                case MagicType.SwiftBlade:
                case MagicType.FireWall:
                case MagicType.GeoManipulation:
                case MagicType.LifeSteal:
                case MagicType.MassTransparency:
                case MagicType.TrapOctagon:
                case MagicType.SwordOfVengeance:
                    if (!Functions.InRange(MapControl.MapLocation, User.CurrentLocation, Globals.MagicRange))
                    {
                        if (CEnvir.Now < OutputTime) return;
                        OutputTime = CEnvir.Now.AddSeconds(1);
                        ReceiveChat($"GameScene.MagicRange".Lang(magic.Info.Name), MessageType.Hint);
                        return;
                    }
                    break;

                case MagicType.MeteorShower:
                case MagicType.Tempest:
                case MagicType.Asteroid:
                    if (CanAttackTarget(MagicObject))
                        target = MagicObject;

                    if (target == null && !Functions.InRange(MapControl.MapLocation, User.CurrentLocation, Globals.MagicRange))
                    {
                        if (CEnvir.Now < OutputTime) return;
                        OutputTime = CEnvir.Now.AddSeconds(1);
                        ReceiveChat($"GameScene.MagicRange".Lang(magic.Info.Name), MessageType.Hint);
                        return;
                    }
                    break;
                case MagicType.JudgementOfHeaven:
                case MagicType.SeismicSlam:
                case MagicType.CrushingWave:
                case MagicType.Repulsion:
                case MagicType.Teleportation:
                case MagicType.Invisibility:
                case MagicType.MirrorImage:
                case MagicType.SummonSkeleton:
                case MagicType.SummonShinsu:
                case MagicType.SummonJinSkeleton:
                case MagicType.TheNewBeginning:
                case MagicType.DarkConversion:
                case MagicType.DragonRepulse:
                case MagicType.FlashOfLight:
                case MagicType.Evasion:
                case MagicType.RagingWind:
                case MagicType.Concentration:
                    break;
                case MagicType.Transparency:
                    {
                        //if (User.Buffs.Any(x => x.Type == BuffType.Transparency)) return;

                        //if (!Functions.InRange(MapControl.MapLocation, User.CurrentLocation, Globals.MagicRange))
                        //{
                        //    if (CEnvir.Now < OutputTime) return;
                        //    OutputTime = CEnvir.Now.AddSeconds(1);
                        //    ReceiveChat($"GameScene.MagicRange".Lang(magic.Info.Name), MessageType.Hint);
                        //    return;
                        //}
                    }
                    break;
                default:
                    return;
            }

            if (target != null && !Functions.InRange(target.CurrentLocation, User.CurrentLocation, Globals.MagicRange))
            {
                if (CEnvir.Now < OutputTime) return;
                OutputTime = CEnvir.Now.AddSeconds(1);
                ReceiveChat($"GameScene.MagicRange".Lang(magic.Info.Name), MessageType.Hint);
                return;
            }

            //检查攻击范围
            if (target != null && target != User)
                direction = Functions.DirectionFromPoint(User.CurrentLocation, target.CurrentLocation);

            uint targetID = target?.ObjectID ?? 0;
            Point targetLocation = target?.CurrentLocation ?? MapControl.MapLocation; ;

            //switch (magic.Info.Magic)
            //{
            //    case MagicType.Purification:
            //    case MagicType.EvilSlayer:
            //    case MagicType.GreaterEvilSlayer:
            //    case MagicType.ExplosiveTalisman:
            //    case MagicType.ImprovedExplosiveTalisman:
            //    case MagicType.PoisonDust:
            //    case MagicType.Neutralize:
            //    case MagicType.GreaterHolyStrike:
            //        targetLocation = MapControl.MapLocation;
            //        break;
            //    default:
            //        targetLocation = target?.CurrentLocation ?? MapControl.MapLocation;
            //        break;
            //}

            //切换拼写类型
            if (MouseObject != null && MouseObject.Race == ObjectType.Monster)
                FocusObject = (MonsterObject)MouseObject;

            //魔法攻击元素
            Element element = Element.None;
            switch (magic.Info.Magic)
            {
                case MagicType.ElementalSuperiority:
                    if (Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.Amulet)
                    {
                        foreach (KeyValuePair<Stat, int> stat in Equipment[(int)EquipmentSlot.Amulet].Info.Stats.Values)
                        {
                            switch (stat.Key)
                            {
                                case Stat.FireAffinity:
                                    element = Element.Fire;
                                    break;
                                case Stat.IceAffinity:
                                    element = Element.Ice;
                                    break;
                                case Stat.LightningAffinity:
                                    element = Element.Lightning;
                                    break;
                                case Stat.WindAffinity:
                                    element = Element.Wind;
                                    break;
                                case Stat.HolyAffinity:
                                    element = Element.Holy;
                                    break;
                                case Stat.DarkAffinity:
                                    element = Element.Dark;
                                    break;
                                case Stat.PhantomAffinity:
                                    element = Element.Phantom;
                                    break;
                            }
                        }
                    }
                    break;
            }

            User.MagicAction = new ObjectAction(MirAction.Spell, direction, MapObject.User.CurrentLocation, magic.Info.Magic, new List<uint> { targetID }, new List<Point> { targetLocation }, false, element);

        }

        public SelectPlayerType SelectPlayerType = SelectPlayerType.NeerBy;
        public void SetTargetObject(ObjectType type)
        {
            //if (type == ObjectType.Player && SelectPlayerType == SelectPlayerType.AttackBack)
            //{
            //    TargetObject = User.LastAttackPlayer;
            //    MagicObject = User.LastAttackPlayer;
            //    MouseObject = User.LastAttackPlayer;
            //    return;
            //}

            MapObject ob = null;
            int minRange = CEnvir.ClientControl.MaxViewRange - 1;
            int minHp = int.MaxValue;
            foreach (MapObject obj in MapControl.Objects)
            {
                if (obj.Dead || obj.Race != type || !string.IsNullOrEmpty(obj.PetOwner)) continue;
                if (obj.Race == ObjectType.Monster && ((MonsterObject)obj).MonsterInfo.AI < 0) continue;
                if (obj.Race == ObjectType.Player && ((PlayerObject)obj == User || (User.AttackMode == AttackMode.Group && GroupMemberBox.GroupMembers.Any(x => x.ObjectID == obj.ObjectID))
                                                                               || (User.AttackMode == AttackMode.Guild && GuildBox.GuildInfo.Members.Any(x => x.ObjectID == obj.ObjectID))
                                                                               || (User.AttackMode == AttackMode.WarRedBrown && GuildBox.GuildInfo.Members.Any(x => x.ObjectID == obj.ObjectID)))) continue;

                if (type == ObjectType.Player)
                {
                    switch (SelectPlayerType)
                    {
                        case SelectPlayerType.NeerBy:
                            int dis = Functions.Distance(User.CurrentLocation, obj.CurrentLocation);
                            if (dis <= minRange && obj != TargetObject)
                            {
                                ob = obj;
                                minRange = dis;
                            }
                            break;
                        case SelectPlayerType.MinBlood:
                            int hp = obj.CurrentHP;
                            if (hp <= minHp && obj != TargetObject)
                            {
                                ob = obj;
                                minHp = hp;
                            }
                            break;
                    }
                }
                else
                {
                    int dis = Functions.Distance(User.CurrentLocation, obj.CurrentLocation);
                    if (dis <= minRange && obj != TargetObject)
                    {
                        ob = obj;
                        minRange = dis;
                    }
                }
            }
            if (ob != null)
            {
                TargetObject = ob;
                MagicObject = ob;
                MouseObject = ob;
            }
        }

        public void SetMagicTargetObject(ObjectType type, int PoisonDustType = -1)
        {
            //if (type == ObjectType.Player && SelectPlayerType == SelectPlayerType.AttackBack)
            //{
            //    MagicObject = User.LastAttackPlayer;
            //    MouseObject = User.LastAttackPlayer;
            //    return;
            //}

            MapObject ob = null;
            int minRange = Globals.MagicRange - 1;
            int minHp = int.MaxValue;
            foreach (MapObject obj in MapControl.Objects)
            {
                if (obj.Dead || obj.Race != type || !string.IsNullOrEmpty(obj.PetOwner)) continue;
                if (obj.Race == ObjectType.Monster && (((MonsterObject)obj).MonsterInfo.AI < 0 || ((MonsterObject)obj).MonsterInfo.AI == 4)) continue;
                if (obj.Race == ObjectType.Player && ((PlayerObject)obj == User || (User.AttackMode == AttackMode.Group && GroupMemberBox.GroupMembers.Any(x => x.ObjectID == obj.ObjectID))
                                                                                || (User.AttackMode == AttackMode.Guild && GuildBox.GuildInfo.Members.Any(x => x.ObjectID == obj.ObjectID))
                                                                                || (User.AttackMode == AttackMode.WarRedBrown && GuildBox.GuildInfo.Members.Any(x => x.ObjectID == obj.ObjectID)))) continue;
                if ((PoisonDustType == 0 && (obj.Poison & PoisonType.Green) == PoisonType.Green) || (PoisonDustType == 1 && (obj.Poison & PoisonType.Red) == PoisonType.Red)) continue;

                if (type == ObjectType.Player)
                {
                    switch (SelectPlayerType)
                    {
                        case SelectPlayerType.NeerBy:
                            int dis = Functions.Distance(User.CurrentLocation, obj.CurrentLocation);
                            if (dis <= minRange && obj != MagicObject)
                            {
                                ob = obj;
                                minRange = dis;
                            }
                            break;
                        case SelectPlayerType.MinBlood:
                            int hp = obj.CurrentHP;
                            if (hp <= minHp && obj != MagicObject)
                            {
                                ob = obj;
                                minHp = hp;
                            }
                            break;
                    }
                }
                else
                {
                    int dis = Functions.Distance(User.CurrentLocation, obj.CurrentLocation);
                    if (dis <= minRange && obj != MagicObject)
                    {
                        ob = obj;
                        minRange = dis;
                    }
                }

            }
            if (ob != null)
            {
                MagicObject = ob;
                MouseObject = ob;
            }
        }

        #endregion


        #region 手游

        public StickMode StickMode = StickMode.Walk; //默认跑
        //public bool DrawStick;
        public bool IsStickAvailable()
        {
            if (TradeBox.Visible || DXItemCell.SelectedCell != null || MouseControl != MapControl)
            {
                MapControl?.SetStick(0f, 0f);
                return false;
            }
            return true;
        }

        #endregion


        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            //退出保存用户配置文件
            if (_User != null)
                CConfigReader.Save(_User.Name, _User.CharacterIndex);

            base.Dispose(disposing);

            if (disposing)
            {
                if (Game == this) Game = null;

                _SelectedCell = null;

                _User = null;
                _MouseItem = null;
                _MouseMagic = null;

                GoldPickedUp = false;

                MagicObject = null;
                MouseObject = null;
                TargetObject = null;
                FocusObject = null;

                if (ItemLabel != null)
                {
                    if (!ItemLabel.IsDisposed)
                        ItemLabel.Dispose();

                    ItemLabel = null;
                }

                if (MagicLabel != null)
                {
                    if (!MagicLabel.IsDisposed)
                        MagicLabel.Dispose();

                    MagicLabel = null;
                }


                if (MapControl != null)
                {
                    if (!MapControl.IsDisposed)
                        MapControl.Dispose();

                    MapControl = null;
                }

                //if (MainPanel != null)
                //{
                //    if (!MainPanel.IsDisposed)
                //        MainPanel.Dispose();

                //    MainPanel = null;
                //}

                if (ConfigBox != null)
                {
                    if (!ConfigBox.IsDisposed)
                        ConfigBox.Dispose();

                    ConfigBox = null;
                }

                if (InventoryBox != null)
                {
                    if (!InventoryBox.IsDisposed)
                        InventoryBox.Dispose();

                    InventoryBox = null;
                }

                if (InventoryJueSeBox != null)
                {
                    if (!InventoryJueSeBox.IsDisposed)
                        InventoryJueSeBox.Dispose();

                    InventoryJueSeBox = null;
                }

                if (CharacterBox != null)
                {
                    if (!CharacterBox.IsDisposed)
                        CharacterBox.Dispose();

                    CharacterBox = null;
                }

                if (ExitBox != null)
                {
                    if (!ExitBox.IsDisposed)
                        ExitBox.Dispose();

                    ExitBox = null;
                }

                if (ChatTextBox != null)
                {
                    if (!ChatTextBox.IsDisposed)
                        ChatTextBox.Dispose();

                    ChatTextBox = null;
                }

                if (ChatTabBox != null)
                {
                    if (!ChatTabBox.IsDisposed)
                        ChatTabBox.Dispose();

                    ChatTabBox = null;
                }

                if (BeltBox != null)
                {
                    if (!BeltBox.IsDisposed)
                        BeltBox.Dispose();

                    BeltBox = null;
                }

                if (NPCBox != null)
                {
                    if (!NPCBox.IsDisposed)
                        NPCBox.Dispose();

                    NPCBox = null;
                }

                if (NPCGoodsBox != null)
                {
                    if (!NPCGoodsBox.IsDisposed)
                        NPCGoodsBox.Dispose();

                    NPCGoodsBox = null;
                }

                if (NPCSellBox != null)
                {
                    if (!NPCSellBox.IsDisposed)
                        NPCSellBox.Dispose();

                    NPCSellBox = null;
                }

                if (NPCRootSellBox != null)
                {
                    if (!NPCRootSellBox.IsDisposed)
                        NPCRootSellBox.Dispose();

                    NPCRootSellBox = null;
                }

                if (NPCRefinementStoneBox != null)
                {
                    if (!NPCRefinementStoneBox.IsDisposed)
                        NPCRefinementStoneBox.Dispose();

                    NPCRefinementStoneBox = null;
                }

                if (NPCRepairBox != null)
                {
                    if (!NPCRepairBox.IsDisposed)
                        NPCRepairBox.Dispose();

                    NPCRepairBox = null;
                }

                if (NPCSpecialRepairBox != null)
                {
                    if (!NPCSpecialRepairBox.IsDisposed)
                        NPCSpecialRepairBox.Dispose();

                    NPCSpecialRepairBox = null;
                }

                if (NPCRefineBox != null)
                {
                    if (!NPCRefineBox.IsDisposed)
                        NPCRefineBox.Dispose();

                    NPCRefineBox = null;
                }

                if (NPCRefineRetrieveBox != null)
                {
                    if (!NPCRefineRetrieveBox.IsDisposed)
                        NPCRefineRetrieveBox.Dispose();

                    NPCRefineRetrieveBox = null;
                }
                if (NPCMasterRefineBox != null)
                {
                    if (!NPCMasterRefineBox.IsDisposed)
                        NPCMasterRefineBox.Dispose();

                    NPCMasterRefineBox = null;
                }

                if (NPCQuestListBox != null)
                {
                    if (!NPCQuestListBox.IsDisposed)
                        NPCQuestListBox.Dispose();

                    NPCQuestListBox = null;
                }

                if (NPCQuestBox != null)
                {
                    if (!NPCQuestBox.IsDisposed)
                        NPCQuestBox.Dispose();

                    NPCQuestBox = null;
                }

                if (NPCAdoptCompanionBox != null)
                {
                    if (!NPCAdoptCompanionBox.IsDisposed)
                        NPCAdoptCompanionBox.Dispose();

                    NPCAdoptCompanionBox = null;
                }

                if (NPCCompanionStorageBox != null)
                {
                    if (!NPCCompanionStorageBox.IsDisposed)
                        NPCCompanionStorageBox.Dispose();

                    NPCCompanionStorageBox = null;
                }

                if (NPCWeddingRingBox != null)
                {
                    if (!NPCWeddingRingBox.IsDisposed)
                        NPCWeddingRingBox.Dispose();

                    NPCWeddingRingBox = null;
                }

                if (NPCItemFragmentBox != null)
                {
                    if (!NPCItemFragmentBox.IsDisposed)
                        NPCItemFragmentBox.Dispose();

                    NPCItemFragmentBox = null;
                }
                if (NPCAccessoryUpgradeBox != null)
                {
                    if (!NPCAccessoryUpgradeBox.IsDisposed)
                        NPCAccessoryUpgradeBox.Dispose();

                    NPCAccessoryUpgradeBox = null;
                }
                if (NPCAccessoryLevelBox != null)
                {
                    if (!NPCAccessoryLevelBox.IsDisposed)
                        NPCAccessoryLevelBox.Dispose();

                    NPCAccessoryLevelBox = null;
                }
                if (NPCAccessoryResetBox != null)
                {
                    if (!NPCAccessoryResetBox.IsDisposed)
                        NPCAccessoryResetBox.Dispose();

                    NPCAccessoryResetBox = null;
                }

                if (NPCWeaponUpgradeBox != null)
                {
                    if (!NPCWeaponUpgradeBox.IsDisposed)
                        NPCWeaponUpgradeBox.Dispose();

                    NPCWeaponUpgradeBox = null;
                }

                if (NPCAccessoryCombineBox != null)
                {
                    if (!NPCAccessoryCombineBox.IsDisposed)
                        NPCAccessoryCombineBox.Dispose();

                    NPCAccessoryCombineBox = null;
                }

                if (NPCAncientTombstoneBox != null)
                {
                    if (!NPCAncientTombstoneBox.IsDisposed)
                        NPCAncientTombstoneBox.Dispose();

                    NPCAncientTombstoneBox = null;
                }

                if (NPCBuyBackBox != null)
                {
                    if (!NPCBuyBackBox.IsDisposed)
                        NPCBuyBackBox.Dispose();

                    NPCBuyBackBox = null;
                }

                if (MiniMapBox != null)
                {
                    if (!MiniMapBox.IsDisposed)
                        MiniMapBox.Dispose();

                    MiniMapBox = null;
                }

                if (BigMapBox != null)
                {
                    if (!BigMapBox.IsDisposed)
                        BigMapBox.Dispose();

                    BigMapBox = null;
                }

                if (MagicBox != null)
                {
                    if (!MagicBox.IsDisposed)
                        MagicBox.Dispose();

                    MagicBox = null;
                }

                if (MagicJueSeBox != null)
                {
                    if (!MagicJueSeBox.IsDisposed)
                        MagicJueSeBox.Dispose();

                    MagicJueSeBox = null;
                }

                if (GroupBox != null)
                {
                    if (!GroupBox.IsDisposed)
                        GroupBox.Dispose();

                    GroupBox = null;
                }

                if (BuffBox != null)
                {
                    if (!BuffBox.IsDisposed)
                        BuffBox.Dispose();

                    BuffBox = null;
                }

                if (StorageBox != null)
                {
                    if (!StorageBox.IsDisposed)
                        StorageBox.Dispose();

                    StorageBox = null;
                }

                if (BigPatchBox != null)
                {
                    if (!BigPatchBox.IsDisposed)
                        BigPatchBox.Dispose();

                    BigPatchBox = null;
                }

                if (InspectBox != null)
                {
                    if (!InspectBox.IsDisposed)
                        InspectBox.Dispose();

                    InspectBox = null;
                }

                if (RankingBox != null)
                {
                    if (!RankingBox.IsDisposed)
                        RankingBox.Dispose();

                    RankingBox = null;
                }

                if (MarketPlaceBox != null)
                {
                    if (!MarketPlaceBox.IsDisposed)
                        MarketPlaceBox.Dispose();

                    MarketPlaceBox = null;
                }

                if (MarketSearchBox != null)
                {
                    if (!MarketSearchBox.IsDisposed)
                        MarketSearchBox.Dispose();

                    MarketSearchBox = null;
                }

                if (MarketConsignBox != null)
                {
                    if (!MarketConsignBox.IsDisposed)
                        MarketConsignBox.Dispose();

                    MarketConsignBox = null;
                }

                if (GameGoldMarketConsignBox != null)
                {
                    if (!GameGoldMarketConsignBox.IsDisposed)
                        GameGoldMarketConsignBox.Dispose();

                    GameGoldMarketConsignBox = null;
                }

                if (MyMarketBox != null)
                {
                    if (!MyMarketBox.IsDisposed)
                        MyMarketBox.Dispose();

                    MyMarketBox = null;
                }

                if (AccountConsignmentBox != null)
                {
                    if (!AccountConsignmentBox.IsDisposed)
                        AccountConsignmentBox.Dispose();

                    AccountConsignmentBox = null;
                }

                if (AuctionsBox != null)
                {
                    if (!AuctionsBox.IsDisposed)
                        AuctionsBox.Dispose();

                    AuctionsBox = null;
                }

                if (AuctionsConsignBox != null)
                {
                    if (!AuctionsConsignBox.IsDisposed)
                        AuctionsConsignBox.Dispose();

                    AuctionsConsignBox = null;
                }

                if (ReadMailBox != null)
                {
                    if (!ReadMailBox.IsDisposed)
                        ReadMailBox.Dispose();

                    ReadMailBox = null;
                }

                if (TradeBox != null)
                {
                    if (!TradeBox.IsDisposed)
                        TradeBox.Dispose();

                    TradeBox = null;
                }

                if (GuildBox != null)
                {
                    if (!GuildBox.IsDisposed)
                        GuildBox.Dispose();

                    GuildBox = null;
                }

                if (GuildMemberBox != null)
                {
                    if (!GuildMemberBox.IsDisposed)
                        GuildMemberBox.Dispose();

                    GuildMemberBox = null;
                }

                if (QuestBox != null)
                {
                    if (!QuestBox.IsDisposed)
                        QuestBox.Dispose();

                    QuestBox = null;
                }

                if (QuestTrackerBox != null)
                {
                    if (!QuestTrackerBox.IsDisposed)
                        QuestTrackerBox.Dispose();

                    QuestTrackerBox = null;
                }

                if (NPCReplicaBox != null)
                {
                    if (!NPCReplicaBox.IsDisposed)
                        NPCReplicaBox.Dispose();

                    NPCReplicaBox = null;
                }

                if (CompanionBox != null)
                {
                    if (!CompanionBox.IsDisposed)
                        CompanionBox.Dispose();

                    CompanionBox = null;
                }

                if (PickUpSettingsS != null)
                {
                    if (!PickUpSettingsS.IsDisposed)
                        PickUpSettingsS.Dispose();

                    PickUpSettingsS = null;
                }

                if (MonsterBox != null)
                {
                    if (!MonsterBox.IsDisposed)
                        MonsterBox.Dispose();

                    MonsterBox = null;
                }

                if (MagicBarBox != null)
                {
                    if (!MagicBarBox.IsDisposed)
                        MagicBarBox.Dispose();

                    MagicBarBox = null;
                }

                if (GroupMemberBox != null)
                {
                    if (!GroupMemberBox.IsDisposed)
                        GroupMemberBox.Dispose();

                    GroupMemberBox = null;
                }


                if (FixedPointBox != null)
                {
                    if (!FixedPointBox.IsDisposed)
                        FixedPointBox.Dispose();

                    FixedPointBox = null;
                }

                if (VowBox != null)
                {
                    if (!VowBox.IsDisposed)
                        VowBox.Dispose();

                    VowBox = null;
                }

                if (ChatNoticeBox != null)
                {
                    if (!ChatNoticeBox.IsDisposed)
                        ChatNoticeBox.Dispose();

                    ChatNoticeBox = null;
                }

                if (TopInfoBox != null)
                {
                    if (!TopInfoBox.IsDisposed)
                        TopInfoBox.Dispose();

                    TopInfoBox = null;
                }

                if (CommunicationBox != null)
                {
                    if (!CommunicationBox.IsDisposed)
                        CommunicationBox.Dispose();

                    CommunicationBox = null;
                }

                if (BonusPoolBox != null)
                {
                    if (!BonusPoolBox.IsDisposed)
                        BonusPoolBox.Dispose();

                    BonusPoolBox = null;
                }

                if (BonusPoolVersionBox != null)
                {
                    if (!BonusPoolVersionBox.IsDisposed)
                        BonusPoolVersionBox.Dispose();

                    BonusPoolVersionBox = null;
                }

#if Mobile
#else
                if (GuildFlag != null)
                {
                    if (!GuildFlag.IsDisposed)
                        GuildFlag.Dispose();

                    GuildFlag = null;
                }
#endif

                if (GoldTradingBusinessBox != null)
                {
                    if (!GoldTradingBusinessBox.IsDisposed)
                        GoldTradingBusinessBox.Dispose();

                    GoldTradingBusinessBox = null;
                }

                if (WarWeaponBox != null)
                {
                    if (!WarWeaponBox.IsDisposed)
                        WarWeaponBox.Dispose();

                    WarWeaponBox = null;
                }

#if Mobile
                //if (RightButtonsPanel != null)
                //{
                //    if (!RightButtonsPanel.IsDisposed)
                //        RightButtonsPanel.Dispose();

                //    RightButtonsPanel = null;
                //}
                //if (LeftDownButtonsPanel != null)
                //{
                //    if (!LeftDownButtonsPanel.IsDisposed)
                //        LeftDownButtonsPanel.Dispose();

                //    LeftDownButtonsPanel = null;
                //}
                if (LeftCentButtonsPanel != null)
                {
                    if (!LeftCentButtonsPanel.IsDisposed)
                        LeftCentButtonsPanel.Dispose();

                    LeftCentButtonsPanel = null;
                }
                if (UserFunctionControl != null)
                {
                    if (!UserFunctionControl.IsDisposed)
                        UserFunctionControl.Dispose();

                    UserFunctionControl = null;
                }
                if (LeftUpUserPanel != null)
                {
                    if (!LeftUpUserPanel.IsDisposed)
                        LeftUpUserPanel.Dispose();

                    LeftUpUserPanel = null;
                }

                if (TownReviveButton != null)
                {
                    if (!TownReviveButton.IsDisposed)
                        TownReviveButton.Dispose();

                    TownReviveButton = null;
                }
                VirtualWalkStick.TryDispose();
                if (DownCentPanel != null)
                {
                    if (!DownCentPanel.IsDisposed)
                        DownCentPanel.Dispose();

                    DownCentPanel = null;
                }
#endif

                Inventory = null;
                Equipment = null;
                QuestLog = null;
                AchievementLog.Clear();
                AchievementLog = null;

                DataDictionary.Clear();
                DataDictionary = null;

                MoveFrame = false;
                MoveTime = DateTime.MinValue;
                OutputTime = DateTime.MinValue;
                ItemRefreshTime = DateTime.MinValue;

                CanPush = false;
                CanRun = false;
                _AutoRun = false;
                _NPCID = 0;
                _Companion = null;
                _Partner = null;

                PickUpTime = DateTime.MinValue;
                UseItemTime = DateTime.MinValue;
                NPCTime = DateTime.MinValue;
                ToggleTime = DateTime.MinValue;
                InspectTime = DateTime.MinValue;
                ItemTime = DateTime.MinValue;
                ItemReviveTime = DateTime.MinValue;

                _DayTime = 0f;
            }
        }
        #endregion
    }
}
