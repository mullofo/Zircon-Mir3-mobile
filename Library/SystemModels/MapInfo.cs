using MirDB;
using System;
using static MirDB.Association;

namespace Library.SystemModels
{
    /// <summary>
    /// 地图信息
    /// </summary>
    [Lang]
    public sealed class MapInfo : DBObject, ICloneable
    {
        /// <summary>
        /// 地图指定序号名字
        /// </summary>
        public string FileName
        {
            get { return _FileName; }
            set
            {
                if (_FileName == value) return;

                var oldValue = _FileName;
                _FileName = value;

                OnChanged(oldValue, value, "FileName");
            }
        }
        private string _FileName;
        /// <summary>
        /// 地图名
        /// </summary>
        [Lang("地图名")]
        [Export]
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
        /// 小地图序号
        /// </summary>
        public int MiniMap
        {
            get { return _MiniMap; }
            set
            {
                if (_MiniMap == value) return;

                var oldValue = _MiniMap;
                _MiniMap = value;

                OnChanged(oldValue, value, "MiniMap");
            }
        }
        private int _MiniMap;
        /// <summary>
        /// 地图背景最底层
        /// </summary>
        public int Background
        {
            get { return _Background; }
            set
            {
                if (_Background == value) return;

                var oldValue = _Background;
                _Background = value;

                OnChanged(oldValue, value, "Background");
            }
        }
        private int _Background;
        /// <summary>
        /// 地图光效
        /// </summary>
        public LightSetting Light
        {
            get { return _Light; }
            set
            {
                if (_Light == value) return;

                var oldValue = _Light;
                _Light = value;

                OnChanged(oldValue, value, "Light");
            }
        }
        private LightSetting _Light;
        /// <summary>
        /// 地图天气
        /// </summary>
        public WeatherSetting Weather
        {
            get { return _Weather; }
            set
            {
                if (_Weather == value) return;

                var oldValue = _Weather;
                _Weather = value;

                OnChanged(oldValue, value, "Weather");
            }
        }
        private WeatherSetting _Weather;
        /// <summary>
        /// 地图战斗属性
        /// </summary>
        public FightSetting Fight
        {
            get { return _Fight; }
            set
            {
                if (_Fight == value) return;

                var oldValue = _Fight;
                _Fight = value;

                OnChanged(oldValue, value, "Fight");
            }
        }
        private FightSetting _Fight;
        /// <summary>
        /// 是否能传送
        /// </summary>
        public bool AllowRT
        {
            get { return _AllowRT; }
            set
            {
                if (_AllowRT == value) return;

                var oldValue = _AllowRT;
                _AllowRT = value;

                OnChanged(oldValue, value, "AllowRT");
            }
        }
        private bool _AllowRT;
        /// <summary>
        /// 是否限制传送符
        /// </summary>
        public bool AllowFPM
        {
            get { return _AllowFPM; }
            set
            {
                if (_AllowFPM == value) return;

                var oldValue = _AllowFPM;
                _AllowFPM = value;

                OnChanged(oldValue, value, "AllowFPM");
            }
        }
        private bool _AllowFPM;
        /// <summary>
        /// 是否限制技能
        /// </summary>
        public int SkillDelay
        {
            get { return _SkillDelay; }
            set
            {
                if (_SkillDelay == value) return;

                var oldValue = _SkillDelay;
                _SkillDelay = value;

                OnChanged(oldValue, value, "SkillDelay");
            }
        }
        private int _SkillDelay;
        /// <summary>
        /// 是否能骑马
        /// </summary>
        public bool CanHorse
        {
            get { return _CanHorse; }
            set
            {
                if (_CanHorse == value) return;

                var oldValue = _CanHorse;
                _CanHorse = value;

                OnChanged(oldValue, value, "CanHorse");
            }
        }
        private bool _CanHorse;
        /// <summary>
        /// 是否允许回城
        /// </summary>
        public bool AllowTT
        {
            get { return _AllowTT; }
            set
            {
                if (_AllowTT == value) return;

                var oldValue = _AllowTT;
                _AllowTT = value;

                OnChanged(oldValue, value, "AllowTT");
            }
        }
        private bool _AllowTT;
        /// <summary>
        /// 是否地图死亡爆装备
        /// </summary>
        public bool DeathDrop
        {
            get { return _DeathDrop; }
            set
            {
                if (_DeathDrop == value) return;

                var oldValue = _DeathDrop;
                _DeathDrop = value;

                OnChanged(oldValue, value, "DeathDrop");
            }
        }
        private bool _DeathDrop;
        /// <summary>
        /// 是否能挖矿
        /// </summary>
        public bool CanMine
        {
            get { return _CanMine; }
            set
            {
                if (_CanMine == value) return;

                var oldValue = _CanMine;
                _CanMine = value;

                OnChanged(oldValue, value, "CanMine");
            }
        }
        private bool _CanMine;
        /// <summary>
        /// 是否能结婚传送
        /// </summary>
        public bool CanMarriageRecall
        {
            get { return _CanMarriageRecall; }
            set
            {
                if (_CanMarriageRecall == value) return;

                var oldValue = _CanMarriageRecall;
                _CanMarriageRecall = value;

                OnChanged(oldValue, value, "CanMarriageRecall");
            }
        }
        private bool _CanMarriageRecall;
        /// <summary>
        /// 是否允许传唤(目前没启用)
        /// </summary>
        public bool AllowRecall
        {
            get => _AllowRecall;
            set
            {
                if (_AllowRecall == value) return;

                bool oldValue = _AllowRecall;
                _AllowRecall = value;

                OnChanged(oldValue, value, "AllowRecall");
            }
        }
        private bool _AllowRecall;
        /// <summary>
        /// 是否设置成副本地图
        /// </summary>
        public bool IsDynamic
        {
            get => _IsDynamic;
            set
            {
                if (_IsDynamic == value) return;

                bool oldValue = _IsDynamic;
                _IsDynamic = value;

                OnChanged(oldValue, value, "IsDynamic");
            }
        }
        private bool _IsDynamic;
        /// <summary>
        /// 最低进入级别
        /// </summary>
        public int MinimumLevel
        {
            get { return _MinimumLevel; }
            set
            {
                if (_MinimumLevel == value) return;

                var oldValue = _MinimumLevel;
                _MinimumLevel = value;

                OnChanged(oldValue, value, "MinimumLevel");
            }
        }
        private int _MinimumLevel;
        /// <summary>
        /// 最高进入级别
        /// </summary>
        public int MaximumLevel
        {
            get { return _MaximumLevel; }
            set
            {
                if (_MaximumLevel == value) return;

                var oldValue = _MaximumLevel;
                _MaximumLevel = value;

                OnChanged(oldValue, value, "MaximumLevel");
            }
        }
        private int _MaximumLevel;
        /// <summary>
        /// 下线在上所对应的地图
        /// </summary>
        public MapInfo ReconnectMap
        {
            get { return _ReconnectMap; }
            set
            {
                if (_ReconnectMap == value) return;

                var oldValue = _ReconnectMap;
                _ReconnectMap = value;

                OnChanged(oldValue, value, "ReconnectMap");
            }
        }
        private MapInfo _ReconnectMap;
        /// <summary>
        /// 地图播放对应的音乐
        /// </summary>
        public SoundIndex Music
        {
            get { return _Music; }
            set
            {
                if (_Music == value) return;

                var oldValue = _Music;
                _Music = value;

                OnChanged(oldValue, value, "Music");
            }
        }
        private SoundIndex _Music;
        /// <summary>
        /// 自定义地图音效
        /// </summary>
        public string MapSound
        {
            get { return _MapSound; }
            set
            {
                if (_MapSound == value) return;

                var oldValue = _MapSound;
                _MapSound = value;

                OnChanged(oldValue, value, "MapSound");
            }
        }
        private string _MapSound;
        /// <summary>
        /// 怪物血值
        /// </summary>
        public int MonsterHealth
        {
            get { return _MonsterHealth; }
            set
            {
                if (_MonsterHealth == value) return;

                var oldValue = _MonsterHealth;
                _MonsterHealth = value;

                OnChanged(oldValue, value, "MonsterHealth");
            }
        }
        private int _MonsterHealth;
        /// <summary>
        /// 怪物伤害值
        /// </summary>
        public int MonsterDamage
        {
            get { return _MonsterDamage; }
            set
            {
                if (_MonsterDamage == value) return;

                var oldValue = _MonsterDamage;
                _MonsterDamage = value;

                OnChanged(oldValue, value, "MonsterDamage");
            }
        }
        private int _MonsterDamage;

        /// <summary>
        /// 怪物防魔值
        /// </summary>
        public int MonsterBeware
        {
            get { return _MonsterBeware; }
            set
            {
                if (_MonsterBeware == value) return;

                var oldValue = _MonsterBeware;
                _MonsterBeware = value;

                OnChanged(oldValue, value, "MonsterBeware");
            }
        }
        private int _MonsterBeware;

        /// <summary>
        /// 怪物爆率值
        /// </summary>
        public int DropRate
        {
            get { return _DropRate; }
            set
            {
                if (_DropRate == value) return;

                var oldValue = _DropRate;
                _DropRate = value;

                OnChanged(oldValue, value, "DropRate");
            }
        }
        private int _DropRate;
        /// <summary>
        /// 怪物经验值
        /// </summary>
        public int ExperienceRate
        {
            get { return _ExperienceRate; }
            set
            {
                if (_ExperienceRate == value) return;

                var oldValue = _ExperienceRate;
                _ExperienceRate = value;

                OnChanged(oldValue, value, "ExperienceRate");
            }
        }
        private int _ExperienceRate;
        /// <summary>
        /// 怪物金币爆率值
        /// </summary>
        public int GoldRate
        {
            get { return _GoldRate; }
            set
            {
                if (_GoldRate == value) return;

                var oldValue = _GoldRate;
                _GoldRate = value;

                OnChanged(oldValue, value, "GoldRate");
            }
        }
        private int _GoldRate;
        /// <summary>
        /// 最大怪物血值
        /// </summary>
        public int MaxMonsterHealth
        {
            get { return _MaxMonsterHealth; }
            set
            {
                if (_MaxMonsterHealth == value) return;

                var oldValue = _MaxMonsterHealth;
                _MaxMonsterHealth = value;

                OnChanged(oldValue, value, "MaxMonsterHealth");
            }
        }
        private int _MaxMonsterHealth;
        /// <summary>
        /// 最大怪物伤害值
        /// </summary>
        public int MaxMonsterDamage
        {
            get { return _MaxMonsterDamage; }
            set
            {
                if (_MaxMonsterDamage == value) return;

                var oldValue = _MaxMonsterDamage;
                _MaxMonsterDamage = value;

                OnChanged(oldValue, value, "MaxMonsterDamage");
            }
        }
        private int _MaxMonsterDamage;

        /// <summary>
        /// 最大怪物防魔值
        /// </summary>
        public int MaxMonsterBeware
        {
            get { return _MaxMonsterBeware; }
            set
            {
                if (_MaxMonsterBeware == value) return;

                var oldValue = _MaxMonsterBeware;
                _MaxMonsterBeware = value;

                OnChanged(oldValue, value, "MaxMonsterBeware");
            }
        }
        private int _MaxMonsterBeware;

        /// <summary>
        /// 最大怪物爆率值
        /// </summary>
        public int MaxDropRate
        {
            get { return _MaxDropRate; }
            set
            {
                if (_MaxDropRate == value) return;

                var oldValue = _MaxDropRate;
                _MaxDropRate = value;

                OnChanged(oldValue, value, "MaxDropRate");
            }
        }
        private int _MaxDropRate;
        /// <summary>
        /// 最大怪物经验值
        /// </summary>
        public int MaxExperienceRate
        {
            get { return _MaxExperienceRate; }
            set
            {
                if (_MaxExperienceRate == value) return;

                var oldValue = _MaxExperienceRate;
                _MaxExperienceRate = value;

                OnChanged(oldValue, value, "MaxExperienceRate");
            }
        }
        private int _MaxExperienceRate;
        /// <summary>
        /// 最大怪物爆金币值
        /// </summary>
        public int MaxGoldRate
        {
            get { return _MaxGoldRate; }
            set
            {
                if (_MaxGoldRate == value) return;

                var oldValue = _MaxGoldRate;
                _MaxGoldRate = value;

                OnChanged(oldValue, value, "MaxGoldRate");
            }
        }
        private int _MaxGoldRate;
        /// <summary>
        /// 地图血值变化
        /// </summary>
        public int HealthRate
        {
            get { return _HealthRate; }
            set
            {
                if (_HealthRate == value) return;

                var oldValue = _HealthRate;
                _HealthRate = value;

                OnChanged(oldValue, value, "HealthRate");
            }
        }
        private int _HealthRate;
        /// <summary>
        /// 地图是否黑炎属性设置
        /// </summary>
        public bool BlackScorching
        {
            get { return _BlackScorching; }
            set
            {
                if (_BlackScorching == value) return;

                var oldValue = _BlackScorching;
                _BlackScorching = value;

                OnChanged(oldValue, value, "BlackScorching");
            }
        }
        private bool _BlackScorching;
        /// <summary>
        /// 地图对应脚本(目前暂时空置)
        /// </summary>
        public string Script
        {
            get { return _Script; }
            set
            {
                if (_Script == value) return;
                var oldValue = _Script;
                _Script = value;

                OnChanged(oldValue, value, "Script");
            }
        }
        private string _Script;
        /// <summary>
        /// 地图是否禁止使用道具
        /// </summary>
        public string NoUseItem
        {
            get { return _NoUseItem; }
            set
            {
                if (_NoUseItem == value) return;

                var oldValue = _NoUseItem;
                _NoUseItem = value;

                OnChanged(oldValue, value, "NoUseItem");
            }
        }
        private string _NoUseItem;
        /// <summary>
        /// 勾选禁止当前地图挂机
        /// </summary>
        public bool BanAndroidPlayer
        {
            get { return _BanAndroidPlayer; }
            set
            {
                if (_BanAndroidPlayer == value) return;

                var oldValue = _BanAndroidPlayer;
                _BanAndroidPlayer = value;

                OnChanged(oldValue, value, "BanAndroidPlayer");
            }
        }
        private bool _BanAndroidPlayer;
        /// <summary>
        /// 勾选当前地图不显示玩家名字无法查看玩家属性
        /// </summary>
        public bool CanPlayName
        {
            get { return _CanPlayName; }
            set
            {
                if (_CanPlayName == value) return;

                var oldValue = _CanPlayName;
                _CanPlayName = value;

                OnChanged(oldValue, value, "CanPlayName");
            }
        }
        private bool _CanPlayName;
        /// <summary>
        /// 勾选当前地图禁言无法聊天
        /// </summary>
        public bool CanNoChat
        {
            get { return _CanNoChat; }
            set
            {
                if (_CanNoChat == value) return;

                var oldValue = _CanNoChat;
                _CanNoChat = value;

                OnChanged(oldValue, value, "CanNoChat");
            }
        }
        private bool _CanNoChat;
        /// <summary>
        /// 勾选当前地图锁定全体模式无法切换
        /// </summary>
        public AttackMode AttackMode
        {
            get { return _AttackMode; }
            set
            {
                if (_AttackMode == value) return;

                var oldValue = _AttackMode;
                _AttackMode = value;

                OnChanged(oldValue, value, "AttackMode");
            }
        }
        private AttackMode _AttackMode;
        /// <summary>
        /// 勾选当前地图死亡立即回城复活
        /// </summary>
        public bool CanDeadTownRevive
        {
            get { return _CanDeadTownRevive; }
            set
            {
                if (_CanDeadTownRevive == value) return;

                var oldValue = _CanDeadTownRevive;
                _CanDeadTownRevive = value;

                OnChanged(oldValue, value, "CanDeadTownRevive");
            }
        }
        private bool _CanDeadTownRevive;

        [Association("Guards", true)]
        public DBBindingList<GuardInfo> Guards { get; set; }

        [Association("Regions", true)]
        public DBBindingList<MapRegion> Regions { get; set; }

        [Association("Mining", true)]
        public DBBindingList<MineInfo> Mining { get; set; }

        protected override internal void OnCreated() //创建时
        {
            base.OnCreated();

            AllowRT = true;  //允许传送
            AllowTT = true;  //允许回城
            CanMarriageRecall = true;  //允许结婚戒指传送
            AllowRecall = true;        //允许召唤
            DeathDrop = true;          //允许地图死亡爆装
        }
        [IgnoreProperty]
        public int fubenIndex { get; internal set; }

        /// <summary>
        /// 克隆 复刻
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            //return this as object;      //引用同一个对象
            return this.MemberwiseClone(); //浅复制
            //return new DrawBase() as object;//深复制
        }
        //Client Variables

        public bool Expanded = true;
    }
}
