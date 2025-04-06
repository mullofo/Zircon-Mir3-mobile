using MirDB;
using static MirDB.Association;

namespace Library.SystemModels
{
    /// <summary>
    /// 怪物信息
    /// </summary>
    [Lang]
    public sealed class MonsterInfo : DBObject
    {
        /// <summary>
        /// 怪物名字
        /// </summary>
        [Lang("怪物名")]
        [Export]
        public string MonsterName
        {
            get { return _MonsterName; }
            set
            {
                if (_MonsterName == value) return;

                var oldValue = _MonsterName;
                _MonsterName = value;

                OnChanged(oldValue, value, "MonsterName");
            }
        }
        private string _MonsterName;

        /// <summary>
        /// 怪物图片
        /// </summary>
        public MonsterImage Image
        {
            get { return _Image; }
            set
            {
                if (_Image == value) return;

                var oldValue = _Image;
                _Image = value;

                OnChanged(oldValue, value, "Image");
            }
        }
        private MonsterImage _Image;
        /// <summary>
        /// 怪物图库
        /// </summary>
        public LibraryFile File
        {
            get { return _File; }
            set
            {
                if (_File == value) return;

                var oldValue = _File;
                _File = value;

                OnChanged(oldValue, value, "File");
            }
        }
        private LibraryFile _File;
        /// <summary>
        /// 怪物攻击声效
        /// </summary>
        public SoundIndex AttackSound
        {
            get { return _AttackSound; }
            set
            {
                if (_AttackSound == value) return;

                var oldValue = _AttackSound;
                _AttackSound = value;

                OnChanged(oldValue, value, "AttackSound");
            }
        }
        private SoundIndex _AttackSound;
        /// <summary>
        /// 怪物被攻击声效
        /// </summary>
        public SoundIndex StruckSound
        {
            get { return _StruckSound; }
            set
            {
                if (_StruckSound == value) return;

                var oldValue = _StruckSound;
                _StruckSound = value;

                OnChanged(oldValue, value, "StruckSound");
            }
        }
        private SoundIndex _StruckSound;
        /// <summary>
        /// 怪物死亡声效
        /// </summary>
        public SoundIndex DieSound
        {
            get { return _DieSound; }
            set
            {
                if (_DieSound == value) return;

                var oldValue = _DieSound;
                _DieSound = value;

                OnChanged(oldValue, value, "DieSound");
            }
        }
        private SoundIndex _DieSound;
        /// <summary>
        /// 怪物形态
        /// </summary>
        public int BodyShape
        {
            get { return _BodyShape; }
            set
            {
                if (_BodyShape == value) return;

                var oldValue = _BodyShape;
                _BodyShape = value;

                OnChanged(oldValue, value, "BodyShape");
            }

        }
        private int _BodyShape;
        /// <summary>
        /// 怪物图片
        /// </summary>
        public int Portrait
        {
            get { return _Portrait; }
            set
            {
                if (_Portrait == value) return;

                var oldValue = _Portrait;
                _Portrait = value;

                OnChanged(oldValue, value, "Portrait");
            }
        }
        private int _Portrait;
        /// <summary>
        /// 怪物AI
        /// </summary>
        public int AI
        {
            get { return _AI; }
            set
            {
                if (_AI == value) return;

                var oldValue = _AI;
                _AI = value;

                OnChanged(oldValue, value, "AI");
            }
        }
        private int _AI;
        /// <summary>
        /// 怪物等级
        /// </summary>
        public int Level
        {
            get { return _Level; }
            set
            {
                if (_Level == value) return;

                var oldValue = _Level;
                _Level = value;

                OnChanged(oldValue, value, "Level");
            }
        }
        private int _Level;
        /// <summary>
        /// 怪物视野
        /// </summary>
        public int ViewRange
        {
            get { return _ViewRange; }
            set
            {
                if (_ViewRange == value) return;

                var oldValue = _ViewRange;
                _ViewRange = value;

                OnChanged(oldValue, value, "ViewRange");
            }
        }
        private int _ViewRange;
        /// <summary>
        /// 怪物反隐身几率
        /// </summary>
        public int CoolEye
        {
            get { return _CoolEye; }
            set
            {
                if (_CoolEye == value) return;

                var oldValue = _CoolEye;
                _CoolEye = value;

                OnChanged(oldValue, value, "CoolEye");
            }
        }
        private int _CoolEye;
        /// <summary>
        /// 怪物经验
        /// </summary>
        public decimal Experience
        {
            get { return _Experience; }
            set
            {
                if (_Experience == value) return;

                var oldValue = _Experience;
                _Experience = value;

                OnChanged(oldValue, value, "Experience");
            }
        }
        private decimal _Experience;
        /// <summary>
        /// 怪物是否死系或者生系
        /// </summary>
        public bool Undead
        {
            get { return _Undead; }
            set
            {
                if (_Undead == value) return;

                var oldValue = _Undead;
                _Undead = value;

                OnChanged(oldValue, value, "Undead");
            }
        }
        private bool _Undead;

        /// <summary>
        /// 怪物是否可以抗拒或者推
        /// </summary>
        public bool CanPush
        {
            get { return _CanPush; }
            set
            {
                if (_CanPush == value) return;

                var oldValue = _CanPush;
                _CanPush = value;

                OnChanged(oldValue, value, "CanPush");
            }
        }
        private bool _CanPush;
        /// <summary>
        /// 怪物是否可以诱惑
        /// </summary>
        public bool CanTame
        {
            get { return _CanTame; }
            set
            {
                if (_CanTame == value) return;

                var oldValue = _CanTame;
                _CanTame = value;

                OnChanged(oldValue, value, "CanTame");
            }
        }
        private bool _CanTame;

        /// <summary>
        /// 怪物攻击速度
        /// </summary>
        public int AttackDelay
        {
            get { return _AttackDelay; }
            set
            {
                if (_AttackDelay == value) return;

                var oldValue = _AttackDelay;
                _AttackDelay = value;

                OnChanged(oldValue, value, "AttackDelay");
            }
        }
        private int _AttackDelay;
        /// <summary>
        /// 怪物移动速度
        /// </summary>
        public int MoveDelay
        {
            get { return _MoveDelay; }
            set
            {
                if (_MoveDelay == value) return;

                var oldValue = _MoveDelay;
                _MoveDelay = value;

                OnChanged(oldValue, value, "MoveDelay");
            }
        }
        private int _MoveDelay;
        /// <summary>
        /// 怪物是否BOSS
        /// </summary>
        public bool IsBoss
        {
            get { return _IsBoss; }
            set
            {
                if (_IsBoss == value) return;

                var oldValue = _IsBoss;
                _IsBoss = value;

                OnChanged(oldValue, value, "IsBoss");
            }
        }
        private bool _IsBoss;
        /// <summary>
        /// 怪物标识
        /// </summary>
        public MonsterFlag Flag
        {
            get { return _Flag; }
            set
            {
                if (_Flag == value) return;

                var oldValue = _Flag;
                _Flag = value;

                OnChanged(oldValue, value, "Flag");
            }
        }
        private MonsterFlag _Flag;
        /// <summary>
        /// 怪物是否能被召唤成宠物
        /// </summary>
        public bool CallFightPet
        {
            get { return _CallFightPet; }
            set
            {
                if (_CallFightPet == value) return;

                var oldValue = _CallFightPet;
                _CallFightPet = value;

                OnChanged(oldValue, value, "CallFightPet");
            }
        }
        private bool _CallFightPet;
        /// <summary>
        /// 怪物是否能被口哨唤出
        /// </summary>
        public bool CallMapSummoning
        {
            get { return _CallMapSummoning; }
            set
            {
                if (_CallMapSummoning == value) return;

                var oldValue = _CallMapSummoning;
                _CallMapSummoning = value;

                OnChanged(oldValue, value, "CallMapSummoning");
            }
        }
        private bool _CallMapSummoning;

        public int DiyMoveMode//1 原地不动  0 正常移动 -1 反方向跑  2瞬移模式
        {
            get { return _DiyMoveMode; }
            set
            {
                if (_DiyMoveMode == value) return;

                var oldValue = _DiyMoveMode;
                _DiyMoveMode = value;

                OnChanged(oldValue, value, "DiyMoveMode");
            }
        }
        private int _DiyMoveMode;

        /// <summary>
        /// 怪物是否被冰系技能冰冻
        /// </summary>
        public bool IsSlow
        {
            get { return _IsSlow; }
            set
            {
                if (_IsSlow == value) return;

                var oldValue = _IsSlow;
                _IsSlow = value;

                OnChanged(oldValue, value, "IsSlow");
            }
        }
        private bool _IsSlow;

        /// <summary>
        /// 怪物信息属性列表
        /// </summary>
        [Association("MonsterInfoStats", true)]
        public DBBindingList<MonsterInfoStat> MonsterInfoStats { get; set; }
        /// <summary>
        /// 怪物刷新信息列表
        /// </summary>
        [Association("Respawns", true)]
        public DBBindingList<RespawnInfo> Respawns { get; set; }
        /// <summary>
        /// 怪物爆率信息列表
        /// </summary>
        [Association("Drops", true)]
        public DBBindingList<DropInfo> Drops { get; set; }

        /// <summary>
        /// 怪物目标时间信息列表
        /// </summary>
        [Association("Events", true)]
        public DBBindingList<MonsterEventTarget> Events { get; set; }
        /// <summary>
        /// 怪物任务信息列表
        /// </summary>
        [Association("QuestDetails", true)]
        public DBBindingList<QuestTaskMonsterDetails> QuestDetails { get; set; }

        public Stats Stats = new Stats();

        /// <summary>
        /// 自定义怪物AI
        /// </summary>
        [Association("MonDiyAiActions", true)]
        public DBBindingList<MonDiyAiAction> MonDiyAiActions { get; set; }
        /// <summary>
        /// 怪物自定义信息列表
        /// </summary>
        [Association("MonAnimationFrames", true)]
        public DBBindingList<MonAnimationFrame> MonAnimationFrames { get; set; }


        protected override internal void OnCreated()
        {
            base.OnCreated();

            CanPush = true;

            ViewRange = 7;

            AttackDelay = 2500;
            MoveDelay = 1800;
        }

        protected override internal void OnLoaded()
        {
            base.OnLoaded();
            StatsChanged();
        }

        public void StatsChanged()
        {
            Stats.Clear();
            foreach (MonsterInfoStat stat in MonsterInfoStats)
                Stats[stat.Stat] += stat.Amount;
        }
    }
}
