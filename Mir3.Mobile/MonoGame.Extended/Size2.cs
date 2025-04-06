using Microsoft.Xna.Framework;
using System;

namespace MonoGame.Extended
{
    public struct Size2 : IEquatable<Size2>
    {
        public static readonly Size2 Empty;

        public float Width;

        public float Height;

        public bool IsEmpty
        {
            get
            {
                if (Width == 0f)
                {
                    return Height == 0f;
                }
                return false;
            }
        }

        internal string DebugDisplayString => ToString();

        public Size2(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public static bool operator ==(Size2 first, Size2 second)
        {
            return first.Equals(ref second);
        }

        public bool Equals(Size2 size)
        {
            return Equals(ref size);
        }

        public bool Equals(ref Size2 size)
        {
            if (Width == size.Width)
            {
                return Height == size.Height;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is Size2)
            {
                return Equals((Size2)obj);
            }
            return false;
        }

        public static bool operator !=(Size2 first, Size2 second)
        {
            return !(first == second);
        }

        public static Size2 operator +(Size2 first, Size2 second)
        {
            return Add(first, second);
        }

        public static Size2 Add(Size2 first, Size2 second)
        {
            Size2 result = default;
            result.Width = first.Width + second.Width;
            result.Height = first.Height + second.Height;
            return result;
        }

        public static Size2 operator -(Size2 first, Size2 second)
        {
            return Subtract(first, second);
        }

        public static Size2 operator /(Size2 size, float value)
        {
            return new Size2(size.Width / value, size.Height / value);
        }

        public static Size2 operator *(Size2 size, float value)
        {
            return new Size2(size.Width * value, size.Height * value);
        }

        public static Size2 Subtract(Size2 first, Size2 second)
        {
            Size2 result = default;
            result.Width = first.Width - second.Width;
            result.Height = first.Height - second.Height;
            return result;
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() * 397 ^ Height.GetHashCode();
        }

        public static implicit operator Size2(Point2 point)
        {
            return new Size2(point.X, point.Y);
        }

        public static implicit operator Size2(Point point)
        {
            return new Size2(point.X, point.Y);
        }

        public static implicit operator Point2(Size2 size)
        {
            return new Point2(size.Width, size.Height);
        }

        public static implicit operator Vector2(Size2 size)
        {
            return new Vector2(size.Width, size.Height);
        }

        public static implicit operator Size2(Vector2 vector)
        {
            return new Size2(vector.X, vector.Y);
        }

        public static explicit operator Point(Size2 size)
        {
            return new Point((int)size.Width, (int)size.Height);
        }

        public override string ToString()
        {
            return $"Width: {Width}, Height: {Height}";
        }
    }
}
