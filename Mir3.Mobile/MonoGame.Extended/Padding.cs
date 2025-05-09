﻿using System;
using System.Drawing;

namespace MonoGame.Extended
{
    public struct Padding
    {
        private bool _all; // Do NOT rename (binary serialization).
        private int _top; // Do NOT rename (binary serialization).
        private int _left; // Do NOT rename (binary serialization).
        private int _right; // Do NOT rename (binary serialization).
        private int _bottom; // Do NOT rename (binary serialization).

        public static readonly Padding Empty = new Padding(0);

        public Padding(int all)
        {
            _all = true;
            _top = _left = _right = _bottom = all;
        }

        public Padding(int left, int top, int right, int bottom)
        {
            _top = top;
            _left = left;
            _right = right;
            _bottom = bottom;
            _all = _top == _left && _top == _right && _top == _bottom;
        }

        public int All
        {
            get => _all ? _top : -1;
            set
            {
                if (_all != true || _top != value)
                {
                    _all = true;
                    _top = _left = _right = _bottom = value;
                }
            }
        }

        public int Bottom
        {
            get => _all ? _top : _bottom;
            set
            {
                if (_all || _bottom != value)
                {
                    _all = false;
                    _bottom = value;
                }
            }
        }

        public int Left
        {
            get => _all ? _top : _left;
            set
            {
                if (_all || _left != value)
                {
                    _all = false;
                    _left = value;
                }
            }
        }

        public int Right
        {
            get => _all ? _top : _right;
            set
            {
                if (_all || _right != value)
                {
                    _all = false;
                    _right = value;
                }
            }
        }

        public int Top
        {
            get => _top;
            set
            {
                if (_all || _top != value)
                {
                    _all = false;
                    _top = value;
                }
            }
        }

        public int Horizontal => Left + Right;

        public int Vertical => Top + Bottom;

        public Size Size => new Size(Horizontal, Vertical);

        public static Padding Add(Padding p1, Padding p2) => p1 + p2;

        public static Padding Subtract(Padding p1, Padding p2) => p1 - p2;

        /// <summary>
        ///  Performs vector addition of two <see cref='Padding'/> objects.
        /// </summary>
        public static Padding operator +(Padding p1, Padding p2)
        {
            return new Padding(p1.Left + p2.Left, p1.Top + p2.Top, p1.Right + p2.Right, p1.Bottom + p2.Bottom);
        }

        /// <summary>
        ///  Contracts a by another.
        /// </summary>
        public static Padding operator -(Padding p1, Padding p2)
        {
            return new Padding(p1.Left - p2.Left, p1.Top - p2.Top, p1.Right - p2.Right, p1.Bottom - p2.Bottom);
        }

        /// <summary>
        ///  Tests whether two <see cref='Padding'/> objects are identical.
        /// </summary>
        public static bool operator ==(Padding p1, Padding p2)
        {
            return p1.Left == p2.Left && p1.Top == p2.Top && p1.Right == p2.Right && p1.Bottom == p2.Bottom;
        }

        /// <summary>
        ///  Tests whether two <see cref='Padding'/> objects are different.
        /// </summary>
        public static bool operator !=(Padding p1, Padding p2) => !(p1 == p2);

        public override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);

        public override string ToString() => $"{{Left={Left},Top={Top},Right={Right},Bottom={Bottom}}}";

        private void ResetAll() => All = 0;

        private void ResetBottom() => Bottom = 0;

        private void ResetLeft() => Left = 0;

        private void ResetRight() => Right = 0;

        private void ResetTop() => Top = 0;

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var p2 = (Padding)obj;
            return Left == p2.Left && Top == p2.Top && Right == p2.Right && Bottom == p2.Bottom;
        }
    }
}
