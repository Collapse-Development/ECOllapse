namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    /// <summary>
    /// Base interface for all effects. Provides the update mechanism for time-based processing.
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// Updates the effect, processing all active modifiers and recomputing the result value.
        /// </summary>
        /// <param name="dt">Delta time in seconds since last update</param>
        void Tick(float dt);
    }
    
    /// <summary>
    /// Generic effect interface that manages modifiers of a specific type.
    /// </summary>
    /// <typeparam name="TModifier">The type of modifier this effect accepts</typeparam>
    public interface IEffect<TModifier> : IEffect where TModifier : IModifier
    {
        /// <summary>
        /// Adds a modifier to this effect and returns a handle for cancellation.
        /// </summary>
        /// <param name="modifier">The modifier to add</param>
        /// <returns>A handle that can be used to cancel the modifier</returns>
        EffectHandle Add(TModifier modifier);
    }
}
