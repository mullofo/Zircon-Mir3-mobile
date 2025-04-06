namespace Client.Controls
{
    /// <summary>
    /// 自定按钮控件
    /// </summary>
    public class DxMirButton : DXImageControl
    {
        public MirButtonType MirButtonType { get; set; } = MirButtonType.Normal;

        private int _normalIndex = -1;

        /// <summary>
        /// 重置按钮状态,便于重新注册Index
        /// 通常整个按钮的图标发生变化时
        /// </summary>
        public void Reset()
        {
            _normalIndex = -1;
            Index = _normalIndex;
        }
        /// <summary>
        /// 启用时更改
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnEnabledChanged(bool oValue, bool nValue)
        {
            if (MirButtonType == MirButtonType.FourStatuReverse)
            {
                Index = !nValue ? _normalIndex + 1 : _normalIndex;
            }
            else if (MirButtonType == MirButtonType.FourStatu)
            {
                Index = !nValue ? _normalIndex - 2 : _normalIndex;
            }
        }
        /// <summary>
        /// 索引更改时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnIndexChanged(int oValue, int nValue)
        {
            base.OnIndexChanged(oValue, nValue);
            if (_normalIndex == -1)
            {
                _normalIndex = nValue == 0 ? oValue : nValue;
            }
        }
        /// <summary>
        /// 自定按钮控件
        /// </summary>
        public DxMirButton() : base()
        {
            MouseEnter += (s, e) =>   //鼠标进入
            {
                if (!Enabled)
                {
                    return;
                }
                if (MirButtonType == MirButtonType.FourStatu)
                {
                    Index = _normalIndex - 2;
                }
                else if (MirButtonType == MirButtonType.FourStatuReverse || MirButtonType == MirButtonType.TowStatu2)
                {
                    Index = _normalIndex + 1;
                }
            };
            MouseDown += (s, e) =>  //鼠标按下
            {
                if (!Enabled)
                {
                    return;
                }
                if (MirButtonType == MirButtonType.Normal) return;
                if (MirButtonType == MirButtonType.FourStatu || MirButtonType == MirButtonType.FourStatuReverse)
                {
                    Index = _normalIndex - 1;
                }
            };
            MouseUp += (s, e) =>   //鼠标向上
            {
                if (!Enabled)
                {
                    return;
                }
                if (MirButtonType == MirButtonType.Normal) return;
                if (MirButtonType == MirButtonType.FourStatu)
                {
                    Index = _normalIndex;
                }
                else if (MirButtonType == MirButtonType.TowStatu || MirButtonType == MirButtonType.FourStatuReverse)
                {
                    Index = _normalIndex + 1;
                }
            };
            MouseLeave += (s, e) =>   //鼠标离开时
            {
                if (!Enabled)
                {
                    return;
                }
                if (MirButtonType == MirButtonType.Normal) return;
                Index = _normalIndex;
            };
        }
    }

    /// <summary>
    /// 自定按键类型
    /// </summary>
    public enum MirButtonType
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal,
        /// <summary>
        /// 鼠标向下向上 
        /// </summary>
        TowStatu,
        /// <summary>
        /// 鼠标悬停向上离开索引 正向Index
        /// </summary>
        FourStatu,
        /// <summary>
        /// 鼠标悬停向上离开索引 反向index
        /// </summary>
        FourStatuReverse,
        /// <summary>
        /// 鼠标进入离开
        /// </summary>
        TowStatu2
    }
}
