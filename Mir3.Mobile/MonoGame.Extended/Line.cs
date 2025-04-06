using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Extended
{
    public class Line : IDisposable
    {
        public float Width { get; set; }

        private SpriteBatch SpriteBatch { get; set; }

        public bool Disposed { get; private set; }

        public Line(Device device)
        {
            SpriteBatch = device.GetSpriteBatch();
        }

        public Line(SpriteBatch spriteBatch)
        {
            SpriteBatch = spriteBatch;
        }

        public void Draw(Vector2[] vertexList, System.Drawing.Color color)
        {
            Microsoft.Xna.Framework.Color color2 = new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A);
            if (vertexList.Length == 0)
            {
                return;
            }
            if (vertexList.Length == 1)
            {
                SpriteBatch.DrawPoint(vertexList[0], color2, Width);
                return;
            }
            Vector2 point = vertexList[0];
            for (int i = 1; i < vertexList.Length; i++)
            {
                SpriteBatch.DrawLine(point, vertexList[i], color2, Width);
                point = vertexList[i];
            }
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposed = true;
                Width = 0f;
            }
        }

        public void Dispose()
        {
            Dispose(!Disposed);
            GC.SuppressFinalize(this);
        }

        ~Line()
        {
            Dispose(disposing: false);
        }
    }
}
