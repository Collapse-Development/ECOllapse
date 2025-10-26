using UnityEngine;
using System;
using Project.Code.Features.Character.MB;

namespace Project.Code.Features.Character.MB.HitProcessingSystem
{
    [DisallowMultipleComponent]
    public class CharacterHitProcessingSystem : MonoBehaviour, IHitProcessingSystem
    {
        [SerializeField] private Character _character;

        public event Action OnHit;

        private bool _isRegistered;

        private void Awake()
        {
            if (_character == null)
            {
                Debug.LogError("CharacterHitProcessingSystem: поле _character не назначено!", this);
                return;
            }

            _isRegistered = _character.TryRegisterSystem<IHitProcessingSystem>(this);
            if (!_isRegistered)
            {
                Debug.LogWarning("IHitProcessingSystem уже зарегистрирована для этого персонажа", this);
            }
        }

        public void ProcessHit(float damage)
        {
            if (_character == null)
            {
                Debug.LogWarning("Character не назначен в HitProcessingSystem");
                return;
            }

            OnHit?.Invoke();

            var healthSystem = _character.GetSystem<IHealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.ApplyDamage(damage);
            }
            else
            {
                Debug.LogWarning("HealthSystem не найдена, урон не применён");
            }
        }
    }
}
