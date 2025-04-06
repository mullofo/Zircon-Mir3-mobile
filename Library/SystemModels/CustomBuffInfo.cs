using MirDB;
using System;
using static MirDB.Association;

namespace Library.SystemModels
{
    [Lang]
    public class CustomBuffInfo : DBObject
    {
        /// <summary>
        /// BUFF类型
        /// </summary>
        public BuffType BuffType
        {
            get { return _BuffType; }
            set
            {
                if (_BuffType == value) return;

                var oldValue = _BuffType;
                _BuffType = value;

                OnChanged(oldValue, value, "BuffType");
            }
        }
        private BuffType _BuffType;
        /// <summary>
        /// BUFF等级
        /// </summary>
        public int BuffLV
        {
            get { return _BuffLV; }
            set
            {
                if (_BuffLV == value) return;

                var oldValue = _BuffLV;
                _BuffLV = value;

                OnChanged(oldValue, value, "BuffLV");
            }
        }
        private int _BuffLV;
        /// <summary>
        /// BUFF名称
        /// </summary>
        [Lang("BUFF名称")]
        public string BuffName
        {
            get { return _BuffName; }
            set
            {
                if (_BuffName == value) return;

                var oldValue = _BuffName;
                _BuffName = value;

                OnChanged(oldValue, value, "BuffName");
            }
        }
        private string _BuffName;
        /// <summary>
        /// BUFF分组
        /// </summary>
        public string BuffGroup
        {
            get { return _BuffGroup; }
            set
            {
                if (_BuffGroup == value) return;

                var oldValue = _BuffGroup;
                _BuffGroup = value;

                OnChanged(oldValue, value, "BuffGroup");
            }
        }
        private string _BuffGroup;
        /// <summary>
        /// 小图标
        /// </summary>
        public int SmallBuffIcon
        {
            get { return _smallBuffIcon; }
            set
            {
                if (_smallBuffIcon == value) return;

                var oldValue = _smallBuffIcon;
                _smallBuffIcon = value;

                OnChanged(oldValue, value, "SmallBuffIcon");
            }
        }
        private int _smallBuffIcon;
        /// <summary>
        /// 大图标
        /// </summary>
        public int BigBuffIcon
        {
            get { return _bigBuffIcon; }
            set
            {
                if (_bigBuffIcon == value) return;

                var oldValue = _bigBuffIcon;
                _bigBuffIcon = value;

                OnChanged(oldValue, value, "BigBuffIcon");
            }
        }
        private int _bigBuffIcon;
        /// <summary>
        /// 获取几率1
        /// </summary>
        public int GetRate
        {
            get { return _GetRate; }
            set
            {
                if (_GetRate == value) return;

                var oldValue = _GetRate;
                _GetRate = value;

                OnChanged(oldValue, value, "GetRate");
            }
        }
        private int _GetRate;
        /// <summary>
        /// 获取几率2
        /// </summary>
        public int GetRate2
        {
            get { return _GetRate2; }
            set
            {
                if (_GetRate2 == value) return;

                var oldValue = _GetRate2;
                _GetRate2 = value;

                OnChanged(oldValue, value, "GetRate2");
            }
        }
        private int _GetRate2;
        /// <summary>
        /// 持续使用时间
        /// </summary>
        public TimeSpan Duration
        {
            get { return _Duration; }
            set
            {
                if (_Duration == value) return;

                var oldValue = _Duration;
                _Duration = value;

                OnChanged(oldValue, value, "Duration");
            }
        }
        private TimeSpan _Duration;
        /// <summary>
        /// 安全区内是否暂停
        /// </summary>
        public bool PauseInSafeZone
        {
            get { return _PauseInSafeZone; }
            set
            {
                if (_PauseInSafeZone == value) return;

                var oldValue = _PauseInSafeZone;
                _PauseInSafeZone = value;

                OnChanged(oldValue, value, "PauseInSafeZone");
            }
        }
        private bool _PauseInSafeZone;
        /// <summary>
        /// 离线是否计时
        /// </summary>
        public bool OfflineTicking
        {
            get { return _OfflineTicking; }
            set
            {
                if (_OfflineTicking == value) return;

                var oldValue = _OfflineTicking;
                _OfflineTicking = value;

                OnChanged(oldValue, value, "OfflineTicking");
            }
        }
        private bool _OfflineTicking;
        /// <summary>
        /// 勾选0点删除BUFF
        /// </summary>
        public bool DeleteAtMidnight
        {
            get { return _deleteAtMidnight; }
            set
            {
                if (_deleteAtMidnight == value) return;

                var oldValue = _deleteAtMidnight;
                _deleteAtMidnight = value;

                OnChanged(oldValue, value, "DeleteAtMidnight");
            }
        }
        private bool _deleteAtMidnight;
        /// <summary>
        /// 头顶动态称号
        /// </summary>
        public int OverheadTitle
        {
            get { return _OverheadTitle; }
            set
            {
                if (_OverheadTitle == value) return;

                var oldValue = _OverheadTitle;
                _OverheadTitle = value;

                OnChanged(oldValue, value, "OverheadTitle");
            }
        }
        private int _OverheadTitle;
        /// <summary>
        /// 动画图片帧数
        /// </summary>
        public int frameCount
        {
            get { return _frameCount; }
            set
            {
                if (_frameCount == value) return;

                var oldValue = _frameCount;
                _frameCount = value;

                OnChanged(oldValue, value, "frameCount");
            }
        }
        private int _frameCount;
        /// <summary>
        /// 动画图片偏移X
        /// </summary>
        public int offSetX
        {
            get { return _offSetX; }
            set
            {
                if (_offSetX == value) return;

                var oldValue = _offSetX;
                _offSetX = value;

                OnChanged(oldValue, value, "offSetX");
            }
        }
        private int _offSetX;
        /// <summary>
        /// 动画图片偏移Y
        /// </summary>
        public int offSetY
        {
            get { return _offSetY; }
            set
            {
                if (_offSetY == value) return;

                var oldValue = _offSetY;
                _offSetY = value;

                OnChanged(oldValue, value, "offSetY");
            }
        }
        private int _offSetY;

        [Association("BuffStats")]
        public DBBindingList<CustomBuffInfoStat> BuffStats { get; set; }

    }
}
