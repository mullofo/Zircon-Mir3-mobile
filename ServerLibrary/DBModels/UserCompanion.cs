using Library;
using Library.SystemModels;
using MirDB;
using System.Linq;

namespace Server.DBModels
{
    [UserObject]
    public sealed class UserCompanion : DBObject   //角色的宠物
    {
        [Association("Companions")]
        public AccountInfo Account   //账号
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

        [Association("Companion")]
        public CharacterInfo Character   //角色
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

        public CompanionInfo Info   //宠物信息
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
        private CompanionInfo _Info;

        public string Name   //名字
        {
            get { return _Name; }
            set
            {
                if (_Name == value) return;

                var oldValue = _Name;
                _Name = value;

                OnChanged(oldValue, value, "Name");
            }
        }
        private string _Name;

        public int Level   //等级
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

        public int Hunger   //饥饿度
        {
            get { return _Hunger; }
            set
            {
                if (_Hunger == value) return;

                var oldValue = _Hunger;
                _Hunger = value;

                OnChanged(oldValue, value, "Hunger");
            }
        }
        private int _Hunger;

        public int Experience   //经验
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
        private int _Experience;

        public Stats Level3
        {
            get { return _Level3; }
            set
            {
                if (_Level3 == value) return;

                var oldValue = _Level3;
                _Level3 = value;

                OnChanged(oldValue, value, "Level3");
            }
        }
        private Stats _Level3;

        public Stats Level5
        {
            get { return _Level5; }
            set
            {
                if (_Level5 == value) return;

                var oldValue = _Level5;
                _Level5 = value;

                OnChanged(oldValue, value, "Level5");
            }
        }
        private Stats _Level5;

        public Stats Level7
        {
            get { return _Level7; }
            set
            {
                if (_Level7 == value) return;

                var oldValue = _Level7;
                _Level7 = value;

                OnChanged(oldValue, value, "Level7");
            }
        }
        private Stats _Level7;

        public Stats Level10
        {
            get { return _Level10; }
            set
            {
                if (_Level10 == value) return;

                var oldValue = _Level10;
                _Level10 = value;

                OnChanged(oldValue, value, "Level10");
            }
        }
        private Stats _Level10;

        public Stats Level11
        {
            get { return _Level11; }
            set
            {
                if (_Level11 == value) return;

                var oldValue = _Level11;
                _Level11 = value;

                OnChanged(oldValue, value, "Level11");
            }
        }
        private Stats _Level11;

        public Stats Level13
        {
            get { return _Level13; }
            set
            {
                if (_Level13 == value) return;

                var oldValue = _Level13;
                _Level13 = value;

                OnChanged(oldValue, value, "Level13");
            }
        }
        private Stats _Level13;

        public Stats Level15
        {
            get { return _Level15; }
            set
            {
                if (_Level15 == value) return;

                var oldValue = _Level15;
                _Level15 = value;

                OnChanged(oldValue, value, "Level15");
            }
        }
        private Stats _Level15;

        public bool AutoFeed         //宠物自动粮仓
        {
            get { return _AutoFeed; }
            set
            {
                if (_AutoFeed == value) return;

                var oldValue = _AutoFeed;
                _AutoFeed = value;

                OnChanged(oldValue, value, "AutoFeed");
            }
        }
        private bool _AutoFeed;

        [Association("Items", true)]
        public DBBindingList<UserItem> Items { get; set; }
        [Association("Sortings", true)]
        public DBBindingList<UserSorting> Sortings { get; set; }
        [Association("SortingsLev", true)]
        public DBBindingList<UserSortingLev> SortingsLev { get; set; }

        protected override internal void OnDeleted()   //删除时
        {
            Account = null;
            Character = null;
            Info = null;

            base.OnDeleted();
        }

#if !ServerTool
        public ClientUserCompanion ToClientInfo()   //更新到客户端信息
        {
            return new ClientUserCompanion
            {
                Index = Index,
                CharacterName = Character?.CharacterName,
                CompanionIndex = Info.Index,
                Name = Name,
                Level = Level,
                Hunger = Hunger,
                Experience = Experience,
                Level3 = Level3,
                Level5 = Level5,
                Level7 = Level7,
                Level10 = Level10,
                Level11 = Level11,
                Level13 = Level13,
                Level15 = Level15,
                AutoFeed = AutoFeed,       //宠物自动粮仓
                SortsLev = SortingsLev.Select(x => x.ToClientInfo()).ToList(),
                Sorts = Sortings.Select(x => x.ToClientInfo()).ToList(),
                Items = Items.Select(x => x.ToClientInfo()).ToList(),

            };
        }
#endif


        public override string ToString()
        {
            return Account?.EMailAddress ?? string.Empty;
        }
    }


    [UserObject]
    public sealed class UserSorting : DBObject
    {
        [Association("Sortings")]
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
        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                if (_Enabled == value) return;

                var oldValue = _Enabled;
                _Enabled = value;

                OnChanged(oldValue, value, "Enabled");
            }
        }
        private bool _Enabled;

        public ItemType SetType
        {
            get { return _SetType; }
            set
            {
                if (_SetType == value) return;

                var oldValue = _SetType;
                _SetType = value;

                OnChanged(oldValue, value, "Enabled");
            }
        }
        private ItemType _SetType;

#if !ServerTool
        public ClientCompanionSortSet ToClientInfo()
        {
            return new ClientCompanionSortSet
            {
                SetType = SetType,
                Enabled = Enabled,
            };

        }
#endif

        public override string ToString()
        {
            return Companion?.Account?.EMailAddress ?? string.Empty;
        }
    }

    [UserObject]
    public sealed class UserSortingLev : DBObject
    {
        [Association("SortingsLev")]
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
        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                if (_Enabled == value) return;

                var oldValue = _Enabled;
                _Enabled = value;

                OnChanged(oldValue, value, "Enabled");
            }
        }
        private bool _Enabled;

        public Rarity SetType
        {
            get { return _SetType; }
            set
            {
                if (_SetType == value) return;

                var oldValue = _SetType;
                _SetType = value;

                OnChanged(oldValue, value, "Enabled");
            }
        }
        private Rarity _SetType;

#if !ServerTool
        public ClientCompanionSortLevSet ToClientInfo()
        {
            return new ClientCompanionSortLevSet
            {
                SetType = SetType,
                Enabled = Enabled,
            };

        }
#endif
        public override string ToString()
        {
            return Companion?.Account?.EMailAddress ?? string.Empty;
        }
    }
}
