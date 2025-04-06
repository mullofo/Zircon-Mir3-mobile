using MirDB;

namespace Library.SystemModels
{
    /// <summary>
    /// 怪物刷新信息
    /// </summary>
    public sealed class RespawnInfo : DBObject
    {
        /// <summary>
        /// 怪物信息名字
        /// </summary>
        [Association("Respawns")]
        public MonsterInfo Monster
        {
            get { return _Monster; }
            set
            {
                if (_Monster == value) return;

                var oldValue = _Monster;
                _Monster = value;

                OnChanged(oldValue, value, "Monster");
            }
        }
        private MonsterInfo _Monster;
        /// <summary>
        /// 怪物刷新区域
        /// </summary>
        public MapRegion Region
        {
            get { return _Region; }
            set
            {
                if (_Region == value) return;

                var oldValue = _Region;
                _Region = value;

                OnChanged(oldValue, value, "Region");
            }
        }
        private MapRegion _Region;
        /// <summary>
        /// 地图ID
        /// </summary>
        public int MapID
        {
            get { return _MapId; }
            set
            {
                if (_MapId == value) return;

                var oldValue = _MapId;
                _MapId = value;

                OnChanged(oldValue, value, "MapId");
            }
        }
        private int _MapId;
        /// <summary>
        /// 地图X坐标
        /// </summary>
        public int MapX
        {
            get { return _MapX; }
            set
            {
                if (_MapX == value) return;

                var oldValue = _MapX;
                _MapX = value;

                OnChanged(oldValue, value, "MapX");
            }
        }
        private int _MapX;
        /// <summary>
        /// 地图Y坐标
        /// </summary>
        public int MapY
        {
            get { return _MapY; }
            set
            {
                if (_MapY == value) return;

                var oldValue = _MapY;
                _MapY = value;

                OnChanged(oldValue, value, "MapY");
            }
        }
        private int _MapY;
        /// <summary>
        /// 刷新范围
        /// </summary>
        public int Range
        {
            get { return _Range; }
            set
            {
                if (_Range == value) return;

                var oldValue = _Range;
                _Range = value;

                OnChanged(oldValue, value, "Range");
            }
        }
        private int _Range;
        /// <summary>
        /// 是否对应事件玩法设置
        /// </summary>
        public bool EventSpawn
        {
            get { return _EventSpawn; }
            set
            {
                if (_EventSpawn == value) return;

                var oldValue = _EventSpawn;
                _EventSpawn = value;

                OnChanged(oldValue, value, "EventSpawn");
            }
        }
        private bool _EventSpawn;
        /// <summary>
        /// 怪物刷新时间
        /// </summary>
        public int Delay
        {
            get { return _Delay; }
            set
            {
                if (_Delay == value) return;

                var oldValue = _Delay;
                _Delay = value;

                OnChanged(oldValue, value, "Delay");
            }
        }
        private int _Delay;
        /// <summary>
        /// 怪物刷新数量
        /// </summary>
        public int Count
        {
            get { return _Count; }
            set
            {
                if (_Count == value) return;

                var oldValue = _Count;
                _Count = value;

                OnChanged(oldValue, value, "Count");
            }
        }
        private int _Count;
        /// <summary>
        /// 怪物随机刷新时间
        /// </summary>
        public int RandomTime
        {
            get { return _RandomTime; }
            set
            {
                if (_RandomTime == value) return;

                var oldValue = _RandomTime;
                _RandomTime = value;

                OnChanged(oldValue, value, "RandomTime");
            }
        }
        private int _RandomTime;
        /// <summary>
        /// 对应爆率控制
        /// </summary>
        public int DropSet
        {
            get { return _DropSet; }
            set
            {
                if (_DropSet == value) return;

                var oldValue = _DropSet;
                _DropSet = value;

                OnChanged(oldValue, value, "DropSet");
            }
        }
        private int _DropSet;
        /// <summary>
        /// 怪物刷新提示
        /// </summary>
        public bool Announce
        {
            get { return _Announce; }
            set
            {
                if (_Announce == value) return;

                var oldValue = _Announce;
                _Announce = value;

                OnChanged(oldValue, value, "Announce");
            }
        }
        private bool _Announce;
        /// <summary>
        /// 怪物准时刷新
        /// </summary>
        public bool Punctual
        {
            get { return _Punctual; }
            set
            {
                if (_Punctual == value) return;

                var oldValue = _Punctual;
                _Punctual = value;

                OnChanged(oldValue, value, "Punctual");
            }
        }
        private bool _Punctual;
        /// <summary>
        /// 活动参数几率设置
        /// </summary>
        public int EasterEventChance
        {
            get { return _EasterEventChance; }
            set
            {
                if (_EasterEventChance == value) return;

                var oldValue = _EasterEventChance;
                _EasterEventChance = value;

                OnChanged(oldValue, value, "EasterEventChance");
            }
        }
        private int _EasterEventChance;
        /// <summary>
        /// 刷怪区域注释名字显示
        /// </summary>
        [IgnoreProperty]
        public string RegionName => Region?.ServerDescription ?? string.Empty;

        /// <summary>
        /// 怪物名字显示
        /// </summary>
        [IgnoreProperty]
        public string MonsterName => Monster?.MonsterName ?? string.Empty;
        /// <summary>
        /// 映射地图文件名
        /// </summary>
        public string StrMapFileName { get; set; }
        /// <summary>
        /// 映射地图区域文件名
        /// </summary>
        public string StrMapRegionDescition { get; set; }

    }
}
