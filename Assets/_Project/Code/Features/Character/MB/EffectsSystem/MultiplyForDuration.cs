namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    /// <summary>
    /// A time-limited modifier that applies a constant multiplication factor for a specified duration.
    /// Automatically expires when the duration elapses or when manually cancelled.
    /// </summary>
    public class MultiplyForDuration : ISpeedModifier
    {
        private readonly float _factor;
        private readonly float _duration;
        private float _elapsed;
        private bool _cancelled;

        /// <summary>
        /// Creates a new time-limited multiplier with the specified factor and duration.
        /// </summary>
        /// <param name="factor">The multiplication factor to apply (e.g., 1.5 for 50% speed increase)</param>
        /// <param name="duration">The duration in seconds that this modifier should remain active</param>
        public MultiplyForDuration(float factor, float duration)
        {
            _factor = factor;
            _duration = duration;
        }

        /// <summary>
        /// Increments the elapsed time and returns the constant factor value.
        /// </summary>
        /// <param name="dt">Delta time in seconds since last evaluation</param>
        /// <returns>The multiplication factor</returns>
        public float Evaluate(float dt)
        {
            _elapsed += dt;
            return _factor;
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
