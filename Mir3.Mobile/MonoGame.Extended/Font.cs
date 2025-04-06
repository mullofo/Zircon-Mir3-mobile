using Client.Envir;
using FontStashSharp;
using System;

namespace MonoGame.Extended
{
    public enum FontStyle
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Strikeout = 8
    }
    public class Font : IDisposable
    {
        public DynamicSpriteFont SpriteFont;

        public string FontName;

        public float Size;

        public bool IsDisposed { get; private set; }

        public Font(string name, float size, FontStyle style)
        {
            FontName = name;
            Size = size;
            SpriteFont = CEnvir.Fonts.GetFont(size * 2);
        }

        public Font(string name, float size)
        {
            FontName = name;
            Size = size;
            SpriteFont = CEnvir.Fonts.GetFont(size * 2);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(!IsDisposed);
            GC.SuppressFinalize(this);
        }

        ~Font()
        {
            Dispose(disposing: false);
        }
    }
}
