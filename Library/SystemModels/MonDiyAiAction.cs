using MirDB;

namespace Library.SystemModels
{
    /// <summary>
    /// 自定义怪物AI
    /// </summary>
    public class MonDiyAiAction : DBObject
    {
        /// <summary>
        /// 怪物信息
        /// </summary>
        [Association("MonDiyAiActions")]
        public MonsterInfo Monster
        {
            get { return _Monster; }
            set
            {
                if (_Monster == value) return;

                var oldValue = _Monster;
                _Monster = value;

                OnChanged(oldValue, value, "Monster");
            }
        }
        private MonsterInfo _Monster;
        /// <summary>
        /// AI描述
        /// </summary>
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
        /// <summary>
        /// 怪物自定义攻击类型
        /// </summary>
        public MonDiyActType ActType
        {
            get { return _ActType; }
            set
            {
                if (_ActType == value) return;

                var oldValue = _ActType;
                _ActType = value;

                OnChanged(oldValue, value, "ActType");
            }
        }
        private MonDiyActType _ActType;
        /// <summary>
        /// 怪物选择动作类型
        /// </summary>
        public MirAnimation ActionID
        {
            get { return _ActionID; }
            set
            {
                if (_ActionID == value) return;

                var oldValue = _ActionID;
                _ActionID = value;

                OnChanged(oldValue, value, "ActionID");
            }
        }
        private MirAnimation _ActionID;
        /// <summary>
        /// 系统魔法选择
        /// </summary>
        public MagicType SystemMagic
        {
            get { return _SystemMagic; }
            set
            {
                if (_SystemMagic == value) return;

                var oldValue = _SystemMagic;
                _SystemMagic = value;

                OnChanged(oldValue, value, "SystemMagic");
            }
        }
        private MagicType _SystemMagic;
        /// <summary>
        /// 自定义技能动画ID
        /// </summary>
        public int MagicID
        {
            get { return _MagicID; }
            set
            {
                if (_MagicID == value) return;

                var oldValue = _MagicID;
                _MagicID = value;

                OnChanged(oldValue, value, "MagicID");
            }
        }
        private int _MagicID;
        /// <summary>
        /// 自定义怪物攻击力类型
        /// </summary>
        public MonDiyPowerType PowerType
        {
            get { return _PowerType; }
            set
            {
                if (_PowerType == value) return;

                var oldValue = _PowerType;
                _PowerType = value;

                OnChanged(oldValue, value, "PowerType");
            }
        }
        private MonDiyPowerType _PowerType;
        /// <summary>
        /// 自定义怪物元素类型
        /// </summary>
        public Element ElementType
        {
            get { return _ElementType; }
            set
            {
                if (_ElementType == value) return;

                var oldValue = _ElementType;
                _ElementType = value;

                OnChanged(oldValue, value, "ElementType");
            }
        }
        private Element _ElementType;
        /// <summary>
        /// 目标类型
        /// </summary>
        public TargetType Target
        {
            get { return _Target; }
            set
            {
                if (_Target == value) return;

                var oldValue = _Target;
                _Target = value;

                OnChanged(oldValue, value, "Target");
            }
        }
        private TargetType _Target;
        /// <summary>
        /// 动作效果延迟
        /// </summary>
        public int nDelay
        {
            get { return _nDelay; }
            set
            {
                if (_nDelay == value) return;

                var oldValue = _nDelay;
                _nDelay = value;

                OnChanged(oldValue, value, "nDelay");
            }
        }
        private int _nDelay;

        [Association("MonActChecks", true)]
        public DBBindingList<MonActCheck> MonActChecks { get; set; }

        [Association("MonAttackEffects", true)]
        public DBBindingList<MonAttackEffect> MonAttackEffects { get; set; }

        protected override internal void OnCreated()
        {
            base.OnCreated();

            Index += 1000;
        }
    }
}
