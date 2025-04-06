using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using Server.Envir;
using Server.Models;
using S = Library.Network.ServerPackets;
using Library.Network.GeneralPackets;
using System.Reflection;
using Server.DBModels;
using System.Collections.Generic;
using System.Linq;
using MirDB;

namespace Server.Views
{
    public partial class ConfigView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ConfigView()          //设置界面视图
        {
            InitializeComponent();

            //MysteryShipRegionIndexEdit.Properties.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            //LairRegionIndexEdit.Properties.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;

            //PenetraliumKeyAEdit.Properties.DataSource = SMain.Session.GetCollection<QuestInfo>().Binding;
            //PenetraliumKeyBEdit.Properties.DataSource = SMain.Session.GetCollection<QuestInfo>().Binding;
            //PenetraliumKeyCEdit.Properties.DataSource = SMain.Session.GetCollection<QuestInfo>().Binding;

            //RightDeliverEdit.Properties.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            //ErrorDeliverEdit.Properties.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;

            //gridControl_GoldBuySet.DataSource = SMain.Session.GetCollection<GameGoldSet>().Binding;

            //PlayerExpGridControl.DataSource = SMain.Session.GetCollection<PlayerExpInfo>().Binding;   //经验列表
            //WeaponExpGridControl.DataSource = SMain.Session.GetCollection<WeaponExpInfo>().Binding;   //武器经验列表
            //AccessoryExpGridControl.DataSource = SMain.Session.GetCollection<AccessoryExpInfo>().Binding;  //首饰经验列表
            //CraftExpGridControl.DataSource = SMain.Session.GetCollection<CraftExpInfo>().Binding;     //制作经验列表
            //GuildLeveLExpGridControl.DataSource = SMain.Session.GetCollection<GuildLevelExp>().Binding;     //行会经验列表
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
            //Network
            ClientNameEdit.EditValue = Config.ClientName;  //客户端名字设置
            WebSiteEdit.EditValue = Config.WebSite;    //网站地址设置
            IPAddressEdit.EditValue = Config.IPAddress;
            PortEdit.EditValue = Config.Port;
            TimeOutEdit.EditValue = Config.TimeOut;
            PingDelayEdit.EditValue = Config.PingDelay;
            UserCountPortEdit.EditValue = Config.UserCountPort;
            MaxPacketEdit.EditValue = Config.MaxPacket;
            PacketBanTimeEdit.EditValue = Config.PacketBanTime;
            MaxConnectionsPerIpEdit.EditValue = Config.MaxConnectionsPerIp;
            MySqlEnableToggleSwitch.IsOn = Config.MySqlEnable;
            MySqlUserEdit.EditValue = Config.MySqlUser;
            MySqlPassEdit.EditValue = Config.MySqlPassword;
            MySqlIPEdit.EditValue = Config.MySqlIP;
            MySqlPortEdit.EditValue = Config.MySqlPort;

            //Control
            AllowNewAccountEdit.EditValue = Config.AllowNewAccount;
            AllowChangePasswordEdit.EditValue = Config.AllowChangePassword;
            AllowLoginEdit.EditValue = Config.AllowLogin;
            AllowNewCharacterEdit.EditValue = Config.AllowNewCharacter;
            AllowDeleteCharacterEdit.EditValue = Config.AllowDeleteCharacter;
            AllowStartGameEdit.EditValue = Config.AllowStartGame;
            AllowWarriorEdit.EditValue = Config.AllowWarrior;
            AllowWizardEdit.EditValue = Config.AllowWizard;
            AllowTaoistEdit.EditValue = Config.AllowTaoist;
            AllowAssassinEdit.EditValue = Config.AllowAssassin;
            RelogDelayEdit.EditValue = Config.RelogDelay;
            AllowRequestPasswordResetEdit.EditValue = Config.AllowRequestPasswordReset;
            AllowWebResetPasswordEdit.EditValue = Config.AllowWebResetPassword;
            AllowManualResetPasswordEdit.EditValue = Config.AllowManualResetPassword;
            AllowDeleteAccountEdit.EditValue = Config.AllowDeleteAccount;
            AllowManualActivationEdit.EditValue = Config.AllowManualActivation;
            AllowWebActivationEdit.EditValue = Config.AllowWebActivation;
            AllowRequestActivationEdit.EditValue = Config.AllowRequestActivation;
            UseInviteCodeCheckEdit.EditValue = Config.UseInviteCode;

            //System
            CheckVersionEdit.EditValue = Config.CheckVersion;
            VersionPathEdit.EditValue = Config.VersionPath;
            VersionPath1Edit.EditValue = Config.VersionPath1;
            DBSaveDelayEdit.EditValue = Config.DBSaveDelay;
            MapPathEdit.EditValue = Config.MapPath;
            MasterPasswordEdit.EditValue = Config.MasterPassword;
            MasterPasswordSwitchCheckEdit.EditValue = Config.MasterPasswordSwitch;
            ClientPathEdit.EditValue = Config.ClientPath;
            ReleaseDateEdit.EditValue = Config.ReleaseDate;
            RabbitEventEndEdit.EditValue = Config.EasterEventEnd;
            NewLevelEdit.EditValue = Config.NewLevel;
            NewGoldEdit.EditValue = Config.NewGold;
            NewGameGoldEdit.EditValue = Config.NewGameGold;
            NewPrestigeEdit.EditValue = Config.NewPrestige;
            NewContributeEdit.EditValue = Config.NewContribute;
            EnvirTickCountEdit.EditValue = Config.EnvirTickCount;
            TestServerCheckEdit.EditValue = Config.TestServer;
            checkAutoCloseServer.EditValue = false;
            TimeAutoCloseServer.EditValue = Config.TimeAutoCloseServer;
            CloseServerTimeEdit.EditValue = Config.CloseServerTime;
            CheckPhoneVersionCheckEdit.EditValue = Config.CheckPhoneVersion;
            PhoneVersionNumberTextEdit.EditValue = Config.PhoneVersionNumber;

            //Mail
            MailActivate.Checked = Config.MailActivate;
            MailServerEdit.EditValue = Config.MailServer;
            MailPortEdit.EditValue = Config.MailPort;
            MailUseSSLEdit.EditValue = Config.MailUseSSL;
            MailAccountEdit.EditValue = Config.MailAccount;
            MailPasswordEdit.EditValue = Config.MailPassword;
            MailFromEdit.EditValue = Config.MailFrom;
            MailDisplayNameEdit.EditValue = Config.MailDisplayName;

            //WebServer
            WebPrefixEdit.EditValue = Config.WebPrefix;
            WebCommandLinkEdit.EditValue = Config.WebCommandLink;
            ActivationSuccessLinkEdit.EditValue = Config.ActivationSuccessLink;
            ActivationFailLinkEdit.EditValue = Config.ActivationFailLink;
            ResetSuccessLinkEdit.EditValue = Config.ResetSuccessLink;
            ResetFailLinkEdit.EditValue = Config.ResetFailLink;
            DeleteSuccessLinkEdit.EditValue = Config.DeleteSuccessLink;
            DeleteFailLinkEdit.EditValue = Config.DeleteFailLink;

            BuyPrefixEdit.EditValue = Config.BuyPrefix;
            BuyAddressEdit.EditValue = Config.BuyAddress;
            RechargeInterfaceCheck.EditValue = Config.RechargeInterface;
            IPNPrefixEdit.EditValue = Config.IPNPrefix;
            ReceiverEMailEdit.EditValue = Config.ReceiverEMail;
            ProcessGameGoldEdit.EditValue = Config.ProcessGameGold;
            AllowBuyGammeGoldEdit.EditValue = Config.AllowBuyGammeGold;
            RequireActivationEdit.EditValue = Config.RequireActivation;
            CanUseChineseName.EditValue = Config.CanUseChineseName;
            CanUseChineseGuildName.EditValue = Config.CanUseChineseGuildName;

            //Players
            MaxViewRangeEdit.EditValue = Config.MaxViewRange;
            MaxNPCViewRangeTextEdit.EditValue = Config.MaxNPCViewRange;
            ShoutDelayEdit.EditValue = Config.ShoutDelay;
            GlobalDelayEdit.EditValue = Config.GlobalDelay;
            MaxLevelEdit.EditValue = Config.MaxLevel;
            MaxLevelLimitCheckEdit.EditValue = Config.MaxLevelLimit;
            DayCycleCountEdit.EditValue = Config.DayCycleCount;

            AllowObservationEdit.EditValue = Config.AllowObservation;
            ObservedCountTextEdit.EditValue = Config.ObservedCount;
            BrownDurationEdit.EditValue = Config.BrownDuration;
            PKPointRateEdit.EditValue = Config.PKPointRate;
            PKPointTickRateEdit.EditValue = Config.PKPointTickRate;
            RedPointEdit.EditValue = Config.RedPoint;
            PvPCurseDurationEdit.EditValue = Config.PvPCurseDuration;
            PvPCurseRateEdit.EditValue = Config.PvPCurseRate;
            PVPLuckCheckEdit.EditValue = Config.PVPLuckCheck;
            AutoReviveDelayEdit.EditValue = Config.AutoReviveDelay;
            AllowStorageEdit.EditValue = Config.Storage;
            RandomAssigncheckEdit.EditValue = Config.RandomAssign;  //随机加点
            AssignHermitMinACMRCheckEdit.EditValue = Config.AssignHermitMinACMR;  //额外属性加上限
            PartnerDeadTeleportCheckEdit.EditValue = Config.PartnerDeadTeleport;  //夫妻传送对方死亡能否传送
            MarriageTeleportDelayEdit.EditValue = Config.MarriageTeleportDelay;  //夫妻传送延迟
            MarriageTeleportLocation.EditValue = Config.MarriageTeleportLocation;  //夫妻传送范围
            GroupRecallTimeEdit.EditValue = Config.GroupRecallTime;  //天地合一冷却时间
            ShowSafezoneEdit.EditValue = Config.ShowSafeZone;   //显示安全区
            DistinctionRepairEdit.EditValue = Config.DistinctionRepair;   //区别维修
            DailyQuestEdit.EditValue = Config.DailyQuestLimit;  //每日任务次数
            RepeatableQuestEdit.EditValue = Config.RepeatableQuestLimit;  //可重复任务次数
            ShortcutEnabledEdit.EditValue = Config.ShortcutEnabled;   //顶部UI图标
            BUFFInSafeZoneEdit.EditValue = Config.SafeZoneBuffPause;    //BUFF安全区暂停
            OfflineBuffTickingCheckEdit.EditValue = Config.OfflineBuffTicking; //buff离线继续计时
            CriticalDamagePVPEdit.EditValue = Config.CriticalDamagePVP;  //PVP暴击伤害%
            RankingLevelTextEdit.EditValue = Config.RankingLevel;  //排行称号显示等级
            WarsTimeTextEdit.EditValue = Config.WarsTime;   //沙巴克攻城时间
            WarsFlagTimeEdit.EditValue = Config.FlagCaptureTime;  //沙巴克夺旗时间            
            DailyFreeCoinsEdit.EditValue = Config.DailyFreeCoins;
            CoinPlaceChoiceCheckEdit.EditValue = Config.CoinPlaceChoice;
            TownReviveHPRate.EditValue = Config.TownReviveHPRate;
            TownReviveMPRate.EditValue = Config.TownReviveMPRate;
            GoldChangeAlertTextEdit.EditValue = Config.GoldChangeAlert;
            WizardMCRateTextEdit.EditValue = Config.WizardMCRate;
            TaoistSCRateTextEdit.EditValue = Config.TaoistSCRate;
            WizardMagicAttackRateTextEdit.EditValue = Config.WizardMagicAttackRate;
            TaoistMagicAttackRateTextEdit.EditValue = Config.TaoistMagicAttackRate;
            ClearOverMarketTextEdit.EditValue = Config.ClearOverMarket;
            LevelReductionDamage.EditValue = Config.LevelReductionDamage;
            CompareLevelValues.EditValue = Config.CompareLevelValues;
            WarriorReductionDamage.EditValue = Config.WarriorReductionDamage;

            //Monsters
            DeadDurationEdit.EditValue = Config.DeadDuration;
            HarvestDurationEdit.EditValue = Config.HarvestDuration;
            MysteryShipRegionIndexEdit.EditValue = Config.MysteryShipRegionIndex;
            LairRegionIndexEdit.EditValue = Config.LairRegionIndex;
            PenetraliumKeyAEdit.EditValue = Config.PenetraliumKeyA;
            PenetraliumKeyBEdit.EditValue = Config.PenetraliumKeyB;
            PenetraliumKeyCEdit.EditValue = Config.PenetraliumKeyC;
            RightDeliverEdit.EditValue = Config.RightDeliver;
            ErrorDeliverEdit.EditValue = Config.ErrorDeliver;
            LevelDifferenceEdit.EditValue = Config.LevelDifference;   //怪物经验衰减
            HPTimeEdit.EditValue = Config.HPTime;      //黑炎时间
            BACEdit.EditValue = Config.BAC;     //黑炎防御
            BCEdit.EditValue = Config.BC;    //黑炎攻击
            UpgradePetAddEdit.EditValue = Config.UpgradePetAdd;   //升级加宝宝属性
            PetPhantomAttack.EditValue = Config.PetPhantomAttack;
            PetPhantomAttackTextEdit.EditValue = Config.PetPhantomAttackEdit;
            PetPhantomAcMrTextEdit.EditValue = Config.PetPhantomAcMrEdit;
            UpgradePetExeEdit.EditValue = Config.UpgradePetExe;
            PetMaxLevelEdit.EditValue = Config.PetMaxLevel;
            PetMaxHPEdit.EditValue = Config.PetMaxHP;
            PetMinACEdit.EditValue = Config.PetMinAC;
            PetMaxACEdit.EditValue = Config.PetMaxAC;
            PetMinMREdit.EditValue = Config.PetMinMR;
            PetMaxMREdit.EditValue = Config.PetMaxMR;
            PetMinDCEdit.EditValue = Config.PetMinDC;
            PetMaxDCEdit.EditValue = Config.PetMaxDC;
            PetMinMCEdit.EditValue = Config.PetMinMC;
            PetMaxMCEdit.EditValue = Config.PetMaxMC;
            PetMinSCEdit.EditValue = Config.PetMinSC;
            PetMaxSCEdit.EditValue = Config.PetMaxSC;
            CallPetCountEdit.EditValue = Config.CallPetCount;  //战斗宠物最大召唤值
            CallPetLevelEdit.EditValue = Config.CallPetLevel;  //战斗宠物最大召唤等级
            MonHatredCheckEdit.EditValue = Config.MonHatred;
            MonHatredTimeEdit.EditValue = Config.MonHatredTime;
            BufferMapEffectShowCheckEdit.EditValue = Config.BufferMapEffectShow;
            PetACPowerRateTextEdit.EditValue = Config.PetACPowerRate;
            PetACPowerTimeTextEdit.EditValue = Config.PetACPowerTime;

            //Items
            DropDurationEdit.EditValue = Config.DropDuration;
            DropDistanceEdit.EditValue = Config.DropDistance;
            DropLayersEdit.EditValue = Config.DropLayers;
            CanItemPickupEdit.EditValue = Config.CanItemPickup;
            TorchRateEdit.EditValue = Config.TorchRate;
            SpecialRepairDelayEdit.EditValue = Config.SpecialRepairDelay;
            MaxLuckEdit.EditValue = Config.MaxLuck;
            LuckRateEdit.EditValue = Config.LuckRate;
            MaxCurseEdit.EditValue = Config.MaxCurse;
            CurseRateEdit.EditValue = Config.CurseRate;
            MaxStrengthEdit.EditValue = Config.MaxStrength;
            StrengthAddRateEdit.EditValue = Config.StrengthAddRate;
            StrengthLossRateEdit.EditValue = Config.StrengthLossRate;
            ResetCoolDownEdit.EditValue = Config.ResetCoolDown;  //武器重置冷却时间
            ResetCoolTimeEdit.EditValue = Config.ResetCoolTime;  //武器重置取回时间
            ResetAddValueTextEdit.EditValue = Config.ResetAddValue;   //武器重置加点率
            ResetStatValueTextEdit.EditValue = Config.ResetStatValue;  //武器重置增加的攻击最大值
            ResetElementValueTextEdit.EditValue = Config.ResetElementValue; //武器重置增加的元素最大值
            ResetExtraValueTextEdit.EditValue = Config.ResetExtraValue;   //武器额外最大值
            CommonResetProbability1.EditValue = Config.CommonResetProbability1;
            CommonResetProbability2.EditValue = Config.CommonResetProbability2;
            CommonResetProbability3.EditValue = Config.CommonResetProbability3;
            SuperiorResetProbability1.EditValue = Config.SuperiorResetProbability1;
            SuperiorResetProbability2.EditValue = Config.SuperiorResetProbability2;
            SuperiorResetProbability3.EditValue = Config.SuperiorResetProbability3;
            EliteResetProbability1.EditValue = Config.EliteResetProbability1;
            EliteResetProbability2.EditValue = Config.EliteResetProbability2;
            EliteResetProbability3.EditValue = Config.EliteResetProbability3;
            DigMineralCheckEdit.EditValue = Config.DigMineral;  //是否挖矿所得绑定
            HuntGoldPricecheckEdit.EditValue = Config.HuntGoldPrice;  //是否商城赏金购物绑定
            MarketPlaceStoreBuyLevelBound.EditValue = Config.MarketPlaceStoreBuyLevelBound;
            ShopNonRefinableCheckEdit.EditValue = Config.ShopNonRefinable; //商店物品是否能精炼
            UseFixedPointCheckEdit.EditValue = Config.UseFixedPoint;    //是否使用记忆传送功能
            IntFixedPointCountEdit.EditValue = Config.IntFixedPointCount;  //记忆传送基数
            MaxFixedPointCountEdit.EditValue = Config.MaxFixedPointCount;  //记忆传送最大值
            ItemNoticecheckEdit.EditValue = Config.ItemNotice;   //高级道具爆出系统提示
            TeleportRingEdit.EditValue = Config.TeleportRingCooling; //传送戒指冷却时间
            TeleportTimeEdit.EditValue = Config.TeleportTime;   //传送命令冷却时间
            TeleportIimitCheckEdit.EditValue = Config.TeleportIimit;  //传送是否地图限制
            FallPartOnlyEdit.EditValue = Config.FallPartOnly;   //道具碎片是否开启爆率
            ShowItemSourceCheckEdit.EditValue = Config.ShowItemSource;   //是否显示道具来源
            ShowGMItemSourceCheckEdit.EditValue = Config.ShowGMItemSource;  //是否显示道具来源GM制作
            UseOldItemDropCheckEdit.EditValue = Config.UseOldItemDrop;  //旧版爆率设置
            PersonalDropEnabledEdit.EditValue = Config.PersonalDropDisabled;  //个人爆率开关
            NewWeaponUpgradeEdit.EditValue = Config.NewWeaponUpgrade;   //新版武器升级开关
            CharacterInventoryDeathDropEdit.EditValue = Config.CharacterInventoryDeathDrop;
            InventoryAshDeathDrop.EditValue = Config.InventoryAshDeathDrop;
            InventoryRedDeathDrop.EditValue = Config.InventoryRedDeathDrop;
            CompanionInventoryDeathDropEdit.EditValue = Config.CompanionInventoryDeathDrop;
            ComInventoryAshDeathDrop.EditValue = Config.ComInventoryAshDeathDrop;
            ComInventoryRedDeathDrop.EditValue = Config.ComInventoryRedDeathDrop;
            CharacterEquipmentDeathDropEdit.EditValue = Config.CharacterEquipmentDeathDrop;
            EquipmentAshDeathDrop.EditValue = Config.EquipmentAshDeathDrop;
            EquipmentRedDeathDrop.EditValue = Config.EquipmentRedDeathDrop;
            WeapEquipmentDeathDrop.EditValue = Config.WeapEquipmentDeathDrop;
            DieRedRandomChanceTextEdit.EditValue = Config.DieRedRandomChance;
            MasterRefineChanceEdit.EditValue = Config.MasterRefineChance;
            MasterRefineCountEdit.EditValue = Config.MasterRefineCount;
            MasterRefineRandomEdit.EditValue = Config.MasterRefineRandom;
            DurabilityRateEdit.EditValue = Config.DurabilityRate;
            InSafeZoneItemExpireCheckEdit.EditValue = Config.InSafeZoneItemExpire;
            MedicamentHPMPTimeEdit.EditValue = Config.MedicamentHPMPTime;
            MedicamentHPTimeEdit.EditValue = Config.MedicamentHPTime;
            MedicamentMPTimeEdit.EditValue = Config.MedicamentMPTime;
            ItemCanRepairCheckEdit.EditValue = Config.ItemCanRepair;

            JewelryExpShowsCheckEdit.EditValue = Config.JewelryExpShows;
            JewelryLvShowsCheckEdit.EditValue = Config.JewelryLvShows;
            ACGoldRate.EditValue = Config.ACGoldRate;
            CommonItemSuccessRate.EditValue = Config.CommonItemSuccessRate;
            CommonItemReduceRate.EditValue = Config.CommonItemReduceRate;
            SuperiorItemSuccessRate.EditValue = Config.SuperiorItemSuccessRate;
            SuperiorItemReduceRate.EditValue = Config.SuperiorItemReduceRate;
            EliteItemSuccessRate.EditValue = Config.EliteItemSuccessRate;
            EliteItemReduceRate.EditValue = Config.EliteItemReduceRate;
            CommonItemLadder1.EditValue = Config.CommonItemLadder1;
            CommonItemAdditionalValue1.EditValue = Config.CommonItemAdditionalValue1;
            CommonItemLadder2.EditValue = Config.CommonItemLadder2;
            CommonItemAdditionalValue2.EditValue = Config.CommonItemAdditionalValue2;
            CommonItemLadder3.EditValue = Config.CommonItemLadder3;
            CommonItemAdditionalValue3.EditValue = Config.CommonItemAdditionalValue3;
            CommonItemLevelValue.EditValue = Config.CommonItemLevelValue;
            SuperiorItemLadder1.EditValue = Config.SuperiorItemLadder1;
            SuperiorItemAdditionalValue1.EditValue = Config.SuperiorItemAdditionalValue1;
            SuperiorItemLadder2.EditValue = Config.SuperiorItemLadder2;
            SuperiorItemAdditionalValue2.EditValue = Config.SuperiorItemAdditionalValue2;
            SuperiorItemLadder3.EditValue = Config.SuperiorItemLadder3;
            SuperiorItemAdditionalValue3.EditValue = Config.SuperiorItemAdditionalValue3;
            SuperiorItemLevelValue.EditValue = Config.SuperiorItemLevelValue;
            EliteItemLadder1.EditValue = Config.EliteItemLadder1;
            EliteItemAdditionalValue1.EditValue = Config.EliteItemAdditionalValue1;
            EliteItemLadder2.EditValue = Config.EliteItemLadder2;
            EliteItemAdditionalValue2.EditValue = Config.EliteItemAdditionalValue2;
            EliteItemLadder3.EditValue = Config.EliteItemLadder3;
            EliteItemAdditionalValue3.EditValue = Config.EliteItemAdditionalValue3;
            EliteItemLevelValue.EditValue = Config.EliteItemLevelValue;
            DeadLoseItemCheckEdit.EditValue = Config.DeadLoseItem;

            //Rates
            ExperienceRateEdit.EditValue = Config.ExperienceRate;
            DropRateEdit.EditValue = Config.DropRate;
            GoldRateEdit.EditValue = Config.GoldRate;
            SkillRateEdit.EditValue = Config.SkillRate;
            CompanionRateEdit.EditValue = Config.CompanionRate;
            VeteranRateEdit.EditValue = Config.VeteranRate;
            StarterGuildLevelEdit.EditValue = Config.StarterGuildLevelRate;
            StarterGuildExperienceEdit.EditValue = Config.StarterGuildExperienceRate;
            StarterGuildDropEdit.EditValue = Config.StarterGuildDropRate;
            StarterGuildGoldEdit.EditValue = Config.StarterGuildGoldRate;
            CastleExperienceEdit.EditValue = Config.CastleExperienceRate;
            CastleDropEdit.EditValue = Config.CastleDropRate;
            CastleGoldEdit.EditValue = Config.CastleGoldRate;
            GuildLevelEdit.EditValue = Config.GuildLevelRate;
            GuildExperienceEdit.EditValue = Config.GuildExperienceRate;
            GuildDropEdit.EditValue = Config.GuildDropRate;
            GuildGoldEdit.EditValue = Config.GuildGoldRate;
            GuildLevel1Edit.EditValue = Config.GuildLevel1Rate;
            GuildExperience1Edit.EditValue = Config.GuildExperience1Rate;
            GuildDrop1Edit.EditValue = Config.GuildDrop1Rate;
            GuildGold1Edit.EditValue = Config.GuildGold1Rate;
            GuildLevel2Edit.EditValue = Config.GuildLevel2Rate;
            GuildExperience2Edit.EditValue = Config.GuildExperience2Rate;
            GuildDrop2Edit.EditValue = Config.GuildDrop2Rate;
            GuildGold2Edit.EditValue = Config.GuildGold2Rate;
            GuildExperience3Edit.EditValue = Config.GuildExperience3Rate;
            GuildDrop3Edit.EditValue = Config.GuildDrop3Rate;
            GuildGold3Edit.EditValue = Config.GuildGold3Rate;
            StatExperienceEdit.EditValue = Config.StatExperienceRate;
            StatDropEdit.EditValue = Config.StatDropRate;
            StatGoldEdit.EditValue = Config.StatGoldRate;
            ZDEXPEdit.EditValue = Config.ZDEXPRate;
            DZEXPEdit.EditValue = Config.DZEXPRate;
            SZEXPEdit.EditValue = Config.SZEXPRate;
            DRZEXPEdit.EditValue = Config.DRZEXPRate;
            DZBLEdit.EditValue = Config.DZBLRate;
            SZBLEdit.EditValue = Config.SZBLRate;
            DRZBLEdit.EditValue = Config.DRZBLRate;
            Metxt_LockIps.Text = Config.LockIps;
            NowCountEdit.EditValue = Config.NowCount;
            DayCountEdit.EditValue = Config.DayCount;
            DaysLimitEdit.EditValue = Config.DaysLimit;
            GroupOrGuildCheckEdit.EditValue = Config.GroupOrGuild;
            AutoTrivialCheckEdit.EditValue = Config.AutoTrivial;
            GroupInZoneAddExp.EditValue = Config.GroupInZoneAddExp;
            GroupInZoneAddDrop.EditValue = Config.GroupInZoneAddDrop;
            GroupAddBaseExp.EditValue = Config.GroupAddBaseExp;
            GroupAddBaseDrop.EditValue = Config.GroupAddBaseDrop;
            GroupAddBaseGold.EditValue = Config.GroupAddBaseGold;
            GroupAddBaseHp.EditValue = Config.GroupAddBaseHp;
            GroupAddWarRate.EditValue = Config.GroupAddWarRate;
            GroupAddWizRate.EditValue = Config.GroupAddWizRate;
            GroupAddTaoRate.EditValue = Config.GroupAddTaoRate;
            GroupAddAssRate.EditValue = Config.GroupAddAssRate;

            //globals
            MarketTaxEdit.EditValue = Config.MarketPlaceTax;   //税率           
            ExitGuildEdit.EditValue = Config.ExitGuild;        //退出行会
            HuntGoldCapEdit.EditValue = Config.HuntGoldCap;    //赏金上限
            HuntGoldTimeEdit.EditValue = Config.HuntGoldTime;  //赏金每分钟获得数量
            ReferrerLevel1Edit.EditValue = Config.ReferrerLevel1;  //推荐人等级
            ReferrerLevel2Edit.EditValue = Config.ReferrerLevel2;
            ReferrerLevel3Edit.EditValue = Config.ReferrerLevel3;
            ReferrerLevel4Edit.EditValue = Config.ReferrerLevel4;
            ReferrerLevel5Edit.EditValue = Config.ReferrerLevel5;
            ReferrerHuntGold1Edit.EditValue = Config.ReferrerHuntGold1;  //推荐人获得赏金
            ReferrerHuntGold2Edit.EditValue = Config.ReferrerHuntGold2;
            ReferrerHuntGold3Edit.EditValue = Config.ReferrerHuntGold3;
            ReferrerHuntGold4Edit.EditValue = Config.ReferrerHuntGold4;
            ReferrerHuntGold5Edit.EditValue = Config.ReferrerHuntGold5;
            HuntGoldRatedEdit.EditValue = Config.HuntGoldRated;   //充值赏金获得比例
            ResetEdit.EditValue = Config.Reset;           //传奇宝箱重置元宝
            LuckDrawEdit.EditValue = Config.LuckDraw;     //传奇宝箱抽奖元宝
            RebirthLevelEdit.EditValue = Config.RebirthLevel;  //转生降低等级
            RebirthExpEdit.EditValue = Config.RebirthExp;      //转生降低经验倍率
            RebirthGoldEdit.EditValue = Config.RebirthGold;    //转生增加金币倍率
            RebirthDropEdit.EditValue = Config.RebirthDrop;    //转生增加爆率倍率
            RebirthPVEEdit.EditValue = Config.RebirthPVE;      //转生增加PVE比例
            RebirthPVPEdit.EditValue = Config.RebirthPVP;      //转生增加PVP比例
            RebirthReduceExpEdit.EditValue = Config.RebirthReduceExp;   //转生后获得的经验比率
            RebirthACEdit.EditValue = Config.RebirthAC;        //转生增加最大防御
            RebirthMACEdit.EditValue = Config.RebirthMAC;      //转生增加最大魔域
            RebirthDCEdit.EditValue = Config.RebirthDC;        //转生增加最大破坏
            RebirthMCEdit.EditValue = Config.RebirthMC;       // 转生增加最大自然
            RebirthSCEdit.EditValue = Config.RebirthSC;       //转生增加最大灵魂
            RebirthDieEdit.EditValue = Config.RebirthDie;     //转生死亡是否丢失经验            
            RankingShowCheckEdit.EditValue = Config.RankingShow;    //排行版显示开关
            AutoPotionForCompanionEdit.EditValue = Config.AutoPotionForCompanion;   //宠物包喝药
            UserCountcheckEdit.EditValue = Config.UserCount;   //在线人数显示
            UserCountDoubleEdit.EditValue = Config.UserCountDouble;  //在线人数倍率
            UserCountTimeTextEdit.EditValue = Config.UserCountTime;  //提示时间
            RateQueryShowCheckEdit.EditValue = Config.RateQueryShow;  //爆率查询

            FireREdit.EditValue = Config.ElementResistance;            //防御元素

            ComfortREdit.EditValue = Config.Comfort;        //舒适
            AttackREdit.EditValue = Config.AttackSpeed;    //攻击速度
            MaxLuckyEdit.EditValue = Config.MaxLucky;     //幸运值

            //全局调整攻击和血量
            MonLvMin1Edit.EditValue = Config.MonLvMin1;
            HPDifficulty1Edit.EditValue = Config.HPDifficulty1;
            ACDifficulty1Edit.EditValue = Config.ACDifficulty1;
            ACDifficulty11Edit.EditValue = Config.ACDifficulty11;
            PWDifficulty1Edit.EditValue = Config.PWDifficulty1;
            MonLvMin2Edit.EditValue = Config.MonLvMin2;
            HPDifficulty2Edit.EditValue = Config.HPDifficulty2;
            ACDifficulty2Edit.EditValue = Config.ACDifficulty2;
            ACDifficulty22Edit.EditValue = Config.ACDifficulty22;
            PWDifficulty2Edit.EditValue = Config.PWDifficulty2;
            MonLvMin3Edit.EditValue = Config.MonLvMin3;
            HPDifficulty3Edit.EditValue = Config.HPDifficulty3;
            ACDifficulty3Edit.EditValue = Config.ACDifficulty3;
            ACDifficulty33Edit.EditValue = Config.ACDifficulty33;
            PWDifficulty3Edit.EditValue = Config.PWDifficulty3;
            OverallExpTextEdit.EditValue = Config.OverallExp;

            BOSSMonLvMin1Edit.EditValue = Config.BOSSMonLvMin1;
            BOSSHPDifficulty1Edit.EditValue = Config.BOSSHPDifficulty1;
            BOSSACDifficulty1Edit.EditValue = Config.BOSSACDifficulty1;
            BOSSACDifficulty11Edit.EditValue = Config.BOSSACDifficulty11;
            BOSSPWDifficulty1Edit.EditValue = Config.BOSSPWDifficulty1;
            BOSSMonLvMin2Edit.EditValue = Config.BOSSMonLvMin2;
            BOSSHPDifficulty2Edit.EditValue = Config.BOSSHPDifficulty2;
            BOSSACDifficulty2Edit.EditValue = Config.BOSSACDifficulty2; 
            BOSSACDifficulty22Edit.EditValue = Config.BOSSACDifficulty22;
            BOSSPWDifficulty2Edit.EditValue = Config.BOSSPWDifficulty2;
            BOSSMonLvMin3Edit.EditValue = Config.BOSSMonLvMin3;
            BOSSHPDifficulty3Edit.EditValue = Config.BOSSHPDifficulty3;
            BOSSACDifficulty3Edit.EditValue = Config.BOSSACDifficulty3;
            BOSSACDifficulty33Edit.EditValue = Config.BOSSACDifficulty33;
            BOSSPWDifficulty3Edit.EditValue = Config.BOSSPWDifficulty3;

            PhysicalResistanceCheckEdit.EditValue = Config.PhysicalResistanceSwitch; //体质开关

            //CreateDropItem
            GourmetRandomEdit.EditValue = Config.GourmetRandom;  //极品几率
            GourmetTypeEdit.EditValue = Config.GourmetType;  //极品类别
            WeaponDC1.EditValue = Config.WeaponDC1;
            WeaponDC11.EditValue = Config.WeaponDC11;
            WeaponDC2.EditValue = Config.WeaponDC2;
            WeaponDC22.EditValue = Config.WeaponDC22;
            WeaponDC3.EditValue = Config.WeaponDC3;
            WeaponDC33.EditValue = Config.WeaponDC33;
            WeaponMSC1.EditValue = Config.WeaponMSC1;
            WeaponMSC11.EditValue = Config.WeaponMSC11;
            WeaponMSC2.EditValue = Config.WeaponMSC2;
            WeaponMSC22.EditValue = Config.WeaponMSC22;
            WeaponMSC3.EditValue = Config.WeaponMSC3;
            WeaponMSC33.EditValue = Config.WeaponMSC33;
            WeaponACC1.EditValue = Config.WeaponACC1;
            WeaponACC11.EditValue = Config.WeaponACC11;
            WeaponACC2.EditValue = Config.WeaponACC2;
            WeaponACC22.EditValue = Config.WeaponACC22;
            WeaponACC3.EditValue = Config.WeaponACC3;
            WeaponACC33.EditValue = Config.WeaponACC33;
            WeaponELE1.EditValue = Config.WeaponELE1;
            WeaponELE11.EditValue = Config.WeaponELE11;
            WeaponELE2.EditValue = Config.WeaponELE2;
            WeaponELE22.EditValue = Config.WeaponELE22;
            WeaponELE3.EditValue = Config.WeaponELE3;
            WeaponELE33.EditValue = Config.WeaponELE33;
            WeaponAS1.EditValue = Config.WeaponAS1;
            WeaponAS11.EditValue = Config.WeaponAS11;
            WeaponAS2.EditValue = Config.WeaponAS2;
            WeaponAS22.EditValue = Config.WeaponAS22;
            WeaponAS3.EditValue = Config.WeaponAS3;
            WeaponAS33.EditValue = Config.WeaponAS33;

            ArmourAC1.EditValue = Config.ArmourAC1;
            ArmourAC11.EditValue = Config.ArmourAC11;
            ArmourAC2.EditValue = Config.ArmourAC2;
            ArmourAC22.EditValue = Config.ArmourAC22;
            ArmourAC3.EditValue = Config.ArmourAC3;
            ArmourAC33.EditValue = Config.ArmourAC33;
            ArmourMR1.EditValue = Config.ArmourMR1;
            ArmourMR11.EditValue = Config.ArmourMR11;
            ArmourMR2.EditValue = Config.ArmourMR2;
            ArmourMR22.EditValue = Config.ArmourMR22;
            ArmourMR3.EditValue = Config.ArmourMR3;
            ArmourMR33.EditValue = Config.ArmourMR33;
            ArmourDC1.EditValue = Config.ArmourDC1;
            ArmourDC11.EditValue = Config.ArmourDC11;
            ArmourDC2.EditValue = Config.ArmourDC2;
            ArmourDC22.EditValue = Config.ArmourDC22;
            ArmourDC3.EditValue = Config.ArmourDC3;
            ArmourDC33.EditValue = Config.ArmourDC33;
            ArmourMSC1.EditValue = Config.ArmourMSC1;
            ArmourMSC11.EditValue = Config.ArmourMSC11;
            ArmourMSC2.EditValue = Config.ArmourMSC2;
            ArmourMSC22.EditValue = Config.ArmourMSC22;
            ArmourMSC3.EditValue = Config.ArmourMSC3;
            ArmourMSC33.EditValue = Config.ArmourMSC33;
            ArmourHP1.EditValue = Config.ArmourHP1;
            ArmourHP11.EditValue = Config.ArmourHP11;
            ArmourHP2.EditValue = Config.ArmourHP2;
            ArmourHP22.EditValue = Config.ArmourHP22;
            ArmourHP3.EditValue = Config.ArmourHP3;
            ArmourHP33.EditValue = Config.ArmourHP33;
            ArmourMP1.EditValue = Config.ArmourMP1;
            ArmourMP11.EditValue = Config.ArmourMP11;
            ArmourMP2.EditValue = Config.ArmourMP2;
            ArmourMP22.EditValue = Config.ArmourMP22;
            ArmourMP3.EditValue = Config.ArmourMP3;
            ArmourMP33.EditValue = Config.ArmourMP33;
            ArmourELE1.EditValue = Config.ArmourELE1;
            ArmourELE11.EditValue = Config.ArmourELE11;
            ArmourELE2.EditValue = Config.ArmourELE2;
            ArmourELE22.EditValue = Config.ArmourELE22;
            ArmourELE3.EditValue = Config.ArmourELE3;
            ArmourELE33.EditValue = Config.ArmourELE33;
            RArmourELE1.EditValue = Config.RArmourELE1;
            RArmourELE11.EditValue = Config.RArmourELE11;
            RArmourELE2.EditValue = Config.RArmourELE2;
            RArmourELE22.EditValue = Config.RArmourELE22;
            RArmourELE3.EditValue = Config.RArmourELE3;
            RArmourELE33.EditValue = Config.RArmourELE33;

            HelmetAC1.EditValue = Config.HelmetAC1;
            HelmetAC11.EditValue = Config.HelmetAC11;
            HelmetAC2.EditValue = Config.HelmetAC2;
            HelmetAC22.EditValue = Config.HelmetAC22;
            HelmetAC3.EditValue = Config.HelmetAC3;
            HelmetAC33.EditValue = Config.HelmetAC33;
            HelmetMR1.EditValue = Config.HelmetMR1;
            HelmetMR11.EditValue = Config.HelmetMR11;
            HelmetMR2.EditValue = Config.HelmetMR2;
            HelmetMR22.EditValue = Config.HelmetMR22;
            HelmetMR3.EditValue = Config.HelmetMR3;
            HelmetMR33.EditValue = Config.HelmetMR33;
            HelmetDC1.EditValue = Config.HelmetDC1;
            HelmetDC11.EditValue = Config.HelmetDC11;
            HelmetDC2.EditValue = Config.HelmetDC2;
            HelmetDC22.EditValue = Config.HelmetDC22;
            HelmetDC3.EditValue = Config.HelmetDC3;
            HelmetDC33.EditValue = Config.HelmetDC33;
            HelmetMSC1.EditValue = Config.HelmetMSC1;
            HelmetMSC11.EditValue = Config.HelmetMSC11;
            HelmetMSC2.EditValue = Config.HelmetMSC2;
            HelmetMSC22.EditValue = Config.HelmetMSC22;
            HelmetMSC3.EditValue = Config.HelmetMSC3;
            HelmetMSC33.EditValue = Config.HelmetMSC33;
            HelmetHP1.EditValue = Config.HelmetHP1;
            HelmetHP11.EditValue = Config.HelmetHP11;
            HelmetHP2.EditValue = Config.HelmetHP2;
            HelmetHP22.EditValue = Config.HelmetHP22;
            HelmetHP3.EditValue = Config.HelmetHP3;
            HelmetHP33.EditValue = Config.HelmetHP33;
            HelmetMP1.EditValue = Config.HelmetMP1;
            HelmetMP11.EditValue = Config.HelmetMP11;
            HelmetMP2.EditValue = Config.HelmetMP2;
            HelmetMP22.EditValue = Config.HelmetMP22;
            HelmetMP3.EditValue = Config.HelmetMP3;
            HelmetMP33.EditValue = Config.HelmetMP33;

            HelmetGELE1.EditValue = Config.HelmetGELE1;
            HelmetGELE11.EditValue = Config.HelmetGELE11;
            HelmetGELE2.EditValue = Config.HelmetGELE2;
            HelmetGELE22.EditValue = Config.HelmetGELE22;
            HelmetGELE3.EditValue = Config.HelmetGELE3;
            HelmetGELE33.EditValue = Config.HelmetGELE33;

            HelmetELE1.EditValue = Config.HelmetELE1;
            HelmetELE11.EditValue = Config.HelmetELE11;
            HelmetELE2.EditValue = Config.HelmetELE2;
            HelmetELE22.EditValue = Config.HelmetELE22;
            HelmetELE3.EditValue = Config.HelmetELE3;
            HelmetELE33.EditValue = Config.HelmetELE33;
            RHelmetELE1.EditValue = Config.RHelmetELE1;
            RHelmetELE11.EditValue = Config.RHelmetELE11;
            RHelmetELE2.EditValue = Config.RHelmetELE2;
            RHelmetELE22.EditValue = Config.RHelmetELE22;
            RHelmetELE3.EditValue = Config.RHelmetELE3;
            RHelmetELE33.EditValue = Config.RHelmetELE33;

            NecklaceDC1.EditValue = Config.NecklaceDC1;
            NecklaceDC11.EditValue = Config.NecklaceDC11;
            NecklaceDC2.EditValue = Config.NecklaceDC2;
            NecklaceDC22.EditValue = Config.NecklaceDC22;
            NecklaceDC3.EditValue = Config.NecklaceDC3;
            NecklaceDC33.EditValue = Config.NecklaceDC33;
            NecklaceMSC1.EditValue = Config.NecklaceMSC1;
            NecklaceMSC11.EditValue = Config.NecklaceMSC11;
            NecklaceMSC2.EditValue = Config.NecklaceMSC2;
            NecklaceMSC22.EditValue = Config.NecklaceMSC22;
            NecklaceMSC3.EditValue = Config.NecklaceMSC3;
            NecklaceMSC33.EditValue = Config.NecklaceMSC33;
            NecklaceME1.EditValue = Config.NecklaceME1;
            NecklaceME11.EditValue = Config.NecklaceME11;
            NecklaceME2.EditValue = Config.NecklaceME2;
            NecklaceME22.EditValue = Config.NecklaceME22;
            NecklaceME3.EditValue = Config.NecklaceME3;
            NecklaceME33.EditValue = Config.NecklaceME33;
            NecklaceACC1.EditValue = Config.NecklaceACC1;
            NecklaceACC11.EditValue = Config.NecklaceACC11;
            NecklaceACC2.EditValue = Config.NecklaceACC2;
            NecklaceACC22.EditValue = Config.NecklaceACC22;
            NecklaceACC3.EditValue = Config.NecklaceACC3;
            NecklaceACC33.EditValue = Config.NecklaceACC33;
            NecklaceAG1.EditValue = Config.NecklaceAG1;
            NecklaceAG11.EditValue = Config.NecklaceAG11;
            NecklaceAG2.EditValue = Config.NecklaceAG2;
            NecklaceAG22.EditValue = Config.NecklaceAG22;
            NecklaceAG3.EditValue = Config.NecklaceAG3;
            NecklaceAG33.EditValue = Config.NecklaceAG33;
            NecklaceELE1.EditValue = Config.NecklaceELE1;
            NecklaceELE11.EditValue = Config.NecklaceELE11;
            NecklaceELE2.EditValue = Config.NecklaceELE2;
            NecklaceELE22.EditValue = Config.NecklaceELE22;
            NecklaceELE3.EditValue = Config.NecklaceELE3;
            NecklaceELE33.EditValue = Config.NecklaceELE33;

            BraceletDC1.EditValue = Config.BraceletDC1;
            BraceletDC11.EditValue = Config.BraceletDC11;
            BraceletDC2.EditValue = Config.BraceletDC2;
            BraceletDC22.EditValue = Config.BraceletDC22;
            BraceletDC3.EditValue = Config.BraceletDC3;
            BraceletDC33.EditValue = Config.BraceletDC33;
            BraceletMSC1.EditValue = Config.BraceletMSC1;
            BraceletMSC11.EditValue = Config.BraceletMSC11;
            BraceletMSC2.EditValue = Config.BraceletMSC2;
            BraceletMSC22.EditValue = Config.BraceletMSC22;
            BraceletMSC3.EditValue = Config.BraceletMSC3;
            BraceletMSC33.EditValue = Config.BraceletMSC33;
            BraceletAC1.EditValue = Config.BraceletAC1;
            BraceletAC11.EditValue = Config.BraceletAC11;
            BraceletAC2.EditValue = Config.BraceletAC2;
            BraceletAC22.EditValue = Config.BraceletAC22;
            BraceletAC3.EditValue = Config.BraceletAC3;
            BraceletAC33.EditValue = Config.BraceletAC33;
            BraceletMR1.EditValue = Config.BraceletMR1;
            BraceletMR11.EditValue = Config.BraceletMR11;
            BraceletMR2.EditValue = Config.BraceletMR2;
            BraceletMR22.EditValue = Config.BraceletMR22;
            BraceletMR3.EditValue = Config.BraceletMR3;
            BraceletMR33.EditValue = Config.BraceletMR33;
            BraceletACC1.EditValue = Config.BraceletACC1;
            BraceletACC11.EditValue = Config.BraceletACC11;
            BraceletACC2.EditValue = Config.BraceletACC2;
            BraceletACC22.EditValue = Config.BraceletACC22;
            BraceletACC3.EditValue = Config.BraceletACC3;
            BraceletACC33.EditValue = Config.BraceletACC33;
            BraceletAG1.EditValue = Config.BraceletAG1;
            BraceletAG11.EditValue = Config.BraceletAG11;
            BraceletAG2.EditValue = Config.BraceletAG2;
            BraceletAG22.EditValue = Config.BraceletAG22;
            BraceletAG3.EditValue = Config.BraceletAG3;
            BraceletAG33.EditValue = Config.BraceletAG33;
            BraceletGELE1.EditValue = Config.BraceletELE1;
            BraceletGELE11.EditValue = Config.BraceletELE11;
            BraceletGELE2.EditValue = Config.BraceletELE2;
            BraceletGELE22.EditValue = Config.BraceletELE22;
            BraceletGELE3.EditValue = Config.BraceletELE3;
            BraceletGELE33.EditValue = Config.BraceletELE33;
            BraceletElE1.EditValue = Config.BraceletElE1;
            BraceletElE11.EditValue = Config.BraceletElE11;
            BraceletElE2.EditValue = Config.BraceletElE2;
            BraceletElE22.EditValue = Config.BraceletElE22;
            BraceletElE3.EditValue = Config.BraceletElE3;
            BraceletElE33.EditValue = Config.BraceletElE33;
            RBraceletElE1.EditValue = Config.RBraceletElE1;
            RBraceletElE11.EditValue = Config.RBraceletElE11;
            RBraceletElE2.EditValue = Config.RBraceletElE2;
            RBraceletElE22.EditValue = Config.RBraceletElE22;
            RBraceletElE3.EditValue = Config.RBraceletElE3;
            RBraceletElE33.EditValue = Config.RBraceletElE33;

            RingDC1.EditValue = Config.RingDC1;
            RingDC11.EditValue = Config.RingDC11;
            RingDC2.EditValue = Config.RingDC2;
            RingDC22.EditValue = Config.RingDC22;
            RingDC3.EditValue = Config.RingDC3;
            RingDC33.EditValue = Config.RingDC33;
            RingMSC1.EditValue = Config.RingMSC1;
            RingMSC11.EditValue = Config.RingMSC11;
            RingMSC2.EditValue = Config.RingMSC2;
            RingMSC22.EditValue = Config.RingMSC22;
            RingMSC3.EditValue = Config.RingMSC3;
            RingMSC33.EditValue = Config.RingMSC33;
            RingELE1.EditValue = Config.RingELE1;
            RingELE11.EditValue = Config.RingELE11;
            RingELE2.EditValue = Config.RingELE2;
            RingELE22.EditValue = Config.RingELE22;
            RingELE3.EditValue = Config.RingELE3;
            RingELE33.EditValue = Config.RingELE33;

            ShoesAC1.EditValue = Config.ShoesAC1;
            ShoesAC11.EditValue = Config.ShoesAC11;
            ShoesAC2.EditValue = Config.ShoesAC2;
            ShoesAC22.EditValue = Config.ShoesAC22;
            ShoesAC3.EditValue = Config.ShoesAC3;
            ShoesAC33.EditValue = Config.ShoesAC33;
            ShoesMR1.EditValue = Config.ShoesMR1;
            ShoesMR11.EditValue = Config.ShoesMR11;
            ShoesMR2.EditValue = Config.ShoesMR2;
            ShoesMR22.EditValue = Config.ShoesMR22;
            ShoesMR3.EditValue = Config.ShoesMR3;
            ShoesMR33.EditValue = Config.ShoesMR33;
            ShoesHP1.EditValue = Config.ShoesHP1;
            ShoesHP11.EditValue = Config.ShoesHP11;
            ShoesHP2.EditValue = Config.ShoesHP2;
            ShoesHP22.EditValue = Config.ShoesHP22;
            ShoesHP3.EditValue = Config.ShoesHP3;
            ShoesHP33.EditValue = Config.ShoesHP33;
            ShoesMP1.EditValue = Config.ShoesMP1;
            ShoesMP11.EditValue = Config.ShoesMP11;
            ShoesMP2.EditValue = Config.ShoesMP2;
            ShoesMP22.EditValue = Config.ShoesMP22;
            ShoesMP3.EditValue = Config.ShoesMP3;
            ShoesMP33.EditValue = Config.ShoesMP33;
            ShoesCF1.EditValue = Config.ShoesCF1;
            ShoesCF11.EditValue = Config.ShoesCF11;
            ShoesCF2.EditValue = Config.ShoesCF2;
            ShoesCF22.EditValue = Config.ShoesCF22;
            ShoesCF3.EditValue = Config.ShoesCF3;
            ShoesCF33.EditValue = Config.ShoesCF33;
            ShoesELE1.EditValue = Config.ShoesELE1;
            ShoesELE11.EditValue = Config.ShoesELE11;
            ShoesELE2.EditValue = Config.ShoesELE2;
            ShoesELE22.EditValue = Config.ShoesELE22;
            ShoesELE3.EditValue = Config.ShoesELE3;
            ShoesELE33.EditValue = Config.ShoesELE33;
            RShoesELE1.EditValue = Config.RShoesELE1;
            RShoesELE11.EditValue = Config.RShoesELE11;
            RShoesELE2.EditValue = Config.RShoesELE2;
            RShoesELE22.EditValue = Config.RShoesELE22;
            RShoesELE3.EditValue = Config.RShoesELE3;
            RShoesELE33.EditValue = Config.RShoesELE33;

            ShieldDCP1.EditValue = Config.ShieldDCP1;
            ShieldDCP11.EditValue = Config.ShieldDCP11;
            ShieldDCP2.EditValue = Config.ShieldDCP2;
            ShieldDCP22.EditValue = Config.ShieldDCP22;
            ShieldDCP3.EditValue = Config.ShieldDCP3;
            ShieldDCP33.EditValue = Config.ShieldDCP33;
            ShieldMSCP1.EditValue = Config.ShieldMSCP1;
            ShieldMSCP11.EditValue = Config.ShieldMSCP11;
            ShieldMSCP2.EditValue = Config.ShieldMSCP2;
            ShieldMSCP22.EditValue = Config.ShieldMSCP22;
            ShieldMSCP3.EditValue = Config.ShieldMSCP3;
            ShieldMSCP33.EditValue = Config.ShieldMSCP33;
            ShieldBC1.EditValue = Config.ShieldBC1;
            ShieldBC11.EditValue = Config.ShieldBC11;
            ShieldBC2.EditValue = Config.ShieldBC2;
            ShieldBC22.EditValue = Config.ShieldBC22;
            ShieldBC3.EditValue = Config.ShieldBC3;
            ShieldBC33.EditValue = Config.ShieldBC33;
            ShieldEC1.EditValue = Config.ShieldEC1;
            ShieldEC11.EditValue = Config.ShieldEC11;
            ShieldEC2.EditValue = Config.ShieldEC2;
            ShieldEC22.EditValue = Config.ShieldEC22;
            ShieldEC3.EditValue = Config.ShieldEC3;
            ShieldEC33.EditValue = Config.ShieldEC33;
            ShieldPR1.EditValue = Config.ShieldPR1;
            ShieldPR11.EditValue = Config.ShieldPR11;
            ShieldPR2.EditValue = Config.ShieldPR2;
            ShieldPR22.EditValue = Config.ShieldPR22;
            ShieldPR3.EditValue = Config.ShieldPR3;
            ShieldPR33.EditValue = Config.ShieldPR33;
            ShieldELE1.EditValue = Config.ShieldELE1;
            ShieldELE11.EditValue = Config.ShieldELE11;
            ShieldELE2.EditValue = Config.ShieldELE2;
            ShieldELE22.EditValue = Config.ShieldELE22;
            ShieldELE3.EditValue = Config.ShieldELE3;
            ShieldELE33.EditValue = Config.ShieldELE33;
            RShieldELE1.EditValue = Config.RShieldELE1;
            RShieldELE11.EditValue = Config.RShieldELE11;
            RShieldELE2.EditValue = Config.RShieldELE2;
            RShieldELE22.EditValue = Config.RShieldELE22;
            RShieldELE3.EditValue = Config.RShieldELE3;
            RShieldELE33.EditValue = Config.RShieldELE33;

            SkillExpEdit.EditValue = Config.SkillExp;   //技能经验值倍率
            MaxMagicLvEdit.EditValue = Config.MaxMagicLv;  //技能等级限制
            SkillExpDropEdit.EditValue = Config.SkillExpDrop;   //4级技能点数
            CanFlyTargetCheckEdit.EditValue = Config.CanFlyTargetCheck;  //直线魔法限制
            QKDNYEdit.EditValue = Config.QKDNY;
            ZTBSCheckEdit.EditValue = Config.ZTBS;
            ZPXKSCheckEdit.EditValue = Config.PXKS;
            BeckonParalysisCheckEdit.EditValue = Config.BeckonParalysis;
            ZHDKHPEdit.EditValue = Config.ZHDKHP;
            ZHDKFWEdit.EditValue = Config.ZHDKFW;
            ZZDKZEdit.EditValue = Config.ZZDKZ;
            MYWZYXEdit.EditValue = Config.MYWZYX;
            MYWZHYEdit.EditValue = Config.MYWZHY;
            ResurrectionOrderCheckEdit.EditValue = Config.ResurrectionOrder;
            DemonExplosionPVPEdit.EditValue = Config.DemonExplosionPVP;
            PoisonDustValueEdit.EditValue = Config.PoisonDustValue;
            PoisonDustDarkAttack.EditValue = Config.PoisonDustDarkAttack;
            PoisoningBossCheckEdit.EditValue = Config.PoisoningBossCheck;
            PoisonDeadCheckEdit.EditValue = Config.PoisonDead;
            RedPoisonAttackRateTextEdit.EditValue = Config.RedPoisonAttackRate;
            ETDarkAffinityRateTextEdit.EditValue = Config.ETDarkAffinityRate;
            FixedCureCheckEdit.EditValue = Config.FixedCure;
            SummonRandomValueTextEdit.EditValue = Config.SummonRandomValue;
            TraOverTimeTextEdit.EditValue = Config.TraOverTime;
            FixedCureValueTextEdit.EditValue = Config.FixedCureValue;
            YXHWEdit.EditValue = Config.YXHW;
            GeoManipulationSuccessRate.EditValue = Config.GeoManipulationSuccessRate;
            ExpelUndeadSuccessRateEdit.EditValue = Config.ExpelUndeadSuccessRate;
            ExpelUndeadLevelTextEdit.EditValue = Config.ExpelUndeadLevel;
            YJDTYDEdit.EditValue = Config.YJDTYD;
            DanceOfSwallowSilencedCheckEdit.EditValue = Config.DanceOfSwallowSilenced;
            DanceOfSwallowParalysisCheckEdit.EditValue = Config.DanceOfSwallowParalysis;
            MirrorImageCanMoveCheckEdit.EditValue = Config.MirrorImageCanMove;
            MirrorImageDamageDarkStone.EditValue = Config.MirrorImageDamageDarkStone;
            MagicShieldRemainingTime.EditValue = Config.MagicShieldRemainingTime;
            ElectricShockPetsCount.EditValue = Config.ElectricShockPetsCount;
            ElectricShockSuccessRateEdit.EditValue = Config.ElectricShockSuccessRate;
            PetsMutinyTimeTextEdit.EditValue = Config.PetsMutinyTime;
            MeteorShowerTargetsCount.EditValue = Config.MeteorShowerTargetsCount;
            ThunderStrikeGetCellsTextEdit.EditValue = Config.ThunderStrikeGetCells;
            ThunderStrikeRandomTextEdit.EditValue = Config.ThunderStrikeRandom;

            //行会
            StarterGuildRebirthEdit.EditValue = Config.StarterGuildRebirth;
            GuildCreationCostEdit.EditValue = Config.GuildCreationCost;
            GuildMemberCostEdit.EditValue = Config.GuildMemberCost;
            GuildStorageCostEdit.EditValue = Config.GuildStorageCost;
            GuildMemberHardLimitEdit.EditValue = Config.GuildMemberHardLimit;
            GuildWarCostEdit.EditValue = Config.GuildWarCost;

            FlagSuccessTeleportCheck.EditValue = Config.FlagSuccessTeleport;
            EndWarTeleportCheck.EditValue = Config.EndWarTeleport;

            VictoryDelayTextEdit.EditValue = Config.VictoryDelay;
            ConquestFlagDelayCheckEdit.EditValue = Config.ConquestFlagDelay;
            AttackerReviveInDesignatedAreaDuringWar.EditValue = Config.AttackerReviveInDesignatedAreaDuringWar;
            AllowTeleportMagicNearFlag.EditValue = Config.AllowTeleportMagicNearFlag;
            WarFlagRangeLimit.EditValue = Config.WarFlagRangeLimit;

            GuildMaxFundTextEdit.EditValue = Config.GuildMaxFund;
            ActivationCeilingTextEdit.EditValue = Config.ActivationCeiling;
            PersonalGoldRatioTextEdit.EditValue = Config.PersonalGoldRatio;
            PersonalExpRatioTextEdit.EditValue = Config.PersonalExpRatio;

            //大补帖设置
            AutoTimeEdit.EditValue = Config.AutoTime;          //挂机时间
            AutoHookTabEdit.EditValue = Config.AutoHookTab;   //自动挂机
            HooKDropEdit.EditValue = Config.HooKDrop;         //挂机爆率
            BrightBoxEdit.EditValue = Config.BrightBox;       //免蜡
            RunCheckEdit.EditValue = Config.RunCheck;         //免助跑
            RockCheckEdit.EditValue = Config.RockCheck;       //稳如泰山
            ClearBodyCheckEdit.EditValue = Config.ClearBodyCheck;   //清理尸体
            BigPatchAutoPotionDelay.EditValue = Config.BigPatchAutoPotionDelay;  //喝药延迟毫秒

            //内核防挂
            AttackDelayTextEdit.EditValue = Config.GlobalsAttackDelay;
            ASpeedRateTextEdit.EditValue = Config.GlobalsASpeedRate;
            ProjectileSpeedTextEdit.EditValue = Config.GlobalsProjectileSpeed;
            TurnTimeTextEdit.EditValue = Config.GlobalsTurnTime;
            HarvestTimeTextEdit.EditValue = Config.GlobalsHarvestTime;
            MoveTimeTextEdit.EditValue = Config.GlobalsMoveTime;
            AttackTimeTextEdit.EditValue = Config.GlobalsAttackTime;
            CastTimeTextEdit.EditValue = Config.GlobalsCastTime;
            MagicDelayTextEdit.EditValue = Config.GlobalsMagicDelay;

            EncryptDBToggleSwitch.IsOn = Config.EnableDBEncryption;
            EncryptDBPassword.EditValue = Config.DBPassword;
            LicenseFile.EditValue = Config.LicenseFile;

            richTextBox_BanCPU.LoadFile(Application.StartupPath + "\\Database\\BanCPU.txt", RichTextBoxStreamType.PlainText);
            richTextBox_BanHDD.LoadFile(Application.StartupPath + "\\Database\\BanHDD.txt", RichTextBoxStreamType.PlainText);
            richTextBox_BanMAC.LoadFile(Application.StartupPath + "\\Database\\BanMAC.txt", RichTextBoxStreamType.PlainText);
        }
        public void SaveSettings()
        {
            //Network
            Config.WebSite = (string)WebSiteEdit.EditValue;  //注册网址设置
            Config.IPAddress = (string)IPAddressEdit.EditValue;
            Config.Port = (ushort)PortEdit.EditValue;
            Config.TimeOut = (TimeSpan)TimeOutEdit.EditValue;
            Config.PingDelay = (TimeSpan)PingDelayEdit.EditValue;
            Config.UserCountPort = (ushort)UserCountPortEdit.EditValue;
            Config.MaxPacket = (int)MaxPacketEdit.EditValue;
            Config.PacketBanTime = (TimeSpan)PacketBanTimeEdit.EditValue;
            Config.LockIps = Metxt_LockIps.Text;
            Config.NowCount = (int)NowCountEdit.EditValue;
            Config.DayCount = (int)DayCountEdit.EditValue;
            Config.DaysLimit = (int)DaysLimitEdit.EditValue;
            Config.MaxConnectionsPerIp = (int)MaxConnectionsPerIpEdit.EditValue;
            Config.MySqlEnable = (bool)MySqlEnableToggleSwitch.IsOn;
            Config.MySqlUser = (string)MySqlUserEdit.EditValue;
            Config.MySqlPassword = (string)MySqlPassEdit.EditValue;
            Config.MySqlIP = (string)MySqlIPEdit.EditValue;
            Config.MySqlPort = (string)MySqlPortEdit.EditValue;

            //修改客户端名字后发包通知客户端
            if (Config.ClientName != (string)ClientNameEdit.EditValue)
            {
                try
                {
                    Config.ClientName = (string)ClientNameEdit.EditValue;   //客户端名字设置
                    foreach (PlayerObject user in SEnvir.Players)
                        user.SendClientNameChanged();
                }
                catch { }
            }

            //Control
            Config.AllowNewAccount = (bool)AllowNewAccountEdit.EditValue;
            Config.AllowChangePassword = (bool)AllowChangePasswordEdit.EditValue;
            Config.AllowLogin = (bool)AllowLoginEdit.EditValue;
            Config.AllowNewCharacter = (bool)AllowNewCharacterEdit.EditValue;
            Config.AllowDeleteCharacter = (bool)AllowDeleteCharacterEdit.EditValue;
            Config.AllowStartGame = (bool)AllowStartGameEdit.EditValue;
            Config.AllowWarrior = (bool)AllowWarriorEdit.EditValue;
            Config.AllowWizard = (bool)AllowWizardEdit.EditValue;
            Config.AllowTaoist = (bool)AllowTaoistEdit.EditValue;
            Config.AllowAssassin = (bool)AllowAssassinEdit.EditValue;
            Config.RelogDelay = (TimeSpan)RelogDelayEdit.EditValue;
            Config.AllowRequestPasswordReset = (bool)AllowRequestPasswordResetEdit.EditValue;
            Config.AllowWebResetPassword = (bool)AllowWebResetPasswordEdit.EditValue;
            Config.AllowManualResetPassword = (bool)AllowManualResetPasswordEdit.EditValue;
            Config.AllowDeleteAccount = (bool)AllowDeleteAccountEdit.EditValue;
            Config.AllowManualActivation = (bool)AllowManualActivationEdit.EditValue;
            Config.AllowWebActivation = (bool)AllowWebActivationEdit.EditValue;
            Config.AllowRequestActivation = (bool)AllowRequestActivationEdit.EditValue;
            Config.UseInviteCode = (bool)UseInviteCodeCheckEdit.EditValue;

            //System
            Config.CheckVersion = (bool)CheckVersionEdit.EditValue;
            Config.VersionPath = (string)VersionPathEdit.EditValue;
            Config.VersionPath1 = (string)VersionPath1Edit.EditValue;
            Config.DBSaveDelay = (TimeSpan)DBSaveDelayEdit.EditValue;
            Config.MapPath = (string)MapPathEdit.EditValue;
            Config.MasterPassword = (string)MasterPasswordEdit.EditValue;
            Config.MasterPasswordSwitch = (bool)MasterPasswordSwitchCheckEdit.EditValue;
            Config.ClientPath = (string)ClientPathEdit.EditValue;
            Config.ReleaseDate = (DateTime)ReleaseDateEdit.EditValue;
            Config.EasterEventEnd = (DateTime)RabbitEventEndEdit.EditValue;
            Config.NewLevel = (string)NewLevelEdit.EditValue;
            Config.NewGold = (string)NewGoldEdit.EditValue;
            Config.NewGameGold = (string)NewGameGoldEdit.EditValue;
            Config.NewPrestige = (string)NewPrestigeEdit.EditValue;
            Config.NewContribute = (string)NewContributeEdit.EditValue;
            Config.EnvirTickCount = (int)EnvirTickCountEdit.EditValue;
            Config.TestServer = (bool)TestServerCheckEdit.EditValue;
            Config.EnableDBEncryption = EncryptDBToggleSwitch.IsOn;
            Config.DBPassword = (string)EncryptDBPassword.EditValue;
            Config.CheckAutoCloseServer = (bool)checkAutoCloseServer.EditValue;
            Config.TimeAutoCloseServer = Convert.ToInt32(TimeAutoCloseServer.EditValue);
            Config.CloseServerTime = Convert.ToInt32(CloseServerTimeEdit.EditValue);
            Config.CheckPhoneVersion = (bool)CheckPhoneVersionCheckEdit.EditValue;
            Config.PhoneVersionNumber = PhoneVersionNumberTextEdit.EditValue.ToString();

            //Mail
            Config.MailServer = (string)MailServerEdit.EditValue;
            Config.MailPort = (int)MailPortEdit.EditValue;
            Config.MailUseSSL = (bool)MailUseSSLEdit.EditValue;
            Config.MailAccount = (string)MailAccountEdit.EditValue;
            Config.MailPassword = (string)MailPasswordEdit.EditValue;
            Config.MailFrom = (string)MailFromEdit.EditValue;
            Config.MailDisplayName = (string)MailDisplayNameEdit.EditValue;

            //WebServer
            Config.WebPrefix = (string)WebPrefixEdit.EditValue;
            Config.WebCommandLink = (string)WebCommandLinkEdit.EditValue;
            Config.ActivationSuccessLink = (string)ActivationSuccessLinkEdit.EditValue;
            Config.ActivationFailLink = (string)ActivationFailLinkEdit.EditValue;
            Config.ResetSuccessLink = (string)ResetSuccessLinkEdit.EditValue;
            Config.ResetFailLink = (string)ResetFailLinkEdit.EditValue;
            Config.DeleteSuccessLink = (string)DeleteSuccessLinkEdit.EditValue;
            Config.DeleteFailLink = (string)DeleteFailLinkEdit.EditValue;

            Config.BuyPrefix = (string)BuyPrefixEdit.EditValue;
            Config.RechargeInterface = (bool)RechargeInterfaceCheck.EditValue;
            Config.BuyAddress = (string)BuyAddressEdit.EditValue;
            Config.IPNPrefix = (string)IPNPrefixEdit.EditValue;
            Config.ReceiverEMail = (string)ReceiverEMailEdit.EditValue;
            Config.ProcessGameGold = (bool)ProcessGameGoldEdit.EditValue;
            Config.AllowBuyGammeGold = (bool)AllowBuyGammeGoldEdit.EditValue;
            Config.RequireActivation = (bool)RequireActivationEdit.EditValue;
            Config.CanUseChineseName = (bool)CanUseChineseName.EditValue;
            Config.CanUseChineseGuildName = (bool)CanUseChineseGuildName.EditValue;

            //Players
            Config.MaxViewRange = (int)MaxViewRangeEdit.EditValue;
            Config.MaxNPCViewRange = (int)MaxNPCViewRangeTextEdit.EditValue;
            Config.ShoutDelay = (TimeSpan)ShoutDelayEdit.EditValue;
            Config.GlobalDelay = (TimeSpan)GlobalDelayEdit.EditValue;
            Config.MaxLevelLimit = (bool)MaxLevelLimitCheckEdit.EditValue;
            Config.MaxLevel = (int)MaxLevelEdit.EditValue;
            Config.DayCycleCount = (int)DayCycleCountEdit.EditValue;
            Config.AllowObservation = (bool)AllowObservationEdit.EditValue;
            Config.ObservedCount = (int)ObservedCountTextEdit.EditValue;
            Config.BrownDuration = (TimeSpan)BrownDurationEdit.EditValue;
            Config.PKPointRate = (int)PKPointRateEdit.EditValue;
            Config.PKPointTickRate = (TimeSpan)PKPointTickRateEdit.EditValue;
            Config.RedPoint = (int)RedPointEdit.EditValue;
            Config.PvPCurseDuration = (TimeSpan)PvPCurseDurationEdit.EditValue;
            Config.PvPCurseRate = (int)PvPCurseRateEdit.EditValue;
            Config.PVPLuckCheck = (bool)PVPLuckCheckEdit.EditValue;
            Config.AutoReviveDelay = (TimeSpan)AutoReviveDelayEdit.EditValue;
            Config.Storage = (bool)AllowStorageEdit.EditValue;
            Config.RandomAssign = (bool)RandomAssigncheckEdit.EditValue;  //随机加点
            Config.AssignHermitMinACMR = (bool)AssignHermitMinACMRCheckEdit.EditValue;  //额外属性加上限
            Config.PartnerDeadTeleport = (bool)PartnerDeadTeleportCheckEdit.EditValue;  //夫妻传送配偶死亡传送限制
            Config.MarriageTeleportDelay = (TimeSpan)MarriageTeleportDelayEdit.EditValue; //夫妻传送延迟
            Config.MarriageTeleportLocation = (int)MarriageTeleportLocation.EditValue;  //夫妻传送范围
            Config.GroupRecallTime = (int)GroupRecallTimeEdit.EditValue;  //天地合一冷却时间
            Config.ShowSafeZone = (bool)ShowSafezoneEdit.EditValue;     //显示安全区
            Config.DistinctionRepair = (bool)DistinctionRepairEdit.EditValue;   //区别维修
            Config.DailyQuestLimit = (int)DailyQuestEdit.EditValue;  //每日任务次数
            Config.RepeatableQuestLimit = (int)RepeatableQuestEdit.EditValue; //可重复任务次数
            Config.ShortcutEnabled = (bool)ShortcutEnabledEdit.EditValue;  //顶部UI图标
            Config.SafeZoneBuffPause = (bool)BUFFInSafeZoneEdit.EditValue;    //BUFF安全区暂停
            Config.OfflineBuffTicking = (bool)OfflineBuffTickingCheckEdit.EditValue;    //BUFF安全区暂停
            Config.CriticalDamagePVP = (bool)CriticalDamagePVPEdit.EditValue;  //PVP暴击伤害%
            Config.RankingLevel = (int)RankingLevelTextEdit.EditValue;    //排行称号显示等级
            Config.WarsTime = (int)WarsTimeTextEdit.EditValue;  //沙巴克攻城时间
            Config.FlagCaptureTime = (int)WarsFlagTimeEdit.EditValue;  //沙巴克夺旗时间           
            Config.DailyFreeCoins = (int)DailyFreeCoinsEdit.EditValue;
            Config.CoinPlaceChoice = (bool)CoinPlaceChoiceCheckEdit.EditValue;
            Config.TownReviveHPRate = Convert.ToInt32(TownReviveHPRate.EditValue);
            Config.TownReviveMPRate = Convert.ToInt32(TownReviveMPRate.EditValue);
            Config.GoldChangeAlert = (int)GoldChangeAlertTextEdit.EditValue;
            Config.WizardMCRate = (int)WizardMCRateTextEdit.EditValue;
            Config.TaoistSCRate = (int)TaoistSCRateTextEdit.EditValue;
            Config.WizardMagicAttackRate = (int)WizardMagicAttackRateTextEdit.EditValue;
            Config.TaoistMagicAttackRate = (int)TaoistMagicAttackRateTextEdit.EditValue;
            Config.ClearOverMarket = (int)ClearOverMarketTextEdit.EditValue;
            Config.LevelReductionDamage = (int)LevelReductionDamage.EditValue;
            Config.CompareLevelValues = (int)CompareLevelValues.EditValue;
            Config.WarriorReductionDamage = (int)WarriorReductionDamage.EditValue;

            //Monsters
            Config.DeadDuration = (TimeSpan)DeadDurationEdit.EditValue;
            Config.HarvestDuration = (TimeSpan)HarvestDurationEdit.EditValue;
            Config.MysteryShipRegionIndex = (int)MysteryShipRegionIndexEdit.EditValue;
            Config.LairRegionIndex = (int)LairRegionIndexEdit.EditValue;
            Config.LairRegionIndex = (int)LairRegionIndexEdit.EditValue;
            Config.PenetraliumKeyA = (int)PenetraliumKeyAEdit.EditValue;
            Config.PenetraliumKeyB = (int)PenetraliumKeyBEdit.EditValue;
            Config.PenetraliumKeyC = (int)PenetraliumKeyCEdit.EditValue;
            Config.RightDeliver = (int)RightDeliverEdit.EditValue;
            Config.ErrorDeliver = (int)ErrorDeliverEdit.EditValue;
            Config.LevelDifference = (decimal)LevelDifferenceEdit.EditValue;  //怪物经验衰减
            Config.HPTime = (int)HPTimeEdit.EditValue;    //黑炎时间
            Config.BAC = (int)BACEdit.EditValue;   //黑炎防御
            Config.BC = (int)BCEdit.EditValue;    //黑炎攻击
            Config.UpgradePetAdd = (bool)UpgradePetAddEdit.EditValue;  //升级加宝宝属性
            Config.PetPhantomAttack = (bool)PetPhantomAttack.EditValue;
            Config.PetPhantomAttackEdit = (int)PetPhantomAttackTextEdit.EditValue;
            Config.PetPhantomAcMrEdit = (int)PetPhantomAcMrTextEdit.EditValue;
            Config.UpgradePetExe = (int)UpgradePetExeEdit.EditValue;
            Config.PetMaxLevel = (int)PetMaxLevelEdit.EditValue;
            Config.PetMaxHP = (int)PetMaxHPEdit.EditValue;
            Config.PetMinAC = (int)PetMinACEdit.EditValue;
            Config.PetMaxAC = (int)PetMaxACEdit.EditValue;
            Config.PetMinMR = (int)PetMinMREdit.EditValue;
            Config.PetMaxMR = (int)PetMaxMREdit.EditValue;
            Config.PetMinDC = (int)PetMinDCEdit.EditValue;
            Config.PetMaxDC = (int)PetMaxDCEdit.EditValue;
            Config.PetMinMC = (int)PetMinMCEdit.EditValue;
            Config.PetMaxMC = (int)PetMaxMCEdit.EditValue;
            Config.PetMinSC = (int)PetMinSCEdit.EditValue;
            Config.PetMaxSC = (int)PetMaxSCEdit.EditValue;
            Config.CallPetCount = (int)CallPetCountEdit.EditValue;
            Config.CallPetLevel = (int)CallPetLevelEdit.EditValue;
            Config.MonHatred = (bool)MonHatredCheckEdit.EditValue;
            Config.MonHatredTime = (int)MonHatredTimeEdit.EditValue;
            Config.BufferMapEffectShow = (bool)BufferMapEffectShowCheckEdit.EditValue;
            Config.PetACPowerRate = (decimal)PetACPowerRateTextEdit.EditValue;
            Config.PetACPowerTime = (int)PetACPowerTimeTextEdit.EditValue;

            //Items
            Config.DropDuration = (TimeSpan)DropDurationEdit.EditValue;
            Config.DropDistance = (int)DropDistanceEdit.EditValue;
            Config.DropLayers = (int)DropLayersEdit.EditValue;
            Config.CanItemPickup = (TimeSpan)CanItemPickupEdit.EditValue;
            Config.TorchRate = (int)TorchRateEdit.EditValue;
            Config.SpecialRepairDelay = (TimeSpan)SpecialRepairDelayEdit.EditValue;
            Config.MaxLuck = (int)MaxLuckEdit.EditValue;
            Config.LuckRate = (int)LuckRateEdit.EditValue;
            Config.MaxCurse = (int)MaxCurseEdit.EditValue;
            Config.CurseRate = (int)CurseRateEdit.EditValue;

            Config.MaxStrength = (int)MaxStrengthEdit.EditValue;
            Config.StrengthAddRate = (int)StrengthAddRateEdit.EditValue;
            Config.StrengthLossRate = (int)StrengthLossRateEdit.EditValue;
            Config.ResetCoolDown = (int)ResetCoolDownEdit.EditValue;   //武器冷却重置时间
            Config.ResetCoolTime = (int)ResetCoolTimeEdit.EditValue;   //武器重置取回时间
            Config.ResetAddValue = (int)ResetAddValueTextEdit.EditValue;  //武器重置加点比率
            Config.ResetStatValue = (int)ResetStatValueTextEdit.EditValue;  //武器重置增加的攻击最大值
            Config.ResetElementValue = (int)ResetElementValueTextEdit.EditValue; //武器重置增加的元素最大值
            Config.ResetExtraValue = (int)ResetExtraValueTextEdit.EditValue;  //武器重置额外最大值
            Config.CommonResetProbability1 = (int)CommonResetProbability1.EditValue;
            Config.CommonResetProbability2 = (int)CommonResetProbability2.EditValue;
            Config.CommonResetProbability3 = (int)CommonResetProbability3.EditValue;
            Config.SuperiorResetProbability1 = (int)SuperiorResetProbability1.EditValue;
            Config.SuperiorResetProbability2 = (int)SuperiorResetProbability2.EditValue;
            Config.SuperiorResetProbability3 = (int)SuperiorResetProbability3.EditValue;
            Config.EliteResetProbability1 = (int)EliteResetProbability1.EditValue;
            Config.EliteResetProbability2 = (int)EliteResetProbability2.EditValue;
            Config.EliteResetProbability3 = (int)EliteResetProbability3.EditValue;
            Config.DigMineral = (bool)DigMineralCheckEdit.EditValue;  //是否挖矿所得绑定
            Config.HuntGoldPrice = (bool)HuntGoldPricecheckEdit.EditValue;  //是否商城赏金购物绑定
            Config.MarketPlaceStoreBuyLevelBound = (int)MarketPlaceStoreBuyLevelBound.EditValue;
            Config.ShopNonRefinable = (bool)ShopNonRefinableCheckEdit.EditValue;  //商店道具是否能精炼
            Config.UseFixedPoint = (bool)UseFixedPointCheckEdit.EditValue;      //是否使用记忆传送功能
            Config.IntFixedPointCount = (int)IntFixedPointCountEdit.EditValue;  //记忆传送基数
            Config.MaxFixedPointCount = (int)MaxFixedPointCountEdit.EditValue;  //记忆传送最大值
            Config.ItemNotice = (bool)ItemNoticecheckEdit.EditValue;  //高级道具爆出系统公告提示
            Config.TeleportRingCooling = (int)TeleportRingEdit.EditValue;  //传送戒指冷却时间
            Config.TeleportTime = (int)TeleportTimeEdit.EditValue;    //传送命令冷却时间
            Config.TeleportIimit = (bool)TeleportIimitCheckEdit.EditValue;  //传送是否地图限制
            Config.FallPartOnly = (bool)FallPartOnlyEdit.EditValue;   //道具碎片是否开启爆率
            Config.ShowItemSource = (bool)ShowItemSourceCheckEdit.EditValue; //是否在客户端显示物品来源
            Config.ShowGMItemSource = (bool)ShowGMItemSourceCheckEdit.EditValue; //是否显示GM刷的物品来源
            Config.UseOldItemDrop = (bool)UseOldItemDropCheckEdit.EditValue;   //旧版传奇3爆率
            Config.PersonalDropDisabled = (bool)PersonalDropEnabledEdit.EditValue;  //个人爆率开关
            Config.NewWeaponUpgrade = (bool)NewWeaponUpgradeEdit.EditValue;  //新版武器升级开关
            Config.CharacterInventoryDeathDrop = (int)CharacterInventoryDeathDropEdit.EditValue;
            Config.InventoryAshDeathDrop = (int)InventoryAshDeathDrop.EditValue;
            Config.InventoryRedDeathDrop = (int)InventoryRedDeathDrop.EditValue;
            Config.CompanionInventoryDeathDrop = (int)CompanionInventoryDeathDropEdit.EditValue;
            Config.ComInventoryAshDeathDrop = (int)ComInventoryAshDeathDrop.EditValue;
            Config.ComInventoryRedDeathDrop = (int)ComInventoryRedDeathDrop.EditValue;
            Config.CharacterEquipmentDeathDrop = (int)CharacterEquipmentDeathDropEdit.EditValue;
            Config.EquipmentAshDeathDrop = (int)EquipmentAshDeathDrop.EditValue;
            Config.EquipmentRedDeathDrop = (int)EquipmentRedDeathDrop.EditValue;
            Config.DieRedRandomChance = (int)DieRedRandomChanceTextEdit.EditValue;
            Config.WeapEquipmentDeathDrop = (int)WeapEquipmentDeathDrop.EditValue;
            Config.MasterRefineChance = Convert.ToInt32(MasterRefineChanceEdit.EditValue);
            Config.MasterRefineCount = Convert.ToInt32(MasterRefineCountEdit.EditValue);
            Config.MasterRefineRandom = (bool)MasterRefineRandomEdit.EditValue;
            Config.DurabilityRate = Convert.ToInt32(DurabilityRateEdit.EditValue);
            Config.InSafeZoneItemExpire = (bool)InSafeZoneItemExpireCheckEdit.EditValue;
            Config.MedicamentHPMPTime = (int)MedicamentHPMPTimeEdit.EditValue;
            Config.MedicamentHPTime = (int)MedicamentHPTimeEdit.EditValue;
            Config.MedicamentMPTime = (int)MedicamentMPTimeEdit.EditValue;
            Config.ItemCanRepair = (bool)ItemCanRepairCheckEdit.EditValue;

            Config.JewelryExpShows = (bool)JewelryExpShowsCheckEdit.EditValue;
            Config.JewelryLvShows = (bool)JewelryLvShowsCheckEdit.EditValue;
            Config.ACGoldRate = (int)ACGoldRate.EditValue;
            Config.CommonItemSuccessRate = (int)CommonItemSuccessRate.EditValue;
            Config.CommonItemReduceRate = (int)CommonItemReduceRate.EditValue;
            Config.SuperiorItemSuccessRate = (int)SuperiorItemSuccessRate.EditValue;
            Config.SuperiorItemReduceRate = (int)SuperiorItemReduceRate.EditValue;
            Config.EliteItemSuccessRate = (int)EliteItemSuccessRate.EditValue;
            Config.EliteItemReduceRate = (int)EliteItemReduceRate.EditValue;
            Config.CommonItemLadder1 = (int)CommonItemLadder1.EditValue;
            Config.CommonItemAdditionalValue1 = (int)CommonItemAdditionalValue1.EditValue;
            Config.CommonItemLadder2 = (int)CommonItemLadder2.EditValue;
            Config.CommonItemAdditionalValue2 = (int)CommonItemAdditionalValue2.EditValue;
            Config.CommonItemLadder3 = (int)CommonItemLadder3.EditValue;
            Config.CommonItemAdditionalValue3 = (int)CommonItemAdditionalValue3.EditValue;
            Config.CommonItemLevelValue = (int)CommonItemLevelValue.EditValue;
            Config.SuperiorItemLadder1 = (int)SuperiorItemLadder1.EditValue;
            Config.SuperiorItemAdditionalValue1 = (int)SuperiorItemAdditionalValue1.EditValue;
            Config.SuperiorItemLadder2 = (int)SuperiorItemLadder2.EditValue;
            Config.SuperiorItemAdditionalValue2 = (int)SuperiorItemAdditionalValue2.EditValue;
            Config.SuperiorItemLadder3 = (int)SuperiorItemLadder3.EditValue;
            Config.SuperiorItemAdditionalValue3 = (int)SuperiorItemAdditionalValue3.EditValue;
            Config.SuperiorItemLevelValue = (int)SuperiorItemLevelValue.EditValue;
            Config.EliteItemLadder1 = (int)EliteItemLadder1.EditValue;
            Config.EliteItemAdditionalValue1 = (int)EliteItemAdditionalValue1.EditValue;
            Config.EliteItemLadder2 = (int)EliteItemLadder2.EditValue;
            Config.EliteItemAdditionalValue2 = (int)EliteItemAdditionalValue2.EditValue;
            Config.EliteItemLadder3 = (int)EliteItemLadder3.EditValue;
            Config.EliteItemAdditionalValue3 = (int)EliteItemAdditionalValue3.EditValue;
            Config.EliteItemLevelValue = (int)EliteItemLevelValue.EditValue;
            Config.DeadLoseItem = (bool)DeadLoseItemCheckEdit.EditValue;

            //Rates
            Config.ExperienceRate = (int)ExperienceRateEdit.EditValue;
            Config.DropRate = (int)DropRateEdit.EditValue;
            Config.GoldRate = (int)GoldRateEdit.EditValue;
            Config.SkillRate = (int)SkillRateEdit.EditValue;
            Config.CompanionRate = (int)CompanionRateEdit.EditValue;
            Config.VeteranRate = (int)VeteranRateEdit.EditValue;
            Config.StarterGuildLevelRate = (int)StarterGuildLevelEdit.EditValue;
            Config.StarterGuildExperienceRate = (int)StarterGuildExperienceEdit.EditValue;
            Config.StarterGuildDropRate = (int)StarterGuildDropEdit.EditValue;
            Config.StarterGuildGoldRate = (int)StarterGuildGoldEdit.EditValue;
            Config.CastleExperienceRate = (int)CastleExperienceEdit.EditValue;
            Config.CastleDropRate = (int)CastleDropEdit.EditValue;
            Config.CastleGoldRate = (int)CastleGoldEdit.EditValue;
            Config.GuildLevelRate = (int)GuildLevelEdit.EditValue;
            Config.GuildExperienceRate = (int)GuildExperienceEdit.EditValue;
            Config.GuildDropRate = (int)GuildDropEdit.EditValue;
            Config.GuildGoldRate = (int)GuildGoldEdit.EditValue;
            Config.GuildLevel1Rate = (int)GuildLevel1Edit.EditValue;
            Config.GuildExperience1Rate = (int)GuildExperience1Edit.EditValue;
            Config.GuildDrop1Rate = (int)GuildDrop1Edit.EditValue;
            Config.GuildGold1Rate = (int)GuildGold1Edit.EditValue;
            Config.GuildLevel2Rate = (int)GuildLevel2Edit.EditValue;
            Config.GuildExperience2Rate = (int)GuildExperience2Edit.EditValue;
            Config.GuildDrop2Rate = (int)GuildDrop2Edit.EditValue;
            Config.GuildGold2Rate = (int)GuildGold2Edit.EditValue;
            Config.GuildExperience3Rate = (int)GuildExperience3Edit.EditValue;
            Config.GuildDrop3Rate = (int)GuildDrop3Edit.EditValue;
            Config.GuildGold3Rate = (int)GuildGold3Edit.EditValue;
            Config.StatExperienceRate = (int)StatExperienceEdit.EditValue;
            Config.StatDropRate = (int)StatDropEdit.EditValue;
            Config.StatGoldRate = (int)StatGoldEdit.EditValue;
            Config.ZDEXPRate = (decimal)ZDEXPEdit.EditValue;
            Config.DZEXPRate = (decimal)DZEXPEdit.EditValue;
            Config.SZEXPRate = (decimal)SZEXPEdit.EditValue;
            Config.DRZEXPRate = (decimal)DRZEXPEdit.EditValue;
            Config.DZBLRate = (decimal)DZBLEdit.EditValue;
            Config.SZBLRate = (decimal)SZBLEdit.EditValue;
            Config.DRZBLRate = (decimal)DRZBLEdit.EditValue;
            Config.GroupOrGuild = (bool)GroupOrGuildCheckEdit.EditValue;
            Config.AutoTrivial = (bool)AutoTrivialCheckEdit.EditValue;
            Config.GroupInZoneAddExp = (int)GroupInZoneAddExp.EditValue;
            Config.GroupInZoneAddDrop = (int)GroupInZoneAddDrop.EditValue;
            Config.GroupAddBaseExp = (int)GroupAddBaseExp.EditValue;
            Config.GroupAddBaseDrop = (int)GroupAddBaseDrop.EditValue;
            Config.GroupAddBaseGold = (int)GroupAddBaseGold.EditValue;
            Config.GroupAddBaseHp = (int)GroupAddBaseHp.EditValue;
            Config.GroupAddWarRate = (decimal)GroupAddWarRate.EditValue;
            Config.GroupAddWizRate = (decimal)GroupAddWizRate.EditValue;
            Config.GroupAddTaoRate = (decimal)GroupAddTaoRate.EditValue;
            Config.GroupAddAssRate = (decimal)GroupAddAssRate.EditValue;
            //globals
            Config.MarketPlaceTax = (decimal)MarketTaxEdit.EditValue;  //税率            
            Config.ExitGuild = (int)ExitGuildEdit.EditValue;           //退出行会时间
            Config.HuntGoldCap = (int)HuntGoldCapEdit.EditValue;       //赏金上限
            Config.HuntGoldTime = (int)HuntGoldTimeEdit.EditValue;     //赏金每分钟获得数量
            Config.ReferrerLevel1 = (int)ReferrerLevel1Edit.EditValue;  //推荐人等级
            Config.ReferrerLevel2 = (int)ReferrerLevel2Edit.EditValue;
            Config.ReferrerLevel3 = (int)ReferrerLevel3Edit.EditValue;
            Config.ReferrerLevel4 = (int)ReferrerLevel4Edit.EditValue;
            Config.ReferrerLevel5 = (int)ReferrerLevel5Edit.EditValue;
            Config.ReferrerHuntGold1 = (int)ReferrerHuntGold1Edit.EditValue;  //推荐人获得赏金
            Config.ReferrerHuntGold2 = (int)ReferrerHuntGold2Edit.EditValue;
            Config.ReferrerHuntGold3 = (int)ReferrerHuntGold3Edit.EditValue;
            Config.ReferrerHuntGold4 = (int)ReferrerHuntGold4Edit.EditValue;
            Config.ReferrerHuntGold5 = (int)ReferrerHuntGold5Edit.EditValue;
            Config.HuntGoldRated = (int)HuntGoldRatedEdit.EditValue;   //充值元宝获得赏金比例
            Config.Reset = (int)ResetEdit.EditValue;        //传奇宝箱重置元宝
            Config.LuckDraw = (int)LuckDrawEdit.EditValue;  //传奇宝箱抽奖元宝
            Config.RebirthLevel = (int)RebirthLevelEdit.EditValue;  //转生降低等级
            Config.RebirthExp = (int)RebirthExpEdit.EditValue;      //转生降低经验倍率
            Config.RebirthGold = (int)RebirthGoldEdit.EditValue;    //转生增加金币倍率
            Config.RebirthDrop = (int)RebirthDropEdit.EditValue;    //转生增加爆率倍率
            Config.RebirthPVE = (decimal)RebirthPVEEdit.EditValue;      //转生增加PVE比例
            Config.RebirthPVP = (decimal)RebirthPVPEdit.EditValue;      //转生增加PVP比例
            Config.RebirthReduceExp = (decimal)RebirthReduceExpEdit.EditValue;   //转生后获得的经验降低比率
            Config.RebirthAC = (int)RebirthACEdit.EditValue;        //转生增加最大防御
            Config.RebirthMAC = (int)RebirthMACEdit.EditValue;      //转生增加最大魔域
            Config.RebirthDC = (int)RebirthDCEdit.EditValue;        //转生增加最大破坏
            Config.RebirthMC = (int)RebirthMCEdit.EditValue;       // 转生增加最大自然
            Config.RebirthSC = (int)RebirthSCEdit.EditValue;       //转生增加最大灵魂
            Config.RebirthDie = (bool)RebirthDieEdit.EditValue;     //转生死亡是否丢失经验           
            Config.RankingShow = (bool)RankingShowCheckEdit.EditValue;   //显示排行版
            Config.AutoPotionForCompanion = (bool)AutoPotionForCompanionEdit.EditValue; //宠物包喝药
            Config.UserCount = (bool)UserCountcheckEdit.EditValue;  //在线人数显示
            Config.UserCountDouble = (int)UserCountDoubleEdit.EditValue;  //在线人数倍率
            Config.UserCountTime = (int)UserCountTimeTextEdit.EditValue;  //提示时间
            Config.RateQueryShow = (bool)RateQueryShowCheckEdit.EditValue;  //爆率查询

            Config.ElementResistance = (int)FireREdit.EditValue;            //防御元素

            Config.Comfort = (int)ComfortREdit.EditValue;        //舒适
            Config.AttackSpeed = (int)AttackREdit.EditValue;    //攻击速度
            Config.MaxLucky = (int)MaxLuckyEdit.EditValue;   //幸运

            //全局调整血量和攻击
            Config.MonLvMin1 = (int)MonLvMin1Edit.EditValue;
            Config.HPDifficulty1 = (int)HPDifficulty1Edit.EditValue;
            Config.PWDifficulty1 = (int)PWDifficulty1Edit.EditValue;
            Config.ACDifficulty1 = (int)ACDifficulty1Edit.EditValue;
            Config.ACDifficulty11 = (int)ACDifficulty11Edit.EditValue;
            Config.MonLvMin2 = (int)MonLvMin2Edit.EditValue;
            Config.HPDifficulty2 = (int)HPDifficulty2Edit.EditValue;
            Config.PWDifficulty2 = (int)PWDifficulty2Edit.EditValue;
            Config.ACDifficulty2 = (int)ACDifficulty2Edit.EditValue;
            Config.ACDifficulty22 = (int)ACDifficulty22Edit.EditValue;
            Config.MonLvMin3 = (int)MonLvMin3Edit.EditValue;
            Config.HPDifficulty3 = (int)HPDifficulty3Edit.EditValue;
            Config.PWDifficulty3 = (int)PWDifficulty3Edit.EditValue;
            Config.ACDifficulty3 = (int)ACDifficulty3Edit.EditValue;
            Config.ACDifficulty33 = (int)ACDifficulty33Edit.EditValue;
            Config.OverallExp = (int)OverallExpTextEdit.EditValue;

            Config.BOSSMonLvMin1 = (int)BOSSMonLvMin1Edit.EditValue;
            Config.BOSSHPDifficulty1 = (int)BOSSHPDifficulty1Edit.EditValue;
            Config.BOSSPWDifficulty1 = (int)BOSSPWDifficulty1Edit.EditValue;
            Config.BOSSACDifficulty1 = (int)BOSSACDifficulty1Edit.EditValue;
            Config.BOSSACDifficulty11 = (int)BOSSACDifficulty11Edit.EditValue;
            Config.BOSSMonLvMin2 = (int)BOSSMonLvMin2Edit.EditValue;
            Config.BOSSHPDifficulty2 = (int)BOSSHPDifficulty2Edit.EditValue;
            Config.BOSSPWDifficulty2 = (int)BOSSPWDifficulty2Edit.EditValue;
            Config.BOSSACDifficulty2 = (int)BOSSACDifficulty2Edit.EditValue;
            Config.BOSSACDifficulty22 = (int)BOSSACDifficulty22Edit.EditValue;
            Config.MonLvMin3 = (int)BOSSMonLvMin3Edit.EditValue;
            Config.BOSSACDifficulty3 = (int)BOSSACDifficulty3Edit.EditValue;
            Config.BOSSACDifficulty33 = (int)BOSSACDifficulty33Edit.EditValue;
            Config.BOSSHPDifficulty3 = (int)BOSSHPDifficulty3Edit.EditValue;
            Config.BOSSPWDifficulty3 = (int)BOSSPWDifficulty3Edit.EditValue;

            Config.PhysicalResistanceSwitch = (bool)PhysicalResistanceCheckEdit.EditValue;   //体质开关

            //CreateDropItem
            Config.GourmetRandom = (int)GourmetRandomEdit.EditValue;  //极品几率
            Config.GourmetType = (int)GourmetTypeEdit.EditValue;   //极品类别
            Config.WeaponDC1 = (int)WeaponDC1.EditValue;
            Config.WeaponDC11 = (int)WeaponDC11.EditValue;
            Config.WeaponDC2 = (int)WeaponDC2.EditValue;
            Config.WeaponDC22 = (int)WeaponDC22.EditValue;
            Config.WeaponDC3 = (int)WeaponDC3.EditValue;
            Config.WeaponDC33 = (int)WeaponDC33.EditValue;
            Config.WeaponMSC1 = (int)WeaponMSC1.EditValue;
            Config.WeaponMSC11 = (int)WeaponMSC11.EditValue;
            Config.WeaponMSC2 = (int)WeaponMSC2.EditValue;
            Config.WeaponMSC22 = (int)WeaponMSC22.EditValue;
            Config.WeaponMSC3 = (int)WeaponMSC3.EditValue;
            Config.WeaponMSC33 = (int)WeaponMSC33.EditValue;
            Config.WeaponACC1 = (int)WeaponACC1.EditValue;
            Config.WeaponACC11 = (int)WeaponACC11.EditValue;
            Config.WeaponACC2 = (int)WeaponACC2.EditValue;
            Config.WeaponACC22 = (int)WeaponACC22.EditValue;
            Config.WeaponACC3 = (int)WeaponACC3.EditValue;
            Config.WeaponACC33 = (int)WeaponACC33.EditValue;
            Config.WeaponELE1 = (int)WeaponELE1.EditValue;
            Config.WeaponELE11 = (int)WeaponELE11.EditValue;
            Config.WeaponELE2 = (int)WeaponELE2.EditValue;
            Config.WeaponELE22 = (int)WeaponELE22.EditValue;
            Config.WeaponELE3 = (int)WeaponELE3.EditValue;
            Config.WeaponELE33 = (int)WeaponELE33.EditValue;
            Config.WeaponAS1 = (int)WeaponAS1.EditValue;
            Config.WeaponAS11 = (int)WeaponAS11.EditValue;
            Config.WeaponAS2 = (int)WeaponAS2.EditValue;
            Config.WeaponAS22 = (int)WeaponAS22.EditValue;
            Config.WeaponAS3 = (int)WeaponAS3.EditValue;
            Config.WeaponAS33 = (int)WeaponAS33.EditValue;

            Config.ArmourAC1 = (int)ArmourAC1.EditValue;
            Config.ArmourAC11 = (int)ArmourAC11.EditValue;
            Config.ArmourAC2 = (int)ArmourAC2.EditValue;
            Config.ArmourAC22 = (int)ArmourAC22.EditValue;
            Config.ArmourAC3 = (int)ArmourAC3.EditValue;
            Config.ArmourAC33 = (int)ArmourAC33.EditValue;
            Config.ArmourMR1 = (int)ArmourMR1.EditValue;
            Config.ArmourMR11 = (int)ArmourMR11.EditValue;
            Config.ArmourMR2 = (int)ArmourMR2.EditValue;
            Config.ArmourMR22 = (int)ArmourMR22.EditValue;
            Config.ArmourMR3 = (int)ArmourMR3.EditValue;
            Config.ArmourMR33 = (int)ArmourMR33.EditValue;
            Config.ArmourDC1 = (int)ArmourDC1.EditValue;
            Config.ArmourDC11 = (int)ArmourDC11.EditValue;
            Config.ArmourDC2 = (int)ArmourDC2.EditValue;
            Config.ArmourDC22 = (int)ArmourDC22.EditValue;
            Config.ArmourDC3 = (int)ArmourDC3.EditValue;
            Config.ArmourDC33 = (int)ArmourDC33.EditValue;
            Config.ArmourMSC1 = (int)ArmourMSC1.EditValue;
            Config.ArmourMSC11 = (int)ArmourMSC11.EditValue;
            Config.ArmourMSC2 = (int)ArmourMSC2.EditValue;
            Config.ArmourMSC22 = (int)ArmourMSC22.EditValue;
            Config.ArmourMSC3 = (int)ArmourMSC3.EditValue;
            Config.ArmourMSC33 = (int)ArmourMSC33.EditValue;
            Config.ArmourHP1 = (int)ArmourHP1.EditValue;
            Config.ArmourHP11 = (int)ArmourHP11.EditValue;
            Config.ArmourHP2 = (int)ArmourHP2.EditValue;
            Config.ArmourHP22 = (int)ArmourHP22.EditValue;
            Config.ArmourHP3 = (int)ArmourHP3.EditValue;
            Config.ArmourHP33 = (int)ArmourHP33.EditValue;
            Config.ArmourMP1 = (int)ArmourMP1.EditValue;
            Config.ArmourMP11 = (int)ArmourMP11.EditValue;
            Config.ArmourMP2 = (int)ArmourMP2.EditValue;
            Config.ArmourMP22 = (int)ArmourMP22.EditValue;
            Config.ArmourMP3 = (int)ArmourMP3.EditValue;
            Config.ArmourMP33 = (int)ArmourMP33.EditValue;
            Config.ArmourELE1 = (int)ArmourELE1.EditValue;
            Config.ArmourELE11 = (int)ArmourELE11.EditValue;
            Config.ArmourELE2 = (int)ArmourELE2.EditValue;
            Config.ArmourELE22 = (int)ArmourELE22.EditValue;
            Config.ArmourELE3 = (int)ArmourELE3.EditValue;
            Config.ArmourELE33 = (int)ArmourELE33.EditValue;
            Config.RArmourELE1 = (int)RArmourELE1.EditValue;
            Config.RArmourELE11 = (int)RArmourELE11.EditValue;
            Config.RArmourELE2 = (int)RArmourELE2.EditValue;
            Config.RArmourELE22 = (int)RArmourELE22.EditValue;
            Config.RArmourELE3 = (int)RArmourELE3.EditValue;
            Config.RArmourELE33 = (int)RArmourELE33.EditValue;

            Config.HelmetAC1 = (int)HelmetAC1.EditValue;
            Config.HelmetAC11 = (int)HelmetAC11.EditValue;
            Config.HelmetAC2 = (int)HelmetAC2.EditValue;
            Config.HelmetAC22 = (int)HelmetAC22.EditValue;
            Config.HelmetAC3 = (int)HelmetAC3.EditValue;
            Config.HelmetAC33 = (int)HelmetAC33.EditValue;
            Config.HelmetMR1 = (int)HelmetMR1.EditValue;
            Config.HelmetMR11 = (int)HelmetMR11.EditValue;
            Config.HelmetMR2 = (int)HelmetMR2.EditValue;
            Config.HelmetMR22 = (int)HelmetMR22.EditValue;
            Config.HelmetMR3 = (int)HelmetMR3.EditValue;
            Config.HelmetMR33 = (int)HelmetMR33.EditValue;
            Config.HelmetDC1 = (int)HelmetDC1.EditValue;
            Config.HelmetDC11 = (int)HelmetDC11.EditValue;
            Config.HelmetDC2 = (int)HelmetDC2.EditValue;
            Config.HelmetDC22 = (int)HelmetDC22.EditValue;
            Config.HelmetDC3 = (int)HelmetDC3.EditValue;
            Config.HelmetDC33 = (int)HelmetDC33.EditValue;
            Config.HelmetMSC1 = (int)HelmetMSC1.EditValue;
            Config.HelmetMSC11 = (int)HelmetMSC11.EditValue;
            Config.HelmetMSC2 = (int)HelmetMSC2.EditValue;
            Config.HelmetMSC22 = (int)HelmetMSC22.EditValue;
            Config.HelmetMSC3 = (int)HelmetMSC3.EditValue;
            Config.HelmetMSC33 = (int)HelmetMSC33.EditValue;
            Config.HelmetHP1 = (int)HelmetHP1.EditValue;
            Config.HelmetHP11 = (int)HelmetHP11.EditValue;
            Config.HelmetHP2 = (int)HelmetHP2.EditValue;
            Config.HelmetHP22 = (int)HelmetHP22.EditValue;
            Config.HelmetHP3 = (int)HelmetHP3.EditValue;
            Config.HelmetHP33 = (int)HelmetHP33.EditValue;
            Config.HelmetMP1 = (int)HelmetMP1.EditValue;
            Config.HelmetMP11 = (int)HelmetMP11.EditValue;
            Config.HelmetMP2 = (int)HelmetMP2.EditValue;
            Config.HelmetMP22 = (int)HelmetMP22.EditValue;
            Config.HelmetMP3 = (int)HelmetMP3.EditValue;
            Config.HelmetMP33 = (int)HelmetMP33.EditValue;

            Config.HelmetGELE1 = (int)HelmetGELE1.EditValue;
            Config.HelmetGELE11 = (int)HelmetGELE11.EditValue;
            Config.HelmetGELE2 = (int)HelmetGELE2.EditValue;
            Config.HelmetGELE22 = (int)HelmetGELE22.EditValue;
            Config.HelmetGELE3 = (int)HelmetGELE3.EditValue;
            Config.HelmetGELE33 = (int)HelmetGELE33.EditValue;

            Config.HelmetELE1 = (int)HelmetELE1.EditValue;
            Config.HelmetELE11 = (int)HelmetELE11.EditValue;
            Config.HelmetELE2 = (int)HelmetELE2.EditValue;
            Config.HelmetELE22 = (int)HelmetELE22.EditValue;
            Config.HelmetELE3 = (int)HelmetELE3.EditValue;
            Config.HelmetELE33 = (int)HelmetELE33.EditValue;
            Config.RHelmetELE1 = (int)RHelmetELE1.EditValue;
            Config.RHelmetELE11 = (int)RHelmetELE11.EditValue;
            Config.RHelmetELE2 = (int)RHelmetELE2.EditValue;
            Config.RHelmetELE22 = (int)RHelmetELE22.EditValue;
            Config.RHelmetELE3 = (int)RHelmetELE3.EditValue;
            Config.RHelmetELE33 = (int)RHelmetELE33.EditValue;

            Config.NecklaceDC1 = (int)NecklaceDC1.EditValue;
            Config.NecklaceDC11 = (int)NecklaceDC11.EditValue;
            Config.NecklaceDC2 = (int)NecklaceDC2.EditValue;
            Config.NecklaceDC22 = (int)NecklaceDC22.EditValue;
            Config.NecklaceDC3 = (int)NecklaceDC3.EditValue;
            Config.NecklaceDC33 = (int)NecklaceDC33.EditValue;
            Config.NecklaceMSC1 = (int)NecklaceMSC1.EditValue;
            Config.NecklaceMSC11 = (int)NecklaceMSC11.EditValue;
            Config.NecklaceMSC2 = (int)NecklaceMSC2.EditValue;
            Config.NecklaceMSC22 = (int)NecklaceMSC22.EditValue;
            Config.NecklaceMSC3 = (int)NecklaceMSC3.EditValue;
            Config.NecklaceMSC33 = (int)NecklaceMSC33.EditValue;
            Config.NecklaceME1 = (int)NecklaceME1.EditValue;
            Config.NecklaceME11 = (int)NecklaceME11.EditValue;
            Config.NecklaceME2 = (int)NecklaceME2.EditValue;
            Config.NecklaceME22 = (int)NecklaceME22.EditValue;
            Config.NecklaceME3 = (int)NecklaceME3.EditValue;
            Config.NecklaceME33 = (int)NecklaceME33.EditValue;
            Config.NecklaceACC1 = (int)NecklaceACC1.EditValue;
            Config.NecklaceACC11 = (int)NecklaceACC11.EditValue;
            Config.NecklaceACC2 = (int)NecklaceACC2.EditValue;
            Config.NecklaceACC22 = (int)NecklaceACC22.EditValue;
            Config.NecklaceACC3 = (int)NecklaceACC3.EditValue;
            Config.NecklaceACC33 = (int)NecklaceACC33.EditValue;
            Config.NecklaceAG1 = (int)NecklaceAG1.EditValue;
            Config.NecklaceAG11 = (int)NecklaceAG11.EditValue;
            Config.NecklaceAG2 = (int)NecklaceAG2.EditValue;
            Config.NecklaceAG22 = (int)NecklaceAG22.EditValue;
            Config.NecklaceAG3 = (int)NecklaceAG3.EditValue;
            Config.NecklaceAG33 = (int)NecklaceAG33.EditValue;
            Config.NecklaceELE1 = (int)NecklaceELE1.EditValue;
            Config.NecklaceELE11 = (int)NecklaceELE11.EditValue;
            Config.NecklaceELE2 = (int)NecklaceELE2.EditValue;
            Config.NecklaceELE22 = (int)NecklaceELE22.EditValue;
            Config.NecklaceELE3 = (int)NecklaceELE3.EditValue;
            Config.NecklaceELE33 = (int)NecklaceELE33.EditValue;

            Config.BraceletDC1 = (int)BraceletDC1.EditValue;
            Config.BraceletDC11 = (int)BraceletDC11.EditValue;
            Config.BraceletDC2 = (int)BraceletDC2.EditValue;
            Config.BraceletDC22 = (int)BraceletDC22.EditValue;
            Config.BraceletDC3 = (int)BraceletDC3.EditValue;
            Config.BraceletDC33 = (int)BraceletDC33.EditValue;
            Config.BraceletMSC1 = (int)BraceletMSC1.EditValue;
            Config.BraceletMSC11 = (int)BraceletMSC11.EditValue;
            Config.BraceletMSC2 = (int)BraceletMSC2.EditValue;
            Config.BraceletMSC22 = (int)BraceletMSC22.EditValue;
            Config.BraceletMSC3 = (int)BraceletMSC3.EditValue;
            Config.BraceletMSC33 = (int)BraceletMSC33.EditValue;
            Config.BraceletAC1 = (int)BraceletAC1.EditValue;
            Config.BraceletAC11 = (int)BraceletAC11.EditValue;
            Config.BraceletAC2 = (int)BraceletAC2.EditValue;
            Config.BraceletAC22 = (int)BraceletAC22.EditValue;
            Config.BraceletAC3 = (int)BraceletAC3.EditValue;
            Config.BraceletAC33 = (int)BraceletAC33.EditValue;
            Config.BraceletMR1 = (int)BraceletMR1.EditValue;
            Config.BraceletMR11 = (int)BraceletMR11.EditValue;
            Config.BraceletMR2 = (int)BraceletMR2.EditValue;
            Config.BraceletMR22 = (int)BraceletMR22.EditValue;
            Config.BraceletMR3 = (int)BraceletMR3.EditValue;
            Config.BraceletMR33 = (int)BraceletMR33.EditValue;
            Config.BraceletACC1 = (int)BraceletACC1.EditValue;
            Config.BraceletACC11 = (int)BraceletACC11.EditValue;
            Config.BraceletACC2 = (int)BraceletACC2.EditValue;
            Config.BraceletACC22 = (int)BraceletACC22.EditValue;
            Config.BraceletACC3 = (int)BraceletACC3.EditValue;
            Config.BraceletACC33 = (int)BraceletACC33.EditValue;
            Config.BraceletAG1 = (int)BraceletAG1.EditValue;
            Config.BraceletAG11 = (int)BraceletAG11.EditValue;
            Config.BraceletAG2 = (int)BraceletAG2.EditValue;
            Config.BraceletAG22 = (int)BraceletAG22.EditValue;
            Config.BraceletAG3 = (int)BraceletAG3.EditValue;
            Config.BraceletAG33 = (int)BraceletAG33.EditValue;
            Config.BraceletELE1 = (int)BraceletGELE1.EditValue;
            Config.BraceletELE11 = (int)BraceletGELE11.EditValue;
            Config.BraceletELE2 = (int)BraceletGELE2.EditValue;
            Config.BraceletELE22 = (int)BraceletGELE22.EditValue;
            Config.BraceletELE3 = (int)BraceletGELE3.EditValue;
            Config.BraceletELE33 = (int)BraceletGELE33.EditValue;
            Config.BraceletElE1 = (int)BraceletElE1.EditValue;
            Config.BraceletElE11 = (int)BraceletElE11.EditValue;
            Config.BraceletElE2 = (int)BraceletElE2.EditValue;
            Config.BraceletElE22 = (int)BraceletElE22.EditValue;
            Config.BraceletElE3 = (int)BraceletElE3.EditValue;
            Config.BraceletElE33 = (int)BraceletElE33.EditValue;
            Config.RBraceletElE1 = (int)RBraceletElE1.EditValue;
            Config.RBraceletElE11 = (int)RBraceletElE11.EditValue;
            Config.RBraceletElE2 = (int)RBraceletElE2.EditValue;
            Config.RBraceletElE22 = (int)RBraceletElE22.EditValue;
            Config.RBraceletElE3 = (int)RBraceletElE3.EditValue;
            Config.RBraceletElE33 = (int)RBraceletElE33.EditValue;

            Config.RingDC1 = (int)RingDC1.EditValue;
            Config.RingDC11 = (int)RingDC11.EditValue;
            Config.RingDC2 = (int)RingDC2.EditValue;
            Config.RingDC22 = (int)RingDC22.EditValue;
            Config.RingDC3 = (int)RingDC3.EditValue;
            Config.RingDC33 = (int)RingDC33.EditValue;
            Config.RingMSC1 = (int)RingMSC1.EditValue;
            Config.RingMSC11 = (int)RingMSC11.EditValue;
            Config.RingMSC2 = (int)RingMSC2.EditValue;
            Config.RingMSC22 = (int)RingMSC22.EditValue;
            Config.RingMSC3 = (int)RingMSC3.EditValue;
            Config.RingMSC33 = (int)RingMSC33.EditValue;
            Config.RingELE1 = (int)RingELE1.EditValue;
            Config.RingELE11 = (int)RingELE11.EditValue;
            Config.RingELE2 = (int)RingELE2.EditValue;
            Config.RingELE22 = (int)RingELE22.EditValue;
            Config.RingELE3 = (int)RingELE3.EditValue;
            Config.RingELE33 = (int)RingELE33.EditValue;

            Config.ShoesAC1 = (int)ShoesAC1.EditValue;
            Config.ShoesAC11 = (int)ShoesAC11.EditValue;
            Config.ShoesAC2 = (int)ShoesAC2.EditValue;
            Config.ShoesAC22 = (int)ShoesAC22.EditValue;
            Config.ShoesAC3 = (int)ShoesAC3.EditValue;
            Config.ShoesAC33 = (int)ShoesAC33.EditValue;
            Config.ShoesMR1 = (int)ShoesMR1.EditValue;
            Config.ShoesMR11 = (int)ShoesMR11.EditValue;
            Config.ShoesMR2 = (int)ShoesMR2.EditValue;
            Config.ShoesMR22 = (int)ShoesMR22.EditValue;
            Config.ShoesMR3 = (int)ShoesMR3.EditValue;
            Config.ShoesMR33 = (int)ShoesMR33.EditValue;
            Config.ShoesHP1 = (int)ShoesHP1.EditValue;
            Config.ShoesHP11 = (int)ShoesHP11.EditValue;
            Config.ShoesHP2 = (int)ShoesHP2.EditValue;
            Config.ShoesHP22 = (int)ShoesHP22.EditValue;
            Config.ShoesHP3 = (int)ShoesHP3.EditValue;
            Config.ShoesHP33 = (int)ShoesHP33.EditValue;
            Config.ShoesMP1 = (int)ShoesMP1.EditValue;
            Config.ShoesMP11 = (int)ShoesMP11.EditValue;
            Config.ShoesMP2 = (int)ShoesMP2.EditValue;
            Config.ShoesMP22 = (int)ShoesMP22.EditValue;
            Config.ShoesMP3 = (int)ShoesMP3.EditValue;
            Config.ShoesMP33 = (int)ShoesMP33.EditValue;
            Config.ShoesCF1 = (int)ShoesCF1.EditValue;
            Config.ShoesCF11 = (int)ShoesCF11.EditValue;
            Config.ShoesCF2 = (int)ShoesCF2.EditValue;
            Config.ShoesCF22 = (int)ShoesCF22.EditValue;
            Config.ShoesCF3 = (int)ShoesCF3.EditValue;
            Config.ShoesCF33 = (int)ShoesCF33.EditValue;
            Config.ShoesELE1 = (int)ShoesELE1.EditValue;
            Config.ShoesELE11 = (int)ShoesELE11.EditValue;
            Config.ShoesELE2 = (int)ShoesELE2.EditValue;
            Config.ShoesELE22 = (int)ShoesELE22.EditValue;
            Config.ShoesELE3 = (int)ShoesELE3.EditValue;
            Config.ShoesELE33 = (int)ShoesELE33.EditValue;
            Config.RShoesELE1 = (int)RShoesELE1.EditValue;
            Config.RShoesELE11 = (int)RShoesELE11.EditValue;
            Config.RShoesELE2 = (int)RShoesELE2.EditValue;
            Config.RShoesELE22 = (int)RShoesELE22.EditValue;
            Config.RShoesELE3 = (int)RShoesELE3.EditValue;
            Config.RShoesELE33 = (int)RShoesELE33.EditValue;

            Config.ShieldDCP1 = (int)ShieldDCP1.EditValue;
            Config.ShieldDCP11 = (int)ShieldDCP11.EditValue;
            Config.ShieldDCP2 = (int)ShieldDCP2.EditValue;
            Config.ShieldDCP22 = (int)ShieldDCP22.EditValue;
            Config.ShieldDCP3 = (int)ShieldDCP3.EditValue;
            Config.ShieldDCP33 = (int)ShieldDCP33.EditValue;
            Config.ShieldMSCP1 = (int)ShieldMSCP1.EditValue;
            Config.ShieldMSCP11 = (int)ShieldMSCP11.EditValue;
            Config.ShieldMSCP2 = (int)ShieldMSCP2.EditValue;
            Config.ShieldMSCP22 = (int)ShieldMSCP22.EditValue;
            Config.ShieldMSCP3 = (int)ShieldMSCP3.EditValue;
            Config.ShieldMSCP33 = (int)ShieldMSCP33.EditValue;
            Config.ShieldBC1 = (int)ShieldBC1.EditValue;
            Config.ShieldBC11 = (int)ShieldBC11.EditValue;
            Config.ShieldBC2 = (int)ShieldBC2.EditValue;
            Config.ShieldBC22 = (int)ShieldBC22.EditValue;
            Config.ShieldBC3 = (int)ShieldBC3.EditValue;
            Config.ShieldBC33 = (int)ShieldBC33.EditValue;
            Config.ShieldEC1 = (int)ShieldEC1.EditValue;
            Config.ShieldEC11 = (int)ShieldEC11.EditValue;
            Config.ShieldEC2 = (int)ShieldEC2.EditValue;
            Config.ShieldEC22 = (int)ShieldEC22.EditValue;
            Config.ShieldEC3 = (int)ShieldEC3.EditValue;
            Config.ShieldEC33 = (int)ShieldEC33.EditValue;
            Config.ShieldPR1 = (int)ShieldPR1.EditValue;
            Config.ShieldPR11 = (int)ShieldPR11.EditValue;
            Config.ShieldPR2 = (int)ShieldPR2.EditValue;
            Config.ShieldPR22 = (int)ShieldPR22.EditValue;
            Config.ShieldPR3 = (int)ShieldPR3.EditValue;
            Config.ShieldPR33 = (int)ShieldPR33.EditValue;
            Config.ShieldELE1 = (int)ShieldELE1.EditValue;
            Config.ShieldELE11 = (int)ShieldELE11.EditValue;
            Config.ShieldELE2 = (int)ShieldELE2.EditValue;
            Config.ShieldELE22 = (int)ShieldELE22.EditValue;
            Config.ShieldELE3 = (int)ShieldELE3.EditValue;
            Config.ShieldELE33 = (int)ShieldELE33.EditValue;
            Config.RShieldELE1 = (int)RShieldELE1.EditValue;
            Config.RShieldELE11 = (int)RShieldELE11.EditValue;
            Config.RShieldELE2 = (int)RShieldELE2.EditValue;
            Config.RShieldELE22 = (int)RShieldELE22.EditValue;
            Config.RShieldELE3 = (int)RShieldELE3.EditValue;
            Config.RShieldELE33 = (int)RShieldELE33.EditValue;

            Config.SkillExp = (int)SkillExpEdit.EditValue;
            Config.MaxMagicLv = (int)MaxMagicLvEdit.EditValue;
            Config.SkillExpDrop = (int)SkillExpDropEdit.EditValue;
            Config.CanFlyTargetCheck = (bool)CanFlyTargetCheckEdit.EditValue;
            Config.QKDNY = (bool)QKDNYEdit.EditValue;
            Config.BeckonParalysis = (bool)BeckonParalysisCheckEdit.EditValue;
            Config.ZTBS = (bool)ZTBSCheckEdit.EditValue;
            Config.PXKS = (bool)ZPXKSCheckEdit.EditValue;
            Config.ExpelUndeadSuccessRate = (int)ExpelUndeadSuccessRateEdit.EditValue;
            Config.ExpelUndeadLevel = (int)ExpelUndeadLevelTextEdit.EditValue;
            Config.MirrorImageCanMove = (bool)MirrorImageCanMoveCheckEdit.EditValue;
            Config.MirrorImageDamageDarkStone = (int)MirrorImageDamageDarkStone.EditValue;
            Config.ThunderStrikeGetCells = (int)ThunderStrikeGetCellsTextEdit.EditValue;
            Config.ThunderStrikeRandom = (int)ThunderStrikeRandomTextEdit.EditValue;
            Config.MagicShieldRemainingTime = (int)MagicShieldRemainingTime.EditValue;
            Config.MeteorShowerTargetsCount = (int)MeteorShowerTargetsCount.EditValue;
            Config.PetsMutinyTime = (int)PetsMutinyTimeTextEdit.EditValue;
            Config.ElectricShockPetsCount = (int)ElectricShockPetsCount.EditValue;
            Config.ElectricShockSuccessRate = (int)ElectricShockSuccessRateEdit.EditValue;
            Config.YXHW = (bool)YXHWEdit.EditValue;
            Config.GeoManipulationSuccessRate = (int)GeoManipulationSuccessRate.EditValue;
            Config.MYWZYX = (bool)MYWZYXEdit.EditValue;
            Config.ResurrectionOrder = (bool)ResurrectionOrderCheckEdit.EditValue;
            Config.PoisonDustValue = (float)PoisonDustValueEdit.EditValue;
            Config.PoisonDead = (bool)PoisonDeadCheckEdit.EditValue;
            Config.PoisoningBossCheck = (bool)PoisoningBossCheckEdit.EditValue;
            Config.PoisonDustDarkAttack = (bool)PoisonDustDarkAttack.EditValue;
            Config.RedPoisonAttackRate = (float)RedPoisonAttackRateTextEdit.EditValue;
            Config.ETDarkAffinityRate = (float)ETDarkAffinityRateTextEdit.EditValue;
            Config.SummonRandomValue = (int)SummonRandomValueTextEdit.EditValue;
            Config.TraOverTime = (int)TraOverTimeTextEdit.EditValue;
            Config.FixedCure = (bool)FixedCureCheckEdit.EditValue;
            Config.FixedCureValue = (int)FixedCureValueTextEdit.EditValue;
            Config.DemonExplosionPVP = (int)DemonExplosionPVPEdit.EditValue;
            Config.MYWZHY = (bool)MYWZHYEdit.EditValue;
            Config.ZHDKHP = (int)ZHDKHPEdit.EditValue;
            Config.ZHDKFW = (int)ZHDKFWEdit.EditValue;
            Config.ZZDKZ = (bool)ZZDKZEdit.EditValue;
            Config.YJDTYD = (bool)YJDTYDEdit.EditValue;
            Config.DanceOfSwallowSilenced = (bool)DanceOfSwallowSilencedCheckEdit.EditValue;
            Config.DanceOfSwallowParalysis = (bool)DanceOfSwallowParalysisCheckEdit.EditValue;

            //Guild
            Config.StarterGuildRebirth = (int)StarterGuildRebirthEdit.EditValue;
            Config.GuildCreationCost = (int)GuildCreationCostEdit.EditValue;
            Config.GuildMemberCost = (int)GuildMemberCostEdit.EditValue;
            Config.GuildStorageCost = (int)GuildStorageCostEdit.EditValue;
            Config.GuildMemberHardLimit = (int)GuildMemberHardLimitEdit.EditValue;
            Config.GuildWarCost = (int)GuildWarCostEdit.EditValue;
            Config.VictoryDelay = (int)VictoryDelayTextEdit.EditValue;
            Config.ConquestFlagDelay = (bool)ConquestFlagDelayCheckEdit.EditValue;
            Config.AttackerReviveInDesignatedAreaDuringWar = (bool)AttackerReviveInDesignatedAreaDuringWar.EditValue;
            Config.AllowTeleportMagicNearFlag = (bool)AllowTeleportMagicNearFlag.EditValue;
            Config.WarFlagRangeLimit = (int)WarFlagRangeLimit.EditValue;

            Config.FlagSuccessTeleport = (bool)FlagSuccessTeleportCheck.EditValue;
            Config.EndWarTeleport = (bool)EndWarTeleportCheck.EditValue;
            Config.GuildMaxFund = (long)GuildMaxFundTextEdit.EditValue;
            Config.ActivationCeiling = (int)ActivationCeilingTextEdit.EditValue;
            Config.PersonalGoldRatio = (int)PersonalGoldRatioTextEdit.EditValue;
            Config.PersonalExpRatio = (int)PersonalExpRatioTextEdit.EditValue;

            //大补帖
            Config.AutoTime = (int)AutoTimeEdit.EditValue;             //挂机时间
            Config.AutoHookTab = (bool)AutoHookTabEdit.EditValue;   //自动挂机
            Config.HooKDrop = (int)HooKDropEdit.EditValue;          //挂机爆率
            Config.BrightBox = (bool)BrightBoxEdit.EditValue;      //免蜡
            Config.RunCheck = (bool)RunCheckEdit.EditValue;        //免助跑
            Config.RockCheck = (bool)RockCheckEdit.EditValue;      //稳如泰山
            Config.ClearBodyCheck = (bool)ClearBodyCheckEdit.EditValue;   //清理尸体
            Config.BigPatchAutoPotionDelay = (int)BigPatchAutoPotionDelay.EditValue;  //喝药延迟毫秒

            //内核防挂
            Config.GlobalsAttackDelay = (int)AttackDelayTextEdit.EditValue;
            Config.GlobalsASpeedRate = (int)ASpeedRateTextEdit.EditValue;
            Config.GlobalsProjectileSpeed = (int)ProjectileSpeedTextEdit.EditValue;
            Config.GlobalsTurnTime = (int)TurnTimeTextEdit.EditValue;
            Config.GlobalsHarvestTime = (int)HarvestTimeTextEdit.EditValue;
            Config.GlobalsMoveTime = (int)MoveTimeTextEdit.EditValue;
            Config.GlobalsAttackTime = (int)AttackTimeTextEdit.EditValue;
            Config.GlobalsCastTime = (int)CastTimeTextEdit.EditValue;
            Config.GlobalsMagicDelay = (int)MagicDelayTextEdit.EditValue;

            File.Delete(Application.StartupPath + "\\Database\\BanCPU.txt");
            File.Delete(Application.StartupPath + "\\Database\\BanHDD.txt");
            File.Delete(Application.StartupPath + "\\Database\\BanMAC.txt");
            File.AppendAllText(Application.StartupPath + "\\Database\\BanCPU.txt", richTextBox_BanCPU.Text, System.Text.Encoding.Default);
            File.AppendAllText(Application.StartupPath + "\\Database\\BanHDD.txt", richTextBox_BanHDD.Text, System.Text.Encoding.Default);
            File.AppendAllText(Application.StartupPath + "\\Database\\BanMAC.txt", richTextBox_BanMAC.Text, System.Text.Encoding.Default);

            Config.LicenseFile = (string)LicenseFile.EditValue; //授权文件位置

            if (SEnvir.Started)
            {
                SEnvir.ServerBuffChanged = true;
                SEnvir.LoadBanIpInfo();
                SEnvir.LoadSha256ProcessBanList();
            }

            ConfigReader.Save();
        }

        public void LoadLicenseInfo(bool showLog = false)
        {
            bool refresh = Helpers.LicenseHelper.ReLoadLicense(Config.LicenseFile);

            HardwareIDTextBox.Text = Helpers.LicenseHelper.CurrentHardwareID;
            LicenseState status = Helpers.LicenseHelper.CheckLicense();
            string statusText = Functions.GetEnumDescription(status);

            SEnvir.LicenseValid = (int)status >= (int)LicenseState.Licensed;
            SEnvir.LicenseState = statusText;

            if (showLog)
            {
                SEnvir.Log($"授权位置={Config.LicenseFile}");
                SEnvir.Log($"授权状态={statusText}");
                SEnvir.Log($"可以启动={SEnvir.LicenseValid}");
            }

            CurrentLicenseStatus.Text = statusText;

            switch (status)
            {
                case LicenseState.Invalid:
                    CurrentLicenseStatus.ForeColor = Color.Red;
                    break;
                case LicenseState.Missing:
                    CurrentLicenseStatus.ForeColor = Color.Red;
                    break;
                case LicenseState.Expired:
                    CurrentLicenseStatus.ForeColor = Color.Orange;
                    break;
                case LicenseState.HardwareNotMatched:
                    CurrentLicenseStatus.ForeColor = Color.Orange;
                    break;
                case LicenseState.EvaluationExpired:
                    CurrentLicenseStatus.ForeColor = Color.Red;
                    break;
                case LicenseState.UsesExceeded:
                    CurrentLicenseStatus.ForeColor = Color.Orange;
                    break;

                case LicenseState.Licensed:
                    CurrentLicenseStatus.ForeColor = Color.LimeGreen;
                    break;
                case LicenseState.DebugMode:
                    CurrentLicenseStatus.ForeColor = Color.Purple;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            RefreshLicenseInfo();
        }

        private void RefreshLicenseInfo()
        {
            CurrentVersionStatus.Text = Helpers.LicenseHelper.IsEvaluation ? "试用版" : "商业版";

            EvalutionTypeLabel.Text = Helpers.LicenseHelper.EvaluationType;

            EvalutionReaminingTimeLabel.Text =
                Helpers.LicenseHelper.IsEvaluation ? (Helpers.LicenseHelper.EvaluationRemainingTime).ToString() : "无限制";

            HardwareIDTextBox2.Text = Helpers.LicenseHelper.CurrentHardwareID;
            HardwareIDFromLicenseTextBox.Text = Helpers.LicenseHelper.LicenseHardwareID;
            HardwareLockCheckBox.Checked = Helpers.LicenseHelper.HardwareLockEnabled;
            HardwareIDMatchCheckBox.Checked = Helpers.LicenseHelper.HardwareMatch;

            LicenseExistsCheckBox.Checked = File.Exists(Config.LicenseFile);

            listView1.Items.Clear();
            foreach (var kvp in Helpers.LicenseHelper.GetAdditionalInfo())
            {
                listView1.Items.Add(new ListViewItem(new string[]
                {
                    kvp.Key, kvp.Value
                }));
            }

        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveSettings();
        }
        private void ReloadButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadSettings();
        }

        private void MailActivate_CheckedChanged(object sender, EventArgs e)
        {
            Config.MailActivate = MailActivate.Checked;
        }
        private void CheckVersionButton_Click(object sender, EventArgs e)
        {
            byte[] old = Config.ClientHash;
            byte[] old1 = Config.ClientHash1;

            Config.LoadVersion();

            if (Functions.IsMatch(old, Config.ClientHash) || Functions.IsMatch(old1, Config.ClientHash1) || !SEnvir.Started) return;

            SEnvir.Broadcast(new S.Chat { Text = "已提供新版本，请尽快更新。", Type = MessageType.Announcement });
        }
        private void VersionPathEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (OpenDialog.ShowDialog() != DialogResult.OK) return;

            VersionPathEdit.EditValue = OpenDialog.FileName;
        }
        private void VersionPath1Edit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (Open1Dialog.ShowDialog() != DialogResult.OK) return;

            VersionPath1Edit.EditValue = Open1Dialog.FileName;
        }
        private void MapPathEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (FolderDialog.ShowDialog() != DialogResult.OK) return;

            MapPathEdit.EditValue = FolderDialog.SelectedPath;
        }

        private void ClientPathEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (FolderDialog.ShowDialog() != DialogResult.OK) return;

            ClientPathEdit.EditValue = FolderDialog.SelectedPath;
        }

        private void LicenseFile_EditValueChanged(object sender, EventArgs e)
        {
            Config.LicenseFile = (string)LicenseFile.EditValue;

            LoadLicenseInfo(true);
        }

        private void InvalidateButton_Click(object sender, EventArgs e)
        {
            string message = "注意: 移除授权后，本机将无法使用任何授权文件，需要激活码才可以重新激活。是否继续？";
            string caption = "警告";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, caption, buttons);

            if (result == DialogResult.Yes)
            {
                string confirmation = Helpers.LicenseHelper.InvalidateLicense();

                if (Helpers.LicenseHelper.CheckConfirmationCode(confirmation))
                {
                    string message2 = "成功生成解绑确认码, 本机将无法再使用任何授权";
                    string caption2 = "解绑成功";
                    MessageBoxButtons buttons2 = MessageBoxButtons.YesNo;
                    DialogResult result2 = MessageBox.Show(message2, caption2, buttons2);
                }
                else
                {
                    string message2 = "生成失败, 请确认授权文件是否正常,以及是否已经解绑";
                    string caption2 = "解绑失败";
                    MessageBoxButtons buttons2 = MessageBoxButtons.OK;
                    DialogResult result2 = MessageBox.Show(message2, caption2, buttons2);
                }

                ConfirmationKeyTextBox.Text = confirmation;
                LoadLicenseInfo(true);
            }
        }

        private void ReactivateButton_Click(object sender, EventArgs e)
        {
            string reactivationCode = ReactivationCodeTextBox.Text;
            if (Helpers.LicenseHelper.ReactivateLicense(reactivationCode))
            {
                string message = "重新激活成功,请选择授权文件";
                string caption = "激活成功";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result = MessageBox.Show(message, caption, buttons);
                LoadLicenseInfo(true);
            }
            else
            {
                string message = "重新激活失败,请检查激活码";
                string caption = "激活失败";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result = MessageBox.Show(message, caption, buttons);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            string message = "刷新成功,请查看授权信息是否正确";
            string caption = "成功";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result = MessageBox.Show(message, caption, buttons);
            LoadLicenseInfo(true);
        }

        public PlayerObject CurPlayer;

        private void RefreshButton_Click(object sender, EventArgs e)  //刷新在线角色列表
        {
            RefreshOnLinePlayer();
        }

        private void PreserveButton_Click(object sender, EventArgs e)  //保存角色修改
        {
            SaveSelectPlayerInfo();
        }

        private void KickButton_Click(object sender, EventArgs e)  //把在线玩家踢出游戏
        {
            var objectid = CurPlayer.ObjectID;
            CurPlayer = null;
            for (var i = 0; i < SEnvir.Players.Count; i++)
            {
                if (SEnvir.Players[i]?.ObjectID == objectid)
                {
                    CurPlayer = SEnvir.Players[i];
                    break;
                }
            }

            if (CurPlayer != null)  //如果角色不等空
            {
                SEnvir.Log($"将玩家[{CurPlayer.Character.CharacterName}]踢出游戏");
                CurPlayer.Character.Account.Connection.SendDisconnect(new Disconnect
                {
                    Reason = DisconnectReason.Unknown
                });

                CurPlayer = null;
                OnLinePlayerNameEdit.Text = string.Empty;
                OnLinePlayerLevelEdit.Text = string.Empty;

                OnLinePlayerJobEdit.Text = string.Empty;

                OnLinePlayerMapEdit.Text = string.Empty;
                OnLinePlayerGoldEdit.Text = string.Empty;
                OnLinePlayerGameGoldEdit.Text = string.Empty;
                OnLinePlayerHutGoldEdit.Text = string.Empty;
                OnLinePlayerPrestigeEdit.Text = string.Empty;
                OnLinePlayerContributeEdit.Text = string.Empty;
                OnLinePlayerGMCheckBox.Checked = false;

                OnLinePlayerIDEdit.Tag = null;
                RefreshOnLinePlayer();
            }
        }

        public void RefreshOnLinePlayer()   //刷新在线玩家
        {
            OnLinePlayerlistView.BeginUpdate();  //开始更新
            OnLinePlayerlistView.Items.Clear();  //先清理所有项
            if (SEnvir.Players.Count > 0)    //当前连接数大于0
            {
                int index = 0;
                foreach (PlayerObject player in SEnvir.Players)
                {
                    ListViewItem listViewItem = new ListViewItem();
                    listViewItem.Text = (index + 1).ToString();                                     //输出序号增值
                    listViewItem.Tag = player.ObjectID;
                    listViewItem.SubItems.Add(player.Character.Account.ToString());  //输出角色账号
                    listViewItem.SubItems.Add(player.Character.CharacterName);       //输出角色名字
                    listViewItem.SubItems.Add(player.Character.Level.ToString());    //输出角色等级
                    listViewItem.SubItems.Add(player.Character.Account.LastIP);      //输出登录的IP地址
                    OnLinePlayerlistView.Items.Add(listViewItem);
                    index++;
                }
            }
            OnLinePlayerlistView.EndUpdate(); //结束更新
        }

        public void UpdateSelectPlayerInfo(uint objectId)  //更新选择玩家信息
        {
            CurPlayer = null;
            for (var i = 0; i < SEnvir.Players.Count; i++)
            {
                if (SEnvir.Players[i]?.ObjectID == objectId)
                {
                    CurPlayer = SEnvir.Players[i];
                    break;
                }
            }

            if (CurPlayer != null)  //如果玩家不为空
            {
                OnLinePlayerNameEdit.Text = CurPlayer.Name;                                           //输出玩家名字
                OnLinePlayerLevelEdit.Text = CurPlayer.Level.ToString();                              //输出玩家等级

                Type type = typeof(MirClass);
                MemberInfo[] infos = type.GetMember(CurPlayer.Class.ToString());
                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();
                OnLinePlayerJobEdit.Text = description?.Description ?? CurPlayer.Class.ToString();    //输出玩家职业

                OnLinePlayerMapEdit.Text = CurPlayer.CurrentMap.Info.Description;                     //输出玩家当前所在地图
                OnLinePlayerGoldEdit.Text = CurPlayer.Gold.ToString();                                //输出玩家金币
                OnLinePlayerGameGoldEdit.Text = CurPlayer.Character.Account.GameGold.ToString();      //输出玩家元宝
                OnLinePlayerHutGoldEdit.Text = CurPlayer.Character.Account.HuntGold.ToString();       //输出玩家赏金
                OnLinePlayerPrestigeEdit.Text = CurPlayer.Character.Prestige.ToString();              //输出玩家声望
                OnLinePlayerContributeEdit.Text = CurPlayer.Character.Contribute.ToString();          //输出玩家贡献

                OnLinePlayerGMCheckBox.Checked = CurPlayer.Character.Account.TempAdmin;               //显示玩家是否管理员

                OnLinePlayerIDEdit.Tag = CurPlayer.Character.Account.EMailAddress;

            }
        }

        public void SaveSelectPlayerInfo()   //保存在线角色信息
        {
            PlayerObject player = null;
            for (var i = 0; i < SEnvir.Players.Count; i++)
            {
                if (CurPlayer.ObjectID == SEnvir.Players[i]?.ObjectID)
                {
                    player = SEnvir.Players[i];
                    break;
                }
            }
            if (player != null)  //如果玩家不为空
            {
                try
                {
                    List<AccountInfo> AccountInfoList = SEnvir.AccountInfoList.Binding as List<AccountInfo>;
                    AccountInfo AccountInfo = AccountInfoList.Where(o => o.EMailAddress == OnLinePlayerIDEdit.Tag.ToString()).FirstOrDefault();

                    AccountInfo.Gold = long.Parse(OnLinePlayerGoldEdit.Text);
                    AccountInfo.GameGold = Convert.ToInt32(OnLinePlayerGameGoldEdit.Text);
                    AccountInfo.HuntGold = Convert.ToInt32(OnLinePlayerHutGoldEdit.Text);
                    AccountInfo.TempAdmin = OnLinePlayerGMCheckBox.Checked;

                    List<CharacterInfo> CharacterInfoList = SEnvir.CharacterInfoList.Binding as List<CharacterInfo>;
                    CharacterInfo CharacterInfo = CharacterInfoList.Where(o => o.CharacterName == OnLinePlayerNameEdit.Text.Trim()).FirstOrDefault();

                    CharacterInfo.Level = Convert.ToInt32(OnLinePlayerLevelEdit.Text);
                    CharacterInfo.Contribute = Convert.ToInt32(OnLinePlayerContributeEdit.Text);
                    CharacterInfo.Prestige = Convert.ToInt32(OnLinePlayerPrestigeEdit.Text);

                    SMain.Session.Save(true, SessionMode.Server);

                    player.Character.Account.Connection.SendDisconnect(new Disconnect
                    {
                        Reason = DisconnectReason.Unknown
                    });

                    RefreshOnLinePlayer();
                }
                catch { }
            }
        }

        private void OnLinePlayerlistView_SelectedIndexChanged(object sender, EventArgs e)
        {
            //CurPlayer = null;
            try
            {
                if (OnLinePlayerlistView.SelectedItems.Count != 0)    //如果选定项目的计数不等0
                {
                    //CurPlayer = OnLinePlayerlistView.SelectedItems[0].Tag as PlayerObject;

                    if (uint.TryParse(OnLinePlayerlistView.SelectedItems[0].Tag?.ToString() ?? "", out uint objectId))
                    {
                        OnLinePlayerIDEdit.Text = OnLinePlayerlistView.SelectedItems[0].Text;
                        UpdateSelectPlayerInfo(objectId);
                    }

                    //OnLinePlayerIDEdit.Text = OnLinePlayerlistView.SelectedItems[0].Text;

                    //int result = -1;
                    //if (int.TryParse(OnLinePlayerIDEdit.Text, out result) && result - 1 < SEnvir.Connections.Count)
                    //{
                    //    CurPlayer = SEnvir.Players[result - 1];
                    //}
                    //UpdateSelectPlayerInfo();
                }
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.Message + ex.StackTrace);
            }
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
            if (Config.EnableDBEncryption == EncryptDBToggleSwitch.IsOn) return;
            Config.EnableDBEncryption = EncryptDBToggleSwitch.IsOn;

            SMain.Session.XOREncrypt = EncryptDBToggleSwitch.IsOn;
            string key = (string)EncryptDBPassword.EditValue;
            SMain.Session.XORKey = string.IsNullOrEmpty(key) ? Config.DBPassword : key;
        }

        //关闭服务器自动广告功能  定时器
        private void checkAutoCloseServer_CheckedChanged(object sender, EventArgs e)
        {
            if (checkAutoCloseServer.Checked)
            {
                if (!SEnvir.Started)
                {
                    MessageBox.Show("服务器启动中才能使用！");
                    checkAutoCloseServer.Checked = false;
                    return;
                }
                //if (Config.AutoCloseServerText == "")
                //{
                //    MessageBox.Show("请填写好提示语句！");
                //    checkAutoCloseServer.Checked = false;
                //    return;
                //}

                SEnvir.TimerSecondTick = Config.CloseServerTime;
                Config.CheckAutoCloseServer = true;
            }
            else
            {
                Config.CheckAutoCloseServer = false;
            }
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