using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public class GolbleValue : DBObject
    {
        /// <summary>
        /// 角色绑定的键
        /// </summary>
        public int Key
        {
            get { return _Key; }
            set
            {
                if (_Key == value) return;

                var oldValue = _Key;
                _Key = value;

                OnChanged(oldValue, value, "Key");
            }
        }
        private int _Key;
        /// <summary>
        /// 角色绑定的键值
        /// </summary>
        public int Value
        {
            get { return _Value; }
            set
            {
                if (_Value == value) return;

                var oldValue = _Value;
                _Value = value;

                OnChanged(oldValue, value, "Value");
            }
        }
        private int _Value;

        public object ObjctValue
        {
            get { return _ObjctValue; }
            set
            {
                if (_ObjctValue == value) return;

                var oldValue = _ObjctValue;
                _ObjctValue = value;

                OnChanged(oldValue, value, "ObjctValue");
            }
        }
        private object _ObjctValue;
    }
}
