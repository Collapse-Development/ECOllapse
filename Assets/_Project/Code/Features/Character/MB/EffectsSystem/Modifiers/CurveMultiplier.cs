using UnityEngine;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public class CurveMultiplier : ICharacterEffectModifier
    {
        private readonly AnimationCurve _curve;
        private readonly float _duration;
        private float _elapsed;
        private bool _cancelled;
        private float _factor;
        
        public CurveMultiplier(AnimationCurve curve, float duration)
        {
            _curve = curve;
            _duration = duration;
            Evaluate(0f);
        }

        public void Evaluate(float dt)
        {
            _elapsed += dt;
            float normalizedTime = Mathf.Clamp01(_elapsed / _duration);
            _factor = _curve.Evaluate(normalizedTime);
        }

        public void Apply(ref float value)
        {
            value *= _factor;
        }

        public bool IsExpired => _elapsed >= _duration || _cancelled;
        
        public void Cancel()
        {
            _cancelled = true;
        }
    }
}
