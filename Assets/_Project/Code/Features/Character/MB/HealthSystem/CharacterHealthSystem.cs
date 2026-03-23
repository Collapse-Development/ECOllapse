using UnityEngine;
using System;
using _Project.Code.Features.Character.MB;

namespace CharacterSystems
{
    public class CharacterHealthSystem : MonoBehaviour, ICharacterHealthSystem
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private Character _character;
        private float _currentHealth;

        public float CurrentHealth
        {
            get => _currentHealth;
            private set
            {
                _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
                Debug.Log("CurrentHealth: " + _currentHealth);
                OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

                if (_currentHealth == 0)
                {
                    OnDeath?.Invoke();
                }
            }
        }

        public float MaxHealth => _maxHealth;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;
        
        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterHealthSystemConfig healthCfg) return false;
            
            _character = character;
            if (!_character.TryRegisterSystem<ICharacterHealthSystem>(this)) return false;
            
            _maxHealth = healthCfg.MaxHealth;
            
            Debug.Log($"HealthSystem initialized with config: MaxHealth={_maxHealth}");
            return true;
        }

        private void Awake()
        {
            CurrentHealth = _maxHealth;
        }

        public void TakeDamage (float value)
        {
            if (value <= 0)
            {
                return;
            }

            CurrentHealth -= value;
        }

        public void AddHealth(float value)
        {
            if (value <= 0)
            {
                return;
            }

            CurrentHealth += value;
        }
    }
}
