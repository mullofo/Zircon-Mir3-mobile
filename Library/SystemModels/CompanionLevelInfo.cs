using MirDB;

namespace Library.SystemModels
{
    /// <summary>
    /// 宠物等级信息
    /// </summary>
    public sealed class CompanionLevelInfo : DBObject
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
        /// 宠物等级设置
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
        /// 宠物最大经验值
        /// </summary>
        public int MaxExperience
        {
            get { return _MaxExperience; }
            set
            {
                if (_MaxExperience == value) return;

                var oldValue = _MaxExperience;
                _MaxExperience = value;

                OnChanged(oldValue, value, "MaxExperience");
            }
        }
        private int _MaxExperience;
        /// <summary>
        /// 宠物背包格子数
        /// </summary>
        public int InventorySpace
        {
            get { return _InventorySpace; }
            set
            {
                if (_InventorySpace == value) return;

                var oldValue = _InventorySpace;
                _InventorySpace = value;

                OnChanged(oldValue, value, "InventorySpace");
            }
        }
        private int _InventorySpace;
        /// <summary>
        /// 宠物包裹负重
        /// </summary>
        public int InventoryWeight
        {
            get { return _InventoryWeight; }
            set
            {
                if (_InventoryWeight == value) return;

                var oldValue = _InventoryWeight;
                _InventoryWeight = value;

                OnChanged(oldValue, value, "InventoryWeight");
            }
        }
        private int _InventoryWeight;
        /// <summary>
        /// 宠物最大饥饿度
        /// </summary>
        public int MaxHunger
        {
            get { return _MaxHunger; }
            set
            {
                if (_MaxHunger == value) return;

                var oldValue = _MaxHunger;
                _MaxHunger = value;

                OnChanged(oldValue, value, "MaxHunger");
            }
        }
        private int _MaxHunger;

    }
}
