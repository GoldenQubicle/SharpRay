using static SharpRay.Core.Application;

namespace SharpRay.Components
{
    public class Easing : Component
    {
        private readonly Func<float, float, float, float, float> _easingFunction;
        private readonly double _intervalTime;
        private bool _isReversed;
        private readonly bool _isRepeated;
        private double _elapsedTime;

        public Easing(Func<float, float, float, float, float> easingFunction, double intervalTime, bool isReversed = false, bool isRepeated = false)
        {
            _easingFunction = easingFunction;
            _intervalTime = intervalTime * TickMultiplier;
            _isReversed = isReversed;
            _isRepeated = isRepeated;
        }

        /// <summary>
        /// Returns false when the easing is done, i.e. elapsed time is larger than interval time.
        /// Will return false for repeating easings. 
        /// </summary>
        /// <returns></returns>
        public bool IsDone() => !_isRepeated && _elapsedTime > _intervalTime;
        
        /// <summary>
        /// Resets elapsed time to zero. 
        /// </summary>
        public void Reset() => _elapsedTime = 0;

        /// <summary>
        /// Set the elapsed interval time based on normalized value 0-1
        /// </summary>
        /// <param name="v"></param>
        public void SetElapsedTime(float v)
        {
            var s = _isReversed ? 1 - v : v;
            _elapsedTime = MapRange(s, 0d, 1d, 0d, _intervalTime);
        }

        /// <summary>
        /// Get normalized value 0-1 based on the elapsed time
        /// </summary>
        /// <param></param>
        /// <returns>float</returns>
        public float GetValue()
        {
            var t = Math.Min(_elapsedTime, _intervalTime);
            var e = _easingFunction((float)t, 0f, 1f, (float)_intervalTime);
            return _isReversed ? 1 - e : e;
        }

        public override void Update(double deltaTime)
        {
            _elapsedTime += deltaTime;

            if (_isRepeated)
                if (_elapsedTime > _intervalTime)
                {
                    _elapsedTime = 0;
                    _isReversed = !_isReversed;
                }
        }
    }
}
