namespace _Project.Code.Features.Character.MB.EffectsSystem.EffectValues
{
    public class PeriodicEffectValue<T> : ICharacterEffectValue<T>
    {
        private T _value;
        private T _defaultValue;
        private float _interval;
        private float _currentTime;
        private bool _shouldTrigger;

        public PeriodicEffectValue(T value, T defaultValue, float interval)
        {
            _value = value;
            _defaultValue = defaultValue;
            _interval = interval;
            _currentTime = 0f;
            _shouldTrigger = false;
        }

        public void Tick(float dt)
        {
            _currentTime += dt;
            
            if (_currentTime >= _interval)
            {
                _shouldTrigger = true;
                _currentTime = 0f;
            }
            else
            {
                _shouldTrigger = false;
            }
        }

        public T GetValue()
        {
            return _shouldTrigger ? _value : _defaultValue;
        }
    }
}