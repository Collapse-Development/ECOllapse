using UnityEngine;

namespace CharacterSystems
{
    public interface IHealthSystem : ICharacterSystem
    {
        float CurrentHealth { get; };
        float MaxHealth { get; };

        void TakeDamage(float value);
        void AddHealth(float value);
    }
}

