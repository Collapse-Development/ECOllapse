using UnityEngine;
using _Project.Code.Features.Character.MB;

namespace CharacterSystems
{
    public class CharacterDamageResistanceSystem : MonoBehaviour, ICharacterDamageResistanceSystem
    {
        [SerializeField] private float _resistance;
        [SerializeField] private Character _character;

        public float Resistance
        {
            get => _resistance;
            set => _resistance = value;
        }

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterDamageResistanceSystemConfig resistanceCfg) return false;
            
            _character = character;
            if (!_character.TryRegisterSystem<ICharacterDamageResistanceSystem>(this)) return false;
            
            _resistance = resistanceCfg.BaseResistance;
            
            Debug.Log($"DamageResistanceSystem initialized with config: Resistance={_resistance}");
            return true;
        }
    }
}
