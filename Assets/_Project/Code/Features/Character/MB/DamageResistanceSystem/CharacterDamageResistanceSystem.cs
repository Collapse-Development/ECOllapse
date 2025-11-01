using UnityEngine;
using System.Collections.Generic;
using _Project.Code.Features.Character.MB;

namespace CharacterSystems
{
    public class CharacterDamageResistanceSystem : MonoBehaviour, ICharacterDamageResistanceSystem
    {
        [SerializeField] private float _baseResistance;
        [SerializeField] private Character _character;
        private Dictionary<string, float> _resistanceModifiers = new Dictionary<string, float>();

        public float TotalResistance => CalculateTotalResistance();

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterDamageResistanceSystemConfig resistanceCfg) return false;
            
            _character = character;
            if (!_character.TryRegisterSystem<ICharacterDamageResistanceSystem>(this)) return false;
            
            _baseResistance = resistanceCfg.BaseResistance;
            
            Debug.Log($"DamageResistanceSystem initialized with config: BaseResistance={_baseResistance}");
            return true;
        }

        public void AddResistance(string key, float value)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("Cannot add resistance with null or empty key");
                return;
            }

            _resistanceModifiers[key] = value;
        }

        public bool RemoveResistance(string key)
        {
            return _resistanceModifiers.Remove(key);
        }

        public float CalculateDamage(float incomingDamage)
        {
            float resistance = Mathf.Clamp01(TotalResistance);
            return incomingDamage * (1f - resistance);
        }

        public void ClearAllModifiers()
        {
            _resistanceModifiers.Clear();
        }

        public bool TryGetResistance(string key, out float value)
        {
            return _resistanceModifiers.TryGetValue(key, out value);
        }

        private float CalculateTotalResistance()
        {
            float total = _baseResistance;
            foreach (var modifier in _resistanceModifiers.Values)
            {
                total += modifier;
            }
            return Mathf.Clamp(total, 0f, 1f);
        }
    }
}
