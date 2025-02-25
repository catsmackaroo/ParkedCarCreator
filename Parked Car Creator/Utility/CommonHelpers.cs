using System;
using System.Diagnostics;
using static IVSDKDotNet.Native.Natives;

namespace ParkedCarCreator.NETBasePreset
{
    internal static class CommonHelpers
    {
        private static readonly Stopwatch stopwatch = new Stopwatch();

        public static bool ShouldExecute(int delayInMilliseconds)
        {
            if (!stopwatch.IsRunning)
                stopwatch.Start();

            if (stopwatch.ElapsedMilliseconds >= delayInMilliseconds)
            {
                stopwatch.Restart();
                return true;
            }

            return false;
        }
        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
