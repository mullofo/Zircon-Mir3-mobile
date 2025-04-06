using MirDB;

namespace Library.SystemModels
{
    /// <summary>
    /// 新版武器升级
    /// </summary>
    public sealed class WeaponUpgradeNew : DBObject
    {
        /// <summary>
        /// 增加等级
        /// </summary>
        public int Increment
        {
            get => _Increment;
            set
            {
                if (_Increment == value) return;

                var oldValue = _Increment;
                _Increment = value;

                OnChanged(oldValue, value, "Increment");
            }
        }
        private int _Increment;
        /// <summary>
        /// 黑铁矿
        /// </summary>
        public int BlackIronOre
        {
            get => _BlackIronOre;
            set
            {
                if (_BlackIronOre == value) return;

                var oldValue = _BlackIronOre;
                _BlackIronOre = value;

                OnChanged(oldValue, value, "BlackIronOre");
            }
        }
        private int _BlackIronOre;
        /// <summary>
        /// 花费金币
        /// </summary>
        public int SpendGold
        {
            get => _SpendGold;
            set
            {
                if (_SpendGold == value) return;

                var oldValue = _SpendGold;
                _SpendGold = value;

                OnChanged(oldValue, value, "SpendGold");
            }
        }
        private int _SpendGold;
        /// <summary>
        /// 初级碎片
        /// </summary>
        public int BasicFragment
        {
            get => _BasicFragment;
            set
            {
                if (_BasicFragment == value) return;

                var oldValue = _BasicFragment;
                _BasicFragment = value;

                OnChanged(oldValue, value, "BasicFragment");
            }
        }
        private int _BasicFragment;
        /// <summary>
        /// 中级碎片
        /// </summary>
        public int AdvanceFragment
        {
            get => _AdvanceFragment;
            set
            {
                if (_AdvanceFragment == value) return;

                var oldValue = _AdvanceFragment;
                _AdvanceFragment = value;

                OnChanged(oldValue, value, "AdvanceFragment");
            }
        }
        private int _AdvanceFragment;
        /// <summary>
        /// 高级碎片
        /// </summary>
        public int SeniorFragment
        {
            get => _SeniorFragment;
            set
            {
                if (_SeniorFragment == value) return;

                var oldValue = _SeniorFragment;
                _SeniorFragment = value;

                OnChanged(oldValue, value, "SeniorFragment");
            }
        }
        private int _SeniorFragment;
        /// <summary>
        /// 制练石
        /// </summary>
        public int RefinementStone
        {
            get => _RefinementStone;
            set
            {
                if (_RefinementStone == value) return;

                var oldValue = _RefinementStone;
                _RefinementStone = value;

                OnChanged(oldValue, value, "RefinementStone");
            }
        }
        private int _RefinementStone;
        /// <summary>
        /// 成功率
        /// </summary>
        public int SuccessRate
        {
            get { return _SuccessRate; }
            set
            {
                if (_SuccessRate == value) return;

                var oldValue = _SuccessRate;
                _SuccessRate = value;

                OnChanged(oldValue, value, "SuccessRate");
            }
        }
        private int _SuccessRate;
        /// <summary>
        /// 失败率
        /// </summary>
        public int FailureRate
        {
            get { return _FailureRate; }
            set
            {
                if (_FailureRate == value) return;

                var oldValue = _FailureRate;
                _FailureRate = value;

                OnChanged(oldValue, value, "FailureRate");
            }
        }
        private int _FailureRate;
        /// <summary>
        /// 所需时间
        /// </summary>
        public int TimeCost
        {
            get { return _TimeCost; }
            set
            {
                if (_TimeCost == value) return;

                var oldValue = _TimeCost;
                _TimeCost = value;

                OnChanged(oldValue, value, "TimeCost");
            }
        }
        private int _TimeCost;
    }
}
