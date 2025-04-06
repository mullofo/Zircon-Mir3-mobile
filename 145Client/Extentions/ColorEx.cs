using SharpDX.Mathematics.Interop;
using System;

namespace SharpDX
{
    public static class ColorEx
    {
        public static RawColorBGRA ToRawColorBGRA(this System.Drawing.Color color)
        {
            return new RawColorBGRA(color.B, color.G, color.R, color.A);
        }
        public static RawColor4 ToRawColor4(this System.Drawing.Color color)
        {
            return new RawColor4(color.R, color.G, color.B, color.A);
        }

        public static System.Drawing.Color ToColor(this Color4 color)
        {
            return System.Drawing.Color.FromArgb(Convert.ToInt32(color.Alpha * 255), Convert.ToInt32(color.Red * 255), Convert.ToInt32(color.Green * 255), Convert.ToInt32(color.Blue * 255));
        }
    }
}
