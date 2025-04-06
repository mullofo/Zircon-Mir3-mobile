using MirDB;

namespace Library.SystemModels
{
    /// <summary>
    /// 自定义怪物动画效果
    /// </summary>
    public sealed class MonAnimationEffect : DBObject
    {
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
