using System;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    /// <summary>
    /// A lightweight handle that provides a reference to cancel a specific modifier.
    /// Implemented as a readonly struct for performance and immutability.
    /// </summary>
    public readonly struct EffectHandle
    {
        private readonly IModifier _modifier;
        private readonly Action<IModifier> _removeCallback;
        
        /// <summary>
        /// Creates a new effect handle.
        /// </summary>
        /// <param name="modifier">The modifier this handle references</param>
        /// <param name="removeCallback">Callback to remove the modifier from the effect</param>
        internal EffectHandle(IModifier modifier, Action<IModifier> removeCallback)
        {
            _modifier = modifier;
            _removeCallback = removeCallback;
        }
        
        /// <summary>
        /// Cancels the associated modifier, marking it as expired and removing it from the effect.
        /// Safe to call on invalid/default handles.
        /// </summary>
        public void Cancel()
        {
            _modifier?.Cancel();
            _removeCallback?.Invoke(_modifier);
        }
    }
}
