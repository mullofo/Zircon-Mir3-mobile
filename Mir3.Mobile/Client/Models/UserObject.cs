using Client.Envir;
using Client.Extentions;
using Client.Scenes;
using Client.Scenes.Configs;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C = Library.Network.ClientPackets;

namespace Client.Models
{
    /// <summary>
    /// 用户对象 玩家
    /// </summary>
    public sealed class UserObject : PlayerObject
    {
        #region Stats
        /// <summary>
        /// 属性状态
        /// </summary>
        public override Stats Stats
        {
            get { return _Stats; }
            set
            {
                _Stats = value;

                GameScene.Game.StatsChanged();
            }
        }
        private Stats _Stats = new Stats();
        #endregion

        #region Hermit Stats
        /// <summary>
        /// 额外加点
        /// </summary>
        public Stats HermitStats
        {
            get { return _HermitStats; }
            set
            {
                if (_HermitStats == value) return;

                _HermitStats = value;

                GameScene.Game.StatsChanged();
            }
        }
        private Stats _HermitStats = new Stats();

        #endregion

        #region Level
        /// <summary>
        /// 等级
        /// </summary>
        public override int Level
        {
            get { return _level; }
            set
            {
                _level = value;

                GameScene.Game.LevelChanged();
            }
        }
        private int _level;
        #endregion

        #region Class
        /// <summary>
        /// 职业
        /// </summary>
        public override MirClass Class
        {
            get { return _Class; }
            set
            {
                _Class = value;

                GameScene.Game.ClassChanged();
            }
        }
        private MirClass _Class;
        #endregion

        #region Experience
        /// <summary>
        /// 经验值
        /// </summary>
        public decimal Experience
        {
            get { return _Experience; }
            set
            {
                _Experience = value;

                GameScene.Game.ExperienceChanged();
            }
        }
        private decimal _Experience;
        #endregion

        #region MaxExperience
        /// <summary>
        /// 最大经验值
        /// </summary>
        public decimal MaxExperience
        {
            get { return _MaxExperience; }
            set
            {
                _MaxExperience = value;

                GameScene.Game.ExperienceChanged();
            }
        }
        private decimal _MaxExperience;
        #endregion

        #region CurrentHP
        /// <summary>
        /// 当前HP
        /// </summary>
        public override int CurrentHP
        {
            get { return _CurrentHP; }
            set
            {
                _CurrentHP = value;

                GameScene.Game.HealthChanged();
            }
        }
        private int _CurrentHP;
        #endregion

        #region CurrentMP
        /// <summary>
        /// 当前MP
        /// </summary>
        public override int CurrentMP
        {
            get { return _CurrentMP; }
            set
            {
                _CurrentMP = value;

                GameScene.Game.ManaChanged();
            }
        }
        private int _CurrentMP;
        #endregion

        #region AttackMode
        /// <summary>
        /// 攻击模式
        /// </summary>
        public AttackMode AttackMode
        {
            get { return _AttackMode; }
            set
            {
                _AttackMode = value;

                GameScene.Game.AttackModeChanged();
            }
        }
        private AttackMode _AttackMode;
        #endregion

        #region PetMode
        /// <summary>
        /// 宠物攻击模式
        /// </summary>
        public PetMode PetMode
        {
            get { return _PetMode; }
            set
            {
                _PetMode = value;

                GameScene.Game.PetModeChanged();
            }
        }
        private PetMode _PetMode;
        #endregion

        #region Gold
        /// <summary>
        /// 金币
        /// </summary>
        public long Gold
        {
            get { return _Gold; }
            set
            {
                _Gold = value;

                GameScene.Game.GoldChanged();
            }
        }
        private long _Gold;
        #endregion

        #region Game Gold        
        /// <summary>
        /// 元宝
        /// </summary>
        public long GameGold
        {
            get { return _GameGold; }
            set
            {
                _GameGold = value;

                GameScene.Game.GoldChanged();
            }
        }
        private long _GameGold;
        #endregion

        #region Atuo Time
        /// <summary>
        /// 自动挂机时间
        /// </summary>
        public long AutoTime
        {
            get { return _AutoTime; }
            set
            {
                _AutoTime = value;

                GameScene.Game.AutoTimeChanged();
            }
        }
        private long _AutoTime;
        #endregion

        #region Hunt Gold
        /// <summary>
        /// 赏金
        /// </summary>
        public int HuntGold
        {
            get { return _HuntGold; }
            set
            {
                _HuntGold = value;

                GameScene.Game.GoldChanged();
            }
        }
        private int _HuntGold;
        #endregion

        #region Prestige
        /// <summary>
        /// 声望
        /// </summary>
        public int Prestige
        {
            get { return _Prestige; }
            set
            {
                _Prestige = value;

                GameScene.Game.GoldChanged();
            }
        }
        private int _Prestige;
        #endregion

        #region Contribute
        /// <summary>
        /// 贡献
        /// </summary>
        public int Contribute
        {
            get { return _Contribute; }
            set
            {
                _Contribute = value;

                GameScene.Game.GoldChanged();
            }
        }
        private int _Contribute;
        #endregion

        #region 制造系统
        /// <summary>
        /// 制作等级
        /// </summary>
        public override int CraftLevel
        {
            get { return _CraftLevel; }
            set
            {
                if (_CraftLevel == value) return;
                _CraftLevel = value;
                GameScene.Game.CraftStatsChanged();
            }
        }
        private int _CraftLevel;
        /// <summary>
        /// 制作熟练度
        /// </summary>
        public override int CraftExp
        {
            get { return _CraftExp; }
            set
            {
                if (_CraftExp == value) return;
                _CraftExp = value;
                GameScene.Game.CraftStatsChanged();
            }
        }
        private int _CraftExp;
        /// <summary>
        /// 制作完成时间
        /// </summary>
        public override DateTime CraftFinishTime
        {
            get { return _CraftFinishTime; }
            set
            {
                if (_CraftFinishTime == value) return;
                _CraftFinishTime = value;
                //GameScene.Game.CraftResultBox.UpdateCraftingStatus();
            }
        }
        private DateTime _CraftFinishTime;
        /// <summary>
        /// 快捷列表添加制作物品
        /// </summary>
        public override CraftItemInfo BookmarkedCraftItemInfo
        {
            get { return _BookmarkedCraftItemInfo; }
            set
            {
                if (_BookmarkedCraftItemInfo == value) return;
                _BookmarkedCraftItemInfo = value;
                GameScene.Game.CraftBookmarkChanged();
            }
        }
        private CraftItemInfo _BookmarkedCraftItemInfo;
        /// <summary>
        /// 正在制作的物品
        /// </summary>
        public override CraftItemInfo CraftingItem
        {
            get { return _CraftingItem; }
            set
            {
                if (_CraftingItem == value) return;
                _CraftingItem = value;
                //GameScene.Game.CraftResultBox.UpdateCraftingStatus();
            }
        }
        private CraftItemInfo _CraftingItem;
        #endregion

        #region 钓鱼

        public DateTime FishingStartTime { get; set; }
        public DateTime FishingEndTime { get; set; }
        public DateTime FishingPerfectTime { get; set; }
        public bool HasSentFishNotification { get; set; }

        public void ResetFishing()
        {
            IsFishing = false;
            FishingStartTime = DateTime.MaxValue;
            FishingEndTime = DateTime.MaxValue;
            HasSentFishNotification = false;
            GameScene.Game.FishingStatusBox.blink.Visible = false;
            GameScene.Game.FishingStatusBox.HideRipple();
            ActionQueue.RemoveAll(x => x.Action == MirAction.FishingWait);
        }
        public void CheckFishing()
        {
            if (IsFishing && CEnvir.Now > FishingEndTime)
            {
                ResetFishing();
                return;
            }

            if (IsFishing && CEnvir.Now > FishingPerfectTime && !HasSentFishNotification)
            {
                GameScene.Game.ReceiveChat("似乎有鱼儿咬钩了".Lang(), MessageType.Hint);
                HasSentFishNotification = true;
                GameScene.Game.FishingStatusBox.blink.Visible = true;
            }

            /*
            if (IsFishing && CEnvir.Now > FishingStartTime && CEnvir.Now < FishingEndTime)
            {
                if (ActionQueue.Count == 0 || ActionQueue[0].Action != MirAction.FishingWait)
                {
                    ActionQueue.Add(new ObjectAction(MirAction.FishingWait, User.Direction, User.CurrentLocation, MagicType.None));
                    //继续播放水波动画
                    //GameScene.Game.FishingStatusBox.ShowRipple();
                }
            }
            */
        }

        #endregion

        #region 奖金池

        public decimal RewardPoolCoin { get; set; }

        #endregion

        public bool YeManCanRun = false;

        /// <summary>
        /// 当前可重复任务次数
        /// </summary>
        public override int RepeatableQuestRemains
        {
            get { return _repeatableQuestRemains; }
            set
            {
                if (_repeatableQuestRemains == value) return;
                _repeatableQuestRemains = value;
                //GameScene.Game.CraftStatsChanged();
            }
        }
        private int _repeatableQuestRemains;
        /// <summary>
        /// 当前每日任务已完成次数
        /// </summary>
        public override int DailyQuestRemains
        {
            get { return _dailyQuestRemains; }
            set
            {
                if (_dailyQuestRemains == value) return;
                _dailyQuestRemains = value;
                //GameScene.Game.CraftStatsChanged();
            }
        }
        private int _dailyQuestRemains;

        public int BagWeight, WearWeight, HandWeight;
        /// <summary>
        /// 震动屏幕计数
        /// </summary>
        public float ShakeScreenCount;
        /// <summary>
        /// 震动屏幕偏移
        /// </summary>
        public Point ShakeScreenOffset;
        public DateTime ShakeScreenTime = CEnvir.Now;
        public DateTime SpeedDelay;
        /// <summary>
        /// 安全区
        /// </summary>
        public bool InSafeZone
        {
            get { return _InSafeZone; }
            set
            {
                if (_InSafeZone == value) return;

                _InSafeZone = value;

                GameScene.Game.SafeZoneChanged();
            }
        }
        private bool _InSafeZone;
        /// <summary>
        /// 额外加点点数
        /// </summary>
        public int HermitPoints;
        /// <summary>
        /// BUFF信息列表
        /// </summary>
        public List<ClientBuffInfo> Buffs = new List<ClientBuffInfo>();
        /// <summary>
        /// 角色魔法技能信息
        /// </summary>
        public Dictionary<MagicInfo, ClientUserMagic> Magics = new Dictionary<MagicInfo, ClientUserMagic>();

        public DateTime NextActionTime, ServerTime, AttackTime, NextRunTime, NextMagicTime, BuffTime = CEnvir.Now, LotusTime, CombatTime, MoveTime, WarReviveTime;
        /// <summary>
        /// 魔法类型 攻击魔法
        /// </summary>
        public MagicType AttackMagic;
        /// <summary>
        /// 魔法动作
        /// </summary>
        public ObjectAction MagicAction;
        /// <summary>
        /// 可以强行攻击
        /// </summary>
        public bool CanPowerAttack;
        /// <summary>
        /// 刺杀剑术开关
        /// </summary>
        public bool CanThrusting
        {
            get { return _canThrusting; }
            set
            {
                if (_canThrusting == value) return;

                _canThrusting = value;

                GameScene.Game.ReceiveChat(CanThrusting ? "刺杀剑术已开启".Lang() : "刺杀剑术已关闭".Lang(), MessageType.Hint);
            }
        }
        private bool _canThrusting;
        /// <summary>
        /// 半月开关
        /// </summary>
        public bool CanHalfMoon
        {
            get { return _CanHalfMoon; }
            set
            {
                if (_CanHalfMoon == value) return;

                _CanHalfMoon = value;

                GameScene.Game.ReceiveChat(CanHalfMoon ? "半月弯刀已开启".Lang() : "半月弯刀已关闭".Lang(), MessageType.Hint);
            }
        }
        private bool _CanHalfMoon;
        /// <summary>
        /// 十方斩开关
        /// </summary>
        public bool CanDestructiveBlow
        {
            get { return _CanDestructiveBlow; }
            set
            {
                if (_CanDestructiveBlow == value) return;
                _CanDestructiveBlow = value;

                GameScene.Game.ReceiveChat(CanDestructiveBlow ? "十方斩已开启".Lang() : "十方斩已关闭".Lang(), MessageType.Hint);
            }
        }
        private bool _CanDestructiveBlow;

        public bool CanFlamingSword, CanDragonRise, CanBladeStorm, CanMaelstromBlade, ComboActive;
        public bool canCombo1 => BigPatchConfig.Combo1 == MagicType.FlamingSword ?
                               CanFlamingSword : BigPatchConfig.Combo1 == MagicType.DragonRise ?
                               CanDragonRise : BigPatchConfig.Combo1 == MagicType.BladeStorm ?
                               CanBladeStorm : BigPatchConfig.Combo1 == MagicType.MaelstromBlade ?
                               CanMaelstromBlade : false;
        public bool canCombo2 => BigPatchConfig.Combo2 == MagicType.FlamingSword ?
                               CanFlamingSword : BigPatchConfig.Combo2 == MagicType.DragonRise ?
                               CanDragonRise : BigPatchConfig.Combo2 == MagicType.BladeStorm ?
                               CanBladeStorm : BigPatchConfig.Combo2 == MagicType.MaelstromBlade ?
                               CanMaelstromBlade : false;
        public bool canCombo3 => BigPatchConfig.Combo3 == MagicType.FlamingSword ?
                               CanFlamingSword : BigPatchConfig.Combo3 == MagicType.DragonRise ?
                               CanDragonRise : BigPatchConfig.Combo3 == MagicType.BladeStorm ?
                               CanBladeStorm : BigPatchConfig.Combo3 == MagicType.MaelstromBlade ?
                               CanMaelstromBlade : false;
        public bool canCombo4 => BigPatchConfig.Combo4 == MagicType.FlamingSword ?
                               CanFlamingSword : BigPatchConfig.Combo4 == MagicType.DragonRise ?
                               CanDragonRise : BigPatchConfig.Combo4 == MagicType.BladeStorm ?
                               CanBladeStorm : BigPatchConfig.Combo4 == MagicType.MaelstromBlade ?
                               CanMaelstromBlade : false;
        public bool canCombo5 => BigPatchConfig.Combo5 == MagicType.FlamingSword ?
                               CanFlamingSword : BigPatchConfig.Combo5 == MagicType.DragonRise ?
                               CanDragonRise : BigPatchConfig.Combo5 == MagicType.BladeStorm ?
                               CanBladeStorm : BigPatchConfig.Combo5 == MagicType.MaelstromBlade ?
                               CanMaelstromBlade : false;
        /// <summary>
        /// 新月炎龙开关
        /// </summary>
        public bool CanFlameSplash
        {
            get { return _CanFlameSplash; }
            set
            {
                if (_CanFlameSplash == value) return;
                _CanFlameSplash = value;

                GameScene.Game.ReceiveChat(CanFlameSplash ? "新月爆炎龙已开启".Lang() : "新月爆炎龙已关闭".Lang(), MessageType.Hint);
            }
        }
        private bool _CanFlameSplash;

        /// <summary>
        /// 下次可以点击抢红包按钮的时间
        /// </summary>
        public DateTime NextClaimRedpacketTime { get; set; } = DateTime.MinValue;

        //public MapObject LastAttackPlayer 
        //{
        //    get { return _LastAttackPlayer; }
        //    set
        //    {
        //        if (_LastAttackPlayer == value) return;
        //        _LastAttackPlayer = value;
        //    }
        //}
        //private MapObject _LastAttackPlayer;

        /// <summary>
        /// 用户对象 开始信息
        /// </summary>
        /// <param name="info"></param>
        public UserObject(StartInformation info)
        {
            CharacterIndex = info.Index;

            ObjectID = info.ObjectID;

            Name = info.Name;
            NameColour = info.NameColour;
            AchievementTitle = info.AchievementTitle;

            Class = info.Class;
            Gender = info.Gender;

            Title = info.GuildName;
            GuildRank = info.GuildRank;

            CurrentLocation = info.Location;
            //初始化Maplocation为人物location
            GameScene.Game.MapControl.MapLocation = CurrentLocation;

            Direction = info.Direction;

            CurrentHP = info.CurrentHP;
            CurrentMP = info.CurrentMP;

            Level = info.Level;
            Experience = info.Experience;

            HairType = info.HairType;
            HairColour = info.HairColour;

            ArmourShape = info.Armour;
            ArmourImage = info.ArmourImage;
            ArmourIndex = info.ArmourIndex;
            ArmourColour = info.ArmourColour;
            LibraryWeaponShape = info.Weapon;
            WeaponImage = info.WeaponImage;
            WeaponIndex = info.WeaponIndex;

            Poison = info.Poison;

            InSafeZone = info.InSafeZone;

            AttackMode = info.AttackMode;
            PetMode = info.PetMode;

            Horse = info.Horse;
            HorseType = info.HorseType;

            Dead = info.Dead;

            HorseShape = info.HorseShape;
            HelmetShape = info.HelmetShape;
            ShieldShape = info.ShieldShape;
            FashionShape = info.FashionShape;
            FashionImage = info.FashionImage;
            EmblemShape = info.EmblemShape;

            CraftLevel = info.CraftLevel;
            CraftExp = info.CraftExp;
            CraftFinishTime = info.CraftFinishTime;
            BookmarkedCraftItemInfo = Globals.CraftItemInfoList.Binding.FirstOrDefault(x => x.Index == info.BookmarkedCraftItemInfoIndex);
            CraftingItem = Globals.CraftItemInfoList.Binding.FirstOrDefault(x => x.Index == info.CraftingItemIndex);

            RepeatableQuestRemains = info.RepeatableQuestRemains;
            DailyQuestRemains = info.DailyQuestRemains;

            Gold = info.Gold;
            GameScene.Game.DayTime = info.DayTime;
            GameScene.Game.GroupBox.AllowGroup = info.AllowGroup;
            GameScene.Game.CommunicationBox.AllowFriend = info.AllowFriend;

            HermitPoints = info.HermitPoints;

            foreach (ClientUserMagic magic in info.Magics)
                Magics[magic.Info] = magic;

            foreach (ClientBuffInfo buff in info.Buffs)
            {
                Buffs.Add(buff);
                VisibleBuffs.Add(buff.Type);
                if (buff.Type == BuffType.CustomBuff)
                {
                    CustomBuffInfo customBuff = Globals.CustomBuffInfoList.Binding.FirstOrDefault(x => x.Index == buff.FromCustomBuff);
                    if (customBuff != null)
                        VisibleCustomBuffs.Add(customBuff);
                }
            }

            UpdateLibraries();

            SetFrame(new ObjectAction(!Dead ? MirAction.Standing : MirAction.Dead, Direction, CurrentLocation));

            GameScene.Game.FillItems(info.Items);

            foreach (ClientBeltLink link in info.BeltLinks)
            {
                if (link.Slot < 0 || link.Slot >= GameScene.Game.BeltBox.Links.Length) continue;

                GameScene.Game.BeltBox.Links[link.Slot].LinkInfoIndex = link.LinkInfoIndex;
                GameScene.Game.BeltBox.Links[link.Slot].LinkItemIndex = link.LinkItemIndex;
            }
            GameScene.Game.BeltBox.UpdateLinks();

            GameScene.Game?.BigPatchBox?.UpdateLinks(info);

            GameScene.Game.MapControl.AddObject(this);

            GameScene.Game.OnRemoteStorge = CEnvir.ClientControl.OnRemoteStorage;

            Config.SkillExpDrop = info.SkillExpDrop;
        }
        /// <summary>
        /// 位置改变时
        /// </summary>
        public override void LocationChanged()
        {
            base.LocationChanged();

            GameScene.Game.MapControl.UpdateMapLocation();
            GameScene.Game.MapControl.FLayer.TextureValid = false;
        }
        /// <summary>
        /// 设置动作
        /// </summary>
        /// <param name="action"></param>
        public override void SetAction(ObjectAction action)
        {
            if (CEnvir.Now < ServerTime) return; //下一个服务器响应时间

            base.SetAction(action);

            switch (CurrentAction)
            {
                case MirAction.Die:
                case MirAction.Dead:
                    TargetObject = null;
                    break;
                case MirAction.Standing:
                    //if ((GameScene.Game.MapControl.MapButtons & MouseButtons.Right) != MouseButtons.Right)
                    {
                        GameScene.Game.CanRun = false;
                        GameScene.Game.CanPush = false;
                    }
                    break;
            }

            if (Interupt) return;

            NextActionTime = CEnvir.Now;

            foreach (TimeSpan delay in CurrentFrame.Delays)
                NextActionTime += delay;
        }
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="step"></param>
        public void Moving(MirDirection dir, int step)
        {
            if (MoveTime >= CEnvir.Now) return;
            AttemptAction(new ObjectAction(MirAction.Moving, dir, Functions.Move(CurrentLocation, dir, step), step, MagicType.None));
        }
        /// <summary>
        /// 尝试行动
        /// </summary>
        /// <param name="action"></param>
        public void AttemptAction(ObjectAction action)
        {
            if (CEnvir.Now < NextActionTime || ActionQueue.Count > 0) return;
            if (CEnvir.Now < ServerTime) return; //下一个服务器响应时间

            switch (action.Action)
            {
                case MirAction.Moving:
                    if (CEnvir.Now < MoveTime) return;
                    break;
                case MirAction.Attack:
                    action.Extra[2] = Functions.GetElement(Stats);

                    if (GameScene.Game.Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.DarkStone)
                    {
                        foreach (KeyValuePair<Stat, int> stats in GameScene.Game.Equipment[(int)EquipmentSlot.Amulet].Info.Stats.Values)
                        {
                            switch (stats.Key)
                            {
                                case Stat.FireAffinity:
                                    action.Extra[2] = Element.Fire;
                                    break;
                                case Stat.IceAffinity:
                                    action.Extra[2] = Element.Ice;
                                    break;
                                case Stat.LightningAffinity:
                                    action.Extra[2] = Element.Lightning;
                                    break;
                                case Stat.WindAffinity:
                                    action.Extra[2] = Element.Wind;
                                    break;
                                case Stat.HolyAffinity:
                                    action.Extra[2] = Element.Holy;
                                    break;
                                case Stat.DarkAffinity:
                                    action.Extra[2] = Element.Dark;
                                    break;
                                case Stat.PhantomAffinity:
                                    action.Extra[2] = Element.Phantom;
                                    break;
                            }
                        }
                    }

                    MagicType attackMagic = MagicType.None;

                    #region 大补帖刺客自动四花
                    if (Class == MirClass.Assassin && BigPatchConfig.AutoFourFlowers && (TargetObject != null || GameScene.Game.MapControl.HasTarget(Functions.Move(CurrentLocation, action.Direction))))
                    {
                        //如果BUFF不等 盛开 白莲 红莲其中一个
                        if (!Buffs.Any(x => x.Type == BuffType.FullBloom || x.Type == BuffType.WhiteLotus || x.Type == BuffType.RedLotus))
                        {
                            //释放盛开
                            GameScene.Game.UseMagic(MagicType.FullBloom);
                        }
                        //如果BUFF=盛开
                        if (Buffs.Any(x => x.Type == BuffType.FullBloom))
                        {
                            if (!Buffs.Any(x => x.Type == BuffType.WhiteLotus))
                            {
                                //释放白莲
                                GameScene.Game.UseMagic(MagicType.WhiteLotus);
                            }
                        }
                        //如果BUFF=白莲
                        if (Buffs.Any(x => x.Type == BuffType.WhiteLotus))
                        {
                            if (!Buffs.Any(x => x.Type == BuffType.RedLotus))
                            {
                                //释放红莲
                                GameScene.Game.UseMagic(MagicType.RedLotus);
                            }
                        }
                        //如果BUFF=红莲
                        if (Buffs.Any(x => x.Type == BuffType.RedLotus))
                        {
                            //释放月季
                            GameScene.Game.UseMagic(MagicType.SweetBrier);
                        }
                    }
                    #endregion

                    if (AttackMagic != MagicType.None)
                    {
                        foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in Magics)
                        {
                            if (pair.Key.Magic != AttackMagic) continue;

                            if (CEnvir.Now < pair.Value.NextCast) break;

                            if (AttackMagic == MagicType.Karma)
                            {
                                if (Stats[Stat.Health] * pair.Value.Cost / 100 > CurrentHP || Buffs.All(x => x.Type != BuffType.Cloak))
                                    break;
                            }
                            else
                                if (pair.Value.Cost > CurrentMP) break;


                            attackMagic = AttackMagic;
                            break;
                        }
                    }

                    if (CanPowerAttack && TargetObject != null)
                    {
                        foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in Magics)
                        {
                            if (pair.Key.Magic != MagicType.Slaying) continue;

                            if (pair.Value.Cost > CurrentMP) break;

                            attackMagic = pair.Key.Magic;
                            break;
                        }
                    }

                    if (CanThrusting && GameScene.Game.MapControl.CanEnergyBlast(action.Direction))
                    {
                        foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in Magics)
                        {
                            if (pair.Key.Magic != MagicType.Thrusting) continue;

                            if (pair.Value.Cost > CurrentMP) break;

                            attackMagic = pair.Key.Magic;
                            break;
                        }
                    }

                    if (CanHalfMoon && !CanDestructiveBlow && (TargetObject != null || (GameScene.Game.MapControl.CanHalfMoon(action.Direction) &&
                                                                 (GameScene.Game.MapControl.HasTarget(Functions.Move(CurrentLocation, action.Direction)) || attackMagic != MagicType.Thrusting))))
                    {
                        foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in Magics)
                        {
                            if (pair.Key.Magic != MagicType.HalfMoon) continue;

                            if (CanPowerAttack && TargetObject != null && CEnvir.Random.Next(5) == 0)
                            {
                                if (pair.Key.Magic != MagicType.Slaying) continue;
                            }

                            if (pair.Value.Cost > CurrentMP) break;

                            attackMagic = pair.Key.Magic;
                            break;
                        }
                    }

                    if (CanDestructiveBlow && (TargetObject != null || (GameScene.Game.MapControl.CanDestructiveBlow(action.Direction) &&
                                                                        (GameScene.Game.MapControl.HasTarget(Functions.Move(CurrentLocation, action.Direction)) || attackMagic != MagicType.Thrusting))))
                    {
                        foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in Magics)
                        {
                            if (pair.Key.Magic != MagicType.DestructiveSurge) continue;

                            if (CanPowerAttack && TargetObject != null && CEnvir.Random.Next(5) == 0)
                            {
                                if (pair.Key.Magic != MagicType.Slaying) continue;
                            }

                            if (pair.Value.Cost > CurrentMP) break;

                            attackMagic = pair.Key.Magic;
                            break;
                        }
                    }

                    if (attackMagic == MagicType.None && CanFlameSplash && (TargetObject != null || GameScene.Game.MapControl.CanDestructiveBlow(action.Direction)))
                    {
                        foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in Magics)
                        {
                            if (pair.Key.Magic != MagicType.FlameSplash) continue;

                            if (pair.Value.Cost > CurrentMP) break;

                            attackMagic = pair.Key.Magic;
                            break;
                        }
                    }

                    //如果打开了连招 和 连招就绪
                    if (BigPatchConfig.AutoCombo && ComboActive)
                    {
                        if (canCombo1)
                        {
                            attackMagic = BigPatchConfig.Combo1;

                            //如果是二连，第一招释放完后跳至下一招
                            if (BigPatchConfig.ComboType == 2)
                            {
                                if (GameScene.Game.BigPatchBox.AutoActsStep == 11)
                                    GameScene.Game.BigPatchBox.AutoActsStep = 2;
                            }
                        }
                        else if (canCombo2)
                        {
                            attackMagic = BigPatchConfig.Combo2;

                            //如果是三连，第二招释放完后跳至下一招
                            if (BigPatchConfig.ComboType == 3)
                            {
                                if (GameScene.Game.BigPatchBox.AutoActsStep == 11)
                                    GameScene.Game.BigPatchBox.AutoActsStep = 2;
                            }
                        }
                        else if (canCombo3)
                        {
                            attackMagic = BigPatchConfig.Combo3;

                            //如果是四连，第三招释放完后跳至下一招
                            if (BigPatchConfig.ComboType == 4)
                            {
                                if (GameScene.Game.BigPatchBox.AutoActsStep == 11)
                                    GameScene.Game.BigPatchBox.AutoActsStep = 2;
                            }
                        }
                        else if (canCombo4)
                        {
                            attackMagic = BigPatchConfig.Combo4;

                            //如果是五连，第四招释放完后跳至下一招
                            if (BigPatchConfig.ComboType == 5)
                            {
                                if (GameScene.Game.BigPatchBox.AutoActsStep == 11)
                                    GameScene.Game.BigPatchBox.AutoActsStep = 2;
                            }
                        }
                        else if (canCombo5)
                        {
                            attackMagic = BigPatchConfig.Combo5;
                        }
                    }
                    else if (!ComboActive)
                    {
                        if (CanMaelstromBlade)
                            attackMagic = MagicType.MaelstromBlade;
                        else if (CanBladeStorm)
                            attackMagic = MagicType.BladeStorm;
                        else if (CanDragonRise)
                            attackMagic = MagicType.DragonRise;
                        else if (CanFlamingSword)
                            attackMagic = MagicType.FlamingSword;
                    }

                    action.Extra[1] = attackMagic;
                    break;
                case MirAction.Mount:
                    return;
            }

            SetAction(action);

            int attackDelay;
            switch (action.Action)
            {
                case MirAction.Standing:
                    NextActionTime = CEnvir.Now + TimeSpan.FromMilliseconds(CEnvir.ClientControl.GlobalsTurnTime);
                    CEnvir.Enqueue(new C.Turn { Direction = action.Direction });
                    //if ((GameScene.Game.MapControl.MapButtons & MouseButtons.Right) != MouseButtons.Right)
                    {
                        GameScene.Game.CanRun = false;
                        GameScene.Game.CanPush = false;
                    }
                    break;
                case MirAction.Harvest:
                    NextActionTime = CEnvir.Now + TimeSpan.FromMilliseconds(CEnvir.ClientControl.GlobalsHarvestTime);
                    CEnvir.Enqueue(new C.Harvest { Direction = action.Direction });
                    {
                        GameScene.Game.CanRun = false;
                        GameScene.Game.CanPush = false;
                    }
                    break;
                case MirAction.Moving:
                    MoveTime = CEnvir.Now + TimeSpan.FromMilliseconds(CEnvir.ClientControl.GlobalsMoveTime);

                    if (BagWeight > Stats[Stat.BagWeight] || HandWeight > Stats[Stat.HandWeight] || WearWeight > Stats[Stat.WearWeight])
                        MoveTime += TimeSpan.FromMilliseconds(CEnvir.ClientControl.GlobalsMoveTime * 2);

                    CEnvir.Enqueue(new C.Move { Direction = action.Direction, Distance = MoveDistance });
                    {
                        GameScene.Game.CanRun = true;
                        GameScene.Game.CanPush = false;
                    }
                    break;
                case MirAction.Attack:
                    //攻击延迟
                    int aspeed = Stats[Stat.AttackSpeed];
                    if (CEnvir.Now > SpeedDelay)//判断是否是触发时间
                    {
                        //非触发时间
                        if (CEnvir.Random.Next(100) < Stats[Stat.AttackSpeedAdd])
                        {
                            SpeedDelay = CEnvir.Now.AddSeconds(10);
                            aspeed = aspeed * 200 / 100;
                        }
                    }
                    else//触发时间
                    {
                        aspeed = aspeed * 200 / 100;
                    }
                    attackDelay = (int)(CEnvir.ClientControl.GlobalsAttackDelay - aspeed / 10.0 * CEnvir.ClientControl.GlobalsASpeedRate);
                    attackDelay = Math.Max(100, attackDelay);
                    AttackTime = CEnvir.Now + TimeSpan.FromMilliseconds(attackDelay);

                    if (BagWeight > Stats[Stat.BagWeight] || HandWeight > Stats[Stat.HandWeight] || WearWeight > Stats[Stat.WearWeight] || (Poison & PoisonType.Neutralize) == PoisonType.Neutralize)
                        AttackTime += TimeSpan.FromMilliseconds(attackDelay);

                    CEnvir.Enqueue(new C.Attack { Direction = action.Direction, Action = action.Action, AttackMagic = MagicType });
                    {
                        GameScene.Game.CanRun = false;
                        GameScene.Game.CanPush = false;
                    }
                    break;
                case MirAction.Spell:
                    NextMagicTime = CEnvir.Now + TimeSpan.FromMilliseconds(CEnvir.ClientControl.GlobalsMagicDelay);
                    if (BagWeight > Stats[Stat.BagWeight] || HandWeight > Stats[Stat.HandWeight] || WearWeight > Stats[Stat.WearWeight] || (Poison & PoisonType.Neutralize) == PoisonType.Neutralize)
                        NextMagicTime += TimeSpan.FromMilliseconds(CEnvir.ClientControl.GlobalsMagicDelay);

                    CEnvir.Enqueue(new C.Magic { Direction = action.Direction, Action = action.Action, Type = MagicType, Target = AttackTargets?.Count > 0 ? AttackTargets[0].ObjectID : 0, Location = MagicLocations?.Count > 0 ? MagicLocations[0] : Point.Empty });
                    {
                        GameScene.Game.CanRun = false;
                        GameScene.Game.CanPush = false;
                    }
                    break;
                case MirAction.Mining:
                    attackDelay = (int)(CEnvir.ClientControl.GlobalsAttackDelay - Stats[Stat.AttackSpeed] / 10.0 * CEnvir.ClientControl.GlobalsASpeedRate);
                    attackDelay = Math.Max(100, attackDelay);
                    AttackTime = CEnvir.Now + TimeSpan.FromMilliseconds(attackDelay);

                    if (BagWeight > Stats[Stat.BagWeight] || HandWeight > Stats[Stat.HandWeight] || WearWeight > Stats[Stat.WearWeight] || (Poison & PoisonType.Neutralize) == PoisonType.Neutralize)
                        AttackTime += TimeSpan.FromMilliseconds(attackDelay);

                    CEnvir.Enqueue(new C.Mining { Direction = action.Direction });
                    {
                        GameScene.Game.CanRun = false;
                        GameScene.Game.CanPush = false;
                    }
                    break;
                //case MirAction.FishingCast:
                //    //todo fishing
                //告知服务端 开始钓鱼
                //    CEnvir.Enqueue(new C.FishingCast());

                //    GameScene.Game.CanRun = false;
                //    break;
                //case MirAction.FishingWait:
                //    //todo fishing
                //    break;
                //case MirAction.FishingReel:
                //    //todo fishing
                //    break;

                default:
                    GameScene.Game.CanRun = false;
                    GameScene.Game.CanPush = false;
                    break;
            }
            ServerTime = CEnvir.Now.AddSeconds(5);
        }
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            if (DrawColour == DefaultColour)
            {
                if (BagWeight > Stats[Stat.BagWeight] || WearWeight > Stats[Stat.WearWeight] || HandWeight > Stats[Stat.HandWeight])
                    DrawColour = Color.CornflowerBlue;
            }

            //震屏效果
            TimeSpan shakeTicks = CEnvir.Now - ShakeScreenTime;
            ShakeScreenTime = CEnvir.Now;
            ShakeScreenOffset = new Point(new Random().Next(-(int)Math.Abs(ShakeScreenCount * 1f), (int)Math.Abs(ShakeScreenCount * 1f)),
                                          new Random().Next(-(int)Math.Abs(ShakeScreenCount * 2f), (int)Math.Abs(ShakeScreenCount * 2f)));
            //ShakeScreenOffset = new Point(0, (int)(Math.Sin(ShakeScreenCount) * 10));
            if (ShakeScreenCount > 0)
            {
                ShakeScreenCount -= (float)shakeTicks.TotalMilliseconds / 50f;
                GameScene.Game.MapControl.FLayer.TextureValid = false;
            }
            else if (ShakeScreenCount < 0)
            {
                ShakeScreenCount = 0;
                ShakeScreenOffset = new Point(0, 0);
                GameScene.Game.MapControl.FLayer.TextureValid = false;
            }

            TimeSpan ticks = CEnvir.Now - BuffTime;
            BuffTime = CEnvir.Now;

            foreach (ClientBuffInfo buff in Buffs)
            {
                if (buff.Pause || buff.RemainingTime == TimeSpan.MaxValue) continue;
                buff.RemainingTime = Functions.Max(TimeSpan.Zero, buff.RemainingTime - ticks);
            }

            //检查钓鱼
            CheckFishing();

            // 抢红包
            if (NextClaimRedpacketTime < CEnvir.Now)
            {
                if (GameScene.Game.BonusPoolVersionBox?.RedpacketFaceUI?.ClaimButton != null)
                    GameScene.Game.BonusPoolVersionBox.RedpacketFaceUI.ClaimButton.Enabled = true;
            }
        }
        /// <summary>
        /// 帧索引改变时
        /// </summary>
        public override void FrameIndexChanged()
        {
            base.FrameIndexChanged();

            switch (CurrentAction)
            {
                case MirAction.Moving:
                    switch (CurrentAnimation)
                    {
                        case MirAnimation.HorseWalking:
                            if (FrameIndex == 1)
                                DXSoundManager.Play(SoundIndex.HorseWalk1);
                            if (FrameIndex == 4)
                                DXSoundManager.Play(SoundIndex.HorseWalk2);
                            break;
                        case MirAnimation.HorseRunning:
                            if (FrameIndex != 1) return;
                            DXSoundManager.Play(SoundIndex.HorseRun);
                            break;
                        default:
                            if (FrameIndex != 1 && FrameIndex != 4) return;
                            DXSoundManager.Play((SoundIndex)((int)SoundIndex.Foot1 + CEnvir.Random.Next((int)SoundIndex.Foot4 - (int)SoundIndex.Foot1) + 1));
                            break;
                    }
                    break;
                case MirAction.Spell:
                    switch (MagicType)
                    {
                        case MagicType.SeismicSlam:      //天雷锤
                            if (FrameIndex == 4)
                            {
                                ShakeScreenCount = 15F;    //震动屏幕计数
                            }
                            break;
                    }
                    break;
            }
        }
        /// <summary>
        /// 安全提示
        /// </summary>
        bool _isInSafeZone = true;
        /// <summary>
        /// 反向移动偏移
        /// </summary>
        //public Point ReverseMovingOffSet = Point.Empty;
        /// <summary>
        /// 移动偏移改变
        /// </summary>
        public override void MovingOffSetChanged()
        {
            base.MovingOffSetChanged();
            GameScene.Game.MapControl.FLayer.TextureValid = false;

            if (_isInSafeZone != InSafeZone)
            {
                _isInSafeZone = InSafeZone;
                //var str = InSafeZone ? "安全区域".Lang() : "冒险区域".Lang();
                //GameScene.Game.MsgTipTextBox.ShowTip(str, "范围".Lang(), "开始进入".Lang(), InSafeZone);
            }
        }
        /// <summary>
        /// 名字改名
        /// </summary>
        public override void NameChanged()
        {
            base.NameChanged();

            GameScene.Game.CharacterBox.GuildNameLabel.Text = Globals.StarterGuildName == Title ? Title.Lang() : Title;
            GameScene.Game.CharacterBox.GuildRankLabel.Text = Globals.StarterGuildMember == GuildRank ? GuildRank.Lang() : GuildRank;

            GameScene.Game.CharacterBox.CharacterNameLabel.Text = Name;

            GameScene.Game.TradeBox.UserLabel.Text = Name;
        }
        /// <summary>
        /// 更新帧
        /// </summary>
        public override void UpdateFrame()
        {
            //需要同时计算传奇2帧和传奇3帧
            //才能应对传2传3装备混穿的情况
            //传2帧
            if (Mir2Frames == null || Mir2CurrentFrame == null) return;
            //传3帧
            if (Frames == null || CurrentFrame == null) return;

            switch (CurrentAction)
            {
                case MirAction.Moving:
                case MirAction.Pushed:
                    if (!GameScene.Game.MoveFrame) return;
                    break;
            }

            //拿到当前时间对应的那1帧
            int frame = CurrentFrame.GetFrame(FrameStart, CEnvir.Now, (this != User || GameScene.Game.Observer) && ActionQueue.Count > 1);
            //传奇2当前帧
            int mir2Frame = Mir2CurrentFrame.GetFrame(FrameStart, CEnvir.Now, (this != User || GameScene.Game.Observer) && ActionQueue.Count > 1);

            if (frame == CurrentFrame.FrameCount || mir2Frame == Mir2CurrentFrame.FrameCount || (Interupt && ActionQueue.Count > 0))
            {
                DoNextAction();
                frame = CurrentFrame.GetFrame(FrameStart, CEnvir.Now, (this != User || GameScene.Game.Observer) && ActionQueue.Count > 1);
                //传奇2当前帧
                mir2Frame = Mir2CurrentFrame.GetFrame(FrameStart, CEnvir.Now, (this != User || GameScene.Game.Observer) && ActionQueue.Count > 1);

                if (frame == CurrentFrame.FrameCount)
                    frame -= 1;
                if (mir2Frame == Mir2CurrentFrame.FrameCount)
                    mir2Frame -= 1;
            }

            int x = 0, y = 0;
            int mir2x = 0, mir2y = 0;
            #region 传奇2

            switch (CurrentAction)
            {
                case MirAction.Moving:
                case MirAction.Pushed:
                    switch (Direction)
                    {
                        case MirDirection.Up:
                            mir2x = 0;
                            mir2y = (int)(CellHeight * MoveDistance / (float)Mir2CurrentFrame.FrameCount * (Mir2CurrentFrame.FrameCount - (mir2Frame + 1)));
                            break;
                        case MirDirection.UpRight:
                            mir2x = -(int)(CellWidth * MoveDistance / (float)Mir2CurrentFrame.FrameCount * (Mir2CurrentFrame.FrameCount - (mir2Frame + 1)));
                            mir2y = (int)(CellHeight * MoveDistance / (float)Mir2CurrentFrame.FrameCount * (Mir2CurrentFrame.FrameCount - (mir2Frame + 1)));
                            break;
                        case MirDirection.Right:
                            mir2x = -(int)(CellWidth * MoveDistance / (float)Mir2CurrentFrame.FrameCount * (Mir2CurrentFrame.FrameCount - (mir2Frame + 1)));
                            mir2y = 0;
                            break;
                        case MirDirection.DownRight:
                            mir2x = -(int)(CellWidth * MoveDistance / (float)Mir2CurrentFrame.FrameCount * (Mir2CurrentFrame.FrameCount - (mir2Frame + 1)));
                            mir2y = -(int)(CellHeight * MoveDistance / (float)Mir2CurrentFrame.FrameCount * (Mir2CurrentFrame.FrameCount - (mir2Frame + 1)));
                            break;
                        case MirDirection.Down:
                            mir2y = -(int)(CellHeight * MoveDistance / (float)Mir2CurrentFrame.FrameCount * (Mir2CurrentFrame.FrameCount - (mir2Frame + 1)));
                            mir2x = 0;
                            break;
                        case MirDirection.DownLeft:
                            mir2x = (int)(CellWidth * MoveDistance / (float)Mir2CurrentFrame.FrameCount * (Mir2CurrentFrame.FrameCount - (mir2Frame + 1)));
                            mir2y = -(int)(CellHeight * MoveDistance / (float)Mir2CurrentFrame.FrameCount * (Mir2CurrentFrame.FrameCount - (mir2Frame + 1)));
                            break;
                        case MirDirection.Left:
                            mir2x = (int)(CellWidth * MoveDistance / (float)Mir2CurrentFrame.FrameCount * (Mir2CurrentFrame.FrameCount - (mir2Frame + 1)));
                            mir2y = 0;
                            break;
                        case MirDirection.UpLeft:
                            mir2x = (int)(CellWidth * MoveDistance / (float)Mir2CurrentFrame.FrameCount * (Mir2CurrentFrame.FrameCount - (mir2Frame + 1)));
                            mir2y = (int)(CellHeight * MoveDistance / (float)Mir2CurrentFrame.FrameCount * (Mir2CurrentFrame.FrameCount - (mir2Frame + 1)));
                            break;
                    }
                    break;
            }

            #endregion

            #region 传奇3

            switch (CurrentAction)
            {
                case MirAction.Moving:
                case MirAction.Pushed:
                    switch (Direction)
                    {
                        case MirDirection.Up:
                            x = 0;
                            y = (int)(CellHeight * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            break;
                        case MirDirection.UpRight:
                            x = -(int)(CellWidth * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            y = (int)(CellHeight * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            break;
                        case MirDirection.Right:
                            x = -(int)(CellWidth * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            y = 0;
                            break;
                        case MirDirection.DownRight:
                            x = -(int)(CellWidth * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            y = -(int)(CellHeight * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            break;
                        case MirDirection.Down:
                            y = -(int)(CellHeight * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            x = 0;
                            break;
                        case MirDirection.DownLeft:
                            x = (int)(CellWidth * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            y = -(int)(CellHeight * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            break;
                        case MirDirection.Left:
                            x = (int)(CellWidth * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            y = 0;
                            break;
                        case MirDirection.UpLeft:
                            x = (int)(CellWidth * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            y = (int)(CellHeight * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            break;
                    }
                    break;
            }

            #endregion

            x -= x % 2;
            y -= y % 2;

            mir2x -= mir2x % 2;
            mir2y -= mir2y % 2;

            if (CurrentFrame.Reversed)
            {
                frame = CurrentFrame.FrameCount - frame - 1;
                x *= -1;
                y *= -1;
            }

            if (Mir2CurrentFrame.Reversed)
            {
                mir2Frame = Mir2CurrentFrame.FrameCount - mir2Frame - 1;
                mir2x *= -1;
                mir2y *= -1;
            }

            //todo 这里要不要考虑mir2？
            if (GameScene.Game.MapControl.BackgroundImage != null)
                GameScene.Game.MapControl.BackgroundMovingOffset = new Point((int)(x / GameScene.Game.MapControl.BackgroundScaleX), (int)(y / GameScene.Game.MapControl.BackgroundScaleY));

            //todo 这里要不要考虑mir2？
            MovingOffSet = new Point(x, y);

            if (CurrentAction == MirAction.Pushed)
            {
                frame = 0;
                mir2Frame = 0;
            }

            FrameIndex = frame;

            DrawFrame = FrameIndex + CurrentFrame.StartIndex + CurrentFrame.OffSet * (int)Direction;
            Mir2DrawFrame = FrameIndex + Mir2CurrentFrame.StartIndex + Mir2CurrentFrame.OffSet * (int)Direction;
        }

        public void AddBuff(ClientBuffInfo buff)
        {
            Buffs.Add(buff);
            VisibleBuffs.Add(buff.Type);
            if (buff.Type == BuffType.CustomBuff)
            {
                CustomBuffInfo customBuff = Globals.CustomBuffInfoList.Binding.FirstOrDefault(x => x.Index == buff.FromCustomBuff);
                if (customBuff != null)
                    VisibleCustomBuffs.Add(customBuff);
            }
            if (buff.Type == BuffType.SuperiorMagicShield)
            {
                MaximumSuperiorMagicShield = buff.Stats[Stat.SuperiorMagicShield];
                SuperiorMagicShieldEnd();
            }
        }
    }
}
