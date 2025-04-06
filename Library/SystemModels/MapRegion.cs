using MirDB;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Library.SystemModels
{
    /// <summary>
    /// 地图区域设置信息
    /// </summary>
    public sealed class MapRegion : DBObject
    {
        /// <summary>
        /// 地图信息
        /// </summary>
        [Association("Regions")]
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
        /// 区域设置说明
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description == value) return;

                var oldValue = _Description;
                _Description = value;

                OnChanged(oldValue, value, "Description");
            }
        }
        private string _Description;
        /// <summary>
        /// 点区域
        /// </summary>
        public BitArray BitRegion
        {
            get { return _BitRegion; }
            set
            {
                if (_BitRegion == value) return;

                var oldValue = _BitRegion;
                _BitRegion = value;

                OnChanged(oldValue, value, "BitRegion");
            }
        }
        private BitArray _BitRegion;
        /// <summary>
        /// 坐标区域
        /// </summary>
        public Point[] PointRegion
        {
            get { return _PointRegion; }
            set
            {
                if (_PointRegion == value) return;

                var oldValue = _PointRegion;
                _PointRegion = value;

                OnChanged(oldValue, value, "PointRegion");
            }
        }
        private Point[] _PointRegion;

        [IgnoreProperty]
        public string ServerDescription => $"{Map?.Description} - {Description}";

        /// <summary>
        /// 设置的范围大小
        /// </summary>
        public int Size
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
        private int _Size;

        /// <summary>
        /// 点列表
        /// </summary>
        public List<Point> PointList;


        public HashSet<Point> GetPoints(int width)
        {
            HashSet<Point> points = new HashSet<Point>();

            if (BitRegion != null)
            {
                for (int i = 0; i < BitRegion.Length; i++)
                {
                    if (BitRegion[i])
                        points.Add(new Point(i % width, i / width));
                }
            }
            else if (PointRegion != null)
            {
                foreach (Point p in PointRegion)
                    points.Add(p);
            }

            return points;
        }
        /// <summary>
        /// 创建点
        /// </summary>
        /// <param name="width">宽</param>
        public void CreatePoints(int width)
        {
            PointList = new List<Point>();

            if (BitRegion != null)
            {
                if (width == 0) return;

                for (int i = 0; i < BitRegion.Length; i++)
                {
                    if (BitRegion[i])
                        PointList.Add(new Point(i % width, i / width));
                }
            }
            else if (PointRegion != null)
            {
                foreach (Point p in PointRegion)
                    PointList.Add(p);
            }

        }
    }
}
