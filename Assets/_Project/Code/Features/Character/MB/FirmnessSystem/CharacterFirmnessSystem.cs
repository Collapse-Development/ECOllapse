using _Project.Code.Features.Character.MB.EnduranceSystem;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.FirmnessSystem
{
    public class CharacterFirmnessSystem : MonoBehaviour, ICharacterFirmnessSystem
    {
        [SerializeField] private float _currentValue;
        [SerializeField] private float _maxValue = 100f;
        [SerializeField] private float _minValue = -20f;
        
        [SerializeField] private float _minRegenPerSecond = 10f;
        [SerializeField] private float _maxRegenPerSecond = 20f;
        [SerializeField] private float _jumpDamageMultiplier = 1.5f;
        [SerializeField] private float _stunDurationMultiplier = 0.2f;

        private ICharacterEnduranceSystem _enduranceSystem;

        public float CurrentValue
        {
            get => _currentValue;
            private set
            {
                float clampedValue = Mathf.Clamp(value, _minValue, _maxValue);
                if (Mathf.Approximately(_currentValue, clampedValue)) return;

                _currentValue = clampedValue;
                OnCurrentValueChanged?.Invoke(_currentValue, _maxValue);
            }
        }

        public float MaxValue => _maxValue;
        public float MinValue => _minValue;

        public event System.Action<float, float> OnCurrentValueChanged;
        public event System.Action<float> OnMaxValueChanged;
        public event System.Action<float> OnStunned;

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterFirmnessSystemConfig firmnessCfg) return false;

            if (!character.TryRegisterSystem<ICharacterFirmnessSystem>(this)) return false;

            // Получаем ссылку на систему выносливости для расчета регенерации
            _enduranceSystem = character.GetSystem<ICharacterEnduranceSystem>();
            if (_enduranceSystem == null)
            {
                Debug.LogWarning("CharacterFirmnessSystem needs ICharacterEnduranceSystem to calculate regen properly!");
            }

            _maxValue = firmnessCfg.MaxValue;
            _minValue = firmnessCfg.MinValue;
            _minRegenPerSecond = firmnessCfg.MinRegenPerSecond;
            _maxRegenPerSecond = firmnessCfg.MaxRegenPerSecond;
            _jumpDamageMultiplier = firmnessCfg.JumpDamageMultiplier;
            _stunDurationMultiplier = firmnessCfg.StunDurationPerNegativePoint;

            CurrentValue = firmnessCfg.StartFromMaxValue 
                ? _maxValue 
                : Mathf.Clamp(firmnessCfg.CurrentValue, _minValue, _maxValue);

            return true;
        }

        private void Update()
        {
            RegenerateFirmness();
        }

        private void RegenerateFirmness()
        {
            if (CurrentValue >= _maxValue) return;

            float currentRegenRate;

            // Если система стамины сообщает, что мы истощены (сработал штраф на 10 сек)
            if (_enduranceSystem != null && _enduranceSystem.IsExhausted)
            {
                currentRegenRate = _minRegenPerSecond; // Фиксированные 10 Firmness/s
            }
            else if (_enduranceSystem != null && _enduranceSystem.MaxValue > 0)
            {
                // Стандартная формула: 10 + (Stamina/10)
                float enduranceRatio = _enduranceSystem.CurrentValue / _enduranceSystem.MaxValue;
                currentRegenRate = Mathf.Lerp(_minRegenPerSecond, _maxRegenPerSecond, enduranceRatio);
            }
            else
            {
                currentRegenRate = _minRegenPerSecond;
            }
            
            CurrentValue += currentRegenRate * Time.deltaTime;
        }

        public void ReduceFirmness(float baseAmount, bool isJumping)
        {
            if (baseAmount <= 0) return;

            float finalDamage = isJumping ? baseAmount * _jumpDamageMultiplier : baseAmount;
            CurrentValue -= finalDamage;

            // Проверка на пробитие стойкости (оглушение)
            if (CurrentValue < 0)
            {
                HandleStun();
            }
        }

        private void HandleStun()
        {
            // Чем сильнее пробили в минус, тем дольше стан 
            float negativeFirmness = Mathf.Abs(CurrentValue);
            float stunDuration = negativeFirmness * _stunDurationMultiplier;

            // Фиксируем значение для длительности и сразу возвращаем к 0 
            CurrentValue = 0f;

            // Вызываем событие
            OnStunned?.Invoke(stunDuration);
        }

        public void SetMaxValue(float value)
        {
            if (Mathf.Approximately(_maxValue, value)) return;

            _maxValue = value;
            CurrentValue = _currentValue; 
            OnMaxValueChanged?.Invoke(_maxValue);
        }
    }
}