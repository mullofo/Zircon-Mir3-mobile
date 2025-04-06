using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Extended
{
    public enum Pool
    {
        Default,
        Managed,
        SystemMemory,
        Scratch
    }

    public enum Usage
    {
        None,
        RenderTarget
    }

    public class Texture : IDisposable
    {
        private Texture2D texture2D;

        private bool isTarget2D;

        public bool Disposed { get; private set; }

        public Texture(Device device, int width, int height, int levelCount, Usage usage, SurfaceFormat format = SurfaceFormat.Color, Pool pool = Pool.Default)
        {
            if (usage == Usage.RenderTarget)
            {
                isTarget2D = true;
                texture2D = new RenderTarget2D(device.GetGraphicsDevice(), width, height, mipMap: false, format, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            }
            else
            {
                texture2D = new Texture2D(device.GetGraphicsDevice(), width, height, false, format);
            }
        }

        public Texture(Texture2D texture)
        {
            texture2D = texture;
            isTarget2D = false;
        }

        public Surface GetSurfaceLevel(int i)
        {
            if (isTarget2D)
            {
                return new Surface((RenderTarget2D)texture2D);
            }
            return Surface.BackBuffer;
        }

        public Texture2D GetTexture2D()
        {
            return texture2D;
        }

        public void SetData<T>(T[] data) where T : struct
        {
            texture2D.SetData(data);
        }

        public void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            Disposed = true;
            if (texture2D != null)
            {
                if (!texture2D.IsDisposed)
                {
                    texture2D.Dispose();
                }
                texture2D = null;
            }
            isTarget2D = false;
        }

        public void Dispose()
        {
            Dispose(!Disposed);
            GC.SuppressFinalize(this);
        }

        ~Texture()
        {
            Dispose(disposing: false);
        }
    }
}
