namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    /// <summary>
    /// Interface for modifiers that contribute values to effects.
    /// Modifiers can be time-limited or permanent and support manual cancellation.
    /// </summary>
    public interface IModifier
    {
        /// <summary>
        /// Evaluates the modifier's current value and updates internal state.
        /// </summary>
        /// <param name="dt">Delta time in seconds since last evaluation</param>
        /// <returns>The modifier's contribution value</returns>
        float Evaluate(float dt);
        
        /// <summary>
        /// Gets whether this modifier has expired and should be removed.
        /// </summary>
        bool IsExpired { get; }
        
        /// <summary>
        /// Manually cancels this modifier, causing it to expire immediately.
        /// </summary>
        void Cancel();
    }
}
