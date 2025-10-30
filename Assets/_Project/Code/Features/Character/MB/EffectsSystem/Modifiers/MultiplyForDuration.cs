namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public class MultiplyForDuration : ICharacterEffectModifier
    {
        private readonly float _factor;
        private readonly float _duration;
        private float _elapsed;
        private bool _cancelled;
        public MultiplyForDuration(float factor, float duration)
        {
            _factor = factor;
            _duration = duration;
        }

        public void Evaluate(float dt)
        {
            _elapsed += dt;
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
