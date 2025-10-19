using UnityEngine;
using System;

namespace CharacterSystems
{
    public class CharacterHealthSystem : MonoBehaviour, IHealthSystem
    {
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;

        void Start()
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

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
    }
}
