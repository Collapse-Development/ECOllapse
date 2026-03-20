using _Project.Code.Features.Character.MB;

namespace CharacterSystems
{
    public interface ICharacterDamageResistanceSystem : ICharacterSystem
    {
        float Resistance { get; set; }

        /// <summary>
        /// Применяет формулу: max(0, baseDamage * e^(-k * Resistance) - Dabs)
        /// </summary>
        float CalculateEffectiveDamage(float baseDamage);
    }
}
