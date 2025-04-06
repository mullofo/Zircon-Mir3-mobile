using MirDB;
using System.Collections.Generic;
using System.Drawing;

namespace Library.SystemModels
{
    /// <summary>
    /// 安全区信息
    /// </summary>
    public sealed class SafeZoneInfo : DBObject
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
        /// 绑定安全区
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
        /// 锁定的回城坐标X
        /// </summary>
        public int StartPointX
        {
            get { return _StartPointX; }
            set
            {
                if (_StartPointX == value) return;

                var oldValue = _StartPointX;
                _StartPointX = value;

                OnChanged(oldValue, value, "StartPointX");
            }
        }
        private int _StartPointX;
        /// <summary>
        /// 锁定的回城坐标Y
        /// </summary>
        public int StartPointY
        {
            get { return _StartPointY; }
            set
            {
                if (_StartPointY == value) return;

                var oldValue = _StartPointY;
                _StartPointY = value;

                OnChanged(oldValue, value, "StartPointY");
            }
        }
        private int _StartPointY;
        /// <summary>
        /// 开启游戏时对应职业在对应区域
        /// </summary>
        public RequiredClass StartClass
        {
            get { return _StartClass; }
            set
            {
                if (_StartClass == value) return;

                var oldValue = _StartClass;
                _StartClass = value;

                OnChanged(oldValue, value, "StartClass");
            }
        }
        private RequiredClass _StartClass;
        /// <summary>
        /// 红名区
        /// </summary>
        public bool RedZone
        {
            get { return _RedZone; }
            set
            {
                if (_RedZone == value) return;

                var oldValue = _RedZone;
                _RedZone = value;

                OnChanged(oldValue, value, "RedZone");
            }
        }
        private bool _RedZone;
        /// <summary>
        /// 有效的绑定地点
        /// </summary>
        public List<Point> ValidBindPoints = new List<Point>();
    }
}
