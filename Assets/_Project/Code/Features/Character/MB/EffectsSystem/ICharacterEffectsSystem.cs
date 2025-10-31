using System;
using System.Collections.Generic;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public interface ICharacterEffectsSystem : ICharacterSystem
    {
        event Action<ICharacterEffect> OnEffectAdded;
        
        List<T> GetEffectsOfType<T>() where T : class, ICharacterEffect;
        void AddEffect(ICharacterEffect effect);
    }
}