using System;

namespace _Project.Code.Features.Character.MB.EffectsSystem.CancellationRules
{
    public class TimeCancellationRule : ICharacterEffectCancellationRule
    {
        public event Action CancelRequired;

        private float _duration;
        private float _currentTime;

        public TimeCancellationRule(float duration)
        {
            _duration = duration;
            _currentTime = 0f;
        }

        public void Tick(float dt)
        {
            _currentTime += dt;
            
            if (_currentTime >= _duration)
            {
                CancelRequired?.Invoke();
            }
        }
    }
}