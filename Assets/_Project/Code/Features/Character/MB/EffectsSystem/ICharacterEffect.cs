using System;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public interface ICharacterEffect
    {
        event Action<ICharacterEffect> OnEffectEnded;
        
        void Initialize(Character character);
        void Tick(float dt);

        void Cancel();
    }
}
