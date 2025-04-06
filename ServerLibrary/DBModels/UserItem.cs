using Library;
using Library.SystemModels;
using MirDB;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Server.DBModels
{
    /// <summary>
    /// 角色道具
    /// </summary>
    [UserObject]
    public sealed class UserItem : DBObject
    {
        /// <summary>
        /// 道具信息
        /// </summary>
        public ItemInfo Info
        {
            get { return _Info; }
            set
            {
                if (_Info == value) return;

                var oldValue = _Info;
                _Info = value;

                OnChanged(oldValue, value, "Info");
            }
        }
        private ItemInfo _Info;
        /// <summary>
        /// 道具当前持久
        /// </summary>
        public int CurrentDurability
        {
            get { return _CurrentDurability; }
            set
            {
                if (_CurrentDurability == value) return;

                var oldValue = _CurrentDurability;
                _CurrentDurability = value;

                OnChanged(oldValue, value, "CurrentDurability");
            }
        }
        private int _CurrentDurability;
        /// <summary>
        /// 道具最高持久
        /// </summary>
        public int MaxDurability
        {
            get { return _MaxDurability; }
            set
            {
                if (_MaxDurability == value) return;

                var oldValue = _MaxDurability;
                _MaxDurability = value;

                OnChanged(oldValue, value, "MaxDurability");
            }
        }
        private int _MaxDurability;
        /// <summary>
        /// 道具数量
        /// </summary>
        public long Count
        {
            get { return _Count; }
            set
            {
                if (_Count == value) return;

                var oldValue = _Count;
                _Count = value;

                OnChanged(oldValue, value, "Count");
            }
        }
        private long _Count;
        /// <summary>
        /// 道具格子位置信息
        /// </summary>
        public int Slot
        {
            get { return _Slot; }
            set
            {
                if (_Slot == value) return;

                var oldValue = _Slot;
                _Slot = value;

                OnChanged(oldValue, value, "Slot");
            }
        }
        private int _Slot;
        /// <summary>
        /// 道具等级
        /// </summary>
        public int Level
        {
            get { return _Level; }
            set
            {
                if (_Level == value) return;

                var oldValue = _Level;
                _Level = value;

                OnChanged(oldValue, value, "Level");
            }
        }
        private int _Level;
        /// <summary>
        /// 道具经验值
        /// </summary>
        public decimal Experience
        {
            get { return _Experience; }
            set
            {
                if (_Experience == value) return;

                var oldValue = _Experience;
                _Experience = value;

                OnChanged(oldValue, value, "Experience");
            }
        }
        private decimal _Experience;
        /// <summary>
        /// 道具颜色
        /// </summary>
        public Color Colour
        {
            get { return _Colour; }
            set
            {
                if (_Colour == value) return;

                var oldValue = _Colour;
                _Colour = value;

                OnChanged(oldValue, value, "Colour");
            }
        }
        private Color _Colour;
        /// <summary>
        /// 道具特修冷却时间
        /// </summary>
        public DateTime SpecialRepairCoolDown
        {
            get { return _specialRepairCoolDown; }
            set
            {
                if (_specialRepairCoolDown == value) return;

                var oldValue = _specialRepairCoolDown;
                _specialRepairCoolDown = value;

                OnChanged(oldValue, value, "SpecialRepairCoolDown");
            }
        }
        private DateTime _specialRepairCoolDown;
        /// <summary>
        /// 道具重置冷却时间
        /// </summary>
        public DateTime ResetCoolDown
        {
            get { return _ResetCoolDown; }
            set
            {
                if (_ResetCoolDown == value) return;

                var oldValue = _ResetCoolDown;
                _ResetCoolDown = value;

                OnChanged(oldValue, value, "ResetCoolDown");
            }
        }
        private DateTime _ResetCoolDown;

        /// <summary>
        /// 用户任务要求
        /// </summary>
        public UserQuestTask UserTask;

        /// <summary>
        /// 道具持有角色信息
        /// </summary>
        [Association("Items")]
        public CharacterInfo Character
        {
            get { return _Character; }
            set
            {
                if (_Character == value) return;

                var oldValue = _Character;
                _Character = value;

                OnChanged(oldValue, value, "Character");
            }
        }
        private CharacterInfo _Character;
        /// <summary>
        /// 道具持有账号信息
        /// </summary>
        [Association("Items")]
        public AccountInfo Account
        {
            get { return _Account; }
            set
            {
                if (_Account == value) return;

                var oldValue = _Account;
                _Account = value;

                OnChanged(oldValue, value, "Account");
            }
        }
        private AccountInfo _Account;
        /// <summary>
        /// 道具持有行会信息
        /// </summary>
        [Association("Items")]
        public GuildInfo Guild
        {
            get { return _Guild; }
            set
            {
                if (_Guild == value) return;

                var oldValue = _Guild;
                _Guild = value;

                OnChanged(oldValue, value, "Guild");
            }
        }
        private GuildInfo _Guild;
        /// <summary>
        /// 道具持有角色宠物信息
        /// </summary>
        [Association("Items")]
        public UserCompanion Companion
        {
            get { return _Companion; }
            set
            {
                if (_Companion == value) return;

                var oldValue = _Companion;
                _Companion = value;

                OnChanged(oldValue, value, "Companion");
            }
        }
        private UserCompanion _Companion;

        /// <summary>
        /// 道具精炼信息
        /// </summary>
        [Association("Refine")]
        public RefineInfo Refine
        {
            get { return _Refine; }
            set
            {
                if (_Refine == value) return;

                var oldValue = _Refine;
                _Refine = value;

                OnChanged(oldValue, value, "Refine");
            }
        }
        private RefineInfo _Refine;
        /// <summary>
        /// 道具寄售信息
        /// </summary>
        [Association("Auction")]
        public AuctionInfo Auction
        {
            get { return _Auction; }
            set
            {
                if (_Auction == value) return;

                var oldValue = _Auction;
                _Auction = value;

                OnChanged(oldValue, value, "Auction");
            }
        }
        private AuctionInfo _Auction;
        /// <summary>
        /// 道具邮件信息
        /// </summary>
        [Association("Mail")]
        public MailInfo Mail
        {
            get { return _Mail; }
            set
            {
                if (_Mail == value) return;

                var oldValue = _Mail;
                _Mail = value;

                OnChanged(oldValue, value, "Mail");
            }
        }
        private MailInfo _Mail;
        /// <summary>
        /// 角色道具标记
        /// </summary>
        public UserItemFlags Flags
        {
            get { return _Flags; }
            set
            {
                if (_Flags == value) return;

                var oldValue = _Flags;
                _Flags = value;

                OnChanged(oldValue, value, "Flags");
            }
        }
        private UserItemFlags _Flags;
        /// <summary>
        ///幻化过期时间Tick
        /// </summary>
        public TimeSpan IllusionExpireTime
        {
            get { return _IllusionExpireTime; }
            set
            {
                if (_IllusionExpireTime == value) return;

                var oldValue = _IllusionExpireTime;
                _IllusionExpireTime = value;
                IllusionExpireDateTime = SEnvir.Now.AddMilliseconds(value.TotalMilliseconds);

                OnChanged(oldValue, value, "IllusionExpireTime");
            }
        }
        private TimeSpan _IllusionExpireTime;
        /// <summary>
        /// 幻化道具过期日期
        /// </summary>
        public DateTime IllusionExpireDateTime { get; set; }


        /// <summary>
        /// 道具过期时间Tick
        /// </summary>
        public TimeSpan ExpireTime
        {
            get { return _ExpireTime; }
            set
            {
                if (_ExpireTime == value) return;

                var oldValue = _ExpireTime;
                _ExpireTime = value;
                ExpireDateTime = SEnvir.Now.AddMilliseconds(value.TotalMilliseconds);

                OnChanged(oldValue, value, "ExpireTime");
            }
        }
        private TimeSpan _ExpireTime;
        /// <summary>
        /// 道具过期日期
        /// </summary>
        public DateTime ExpireDateTime { get; set; }
        /// <summary>
        /// 道具增加属性状态
        /// </summary>
        [Association("AddedStats", true)]
        public DBBindingList<UserItemStat> AddedStats { get; set; }
        /// <summary>
        /// 获得道具的时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 获得道具的来源地图
        /// </summary>
        public string SourceMap { get; set; }
        /// <summary>
        /// 获得道具的来源类型
        /// NPC,怪物,人物
        /// </summary>
        public ObjectType SourceRace { get; set; }
        /// <summary>
        /// 道具来源的指定名称
        /// </summary>
        public string SourceName { get; set; }
        /// <summary>
        /// 道具获得者
        /// </summary>
        public string OriginalOwner { get; set; }
        /// <summary>
        /// 道具自定义前缀
        /// </summary>
        public string CustomPrefixText { get; set; }
        /// <summary>
        /// 道具自定义前缀颜色
        /// </summary>
        public Color CustomPrefixColor { get; set; }
        /// <summary>
        /// 是否精炼
        /// </summary>
        public bool IsRefine { get; set; }
        /// <summary>
        /// 重置精炼次数
        /// </summary>
        public int RefineResetCount { get; set; }
        /// <summary>
        /// 精炼几率
        /// </summary>
        public int Chance { get; set; }
        /// <summary>
        /// 精炼类型
        /// </summary>
        public RefineType RefineType { get; set; }

        /// <summary>
        /// 道具重量
        /// </summary>
        [IgnoreProperty]
        public int Weight
        {
            get
            {
                if (Info == null)
                    return 0;
                switch (Info.ItemType)
                {
                    case ItemType.Poison:
                    case ItemType.Amulet:
                        return Info.Weight;
                    default:
                        return (int)Math.Min(int.MaxValue, Info.Weight * Count);
                }
            }
        }
        /// <summary>
        /// 道具属性状态
        /// </summary>
        public Stats Stats = new Stats();

        /// <summary>
        /// 道具更改时
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        protected override void OnChanged(object oldValue, object newValue, string propertyName)
        {
            base.OnChanged(oldValue, newValue, propertyName);

            switch (propertyName)
            {
                case "Account":       //账号
                    if (Account != null)
                    {
                        Character = null;
                        Refine = null;
                        Auction = null;
                        Mail = null;
                        Guild = null;
                        Companion = null;
                    }
                    break;
                case "Character":          //角色
                    if (Character != null)
                    {
                        Account = null;
                        Refine = null;
                        Auction = null;
                        Mail = null;
                        Guild = null;
                        Companion = null;
                    }
                    break;
                case "Refine":            //精炼
                    if (Refine != null)
                    {
                        Account = null;
                        Character = null;
                        Auction = null;
                        Mail = null;
                        Guild = null;
                        Companion = null;
                    }
                    break;
                case "Auction":           //寄售
                    if (Auction != null)
                    {
                        Account = null;
                        Character = null;
                        Refine = null;
                        Mail = null;
                        Guild = null;
                        Companion = null;
                    }
                    break;
                case "Mail":              //邮件
                    if (Mail != null)
                    {
                        Account = null;
                        Character = null;
                        Refine = null;
                        Auction = null;
                        Guild = null;
                        Companion = null;
                    }
                    break;
                case "Guild":            //行会
                    if (Guild != null)
                    {
                        Character = null;
                        Account = null;
                        Refine = null;
                        Auction = null;
                        Mail = null;
                        Companion = null;
                    }
                    break;
                case "Companion":          //宠物
                    if (Companion != null)
                    {
                        Character = null;
                        Account = null;
                        Refine = null;
                        Auction = null;
                        Mail = null;
                        Guild = null;
                    }
                    break;

            }
        }
        /// <summary>
        /// 道具删除时
        /// </summary>
        protected override internal void OnDeleted()
        {
            Info = null;

            Character = null;
            Account = null;
            Guild = null;
            Companion = null;
            Refine = null;
            Auction = null;
            Mail = null;
            UserTask = null;

            for (int i = AddedStats.Count - 1; i >= 0; i--)
                AddedStats[i].Delete();

            UserTask = null;

            base.OnDeleted();
        }

        /// <summary>
        /// 道具创建时
        /// </summary>
        protected override internal void OnCreated()
        {
            base.OnCreated();

            Count = 1;
            Slot = -1;
            Level = 1;
        }
        /// <summary>
        /// 道具读取时
        /// </summary>
        protected override internal void OnLoaded()
        {
            base.OnLoaded();

            StatsChanged();
        }

        /// <summary>
        /// 输出字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Info.ToString();
        }

        /// <summary>
        /// 道具属性状态改变时
        /// </summary>
        public void StatsChanged()
        {
            Stats.Clear();

            foreach (UserItemStat stat in AddedStats)
                Stats[stat.Stat] += stat.Amount;
        }

#if !ServerTool
        /// <summary>
        /// 道具增加属性状态
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="amount"></param>
        /// <param name="source"></param>
        /// <param name="sourceName"></param>
        public void AddStat(Stat stat, int amount, StatSource source, string sourceName = "") //todo 搜索此方法的所有引用,视情况传入sourceName
        {
            foreach (UserItemStat addedStat in AddedStats)
            {
                if (addedStat.Stat != stat || addedStat.StatSource != source) continue;
                addedStat.Amount += amount;
                return;
            }
            if (amount == 0) return;

            UserItemStat newStat = SEnvir.UserItemStatsList.CreateNewObject();
            newStat.StatSource = source;
            newStat.Stat = stat;
            newStat.Amount = amount;
            newStat.Item = this;
            newStat.SourceName = sourceName;
        }
        /// <summary>
        /// 道具删除属性状态
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="source"></param>
        public void RemoveStat(Stat stat, StatSource source)
        {
            for (int i = AddedStats.Count - 1; i >= 0; i--)
            {
                var addedStat = AddedStats[i];
                if (addedStat.Stat != stat || addedStat.StatSource != source) continue;
                AddedStats.Remove(addedStat);
                break;
            }
            StatsChanged();
            return;
        }
        /// <summary>
        /// 把另一个物品的属性状态转移到本物品
        /// </summary>
        /// <param name="theGem"></param>
        /// <returns></returns>
        public List<FullItemStat> AttachGem(UserItem theGem)
        {
            List<FullItemStat> transferedStats = new List<FullItemStat>();

            // 判断第几颗宝石
            StatSource gemIndex = StatSource.Gem1 + Stats[Stat.UsedGemSlot];
            // 宝石数目不在允许范围内 返回
            if (!Globals.GemList.Contains(gemIndex))
                return transferedStats;

            foreach (KeyValuePair<Stat, int> kvp in theGem.Info.Stats.Values)
            {
                if (!Globals.AllowedAttachementStats.Contains(kvp.Key))
                    continue;// 跳过不在可转移属性列表中的属性

                AddStat(kvp.Key, kvp.Value, gemIndex, theGem.Info.ItemName);
                transferedStats.Add(new FullItemStat { Stat = kvp.Key, Amount = kvp.Value, StatSource = gemIndex, SourceName = theGem.Info.ItemName });
            }
            return transferedStats;
        }
        /// <summary>
        /// 获取完成的属性状态信息
        /// </summary>
        public List<FullItemStat> GetFullItemStats()
        {
            List<FullItemStat> result = new List<FullItemStat>();
            foreach (UserItemStat addedStat in AddedStats)
            {
                result.Add(new FullItemStat { Stat = addedStat.Stat, Amount = addedStat.Amount, SourceName = addedStat.SourceName, StatSource = addedStat.StatSource });
            }
            if (Info != null)
            {
                foreach (KeyValuePair<Stat, int> pair in Info.Stats.Values)
                {
                    result.Add(new FullItemStat { Stat = pair.Key, Amount = pair.Value, SourceName = "", StatSource = StatSource.None });
                }
            }
            result = result.OrderBy(o => o.Stat).ToList();
            return result;
        }
        /// <summary>
        /// 更新客户端道具信息
        /// </summary>
        /// <returns></returns>
        public ClientUserItem ToClientInfo()
        {
            return new ClientUserItem
            {
                Index = Index,

                FullItemStats = GetFullItemStats(), //详细属性信息

                InfoIndex = Info?.Index ?? 0,

                CurrentDurability = CurrentDurability,
                MaxDurability = MaxDurability,

                Count = Info.Effect == ItemEffect.GameGold ? Count / 100 : Count,

                Slot = Slot,
                Refine = IsRefine,
                Level = Level,
                Experience = Experience,

                Colour = Colour,

                SpecialRepairCoolDown = SpecialRepairCoolDown > SEnvir.Now ? SpecialRepairCoolDown - SEnvir.Now : TimeSpan.Zero,
                ResetCoolDown = ResetCoolDown > SEnvir.Now ? ResetCoolDown - SEnvir.Now : TimeSpan.Zero,

                AddedStats = new Stats(Stats),

                Flags = Flags,

                ExpireTime = ExpireTime,
                ExpireDateTime = ExpireDateTime,

                IllusionExpireTime = IllusionExpireTime,
                IllusionExpireDateTime = IllusionExpireDateTime,

                Guild1Name = Stats[Stat.Guild1] > 0 ? SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Index == Stats[Stat.Guild1])?.GuildName : null,
                Guild2Name = Stats[Stat.Guild2] > 0 ? SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Index == Stats[Stat.Guild2])?.GuildName : null,

                CreationTime = CreateTime,
                SourceMap = SourceMap,
                SourceName = SourceName,
                OriginalOwner = OriginalOwner,

                CustomPrefixText = CustomPrefixText,
                CustomPrefixColor = CustomPrefixColor,

                OwnerlessType = OwnerlessType,
            };
        }
        /// <summary>
        /// 道具出售价格
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public long Price(long count)
        {
            if (Info == null) return 0;   //空信息的价格0
            if ((Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless) return 0;  //不能出售的价格0

            decimal p = Info.Price;  //p等数据库里的道具的价格

            if (Info.Durability > 0)  //道具信息的持久大于0时
            {
                decimal r = Info.Price / 2M / Info.Durability;

                p = MaxDurability * r;

                r = MaxDurability > 0 ? CurrentDurability / (decimal)MaxDurability : 0;

                p = Math.Floor(p / 2M + p / 2M * r + Info.Price / 2M);
            }

            p = p * (Stats.Count * 0.1M + 1M);

            //if (Info.Stats[Stat.SaleBonus20] > 0 && Info.Stats[Stat.SaleBonus20] <= count)
            //    p *= 1.2M;
            //else if (Info.Stats[Stat.SaleBonus15] > 0 && Info.Stats[Stat.SaleBonus15] <= count)
            //    p *= 1.15M;
            //else if (Info.Stats[Stat.SaleBonus10] > 0 && Info.Stats[Stat.SaleBonus10] <= count)
            //    p *= 1.1M;
            //else if (Info.Stats[Stat.SaleBonus5] > 0 && Info.Stats[Stat.SaleBonus5] <= count)
            //    p *= 1.05M;

            return (long)(p * count * Info.SellRate); // * 0.6M);
        }
        /// <summary>
        /// 道具修理费
        /// </summary>
        /// <param name="special"></param>
        /// <returns></returns>
        public long RepairCost(bool special)
        {
            if (Info.Durability == 0 || CurrentDurability >= MaxDurability) return 0;

            decimal rate = special ? 1M : 0.5M;

            decimal p = Math.Floor((MaxDurability - CurrentDurability) * (Info.Price / 1M / MaxDurability)); //+Info.Price / 2M
            //p = p * (AddedStats.Count * 0.1M + 1M);

            return (long)(p * rate); //*Count - Price(Count)
        }
        /// <summary>
        /// 道具碎片
        /// </summary>
        /// <returns></returns>
        public bool CanFragment()
        {
            if ((Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless || (Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

            switch (Info.Rarity)
            {
                case Rarity.Common:
                    if (Info.RequiredAmount <= 15) return false;
                    break;
                case Rarity.Superior:
                    break;
                case Rarity.Elite:
                    break;
            }

            switch (Info.ItemType)
            {
                case ItemType.Weapon:
                case ItemType.Armour:
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:
                case ItemType.Shoes:
                    break;
                default:
                    return false;
            }

            return true;
        }
        /// <summary>
        /// 道具碎片分解成本
        /// </summary>
        /// <returns></returns>
        public int FragmentCost()
        {
            switch (Info.Rarity)
            {
                case Rarity.Common:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                        case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Info.RequiredAmount * 10000 / 9;
                        /*  case ItemType.Helmet:
                          case ItemType.Necklace:
                          case ItemType.Bracelet:
                          case ItemType.Ring:
                          case ItemType.Shoes:
                              return Info.RequiredAmount * 7000 / 9;*/
                        default:
                            return 0;
                    }
                case Rarity.Superior:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                        case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Info.RequiredAmount * 10000 / 2;
                        /*  case ItemType.Helmet:
                          case ItemType.Necklace:
                          case ItemType.Bracelet:
                          case ItemType.Ring:
                          case ItemType.Shoes:
                              return Info.RequiredAmount * 10000 / 10;*/
                        default:
                            return 0;
                    }
                case Rarity.Elite:
                    switch (Info.ItemType)
                    {
                        case ItemType.Weapon:
                        case ItemType.Armour:
                            return 250000;
                        case ItemType.Helmet:
                            return 50000;
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            return 150000;
                        case ItemType.Shoes:
                            return 30000;
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }
        /// <summary>
        /// 道具碎片分解数量
        /// </summary>
        /// <returns></returns>
        public int FragmentCount()
        {
            switch (Info.Rarity)
            {
                case Rarity.Common:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                        case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Math.Max(1, Info.RequiredAmount / 2 + 5);
                        /*  case ItemType.Helmet:
                              return Math.Max(1, (Info.RequiredAmount - 30) / 6);
                          case ItemType.Necklace:
                              return Math.Max(1, Info.RequiredAmount / 8);
                          case ItemType.Bracelet:
                              return Math.Max(1, Info.RequiredAmount / 15);
                          case ItemType.Ring:
                              return Math.Max(1, Info.RequiredAmount / 9);
                          case ItemType.Shoes:
                              return Math.Max(1, (Info.RequiredAmount - 35) / 6);*/
                        default:
                            return 0;
                    }
                case Rarity.Superior:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                        case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Math.Max(1, Info.RequiredAmount / 2 + 5);
                        /*   case ItemType.Helmet:
                               return Math.Max(1, (Info.RequiredAmount - 30) / 6);
                           case ItemType.Necklace:
                               return Math.Max(1, Info.RequiredAmount / 10);
                           case ItemType.Bracelet:
                               return Math.Max(1, Info.RequiredAmount / 15);
                           case ItemType.Ring:
                               return Math.Max(1, Info.RequiredAmount / 10);
                           case ItemType.Shoes:
                               return Math.Max(1, (Info.RequiredAmount - 35) / 6);*/
                        default:
                            return 0;
                    }
                case Rarity.Elite:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                            return 50;
                        case ItemType.Helmet:
                            return 5;
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            return 10;
                        case ItemType.Shoes:
                            return 3;
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }
        /// <summary>
        /// 大师精炼
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public int MergeRefineElements(out Stat element)
        {
            int value = 0;
            element = Stats.GetWeaponElement();

            for (int i = AddedStats.Count - 1; i >= 0; i--)
            {
                UserItemStat stat = AddedStats[i];
                if (stat.StatSource != StatSource.Refine) continue;

                switch (stat.Stat)
                {
                    case Stat.FireAttack:
                    case Stat.IceAttack:
                    case Stat.LightningAttack:
                    case Stat.WindAttack:
                    case Stat.HolyAttack:
                    case Stat.DarkAttack:
                    case Stat.PhantomAttack:
                        value += stat.Amount;
                        stat.Delete();
                        break;
                }
            }

            if (value > 0 && element != Stat.None)
                AddStat(element, value, StatSource.Refine);

            return value;
        }


        /// <summary>
        /// 获取总额外属性
        /// </summary>
        /// <returns></returns>
        public int GetTotalAddedStat(Stat stat)
        {
            return AddedStats.Where(s => s.Stat == stat).Sum(s => s.Amount);
        }

        /// <summary>
        /// 获取总属性
        /// </summary>
        /// <returns></returns>
        public int GetTotalStat(Stat stat)
        {
            return GetFullItemStats().Where(fullItemStat => fullItemStat.Stat == stat).Sum(fullItemStat => fullItemStat.Amount);
        }

        #region 回购

        //哪类物品可以回购
        [IgnoreProperty]
        public bool CanBuyback => Info != null &&
                                  (Info.ItemType == ItemType.Armour ||
                                  Info.ItemType == ItemType.Weapon ||
                                  Info.ItemType == ItemType.Necklace ||
                                  Info.ItemType == ItemType.Helmet ||
                                  Info.ItemType == ItemType.Ring ||
                                  Info.ItemType == ItemType.Bracelet ||
                                  Info.ItemType == ItemType.Shoes ||
                                  Info.ItemType == ItemType.Book ||
                                  Info.ItemType == ItemType.Belt ||
                                  Info.ItemType == ItemType.Meat ||
                                  Info.ItemType == ItemType.Ore);

        public OwnerlessItemType OwnerlessType
        {
            get { return _OwnerlessType; }
            set
            {
                if (_OwnerlessType == value) return;

                var oldValue = _OwnerlessType;
                _OwnerlessType = value;

                OnChanged(oldValue, value, "OwnerlessType");
            }
        }
        private OwnerlessItemType _OwnerlessType = OwnerlessItemType.None;

        public DateTime OwnerLessTime
        {
            get { return _OwnerLessTime; }
            set
            {
                if (_OwnerLessTime == value) return;
                var oldValue = _OwnerLessTime;
                _OwnerLessTime = value;
                OnChanged(oldValue, value, "OwnerLessTime");
            }
        }

        private DateTime _OwnerLessTime;
        [IgnoreProperty]
        public bool IsOwnerless => OwnerlessType != OwnerlessItemType.None;
        public void MarkOwnerless(OwnerlessItemType type)
        {
            Slot = -1;
            Character = null;
            Account = null;
            Guild = null;
            Companion = null;
            Refine = null;
            Auction = null;
            Mail = null;
            UserTask = null;
            OwnerLessTime = SEnvir.Now;
            OwnerlessType = type;
        }

        #endregion
#endif
    }
}
