using System;
using System.Diagnostics;

namespace Library
{
    public static class Time  //时间
    {
        private static readonly DateTime StartTime = DateTime.Now;  //更改时区
        public static readonly Stopwatch Stopwatch = Stopwatch.StartNew();
        public static long NanoSecPerTick => (1000L * 1000L * 1000L) / Stopwatch.Frequency;
        public static DateTime Now => StartTime + Stopwatch.Elapsed;
    }
}
