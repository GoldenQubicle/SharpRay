using static SharpRay.Core.Application;

namespace SharpRay.Components
{
	public class Easing : Component
	{
		private readonly Func<float, float, float, float, float> _easingFunction;
		private readonly double _intervalTime;
		private readonly bool _isRepeated;
		private readonly bool _isRepeatedAndReversed;
		private bool _isReversed;
		private bool _isDone;

		private double _elapsedTime;

		public Easing(Func<float, float, float, float, float> easingFunction, double intervalTime, bool isReversed = false, bool isRepeated = false)
		{
			_easingFunction = easingFunction;
			_intervalTime = intervalTime * TickMultiplier;
			_isReversed = isReversed;
			_isRepeated = isRepeated;
			_isRepeatedAndReversed = isRepeated && isReversed;
		}

		/// <summary>
		/// Returns false when the easing is done, i.e. elapsed time is larger than interval time.
		/// </summary>
		/// <returns></returns>
		public bool IsDone() => _isDone;

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
			Debug.Assert(v is >= 0f and <= 1f);
			var s = _isReversed ? 1f - v : v;
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
			if (_isDone) _isDone = false;

			_elapsedTime += deltaTime;

			if (!(_elapsedTime > _intervalTime))
				return;

			_isDone = true;

			if (_isRepeated)
				Reset( );

			if (_isRepeatedAndReversed)
				_isReversed = !_isReversed;

		}
	}
}
