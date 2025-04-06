using Library;
using MirDB;
using System.Windows.Forms;

namespace Client.UserModels
{
    [UserObject]
    /// <summary>
    /// 按键绑定信息
    /// </summary>
    public sealed class KeyBindInfo : DBObject
    {
        /// <summary>
        /// 类别
        /// </summary>
        public string Category
        {
            get { return _Category; }
            set
            {
                if (_Category == value) return;

                var oldValue = _Category;
                _Category = value;

                OnChanged(oldValue, value, "Category");
            }
        }
        private string _Category;
        /// <summary>
        /// 按键绑定操作
        /// </summary>
        public KeyBindAction Action
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
        private KeyBindAction _Action;

        public bool Control1
        {
            get { return _Control1; }
            set
            {
                if (_Control1 == value) return;

                var oldValue = _Control1;
                _Control1 = value;

                OnChanged(oldValue, value, "Control1");
            }
        }
        private bool _Control1;

        public bool Alt1
        {
            get { return _Alt1; }
            set
            {
                if (_Alt1 == value) return;

                var oldValue = _Alt1;
                _Alt1 = value;

                OnChanged(oldValue, value, "Alt1");
            }
        }
        private bool _Alt1;

        public bool Shift1
        {
            get { return _Shift1; }
            set
            {
                if (_Shift1 == value) return;

                var oldValue = _Shift1;
                _Shift1 = value;

                OnChanged(oldValue, value, "Shift1");
            }
        }
        private bool _Shift1;

        public Keys Key1
        {
            get { return _Key1; }
            set
            {
                if (_Key1 == value) return;

                var oldValue = _Key1;
                _Key1 = value;

                OnChanged(oldValue, value, "Key1");
            }
        }
        private Keys _Key1;

        public bool Control2
        {
            get { return _Control2; }
            set
            {
                if (_Control2 == value) return;

                var oldValue = _Control2;
                _Control2 = value;

                OnChanged(oldValue, value, "Control2");
            }
        }
        private bool _Control2;

        public bool Shift2
        {
            get { return _Shift2; }
            set
            {
                if (_Shift2 == value) return;

                var oldValue = _Shift2;
                _Shift2 = value;

                OnChanged(oldValue, value, "Shift2");
            }
        }
        private bool _Shift2;

        public bool Alt2
        {
            get { return _Alt2; }
            set
            {
                if (_Alt2 == value) return;

                var oldValue = _Alt2;
                _Alt2 = value;

                OnChanged(oldValue, value, "Alt2");
            }
        }
        private bool _Alt2;

        public Keys Key2
        {
            get { return _Key2; }
            set
            {
                if (_Key2 == value) return;

                var oldValue = _Key2;
                _Key2 = value;

                OnChanged(oldValue, value, "Key2");
            }
        }
        private Keys _Key2;
    }
}
