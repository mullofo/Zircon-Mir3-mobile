using System.Drawing;

namespace MonoGame.Extended
{
    public struct Color4
    {
        public float Red;

        public float Green;

        public float Blue;

        public float Alpha;

        public Color4(float red, float green, float blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = 1f;
        }

        public Color4(float alpha, float red, float green, float blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public Color ToColor()
        {
            return Color.FromArgb((int)(Alpha * 255f), (int)(Red * 255f), (int)(Green * 255f), (int)(Blue * 255f));
        }
    }
}
