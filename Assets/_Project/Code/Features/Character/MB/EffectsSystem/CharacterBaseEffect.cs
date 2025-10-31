using System;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public abstract class CharacterBaseEffect : ICharacterEffect
    {
        public event Action<ICharacterEffect> OnEffectCanceled;

        protected Character Character;
        protected ICharacterEffectCancellationRule CancellationRule;
        
        public virtual void Initialize(Character character)
        {
            Character = character;
        }

        public virtual void Tick(float dt)
        {
            CancellationRule?.Tick(dt);
        }

        public virtual void Cancel()
        {
            OnEffectCanceled?.Invoke(this);
        }
    }
}