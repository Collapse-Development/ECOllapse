using CharacterSystems;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.EffectsSystem.Effects
{
    public class RegenerationEffect : CharacterBaseEffect
    {
        private ICharacterHealthSystem _healthSystem;
        private ICharacterEffectValue<float> _healValue;

        public RegenerationEffect(ICharacterEffectValue<float> healValue, ICharacterEffectCancellationRule cancellationRule = null)
        {
            _healValue = healValue;
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
            _healValue.Tick(dt);

            float heal = _healValue.GetValue();
            Debug.Log($"HealValue: {heal}");

            if (heal > 0f && _healthSystem != null)
            {
                Debug.Log("HEALED");

                _healthSystem.AddHealth(heal);
            }

            base.Tick(dt);
        }
    }
}
