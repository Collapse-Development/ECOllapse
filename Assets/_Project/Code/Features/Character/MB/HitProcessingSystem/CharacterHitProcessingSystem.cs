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
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("HealthSystem не найдена, урон не применён");
            }
            
            OnHit?.Invoke();
        }
    }
}
