using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Extended
{
    public sealed class Surface : IDisposable
    {
        private RenderTarget2D renderTarget2D;

        public static readonly Surface BackBuffer = new Surface(null);

        public bool Disposed { get; private set; }

        public Surface(RenderTarget2D rt)
        {
            renderTarget2D = rt;
        }

        public RenderTarget2D GetRenderTarget2D()
        {
            return renderTarget2D;
        }

        public void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            Disposed = true;
            if (renderTarget2D != null)
            {
                if (!renderTarget2D.IsDisposed)
                {
                    renderTarget2D.Dispose();
                }
                renderTarget2D = null;
            }
        }

        public void Dispose()
        {
            Dispose(!Disposed);
            GC.SuppressFinalize(this);
        }

        ~Surface()
        {
            Dispose(disposing: false);
        }
    }
}
