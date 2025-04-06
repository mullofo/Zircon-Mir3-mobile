using Client.Envir;
using Library;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Extended
{
    public enum SpriteFlags
    {
        None = 0,
        DoNotSaveState = 1,
        DoNotModifyRenderState = 2,
        ObjectSpace = 4,
        Billboard = 8,
        AlphaBlend = 0x10,
        SortTexture = 0x20,
        SortDepthFrontToBack = 0x40,
        SortDepthBackToFront = 0x80,
        DoNotAddRefTexture = 0x100
    }

    public class Sprite : IDisposable
    {
        private SpriteBatch SpriteBatch;

        private Device Device;

        private bool _beginCalled;

        private Matrix _Transform;

        public Matrix Transform
        {
            get
            {
                return _Transform;
            }
            set
            {
                if (!(_Transform == value))
                {
                    _ = _Transform;
                    _Transform = value;
                    Flush();
                }
            }
        }

        public SamplerState samplerState { get; set; }

        public bool Disposed { get; private set; }

        public Sprite(Device device)
        {
            SpriteBatch = Device.SpriteBatch;
            Device = device;
            Transform = Matrix.Identity;
            samplerState = SamplerState.PointClamp;
        }

        public void Draw(Texture texture, System.Drawing.Color color)
        {
            if (texture == null) return;

            if (DXManager.Opacity != 1f)
                color = System.Drawing.Color.FromArgb((int)(color.A * DXManager.Opacity), color);

            if (DXManager.GrayScale)
            {
                SpriteBatch.End();
                SpriteBatch.Begin(SpriteSortMode.Immediate, DXManager.AlphaBlend, null, null, null, null, Transform);
                CEnvir.GrayscaleShader.CurrentTechnique.Passes[0].Apply();
                SpriteBatch.Draw(texture.GetTexture2D(), new Vector2(0f, 0f), new Color(color.R, color.G, color.B, color.A));
                SpriteBatch.End();
                SpriteBatch.Begin(SpriteSortMode.Deferred, DXManager.AlphaBlend, null, null, null, null, Transform);
            }
            else
            {
                SpriteBatch.Draw(texture.GetTexture2D(), new Vector2(0f, 0f), new Color(color.R, color.G, color.B, color.A));
            }
        }

        public void Draw(Texture texture, Vector2 center, Vector2 position, System.Drawing.Color color)
        {
            if (texture == null) return;

            if (DXManager.Opacity != 1f)
                color = System.Drawing.Color.FromArgb((int)(color.A * DXManager.Opacity), color);

            if (DXManager.GrayScale)
            {
                SpriteBatch.End();
                SpriteBatch.Begin(SpriteSortMode.Immediate, DXManager.AlphaBlend, null, null, null, null, Transform);
                CEnvir.GrayscaleShader.CurrentTechnique.Passes[0].Apply();
                SpriteBatch.Draw(texture.GetTexture2D(), position, new Color(color.R, color.G, color.B, color.A));
                SpriteBatch.End();
                SpriteBatch.Begin(SpriteSortMode.Deferred, DXManager.AlphaBlend, null, null, null, null, Transform);
            }
            else
            {
                SpriteBatch.Draw(texture.GetTexture2D(), position, new Color(color.R, color.G, color.B, color.A));
            }
        }

        public void Draw(Texture texture, System.Drawing.Rectangle sourceRect, Vector2 center, Vector2 position, System.Drawing.Color color)
        {
            if (texture == null) return;

            if (DXManager.Opacity != 1f)
                color = System.Drawing.Color.FromArgb((int)(color.A * DXManager.Opacity), color);

            if (DXManager.GrayScale)
            {
                SpriteBatch.End();
                SpriteBatch.Begin(SpriteSortMode.Immediate, DXManager.AlphaBlend, null, null, null, null, Transform);
                CEnvir.GrayscaleShader.CurrentTechnique.Passes[0].Apply();
                SpriteBatch.Draw(texture.GetTexture2D(), position, new Rectangle(sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height), new Color(color.R, color.G, color.B, color.A));
                SpriteBatch.End();
                SpriteBatch.Begin(SpriteSortMode.Deferred, DXManager.AlphaBlend, null, null, null, null, Transform);
            }
            else
            {
                SpriteBatch.Draw(texture.GetTexture2D(), position, new Rectangle(sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height), new Color(color.R, color.G, color.B, color.A));
            }
        }

        public void DrawZoom(Texture texture, System.Drawing.Rectangle sourceRect, Vector2 center, Vector2 position, System.Drawing.Color color, float rate)
        {
            if (texture == null) return;

            if (DXManager.Opacity != 1f)
                color = System.Drawing.Color.FromArgb((int)(color.A * DXManager.Opacity), color);

            if (DXManager.GrayScale)
            {
                SpriteBatch.End();
                SpriteBatch.Begin(SpriteSortMode.Immediate, DXManager.AlphaBlend, null, null, null, null, Transform);
                CEnvir.GrayscaleShader.CurrentTechnique.Passes[0].Apply();
                SpriteBatch.Draw(texture.GetTexture2D(), position, new Rectangle(sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height), new Color(color.R, color.G, color.B, color.A), 0f, Vector2.Zero, rate, SpriteEffects.None, 0f);
                SpriteBatch.End();
                SpriteBatch.Begin(SpriteSortMode.Deferred, DXManager.AlphaBlend, null, null, null, null, Transform);
            }
            else
            {
                SpriteBatch.Draw(texture.GetTexture2D(), position, new Rectangle(sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height), new Color(color.R, color.G, color.B, color.A), 0f, Vector2.Zero, rate, SpriteEffects.None, 0f);
            }
        }

        public void DrawZoom(Texture texture, Vector2 position, System.Drawing.Color color, float rate)
        {
            if (texture == null) return;

            if (DXManager.Opacity != 1f)
                color = System.Drawing.Color.FromArgb((int)(color.A * DXManager.Opacity), color);

            if (DXManager.GrayScale)
            {
                SpriteBatch.End();
                SpriteBatch.Begin(SpriteSortMode.Immediate, DXManager.AlphaBlend, null, null, null, null, Transform);
                CEnvir.GrayscaleShader.CurrentTechnique.Passes[0].Apply();
                SpriteBatch.Draw(texture.GetTexture2D(), position, null, new Color(color.R, color.G, color.B, color.A), 0f, Vector2.Zero, rate, SpriteEffects.None, 0f);
                SpriteBatch.End();
                SpriteBatch.Begin(SpriteSortMode.Deferred, DXManager.AlphaBlend, null, null, null, null, Transform);
            }
            else
            {
                SpriteBatch.Draw(texture.GetTexture2D(), position, null, new Color(color.R, color.G, color.B, color.A), 0f, Vector2.Zero, rate, SpriteEffects.None, 0f);
            }
        }

        public void DrawZoom(Texture texture, System.Drawing.Color color, float rate)
        {
            DrawZoom(texture, Vector2.Zero, color, rate);
        }

        public void Begin(SpriteFlags flags = SpriteFlags.AlphaBlend)
        {
            CEnvir.BSCounter++;
            DateTime now = Time.Now;
            if (flags == SpriteFlags.DoNotSaveState)
            {
                SpriteBatch.Begin(SpriteSortMode.Immediate, DXManager.BlendState, samplerState, null, null, null, Transform);
            }
            else
            {
                SpriteBatch.Begin(SpriteSortMode.Deferred, DXManager.BlendState, samplerState, null, null, null, Transform);
            }
            _beginCalled = true;
            CEnvir.BEsDelayCounter += (Time.Now - now).Ticks;
        }

        public void End()
        {
            DateTime now = Time.Now;
            SpriteBatch.End();
            DXManager.BlendState = DXManager.AlphaBlend;
            _beginCalled = false;
            CEnvir.BEsDelayCounter += (Time.Now - now).Ticks;
        }

        public void Flush()
        {
            DateTime now = Time.Now;
            if (_beginCalled)
            {
                SpriteBatch.End();
                DXManager.BlendState = DXManager.AlphaBlend;
                SpriteBatch.Begin(SpriteSortMode.Deferred, DXManager.AlphaBlend, null, null, null, null, Transform);
                CEnvir.BSCounter++;
                CEnvir.BEsDelayCounter += (Time.Now - now).Ticks;
            }
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(!Disposed);
            GC.SuppressFinalize(this);
        }

        ~Sprite()
        {
            Dispose(disposing: false);
        }
    }
}
