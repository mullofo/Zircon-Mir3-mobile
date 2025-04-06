using Microsoft.Xna.Framework;
using System;

namespace MonoGame.Extended
{
    public struct RectangleF : IEquatable<RectangleF>
    {
        public static readonly RectangleF Empty;

        public float X;

        public float Y;

        public float Width;

        public float Height;

        public float Left => X;

        public float Right => X + Width;

        public float Top => Y;

        public float Bottom => Y + Height;

        public bool IsEmpty
        {
            get
            {
                if (Width.Equals(0f) && Height.Equals(0f) && X.Equals(0f))
                {
                    return Y.Equals(0f);
                }
                return false;
            }
        }

        public Point2 Position
        {
            get
            {
                return new Point2(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Size2 Size
        {
            get
            {
                return new Size2(Width, Height);
            }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        public Point2 Center => new Point2(X + Width * 0.5f, Y + Height * 0.5f);

        public Point2 TopLeft => new Point2(X, Y);

        public Point2 TopRight => new Point2(X + Width, Y);

        public Point2 BottomLeft => new Point2(X, Y + Height);

        public Point2 BottomRight => new Point2(X + Width, Y + Height);

        internal string DebugDisplayString => X + "  " + Y + "  " + Width + "  " + Height;

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleF(Point2 position, Size2 size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public static void CreateFrom(Point2 minimum, Point2 maximum, out RectangleF result)
        {
            result.X = minimum.X;
            result.Y = minimum.Y;
            result.Width = maximum.X - minimum.X;
            result.Height = maximum.Y - minimum.Y;
        }

        public static RectangleF CreateFrom(Point2 minimum, Point2 maximum)
        {
            CreateFrom(minimum, maximum, out var result);
            return result;
        }

        public static void Union(ref RectangleF first, ref RectangleF second, out RectangleF result)
        {
            result.X = Math.Min(first.X, second.X);
            result.Y = Math.Min(first.Y, second.Y);
            result.Width = Math.Max(first.Right, second.Right) - result.X;
            result.Height = Math.Max(first.Bottom, second.Bottom) - result.Y;
        }

        public static RectangleF Union(RectangleF first, RectangleF second)
        {
            Union(ref first, ref second, out var result);
            return result;
        }

        public RectangleF Union(RectangleF rectangle)
        {
            Union(ref this, ref rectangle, out var result);
            return result;
        }

        public static void Intersection(ref RectangleF first, ref RectangleF second, out RectangleF result)
        {
            Point2 topLeft = first.TopLeft;
            Point2 bottomRight = first.BottomRight;
            Point2 topLeft2 = second.TopLeft;
            Point2 bottomRight2 = second.BottomRight;
            Point2 minimum = Point2.Maximum(topLeft, topLeft2);
            Point2 maximum = Point2.Minimum(bottomRight, bottomRight2);
            if (maximum.X < minimum.X || maximum.Y < minimum.Y)
            {
                result = default;
            }
            else
            {
                result = CreateFrom(minimum, maximum);
            }
        }

        public static RectangleF Intersection(RectangleF first, RectangleF second)
        {
            Intersection(ref first, ref second, out var result);
            return result;
        }

        public RectangleF Intersection(RectangleF rectangle)
        {
            Intersection(ref this, ref rectangle, out var result);
            return result;
        }

        [Obsolete("RectangleF.Intersect() may be removed in the future. Use Intersection() instead.")]
        public static RectangleF Intersect(RectangleF value1, RectangleF value2)
        {
            Intersection(ref value1, ref value2, out var result);
            return result;
        }

        [Obsolete("RectangleF.Intersect() may be removed in the future. Use Intersection() instead.")]
        public static void Intersect(ref RectangleF value1, ref RectangleF value2, out RectangleF result)
        {
            Intersection(ref value1, ref value2, out result);
        }

        public static bool Intersects(ref RectangleF first, ref RectangleF second)
        {
            if (first.X < second.X + second.Width && first.X + first.Width > second.X && first.Y < second.Y + second.Height)
            {
                return first.Y + first.Height > second.Y;
            }
            return false;
        }

        public static bool Intersects(RectangleF first, RectangleF second)
        {
            return Intersects(ref first, ref second);
        }

        public bool Intersects(RectangleF rectangle)
        {
            return Intersects(ref this, ref rectangle);
        }

        public static bool Contains(ref RectangleF rectangle, ref Point2 point)
        {
            if (rectangle.X <= point.X && point.X < rectangle.X + rectangle.Width && rectangle.Y <= point.Y)
            {
                return point.Y < rectangle.Y + rectangle.Height;
            }
            return false;
        }

        public static bool Contains(RectangleF rectangle, Point2 point)
        {
            return Contains(ref rectangle, ref point);
        }

        public bool Contains(Point2 point)
        {
            return Contains(ref this, ref point);
        }

        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            X -= horizontalAmount;
            Y -= verticalAmount;
            Width += horizontalAmount * 2f;
            Height += verticalAmount * 2f;
        }

        public void Offset(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public void Offset(Vector2 amount)
        {
            X += amount.X;
            Y += amount.Y;
        }

        public static bool operator ==(RectangleF first, RectangleF second)
        {
            return first.Equals(ref second);
        }

        public static bool operator !=(RectangleF first, RectangleF second)
        {
            return !(first == second);
        }

        public bool Equals(RectangleF rectangle)
        {
            return Equals(ref rectangle);
        }

        public bool Equals(ref RectangleF rectangle)
        {
            if (X == rectangle.X && Y == rectangle.Y && Width == rectangle.Width)
            {
                return Height == rectangle.Height;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is RectangleF)
            {
                return Equals((RectangleF)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ((X.GetHashCode() * 397 ^ Y.GetHashCode()) * 397 ^ Width.GetHashCode()) * 397 ^ Height.GetHashCode();
        }

        public static implicit operator RectangleF(Rectangle rectangle)
        {
            RectangleF result = default;
            result.X = rectangle.X;
            result.Y = rectangle.Y;
            result.Width = rectangle.Width;
            result.Height = rectangle.Height;
            return result;
        }

        public static explicit operator Rectangle(RectangleF rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        public override string ToString()
        {
            return $"{{X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
        }
    }
}
