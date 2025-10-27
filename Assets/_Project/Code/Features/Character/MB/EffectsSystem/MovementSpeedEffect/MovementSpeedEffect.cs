namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public class MovementSpeedEffect : BaseEffect, IMovementSpeedEffect
    {
        private float _speedMultiplier;
        public float SpeedMultiplier => _speedMultiplier;

        protected override void ComputeResult()
        {
            _speedMultiplier = 1.0f;
            
            foreach (var modifier in Modifiers)
            {
                modifier.Apply(ref _speedMultiplier);
            }
        }
    }
}
