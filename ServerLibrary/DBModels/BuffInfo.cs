using Library;
using MirDB;
using System;

namespace Server.DBModels
{
    [UserObject]
    public sealed class BuffInfo : DBObject   //BUFF信息
    {
        [Association("Buffs")]
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

        [Association("Buffs")]
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

        public BuffType Type   //BUFF类型
        {
            get { return _Type; }
            set
            {
                if (_Type == value) return;

                var oldValue = _Type;
                _Type = value;

                OnChanged(oldValue, value, "Type");
            }
        }
        private BuffType _Type;

        public Stats Stats  //增加的属性状态
        {
            get { return _Stats; }
            set
            {
                if (_Stats == value) return;

                var oldValue = _Stats;
                _Stats = value;

                OnChanged(oldValue, value, "Stats");
            }
        }
        private Stats _Stats;

        public TimeSpan RemainingTime  //剩余时间
        {
            get { return _RemainingTime; }
            set
            {
                if (_RemainingTime == value) return;

                var oldValue = _RemainingTime;
                _RemainingTime = value;

                OnChanged(oldValue, value, "RemainingTime");
            }
        }
        private TimeSpan _RemainingTime;

        public TimeSpan TickFrequency   //频率
        {
            get { return _TickFrequency; }
            set
            {
                if (_TickFrequency == value) return;

                var oldValue = _TickFrequency;
                _TickFrequency = value;

                OnChanged(oldValue, value, "TickFrequency");
            }
        }
        private TimeSpan _TickFrequency;

        public TimeSpan TickTime   //频率时间
        {
            get { return _TickTime; }
            set
            {
                if (_TickTime == value) return;

                var oldValue = _TickTime;
                _TickTime = value;

                OnChanged(oldValue, value, "TickTime");
            }
        }
        private TimeSpan _TickTime;

        public int ItemIndex   //道具ID
        {
            get { return _ItemIndex; }
            set
            {
                if (_ItemIndex == value) return;

                var oldValue = _ItemIndex;
                _ItemIndex = value;

                OnChanged(oldValue, value, "ItemIndex");
            }
        }
        private int _ItemIndex;

        //从哪个自定义buff转化而来
        public int FromCustomBuff
        {
            get { return _FromCustomBuff; }
            set
            {
                if (_FromCustomBuff == value) return;

                var oldValue = _FromCustomBuff;
                _FromCustomBuff = value;

                OnChanged(oldValue, value, "FromCustomBuff");
            }
        }
        private int _FromCustomBuff;

        public bool Visible   //是否可见
        {
            get { return _Visible; }
            set
            {
                if (_Visible == value) return;

                var oldValue = _Visible;
                _Visible = value;

                OnChanged(oldValue, value, "Visible");
            }
        }
        private bool _Visible;

        public bool Pause  //是否暂停
        {
            get { return _Pause; }
            set
            {
                if (_Pause == value) return;

                var oldValue = _Pause;
                _Pause = value;

                OnChanged(oldValue, value, "Pause");
            }
        }
        private bool _Pause;

        public string Name // BUFF名字 非必填
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
        private string _Name = "";

        protected override internal void OnDeleted()   //删除时
        {
            Account = null;
            Character = null;

            base.OnDeleted();
        }

        //更改时
        protected override void OnChanged(object oldValue, object newValue, string propertyName)
        {
            base.OnChanged(oldValue, newValue, propertyName);

            switch (propertyName)
            {
                case "Character":
                    if (newValue == null) break;

                    Account = null;
                    break;
                case "Account":
                    if (newValue == null) break;

                    Character = null;
                    break;
            }
        }

#if !ServerTool
        public ClientBuffInfo ToClientInfo()
        {
            return new ClientBuffInfo   //用户BUFF信息
            {
                Index = Index,
                RemainingTime = RemainingTime,
                TickFrequency = TickFrequency,
                Type = Type,
                Pause = Pause,
                Stats = Stats,
                ItemIndex = ItemIndex,
                FromCustomBuff = FromCustomBuff
            };
        }
#endif
    }
}
