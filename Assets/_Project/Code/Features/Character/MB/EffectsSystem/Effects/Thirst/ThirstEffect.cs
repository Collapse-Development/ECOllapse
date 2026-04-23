using CharacterSystems;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.EffectsSystem.Effects
{
    public class ThirstEffect : CharacterBaseEffect
    {
        private ICharacterHealthSystem _healthSystem;
        private ICharacterEffectValue<float> _damageValue;

        public ThirstEffect(ICharacterEffectValue<float> damageValue, ICharacterEffectCancellationRule cancellationRule = null)
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
            _healthSystem = character.GetSystem<ICharacterHealthSystem>();
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
