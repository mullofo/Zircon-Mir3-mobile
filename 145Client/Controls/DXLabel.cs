using Client.Envir;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using Rectangle = System.Drawing.Rectangle;


namespace Client.Controls
{
    /// <summary>
    /// 文本标签控件
    /// </summary>
    public class DXLabel : DXControl
    {
        const string ReturnLine = "\\n";//定义换行符
        #region Static
        /// <summary>
        /// 获取尺寸
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="outline"></param>
        /// <param name="size"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static Size GetSize(string text, Font font, bool outline, Size size, TextFormatFlags flags = TextFormatFlags.WordBreak)
        {
            if (string.IsNullOrEmpty(text))
                return Size.Empty;
            if (text != null && text.IndexOf(ReturnLine) != -1)
            {
                text = text?.Replace(ReturnLine, "_");
                var arr = text?.Split('_');
                var length = arr?.Length ?? 1;
                Size MaxSize = new Size(0, 0);
                for (int i = 0; i < length; i++)
                {
                    Size strSize = TextRenderer.MeasureText(DXManager.Graphics, arr[i], font, size, flags);
                    if (strSize.Width > MaxSize.Width)
                    {
                        MaxSize = strSize;
                        MaxSize.Height = (strSize.Height + 2) * (length);
                    }
                }

                if (outline && MaxSize.Width > 0 && MaxSize.Height > 0)
                {
                    MaxSize.Width += 2;
                    MaxSize.Height += 2;
                }
                return MaxSize;
            }
            Size tempSize = TextRenderer.MeasureText(DXManager.Graphics, text, font, size, flags);

            if (outline && tempSize.Width > 0 && tempSize.Height > 0)
            {
                tempSize.Width += 2;
                tempSize.Height += 2;
            }

            return tempSize;
        }

        /// <summary>
        /// 获取高度
        /// </summary>
        /// <param name="label"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static Size GetHeight(DXLabel label, int width)
        {
            if (label.Text != null && label.Text.IndexOf(ReturnLine) != -1)
            {
                var text = label.Text?.Replace(ReturnLine, "_");
                var arr = text?.Split('_');
                var length = arr?.Length ?? 1;
                Size MaxSize = new Size(0, 0);
                Size strSize = TextRenderer.MeasureText(DXManager.Graphics, label.Text, label.Font, new Size(width, 2000), label.DrawFormat);

                if (label.Outline && MaxSize.Width > 0 && MaxSize.Height > 0)
                {
                    MaxSize.Width += 2;
                    MaxSize.Height += 2;
                }
                return MaxSize;
            }
            Size tempSize = TextRenderer.MeasureText(DXManager.Graphics, label.Text, label.Font, new Size(width, 2000), label.DrawFormat);

            if (label.Outline && tempSize.Width > 0 && tempSize.Height > 0)
            {
                tempSize.Width += 2;
                tempSize.Height += 2;
            }

            return tempSize;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 内间距
        /// </summary>
        public Padding Padding { get; set; } = Padding.Empty;

        #region AutoSize
        /// <summary>
        /// 自动大小
        /// </summary>
        public bool AutoSize
        {
            get => _AutoSize;
            set
            {
                if (_AutoSize == value) return;

                bool oldValue = _AutoSize;
                _AutoSize = value;

                OnAutoSizeChanged(oldValue, value);
            }
        }
        private bool _AutoSize;
        public event EventHandler<EventArgs> AutoSizeChanged;
        public virtual void OnAutoSizeChanged(bool oValue, bool nValue)
        {
            TextureValid = false;
            CreateSize();

            AutoSizeChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region DrawFormat
        /// <summary>
        /// 文本格式
        /// </summary>
        public TextFormatFlags DrawFormat
        {
            get => _DrawFormat;
            set
            {
                if (_DrawFormat == value) return;

                TextFormatFlags oldValue = _DrawFormat;
                _DrawFormat = value;

                OnDrawFormatChanged(oldValue, value);
            }
        }
        private TextFormatFlags _DrawFormat;
        public event EventHandler<EventArgs> DrawFormatChanged;
        public virtual void OnDrawFormatChanged(TextFormatFlags oValue, TextFormatFlags nValue)
        {
            TextureValid = false;
            CreateSize();
            DrawFormatChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Font
        /// <summary>
        /// 字体
        /// </summary>
        public Font Font
        {
            get => _Font;
            set
            {
                if (_Font == value) return;

                Font oldValue = _Font;
                _Font = value;

                OnFontChanged(oldValue, value);
            }
        }
        private Font _Font;
        public event EventHandler<EventArgs> FontChanged;
        public virtual void OnFontChanged(Font oValue, Font nValue)
        {
            FontChanged?.Invoke(this, EventArgs.Empty);

            TextureValid = false;
            CreateSize();
        }
        #endregion

        #region Outline
        /// <summary>
        /// 轮廓
        /// </summary>
        public bool Outline
        {
            get => _Outline;
            set
            {
                if (_Outline == value) return;

                bool oldValue = _Outline;
                _Outline = value;

                OnOutlineChanged(oldValue, value);
            }
        }
        private bool _Outline;
        public event EventHandler<EventArgs> OutlineChanged;
        public virtual void OnOutlineChanged(bool oValue, bool nValue)
        {
            TextureValid = false;
            CreateSize();

            OutlineChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region OutlineColour
        /// <summary>
        /// 轮廓颜色
        /// </summary>
        public Color OutlineColour
        {
            get => _OutlineColour;
            set
            {
                if (_OutlineColour == value) return;

                Color oldValue = _OutlineColour;
                _OutlineColour = value;

                OnOutlineColourChanged(oldValue, value);
            }
        }
        private Color _OutlineColour;
        public event EventHandler<EventArgs> OutlineColourChanged;
        public virtual void OnOutlineColourChanged(Color oValue, Color nValue)
        {
            TextureValid = false;

            OutlineColourChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        /// <summary>
        /// 文本更改时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnTextChanged(string oValue, string nValue)
        {
            base.OnTextChanged(oValue, nValue);

            TextureValid = false;
            CreateSize();
        }
        public override void OnForeColourChanged(Color oValue, Color nValue)
        {
            base.OnForeColourChanged(oValue, nValue);

            TextureValid = false;
        }
        #endregion

        /// <summary>
        /// 文本标签控件
        /// </summary>
        public DXLabel()
        {
            BackColour = Color.Empty;
            DrawTexture = true;
            AutoSize = true;
            Font = new Font(Config.FontName, CEnvir.FontSize(9F));
            DrawFormat = TextFormatFlags.WordBreak;

            Outline = true;
            ForeColour = Color.FromArgb(198, 166, 99);
            OutlineColour = Color.Black;
        }

        #region Methods
        /// <summary>
        /// 创建大小
        /// </summary>
        private void CreateSize()
        {
            if (!AutoSize) return;

            Size = GetSize(Text, Font, Outline, new Size(4096, 4096));
            if (Padding != Padding.Empty)
            {
                Size = new Size(Size.Width + Padding.Left + Padding.Right, Size.Height + Padding.Top + Padding.Bottom);
            }
        }

        /// <summary>
        /// 创建纹理
        /// </summary>
        protected override void CreateTexture()
        {
            int width = DisplayArea.Width;
            int height = DisplayArea.Height;

            if (ControlTexture == null || DisplayArea.Size != TextureSize)
            {
                DisposeTexture();
                TextureSize = DisplayArea.Size;
                ControlTexture = new Texture(DXManager.Device, TextureSize.Width, TextureSize.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
                DXManager.ControlList.Add(this);
            }

            DataRectangle rect = ControlTexture.LockRectangle(0, LockFlags.Discard);

            using (Bitmap image = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, rect.DataPointer))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                DXManager.ConfigureGraphics(graphics);
                graphics.Clear(BackColour);

                if (Text != null && Text.IndexOf(ReturnLine) != -1)
                {
                    var text = Text?.Replace(ReturnLine, "_");
                    var arr = text?.Split('_');
                    var length = arr?.Length ?? 1;
                    for (var i = 0; i < arr.Length; i++)
                    {
                        var textWidth = GetSize(arr[i], Font, Outline, new Size(4096, 4096)).Width;
                        var textHeight = GetSize(arr[i], Font, Outline, new Size(4096, 4096)).Height;
                        var x = 0;
                        var y = i * textHeight;

                        if (Outline)
                        {
                            TextRenderer.DrawText(graphics, arr[i], Font, new Rectangle(1 + x + Padding.Left, 0 + y + Padding.Top, width, textHeight), OutlineColour, DrawFormat);
                            TextRenderer.DrawText(graphics, arr[i], Font, new Rectangle(0 + x + Padding.Left, 1 + y + Padding.Top, width, textHeight), OutlineColour, DrawFormat);
                            TextRenderer.DrawText(graphics, arr[i], Font, new Rectangle(2 + x + Padding.Left, 1 + y + Padding.Top, width, textHeight), OutlineColour, DrawFormat);
                            TextRenderer.DrawText(graphics, arr[i], Font, new Rectangle(1 + x + Padding.Left, 2 + y + Padding.Top, width, textHeight), OutlineColour, DrawFormat);
                            TextRenderer.DrawText(graphics, arr[i], Font, new Rectangle(1 + x + Padding.Left, 1 + y + Padding.Top, width, textHeight), ForeColour, DrawFormat);
                        }
                        else
                            TextRenderer.DrawText(graphics, arr[i], Font, new Rectangle(1 + x + Padding.Left, 0 + y + Padding.Top, width, textHeight), ForeColour, DrawFormat);
                    }
                }
                else
                {
                    if (Outline)
                    {
                        TextRenderer.DrawText(graphics, Text, Font, new Rectangle(1 + Padding.Left, 0 + Padding.Top, width, height), OutlineColour, DrawFormat);
                        TextRenderer.DrawText(graphics, Text, Font, new Rectangle(0 + Padding.Left, 1 + Padding.Top, width, height), OutlineColour, DrawFormat);
                        TextRenderer.DrawText(graphics, Text, Font, new Rectangle(2 + Padding.Left, 1 + Padding.Top, width, height), OutlineColour, DrawFormat);
                        TextRenderer.DrawText(graphics, Text, Font, new Rectangle(1 + Padding.Left, 2 + Padding.Top, width, height), OutlineColour, DrawFormat);
                        TextRenderer.DrawText(graphics, Text, Font, new Rectangle(1 + Padding.Left, 1 + Padding.Top, width, height), ForeColour, DrawFormat);
                    }
                    else
                        TextRenderer.DrawText(graphics, Text, Font, new Rectangle(1 + Padding.Left, 0 + Padding.Top, width, height), ForeColour, DrawFormat);
                }
            }
            ControlTexture.UnlockRectangle(0);
            //rect.Data.Dispose();

            TextureValid = true;
            ExpireTime = CEnvir.Now + Config.CacheDuration;
        }

        /// <summary>
        /// 绘制控制
        /// </summary>
        protected override void DrawControl()
        {
            if (!DrawTexture) return;

            if (!TextureValid) CreateTexture();

            DXManager.SetOpacity(Opacity);

            PresentTexture(ControlTexture, Parent, DisplayArea, IsEnabled ? Color.White : Color.FromArgb(75, 75, 75), this);

            ExpireTime = CEnvir.Now + Config.CacheDuration;
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _AutoSize = false;
                _DrawFormat = TextFormatFlags.Default;
                _Font?.Dispose();
                _Font = null;
                _Outline = false;
                _OutlineColour = Color.Empty;

                AutoSizeChanged = null;
                DrawFormatChanged = null;
                FontChanged = null;
                OutlineChanged = null;
                OutlineColourChanged = null;
            }
        }
        #endregion
    }
}
