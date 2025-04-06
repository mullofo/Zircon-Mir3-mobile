using MirDB;
using System.Collections.Generic;
using System.Drawing;

namespace Library.SystemModels
{
    /// <summary>
    /// 钓鱼区信息
    /// </summary>
    public sealed class FishingAreaInfo : DBObject
    {
        /// <summary>
        /// 地图区域
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
        /// 绑定钓鱼区
        /// </summary>
        public MapRegion BindRegion
        {
            get { return _BindRegion; }
            set
            {
                if (_BindRegion == value) return;

                var oldValue = _BindRegion;
                _BindRegion = value;

                OnChanged(oldValue, value, "BindRegion");
            }
        }
        private MapRegion _BindRegion;
        /// <summary>
        /// 有效的绑定地点
        /// </summary>
        public List<Point> ValidBindPoints = new List<Point>();
    }
}