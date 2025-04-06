using Client.Envir;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Extended
{
    public enum ClearFlags
    {
        None = 0,
        Target = 1,
        ZBuffer = 2,
        Stencil = 4,
        All = 7
    }

    public sealed class Device : IDisposable
    {
        public static SpriteBatch SpriteBatch;

        public static GraphicsDevice GraphicsDevice;

        public static GraphicsDeviceManager Graphics;

        public bool Disposed { get; private set; }

        public Device(GraphicsDevice gd, SpriteBatch sp, GraphicsDeviceManager gm)
        {
            SpriteBatch = sp;
            GraphicsDevice = gd;
            Graphics = gm;
        }

        public SpriteBatch GetSpriteBatch()
        {
            return SpriteBatch;
        }

        public GraphicsDevice GetGraphicsDevice()
        {
            return GraphicsDevice;
        }

        public Surface GetBackBuffer(int i, int j)
        {
            return Surface.BackBuffer;
        }

        public void Clear(ClearFlags clearFlags, System.Drawing.Color BackColour, float zdepth, int stencil)
        {
            GraphicsDevice.Clear(ClearOptions.Target, new Microsoft.Xna.Framework.Color(BackColour.R, BackColour.G, BackColour.B, BackColour.A), zdepth, stencil);
        }

        public void Clear(ClearFlags clearFlags, int i, float zdepth, int stencil)
        {
            GraphicsDevice.Clear(ClearOptions.Target, Microsoft.Xna.Framework.Color.Transparent, zdepth, stencil);
        }

        public void SetRenderState(BlendState bs, float rate = 1f)
        {
            DXManager.BlendState = bs;
            //GraphicsDevice.BlendFactor = new Microsoft.Xna.Framework.Color(255f * rate, 255f * rate, 255f * rate, 255f * rate);
        }

        public void SetRenderTarget(int targetIndex, Surface surface)
        {
            GraphicsDevice.SetRenderTarget(surface.GetRenderTarget2D());
        }

        public void Reset(PresentationParameters Parameters)
        {
            Graphics.ApplyChanges();
        }

        public void BeginScene()
        {
        }

        public void EndScene()
        {
        }

        public void Present()
        {
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposed = true;
                SpriteBatch = null;
                GraphicsDevice = null;
            }
        }

        public void Dispose()
        {
            Dispose(!Disposed);
            GC.SuppressFinalize(this);
        }

        ~Device()
        {
            Dispose(disposing: false);
        }
    }
}
