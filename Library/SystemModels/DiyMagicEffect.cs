using MirDB;
using System.Drawing;

namespace Library.SystemModels
{
    /// <summary>
    /// 自定义技能动画
    /// </summary>
    public sealed class DiyMagicEffect : DBObject
    {
        /// <summary>
        /// 自定义技能ID
        /// </summary>
        public int MagicID
        {
            get { return _MagicID; }
            set
            {
                if (_MagicID == value) return;

                var oldValue = _MagicID;
                _MagicID = value;

                OnChanged(oldValue, value, "MagicID");
            }
        }
        private int _MagicID;
        /// <summary>
        /// 自定义技能类型
        /// </summary>
        public DiyMagicType MagicType
        {
            get { return _MagicType; }
            set
            {
                if (_MagicType == value) return;

                var oldValue = _MagicType;
                _MagicType = value;

                OnChanged(oldValue, value, "MagicType");
            }
        }
        private DiyMagicType _MagicType;
        /// <summary>
        /// 魔法技能图片起始索引
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
        /// 魔法技能特效图片数
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
        /// 魔法技能图片播放速度时间
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
        /// 自定义魔法技能释放方向
        /// </summary>
        public MagicDir magicDir
        {
            get { return _magicDir; }
            set
            {
                if (_magicDir == value) return;

                var oldValue = _magicDir;
                _magicDir = value;

                OnChanged(oldValue, value, "magicDir");
            }
        }
        private MagicDir _magicDir;
        /// <summary>
        /// 魔法技能素材库
        /// </summary>
        public LibraryFile file
        {
            get { return _file; }
            set
            {
                if (_file == value) return;

                var oldValue = _file;
                _file = value;

                OnChanged(oldValue, value, "file");
            }
        }
        private LibraryFile _file;
        /// <summary>
        /// 起始光效
        /// </summary>
        public int startLight
        {
            get { return _startLight; }
            set
            {
                if (_startLight == value) return;

                var oldValue = _startLight;
                _startLight = value;

                OnChanged(oldValue, value, "startLight");
            }
        }
        private int _startLight;
        /// <summary>
        /// 结束光效
        /// </summary>
        public int endLight
        {
            get { return _endLight; }
            set
            {
                if (_endLight == value) return;

                var oldValue = _endLight;
                _endLight = value;

                OnChanged(oldValue, value, "endLight");
            }
        }
        private int _endLight;
        /// <summary>
        /// 爆炸效果魔法ID
        /// </summary>
        public int ExplosionMagicID
        {
            get { return _ExplosionMagicID; }
            set
            {
                if (_ExplosionMagicID == value) return;

                var oldValue = _ExplosionMagicID;
                _ExplosionMagicID = value;

                OnChanged(oldValue, value, "ExplosionMagicID");
            }
        }
        private int _ExplosionMagicID;
        /// <summary>
        /// 光效颜色
        /// </summary>
        public Color lightColour
        {
            get { return _lightColour; }
            set
            {
                if (_lightColour == value) return;

                var oldValue = _lightColour;
                _lightColour = value;

                OnChanged(oldValue, value, "lightColour");
            }
        }
        private Color _lightColour;
        /// <summary>
        /// 魔法声效
        /// </summary>
        public SoundIndex MagicSoundIdx
        {
            get { return _MagicSoundIdx; }
            set
            {
                if (_MagicSoundIdx == value) return;

                var oldValue = _MagicSoundIdx;
                _MagicSoundIdx = value;

                OnChanged(oldValue, value, "MagicSoundIdx");
            }
        }
        private SoundIndex _MagicSoundIdx;

        /// <summary>
        /// 自定义技能音效
        /// </summary>
        public string MagicSound
        {
            get { return _MagicSound; }
            set
            {
                if (_MagicSound == value) return;

                var oldValue = _MagicSound;
                _MagicSound = value;

                OnChanged(oldValue, value, "MagicSound");
            }
        }
        private string _MagicSound;
    }
}
