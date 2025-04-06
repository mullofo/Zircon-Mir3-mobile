using Client.Envir;
using Client.Helpers;
using Client.Scenes;
using Library;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mir3.Mobile;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Texture = MonoGame.Extended.Texture;

namespace Client.Controls
{
    /// <summary>
    /// 控制组件
    /// </summary>
    public class DXControl : IDisposable
    {
        #region 手机缩放
        //public bool AutoZoom = true;
        public float ZoomRate
        {
            get
            {
                return _ZoomRate;
            }
            set
            {
                if (_ZoomRate != value)
                {
                    float zoomRate = _ZoomRate;
                    _ZoomRate = value;
                    //OnZoomRateChanged(zoomRate, value);
                }
            }
        }
        private float _ZoomRate = 1f;
        //public event EventHandler<EventArgs> ZoomRateChanged;
        //public virtual void OnZoomRateChanged(float oValue, float nValue)
        //{
        //    //UpdateDisplayArea();
        //    ZoomRateChanged?.Invoke(this, EventArgs.Empty);
        //}
        public int UI_Offset_X
        {
            get
            {
                return _UI_Offset_X;
            }
            set
            {
                if (_UI_Offset_X != value)
                {
                    int oldValue = _UI_Offset_X;
                    _UI_Offset_X = value;
                    //OnZoomRateChanged(oldValue, value);
                }
            }
        }
        private int _UI_Offset_X;
        #endregion

        #region Static
        /// <summary>
        /// 消息框列表
        /// </summary>
        public static List<DXControl> MessageBoxList = new List<DXControl>();

        /// <summary>
        /// 鼠标控制
        /// </summary>
        public static DXControl MouseControl
        {
            get => _MouseControl;
            set
            {
                if (_MouseControl == value) return;

                DXControl oldControl = _MouseControl;
                _MouseControl = value;

                oldControl?.OnMouseLeave();

                _MouseControl?.OnMouseEnter();
            }
        }
        private static DXControl _MouseControl;

        /// <summary>
        /// 焦点控制
        /// </summary>
        public static DXControl FocusControl
        {
            get => _FocusControl;
            set
            {
                if (_FocusControl == value) return;

                DXControl oldControl = _FocusControl;
                _FocusControl = value;
                oldControl?.OnLostFocus();

                _FocusControl?.OnFocus();

                if (!(value is DXTextBox control))
                {
                    Game1.Native.HideInputField();
                }
                else if ((DXTextBox.ActiveTextBox == null || !DXTextBox.ActiveTextBox.KeepFocus) && control != null)
                {
                    DXTextBox.ActiveTextBox = value as DXTextBox;
                    if (ActiveScene is GameScene)
                        GameScene.Game?.ChatTextBox?.OpenChat();
                    Game1.Native.ShowInputField(DXTextBox.ActiveTextBox.TextBox.Text);
                }
            }
        }
        private static DXControl _FocusControl;

        /// <summary>
        /// 活动场景
        /// </summary>
        public static DXScene ActiveScene
        {
            get => _ActiveScene;
            set
            {
                if (_ActiveScene == value) return;

                _ActiveScene = value;

                _ActiveScene?.CheckIsVisible();

                if (LibraryHelper.Message != null && !LibraryHelper.Message.IsDisposed)
                {
                    LibraryHelper.Message.TryDispose();
                    LibraryHelper.Message = DXMessageBox.Show("微端服务连接失败，将影响游戏体验。\n15秒后将重新连接！", "错误");
                }
            }
        }
        private static DXScene _ActiveScene;

        /// <summary>
        /// 默认高度
        /// </summary>
        public static int DefaultHeight { get; }
        /// <summary>
        /// 标签高度
        /// </summary>
        public static int TabHeight { get; }
        /// <summary>
        /// 标题栏大小
        /// </summary>
        public static int HeaderBarSize { get; }
        /// <summary>
        /// 页眉大小
        /// </summary>
        public static int HeaderSize { get; }
        /// <summary>
        /// 页脚大小
        /// </summary>
        public static int FooterSize { get; }
        /// <summary>
        /// 无页脚大小
        /// </summary>
        public static int NoFooterSize { get; }
        /// <summary>
        /// 小按钮高度
        /// </summary>
        public static int SmallButtonHeight { get; }

        public static DXLabel DebugLabel, HintLabel, PingLabel;
        /// <summary>
        /// 接口库
        /// </summary>
        protected static MirLibrary InterfaceLibrary;
        /// <summary>
        /// 控制计数器
        /// </summary>
        public static long _ControlCounter { get; private set; } = 0;
        /// <summary>
        /// 控制组件
        /// </summary>
        static DXControl()
        {
            DebugLabel = new DXLabel   //Bug标签绘制
            {
                BackColour = Color.FromArgb(125, 50, 50, 50),
                Border = true,
                BorderColour = Color.Black,
                Location = new Point(5, 5),
                IsVisible = Config.DebugLabel,
                Outline = false,
                ForeColour = Color.White,
            };
            HintLabel = new DXLabel               //Hint标签绘制
            {
                BackColour = Color.FromArgb(200, 255, 255, 160),
                Border = true,
                BorderColour = Color.White,
                IsVisible = true,
                Outline = false,
                ForeColour = Color.Black,
                Padding = new Padding(2),
            };

            PingLabel = new DXLabel        //Ping标签绘制
            {
                BackColour = Color.FromArgb(125, 50, 50, 50),
                Border = true,
                BorderColour = Color.Black,
                Location = new Point(5, 19),
                IsVisible = Config.DebugLabel,
                Outline = false,
                ForeColour = Color.White,
            };

            CEnvir.LibraryList.TryGetValue(LibraryFile.Interface, out InterfaceLibrary);

            if (InterfaceLibrary == null) return;  //如果接口库等空 跳开

            DefaultHeight = InterfaceLibrary.GetSize(16).Height;
            TabHeight = InterfaceLibrary.GetSize(19).Height;
            SmallButtonHeight = InterfaceLibrary.GetSize(41).Height;

            HeaderBarSize = InterfaceLibrary.GetSize(200).Height;

            HeaderSize = HeaderBarSize;
            HeaderSize += InterfaceLibrary.GetSize(203).Height;

            NoFooterSize = InterfaceLibrary.GetSize(202).Height;

            FooterSize = HeaderBarSize;
            FooterSize += InterfaceLibrary.GetSize(202).Height;
            FooterSize += InterfaceLibrary.GetSize(10).Height;
        }
        #endregion

        #region Properties        
        protected internal List<DXControl> Controls { get; private set; } = new List<DXControl>();

        #region AllowDragOut
        /// <summary>
        /// 允许拖出
        /// </summary>
        public bool AllowDragOut
        {
            get => _AllowDragOut;
            set
            {
                if (_AllowDragOut == value) return;

                bool oldValue = _AllowDragOut;
                _AllowDragOut = value;

                OnAllowDragOutChanged(oldValue, value);
            }
        }
        private bool _AllowDragOut;
        public event EventHandler<EventArgs> AllowDragOutChanged;
        /// <summary>
        /// 启用 允许拖出 更改
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public virtual void OnAllowDragOutChanged(bool oValue, bool nValue)
        {
            AllowDragOutChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region AllowResize
        /// <summary>
        /// 允许调整大小
        /// </summary>
        public bool AllowResize
        {
            get => _AllowResize;
            set
            {
                if (_AllowResize == value) return;

                bool oldValue = _AllowResize;
                _AllowResize = value;

                OnAllowResizeChanged(oldValue, value);
            }
        }
        private bool _AllowResize;
        public event EventHandler<EventArgs> AllowResizeChanged;
        /// <summary>
        /// 启用 允许调整大小 改变
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public virtual void OnAllowResizeChanged(bool oValue, bool nValue)
        {
            AllowResizeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region BackColour
        /// <summary>
        /// 背景颜色
        /// </summary>
        public Color BackColour
        {
            get => _BackColour;
            set
            {
                if (_BackColour == value) return;

                Color oldValue = _BackColour;
                _BackColour = value;

                OnBackColourChanged(oldValue, value);
            }
        }
        private Color _BackColour;
        public event EventHandler<EventArgs> BackColourChanged;
        /// <summary>
        /// 启用 背景颜色 改变
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public virtual void OnBackColourChanged(Color oValue, Color nValue)
        {
            TextureValid = false;
            BackColourChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Border
        /// <summary>
        /// 边界
        /// </summary>
        public bool Border
        {
            get => _Border;
            set
            {
                if (_Border == value) return;

                bool oldValue = _Border;
                _Border = value;

                OnBorderChanged(oldValue, value);
            }
        }
        private bool _Border;
        public event EventHandler<EventArgs> BorderChanged;
        /// <summary>
        /// 启用 边界 改变
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public virtual void OnBorderChanged(bool oValue, bool nValue)
        {
            UpdateBorderInformation();
            BorderChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region BorderColour
        /// <summary>
        /// 边框颜色
        /// </summary>
        public Color BorderColour
        {
            get => _BorderColour;
            set
            {
                if (_BorderColour == value) return;

                Color oldValue = _BorderColour;
                _BorderColour = value;

                OnBorderColourChanged(oldValue, value);
            }
        }
        private Color _BorderColour;
        public event EventHandler<EventArgs> BorderColourChanged;
        /// <summary>
        /// 启用 边框颜色 改变
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public virtual void OnBorderColourChanged(Color oValue, Color nValue)
        {
            BorderColourChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region BorderInformation
        /// <summary>
        /// 边界信息
        /// </summary>
        public Vector2[] BorderInformation
        {
            get => _BorderInformation;
            set
            {
                if (_BorderInformation == value) return;

                Vector2[] oldValue = _BorderInformation;
                _BorderInformation = value;

                OnBorderInformationChanged(oldValue, value);
            }
        }
        private Vector2[] _BorderInformation;
        public event EventHandler<EventArgs> BorderInformationChanged;
        /// <summary>
        /// 启用 边界信息 改变
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public virtual void OnBorderInformationChanged(Vector2[] oValue, Vector2[] nValue)
        {
            BorderInformationChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region BorderSize
        /// <summary>
        /// 边界大小
        /// </summary>
        public float BorderSize
        {
            get => _BorderSize;
            set
            {
                if (_BorderSize == value) return;

                float oldValue = _BorderSize;
                _BorderSize = value;

                OnBorderSizeChanged(oldValue, value);
            }
        }
        private float _BorderSize;
        public event EventHandler<EventArgs> BorderSizeChanged;
        /// <summary>
        /// 启用边界大小 改变
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public virtual void OnBorderSizeChanged(float oValue, float nValue)
        {
            BorderSizeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region CanResizeHeight
        /// <summary>
        /// 可以调整高度
        /// </summary>
        public bool CanResizeHeight
        {
            get => _CanResizeHeight;
            set
            {
                if (_CanResizeHeight == value) return;

                bool oldValue = _CanResizeHeight;
                _CanResizeHeight = value;

                OnCanResizeHeightChanged(oldValue, value);
            }
        }
        private bool _CanResizeHeight;
        public event EventHandler<EventArgs> CanResizeHeightChanged;
        /// <summary>
        /// 开启 调整高度 改变
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public virtual void OnCanResizeHeightChanged(bool oValue, bool nValue)
        {
            CanResizeHeightChanged?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 调整底部高度
        /// </summary>
        public bool CanResizeHeightBottom
        {
            get => _CanResizeHeightBottom;
            set
            {
                if (_CanResizeHeightBottom == value) return;

                bool oldValue = _CanResizeHeightBottom;
                _CanResizeHeightBottom = value;

                OnCanResizeHeightChanged(oldValue, value);
            }
        }
        private bool _CanResizeHeightBottom = true;
        /// <summary>
        /// 向右调整宽度
        /// </summary>
        public bool CanResizeWidthRight
        {
            get => _CanResizeWidthRight;
            set
            {
                if (_CanResizeWidthRight == value) return;

                bool oldValue = _CanResizeWidthRight;
                _CanResizeWidthRight = value;

                OnCanResizeHeightChanged(oldValue, value);
            }
        }
        private bool _CanResizeWidthRight = true;
        /// <summary>
        /// 向左调整宽度
        /// </summary>
        public bool CanResizeWidthLeft
        {
            get => _CanResizeWidthLeft;
            set
            {
                if (_CanResizeWidthLeft == value) return;

                bool oldValue = _CanResizeWidthLeft;
                _CanResizeWidthLeft = value;

                OnCanResizeHeightChanged(oldValue, value);
            }
        }
        private bool _CanResizeWidthLeft = true;
        /// <summary>
        /// 调整顶部高度
        /// </summary>
        public bool CanResizeHeightTop
        {
            get => _CanResizeHeightTop;
            set
            {
                if (_CanResizeHeightTop == value) return;

                bool oldValue = _CanResizeHeightTop;
                _CanResizeHeightTop = value;

                OnCanResizeHeightChanged(oldValue, value);
            }
        }
        private bool _CanResizeHeightTop = true;
        #endregion

        #region CanResizeWidth
        /// <summary>
        /// 调整宽度大小
        /// </summary>
        public bool CanResizeWidth
        {
            get => _CanResizeWidth;
            set
            {
                if (_CanResizeWidth == value) return;

                bool oldValue = _CanResizeWidth;
                _CanResizeWidth = value;

                OnCanResizeWidthChanged(oldValue, value);
            }
        }
        private bool _CanResizeWidth;
        public event EventHandler<EventArgs> CanResizeWidthChanged;
        public virtual void OnCanResizeWidthChanged(bool oValue, bool nValue)
        {
            CanResizeWidthChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region DrawTexture
        /// <summary>
        /// 绘制纹理
        /// </summary>
        public bool DrawTexture
        {
            get => _DrawTexture;
            set
            {
                if (_DrawTexture == value) return;

                bool oldValue = _DrawTexture;
                _DrawTexture = value;

                OnDrawTextureChanged(oldValue, value);
            }
        }
        private bool _DrawTexture;
        public event EventHandler<EventArgs> DrawTextureChanged;

        public virtual void OnDrawTextureChanged(bool oValue, bool nValue)
        {
            TextureValid = false;
            DrawTextureChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region DisplayArea
        /// <summary>
        /// 矩形显示去
        /// </summary>
        public Rectangle DisplayArea
        {
            get => _DisplayArea;
            set
            {
                if (_DisplayArea == value) return;

                Rectangle oldValue = _DisplayArea;
                _DisplayArea = value;

                OnDisplayAreaChanged(oldValue, value);
            }
        }
        private Rectangle _DisplayArea;
        public event EventHandler<EventArgs> DisplayAreaChanged;

        public virtual void OnDisplayAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            foreach (DXControl control in Controls)
                control.UpdateDisplayArea();

            UpdateBorderInformation();
            DisplayAreaChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Enabled
        /// <summary>
        /// 启用 实现
        /// </summary>
        public bool Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled == value) return;

                bool oldValue = _Enabled;
                _Enabled = value;

                OnEnabledChanged(oldValue, value);
            }
        }
        private bool _Enabled;
        public event EventHandler<EventArgs> EnabledChanged;

        public virtual void OnEnabledChanged(bool oValue, bool nValue)
        {
            CheckIsEnabled();
            EnabledChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region ForeColour
        /// <summary>
        /// 前景色
        /// </summary>
        public Color ForeColour
        {
            get => _ForeColour;
            set
            {
                if (_ForeColour == value) return;

                Color oldValue = _ForeColour;
                _ForeColour = value;

                OnForeColourChanged(oldValue, value);
            }
        }
        private Color _ForeColour;
        public event EventHandler<EventArgs> ForeColourChanged;

        public virtual void OnForeColourChanged(Color oValue, Color nValue)
        {
            ForeColourChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Hint
        /// <summary>
        /// 提示标签
        /// </summary>
        public string Hint
        {
            get => _Hint;
            set
            {
                if (_Hint == value) return;

                string oldValue = _Hint;
                _Hint = value;

                OnHintChanged(oldValue, value);
            }
        }
        private string _Hint;
        public event EventHandler<EventArgs> HintChanged;

        public virtual void OnHintChanged(string oValue, string nValue)
        {
            HintChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region IsControl
        /// <summary>
        /// 已控制
        /// </summary>
        public bool IsControl
        {
            get => _IsControl;
            set
            {
                if (_IsControl == value) return;

                bool oldValue = _IsControl;
                _IsControl = value;

                OnIsControlChanged(oldValue, value);
            }
        }
        private bool _IsControl;
        public event EventHandler<EventArgs> IsControlChanged;

        public virtual void OnIsControlChanged(bool oValue, bool nValue)
        {
            if (!IsControl)
            {
                if (FocusControl == this)
                    FocusControl = null;

                if (MouseControl == this)
                    MouseControl = null;
            }

            IsControlChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Location
        /// <summary>
        /// 位置
        /// </summary>
        public Point Location
        {
            get => _Location;
            set
            {
                if (_Location == value) return;

                Point oldValue = _Location;
                _Location = value;

                OnLocationChanged(oldValue, value);
            }
        }
        private Point _Location;
        public event EventHandler<EventArgs> LocationChanged;

        public virtual void OnLocationChanged(Point oValue, Point nValue)
        {
            UpdateDisplayArea();
            LocationChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Modal
        /// <summary>
        /// 形态
        /// </summary>
        public bool Modal
        {
            get => _Modal;
            set
            {
                if (_Modal == value) return;

                bool oldValue = _Modal;
                _Modal = value;

                OnModalChanged(oldValue, value);
            }
        }
        private bool _Modal;
        public event EventHandler<EventArgs> ModalChanged;

        public virtual void OnModalChanged(bool oValue, bool nValue)
        {
            ModalChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Movable
        /// <summary>
        /// 可移动
        /// </summary>
        public bool Movable
        {
            get => _Movable;
            set
            {
                if (_Movable == value) return;

                bool oldValue = _Movable;
                _Movable = value;

                OnMovableChanged(oldValue, value);
            }
        }
        private bool _Movable;
        public event EventHandler<EventArgs> MovableChanged;

        public virtual void OnMovableChanged(bool oValue, bool nValue)
        {
            MovableChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region IgnoreMoveBounds
        /// <summary>
        /// 忽略移动边界
        /// </summary>
        public bool IgnoreMoveBounds
        {
            get => _IgnoreMoveBounds;
            set
            {
                if (_IgnoreMoveBounds == value) return;

                bool oldValue = _IgnoreMoveBounds;
                _IgnoreMoveBounds = value;

                OnIgnoreMoveBoundsChanged(oldValue, value);
            }
        }
        private bool _IgnoreMoveBounds;
        public event EventHandler<EventArgs> IgnoreMoveBoundsChanged;
        public virtual void OnIgnoreMoveBoundsChanged(bool oValue, bool nValue)
        {
            IgnoreMoveBoundsChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Opacity
        /// <summary>
        /// 透明度
        /// </summary>
        public float Opacity
        {
            get => _Opacity;
            set
            {
                if (_Opacity == value) return;

                float oldValue = _Opacity;
                _Opacity = value;

                OnOpacityChanged(oldValue, value);
            }
        }
        private float _Opacity;
        public event EventHandler<EventArgs> OpacityChanged;
        public virtual void OnOpacityChanged(float oValue, float nValue)
        {
            OpacityChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Parent
        /// <summary>
        /// 来源
        /// </summary>
        public DXControl Parent
        {
            get => _Parent;
            set
            {
                if (_Parent == value) return;

                DXControl oldValue = _Parent;
                _Parent = value;

                OnParentChanged(oldValue, value);
            }
        }
        private DXControl _Parent;
        public event EventHandler<EventArgs> ParentChanged;
        public virtual void OnParentChanged(DXControl oValue, DXControl nValue)
        {
            oValue?.Controls.Remove(this);
            Parent?.Controls.Add(this);

            CheckIsVisible();
            CheckIsEnabled();

            UpdateDisplayArea();

            ParentChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region PassThrough
        /// <summary>
        /// 穿透
        /// </summary>
        public bool PassThrough
        {
            get => _PassThrough;
            set
            {
                if (_PassThrough == value) return;

                bool oldValue = _PassThrough;
                _PassThrough = value;

                OnPassThroughChanged(oldValue, value);
            }
        }
        private bool _PassThrough;
        public event EventHandler<EventArgs> PassThroughChanged;
        public virtual void OnPassThroughChanged(bool oValue, bool nValue)
        {
            PassThroughChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Size
        /// <summary>
        /// 大小
        /// </summary>
        public virtual Size Size
        {
            get => _Size;
            set
            {
                if (_Size == value) return;

                Size oldValue = _Size;
                _Size = value;

                OnSizeChanged(oldValue, value);
            }
        }
        private Size _Size;
        public event EventHandler<EventArgs> SizeChanged;
        public virtual void OnSizeChanged(Size oValue, Size nValue)
        {
            UpdateDisplayArea();
            UpdateBorderInformation();
            TextureValid = false;

            SizeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Sort
        /// <summary>
        /// 分类 排序
        /// </summary>
        public bool Sort
        {
            get => _Sort;
            set
            {
                if (_Sort == value) return;

                bool oldValue = _Sort;
                _Sort = value;

                OnSortChanged(oldValue, value);
            }
        }
        private bool _Sort;
        public event EventHandler<EventArgs> SortChanged;
        public virtual void OnSortChanged(bool oValue, bool nValue)
        {
            BringToFront();
            SortChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Sound
        /// <summary>
        /// 音源
        /// </summary>
        public SoundIndex Sound
        {
            get => _Sound;
            set
            {
                if (_Sound == value) return;

                SoundIndex oldValue = _Sound;
                _Sound = value;

                OnSoundChanged(oldValue, value);
            }
        }
        private SoundIndex _Sound;
        public event EventHandler<EventArgs> SoundChanged;
        public virtual void OnSoundChanged(SoundIndex oValue, SoundIndex nValue)
        {
            SoundChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Tag
        /// <summary>
        /// 标签
        /// </summary>
        public object Tag
        {
            get => _Tag;
            set
            {
                if (_Tag == value) return;

                object oldValue = _Tag;
                _Tag = value;

                OnTagChanged(oldValue, value);
            }
        }
        private object _Tag;
        public event EventHandler<EventArgs> TagChanged;
        public virtual void OnTagChanged(object oValue, object nValue)
        {
            TagChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Tag_0

        public object Tag_0
        {
            get => _Tag_0;
            set
            {
                if (_Tag_0 == value) return;

                object oldValue = _Tag_0;
                _Tag_0 = value;

                OnTagChanged(oldValue, value);
            }
        }
        private object _Tag_0;
        public event EventHandler<EventArgs> Tag_0_Changed;
        public virtual void OnTag_0_Changed(object oValue, object nValue)
        {
            Tag_0_Changed?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Text
        /// <summary>
        /// 文本
        /// </summary>
        public string Text
        {
            get => _Text;
            set
            {
                if (_Text == value) return;

                string oldValue = _Text;
                _Text = value;

                OnTextChanged(oldValue, value);
            }
        }
        private string _Text;
        public event EventHandler<EventArgs> TextChanged;
        public virtual void OnTextChanged(string oValue, string nValue)
        {
            TextChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Visible
        /// <summary>
        /// 显示
        /// </summary>
        public bool Visible
        {
            get => _Visible;
            set
            {
                if (_Visible == value) return;

                bool oldValue = _Visible;
                _Visible = value;

                OnVisibleChanged(oldValue, value);
            }
        }
        private bool _Visible;
        public event EventHandler<EventArgs> VisibleChanged;
        public virtual void OnVisibleChanged(bool oValue, bool nValue)
        {
            CheckIsVisible();

            VisibleChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region IsEnabled
        /// <summary>
        /// 已启用
        /// </summary>
        public bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                if (_IsEnabled == value) return;

                bool oldValue = _IsEnabled;
                _IsEnabled = value;

                OnIsEnabledChanged(oldValue, value);
            }
        }
        private bool _IsEnabled;
        public event EventHandler<EventArgs> IsEnabledChanged;
        public virtual void OnIsEnabledChanged(bool oValue, bool nValue)
        {
            foreach (DXControl control in Controls)
                control.CheckIsEnabled();

            IsEnabledChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region IsVisible
        /// <summary>
        /// 已显示
        /// </summary>
        public bool IsVisible
        {
            get => _IsVisible;
            set
            {
                if (_IsVisible == value) return;

                bool oldValue = _IsVisible;
                _IsVisible = value;

                OnIsVisibleChanged(oldValue, value);
            }
        }
        private bool _IsVisible;
        public event EventHandler<EventArgs> IsVisibleChanged;
        public virtual void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            if (!IsVisible)
            {
                if (FocusControl == this)
                    FocusControl = null;

                if (MouseControl == this)
                    MouseControl = null;
            }

            List<DXControl> checks = new List<DXControl>(Controls);

            foreach (DXControl control in checks)
                control.CheckIsVisible();

            IsVisibleChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region IsMoving
        /// <summary>
        /// 可移动
        /// </summary>
        public bool IsMoving
        {
            get => _IsMoving;
            set
            {
                if (_IsMoving == value) return;

                bool oldValue = _IsMoving;
                _IsMoving = value;

                OnIsMovingChanged(oldValue, value);
            }
        }
        private bool _IsMoving;
        public event EventHandler<EventArgs> IsMovingChanged;
        public virtual void OnIsMovingChanged(bool oValue, bool nValue)
        {
            //if (IsMoving)
            //    CEnvir.Target.SuspendLayout();
            //else
            //    CEnvir.Target.ResumeLayout();

            IsMovingChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region IsResizing
        /// <summary>
        /// 正在调整大小
        /// </summary>
        public bool IsResizing
        {
            get => _IsResizing;
            set
            {
                if (_IsResizing == value) return;

                bool oldValue = _IsResizing;
                _IsResizing = value;

                OnIsResizingChanged(oldValue, value);
            }
        }
        private bool _IsResizing;
        public event EventHandler<EventArgs> IsResizingChanged;
        public virtual void OnIsResizingChanged(bool oValue, bool nValue)
        {
            IsResizingChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        public const int ResizeBuffer = 9;
        protected internal Point MovePoint;
        private Point ResizePoint;
        public bool ResizeLeft, ResizeRight, ResizeUp, ResizeDown;
        public bool CanResizeLeft, CanResizeRight, CanResizeUp, CanResizeDown;

        #region Texture
        /// <summary>
        /// 纹理有效
        /// </summary>
        public bool TextureValid { get; set; }
        /// <summary>
        /// 控制纹理
        /// </summary>
        public Texture ControlTexture { get; set; }
        /// <summary>
        /// 纹理大小
        /// </summary>
        public Size TextureSize { get; set; }
        /// <summary>
        /// 控制界面
        /// </summary>
        public Surface ControlSurface { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireTime { get; protected set; }
        /// <summary>
        /// 创建纹理
        /// </summary>
        protected virtual void CreateTexture()
        {
            if (DisplayArea.Size.Width <= 0 || DisplayArea.Size.Height <= 0) return;
            if (ControlTexture == null || DisplayArea.Size != TextureSize)
            {
                DisposeTexture();
                TextureSize = DisplayArea.Size;
                ControlTexture = new Texture(DXManager.Device, (int)Math.Round(TextureSize.Width * ZoomRate), (int)Math.Round(TextureSize.Height * ZoomRate), 1, Usage.RenderTarget, SurfaceFormat.Color, Pool.Default);
                ControlSurface = ControlTexture.GetSurfaceLevel(0);
                DXManager.ControlList.Add(this);
            }

            Surface previous = DXManager.CurrentSurface;
            DXManager.SetSurface(ControlSurface);

            DXManager.Device.Clear(ClearFlags.Target, BackColour, 0, 0);

            OnClearTexture();

            DXManager.SetSurface(previous);
            TextureValid = true;
            ExpireTime = CEnvir.Now + Config.CacheDuration;
        }
        /// <summary>
        /// 纹理清理
        /// </summary>
        protected virtual void OnClearTexture()
        {
        }
        /// <summary>
        /// 处理纹理结构
        /// </summary>
        public virtual void DisposeTexture()
        {
            if (ControlTexture != null)
            {
                if (!ControlTexture.Disposed)
                    ControlTexture.Dispose();

                ControlTexture = null;
            }

            if (ControlSurface != null)
            {
                if (!ControlSurface.Disposed)
                    ControlSurface.Dispose();

                ControlSurface = null;
            }

            TextureSize = Size.Empty;
            ExpireTime = DateTime.MinValue;
            TextureValid = false;

            DXManager.ControlList.Remove(this);
        }
        #endregion

        /// <summary>
        /// 事件处理 事件参数
        /// MouseEnter 鼠标进入
        /// MouseLeave 鼠标离开
        /// Focus 鼠标焦点
        /// LostFocus  鼠标离开焦点
        /// </summary>
        public event EventHandler<EventArgs> MouseEnter, MouseLeave, Focus, LostFocus;
        /// <summary>
        /// 事件处理 鼠标事件参数
        /// MouseDown 鼠标向下点击 
        /// MouseUp 鼠标松开
        /// MouseMove 鼠标移动
        /// Moving 移动
        /// MouseClick 鼠标点击 
        /// MouseDoubleClick 鼠标双击
        /// MouseWheel 鼠标滚轮
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseDown, MouseUp, MouseMove, Moving, MouseClick, MouseDoubleClick, MouseWheel;
        /// <summary>
        /// 事件处理 按键事件参数 
        /// KeyDown 按下键
        /// KeyUp 松开键
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyDown, KeyUp;
        /// <summary>
        /// 事件处理 按下键事件参数
        /// 按下
        /// </summary>
        public event EventHandler<KeyPressEventArgs> KeyPress;

        public event EventHandler<TouchEventArgs> TouchDown, TouchUp, TouchMoved, TouchMoving, FreeDrag, DragComplete, DoubleTap, TouchHold, Tap;

        public Action ProcessAction;
        public long _Cid;
        #endregion

        /// <summary>
        /// 控制控件
        /// </summary>
        public DXControl()
        {
            ZoomRate = CEnvir.UIScale;
            UI_Offset_X = CEnvir.UI_Offset_X;
            _Cid = _ControlCounter++;   //控制计数器
            BackColour = Color.Empty;   //背景色空白
            Enabled = true;             //启用开启
            IsControl = true;           //控制开启
            Opacity = 1F;               //透明度
            BorderSize = 1;             //边框大小
            Visible = true;             //可见
            ForeColour = Color.White;   //前景色白色
            CanResizeHeight = true;     //可以调整高度大小
            CanResizeWidth = true;      //可以调整宽度大小
            CanResizeLeft = true;       //可以向左调整大小
            CanResizeRight = true;      //可以向右调整大小
            CanResizeUp = true;         //可以向上调整大小
            CanResizeDown = true;       //可以向下调整大小
        }

        #region Methods
        public virtual void Process()  //过程
        {
            ProcessAction?.Invoke();  //处理动作？ 调用

            for (var i = 0; i < Controls.Count; i++)
            {
                var control = Controls[i];
                if (!control.IsVisible) continue;  //如果控制不可见 继续

                control.Process();  //控制过程
            }
            //foreach (DXControl control in Controls)
            //{
            //    if (!control.IsVisible) continue;  //如果控制不可见 继续

            //    control.Process();  //控制过程
            //}
        }

        /// <summary>
        /// 更新边框信息
        /// </summary>
        protected internal virtual void UpdateBorderInformation()
        {
            BorderInformation = null;   //边框信息等零

            //边框  显示区域 宽度    显示区域高度
            if (!Border || DisplayArea.Width == 0 || DisplayArea.Height == 0) return;
            /*
            BorderInformation = new[]
            {
                new Vector2(DisplayArea.Left - 1, DisplayArea.Top - 1),
                new Vector2(DisplayArea.Right, DisplayArea.Top - 1),
                new Vector2(DisplayArea.Right, DisplayArea.Bottom),
                new Vector2(DisplayArea.Left - 1, DisplayArea.Bottom),
                new Vector2(DisplayArea.Left - 1, DisplayArea.Top - 1)
            };
            */

            BorderInformation = new[]
            {
                new Vector2(0, 0),
                new Vector2(Size.Width + 1, 0),
                new Vector2(Size.Width + 1, Size.Height + 1),
                new Vector2(0, Size.Height + 1),
                new Vector2(0, 0)
            };
        }
        /// <summary>
        /// 可见度改变
        /// </summary>
        protected internal virtual void CheckIsVisible()
        {
            IsVisible = Visible && Parent != null && Parent.IsVisible;
        }
        /// <summary>
        /// 启用改变
        /// </summary>
        protected internal virtual void CheckIsEnabled()
        {
            IsEnabled = Enabled && (Parent == null || Parent.IsEnabled);
        }
        /// <summary>
        /// 更新显示区域
        /// </summary>
        protected internal virtual void UpdateDisplayArea()
        {
            Rectangle area = new Rectangle(Location, Size);  //矩形区域=新矩形（位置 大小）

            if (Parent != null)
                area.Offset(Parent.DisplayArea.Location);

            DisplayArea = area;
        }
        /// <summary>
        /// 分辨率改变
        /// </summary>
        public virtual void ResolutionChanged() { }
        /// <summary>
        /// 分类排序
        /// </summary>
        public virtual void OnSorted() { }

        //将该节点的显示移到同级节点的最前面
        /// <summary>
        /// 顶部显示
        /// </summary>
        public void TopShow_Parent()
        {
            if (Parent.Controls.Count < 1) return;
            if (Parent.Controls[Parent.Controls.Count - 1] == this) return;
            Parent.Controls.Remove(this);
            Parent.Controls.Add(this);
            OnSorted();
        }
        /// <summary>
        /// 按钮显示
        /// </summary>
        public void BottomShow_Parent()
        {
            if (Parent.Controls.Count < 1) return;
            if (Parent.Controls[0] == this) return;
            Parent.Controls.Remove(this);
            Parent.Controls.Insert(0, this);
            OnSorted();
        }
        /// <summary>
        /// 置前
        /// </summary>
        public void BringToFront()
        {
            if (Parent == null) return;

            Parent.BringToFront();

            if (!Sort || Parent.Controls[Parent.Controls.Count - 1] == this) return;

            Parent.Controls.Remove(this);
            Parent.Controls.Add(this);

            OnSorted();
        }
        /// <summary>
        /// 放后面
        /// </summary>
        public void SendToBack()
        {
            if (Parent == null) return;

            Parent.SendToBack();

            if (!Sort || Parent.Controls[0] == this) return;

            Parent.Controls.Remove(this);
            Parent.Controls.Insert(0, this);
        }
        /// <summary>
        /// 调用鼠标单击
        /// </summary>
        public void InvokeMouseClick()
        {
            if (!IsEnabled) return;

            MouseClick?.Invoke(this, null);
        }
        /// <summary>
        /// 鼠标悬停
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public virtual bool IsMouseOver(Point p)
        {
            if (!IsVisible || !IsControl) return false;

            Point point = ZoomRate == 1F ? p : new Point((int)Math.Round((p.X - UI_Offset_X) / ZoomRate), (int)Math.Round(p.Y / ZoomRate));
            if (!DisplayArea.Contains(point)) return Modal;

            if (!PassThrough) return true;

            if (AllowResize)  //允许调整大小
            {
                bool left = false, right = false, top = false, bottom = false;
                if (CanResizeWidth)   //可以调整宽度
                {
                    if (point.X - DisplayArea.Left < ResizeBuffer)
                        left = CanResizeWidthLeft;
                    else if (DisplayArea.Right - point.X < ResizeBuffer)
                        right = CanResizeWidthRight;
                }

                if (CanResizeHeight)  //可以调整高度
                {
                    if (point.Y - DisplayArea.Top < ResizeBuffer)
                        top = CanResizeHeightTop;
                    else if (DisplayArea.Bottom - point.Y < ResizeBuffer)
                        bottom = CanResizeHeightBottom;
                }

                if (left || right || top || bottom) return true;
            }

            for (int i = Controls.Count - 1; i >= 0; i--)
                if (Controls[i].IsMouseOver(p))
                    return true;

            return false;
        }
        /// <summary>
        /// 鼠标进入时
        /// </summary>
        public virtual void OnMouseEnter()
        {
            if (!IsEnabled) return;

            MouseEnter?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 鼠标离开时
        /// </summary>
        public virtual void OnMouseLeave()
        {
            if (!IsEnabled) return;

            MouseLeave?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 鼠标移动时
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnMouseMove(MouseEventArgs e)
        {
            if (!IsEnabled)   //如果没启用
            {
                MouseControl = this;  //鼠标控制
                return;
            }

            Point location = ZoomRate == 1F ? e.Location : new Point((int)Math.Round((e.Location.X - UI_Offset_X) / ZoomRate), (int)Math.Round(e.Location.Y / ZoomRate));
            bool left = false, right = false, top = false, bottom = false;
            if (IsMoving)  //正在移动
            {
                Point tempPoint = new Point(location.X - MovePoint.X, location.Y - MovePoint.Y);

                if (!AllowDragOut && !IgnoreMoveBounds)  //如果不允许拖动 并且 不忽略移动边界
                {
                    if (Parent == null) return;

                    tempPoint = GetBoundsPoint(tempPoint);
                }

                //在这里修剪
                if (Tag is Size)  //标签大小
                {
                    Size clipSize = (Size)Tag;
                    Point change = new Point(tempPoint.X - Location.X, tempPoint.Y - Location.Y);

                    if (DisplayArea.X + change.X < ActiveScene.Location.X) tempPoint.X -= DisplayArea.X + change.X - ActiveScene.Location.X;
                    if (DisplayArea.Y + change.Y < ActiveScene.Location.Y) tempPoint.Y -= DisplayArea.Y + change.Y - ActiveScene.Location.Y;

                    if (DisplayArea.X + clipSize.Width + change.X - ActiveScene.Location.X >= ActiveScene.DisplayArea.Width) tempPoint.X -= DisplayArea.X + clipSize.Width + change.X - ActiveScene.Location.X - ActiveScene.DisplayArea.Width;
                    if (DisplayArea.Y + clipSize.Height + change.Y - ActiveScene.Location.Y >= ActiveScene.DisplayArea.Height) tempPoint.Y -= DisplayArea.Y + clipSize.Height + change.Y - ActiveScene.Location.Y - ActiveScene.DisplayArea.Height;
                }

                Location = tempPoint;
                Moving?.Invoke(this, e);
            }

            if (!IsMoving && !IsResizing && !left && !right && !top && !bottom)
                for (int i = Controls.Count - 1; i >= 0; i--)
                    if (Controls[i].IsMouseOver(e.Location))
                    {
                        Controls[i].OnMouseMove(e);
                        return;
                    }

            MouseControl = this;

            MouseMove?.Invoke(this, e);
        }
        /// <summary>
        /// 获取边界点
        /// </summary>
        /// <param name="tempPoint"></param>
        /// <returns></returns>
        public virtual Point GetBoundsPoint(Point tempPoint)
        {
            if (tempPoint.X + DisplayArea.Width > Parent.DisplayArea.Width) tempPoint.X = Parent.DisplayArea.Width - DisplayArea.Width;
            if (tempPoint.Y + DisplayArea.Height > Parent.DisplayArea.Height) tempPoint.Y = Parent.DisplayArea.Height - DisplayArea.Height;

            if (tempPoint.X < 0) tempPoint.X = 0;
            if (tempPoint.Y < 0) tempPoint.Y = 0;

            return tempPoint;
        }
        /// <summary>
        /// 获得可接受的大小调整
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public virtual Size GetAcceptableResize(Size size)
        {
            return size;
        }
        /// <summary>
        /// 鼠标按下时
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnMouseDown(MouseEventArgs e)
        {
            if (!IsEnabled) return;

            FocusControl = this;

            BringToFront();

            Point location = ZoomRate == 1F ? e.Location : new Point((int)Math.Round((e.Location.X - UI_Offset_X) / ZoomRate), (int)Math.Round(e.Location.Y / ZoomRate));
            if (!IsResizing && Movable && e.Button.HasFlag(MouseButtons.Left) && (!Modal || DisplayArea.Contains(location)))
            {
                IsMoving = true;
                MovePoint = new Point(location.X - Location.X, location.Y - Location.Y);
                Parent.Controls.Remove(this);
                Parent.Controls.Add(this);
            }

            MouseDown?.Invoke(this, e);
        }
        /// <summary>
        /// 鼠标向上移动
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnMouseUp(MouseEventArgs e)
        {
            if (!IsEnabled) return;

            FocusControl = null;

            MouseUp?.Invoke(this, e);
        }
        /// <summary>
        /// 鼠标单击时
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnMouseClick(MouseEventArgs e)
        {
            if (!IsEnabled) return;

            if (Sound != SoundIndex.None)
                DXSoundManager.Play(Sound);

            MouseClick?.Invoke(this, e);
        }
        /// <summary>
        /// 鼠标双击时
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (!IsEnabled) return;


            if (MouseDoubleClick != null)
            {
                if (Sound != SoundIndex.None)
                    DXSoundManager.Play(Sound);

                MouseDoubleClick?.Invoke(this, e);
            }
            else
                OnMouseClick(e);
        }
        /// <summary>
        /// 鼠标滚轮上
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnMouseWheel(MouseEventArgs e)
        {
            if (!IsEnabled) return;

            MouseWheel?.Invoke(this, e);
        }
        /// <summary>
        /// 聚焦 焦点
        /// </summary>
        public virtual void OnFocus()
        {
            IsMoving = false;
            ResizePoint = Point.Empty;
            MovePoint = Point.Empty;

            Focus?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 失去焦点
        /// </summary>
        public virtual void OnLostFocus()
        {
            if (IsMoving)
            {
                IsMoving = false;
                MovePoint = Point.Empty;
            }

            if (IsResizing)
            {
                IsResizing = false;
                ResizeLeft = false;
                ResizeRight = false;
                ResizeUp = false;
                ResizeDown = false;
                ResizePoint = Point.Empty;
            }

            LostFocus?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 按键时
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnKeyPress(KeyPressEventArgs e)
        {
            if (!IsEnabled) return;

            if (Controls != null)
                for (int i = Controls.Count - 1; i >= 0; i--)
                {
                    if (!Controls[i].IsVisible) continue;

                    Controls[i].OnKeyPress(e);
                    if (e.Handled || Modal) return;
                }

            KeyPress?.Invoke(this, e);
        }
        /// <summary>
        /// 按下按键时
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnKeyDown(KeyEventArgs e)
        {
            if (!IsEnabled) return;

            if (Controls != null)
                for (int i = Controls.Count - 1; i >= 0; i--)
                {
                    if (!Controls[i].IsVisible) continue;

                    Controls[i].OnKeyDown(e);
                    if (e.Handled || Modal) return;
                }

            KeyDown?.Invoke(this, e);
        }
        /// <summary>
        /// 松开按键时
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnKeyUp(KeyEventArgs e)
        {
            if (!IsEnabled) return;

            if (Controls != null)
                for (int i = Controls.Count - 1; i >= 0; i--)
                {
                    if (!Controls[i].IsVisible) continue;

                    Controls[i].OnKeyUp(e);
                    if (e.Handled || Modal) return;
                }

            KeyUp?.Invoke(this, e);
        }

        /// <summary>
        /// 触摸按下时 等同OnMouseDown
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnTouchDown(TouchEventArgs e)
        {
            if (!IsEnabled)
            {
                MouseControl = this;
                return;
            }

            if (!IsMoving && !IsResizing)
            {
                for (int num = Controls.Count - 1; num >= 0; num--)
                {
                    if (Controls[num].IsMouseOver(e.Location))
                    {
                        Controls[num].OnTouchDown(e);
                        return;
                    }
                }
            }
            MouseControl = this;
            FocusControl = this;
            BringToFront();
            Point location = ZoomRate == 1F ? e.Location : new Point((int)Math.Round((e.Location.X - UI_Offset_X) / ZoomRate), (int)Math.Round(e.Location.Y / ZoomRate));
            if (!IsResizing && Movable && (!Modal || DisplayArea.Contains(location)))
            {
                IsMoving = true;
                MovePoint = new Point(location.X - Location.X, location.Y - Location.Y);
                Parent.Controls.Remove(this);
                Parent.Controls.Add(this);
            }
            this.TouchDown?.Invoke(this, e);
        }
        /// <summary>
        /// 触摸移动时 等同OnMouseMove
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnTouchMoved(TouchEventArgs e)
        {
            if (!IsEnabled)
            {
                return;
            }
            if (!Movable)
            {
                this.TouchMoving?.Invoke(this, e);
                return;
            }

            if (!IsMoving && !IsResizing)
            {
                for (int num = Controls.Count - 1; num >= 0; num--)
                {
                    if (Controls[num].IsMouseOver(e.Location))
                    {
                        Controls[num].OnTouchMoved(e);
                        return;
                    }
                }
            }
            MouseControl = this;
            this.TouchMoved?.Invoke(this, e);
        }

        /// <summary>
        /// 触摸抬手时 等同OnMouseUP
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnTouchUp(TouchEventArgs e)
        {
            if (IsEnabled)
            {
                FocusControl = null;
                if (Sound != 0)
                {
                    DXSoundManager.Play(Sound);
                }

                this.TouchUp?.Invoke(this, e);
            }
        }
        /// <summary>
        /// 触摸单击时 等同OnMouseClick
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnTap(TouchEventArgs e)
        {
            if (IsEnabled)
            {
                if (this.Tap == null)
                {
                    this.MouseClick?.Invoke(this, new MouseEventArgs(MouseButtons.Left, 1, e.Location.X, e.Location.Y, 1));
                    return;
                }
                this.Tap?.Invoke(this, e);
            }
        }

        public virtual void OnFreeDrag(TouchEventArgs e)
        {
            if (IsEnabled)
            {
                this.FreeDrag?.Invoke(this, e);
            }
        }

        public virtual void OnDragComplete(TouchEventArgs e)
        {
            if (IsEnabled)
            {
                this.DragComplete?.Invoke(this, e);
            }
        }
        /// <summary>
        /// 触摸双击时 等同OnMouseDoubleClick
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnDoubleTap(TouchEventArgs e)
        {
            if (IsEnabled)
            {
                if (this.DoubleTap == null)
                {
                    this.MouseDoubleClick?.Invoke(this, new MouseEventArgs(MouseButtons.Left, 1, e.Location.X, e.Location.Y, 1));
                    return;
                }
                this.DoubleTap?.Invoke(this, e);
            }
        }

        public virtual void OnCheckHold(TouchEventArgs e)
        {
            if (IsEnabled)
            {
                this.TouchHold?.Invoke(this, e);
            }
        }

        #region Drawing
        /// <summary>
        /// 绘制前
        /// </summary>
        public event EventHandler<EventArgs> BeforeDraw;
        /// <summary>
        /// 绘制后
        /// </summary>
        public event EventHandler<EventArgs> AfterDraw;
        /// <summary>
        /// 子控件绘制前
        /// </summary>
        public event EventHandler<EventArgs> BeforeChildrenDraw;
        /// <summary>
        /// 绘制
        /// </summary>
        public virtual void Draw()
        {
            if (!IsVisible || DisplayArea.Width <= 0 || DisplayArea.Height <= 0) return;

            OnBeforeDraw();
            DrawControl();
            OnBeforeChildrenDraw();
            DrawChildControls();
            DrawBorder();
            OnAfterDraw();
        }
        /// <summary>
        /// 在绘制前打开
        /// </summary>
        protected virtual void OnBeforeDraw()
        {
            BeforeDraw?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 在开始绘制前
        /// </summary>
        protected virtual void OnBeforeChildrenDraw()
        {
            BeforeChildrenDraw?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 在绘制后
        /// </summary>
        protected virtual void OnAfterDraw()
        {
            AfterDraw?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 绘制边框
        /// </summary>
        protected virtual void DrawBorder()
        {
            if (!Border || BorderInformation == null) return;

            if (DXManager.Line.Width != BorderSize)
                DXManager.Line.Width = BorderSize;

            Surface old = DXManager.CurrentSurface;
            DXManager.SetSurface(DXManager.ScratchSurface);

            DXManager.Device.Clear(ClearFlags.Target, Color.Empty, 0, 0);

            DXManager.Line.Draw(BorderInformation, BorderColour);

            DXManager.SetSurface(old);

            PresentMirImage(DXManager.ScratchTexture, Parent, Rectangle.Inflate(DisplayArea, 1, 1), Color.White, this, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
        }
        /// <summary>
        /// 绘制子控件
        /// </summary>
        protected virtual void DrawChildControls()
        {
            foreach (DXControl control in Controls)
                control.Draw();
        }
        /// <summary>
        /// 绘制控件
        /// </summary>
        protected virtual void DrawControl()
        {
            if (!DrawTexture) return;

            if (!TextureValid)
            {
                CreateTexture();

                if (!TextureValid) return;
            }

            float oldOpacity = DXManager.Opacity;

            DXManager.SetOpacity(Opacity);

            PresentControlTexture(ControlTexture, Parent, DisplayArea, IsEnabled ? Color.White : Color.FromArgb(75, 75, 75), this, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

            DXManager.SetOpacity(oldOpacity);

            ExpireTime = CEnvir.Now + Config.CacheDuration;
        }
        /// <summary>
        /// 当前纹理
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="parent"></param>
        /// <param name="displayArea"></param>
        /// <param name="colour"></param>
        /// <param name="control"></param>
        /// <param name="offX"></param>
        /// <param name="offY"></param>
        /// <param name="blend"></param>
        /// <param name="blendrate"></param>
        public static void PresentControlTexture(Texture texture, DXControl parent, Rectangle displayArea, Color colour, DXControl control, int offX = 0, int offY = 0, bool blend = false, float blendrate = 1f, float zoomRate = 1f, int uiOffsetX = 0)
        {
            //校验当前控件是否在ActiveScene的显示区域
            Rectangle bounds = ActiveScene.DisplayArea;
            Rectangle textureArea = Rectangle.Intersect(bounds, displayArea);

            //如果当前控件存在父控件，就校验是否在父控件显示区域
            if (!control.IsMoving || !control.AllowDragOut)
                while (parent != null)
                {
                    if (parent.IsMoving && parent.AllowDragOut)
                    {
                        bounds = ActiveScene.DisplayArea;
                        textureArea = Rectangle.Intersect(bounds, displayArea);
                        break;
                    }

                    bounds = parent.DisplayArea;
                    textureArea = Rectangle.Intersect(bounds, textureArea);

                    if (bounds.IntersectsWith(displayArea))
                    {
                        parent = parent.Parent;
                        continue;
                    }

                    return;
                }

            if (textureArea.IsEmpty) return;

            textureArea.Location = new Point(textureArea.X - displayArea.X, textureArea.Y - displayArea.Y);

            //默认
            Vector2 positon = new Vector2((displayArea.X + textureArea.Location.X + offX) * zoomRate + uiOffsetX, (displayArea.Y + textureArea.Location.Y + offY) * zoomRate);

            if (zoomRate != 1F)
                textureArea = new Rectangle(textureArea.X, textureArea.Y,
                                           (int)Math.Round(textureArea.Width * zoomRate), (int)Math.Round(textureArea.Height * zoomRate));

            if (blend)
            {
                bool oldBlend = DXManager.Blending;
                float oldRate = DXManager.BlendRate;
                DXManager.SetBlend(blend, blendrate);

                DXManager.Sprite.Draw(texture, textureArea, Vector2.Zero, positon, colour);

                DXManager.SetBlend(oldBlend, oldRate);
            }
            else
            {
                DXManager.Sprite.Draw(texture, textureArea, Vector2.Zero, positon, colour);
            }
        }
        /// <summary>
        /// 当前纹理
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="parent"></param>
        /// <param name="displayArea"></param>
        /// <param name="colour"></param>
        /// <param name="control"></param>
        /// <param name="offX"></param>
        /// <param name="offY"></param>
        /// <param name="blend"></param>
        /// <param name="blendrate"></param>
        public static void PresentMirImage(Texture texture, DXControl parent, Rectangle displayArea, Color colour, DXControl control, int offX = 0, int offY = 0, bool blend = false, float blendrate = 1f, float zoomRate = 1f, int uiOffsetX = 0)
        {
            //校验当前控件是否在ActiveScene的显示区域
            Rectangle bounds = ActiveScene.DisplayArea;
            Rectangle textureArea = Rectangle.Intersect(bounds, displayArea);

            //如果当前控件存在父控件，就校验是否在父控件显示区域
            if (!control.IsMoving || !control.AllowDragOut)
                while (parent != null)
                {
                    if (parent.IsMoving && parent.AllowDragOut)
                    {
                        bounds = ActiveScene.DisplayArea;
                        textureArea = Rectangle.Intersect(bounds, displayArea);
                        break;
                    }

                    bounds = parent.DisplayArea;
                    textureArea = Rectangle.Intersect(bounds, textureArea);

                    if (bounds.IntersectsWith(displayArea))
                    {
                        parent = parent.Parent;
                        continue;
                    }

                    return;
                }

            if (textureArea.IsEmpty) return;

            textureArea.Location = new Point(textureArea.X - displayArea.X, textureArea.Y - displayArea.Y);

            //if (control.GetType() == typeof(DXImageControl))
            //{
            //    var image = control as DXImageControl;
            //    //if (image != null && image.TilingMode != TilingMode.None)
            //    //{
            //    //    if (image.TilingMode == TilingMode.Horizontally || image.TilingMode == TilingMode.All)
            //    //    {
            //    //        DXManager.Device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Wrap);//横向平铺
            //    //    }
            //    //    if (image.TilingMode == TilingMode.Vertically || image.TilingMode == TilingMode.All)
            //    //    {
            //    //        DXManager.Device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Wrap);//纵向平铺
            //    //    }
            //    //}

            //    if (image?.Zoom == true)
            //    {
            //        if (image.Scale != 1F)
            //        {
            //            //DXManager.Sprite.Transform = Matrix.Scaling(new Vector3(new Vector2(image.Scale), 0F));
            //            //DXManager.Sprite.Draw(texture, new Rectangle(new Point(textureArea.Location.X+image.ScalingSize.Width, textureArea.Location.Y + image.ScalingSize.Height), image.Size), Vector3.Zero, new Vector3(displayArea.X + textureArea.Location.X + offX, displayArea.Y + textureArea.Location.Y + offY, 0), colour);
            //            //DXManager.Sprite.Transform = Matrix.Scaling(new Vector3(new Vector2(1F), 0F));

            //            DXManager.Sprite.Transform = Matrix.Transformation2D(new Vector2(displayArea.X + textureArea.Location.X + offX, displayArea.Y + textureArea.Location.Y + offY), 0F, new Vector2(image.Scale), new Vector2(0), 0, new Vector2(0, 0));
            //            DXManager.Sprite.Draw(texture, new Rectangle(new Point(0), image.Size), Vector3.Zero, new Vector3(displayArea.X + textureArea.Location.X + offX, displayArea.Y + textureArea.Location.Y + offY, 0), colour);
            //            DXManager.Sprite.Transform = Matrix.Transformation2D(new Vector2(0, 0), 0F, new Vector2(1F), new Vector2(0), 0, new Vector2(0, 0));
            //            return;
            //        }
            //    }
            //}

            //默认
            Vector2 positon = new Vector2(displayArea.X + textureArea.Location.X + offX, displayArea.Y + textureArea.Location.Y + offY);

            if (zoomRate != 1F)
                positon = new Vector2((displayArea.X + textureArea.Location.X + offX) * zoomRate + uiOffsetX, (displayArea.Y + textureArea.Location.Y + offY) * zoomRate);

            if (blend)
            {
                bool oldBlend = DXManager.Blending;
                float oldRate = DXManager.BlendRate;
                DXManager.SetBlend(blend, blendrate);

                if (zoomRate != 1F)
                    DXManager.Sprite.DrawZoom(texture, textureArea, Vector2.Zero, positon, colour, zoomRate);
                else
                    DXManager.Sprite.Draw(texture, textureArea, Vector2.Zero, positon, colour);

                DXManager.SetBlend(oldBlend, oldRate);
            }
            else
            {
                if (zoomRate != 1F)
                    DXManager.Sprite.DrawZoom(texture, textureArea, Vector2.Zero, positon, colour, zoomRate);
                else
                    DXManager.Sprite.Draw(texture, textureArea, Vector2.Zero, positon, colour);
            }

        }
        /// <summary>
        /// 当前纹理
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="parent"></param>
        /// <param name="displayArea"></param>
        /// <param name="colour"></param>
        /// <param name="control"></param>
        /// <param name="offX"></param>
        /// <param name="offY"></param>
        /// <param name="blend"></param>
        /// <param name="blendrate"></param>
        public static void PresentLabelTexture(Texture texture, DXControl parent, Rectangle displayArea, Color colour, DXControl control, int offX = 0, int offY = 0, bool blend = false, float blendrate = 1f, float zoomRate = 1f, int uiOffsetX = 0)
        {
            //校验当前控件是否在ActiveScene的显示区域
            Rectangle bounds = ActiveScene.DisplayArea;
            Rectangle textureArea = Rectangle.Intersect(bounds, displayArea);

            //如果当前控件存在父控件，就校验是否在父控件显示区域
            if (!control.IsMoving || !control.AllowDragOut)
                while (parent != null)
                {
                    if (parent.IsMoving && parent.AllowDragOut)
                    {
                        bounds = ActiveScene.DisplayArea;
                        textureArea = Rectangle.Intersect(bounds, displayArea);
                        break;
                    }

                    bounds = parent.DisplayArea;
                    textureArea = Rectangle.Intersect(bounds, textureArea);

                    if (bounds.IntersectsWith(displayArea))
                    {
                        parent = parent.Parent;
                        continue;
                    }

                    return;
                }

            if (textureArea.IsEmpty) return;

            textureArea.Location = new Point(textureArea.X - displayArea.X, textureArea.Y - displayArea.Y);

            //默认
            Vector2 positon = new Vector2((displayArea.X + textureArea.Location.X + offX) * zoomRate + uiOffsetX, (displayArea.Y + textureArea.Location.Y + offY) * zoomRate);

            if (zoomRate != 1F)
                textureArea = new Rectangle(textureArea.X, textureArea.Y,
                                           (int)Math.Round(textureArea.Width * zoomRate), (int)Math.Round(textureArea.Height * zoomRate));

            if (blend)
            {
                bool oldBlend = DXManager.Blending;
                float oldRate = DXManager.BlendRate;
                DXManager.SetBlend(blend, blendrate);

                DXManager.Sprite.Draw(texture, textureArea, Vector2.Zero, positon, colour);

                DXManager.SetBlend(oldBlend, oldRate);
            }
            else
            {
                DXManager.Sprite.Draw(texture, textureArea, Vector2.Zero, positon, colour);
            }
        }
        #endregion

        #endregion

        #region IDisposable

        public event EventHandler Disposing;
        /// <summary>
        /// 已清除 已销毁
        /// </summary>
        public bool IsDisposed { get; private set; }
        /// <summary>
        /// 清除 销毁
        /// </summary>
        public void Dispose()
        {
            Dispose(!IsDisposed);
            GC.SuppressFinalize(this);
        }
        ~DXControl()
        {
            Dispose(false);
        }
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposing?.Invoke(this, EventArgs.Empty);

                IsDisposed = true;
                Disposing = null;

                #region 自动Dispose

                Type myType = this.GetType();
                FieldInfo[] fields = myType.GetFields();

                foreach (var fieldInfo in fields)
                {
                    object myAttributes = fieldInfo.GetCustomAttributes(typeof(AutoDispose), true).FirstOrDefault() as AutoDispose;
                    if (myAttributes != null)
                    {
                        if (fieldInfo.GetValue(this) is DXControl value)
                        {
                            if (!value.IsDisposed)
                            {
                                value.Dispose();
                            }
                        }
                    }
                }

                #endregion

                //Free Managed Resources
                if (Controls != null)
                {
                    while (Controls.Count > 0)
                    {
                        Controls[0].Dispose();
                    }
                    Controls = null;
                }

                _AllowDragOut = false;
                _AllowResize = false;
                _BackColour = Color.Empty;
                _Border = false;
                _BorderColour = Color.Empty;
                _BorderInformation = null;
                _BorderSize = 0;
                _CanResizeHeight = false;
                _CanResizeWidth = false;
                _DrawTexture = false;
                _DisplayArea = Rectangle.Empty;
                _Enabled = false;
                _ForeColour = Color.Empty;
                _Hint = null;
                _IsControl = false;
                _Location = Point.Empty;
                _Modal = false;
                _Movable = false;
                _IgnoreMoveBounds = false;
                _Opacity = 0F;
                _Parent?.Controls.Remove(this);
                _Parent = null;
                _PassThrough = false;
                _Size = Size.Empty;
                _Sort = false;
                _Sound = SoundIndex.None;
                _Tag = null;
                _Text = null;
                _Visible = false;

                _IsEnabled = false;
                _IsVisible = false;
                _IsMoving = false;
                _IsResizing = false;

                MovePoint = Point.Empty;
                ResizePoint = Point.Empty;
                ResizeLeft = false;
                ResizeRight = false;
                ResizeUp = false;
                ResizeDown = false;

                AllowDragOutChanged = null;
                AllowResizeChanged = null;
                BackColourChanged = null;
                BorderChanged = null;
                BorderColourChanged = null;
                BorderInformationChanged = null;
                BorderSizeChanged = null;
                CanResizeHeightChanged = null;
                CanResizeWidthChanged = null;
                DrawTextureChanged = null;
                DisplayAreaChanged = null;
                EnabledChanged = null;
                ForeColourChanged = null;
                HintChanged = null;
                IsControlChanged = null;
                LocationChanged = null;
                ModalChanged = null;
                MovableChanged = null;
                IgnoreMoveBoundsChanged = null;
                OpacityChanged = null;
                ParentChanged = null;
                PassThroughChanged = null;
                SizeChanged = null;
                SortChanged = null;
                SoundChanged = null;
                TextChanged = null;
                VisibleChanged = null;
                IsEnabledChanged = null;
                IsVisibleChanged = null;
                IsMovingChanged = null;
                IsResizingChanged = null;

                MouseEnter = null;
                MouseLeave = null;
                Focus = null;
                LostFocus = null;
                MouseDown = null;
                MouseUp = null;
                MouseMove = null;
                Moving = null;
                MouseClick = null;
                MouseDoubleClick = null;
                MouseWheel = null;
                KeyDown = null;
                KeyUp = null;
                KeyPress = null;

                BeforeDraw = null;
                BeforeChildrenDraw = null;
                AfterDraw = null;

                ProcessAction = null;
            }

            if (_MouseControl == this) _MouseControl = null;
            if (_FocusControl == this) _FocusControl = null;
            if (_ActiveScene == this) _ActiveScene = null;

            DisposeTexture();
        }
        #endregion
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class AutoDispose : Attribute
    {
        //只支持DXControl的自动Dispose
        //只支持Field
    }
}
