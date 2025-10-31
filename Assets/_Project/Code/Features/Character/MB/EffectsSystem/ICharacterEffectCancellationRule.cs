using System;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public interface ICharacterEffectCancellationRule
    {
        event Action CancelRequired;
        void Tick(float dt);
    }
}