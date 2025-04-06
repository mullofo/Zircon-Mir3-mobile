using Library;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public class AutoPotionLink : DBObject    //自动喝药
    {
        [Association("AutoPotionLinks")]
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

        public int Slot   //对应格子
        {
            get { return _Slot; }
            set
            {
                if (_Slot == value) return;

                var oldValue = _Slot;
                _Slot = value;

                OnChanged(oldValue, value, "Slot");
            }
        }
        private int _Slot;

        public int LinkInfoIndex   //道具ID
        {
            get { return _LinkInfoIndex; }
            set
            {
                if (_LinkInfoIndex == value) return;

                var oldValue = _LinkInfoIndex;
                _LinkInfoIndex = value;

                OnChanged(oldValue, value, "LinkInfoIndex");
            }
        }
        private int _LinkInfoIndex;

        public int Health   //加血
        {
            get { return _Health; }
            set
            {
                if (_Health == value) return;

                var oldValue = _Health;
                _Health = value;

                OnChanged(oldValue, value, "Health");
            }
        }
        private int _Health;

        public int Mana   //加蓝
        {
            get { return _Mana; }
            set
            {
                if (_Mana == value) return;

                var oldValue = _Mana;
                _Mana = value;

                OnChanged(oldValue, value, "Mana");
            }
        }
        private int _Mana;

        public bool Enabled   //启用开关
        {
            get { return _Enabled; }
            set
            {
                if (_Enabled == value) return;

                var oldValue = _Enabled;
                _Enabled = value;

                OnChanged(oldValue, value, "Enabled");
            }
        }
        private bool _Enabled;


        protected override internal void OnDeleted()   //删除时
        {
            Character = null;

            base.OnDeleted();
        }


        public ClientAutoPotionLink ToClientInfo()
        {
            return new ClientAutoPotionLink    //用户自动喝药列
            {
                Slot = Slot,
                LinkInfoIndex = LinkInfoIndex,
                Health = Health,
                Mana = Mana,
                Enabled = Enabled,
            };
        }
    }

    [UserObject]
    public class AutoFightConfig : DBObject    //自动战斗
    {
        [Association("AutoFightLinks")]
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

        public AutoSetConf Slot   //自动设置配置
        {
            get { return _Slot; }
            set
            {
                if (_Slot == value) return;

                var oldValue = _Slot;
                _Slot = value;

                OnChanged(oldValue, value, "Slot");
            }
        }
        private AutoSetConf _Slot;

        public MagicType MagicIndex   //魔法技能ID
        {
            get { return _MagicIndex; }
            set
            {
                if (_MagicIndex == value) return;

                var oldValue = _MagicIndex;
                _MagicIndex = value;

                OnChanged(oldValue, value, "MagicIndex");
            }
        }
        private MagicType _MagicIndex;

        public int TimeCount   //时间计数
        {
            get { return _TimeCount; }
            set
            {
                if (_TimeCount == value) return;

                var oldValue = _TimeCount;
                _TimeCount = value;

                OnChanged(oldValue, value, "TimeCount");
            }
        }
        private int _TimeCount;

        public bool Enabled   //启用开关
        {
            get { return _Enabled; }
            set
            {
                if (_Enabled == value) return;

                var oldValue = _Enabled;
                _Enabled = value;

                OnChanged(oldValue, value, "Enabled");
            }
        }
        private bool _Enabled;


        protected override internal void OnDeleted()   //删除时
        {
            Character = null;

            base.OnDeleted();
        }

#if !ServerTool
        public ClientAutoFightLink ToClientInfo()
        {
            return new ClientAutoFightLink   //用户自动战斗列表
            {
                Slot = Slot,
                MagicIndex = MagicIndex,
                TimeCount = TimeCount,
                Enabled = Enabled,
            };
        }
#endif
    }

}
