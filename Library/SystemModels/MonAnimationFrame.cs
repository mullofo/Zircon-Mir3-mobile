using MirDB;

namespace Library.SystemModels
{
    /// <summary>
    /// 自定义怪物设置
    /// </summary>
    public sealed class MonAnimationFrame : DBObject
    {
        /// <summary>
        /// 怪物信息名称
        /// </summary>
        [Association("MonAnimationFrame")]
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
        /// 怪物ID索引
        /// </summary>
        public int MonsterIdx
        {
            get { return _MonsterIdx; }
            set
            {
                if (_MonsterIdx == value) return;

                var oldValue = _MonsterIdx;
                _MonsterIdx = value;

                OnChanged(oldValue, value, "MonsterIdx");
            }
        }
        private int _MonsterIdx;
        /// <summary>
        /// 动作行为动画
        /// </summary>
        public MirAnimation MonAnimation
        {
            get { return _Animation; }
            set
            {
                if (_Animation == value) return;

                var oldValue = _Animation;
                _Animation = value;

                OnChanged(oldValue, value, "Animation");
            }
        }
        private MirAnimation _Animation;
        /// <summary>
        /// 起始动画
        /// </summary>
        public int startIndex
        {
            get { return _startIndex; }
            set
            {
                if (_startIndex == value) return;

                var oldValue = _startIndex;
                _startIndex = value;

                OnChanged(oldValue, value, "startIndex");
            }
        }
        private int _startIndex;
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
        /// 动画图片偏移
        /// </summary>
        public int offSet
        {
            get { return _offSet; }
            set
            {
                if (_offSet == value) return;

                var oldValue = _offSet;
                _offSet = value;

                OnChanged(oldValue, value, "offSet");
            }
        }
        private int _offSet;
        /// <summary>
        /// 动画循环时间
        /// </summary>
        public int frameDelay
        {
            get { return _frameDelay; }
            set
            {
                if (_frameDelay == value) return;

                var oldValue = _frameDelay;
                _frameDelay = value;

                OnChanged(oldValue, value, "frameDelay");
            }
        }
        private int _frameDelay;
        /// <summary>
        /// 是否反向播放
        /// </summary>
        public bool Reversed
        {
            get { return _Reversed; }
            set
            {
                if (_Reversed == value) return;

                var oldValue = _Reversed;
                _Reversed = value;

                OnChanged(oldValue, value, "Reversed");
            }
        }
        private bool _Reversed;
        /// <summary>
        /// 是否固定播放速度
        /// </summary>
        public bool StaticSpeed
        {
            get { return _StaticSpeed; }
            set
            {
                if (_StaticSpeed == value) return;

                var oldValue = _StaticSpeed;
                _StaticSpeed = value;

                OnChanged(oldValue, value, "StaticSpeed");
            }
        }
        private bool _StaticSpeed;
        /// <summary>
        /// 音效选择
        /// </summary>
        public SoundIndex ActSound
        {
            get { return _ActSound; }
            set
            {
                if (_ActSound == value) return;

                var oldValue = _ActSound;
                _ActSound = value;

                OnChanged(oldValue, value, "ActSound");
            }
        }
        public SoundIndex _ActSound;
        /// <summary>
        /// 自定义音效
        /// </summary>
        public string ActSoundStr
        {
            get { return _ActSoundStr; }
            set
            {
                if (_ActSoundStr == value) return;

                var oldValue = _ActSoundStr;
                _ActSoundStr = value;

                OnChanged(oldValue, value, "ActSoundStr");
            }
        }
        public string _ActSoundStr;
        /// <summary>
        /// 魔法技能素材库
        /// </summary>
        public LibraryFile effectfile
        {
            get { return _effectfile; }
            set
            {
                if (_effectfile == value) return;

                var oldValue = _effectfile;
                _effectfile = value;

                OnChanged(oldValue, value, "effectfile");
            }
        }
        private LibraryFile _effectfile;
        /// <summary>
        /// 魔法技能图片起始索引
        /// </summary>
        public int effectfrom
        {
            get { return _effectfrom; }
            set
            {
                if (_effectfrom == value) return;

                var oldValue = _effectfrom;
                _effectfrom = value;

                OnChanged(oldValue, value, "effectfrom");
            }
        }
        private int _effectfrom;
        /// <summary>
        /// 魔法技能特效图片数
        /// </summary>
        public int effectframe
        {
            get { return _effectframe; }
            set
            {
                if (_effectframe == value) return;

                var oldValue = _effectframe;
                _effectframe = value;

                OnChanged(oldValue, value, "effectframe");
            }
        }
        private int _effectframe;
        /// <summary>
        /// 魔法技能图片播放速度时间
        /// </summary>
        public int effectdelay
        {
            get { return _effectdelay; }
            set
            {
                if (_effectdelay == value) return;

                var oldValue = _effectdelay;
                _effectdelay = value;

                OnChanged(oldValue, value, "effectdelay");
            }
        }
        private int _effectdelay;
        /// <summary>
        /// 自定义魔法技能释放方向
        /// </summary>
        public MagicDir effectdir
        {
            get { return _effectdir; }
            set
            {
                if (_effectdir == value) return;

                var oldValue = _effectdir;
                _effectdir = value;

                OnChanged(oldValue, value, "effectdir");
            }
        }
        private MagicDir _effectdir;
    }
}
