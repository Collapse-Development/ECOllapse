using _Project.Code.Features.Character.MB.EffectsSystem.CancellationRules;
using _Project.Code.Features.Character.MB.EffectsSystem.EffectValues;
using _Project.Code.Features.Character.MB.Thirst;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.EffectsSystem.Effects.Thirst
{
    public static class ThirstUtility
    {
        public static void ApplyThirstEffect(Character character, float damagePerTick, float tickInterval)
        {
            var effectsSystem = character.GetSystem<ICharacterEffectsSystem>();
            if (effectsSystem == null) return;

            var thirstSystem = character.GetSystem<ICharacterThirstSystem>();
            if (thirstSystem == null) return;

            var periodicDamage = new PeriodicEffectValue<float>(damagePerTick, 0f, tickInterval);
            var cancellationRule = new ThirstCancellationRule(thirstSystem);

            var thirstEffect = new ThirstEffect(periodicDamage, cancellationRule);

            effectsSystem.AddEffect(thirstEffect);
        }

        public static void ApplyThirstEffect(Character character, float damagePerSecond)
        {
            ApplyThirstEffect(character, damagePerSecond, 1f);
        }
    }
}
