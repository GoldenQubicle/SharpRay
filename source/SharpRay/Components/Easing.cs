using static SharpRay.Core.Application;
using System;
using SharpRay.Core;

namespace SharpRay.Components
{
    public class Easing : Component
    {
        private readonly Func<float, float, float, float, float> EasingFunction;
        private readonly double IntervalTime;
        private bool IsReversed;
        private readonly bool IsRepeated;
        private double ElapsedTime;

        public Easing(Func<float, float, float, float, float> easingFunction, double intervalTime, bool isReversed = false, bool isRepeated = false)
        {
            EasingFunction = easingFunction;
            IntervalTime = intervalTime * SharpRayConfig.TickMultiplier;
            IsReversed = isReversed;
            IsRepeated = isRepeated;
        }

        /// <summary>
        /// Set the elapsed interval time based on normalized value 0-1
        /// </summary>
        /// <param name="v"></param>
        public void SetElapsedTime(float v)
        {
            var s = IsReversed ? 1 - v : v;
            ElapsedTime = MapRange(s, 0d, 1d, 0d, IntervalTime);
        }

        /// <summary>
        /// Get normalized value 0-1 based on the elapsed time
        /// </summary>
        /// <param></param>
        /// <returns>float</returns>
        public float GetValue()
        {
            var t = Math.Min(ElapsedTime, IntervalTime);
            var e = EasingFunction((float)t, 0f, 1f, (float)IntervalTime);
            return IsReversed ? 1 - e : e;
        }

        public override void Update(double deltaTime)
        {
            ElapsedTime += deltaTime;

            if (IsRepeated)
                if (ElapsedTime > IntervalTime)
                {
                    ElapsedTime = 0;
                    IsReversed = !IsReversed;
                }
        }
    }
}
