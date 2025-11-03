using _Project.Code.Features.Character.MB.EffectsSystem.CancellationRules;
using _Project.Code.Features.Character.MB.EffectsSystem.Effects;
using _Project.Code.Features.Character.MB.EffectsSystem.EffectValues;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public static class PoisonUtility
    {
        /// <summary>
        /// Applies poison effect to a character
        /// </summary>
        /// <param name="character">Target character</param>
        /// <param name="damagePerTick">Damage dealt every interval</param>
        /// <param name="tickInterval">Time between damage ticks in seconds</param>
        /// <param name="duration">Total duration of poison effect in seconds</param>
        public static void ApplyPoison(Character character, float damagePerTick, float tickInterval, float duration)
        {
            var effectsSystem = character.GetSystem<ICharacterEffectsSystem>();
            if (effectsSystem == null) return;

            var periodicDamage = new PeriodicEffectValue<float>(damagePerTick, 0f, tickInterval);
            var timeCancellation = new TimeCancellationRule(duration);
            var poisonEffect = new PoisonEffect(periodicDamage, timeCancellation);

            effectsSystem.AddEffect(poisonEffect);
        }

        /// <summary>
        /// Applies poison effect with default 1 second tick interval
        /// </summary>
        /// <param name="character">Target character</param>
        /// <param name="damagePerSecond">Damage dealt every second</param>
        /// <param name="duration">Total duration of poison effect in seconds</param>
        public static void ApplyPoison(Character character, float damagePerSecond, float duration)
        {
            ApplyPoison(character, damagePerSecond, 1f, duration);
        }
    }
}