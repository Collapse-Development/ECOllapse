namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public static class CharacterEffectsHelper
    {
        public static ICharacterEffect CreateEffect(CharacterEffects effect)
        {
            return effect switch
            {
                CharacterEffects.MovementSpeed => new MovementSpeedEffect(),
                _ => null
            };
        }
    }
}