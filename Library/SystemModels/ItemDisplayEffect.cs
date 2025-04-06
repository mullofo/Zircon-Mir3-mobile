using MirDB;

namespace Library.SystemModels
{
    public sealed class ItemDisplayEffect : DBObject
    {
        [Association("ItemDisplayEffects")]
        public ItemInfo Info
        {
            get { return _Info; }
            set
            {
                if (_Info == value) return;

                var oldValue = _Info;
                _Info = value;

                OnChanged(oldValue, value, "Info");
            }
        }
        private ItemInfo _Info;

        public bool DrawInnerEffect
        {
            get { return _DrawInnerEffect; }
            set
            {
                if (_DrawInnerEffect == value) return;

                var oldValue = _DrawInnerEffect;
                _DrawInnerEffect = value;

                OnChanged(oldValue, value, "DrawInnerEffect");
            }
        }
        private bool _DrawInnerEffect;

        public LibraryFile InnerEffectLibrary
        {
            get { return _InnerEffectLibrary; }
            set
            {
                if (_InnerEffectLibrary == value) return;

                var oldValue = _InnerEffectLibrary;
                _InnerEffectLibrary = value;

                OnChanged(oldValue, value, "InnerEffectLibrary");
            }
        }
        private LibraryFile _InnerEffectLibrary;

        public int InnerImageStartIndex
        {
            get { return _InnerImageStartIndex; }
            set
            {
                if (_InnerImageStartIndex == value) return;

                var oldValue = _InnerImageStartIndex;
                _InnerImageStartIndex = value;

                OnChanged(oldValue, value, "InnerImageStartIndex");
            }
        }
        private int _InnerImageStartIndex;

        public int InnerImageCount
        {
            get { return _InnerImageCount; }
            set
            {
                if (_InnerImageCount == value) return;

                var oldValue = _InnerImageCount;
                _InnerImageCount = value;

                OnChanged(oldValue, value, "InnerImageCount");
            }
        }
        private int _InnerImageCount;

        public bool EffectBehindImage
        {
            get { return _EffectBehindImage; }
            set
            {
                if (_EffectBehindImage == value) return;

                var oldValue = _EffectBehindImage;
                _EffectBehindImage = value;

                OnChanged(oldValue, value, "EffectBehindImage");
            }
        }
        private bool _EffectBehindImage;

        public bool DrawOuterEffect
        {
            get { return _DrawOuterEffect; }
            set
            {
                if (_DrawOuterEffect == value) return;

                var oldValue = _DrawOuterEffect;
                _DrawOuterEffect = value;

                OnChanged(oldValue, value, "DrawOuterEffect");
            }
        }
        private bool _DrawOuterEffect;

        public LibraryFile OuterEffectLibrary
        {
            get { return _OuterEffectLibrary; }
            set
            {
                if (_OuterEffectLibrary == value) return;

                var oldValue = _OuterEffectLibrary;
                _OuterEffectLibrary = value;

                OnChanged(oldValue, value, "OuterEffectLibrary");
            }
        }
        private LibraryFile _OuterEffectLibrary;

        public bool IsUnisex
        {
            get { return _IsUnisex; }
            set
            {
                if (_IsUnisex == value) return;

                var oldValue = _IsUnisex;
                _IsUnisex = value;

                OnChanged(oldValue, value, "IsUnisex");
            }
        }
        private bool _IsUnisex;

        public int OuterImageStartIndex
        {
            get { return _OuterImageStartIndex; }
            set
            {
                if (_OuterImageStartIndex == value) return;

                var oldValue = _OuterImageStartIndex;
                _OuterImageStartIndex = value;

                OnChanged(oldValue, value, "OuterImageStartIndex");
            }
        }
        private int _OuterImageStartIndex;

        public int OuterImageCount
        {
            get { return _OuterImageCount; }
            set
            {
                if (_OuterImageCount == value) return;

                var oldValue = _OuterImageCount;
                _OuterImageCount = value;

                OnChanged(oldValue, value, "OuterImageCount");
            }
        }
        private int _OuterImageCount;

        public int InnerX
        {
            get { return _InnerX; }
            set
            {
                if (_InnerX == value) return;

                var oldValue = _InnerX;
                _InnerX = value;

                OnChanged(oldValue, value, "InnerX");
            }
        }
        private int _InnerX;

        public int InnerY
        {
            get { return _InnerY; }
            set
            {
                if (_InnerY == value) return;

                var oldValue = _InnerY;
                _InnerY = value;

                OnChanged(oldValue, value, "InnerY");
            }
        }
        private int _InnerY;

        public int OuterX
        {
            get { return _OuterX; }
            set
            {
                if (_OuterX == value) return;

                var oldValue = _OuterX;
                _OuterX = value;

                OnChanged(oldValue, value, "OuterX");
            }
        }
        private int _OuterX;

        public int OuterY
        {
            get { return _OuterY; }
            set
            {
                if (_OuterY == value) return;

                var oldValue = _OuterY;
                _OuterY = value;

                OnChanged(oldValue, value, "OuterY");
            }
        }
        private int _OuterY;
    }
}
