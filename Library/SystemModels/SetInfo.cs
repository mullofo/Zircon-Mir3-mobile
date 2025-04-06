using MirDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.SystemModels
{
    /// <summary>
    /// 套装属性信息
    /// </summary>
    public class SetInfo : DBObject
    {
        /// <summary>
        /// 套装名字
        /// </summary>
        public string SetName
        {
            get { return _SetName; }
            set
            {
                if (_SetName == value) return;

                var oldValue = _SetName;
                _SetName = value;

                OnChanged(oldValue, value, "SetName");
            }
        }
        private string _SetName;

        /// <summary>
        /// 套装信息是否显示
        /// </summary>
        public bool SetInfoShow
        {
            get { return _SetInfoShowe; }
            set
            {
                if (_SetInfoShowe == value) return;

                var oldValue = _SetInfoShowe;
                _SetInfoShowe = value;

                OnChanged(oldValue, value, "SetInfoShow");
            }
        }
        private bool _SetInfoShowe;

        /// <summary>
        /// 套装描述
        /// </summary>
        public string SetDescription
        {
            get { return _SetDescription; }
            set
            {
                if (_SetDescription == value) return;

                var oldValue = _SetDescription;
                _SetDescription = value;

                OnChanged(oldValue, value, "SetDescription");
            }
        }
        private string _SetDescription;

        /// <summary>
        /// 套装搭配
        /// </summary>
        [Association("SetGroups", true)]
        public DBBindingList<SetGroup> SetGroups { get; set; }

    }

    /// <summary>
    /// 套装分组
    /// </summary>
    public sealed class SetGroup : DBObject
    {
        /// <summary>
        /// 搭配名称
        /// </summary>
        public string GroupName
        {
            get { return _GroupName; }
            set
            {
                if (_GroupName == value) return;

                var oldValue = _GroupName;
                _GroupName = value;

                OnChanged(oldValue, value, "GroupName");
            }
        }
        private string _GroupName;

        /// <summary>
        /// 搭配要求类型
        /// </summary>
        public ItemSetRequirementType SetRequirement
        {
            get { return _SetRequirement; }
            set
            {
                if (_SetRequirement == value) return;

                var oldValue = _SetRequirement;
                _SetRequirement = value;

                OnChanged(oldValue, value, "SetRequirement");
            }
        }
        private ItemSetRequirementType _SetRequirement;

        /// <summary>
        /// 触发所需件数
        /// </summary>
        public int RequiredNumItems
        {
            get { return _requiredNumItems; }
            set
            {
                if (_requiredNumItems == value) return;

                var oldValue = _requiredNumItems;
                _requiredNumItems = value;

                //if (_requiredNumItems > SetGroupItems.Count)
                //    _requiredNumItems = SetGroupItems.Count;

                OnChanged(oldValue, value, "RequiredNumItems");
            }
        }
        private int _requiredNumItems;

        /// <summary>
        /// 道具信息
        /// </summary>
        [Association("SetGroupItems", true)]
        public DBBindingList<SetGroupItem> SetGroupItems { get; set; }

        /// <summary>
        /// 套装属性
        /// </summary>
        [Association("GroupStats", true)]
        public DBBindingList<SetInfoStat> GroupStats { get; set; }

        [Association("SetGroups")]
        public SetInfo Set
        {
            get { return _Set; }
            set
            {
                if (_Set == value) return;

                var oldValue = _Set;
                _Set = value;

                OnChanged(oldValue, value, "Set");
            }
        }
        private SetInfo _Set;

        // 对于当前SetGroup, 玩家穿了哪几件
        public List<ItemInfo> GetItemsWearing(List<ItemInfo> wearing)
        {
            var result = new List<ItemInfo>();
            if (wearing == null || wearing.Count < 1) return result;

            int[] pool = GetSetGroupItemInfoList().Select(x => x.Index).ToArray();
            foreach (ItemInfo itemInfo in wearing)
            {
                int index = Array.IndexOf(pool, itemInfo.Index);
                if (index >= 0)
                {
                    pool[index] = -1; // -1 marks it as empty
                    result.Add(itemInfo);
                }
            }
            return result;
            //return wearing.Where(GetSetGroupItemInfoList().Remove).ToList();
            //return wearing.Intersect(GetSetGroupItemInfoList()).ToList();
        }

        // 对于当前SetGroup, 玩家穿了几件
        public int GetNumItemsWearing(List<ItemInfo> wearing)
        {
            return GetItemsWearing(wearing).Count();
        }

        // 想要激活此SetGroup, 需要穿几件
        public int GetRequiredNumItem()
        {
            return Math.Min(SetGroupItems.Count, RequiredNumItems);
        }

        // 获取套件列表
        public List<ItemInfo> GetSetGroupItemInfoList()
        {
            return SetGroupItems.Select(x => x.SetGroupItemInfo).ToList();
        }

        // 获取搭配属性
        public Stats GetSetGroupStats(int level, MirClass playerClass)
        {
            Stats groupStats = new Stats();

            foreach (SetInfoStat stat in GroupStats)
            {
                if (level < stat.Level) continue;

                switch (playerClass)
                {
                    case MirClass.Warrior:
                        if ((stat.Class & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                        break;
                    case MirClass.Wizard:
                        if ((stat.Class & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                        break;
                    case MirClass.Taoist:
                        if ((stat.Class & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                        break;
                    case MirClass.Assassin:
                        if ((stat.Class & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                        break;
                }
                groupStats[stat.Stat] += stat.Amount;
            }

            return groupStats;
        }

        protected internal override void OnCreated()
        {
            base.OnCreated();

            SetRequirement = ItemSetRequirementType.WithoutReplacement;
        }

    }

    public sealed class SetGroupItem : DBObject
    {
        [Association("SetGroupItems")]
        public SetGroup SetGroupInfo
        {
            get { return _SetGroupInfo; }
            set
            {
                if (_SetGroupInfo == value) return;

                var oldValue = _SetGroupInfo;
                _SetGroupInfo = value;

                OnChanged(oldValue, value, "SetGroupInfo");
            }
        }
        private SetGroup _SetGroupInfo;

        /// <summary>
        /// 道具信息
        /// </summary>
       // [Association("GroupItemInfo")]
        public ItemInfo SetGroupItemInfo
        {
            get { return _SetGroupItemInfo; }
            set
            {
                if (_SetGroupItemInfo == value) return;

                var oldValue = _SetGroupItemInfo;
                _SetGroupItemInfo = value;

                OnChanged(oldValue, value, "SetGroupItemInfo");
            }
        }
        private ItemInfo _SetGroupItemInfo;
    }

    public sealed class SetInfoStat : DBObject    //套装属性信息
    {
        [Association("GroupStats")]
        public SetGroup Group
        {
            get { return _Group; }
            set
            {
                if (_Group == value) return;

                var oldValue = _Group;
                _Group = value;

                OnChanged(oldValue, value, "Group");
            }
        }
        private SetGroup _Group;

        public Stat Stat
        {
            get { return _Stat; }
            set
            {
                if (_Stat == value) return;

                var oldValue = _Stat;
                _Stat = value;

                OnChanged(oldValue, value, "Stat");
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

                OnChanged(oldValue, value, "Amount");
            }
        }
        private int _Amount;

        public RequiredClass Class
        {
            get { return _Class; }
            set
            {
                if (_Class == value) return;

                var oldValue = _Class;
                _Class = value;

                OnChanged(oldValue, value, "Class");
            }
        }
        private RequiredClass _Class;

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


        protected internal override void OnCreated()
        {
            base.OnCreated();

            Class = RequiredClass.All;
        }
    }
}
