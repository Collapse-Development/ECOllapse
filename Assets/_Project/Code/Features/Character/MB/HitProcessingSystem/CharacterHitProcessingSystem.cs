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

        public event Action OnHit;

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterHitProcessingSystemConfig systemCfg)
                return false;

            _character = character;
            if (!_character.TryRegisterSystem<ICharacterHitProcessingSystem>(this))
                return false;

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
            var resistanceSystem = _character.GetSystem<ICharacterDamageResistanceSystem>();

            if (healthSystem != null)
            {
                float finalDamage = damage;

                if (resistanceSystem != null)
                {
                    float R = resistanceSystem.Resistance; 
                    float k = 0.05f; 

                    finalDamage = damage * Mathf.Exp(-k * R);

                    finalDamage = Mathf.Max(0f, finalDamage);
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
