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

        public bool TryRegister(Character character)
        {
            _character = character;
            return _character.TryRegisterSystem<ICharacterHitProcessingSystem>(this);
        }
        
        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterHitProcessingSystemConfig systemCfg) return false;
            
            Debug.Log($"HitProcessing initialized");
            return true;
        }

        public bool TryResolveDependencies(Character character)
        {
            _healthSystem = character.GetSystem<ICharacterHealthSystem>();
            return true;
        }

        public void ProcessHit(float damage)
        {
            if (_character == null)
            {
                Debug.LogWarning("Character не назначен в HitProcessingSystem");
                return;
            }

            if (_healthSystem != null)
            {
                _healthSystem.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("HealthSystem не найдена, урон не применён");
            }
            
            OnHit?.Invoke();
        }
    }
}
