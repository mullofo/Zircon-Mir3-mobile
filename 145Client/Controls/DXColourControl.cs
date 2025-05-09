﻿using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using System;
using System.Drawing;
using System.Windows.Forms;

//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 色彩调节控件
    /// </summary>
    public sealed class DXColourControl : DXControl
    {
        #region Properies
        private DXColourPicker Window;
        #endregion
        /// <summary>
        /// 颜色控制控件
        /// </summary>
        public DXColourControl()
        {
            DrawTexture = true;
            Border = true;
            BorderColour = Color.FromArgb(198, 166, 99);
            Size = new Size(40, 15);
            BackColour = Color.Black;

            MouseClick += DXColourControl_MouseClick;
        }

        #region Methods
        /// <summary>
        /// 鼠标点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DXColourControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (Window != null)
            {
                if (!Window.IsDisposed)
                    Window.Dispose();
                Window = null;
            }

            Window = new DXColourPicker
            {
                Target = this,
                Parent = ActiveScene,
                PreviousColour = BackColour,
                SelectedColour = BackColour,
            };
            Window.Location = new Point((ActiveScene.Size.Width - Window.Size.Width) / 2, (ActiveScene.Size.Height - Window.Size.Height) / 2);
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Window != null)
                {
                    if (!Window.IsDisposed)
                        Window.Dispose();

                    Window = null;
                }
            }
        }
        #endregion
    }

    public sealed class DXColourPicker : DXWindow  //颜色选择器
    {
        #region Properties

        #region SelectedColour
        /// <summary>
        /// 选定的颜色
        /// </summary>
        public Color SelectedColour
        {
            get => _SelectedColour;
            set
            {
                if (_SelectedColour == value) return;

                Color oldValue = _SelectedColour;
                _SelectedColour = value;

                OnSelectedColourChanged(oldValue, value);
            }
        }
        private Color _SelectedColour;
        public event EventHandler<EventArgs> SelectedColourChanged;
        public void OnSelectedColourChanged(Color oValue, Color nValue)
        {
            SelectedColourChanged?.Invoke(this, EventArgs.Empty);

            if (ColourBox != null)
                ColourBox.BackColour = SelectedColour;

            if (Target != null)
                Target.BackColour = SelectedColour;

            Updating = true;
            RedBox.Value = SelectedColour.R;
            GreenBox.Value = SelectedColour.G;
            BlueBox.Value = SelectedColour.B;
            Updating = false;
        }
        #endregion

        public Color PreviousColour;
        public bool Updating;

        public DXButton SelectButton, CancelButton;
        public DXColourControl Target;
        public DXNumberBox RedBox, GreenBox, BlueBox;
        public DXControl ColourScaleBox;
        public DXControl ColourBox;

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        /// <summary>
        /// 颜色选择器控件
        /// </summary>
        public DXColourPicker()
        {
            Size = new Size(380, 253);
            TitleLabel.Text = "颜色设置".Lang();
            Modal = true;
            HasFooter = true;
            CloseButton.Visible = false;

            CancelButton = new DXButton
            {
                Parent = this,
                Label = { Text = "取消".Lang() },
                Location = new Point(Size.Width / 2 + 10, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
            };
            CancelButton.MouseClick += CancelButton_MouseClick;
            CloseButton.MouseClick += CancelButton_MouseClick;

            SelectButton = new DXButton
            {
                Parent = this,
                Label = { Text = "选择".Lang() },
                Location = new Point((Size.Width) / 2 - 80 - 10, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
            };
            SelectButton.MouseClick += (o, e) => Dispose();


            ColourScaleBox = new DXControl   //色标盒
            {
                Location = new Point(20, 40),
                Parent = this,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(200, 149)
            };
            AfterDraw += (o, e) =>
            {
                PresentTexture(DXManager.ColourPallete, ColourScaleBox, ColourScaleBox.DisplayArea, Color.White, this);
            };
            ColourScaleBox.MouseClick += ColourScaleBox_MouseClick;

            RedBox = new DXNumberBox
            {
                Change = 5,
                MaxValue = 255,
                Parent = this,
            };
            RedBox.Location = new Point(Size.Width - RedBox.Size.Width - 10, 40);
            RedBox.ValueTextBox.ValueChanged += ColourBox_ValueChanged;

            DXLabel label = new DXLabel
            {
                Parent = this,
                Text = "红色".Lang(),
            };
            label.Location = new Point(RedBox.Location.X - label.Size.Width - 5, (RedBox.Size.Height - label.Size.Height) / 2 + RedBox.Location.Y);

            GreenBox = new DXNumberBox
            {
                Change = 5,
                MaxValue = 255,
                Parent = this,
            };
            GreenBox.Location = new Point(Size.Width - GreenBox.Size.Width - 10, 65);
            GreenBox.ValueTextBox.ValueChanged += ColourBox_ValueChanged;

            label = new DXLabel
            {
                Parent = this,
                Text = "绿色".Lang(),
            };
            label.Location = new Point(GreenBox.Location.X - label.Size.Width - 5, (GreenBox.Size.Height - label.Size.Height) / 2 + GreenBox.Location.Y);

            BlueBox = new DXNumberBox
            {
                Change = 5,
                MaxValue = 255,
                Parent = this,
            };
            BlueBox.Location = new Point(Size.Width - BlueBox.Size.Width - 10, 90);
            BlueBox.ValueTextBox.ValueChanged += ColourBox_ValueChanged;

            label = new DXLabel
            {
                Parent = this,
                Text = "蓝色".Lang(),
            };
            label.Location = new Point(BlueBox.Location.X - label.Size.Width - 5, (BlueBox.Size.Height - label.Size.Height) / 2 + BlueBox.Location.Y);

            ColourBox = new DXControl
            {
                Size = BlueBox.ValueTextBox.Size,
                Location = new Point(BlueBox.Location.X + BlueBox.ValueTextBox.Location.X, 172),
                BackColour = SelectedColour,
                Border = true,
                DrawTexture = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Parent = this,
            };
            label = new DXLabel
            {
                Parent = this,
                Text = "颜色".Lang(),
            };
            label.Location = new Point(BlueBox.Location.X - label.Size.Width - 5, (ColourBox.Size.Height - label.Size.Height) / 2 + ColourBox.Location.Y);
        }

        #region Methods
        /// <summary>
        /// 色标盒鼠标点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColourScaleBox_MouseClick(object sender, MouseEventArgs e)
        {

            int x = e.X - ColourScaleBox.DisplayArea.X;
            int y = e.Y - ColourScaleBox.DisplayArea.Y;

            if (x < 0 || y < 0 || x >= 200 || y >= 149) return;

            SelectedColour = Color.FromArgb(DXManager.PalleteData[(y * 200 + x) * 4 + 2], DXManager.PalleteData[(y * 200 + x) * 4 + 1], DXManager.PalleteData[(y * 200 + x) * 4]);
        }
        /// <summary>
        /// 取消按钮鼠标点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_MouseClick(object sender, MouseEventArgs e)
        {
            Target.BackColour = PreviousColour;
            Dispose();
        }
        /// <summary>
        /// 颜色框的值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColourBox_ValueChanged(object sender, EventArgs e)
        {
            if (Updating) return;

            SelectedColour = Color.FromArgb((int)RedBox.Value, (int)GreenBox.Value, (int)BlueBox.Value);
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Updating = false;
                _SelectedColour = Color.Empty;
                SelectedColourChanged = null;
                PreviousColour = Color.Empty;

                if (SelectButton != null)
                {
                    if (!SelectButton.IsDisposed)
                        SelectButton.Dispose();

                    SelectButton = null;
                }

                if (CancelButton != null)
                {
                    if (!CancelButton.IsDisposed)
                        CancelButton.Dispose();

                    CancelButton = null;
                }

                Target = null;

                if (RedBox != null)
                {
                    if (!RedBox.IsDisposed)
                        RedBox.Dispose();

                    RedBox = null;
                }

                if (GreenBox != null)
                {
                    if (!GreenBox.IsDisposed)
                        GreenBox.Dispose();

                    GreenBox = null;
                }

                if (BlueBox != null)
                {
                    if (!BlueBox.IsDisposed)
                        BlueBox.Dispose();

                    BlueBox = null;
                }

                if (ColourScaleBox != null)
                {
                    if (!ColourScaleBox.IsDisposed)
                        ColourScaleBox.Dispose();

                    ColourScaleBox = null;
                }

                if (ColourBox != null)
                {
                    if (!ColourBox.IsDisposed)
                        ColourBox.Dispose();

                    ColourBox = null;
                }
            }
        }
        #endregion
    }
}
