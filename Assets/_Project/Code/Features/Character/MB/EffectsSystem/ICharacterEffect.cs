using System;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public interface ICharacterEffect
    {
        event Action<ICharacterEffect> OnEffectCanceled;
        
        void Initialize(Character character);
        void Tick(float dt);

        void Cancel();
    }
}
