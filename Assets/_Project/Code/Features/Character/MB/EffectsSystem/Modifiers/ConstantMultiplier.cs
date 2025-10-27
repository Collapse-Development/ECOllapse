namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public class ConstantMultiplier : ICharacterEffectModifier
    {
        private readonly float _factor;
        private bool _cancelled;
        
        public ConstantMultiplier(float factor)
        {
            _factor = factor;
        }

        public void Evaluate(float dt)
        {
            return;
        }

        public void Apply(ref float value)
        {
            value *= _factor;
        }

        public bool IsExpired => _cancelled;
        
        public void Cancel()
        {
            _cancelled = true;
        }
    }
}
