namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    /// <summary>
    /// Concrete implementation of the movement speed effect.
    /// Uses multiplication as the composition rule to combine modifier values.
    /// </summary>
    public class MovementSpeedEffect : BaseEffect<ISpeedModifier>, IMovementSpeedEffect
    {
        /// <summary>
        /// Gets the current speed multiplier computed from all active modifiers.
        /// Base value is 1.0f (normal speed). Values > 1.0f increase speed, values < 1.0f decrease speed.
        /// </summary>
        public float SpeedMultiplier { get; private set; }
        
        /// <summary>
        /// Computes the final speed multiplier by multiplying all modifier values together.
        /// Starts with base value of 1.0f and multiplies each modifier's evaluated result.
        /// </summary>
        protected override void ComputeResult()
        {
            // Start with base value of 1.0f (neutral element for multiplication)
            SpeedMultiplier = 1.0f;
            
            // Multiply all modifier values together
            foreach (var modifier in _modifiers)
            {
                // Pass 0f since modifiers were already updated in Tick()
                SpeedMultiplier *= modifier.Evaluate(0f);
            }
        }
    }
}
