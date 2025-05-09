﻿using Client.Envir;
using System;

//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 动画控件
    /// </summary>
    public class DXAnimatedControl : DXImageControl
    {
        #region Properties

        #region Animated
        /// <summary>
        /// 动画
        /// </summary>
        public bool Animated
        {
            get => _Animated;
            set
            {
                if (_Animated == value) return;

                bool oldValue = _Animated;
                _Animated = value;

                OnAnimatedChanged(oldValue, value);
            }
        }
        private bool _Animated;
        public event EventHandler<EventArgs> AnimatedChanged;
        public virtual void OnAnimatedChanged(bool oValue, bool nValue)
        {
            if (!Animated) Index = BaseIndex;

            Process();

            AnimatedChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region BaseIndex
        /// <summary>
        /// 起始序号值
        /// </summary>
        public int BaseIndex
        {
            get => _BaseIndex;
            set
            {
                if (_BaseIndex == value) return;

                int oldValue = _BaseIndex;
                _BaseIndex = value;

                OnBaseIndexChanged(oldValue, value);
            }
        }
        private int _BaseIndex;
        public event EventHandler<EventArgs> BaseIndexChanged;
        public virtual void OnBaseIndexChanged(int oValue, int nValue)
        {
            if (!Animated) Index = BaseIndex;

            Process();

            BaseIndexChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region FrameCount
        /// <summary>
        /// 帧数
        /// </summary>
        public int FrameCount
        {
            get => _FrameCount;
            set
            {
                if (_FrameCount == value) return;

                int oldValue = _FrameCount;
                _FrameCount = value;

                OnFrameCountChanged(oldValue, value);
            }
        }
        private int _FrameCount;
        public event EventHandler<EventArgs> FrameCountChanged;
        public virtual void OnFrameCountChanged(int oValue, int nValue)
        {
            FrameCountChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region AnimationDelay
        /// <summary>
        /// 动画延迟
        /// </summary>
        public TimeSpan AnimationDelay
        {
            get => _AnimationDelay;
            set
            {
                if (_AnimationDelay == value) return;

                TimeSpan oldValue = _AnimationDelay;
                _AnimationDelay = value;

                OnAnimationDelayChanged(oldValue, value);
            }
        }
        private TimeSpan _AnimationDelay;
        public event EventHandler<EventArgs> AnimationDelayChanged;
        public virtual void OnAnimationDelayChanged(TimeSpan oValue, TimeSpan nValue)
        {
            Process();

            AnimationDelayChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Loop
        /// <summary>
        /// 循环
        /// </summary>
        public bool Loop
        {
            get => _Loop;
            set
            {
                if (_Loop == value) return;

                bool oldValue = _Loop;
                _Loop = value;

                OnLoopChanged(oldValue, value);
            }
        }
        private bool _Loop;
        public event EventHandler<EventArgs> LoopChanged;
        public virtual void OnLoopChanged(bool oValue, bool nValue)
        {
            LoopChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        public event EventHandler AfterAnimation;
        public DateTime AnimationStart;
        /// <summary>
        /// 倒序播放动画
        /// </summary>
        public bool Reversed;
        #endregion
        /// <summary>
        /// 动画控件
        /// </summary>
        public DXAnimatedControl()
        {
            Animated = true;
            BaseIndex = -1;
            Loop = true;
        }
        #region Methods
        /// <summary>
        /// 动画过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            if (!IsVisible) return;


            if (!Animated || AnimationDelay == TimeSpan.Zero || FrameCount == 0) return;

            if (AnimationStart == DateTime.MinValue) AnimationStart = CEnvir.Now;

            TimeSpan time = CEnvir.Now - AnimationStart;
            Index = (Reversed ? ((BaseIndex + FrameCount - 1) - (int)(time.Ticks / (AnimationDelay.Ticks / FrameCount) % FrameCount)) : (BaseIndex + (int)(time.Ticks / (AnimationDelay.Ticks / FrameCount) % FrameCount)));

            if (Loop || time < AnimationDelay) return;

            Animated = false;
            Index = BaseIndex + FrameCount - 1;

            AfterAnimation?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Animated = false;
                _BaseIndex = 0;
                _FrameCount = 0;
                _AnimationDelay = TimeSpan.Zero;
                _Loop = false;

                AnimatedChanged = null;
                BaseIndexChanged = null;
                FrameCountChanged = null;
                AnimationDelayChanged = null;
                LoopChanged = null;

                AfterAnimation = null;
                AnimationStart = DateTime.MinValue;
            }
        }
        #endregion
    }
}
