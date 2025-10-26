using System.Collections.Generic;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    /// <summary>
    /// Abstract base class for effect implementations that provides common modifier management logic.
    /// Subclasses define the composition rule by implementing ComputeResult().
    /// </summary>
    /// <typeparam name="TModifier">The type of modifier this effect manages</typeparam>
    public abstract class BaseEffect<TModifier> : IEffect<TModifier> where TModifier : IModifier
    {
        /// <summary>
        /// List of active modifiers managed by this effect.
        /// </summary>
        protected List<TModifier> _modifiers = new List<TModifier>();
        
        /// <summary>
        /// Adds a modifier to this effect and returns a handle for cancellation.
        /// </summary>
        /// <param name="modifier">The modifier to add</param>
        /// <returns>A handle that can be used to cancel the modifier</returns>
        public EffectHandle Add(TModifier modifier)
        {
            _modifiers.Add(modifier);
            return new EffectHandle(modifier, RemoveModifier);
        }
        
        /// <summary>
        /// Updates the effect by evaluating all modifiers, removing expired ones, and recomputing the result.
        /// </summary>
        /// <param name="dt">Delta time in seconds since last update</param>
        public void Tick(float dt)
        {
            // Update all modifiers
            foreach (var modifier in _modifiers)
            {
                modifier.Evaluate(dt);
            }
            
            // Remove expired modifiers
            _modifiers.RemoveAll(m => m.IsExpired);
            
            // Recompute result value
            ComputeResult();
        }
        
        /// <summary>
        /// Computes the final result value by applying the effect's composition rule to all active modifiers.
        /// Subclasses must implement this to define their specific composition logic (e.g., multiplication, addition).
        /// </summary>
        protected abstract void ComputeResult();
        
        /// <summary>
        /// Callback method for EffectHandle to remove a specific modifier.
        /// </summary>
        /// <param name="modifier">The modifier to remove</param>
        private void RemoveModifier(IModifier modifier)
        {
            _modifiers.Remove((TModifier)modifier);
        }
    }
}
