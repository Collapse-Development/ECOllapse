using System;
using _Project.Code.Features.Character.MB;

namespace CharacterSystems
{
    public interface ICharacterHealthSystem : ICharacterSystem
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }

        event Action<float, float> OnHealthChanged;
        event Action OnDeath;

        void TakeDamage(float value);
        void AddHealth(float value);
    }
}

