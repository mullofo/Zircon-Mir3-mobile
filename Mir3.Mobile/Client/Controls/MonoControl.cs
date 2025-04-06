using Client.Envir;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Drawing;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Texture = MonoGame.Extended.Texture;

namespace Client.Controls
{
    /// <summary>
    /// 控制组件
    /// </summary>
    public class MonoControl : DXControl
    {
        #region Properties 

        #region Texture
        /// <summary>
        /// 创建纹理
        /// </summary>
        protected override void CreateTexture()
        {
            if (ControlTexture == null || DisplayArea.Size != TextureSize)
            {
                DisposeTexture();
                TextureSize = DisplayArea.Size;
                ControlTexture = new Texture(DXManager.Device, TextureSize.Width, TextureSize.Height, 1, Usage.RenderTarget, SurfaceFormat.Color, Pool.Default);
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
        #endregion

        #endregion

        /// <summary>
        /// 控制控件
        /// </summary>
        public MonoControl()
        {
            ZoomRate = 1F;
            UI_Offset_X = 0;
        }

        #region Methods
        /// <summary>
        /// 获取边界点
        /// </summary>
        /// <param name="tempPoint"></param>
        /// <returns></returns>
        public override Point GetBoundsPoint(Point tempPoint)
        {
            var parentWidth = Parent.ZoomRate == 1F ? Parent.DisplayArea.Width : (int)Math.Round(Parent.DisplayArea.Width * Parent.ZoomRate);
            var parentHeight = Parent.ZoomRate == 1F ? Parent.DisplayArea.Height : (int)Math.Round(Parent.DisplayArea.Height * Parent.ZoomRate);
            if (tempPoint.X + DisplayArea.Width > parentWidth) tempPoint.X = parentWidth - DisplayArea.Width;
            if (tempPoint.Y + DisplayArea.Height > parentHeight) tempPoint.Y = parentHeight - DisplayArea.Height;

            if (tempPoint.X < 0) tempPoint.X = 0;
            if (tempPoint.Y < 0) tempPoint.Y = 0;

            return tempPoint;
        }

        #region Drawing

        /// <summary>
        /// 绘制边框
        /// </summary>
        protected override void DrawBorder()
        {
            if (!Border || BorderInformation == null) return;

            if (DXManager.Line.Width != BorderSize)
                DXManager.Line.Width = BorderSize;

            Surface old = DXManager.CurrentSurface;
            DXManager.SetSurface(DXManager.ScratchSurface);

            DXManager.Device.Clear(ClearFlags.Target, Color.Empty, 0, 0);

            DXManager.Line.Draw(BorderInformation, BorderColour);

            DXManager.SetSurface(old);

            DXManager.Sprite.Draw(DXManager.ScratchTexture, Rectangle.Inflate(new Rectangle(0, 0, Size.Width, Size.Height), 1, 1), Vector2.Zero, new Vector2(DisplayArea.Location.X, DisplayArea.Location.Y), Color.White);
        }
        /// <summary>
        /// 绘制控件
        /// </summary>
        protected override void DrawControl()
        {
            if (!DrawTexture) return;

            if (!TextureValid)
            {
                CreateTexture();

                if (!TextureValid) return;
            }

            float oldOpacity = DXManager.Opacity;

            DXManager.SetOpacity(Opacity);

            DXManager.Sprite.Draw(ControlTexture, Vector2.Zero, new Vector2(DisplayArea.Location.X, DisplayArea.Location.Y), IsEnabled ? Color.White : Color.FromArgb(75, 75, 75));

            DXManager.SetOpacity(oldOpacity);

            ExpireTime = CEnvir.Now + Config.CacheDuration;
        }

        #endregion

        #endregion

    }
}
