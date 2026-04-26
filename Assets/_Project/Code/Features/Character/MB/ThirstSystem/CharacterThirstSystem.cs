using _Project.Code.Features.Character.MB.MovementSystem;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.Thirst
{
    public class CharacterThirstSystem : MonoBehaviour, ICharacterThirstSystem
    {
        [SerializeField] private float _currentValue;
        [SerializeField] private float _maxValue = 100f;
        [SerializeField] private float _decreasePerSecond = 1f;
        [SerializeField] private float _runningDecreaseMultiplier = 2f;

        private ICharacterMovementSystem _movementSystem;

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
        public float Hydration => CurrentValue;

        public event System.Action<float, float> OnCurrentValueChanged;
        public event System.Action<float> OnMaxValueChanged;

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterThirstSystemConfig thirstCfg) return false;

            if (!character.TryRegisterSystem<ICharacterThirstSystem>(this)) return false;

            _movementSystem = character.GetSystem<ICharacterMovementSystem>();

            _maxValue = Mathf.Max(0f, thirstCfg.MaxValue);
            _decreasePerSecond = Mathf.Max(0f, thirstCfg.DecreasePerSecond);
            _runningDecreaseMultiplier = Mathf.Max(0f, thirstCfg.RunningDecreaseMultiplier);
            CurrentValue = thirstCfg.StartFromMaxValue
                ? _maxValue
                : Mathf.Clamp(thirstCfg.CurrentValue, 0f, _maxValue);

            Debug.Log($"ThirstSystem initialized: CurrentValue={_currentValue}, MaxValue={_maxValue}, DecreasePerSecond={_decreasePerSecond}, RunningDecreaseMultiplier={_runningDecreaseMultiplier}");
            return true;
        }

        private void Update()
        {
            if (_decreasePerSecond <= 0f || CurrentValue <= 0f) return;

            float multiplier = _movementSystem != null && _movementSystem.IsRunning
                ? _runningDecreaseMultiplier
                : 1f;

            ReduceValue(_decreasePerSecond * multiplier * Time.deltaTime);
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
