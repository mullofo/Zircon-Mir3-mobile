using System;
using System.Collections.Generic;

namespace MonoGame.Extended
{
    internal class ThrowHelper
    {
        public static void KeyNotFoundException()
        {
            throw new KeyNotFoundException();
        }

        public static void ArgumentOutOfRangeException()
        {
            throw new ArgumentOutOfRangeException();
        }

        public static void InvalidOperationException()
        {
            throw new InvalidOperationException();
        }

        public static void ArgumentException()
        {
            throw new ArgumentException();
        }
    }
}
