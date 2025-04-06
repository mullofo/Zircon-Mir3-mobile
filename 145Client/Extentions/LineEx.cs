using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using System.Linq;

namespace SharpDX
{
    public static class LineEx
    {
        public static void Draw(this Line line, Vector2[] vector, System.Drawing.Color color)
        {
            line.Draw(vector.Select(p => new RawVector2(p.X, p.Y)).ToArray(), color.ToRawColorBGRA());
        }
    }
}
