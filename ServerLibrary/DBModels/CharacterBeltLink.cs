using Library;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public sealed class CharacterBeltLink : DBObject  //角色药水快捷栏
    {
        [Association("BeltLinks")]
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

        public int Slot   //药水格子
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

        public int LinkInfoIndex   //链接信息ID
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


        public int LinkItemIndex   //链接道具ID
        {
            get { return _LinkItemIndex; }
            set
            {
                if (_LinkItemIndex == value) return;

                var oldValue = _LinkItemIndex;
                _LinkItemIndex = value;

                OnChanged(oldValue, value, "LinkItemIndex");
            }
        }
        private int _LinkItemIndex;

        protected override internal void OnDeleted()   //删除时
        {
            Character = null;

            base.OnDeleted();
        }

#if !ServerTool
        public ClientBeltLink ToClientInfo()
        {
            return new ClientBeltLink   //用户药水栏链接
            {
                Slot = Slot,
                LinkInfoIndex = LinkInfoIndex,
                LinkItemIndex = LinkItemIndex
            };
        }
#endif
    }
}
