using MirDB;

namespace Library.SystemModels
{
    /// <summary>
    /// 宠物技能信息
    /// </summary>
    public sealed class CompanionSkillInfo : DBObject
    {
        /// <summary>
        /// 宠物信息
        /// </summary>
        public MonsterInfo MonsterInfo
        {
            get { return _MonsterInfo; }
            set
            {
                if (_MonsterInfo == value) return;

                var oldValue = _MonsterInfo;
                _MonsterInfo = value;

                OnChanged(oldValue, value, "MonsterInfo");
            }
        }
        private MonsterInfo _MonsterInfo;
        /// <summary>
        /// 宠物技能等级设置
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
        /// 宠物增加的属性
        /// </summary>
        public Stat StatType
        {
            get { return _StatType; }
            set
            {
                if (_StatType == value) return;

                var oldValue = _StatType;
                _StatType = value;

                OnChanged(oldValue, value, "StatType");
            }
        }
        private Stat _StatType;
        /// <summary>
        /// 宠物属性最大值
        /// </summary>
        public int MaxAmount
        {
            get { return _MaxAmount; }
            set
            {
                if (_MaxAmount == value) return;

                var oldValue = _MaxAmount;
                _MaxAmount = value;

                OnChanged(oldValue, value, "MaxAmount");
            }
        }
        private int _MaxAmount;
        /// <summary>
        /// 宠物获得属性的几率值
        /// </summary>
        public int Weight
        {
            get { return _Weight; }
            set
            {
                if (_Weight == value) return;

                var oldValue = _Weight;
                _Weight = value;

                OnChanged(oldValue, value, "Weight");
            }
        }
        private int _Weight;
    }
}
