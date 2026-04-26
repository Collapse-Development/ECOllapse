using _Project.Code.Features.Character.MB.MovementSystem;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.EnduranceSystem
{
    public class CharacterEnduranceSystem : MonoBehaviour, ICharacterEnduranceSystem
    {
        [SerializeField] private float _currentValue;
        [SerializeField] private float _maxValue = 100f;
        [SerializeField] private float _decreasePerSecond = 10f;
        [SerializeField] private float _restorePerSecond = 15f;
        [SerializeField] private float _restoreDelay = 1f;

        private ICharacterMovementSystem _movementSystem;
        private float _restoreDelayTimer;

        public float CurrentValue
        {
            get => _currentValue;
            private set
            {
                float clampedValue = Mathf.Clamp(value, 0f, _maxValue);
                if (Mathf.Approximately(_currentValue, clampedValue)) return;

                _currentValue = clampedValue;
                OnCurrentValueChanged?.Invoke(_currentValue, _maxValue);
            }
        }

        public float MaxValue => _maxValue;

        public event System.Action<float, float> OnCurrentValueChanged;
        public event System.Action<float> OnMaxValueChanged;

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterEnduranceSystemConfig enduranceCfg) return false;

            if (!character.TryRegisterSystem<ICharacterEnduranceSystem>(this)) return false;

            _movementSystem = character.GetSystem<ICharacterMovementSystem>();

            _maxValue = Mathf.Max(0f, enduranceCfg.MaxValue);
            _decreasePerSecond = Mathf.Max(0f, enduranceCfg.DecreasePerSecond);
            _restorePerSecond = Mathf.Max(0f, enduranceCfg.RestorePerSecond);
            _restoreDelay = Mathf.Max(0f, enduranceCfg.RestoreDelay);
            CurrentValue = enduranceCfg.StartFromMaxValue
                ? _maxValue
                : Mathf.Clamp(enduranceCfg.CurrentValue, 0f, _maxValue);

            Debug.Log($"EnduranceSystem initialized: CurrentValue={_currentValue}, MaxValue={_maxValue}, DecreasePerSecond={_decreasePerSecond}, RestorePerSecond={_restorePerSecond}, RestoreDelay={_restoreDelay}");
            return true;
        }

        private void Update()
        {
            if (_movementSystem != null && _movementSystem.IsRunning)
            {
                _restoreDelayTimer = _restoreDelay;
                ReduceValue(_decreasePerSecond * Time.deltaTime);
                return;
            }

            if (_restoreDelayTimer > 0f)
            {
                _restoreDelayTimer -= Time.deltaTime;
                return;
            }

            AddValue(_restorePerSecond * Time.deltaTime);
        }

        public void AddValue(float value)
        {
            if (value <= 0f) return;

            CurrentValue += value;
        }

        public void ReduceValue(float value)
        {
            if (value <= 0f) return;

            CurrentValue -= value;
        }

        public void SetCurrentValue(float value)
        {
            CurrentValue = value;
        }

        public void SetMaxValue(float value)
        {
            if (value < 0f) return;
            if (Mathf.Approximately(_maxValue, value)) return;

            _maxValue = value;
            CurrentValue = _currentValue;
            OnMaxValueChanged?.Invoke(_maxValue);
        }
    }
}
