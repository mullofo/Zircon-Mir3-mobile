using MirDB;

namespace Library.SystemModels
{
    /// <summary>
    /// 地图挖矿信息
    /// </summary>
    public sealed class MineInfo : DBObject
    {
        /// <summary>
        /// 地图信息
        /// </summary>
        [Association("Mining")]
        public MapInfo Map
        {
            get { return _Map; }
            set
            {
                if (_Map == value) return;

                var oldValue = _Map;
                _Map = value;

                OnChanged(oldValue, value, "Map");
            }
        }
        private MapInfo _Map;
        /// <summary>
        /// 道具信息
        /// </summary>
        public ItemInfo Item
        {
            get { return _Item; }
            set
            {
                if (_Item == value) return;

                var oldValue = _Item;
                _Item = value;

                OnChanged(oldValue, value, "Item");
            }
        }
        private ItemInfo _Item;
        /// <summary>
        /// 成功几率
        /// </summary>
        public int Chance
        {
            get { return _Chance; }
            set
            {
                if (_Chance == value) return;

                var oldValue = _Chance;
                _Chance = value;

                OnChanged(oldValue, value, "Chance");
            }
        }
        private int _Chance;

        /// <summary>
        /// 输出道具名字
        /// </summary>
        public string StrItemName { get; set; }//杨伟
    }
}
