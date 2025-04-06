using MirDB;
using System.Drawing;

namespace Client.UserModels
{
    [UserObject]
    /// <summary>
    /// 聊天选项卡控件设置
    /// </summary>
    public sealed class ChatTabControlSetting : DBObject
    {
        /// <summary>
        /// 分辨率大小
        /// </summary>
        public Size Resolution
        {
            get { return _Resolution; }
            set
            {
                if (_Resolution == value) return;

                var oldValue = _Resolution;
                _Resolution = value;

                OnChanged(oldValue, value, "Resolution");
            }
        }
        private Size _Resolution;
        /// <summary>
        /// 位置
        /// </summary>
        public Point Location
        {
            get { return _Location; }
            set
            {
                if (_Location == value) return;

                var oldValue = _Location;
                _Location = value;

                OnChanged(oldValue, value, "Location");
            }
        }
        private Point _Location;
        /// <summary>
        /// 尺寸大小
        /// </summary>
        public Size Size
        {
            get { return _Size; }
            set
            {
                if (_Size == value) return;

                var oldValue = _Size;
                _Size = value;

                OnChanged(oldValue, value, "Size");
            }
        }
        private Size _Size;
        /// <summary>
        /// 聊天选项卡页面设置 所选页面
        /// </summary>
        public ChatTabPageSetting SelectedPage
        {
            get { return _SelectedPage; }
            set
            {
                if (_SelectedPage == value) return;

                var oldValue = _SelectedPage;
                _SelectedPage = value;

                OnChanged(oldValue, value, "SelectedPage");
            }
        }
        private ChatTabPageSetting _SelectedPage;


        [Association("Controls", true)]
        public DBBindingList<ChatTabPageSetting> Controls { get; set; }
    }


}
