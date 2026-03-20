using UnityEngine;
using _Project.Code.Features.Character.MB;

namespace CharacterSystems
{
    /// <summary>
    /// Универсальная система сопротивления урону для игрока и NPC.
    /// Формула: EffectiveDamage(R) = max(0, Dbase * e^(-k*R) - Dabs)
    /// </summary>
    public class CharacterDamageResistanceSystem : MonoBehaviour, ICharacterDamageResistanceSystem
    {
        [SerializeField] private float _resistance;
        [SerializeField] private float _resistanceDecayK = 0.05f;
        [SerializeField] private float _absorbedDamage = 1f;
        [SerializeField] private Character _character;

        public float Resistance
        {
            get => _resistance;
            set => _resistance = Mathf.Max(0f, value);
        }

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterDamageResistanceSystemConfig resistanceCfg) return false;

            _character = character;
            if (!_character.TryRegisterSystem<ICharacterDamageResistanceSystem>(this)) return false;

            _resistance = resistanceCfg.BaseResistance;
            _resistanceDecayK = resistanceCfg.ResistanceDecayK;
            _absorbedDamage = resistanceCfg.AbsorbedDamage;

            return true;
        }

        /// <summary>
        /// Возвращает эффективный урон с учётом сопротивления.
        /// EffectiveDamage = max(0, baseDamage * e^(-k * R) - Dabs)
        /// </summary>
        public float CalculateEffectiveDamage(float baseDamage)
        {
            float attenuated = baseDamage * Mathf.Exp(-_resistanceDecayK * _resistance);
            return Mathf.Max(0f, attenuated - _absorbedDamage);
        }
    }
}
