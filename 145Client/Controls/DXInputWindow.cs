using Client.UserModels;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace Client.Controls
{
    /// <summary>
    /// 确认窗口 
    /// </summary>
    public sealed class DXInputWindow : DXWindow
    {

        #region Properties

        #region Value
        /// <summary>
        /// 数值
        /// </summary>
        public string Value
        {
            get
            {
                if (null != ValueBox?.TextBox)
                {
                    return ValueBox.TextBox.Text;
                }

                return null;
            }
            set
            {
                if (null != ValueBox?.TextBox)
                {
                    ValueBox.TextBox.Text = value;
                    ValueBox.NeedFocus = true;
                    ValueBox.CheckFocus();
                }
            }
        }
        #endregion

        #region ContentSize
        /// <summary>
        /// 内容大小
        /// </summary>
        public int ContentSize
        {
            get => _ContentSize;
            set
            {
                if (_ContentSize == value) return;

                int oldValue = _ContentSize;
                _ContentSize = value;

                OnContentSizeChanged(oldValue, value);
            }
        }
        private int _ContentSize;
        public void OnContentSizeChanged(int oValue, int nValue)
        {
            UpdateLocations();
        }
        #endregion

        #region ValueColor
        /// <summary>
        /// 数值颜色
        /// </summary>
        public Color ValueColor
        {
            get
            {
                return _ValueColor;
            }
            set
            {
                if (_ValueColor == value) return;
                _ValueColor = value;
                if (ValueBox != null)
                {
                    ValueBox.ForeColour = value;
                }
                if (null != SelectColor && null != ColorItems[value.ToArgb()])
                {
                    SelectColor.Location = ColorItems[value.ToArgb()].Location;
                    //SelectColor.Index = (SelectColor.Location.X - 40) / 20 + 570;
                }
            }
        }
        private Color _ValueColor; //Don't Dispose
        #endregion

        private DXImageControl SelectColor;
        private Dictionary<int, DXImageControl> ColorItems = new Dictionary<int, DXImageControl>();
        public DxMirButton ConfirmButton, CancelButton;
        public DXImageControl Background;
        public DXLabel Content;
        public DXTextBox ValueBox;
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        ClientUserItem _item;
        /// <summary>
        /// 仅数字输入
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ok"></param>
        /// <param name="amount"></param>
        /// <param name="content"></param>
        /// <param name="cancel"></param>
        public DXInputWindow(ClientUserItem item, Action<int> ok, long amount = 1, string content = "", Action cancel = null)
        {
            Init(item, amount, content, cancel);
            ConfirmButton.MouseClick += (o, e) =>
            {
                if (string.IsNullOrEmpty(ValueBox.TextBox.Text))
                {
                    return;
                }
                DXItemCell.SelectedInventoryCell = null;
                ok?.Invoke(Convert.ToInt32(ValueBox.TextBox.Text));
                Dispose();
            };
        }

        public delegate bool ChangeAction(string str);
        /// <summary>
        /// 输入文本
        /// </summary>
        /// <param name="change">文本改变事件 返回bool值 </param>
        /// <param name="ok"></param>
        /// <param name="content"></param>
        /// <param name="cancel"></param>
        public DXInputWindow(ChangeAction change, Action<string> ok, string content = "", Action cancel = null)
        {
            Init(null, -1, content, cancel);
            ValueBox.TextBox.TextChanged += (s, e) =>
            {
                if (change != null)
                {
                    ConfirmButton.Enabled = change(ValueBox.TextBox.Text);
                }
            };
            ConfirmButton.MouseClick += (s, e) =>
            {
                if (string.IsNullOrEmpty(ValueBox.TextBox.Text))
                {
                    return;
                }
                DXItemCell.SelectedInventoryCell = null;
                ok?.Invoke(ValueBox.TextBox.Text);
                Dispose();
            };
        }

        /// <summary>
        /// 颜色输入
        /// </summary>
        /// <param name="ok"></param>
        /// <param name="message"></param>
        public DXInputWindow(ChangeAction change, Action<string, Color> ok, string content = "", Action cancel = null)
        {
            Init(null, -1, content, cancel);
            ValueBox.TextBox.TextChanged += (s, e) =>
            {
                if (change != null)
                {
                    ConfirmButton.Enabled = change(ValueBox.TextBox.Text);
                }
            };

            ConfirmButton.MouseClick += (o, e) =>
            {
                if (string.IsNullOrEmpty(ValueBox.TextBox.Text))
                {
                    return;
                }
                DXItemCell.SelectedInventoryCell = null;
                ok?.Invoke(ValueBox.TextBox.Text, ValueColor);
                Dispose();
            };

            ColorItems[Color.White.ToArgb()] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 3270,
                Location = new Point(27, 98),
            };
            ColorItems[Color.DarkGray.ToArgb()] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 3271,
                Location = new Point(47, 98),
            };
            ColorItems[Color.SandyBrown.ToArgb()] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 3272,
                Location = new Point(67, 98),
            };
            ColorItems[Color.Red.ToArgb()] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 3273,
                Location = new Point(87, 98),
            };
            ColorItems[Color.PaleVioletRed.ToArgb()] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 3274,
                Location = new Point(107, 98),
            };
            ColorItems[Color.Yellow.ToArgb()] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 3275,
                Location = new Point(127, 98),
            };
            ColorItems[Color.LightGreen.ToArgb()] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 3276,
                Location = new Point(147, 98),
            };
            ColorItems[Color.SeaGreen.ToArgb()] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 3277,
                Location = new Point(167, 98),
            };
            ColorItems[Color.DeepSkyBlue.ToArgb()] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 3278,
                Location = new Point(187, 98),
            };
            ColorItems[Color.RoyalBlue.ToArgb()] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 3279,
                Location = new Point(207, 98),
            };
            ColorItems[Color.DarkOrchid.ToArgb()] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 3280,
                Location = new Point(227, 98),
            };
            ColorItems[Color.MediumOrchid.ToArgb()] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 3281,
                Location = new Point(247, 98),
            };
            foreach (var item in ColorItems)
            {
                item.Value.MouseClick += (o, e) =>
                {
                    ValueColor = Color.FromArgb(item.Key);
                    SelectColor.Location = item.Value.Location;
                    ValueBox.CheckFocus();
                };
            };

            SelectColor = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 3230,
                Location = new Point(27, 98),
            };
        }

        #region Methods
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <param name="content"></param>
        /// <param name="cancel"></param>
        private void Init(ClientUserItem item, long amount = 1, string content = "", Action cancel = null)
        {
            _item = item;
            HasFooter = false;
            HasTitle = false;
            HasTopBorder = false;
            HasTransparentEdges = true;

            TitleLabel.Visible = false;
            Parent = ActiveScene;
            MessageBoxList.Add(this);
            Modal = true;
            Opacity = 0F;

            Background = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1240,
            };

            Content = new DXLabel
            {
                Text = content,
                Parent = Background,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                AutoSize = false,
                Location = new Point(Background.Location.X, Background.Location.Y + 125)
            };

            ValueBox = new DXTextBox   //数值容器
            {
                ForeColour = Color.White,
                Parent = Background,
                BackColour = Color.FromArgb(19, 8, 6),
                Border = false,
                Size = new Size(244, 16),
                Opacity = 1F,
                Location = new Point(Background.Location.X + 50, Background.Location.Y + 230),
                KeepFocus = true,
            };
            if (_item != null)
            {
                ValueBox.TextBox.Text = amount.ToString();
                ValueBox.SetFocus();
                ValueBox.TextBox.KeyPress += TextBox_KeyPress;
            }
            ValueBox.TextBox.KeyUp += TextBox_KeyUp;

            var h = DXLabel.GetHeight(Content, Content.Size.Width).Height;
            Content.Size = new Size(Background.Size.Width, h);

            Background.Size = new Size(Background.Size.Width, h + ValueBox.Size.Height + 10);

            ConfirmButton = new DxMirButton    //确认按钮
            {
                MirButtonType = MirButtonType.Normal,
                LibraryFile = LibraryFile.UI1,
                Index = 1241,
                Location = new Point(50, 190),
                Size = new Size(80, DefaultHeight),
                Parent = Background,
                //Enabled = _item != null
            };
            ConfirmButton.MouseDown += (sender, args) =>
            {
                ConfirmButton.Index = 1242;
            };
            ConfirmButton.MouseUp += (sender, args) =>
            {
                ConfirmButton.Index = 1241;
            };

            CancelButton = new DxMirButton    //取消按钮
            {
                MirButtonType = MirButtonType.Normal,
                LibraryFile = LibraryFile.UI1,
                Index = 1245,
                Size = new Size(80, DefaultHeight),
                Parent = Background
            };
            CancelButton.MouseDown += (sender, args) =>
            {
                CancelButton.Index = 1246;
            };
            CancelButton.MouseUp += (sender, args) =>
            {
                CancelButton.Index = 1245;
            };

            CancelButton.Location = new Point(Background.Size.Width - CancelButton.Size.Width - 50, 190);
            CancelButton.MouseClick += (s, e) =>
            {
                DXItemCell.SelectedInventoryCell = null;
                cancel?.Invoke();
                Dispose();
            };
            CloseButton.Visible = false;
            CloseButton.MouseClick += (s, e) =>
            {
                CancelButton.InvokeMouseClick();
            };

            UpdateLocations();
        }

        /// <summary>
        /// 消息面板松开按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            ConfirmButton.Enabled = !string.IsNullOrEmpty(ValueBox.TextBox.Text);
        }

        /// <summary>
        /// 消息面板按键处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)     //回车执行事件
            {
                ConfirmButton.InvokeMouseClick();
                return;
            }
            else if (e.KeyChar == (char)Keys.Escape)  //ESC执行事件
            {
                CancelButton.InvokeMouseClick();
                return;
            }

            if (_item != null)
            {
                if (e.KeyChar != '\b')//这是允许输入退格键  
                {
                    int len = ValueBox.TextBox.Text.Length;
                    if (len < 1 && e.KeyChar == '0')
                    {
                        ValueBox.TextBox.Text = "1";
                        ValueBox.SetFocus();
                        e.Handled = true;
                    }
                    else if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字  
                    {
                        e.Handled = true;
                    }
                    else if (!string.IsNullOrEmpty(ValueBox.TextBox.Text + e.KeyChar) && long.Parse(ValueBox.TextBox.Text + e.KeyChar) > _item.Count)
                    {
                        if (ValueBox.TextBox.Text == _item.Count.ToString())
                        {
                            ValueBox.TextBox.Text = "";
                        }
                        else
                        {
                            ValueBox.TextBox.Text = _item.Count.ToString();
                            ValueBox.SetFocus();
                            e.Handled = true;
                        }
                    }
                }
            }

        }
        /// <summary>
        /// 更新显示坐标
        /// </summary>
        private void UpdateLocations()
        {
            Size = new Size(Background.Size.Width, Background.Size.Height);
            Location = new Point((ActiveScene.DisplayArea.Width - DisplayArea.Width) / 2, (ActiveScene.DisplayArea.Height - DisplayArea.Height - 68) / 2);
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (ColorItems != null)
                {
                    foreach (var item in ColorItems)
                    {
                        if (!item.Value.IsDisposed)
                            item.Value.Dispose();
                    }
                    ColorItems.Clear();
                    ColorItems = null;
                }
                if (Background != null)
                {
                    if (!Background.IsDisposed)
                        Background.Dispose();
                    Background = null;
                }

                if (Content != null)
                {
                    if (!Content.IsDisposed)
                        Content.Dispose();
                    Content = null;
                }

                if (ConfirmButton != null)
                {
                    if (!ConfirmButton.IsDisposed)
                        ConfirmButton.Dispose();
                    ConfirmButton = null;
                }
                if (CancelButton != null)
                {
                    if (!CancelButton.IsDisposed)
                        CancelButton.Dispose();
                    CancelButton = null;
                }

                if (ValueBox != null)
                {
                    if (!ValueBox.IsDisposed)
                        ValueBox.Dispose();
                    ValueBox = null;
                }
            }
        }
        #endregion
    }
}
