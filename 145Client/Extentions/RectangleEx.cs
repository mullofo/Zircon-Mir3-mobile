using System.Drawing;

namespace SharpDX
{
    public static class RectangleEx
    {
        public static Point Location(this Rectangle rectangle)
        {
            return new Point(rectangle.X, rectangle.Y);
        }

        public static Size Size(this Rectangle rectangle)
        {
            return new Size(rectangle.Width, rectangle.Height);
        }

        public static void Offset(this Rectangle rectangle, Point location)
        {
            rectangle.Offset(location.X, location.Y);
        }

        public static Rectangle ToRawRectangle(this System.Drawing.Rectangle rectangle)
        {
            return new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
    }
}
