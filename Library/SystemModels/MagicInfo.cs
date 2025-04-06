using MirDB;
using static MirDB.Association;

namespace Library.SystemModels
{
    /// <summary>
    /// 魔法技能信息
    /// </summary>
    [Lang]
    public sealed class MagicInfo : DBObject
    {
        /// <summary>
        /// 技能名字
        /// </summary>
        [Lang("技能名")]
        [Export]
        public string Name
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

        /// <summary>
        /// 技能类型
        /// </summary>
        public MagicType Magic
        {
            get { return _Magic; }
            set
            {
                if (_Magic == value) return;

                var oldValue = _Magic;
                _Magic = value;

                OnChanged(oldValue, value, "Magic");
            }
        }
        private MagicType _Magic;

        /// <summary>
        /// 技能对应职业
        /// </summary>
        public MirClass Class
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
        private MirClass _Class;

        /// <summary>
        /// 技能树
        /// </summary>
        public MagicSchool School
        {
            get { return _School; }
            set
            {
                if (_School == value) return;

                var oldValue = _School;
                _School = value;

                OnChanged(oldValue, value, "School");
            }
        }
        private MagicSchool _School;

        /// <summary>
        /// 技能主动被动区分
        /// </summary>
        public MagicAction Action
        {
            get { return _Action; }
            set
            {
                if (_Action == value) return;

                var oldValue = _Action;
                _Action = value;

                OnChanged(oldValue, value, "Action");
            }
        }
        private MagicAction _Action;

        /// <summary>
        /// 技能图标
        /// </summary>
        public int Icon
        {
            get { return _Icon; }
            set
            {
                if (_Icon == value) return;

                var oldValue = _Icon;
                _Icon = value;

                OnChanged(oldValue, value, "Icon");
            }
        }
        private int _Icon;

        /// <summary>
        /// 技能最小基础攻击值
        /// </summary>
        public int MinBasePower
        {
            get { return _MinBasePower; }
            set
            {
                if (_MinBasePower == value) return;

                var oldValue = _MinBasePower;
                _MinBasePower = value;

                OnChanged(oldValue, value, "MinBasePower");
            }
        }
        private int _MinBasePower;

        /// <summary>
        /// 技能最大基础攻击值
        /// </summary>
        public int MaxBasePower
        {
            get { return _MaxBasePower; }
            set
            {
                if (_MaxBasePower == value) return;

                var oldValue = _MaxBasePower;
                _MaxBasePower = value;

                OnChanged(oldValue, value, "MaxBasePower");
            }
        }
        private int _MaxBasePower;

        /// <summary>
        /// 技能升级最小攻击加成值
        /// </summary>
        public int MinLevelPower
        {
            get { return _MinLevelPower; }
            set
            {
                if (_MinLevelPower == value) return;

                var oldValue = _MinLevelPower;
                _MinLevelPower = value;

                OnChanged(oldValue, value, "MinLevelPower");
            }
        }
        private int _MinLevelPower;

        /// <summary>
        /// 技能升级最大攻击加成值
        /// </summary>
        public int MaxLevelPower
        {
            get { return _MaxLevelPower; }
            set
            {
                if (_MaxLevelPower == value) return;

                var oldValue = _MaxLevelPower;
                _MaxLevelPower = value;

                OnChanged(oldValue, value, "MaxLevelPower");
            }
        }
        private int _MaxLevelPower;

        /// <summary>
        /// 技能基础施法消耗
        /// </summary>
        public int BaseCost
        {
            get { return _BaseCost; }
            set
            {
                if (_BaseCost == value) return;

                var oldValue = _BaseCost;
                _BaseCost = value;

                OnChanged(oldValue, value, "BaseCost");
            }
        }
        private int _BaseCost;

        /// <summary>
        /// 技能升级施法消耗
        /// </summary>
        public int LevelCost
        {
            get { return _LevelCost; }
            set
            {
                if (_LevelCost == value) return;

                var oldValue = _LevelCost;
                _LevelCost = value;

                OnChanged(oldValue, value, "LevelCost");
            }
        }
        private int _LevelCost;

        /// <summary>
        /// 技能等级1
        /// </summary>
        public int NeedLevel1
        {
            get { return _NeedLevel1; }
            set
            {
                if (_NeedLevel1 == value) return;

                var oldValue = _NeedLevel1;
                _NeedLevel1 = value;

                OnChanged(oldValue, value, "NeedLevel1");
            }
        }
        private int _NeedLevel1;

        /// <summary>
        /// 技能等级2
        /// </summary>
        public int NeedLevel2
        {
            get { return _NeedLevel2; }
            set
            {
                if (_NeedLevel2 == value) return;

                var oldValue = _NeedLevel2;
                _NeedLevel2 = value;

                OnChanged(oldValue, value, "NeedLevel2");
            }
        }
        private int _NeedLevel2;

        /// <summary>
        /// 技能等级3
        /// </summary>
        public int NeedLevel3
        {
            get { return _NeedLevel3; }
            set
            {
                if (_NeedLevel3 == value) return;

                var oldValue = _NeedLevel3;
                _NeedLevel3 = value;

                OnChanged(oldValue, value, "NeedLevel3");
            }
        }
        private int _NeedLevel3;

        /// <summary>
        /// 技能等级1经验要求
        /// </summary>
        public int Experience1
        {
            get { return _Experience1; }
            set
            {
                if (_Experience1 == value) return;

                var oldValue = _Experience1;
                _Experience1 = value;

                OnChanged(oldValue, value, "Experience1");
            }
        }
        private int _Experience1;

        /// <summary>
        /// 技能等级2经验要求
        /// </summary>
        public int Experience2
        {
            get { return _Experience2; }
            set
            {
                if (_Experience2 == value) return;

                var oldValue = _Experience2;
                _Experience2 = value;

                OnChanged(oldValue, value, "Experience2");
            }
        }
        private int _Experience2;

        /// <summary>
        /// 技能等级3经验要求
        /// </summary>
        public int Experience3
        {
            get { return _experience3; }
            set
            {
                if (_experience3 == value) return;

                var oldValue = _experience3;
                _experience3 = value;

                OnChanged(oldValue, value, "Experience3");
            }
        }
        private int _experience3;

        /// <summary>
        /// 技能释放延迟时间
        /// </summary>
        public int Delay
        {
            get { return _Delay; }
            set
            {
                if (_Delay == value) return;

                var oldValue = _Delay;
                _Delay = value;

                OnChanged(oldValue, value, "Delay");
            }
        }
        private int _Delay;

        /// <summary>
        /// 技能说明
        /// </summary>
        [Lang("技能说明")]
        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description == value) return;

                var oldValue = _Description;
                _Description = value;

                OnChanged(oldValue, value, "Description");
            }
        }
        private string _Description;

        #region UI
        /// <summary>
        /// 145技能页面对应X坐标
        /// </summary>
        public int X
        {
            get { return _X; }
            set
            {
                if (_X == value) return;

                var oldValue = _X;
                _X = value;

                OnChanged(oldValue, value, "X");
            }
        }
        private int _X;
        /// <summary>
        /// 145技能页面对应Y坐标
        /// </summary>
        public int Y
        {
            get { return _Y; }
            set
            {
                if (_Y == value) return;

                var oldValue = _Y;
                _Y = value;

                OnChanged(oldValue, value, "Y");
            }
        }
        private int _Y;
        #endregion
    }
}
