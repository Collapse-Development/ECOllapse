using UnityEngine;
using System;
using _Project.Code.Features.Character.Configurations.Systems;
using _Project.Code.Features.Character.MB;
using Project.Code.Features.Character.MB.HitProcessingSystem;

namespace CharacterSystems
{
    public class CharacterHitProcessingSystem : MonoBehaviour, ICharacterHitProcessingSystem
    {
        private Character _character;
        private ICharacterHealthSystem _healthSystem;

        public event Action OnHit;
        
        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterHitProcessingSystemConfig systemCfg) return false;
            
            _character = character;
            if (!_character.TryRegisterSystem<ICharacterHitProcessingSystem>(this)) return false;
            
            Debug.Log($"HitProcessing initialized");
            return true;
        }

        public void ProcessHit(float damage)
        {
            if (_character == null)
            {
                Debug.LogWarning("Character не назначен в HitProcessingSystem");
                return;
            }

            var healthSystem = _character.GetSystem<ICharacterHealthSystem>();
            var resistanceSystem = _character.GetSystem<ICharacterDamageResistanceSystem>(); // ✅ добавлено

            if (healthSystem != null)
            {
                // ✅ Учитываем сопротивление урону
                float finalDamage = damage;
                if (resistanceSystem != null)
                {
                    float resistance = Mathf.Clamp01(resistanceSystem.DamageResistance); // от 0 до 1
                    finalDamage *= 1f - resistance;
                    Debug.Log($"Damage adjusted by resistance ({resistance * 100f}%). Final damage: {finalDamage}");
                }

                healthSystem.TakeDamage(finalDamage);
            }
            else
            {
                Debug.LogWarning("HealthSystem не найдена, урон не применён");
            }
            
            OnHit?.Invoke();
        }
    }
}

