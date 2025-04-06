using Client.Envir;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Drawing;
using Color = System.Drawing.Color;
using Font = MonoGame.Extended.Font;
using Rectangle = System.Drawing.Rectangle;

namespace Client.Controls
{
    /// <summary>
    /// 文本标签控件
    /// </summary>
    public class MonoLabel : MonoControl
    {
        #region Static
        /// <summary>
        /// 获取尺寸
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="outline"></param>
        /// <returns></returns>
        public static Size GetSize(string text, Font font, bool outline)
        {
            if (string.IsNullOrEmpty(text))
                return Size.Empty;

            Size tempSize = TextRenderer.MeasureText(DXManager.Graphics, text, font);

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
        public MonoLabel()
        {
            BackColour = Color.Empty;
            DrawTexture = true;
            AutoSize = true;
            Font = new Font(Config.FontName, 9F * CEnvir.UIScale);
            DrawFormat = TextFormatFlags.WordBreak;

            Outline = true;
            ForeColour = Color.FromArgb(198, 166, 99);
            OutlineColour = Color.Black;

            ZoomRate = 1F;
            UI_Offset_X = 0;
        }

        #region Methods
        /// <summary>
        /// 创建大小
        /// </summary>
        private void CreateSize()
        {
            if (!AutoSize) return;

            Size = GetSize(Text, Font, Outline);
        }

        /// <summary>
        /// 创建纹理
        /// </summary>
        protected override void CreateTexture()
        {
            if (DisplayArea.Size.Width <= 0 || DisplayArea.Size.Height <= 0) return;
            int width = DisplayArea.Width;
            int height = DisplayArea.Height;

            if (ControlTexture == null || DisplayArea.Size != TextureSize)
            {
                DisposeTexture();
                TextureSize = DisplayArea.Size;
                ControlTexture = new MonoGame.Extended.Texture(DXManager.Device, TextureSize.Width, TextureSize.Height, 1, Usage.RenderTarget, Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color, Pool.Managed);
                ControlSurface = ControlTexture.GetSurfaceLevel(0);
                DXManager.ControlList.Add(this);
            }

            Surface currentSurface = DXManager.CurrentSurface;
            DXManager.SetSurface(ControlSurface);
            DXManager.Device.Clear(ClearFlags.Target, BackColour, 0f, 0);
            if (Text != null)
            {
                //if (Outline)
                //{
                //    TextRenderer.DrawText(Device.SpriteBatch, Text, Font, new Rectangle(1, 0, width, height), OutlineColour, DrawFormat);
                //    TextRenderer.DrawText(Device.SpriteBatch, Text, Font, new Rectangle(0, 1, width, height), OutlineColour, DrawFormat);
                //    TextRenderer.DrawText(Device.SpriteBatch, Text, Font, new Rectangle(2, 1, width, height), OutlineColour, DrawFormat);
                //    TextRenderer.DrawText(Device.SpriteBatch, Text, Font, new Rectangle(1, 2, width, height), OutlineColour, DrawFormat);
                //    TextRenderer.DrawText(Device.SpriteBatch, Text, Font, new Rectangle(1, 1, width, height), ForeColour, DrawFormat);
                //}
                //else
                TextRenderer.DrawText(Device.SpriteBatch, Text, Font, Outline, new Rectangle(1, 0, width, height), ForeColour, DrawFormat);
            }
            DXManager.SetSurface(currentSurface);

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

            DXManager.Sprite.Draw(ControlTexture, Vector2.Zero, new Vector2(DisplayArea.Location.X, DisplayArea.Location.Y), IsEnabled ? Color.White : Color.FromArgb(75, 75, 75));

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
