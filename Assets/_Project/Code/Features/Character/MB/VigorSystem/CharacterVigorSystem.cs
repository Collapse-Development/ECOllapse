using UnityEngine;

namespace _Project.Code.Features.Character.MB.Vigor
{
    public class CharacterVigorSystem : MonoBehaviour, ICharacterVigorSystem
    {
        [SerializeField] private float _currentValue;
        [SerializeField] private float _maxValue = 100f;
        [SerializeField] private float _decreasePerSecond = 1f;

        public float CurrentValue
        {
            get => _currentValue;
            private set
            {
                float clampedValue = Mathf.Clamp(value, 0f, _maxValue);
                if (Mathf.Approximately(_currentValue, clampedValue)) return;

                _currentValue = clampedValue;
                OnValueChanged?.Invoke(_currentValue, _maxValue);
            }
        }

        public float MaxValue => _maxValue;
        public float Vigor => CurrentValue;

        public event System.Action<float, float> OnValueChanged;
        public event System.Action<float> OnMaxValueChanged;

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterVigorSystemConfig vigorCfg) return false;

            if (!character.TryRegisterSystem<ICharacterVigorSystem>(this)) return false;

            _maxValue = Mathf.Max(0f, vigorCfg.MaxValue);
            _decreasePerSecond = Mathf.Max(0f, vigorCfg.DecreasePerSecond);
            CurrentValue = vigorCfg.StartFromMaxValue
                ? _maxValue
                : Mathf.Clamp(vigorCfg.CurrentValue, 0f, _maxValue);

            Debug.Log($"VigorSystem initialized: CurrentValue={_currentValue}, MaxValue={_maxValue}, DecreasePerSecond={_decreasePerSecond}");
            return true;
        }

        private void Update()
        {
            if (_decreasePerSecond <= 0f || CurrentValue <= 0f) return;

            ReduceValue(_decreasePerSecond * Time.deltaTime);
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
