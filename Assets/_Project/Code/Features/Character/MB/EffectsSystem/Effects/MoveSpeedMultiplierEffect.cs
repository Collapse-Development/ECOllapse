using System;
using _Project.Code.Features.Character.MB.MovementSystem;

namespace _Project.Code.Features.Character.MB.EffectsSystem.Effects
{
    public class MoveSpeedMultiplierEffect : CharacterBaseEffect
    {
        private ICharacterMovementSystem _movementSystem;
        private ICharacterEffectValue<float> _effectValue;

        public MoveSpeedMultiplierEffect(ICharacterEffectValue<float> effectValue, ICharacterEffectCancellationRule cancellationRule = null)
        {
            _effectValue = effectValue;

            if (cancellationRule != null)
            {
                cancellationRule.CancelRequired += base.Cancel;
            }
        }

        public override void Initialize(Character character)
        {
            base.Initialize(character);

            _movementSystem = Character.GetSystem<ICharacterMovementSystem>();
        }

        public override void Tick(float dt)
        {
            _effectValue.Tick(dt);
            
            _movementSystem?.ApplyFrameSpeedMultiplier(_effectValue.GetValue());
            
            base.Tick(dt);
        }
    }
}