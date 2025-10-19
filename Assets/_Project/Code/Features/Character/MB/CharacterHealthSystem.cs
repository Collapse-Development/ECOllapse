using UnityEngine;
using System;

namespace CharacterSystems
{
    public class CharacterHealthSystem : MonoBehaviour, IHealthSystem
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private Character _character;
        private float currentHealth;

        public float CurrentHealth
        {
            get => currentHealth;
            private set
            {
                currentHealth = maxHealth;
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
            }
        }
        public float MaxHealth => maxHealth;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;

        public void TakeDamage (float value)
        {
            if (value <= 0)
            {
                return;
            }

            currentHealth -= value;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth == 0)
            {
                OnDeath?.Invoke();
            }
        }

        public void AddHealth(float value)
        {
            if (value <= 0)
            {
                return;
            }

            currentHealth += value;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        private void Awake()
        {
            _character.TryRegisterSystem<IHealthSystem>(this);
        }
    }
}
