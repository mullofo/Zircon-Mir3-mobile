using MirDB;

namespace Library.SystemModels
{
    /// <summary>
    /// 自定义怪物AI的攻击效果
    /// </summary>
    public sealed class MonAttackEffect : DBObject
    {
        /// <summary>
        /// 自定义怪物AI的攻击效果
        /// </summary>
        [Association("MonAttackEffects")]
        public MonDiyAiAction MonDAction
        {
            get { return _MonDAction; }
            set
            {
                if (_MonDAction == value) return;

                var oldValue = _MonDAction;
                _MonDAction = value;

                OnChanged(oldValue, value, "MonDAction");
            }
        }
        private MonDiyAiAction _MonDAction;
        /// <summary>
        /// 攻击效果
        /// </summary>
        public AttackEffect AtkEffect
        {
            get { return _AtkEffect; }
            set
            {
                if (_AtkEffect == value) return;

                var oldValue = _AtkEffect;
                _AtkEffect = value;

                OnChanged(oldValue, value, "AtkEffect");
            }
        }
        private AttackEffect _AtkEffect;
        /// <summary>
        /// 数值参数
        /// </summary>
        public int nParameter
        {
            get { return _nParameter; }
            set
            {
                if (_nParameter == value) return;

                var oldValue = _nParameter;
                _nParameter = value;

                OnChanged(oldValue, value, "nParameter");
            }
        }
        private int _nParameter;
        /// <summary>
        /// 文本参数
        /// </summary>
        public string sParameter
        {
            get { return _sParameter; }
            set
            {
                if (_sParameter == value) return;

                var oldValue = _sParameter;
                _sParameter = value;

                OnChanged(oldValue, value, "_sParameter");
            }
        }
        private string _sParameter;
    }
}
