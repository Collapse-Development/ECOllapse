using UnityEngine;
using System;
using _Project.Code.Features.Character.MB;

namespace CharacterSystems
{
    public class CharacterHealthSystem : BaseCharacterSystem, ICharacterHealthSystem
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private Character _character;
        [SerializeField] private float _currentHealth;

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

        public float MaxHealth
        {
            get => _maxHealth;
        }    

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;
        
        public override bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (!base.TryInitialize(character, cfg)) return false;

            if (cfg is not CharacterHealthSystemConfig healthCfg) return false;
            
            _character = character;
            if (!_character.TryRegisterSystem<ICharacterHealthSystem>(this)) return false;
            
            _maxHealth = healthCfg.MaxHealth;
            
            Debug.Log($"HealthSystem initialized with config: MaxHealth={_maxHealth}. IsActive={IsActive}");
            return true;
        }

        private void Awake()
        {
            CurrentHealth = _maxHealth;
        }

        public void TakeDamage (float value)
        {
            if (!IsActive) return;

            if (value <= 0)
            {
                return;
            }

            CurrentHealth -= value;
        }

        public void AddHealth(float value)
        {
            if (!IsActive) return;

            if (value <= 0)
            {
                return;
            }

            CurrentHealth += value;
        }

        public void SetMaxHealth(float value)
        {
            if (!IsActive) return;
            
            if (value <= 0)
            {
                return;
            }

            _maxHealth = value;
            CurrentHealth = _currentHealth;
        }
    }
}
