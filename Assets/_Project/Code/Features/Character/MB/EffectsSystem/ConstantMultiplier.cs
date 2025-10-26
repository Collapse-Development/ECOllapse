namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    /// <summary>
    /// A permanent modifier that applies a constant multiplication factor.
    /// This modifier never expires unless manually cancelled.
    /// </summary>
    public class ConstantMultiplier : ISpeedModifier
    {
        private readonly float _factor;
        private bool _cancelled;

        /// <summary>
        /// Creates a new constant multiplier with the specified factor.
        /// </summary>
        /// <param name="factor">The multiplication factor to apply (e.g., 1.5 for 50% speed increase)</param>
        public ConstantMultiplier(float factor)
        {
            _factor = factor;
        }

        /// <summary>
        /// Returns the constant factor value.
        /// </summary>
        /// <param name="dt">Delta time (unused for constant modifiers)</param>
        /// <returns>The multiplication factor</returns>
        public float Evaluate(float dt)
        {
            return _factor;
        }

        /// <summary>
        /// Gets whether this modifier has been cancelled.
        /// Constant modifiers only expire when manually cancelled.
        /// </summary>
        public bool IsExpired => _cancelled;

        /// <summary>
        /// Manually cancels this modifier, causing it to expire.
        /// </summary>
        public void Cancel()
        {
            _cancelled = true;
        }
    }
}
