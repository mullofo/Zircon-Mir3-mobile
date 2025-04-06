using MirDB;
using System.Drawing;

namespace Client.UserModels
{
    [UserObject]
    /// <summary>
    /// 窗口设置
    /// </summary>
    public class WindowSetting : DBObject
    {
        /// <summary>
        /// 窗口
        /// </summary>
        public WindowType Window
        {
            get { return _Window; }
            set
            {
                if (_Window == value) return;

                var oldValue = _Window;
                _Window = value;

                OnChanged(oldValue, value, "Window");
            }
        }
        private WindowType _Window;
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
        /// 可见
        /// </summary>
        public bool Visible
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
        /// 额外
        /// </summary>
        public int Extra
        {
            get { return _Extra; }
            set
            {
                if (_Extra == value) return;

                var oldValue = _Extra;
                _Extra = value;

                OnChanged(oldValue, value, "Extra");
            }
        }
        private int _Extra;
        /// <summary>
        /// 额外2
        /// </summary>
        public int Extra2
        {
            get { return _Extra2; }
            set
            {
                if (_Extra2 == value) return;

                var oldValue = _Extra2;
                _Extra2 = value;

                OnChanged(oldValue, value, "Extra2");
            }
        }
        private int _Extra2;
    }

    /// <summary>
    /// 窗口类型
    /// </summary>
    public enum WindowType
    {
        /// <summary>
        /// 空置
        /// </summary>
        None,
        /// <summary>
        /// 设置界面
        /// </summary>
        ConfigBox,
        /// <summary>
        /// 背包界面
        /// </summary>
        InventoryBox,
        /// <summary>
        /// 角色属性界面
        /// </summary>
        CharacterBox,
        /// <summary>
        /// 离开窗口
        /// </summary>
        ExitBox,
        /// <summary>
        /// 聊天信息框
        /// </summary>
        ChatTextBox,
        /// <summary>
        /// 聊天框界面
        /// </summary>
        ChatBox,
        /// <summary>
        /// 药品快捷栏
        /// </summary>
        BeltBox,
        /// <summary>
        /// 聊天框设置界面
        /// </summary>
        ChatOptionsBox,
        /// <summary>
        /// 小地图界面
        /// </summary>
        MiniMapBox,
        /// <summary>
        /// 组队界面
        /// </summary>
        GroupBox,
        GroupMembersBox,
        /// <summary>
        /// BUFF界面
        /// </summary>
        BuffBox,
        /// <summary>
        /// 仓库界面
        /// </summary>
        StorageBox,
        /// <summary>
        /// 自动喝药界面
        /// </summary>
        AutoPotionBox,
        /// <summary>
        /// 查看玩家界面
        /// </summary>
        InspectBox,
        /// <summary>
        /// 排行版界面
        /// </summary>
        RankingBox,
        /// <summary>
        /// 商城界面
        /// </summary>
        MarketPlaceBox,
        /// <summary>
        /// 邮件窗口
        /// </summary>
        MailBox,
        /// <summary>
        /// 读邮件窗口
        /// </summary>
        ReadMailBox,
        /// <summary>
        /// 新邮件窗口
        /// </summary>
        SendMailBox,
        /// <summary>
        /// 交易窗口
        /// </summary>
        TradeBox,
        /// <summary>
        /// 行会面板
        /// </summary>
        GuildBox,
        /// <summary>
        /// 行会成员面板
        /// </summary>
        GuildMemberBox,
        /// <summary>
        /// 任务面板
        /// </summary>
        QuestBox,
        /// <summary>
        /// 任务跟踪面板
        /// </summary>
        QuestTrackerBox,
        /// <summary>
        /// 宠物面板
        /// </summary>
        CompanionBox,
        /// <summary>
        /// 黑名单
        /// </summary>
        BlockBox,
        /// <summary>
        /// 怪物面板
        /// </summary>
        MonsterBox,
        /// <summary>
        /// 技能面板
        /// </summary>
        MagicBox,
        /// <summary>
        /// 技能快捷栏
        /// </summary>
        MagicBarBox,
        /// <summary>
        /// 信息输入框
        /// </summary>
        MessageBox,
        /// <summary>
        /// 道具交易框
        /// </summary>
        ItemAmountBox,
        /// <summary>
        /// 输入框
        /// </summary>
        InputWindow,
        /// <summary>
        /// 大补贴
        /// </summary>
        BigPatchWindow,
        /// <summary>
        /// 制作
        /// </summary>
        CraftBox,
        /// <summary>
        /// 成就
        /// </summary>
        AchievementBox,
        /// <summary>
        /// 定位传送
        /// </summary>
		FixedPointBox,
        /// <summary>
        /// 泰山许愿池
        /// </summary>
        VowBox,
    }
}
