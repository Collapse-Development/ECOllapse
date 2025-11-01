using CharacterSystems;

namespace _Project.Code.Features.Character.MB.EffectsSystem.Effects
{
    public class PoisonEffect : CharacterBaseEffect
    {
        private ICharacterHealthSystem _healthSystem;
        private ICharacterEffectValue<float> _damageValue;

        public PoisonEffect(ICharacterEffectValue<float> damageValue, ICharacterEffectCancellationRule cancellationRule = null)
        {
            _damageValue = damageValue;
            CancellationRule = cancellationRule;

            if (cancellationRule != null)
            {
                cancellationRule.CancelRequired += base.Cancel;
            }
        }

        public override void Initialize(Character character)
        {
            base.Initialize(character);
            _healthSystem = Character.GetSystem<ICharacterHealthSystem>();
        }

        public override void Tick(float dt)
        {
            _damageValue.Tick(dt);
            
            float damage = _damageValue.GetValue();
            if (damage > 0f && _healthSystem != null)
            {
                _healthSystem.TakeDamage(damage);
            }
            
            base.Tick(dt);
        }
    }
}