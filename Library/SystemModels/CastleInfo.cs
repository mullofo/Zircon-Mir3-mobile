using MirDB;
using System;
using System.Drawing;
using static MirDB.Association;

namespace Library.SystemModels
{
    /// <summary>
    /// 城堡信息
    /// </summary>
    [Lang]
    public sealed class CastleInfo : DBObject
    {
        /// <summary>
        /// 城堡名字
        /// </summary>
        [Lang("城堡名字")]
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value) return;

                var oldValue = _Name;
                _Name = value;

                OnChanged(oldValue, value, "Name");
            }
        }
        private string _Name;
        /// <summary>
        /// 城堡地图信息
        /// </summary>
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
        /// 攻城开始时间
        /// </summary>
        public TimeSpan StartTime
        {
            get { return _StartTime; }
            set
            {
                if (_StartTime == value) return;

                var oldValue = _StartTime;
                _StartTime = value;

                OnChanged(oldValue, value, "StartTime");
            }
        }
        private TimeSpan _StartTime;
        /// <summary>
        /// 攻城结束时间
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
        /// 行会旗帜坐标
        /// </summary>
        public Point FlagPoint
        {
            get { return _FlagPoint; }
            set
            {
                if (_FlagPoint == value) return;

                var oldValue = _FlagPoint;
                _FlagPoint = value;

                OnChanged(oldValue, value, "FlagPoint");
            }
        }
        private Point _FlagPoint;
        /// <summary>
        /// 攻城区域
        /// </summary>
        public MapRegion CastleRegion
        {
            get { return _CastleRegion; }
            set
            {
                if (_CastleRegion == value) return;

                var oldValue = _CastleRegion;
                _CastleRegion = value;

                OnChanged(oldValue, value, "CastleRegion");
            }
        }
        private MapRegion _CastleRegion;
        /// <summary>
        /// 攻城方复活点
        /// </summary>
        public MapRegion AttackSpawnRegion
        {
            get { return _AttackSpawnRegion; }
            set
            {
                if (_AttackSpawnRegion == value) return;

                var oldValue = _AttackSpawnRegion;
                _AttackSpawnRegion = value;

                OnChanged(oldValue, value, "AttackSpawnRegion");
            }
        }
        private MapRegion _AttackSpawnRegion;
        /// <summary>
        /// 攻城方联盟复活点
        /// </summary>
        public MapRegion AttackSpawnUnionRegion
        {
            get { return _AttackSpawnUnionRegion; }
            set
            {
                if (_AttackSpawnUnionRegion == value) return;

                var oldValue = _AttackSpawnUnionRegion;
                _AttackSpawnUnionRegion = value;

                OnChanged(oldValue, value, "AttackSpawnUnionRegion");
            }
        }
        private MapRegion _AttackSpawnUnionRegion;
        /// <summary>
        /// 守城方复活点
        /// </summary>
        public MapRegion DefenderSpawnRegion
        {
            get { return _DefenderSpawnRegion; }
            set
            {
                if (_DefenderSpawnRegion == value) return;

                var oldValue = _DefenderSpawnRegion;
                _DefenderSpawnRegion = value;

                OnChanged(oldValue, value, "DefenderSpawnRegion");
            }
        }
        private MapRegion _DefenderSpawnRegion;
        /// <summary>
        /// 守城方联盟复活点
        /// </summary>
        public MapRegion DefenderSpawnUnionRegion
        {
            get { return _DefenderSpawnUnionRegion; }
            set
            {
                if (_DefenderSpawnUnionRegion == value) return;

                var oldValue = _DefenderSpawnUnionRegion;
                _DefenderSpawnUnionRegion = value;

                OnChanged(oldValue, value, "DefenderSpawnUnionRegion");
            }
        }
        private MapRegion _DefenderSpawnUnionRegion;
        /// <summary>
        /// 申请攻城的道具
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
        /// 攻城玩法类型
        /// </summary>
        public ConquestType Type
        {
            get { return _Type; }
            set
            {
                if (_Type == value) return;

                var oldValue = _Type;
                _Type = value;

                OnChanged(oldValue, value, "Type");
            }
        }
        private ConquestType _Type;
        /// <summary>
        /// 关联类型ID
        /// </summary>
        public int InfoID
        {
            get { return _InfoID; }
            set
            {
                if (_InfoID == value) return;

                var oldValue = _InfoID;
                _InfoID = value;

                OnChanged(oldValue, value, "InfoID");
            }
        }
        private int _InfoID;
        /// <summary>
        /// 城堡税率
        /// </summary>
        public decimal TaxRate
        {
            get { return _TaxRate; }
            set
            {
                if (_TaxRate == value) return;

                var oldValue = _TaxRate;
                _TaxRate = value;

                OnChanged(oldValue, value, "TaxRate");
            }
        }
        private decimal _TaxRate;
        /// <summary>
        /// 占领方购物折扣
        /// </summary>
        public decimal Discount
        {
            get { return _Discount; }
            set
            {
                if (_Discount == value) return;

                var oldValue = _Discount;
                _Discount = value;

                OnChanged(oldValue, value, "Discount");
            }
        }
        private decimal _Discount;

        /// <summary>
        /// 攻城日期
        /// </summary>
        public DateTime WarDate;
    }
}



