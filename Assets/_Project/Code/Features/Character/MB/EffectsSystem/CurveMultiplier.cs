using UnityEngine;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    /// <summary>
    /// A time-limited modifier that applies a multiplication factor sampled from an AnimationCurve.
    /// The curve is evaluated based on normalized time (0 to 1) over the specified duration.
    /// Automatically expires when the duration elapses or when manually cancelled.
    /// </summary>
    public class CurveMultiplier : ISpeedModifier
    {
        private readonly AnimationCurve _curve;
        private readonly float _duration;
        private float _elapsed;
        private bool _cancelled;

        /// <summary>
        /// Creates a new curve-based multiplier with the specified curve and duration.
        /// </summary>
        /// <param name="curve">The AnimationCurve to sample values from (evaluated at normalized time 0-1)</param>
        /// <param name="duration">The duration in seconds that this modifier should remain active</param>
        public CurveMultiplier(AnimationCurve curve, float duration)
        {
            _curve = curve;
            _duration = duration;
        }

        /// <summary>
        /// Increments the elapsed time, calculates normalized time, and samples the curve.
        /// </summary>
        /// <param name="dt">Delta time in seconds since last evaluation</param>
        /// <returns>The multiplication factor sampled from the curve at the current normalized time</returns>
        public float Evaluate(float dt)
        {
            _elapsed += dt;
            float normalizedTime = Mathf.Clamp01(_elapsed / _duration);
            return _curve.Evaluate(normalizedTime);
        }

        /// <summary>
        /// Gets whether this modifier has expired due to duration elapsing or manual cancellation.
        /// </summary>
        public bool IsExpired => _elapsed >= _duration || _cancelled;

        /// <summary>
        /// Manually cancels this modifier, causing it to expire immediately.
        /// </summary>
        public void Cancel()
        {
            _cancelled = true;
        }
    }
}
