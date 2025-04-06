using Client.Envir;
using Library;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Drawing;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 选项卡控件
    /// </summary>
    public class DXTabControl : DXControl
    {
        #region Properties

        #region SelectedTab
        /// <summary>
        /// 选定选项卡
        /// </summary>
        public DXTab SelectedTab
        {
            get => _SelectedTab;
            set
            {
                if (_SelectedTab == value) return;

                DXTab oldValue = _SelectedTab;
                _SelectedTab = value;

                OnSelectedTabChanged(oldValue, value);
            }
        }
        private DXTab _SelectedTab;
        public event EventHandler<EventArgs> SelectedTabChanged;
        public virtual void OnSelectedTabChanged(DXTab oValue, DXTab nValue)
        {
            SelectedTabChanged?.Invoke(this, EventArgs.Empty);

            if (oValue != null && oValue.Parent == this)
                oValue.Selected = false;

            if (nValue != null)
                nValue.Selected = true;
        }
        #endregion

        public List<DXButton> TabButtons = new List<DXButton>();

        public override void OnDisplayAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnDisplayAreaChanged(oValue, nValue);

            if (Parent == null) return;

            foreach (DXControl control in Controls)
            {
                DXTab tab = control as DXTab;

                if (tab == null || tab.Updating) continue;

                control.Size = new Size(Size.Width - control.Location.X, Size.Height - control.Location.Y);
            }
            TabsChanged();
        }
        #endregion
        /// <summary>
        /// 选项卡控件
        /// </summary>
        public DXTabControl()
        {
            PassThrough = true;
        }

        #region Methods
        /// <summary>
        /// 设置新选项卡
        /// </summary>
        public void SetNewTab()
        {
            if (IsDisposed) return;

            foreach (DXControl control in Controls)
            {
                DXTab tab = control as DXTab;

                if (tab == null || tab == SelectedTab) continue;

                _SelectedTab = null;
                SelectedTab = tab;
                return;
            }

            _SelectedTab = null;
        }
        /// <summary>
        /// 选项卡改变
        /// </summary>
        public virtual void TabsChanged()
        {

            if (SelectedTab == null)
            {
                foreach (DXControl control in Controls)
                {
                    DXTab tab = control as DXTab;

                    if (tab == null || tab == SelectedTab) continue;

                    SelectedTab = tab;
                    break;
                }
            }

            int x = 0;
            int width = 0;
            foreach (DXButton control in TabButtons)
            {
                if (!control.Visible) continue;

                if (control.RightAligned)
                {
                    width += control.Size.Width + 1;
                    continue;
                }

                //control.Visible = true;
                control.Location = new Point(x, 0);
                x += control.Size.Width + 1;
            }

            foreach (DXButton control in TabButtons)
            {
                if (!control.Visible) continue;

                if (!control.RightAligned) continue;

                //    control.Visible = true;
                control.Location = new Point(Size.Width - width, 0);
                width -= control.Size.Width + 1;
            }

        }

        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _SelectedTab = null;
                SelectedTabChanged = null;

                TabButtons.Clear();
                TabButtons = null;
            }
        }
        #endregion
    }

    /// <summary>
    /// 选项卡标签
    /// </summary>
    public class DXTab : DXControl
    {
        #region Properties

        #region CurrentTabControl
        /// <summary>
        /// 当前选项卡控件
        /// </summary>
        public DXTabControl CurrentTabControl
        {
            get => _CurrentTabControl;
            set
            {
                if (_CurrentTabControl == value) return;

                DXTabControl oldValue = _CurrentTabControl;
                _CurrentTabControl = value;

                OnCurrentTabControlChanged(oldValue, value);
            }
        }
        private DXTabControl _CurrentTabControl;
        public event EventHandler<EventArgs> CurrentTabControlChanged;
        public virtual void OnCurrentTabControlChanged(DXTabControl oValue, DXTabControl nValue)
        {
            if (oValue?.SelectedTab == this)
            {
                oValue.SelectedTab = null;
                oValue.SetNewTab();
            }


            if (oValue != null && nValue != null)
                TabButton.MovePoint = new Point(TabButton.MovePoint.X - oValue.DisplayArea.X + nValue.DisplayArea.X, TabButton.MovePoint.Y - oValue.DisplayArea.Y + nValue.DisplayArea.Y);

            if (oValue != null && oValue.Controls.Count == 0)
                oValue.Dispose();

            CurrentTabControlChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region DrawOtherBorder
        /// <summary>
        /// 绘制其他边框
        /// </summary>
        public bool DrawOtherBorder
        {
            get => _DrawOtherBorder;
            set
            {
                if (_DrawOtherBorder == value) return;

                bool oldValue = _DrawOtherBorder;
                _DrawOtherBorder = value;

                OnDrawOtherBorderChanged(oldValue, value);
            }
        }
        private bool _DrawOtherBorder;
        public event EventHandler<EventArgs> DrawOtherBorderChanged;
        public virtual void OnDrawOtherBorderChanged(bool oValue, bool nValue)
        {
            DrawOtherBorderChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Selected
        /// <summary>
        /// 选择
        /// </summary>
        public bool Selected
        {
            get => _Selected;
            set
            {
                if (_Selected == value) return;

                bool oldValue = _Selected;
                _Selected = value;

                OnSelectedChanged(oldValue, value);
            }
        }
        private bool _Selected;
        public event EventHandler<EventArgs> SelectedChanged;
        public virtual void OnSelectedChanged(bool oValue, bool nValue)
        {
            if (Selected)
            {
                Visible = true;
                if (TabButton.LibraryFile != LibraryFile.None)
                {
                    TabButton.Pressed = true;
                }
                else
                {
                    TabButton.ButtonType = ButtonType.SelectedTab;
                    TabButton.Label.ForeColour = Color.White;
                }
            }
            else
            {
                Visible = false;

                if (TabButton.LibraryFile != LibraryFile.None)
                {
                    TabButton.Pressed = false;
                }
                else
                {
                    TabButton.ButtonType = ButtonType.DeselectedTab;
                    TabButton.Label.ForeColour = Color.FromArgb(198, 166, 99);
                }
            }

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        public DXButton TabButton { get; private set; }

        public float? OldOpacity { get; set; }

        public bool Updating;
        /// <summary>
        /// 显示区域已更改
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnDisplayAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnDisplayAreaChanged(oValue, nValue);

            if (Parent == null) return;

            if (IsResizing && Updating) return;
            //我不希望DXTab 固定为 选项卡+分页  因为我可能还要实现其他的子页面
            Size = new Size(nValue.Width, nValue.Height);//(Parent.Size.Width - Location.X, Parent.Size.Height - Location.Y);
        }
        /// <summary>
        /// 显示面板已更改
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnParentChanged(DXControl oValue, DXControl nValue)
        {
            base.OnParentChanged(oValue, nValue);

            DXTabControl oldTab = oValue as DXTabControl;

            if (oldTab != null)
            {
                oldTab.TabButtons.Remove(TabButton);
                oldTab.TabsChanged();
            }

            if (Parent == null)
                TabButton.Parent = null;

            if (Parent == null) return;

            Size = new Size(Parent.Size.Width - Location.X, Parent.Size.Height - Location.Y);

            DXTabControl tab = Parent as DXTabControl;

            if (tab == null) return;
            Selected = tab.SelectedTab == this;
            TabButton.Parent = tab;
            tab.Controls.Remove(TabButton);
            tab.Controls.Insert(0, TabButton);
            tab.TabButtons.Add(TabButton);
            tab.TabsChanged();

            CurrentTabControl = tab;
        }
        /// <summary>
        /// 尺寸更改时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            if (IsResizing)
            {
                if (Updating) return;
                Point location = Parent.Location;
                Size size = new Size(Parent.Size.Width - oValue.Width + nValue.Width, Parent.Size.Height - oValue.Height + nValue.Height);

                if (ResizeUp)
                    location = new Point(location.X, location.Y + oValue.Height - nValue.Height);

                if (ResizeLeft)
                    location = new Point(location.X + oValue.Width - nValue.Width, location.Y);

                Updating = true;
                Parent.Size = size;
                Parent.Location = location;
                Location = new Point(0, TabHeight - 1);
                Updating = false;
                return;
            }

            base.OnSizeChanged(oValue, nValue);
        }
        #endregion

        #region Blend

        public bool Blend
        {
            get => _Blend;
            set
            {
                if (_Blend == value) return;

                bool oldValue = _Blend;
                _Blend = value;

                OnBlendChanged(oldValue, value);
            }
        }
        private bool _Blend;
        public event EventHandler<EventArgs> BlendChanged;
        public virtual void OnBlendChanged(bool oValue, bool nValue)
        {
            BlendChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region ImageOpacity

        public float ImageOpacity
        {
            get => _ImageOpacity;
            set
            {
                if (_ImageOpacity == value) return;

                float oldValue = _ImageOpacity;
                _ImageOpacity = value;

                OnImageOpacityChanged(oldValue, value);
            }
        }
        private float _ImageOpacity;
        public event EventHandler<EventArgs> ImageOpacityChanged;
        public virtual void OnImageOpacityChanged(float oValue, float nValue)
        {
            ImageOpacityChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region LibraryFile
        public MirLibrary Library;

        public LibraryFile LibraryFile
        {
            get => _LibraryFile;
            set
            {
                if (_LibraryFile == value) return;

                LibraryFile oldValue = _LibraryFile;
                _LibraryFile = value;

                OnLibraryFileChanged(oldValue, value);
            }
        }
        private LibraryFile _LibraryFile;
        public event EventHandler<EventArgs> LibraryFileChanged;
        public virtual void OnLibraryFileChanged(LibraryFile oValue, LibraryFile nValue)
        {
            CEnvir.LibraryList.TryGetValue(LibraryFile, out Library);

            TextureValid = false;
            UpdateDisplayArea();

            LibraryFileChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region PixelDetect

        public bool PixelDetect
        {
            get => _PixelDetect;
            set
            {
                if (_PixelDetect == value) return;

                bool oldValue = _PixelDetect;
                _PixelDetect = value;

                OnPixelDetectChanged(oldValue, value);
            }
        }
        private bool _PixelDetect;
        public event EventHandler<EventArgs> PixelDetectChanged;
        public virtual void OnPixelDetectChanged(bool oValue, bool nValue)
        {
            PixelDetectChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region DrawImage

        public bool DrawImage
        {
            get => _DrawImage;
            set
            {
                if (_DrawImage == value) return;

                bool oldValue = _DrawImage;
                _DrawImage = value;

                OnDrawImageChanged(oldValue, value);
            }
        }
        private bool _DrawImage;
        public event EventHandler<EventArgs> DrawImageChanged;
        public virtual void OnDrawImageChanged(bool oValue, bool nValue)
        {
            DrawImageChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Index

        public int Index
        {
            get => _Index;
            set
            {
                if (_Index == value) return;

                int oldValue = _Index;
                _Index = value;

                OnIndexChanged(oldValue, value);
            }
        }
        private int _Index;
        public event EventHandler<EventArgs> IndexChanged;
        public virtual void OnIndexChanged(int oValue, int nValue)
        {
            TextureValid = false;
            UpdateDisplayArea();
            IndexChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXTab()
        {
            Location = new Point(0, TabHeight - 1);
            BackColour = BackColour = Color.FromArgb(16, 8, 8);
            DrawTexture = true;
            BorderColour = Color.FromArgb(198, 166, 99);
            PassThrough = true;
            Visible = false;

            TabButton = new DXButton
            {
                ButtonType = ButtonType.DeselectedTab,
                Size = new Size(60, TabHeight),
            };
            TabButton.Label.TextChanged += (o, e) =>
            {
                TabButton.Size = new Size(Math.Max(60, DXLabel.GetSize(TabButton.Label.Text, TabButton.Label.Font, TabButton.Label.Outline, new Size(4096, 4096)).Width), TabHeight);

            };
            TabButton.MouseClick += (o, e) =>
            {
                DXTabControl tab = TabButton.Parent as DXTabControl;
                if (tab == null) return;
                tab.SelectedTab = this;
            };
            TabButton.LocationChanged += TabButton_LocationChanged;
            TabButton.IsMovingChanged += TabButton_IsMovingChanged;
        }

        #region Methods
        /// <summary>
        /// 按钮移动改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabButton_IsMovingChanged(object sender, EventArgs e)
        {
            if (!IsMoving)
            {
                DXTabControl cTab = Parent as DXTabControl;
                if (cTab != null)
                {
                    cTab.Controls.Remove(TabButton);
                    cTab.Controls.Insert(0, TabButton);
                    cTab.TabsChanged();
                }
                else
                {
                    DXControl oldParent = Parent;

                    DXTabControl nTab = new DXTabControl
                    {
                        Parent = TabButton.Parent.Parent,
                        Location = new Point(TabButton.DisplayArea.X - ActiveScene.Location.X, TabButton.DisplayArea.Y - ActiveScene.Location.Y),
                        Visible = true,

                        PassThrough = TabButton.Parent.PassThrough,
                        Size = TabButton.Parent.Size,
                        Movable = TabButton.Parent.Movable,
                        Border = TabButton.Parent.Border,
                        BorderColour = TabButton.Parent.BorderColour,
                        AllowResize = TabButton.Parent.AllowResize,
                    };

                    Parent = nTab;
                    nTab.SelectedTab = this;
                    oldParent.Dispose();
                }

                TabButton.Tag = null;
                //我不希望DXTab 固定为 选项卡+分页  因为我可能还要实现其他的子页面
                //Size = new Size(Parent.Size.Width - Location.X, Parent.Size.Height - Location.Y);


            }
        }
        /// <summary>
        /// 按钮位置改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabButton_LocationChanged(object sender, EventArgs e)
        {
            if (Updating || !TabButton.IsMoving) return;

            if (Parent?.Parent == null) return;

            const int threshhold = 20;
            foreach (DXControl control in Parent?.Parent.Controls) //parent.parent
            {
                DXTabControl tab = control as DXTabControl;

                if (tab == null) continue;
                if (tab.DisplayArea.Left - TabButton.DisplayArea.Right > threshhold || tab.DisplayArea.Top - TabButton.DisplayArea.Bottom > threshhold ||
                    TabButton.DisplayArea.Left - tab.DisplayArea.Right > threshhold || TabButton.DisplayArea.Top - tab.DisplayArea.Top > threshhold) continue;

                DXControl oldParent = Parent;
                Updating = true;
                Parent = control;

                Updating = false;

                if (!(oldParent is DXTabControl))
                    oldParent.Dispose();
                //FOUND 

                //Visible = tab == Parent;
                Parent = tab;

                if (OldOpacity.HasValue)
                    Opacity = OldOpacity.Value;

                TabButton.Tag = null;

                int w = 0;
                int pivot = TabButton.Location.X + TabButton.Size.Width / 2;
                for (int i = 0; i < tab.TabButtons.Count; i++)
                {
                    DXButton button = tab.TabButtons[i];

                    w += button.Size.Width;

                    if (w < pivot) continue;

                    if (tab.TabButtons[i] == TabButton) return; //IF SAME PARENT


                    tab.TabButtons.Remove(TabButton);
                    tab.TabButtons.Insert(i, TabButton);
                    Updating = true;
                    tab.TabsChanged();
                    Updating = false;
                    break;
                }
                return;
            }

            if (!(Parent is DXTabControl))
            {
                Parent.Location = new Point(TabButton.DisplayArea.X - ActiveScene.Location.X, TabButton.DisplayArea.Y - ActiveScene.Location.Y);
                return;
            }

            DXControl panel = new DXLabel
            {
                Visible = true,
                Parent = Parent.Parent, //Parent.Parent
                Size = Parent.Size,
                Location = new Point(TabButton.DisplayArea.X - ActiveScene.Location.X, TabButton.DisplayArea.Y - ActiveScene.Location.Y),
            };

            TabButton.Tag = Parent.Size;
            Parent = panel;
            Visible = true;

            OldOpacity = Opacity;
            Opacity = 0.5F;
        }
        /// <summary>
        /// 更新边框信息
        /// </summary>
        protected internal override void UpdateBorderInformation()
        {
            BorderInformation = null;
            if (!Border || Size.Width == 0 || Size.Height == 0) return;

            BorderInformation = new[]
            {
                new Vector2(1, 1),
                new Vector2(Size.Width - 1, 1 ),
                new Vector2(Size.Width - 1, Size.Height - 1),
                new Vector2(1 , Size.Height - 1),
                new Vector2(1 , 1)
            };
        }
        /// <summary>
        /// 绘制
        /// </summary>
        public override void Draw()
        {
            if (!IsVisible || Size.Width == 0 || Size.Height == 0) return;

            OnBeforeDraw();
            DrawControl();
            OnBeforeChildrenDraw();
            if (DrawOtherBorder)
                DrawTabBorder();
            DrawChildControls();
            DrawBorder();
            OnAfterDraw();
        }
        /// <summary>
        /// 绘制选项卡边框
        /// </summary>
        protected void DrawTabBorder()
        {
            if (InterfaceLibrary == null) return;

            Surface oldSurface = DXManager.CurrentSurface;
            DXManager.SetSurface(DXManager.ScratchSurface);
            DXManager.Device.Clear(ClearFlags.Target, Color.Empty, 0, 0);

            DrawEdges();

            DXManager.SetSurface(oldSurface);

            float oldOpacity = DXManager.Opacity;

            DXManager.SetOpacity(Opacity);

            PresentControlTexture(DXManager.ScratchTexture, Parent, DisplayArea, ForeColour, this, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

            DXManager.SetOpacity(oldOpacity);
        }
        /// <summary>
        /// 绘制边缘
        /// </summary>
        public void DrawEdges()
        {
            InterfaceLibrary.Draw(225, 0, 0, Color.White, false, 1F, ImageType.Image);

            Size s = InterfaceLibrary.GetSize(226);
            InterfaceLibrary.Draw(226, Size.Width - s.Width, 0, Color.White, false, 1F, ImageType.Image);

            s = InterfaceLibrary.GetSize(208);
            InterfaceLibrary.Draw(208, 0, Size.Height - s.Height, Color.White, false, 1F, ImageType.Image);

            s = InterfaceLibrary.GetSize(209);
            InterfaceLibrary.Draw(209, Size.Width - s.Width, Size.Height - s.Height, Color.White, false, 1F, ImageType.Image);

            int x = s.Width;
            int y = s.Height;

            s = InterfaceLibrary.GetSize(202);
            InterfaceLibrary.Draw(202, x, 0, Color.White, new Rectangle(0, 0, Size.Width - x * 2, s.Height), 1f, ImageType.Image);
            InterfaceLibrary.Draw(202, x, Size.Height - s.Height, Color.White, new Rectangle(0, 0, Size.Width - x * 2, s.Height), 1f, ImageType.Image);

            s = InterfaceLibrary.GetSize(201);
            InterfaceLibrary.Draw(201, 0, y, Color.White, new Rectangle(0, 0, s.Width, Size.Height - y * 2), 1F, ImageType.Image);
            InterfaceLibrary.Draw(201, Size.Width - s.Width, y, Color.White, new Rectangle(0, 0, s.Width, Size.Height - y * 2), 1F, ImageType.Image);
        }


        protected override void DrawControl()
        {
            base.DrawControl();

            if (!DrawImage) return;

            DrawMirTexture();
        }

        protected virtual void DrawMirTexture()
        {
            bool oldBlend = DXManager.Blending;
            float oldRate = DXManager.BlendRate;

            MirImage image = Library.CreateImage(Index, ImageType.Image);

            if (image?.Image == null) return;

            if (Blend)
                DXManager.SetBlend(true, ImageOpacity);
            else
                DXManager.SetOpacity(ImageOpacity);


            PresentMirImage(image.Image, Parent, DisplayArea, IsEnabled ? ForeColour : Color.FromArgb(75, 75, 75), this, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

            if (Blend)
                DXManager.SetBlend(oldBlend, oldRate);
            else
                DXManager.SetOpacity(1F);

            image.ExpireTime = Time.Now + Config.CacheDuration;
        }

        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            (Parent as DXTabControl)?.SetNewTab();

            base.Dispose(disposing);

            if (disposing)
            {
                if (TabButton != null)
                {
                    if (!TabButton.IsDisposed)
                        TabButton.Dispose();

                    TabButton = null;
                }

                OldOpacity = null;
                _DrawOtherBorder = false;
                Updating = false;

                _CurrentTabControl = null;
                _Selected = false;

                SelectedChanged = null;
                DrawOtherBorderChanged = null;
                CurrentTabControlChanged = null;
            }
        }
        #endregion
    }
}
