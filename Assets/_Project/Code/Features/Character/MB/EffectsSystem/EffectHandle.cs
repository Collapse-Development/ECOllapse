using System;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public readonly struct EffectHandle
    {
        private readonly ICharacterEffectModifier _modifier;
        private readonly Action<ICharacterEffectModifier> _removeCallback;
        
        internal EffectHandle(ICharacterEffectModifier modifier, Action<ICharacterEffectModifier> removeCallback)
        {
            _modifier = modifier;
            _removeCallback = removeCallback;
        }
        
        public void Cancel()
        {
            _modifier?.Cancel();
            _removeCallback?.Invoke(_modifier);
        }
    }
}
