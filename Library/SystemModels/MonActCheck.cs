using MirDB;

namespace Library.SystemModels
{
    /// <summary>
    /// 自定义怪物AI里怪物的攻击检测
    /// </summary>
    public sealed class MonActCheck : DBObject
    {
        /// <summary>
        ///自定义怪物AI里怪物的攻击检测
        /// </summary>
        [Association("MonActChecks")]
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
        /// 检测类型
        /// </summary>
        public MonCheckType CheckType
        {
            get { return _CheckType; }
            set
            {
                if (_CheckType == value) return;

                var oldValue = _CheckType;
                _CheckType = value;

                OnChanged(oldValue, value, "CheckType");
            }
        }
        private MonCheckType _CheckType;
        /// <summary>
        /// 符号变量
        /// </summary>
        public Operators Operators
        {
            get { return _Operators; }
            set
            {
                if (_Operators == value) return;

                var oldValue = _Operators;
                _Operators = value;

                OnChanged(oldValue, value, "Operators");
            }
        }
        private Operators _Operators;
        /// <summary>
        /// 检测值
        /// </summary>
        public int IntParameter
        {
            get { return _IntParameter; }
            set
            {
                if (_IntParameter == value) return;

                var oldValue = _IntParameter;
                _IntParameter = value;

                OnChanged(oldValue, value, "IntParameter");
            }
        }
        private int _IntParameter;
    }
}
