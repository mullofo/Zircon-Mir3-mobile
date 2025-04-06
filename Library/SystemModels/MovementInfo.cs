using MirDB;

namespace Library.SystemModels
{
    /// <summary>
    /// 地图门点坐标链接信息
    /// </summary>
    public sealed class MovementInfo : DBObject
    {
        /// <summary>
        /// 地图来源区域
        /// </summary>
        public MapRegion SourceRegion
        {
            get { return _SourceRegion; }
            set
            {
                if (_SourceRegion == value) return;

                var oldValue = _SourceRegion;
                _SourceRegion = value;

                OnChanged(oldValue, value, "SourceRegion");
            }
        }
        private MapRegion _SourceRegion;
        /// <summary>
        /// 地图目标区域
        /// </summary>
        public MapRegion DestinationRegion
        {
            get { return _DestinationRegion; }
            set
            {
                if (_DestinationRegion == value) return;

                var oldValue = _DestinationRegion;
                _DestinationRegion = value;

                OnChanged(oldValue, value, "DestinationRegion");
            }
        }
        private MapRegion _DestinationRegion;
        /// <summary>
        /// 地图图标
        /// </summary>
        public MapIcon Icon
        {
            get { return _Icon; }
            set
            {
                if (_Icon == value) return;

                var oldValue = _Icon;
                _Icon = value;

                OnChanged(oldValue, value, "Icon");
            }
        }
        private MapIcon _Icon;
        /// <summary>
        /// 需要道具才能进入该地图
        /// </summary>
        public ItemInfo NeedItem
        {
            get { return _NeedItem; }
            set
            {
                if (_NeedItem == value) return;

                var oldValue = _NeedItem;
                _NeedItem = value;

                OnChanged(oldValue, value, "NeedItem");
            }
        }
        private ItemInfo _NeedItem;
        /// <summary>
        /// 需要刷新指定怪物才能进入该地图
        /// </summary>
        public RespawnInfo NeedSpawn
        {
            get { return _NeedSpawn; }
            set
            {
                if (_NeedSpawn == value) return;

                var oldValue = _NeedSpawn;
                _NeedSpawn = value;

                OnChanged(oldValue, value, "NeedSpawn");
            }
        }
        private RespawnInfo _NeedSpawn;
        /// <summary>
        /// 地图链接点对于效果
        /// </summary>
        public MovementEffect Effect
        {
            get { return _Effect; }
            set
            {
                if (_Effect == value) return;

                var oldValue = _Effect;
                _Effect = value;

                OnChanged(oldValue, value, "Effect");
            }
        }
        private MovementEffect _Effect;
        /// <summary>
        /// 进入地图的职业限制
        /// </summary>
        public RequiredClass RequiredClass
        {
            get { return _RequiredClass; }
            set
            {
                if (_RequiredClass == value) return;

                var oldValue = _RequiredClass;
                _RequiredClass = value;

                OnChanged(oldValue, value, "RequiredClass");
            }
        }
        private RequiredClass _RequiredClass;

        /// <summary>
        /// 存储额外信息
        /// </summary>
        public string ExtraInfo
        {
            get { return _ExtraInfo; }
            set
            {
                if (_ExtraInfo == value) return;

                var oldValue = _ExtraInfo;
                _ExtraInfo = value;

                OnChanged(oldValue, value, "ExtraInfo");
            }
        }
        private string _ExtraInfo;
        /// <summary>
        /// 显示门点提示
        /// </summary>
        public bool CanLinkTips
        {
            get { return _CanLinkTips; }
            set
            {
                if (_CanLinkTips == value) return;

                var oldValue = _CanLinkTips;
                _CanLinkTips = value;

                OnChanged(oldValue, value, "CanLinkTips");
            }
        }
        private bool _CanLinkTips;
        /// <summary>
        /// 门点关闭延迟时间
        /// </summary>
        public int SpanTime
        {
            get { return _SpanTime; }
            set
            {
                if (_SpanTime == value) return;

                var oldValue = _SpanTime;
                _SpanTime = value;

                OnChanged(oldValue, value, "SpanTime");
            }
        }
        private int _SpanTime;

        /// <summary>
        /// 创建时
        /// </summary>
        protected override internal void OnCreated()
        {
            base.OnCreated();

            RequiredClass = RequiredClass.All;
        }
    }
}
