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

        public bool TryRegister(Character character)
        {
            _character = character;
            return _character.TryRegisterSystem<ICharacterDamageResistanceSystem>(this);
        }

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterDamageResistanceSystemConfig resistanceCfg) return false;
            
            _resistance = resistanceCfg.BaseResistance;
            
            Debug.Log($"DamageResistanceSystem initialized with config: Resistance={_resistance}");
            return true;
        }

        public bool TryResolveDependencies(Character character)
        {
            return character != null;
        }
    }
}
