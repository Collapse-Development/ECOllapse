using UnityEngine;
using System.Collections;
using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.Character.MB.EnduranceSystem
{
    public class CharacterEnduranceSystem : MonoBehaviour, IEnduranceSystem
    {
        [Header("Endurance Settings")]
        [SerializeField] private float maxEndurance = 100f;
        [SerializeField] private float minEndurance = -20f;
        [SerializeField] private float baseStaminaForRegen = 50f; // Временное значение вместо системы выносливости
        
        [Header("Current State")]
        [SerializeField] private float currentEndurance;
        [SerializeField] private bool isStunned;
        [SerializeField] private float stunDuration;
        
        private Character _character;
        private bool _isInitialized = false;
        private Coroutine _stunCoroutine;
        
        // ICharacterSystem implementation
        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            var enduranceCfg = cfg as CharacterEnduranceSystemConfig;
            if (enduranceCfg == null) return false;
            
            _character = character;
            if (!_character.TryRegisterSystem<IEnduranceSystem>(this)) return false;
            
            maxEndurance = enduranceCfg.MaxEndurance;
            minEndurance = enduranceCfg.MinEndurance;
            currentEndurance = maxEndurance;
            
            StartCoroutine(EnduranceRegenCoroutine());
            _isInitialized = true;
            
            Debug.Log($"EnduranceSystem initialized: MaxEndurance={maxEndurance}");
            return true;
        }
        
        // IEnduranceSystem implementation
        public float CurrentEndurance => currentEndurance;
        public float MaxEndurance => maxEndurance;
        public bool IsStunned => isStunned;
        
        public void TakeEnduranceDamage(float damage, EnduranceDamageType damageType)
        {
            if (!_isInitialized || isStunned) return;
            
            float finalDamage = CalculateFinalDamage(damage, damageType);
            currentEndurance = Mathf.Max(minEndurance, currentEndurance - finalDamage);
            
            Debug.Log($"Endurance damage: {finalDamage}. Current: {currentEndurance}");
            
            // Проверка на оглушение
            if (currentEndurance <= 0f)
            {
                float stunPower = Mathf.Abs(currentEndurance);
                ApplyStun(CalculateStunDuration(stunPower));
                currentEndurance = 0f; // Возвращаем к 0 после пробития
            }
        }
        
        public void ApplyStun(float duration)
        {
            if (_stunCoroutine != null)
                StopCoroutine(_stunCoroutine);
            
            _stunCoroutine = StartCoroutine(StunCoroutine(duration));
        }
        
        private float CalculateFinalDamage(float baseDamage, EnduranceDamageType damageType)
        {
            float multiplier = 1f;
            
            switch (damageType)
            {
                case EnduranceDamageType.Standing:
                    multiplier = 0.7f; // Меньше урона стоя
                    break;
                case EnduranceDamageType.Jumping:
                    multiplier = 1.5f; // Больше урона в прыжке
                    break;
                case EnduranceDamageType.Falling:
                    multiplier = 2f; // Особый урон от падения
                    break;
            }
            
            return baseDamage * multiplier;
        }
        
        private float CalculateStunDuration(float stunPower)
        {
            // Длительность оглушения зависит от силы пробития стойкости
            // Максимальная длительность - 5 секунд
            float baseDuration = stunPower * 0.1f;
            return Mathf.Min(baseDuration, 5f);
        }
        
        private IEnumerator EnduranceRegenCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                
                if (_isInitialized && !isStunned && currentEndurance < maxEndurance)
                {
                    // Временная реализация без зависимости от системы выносливости
                    // Формула: EnduranceRegen = 10 + (Stamina/10)
                    float regenAmount = 10f + (baseStaminaForRegen / 10f);
                    currentEndurance = Mathf.Min(maxEndurance, currentEndurance + regenAmount * Time.deltaTime);
                }
            }
        }
        
        private IEnumerator StunCoroutine(float duration)
        {
            isStunned = true;
            Debug.Log($"Stunned for {duration} seconds");
            
            // Применяем эффекты оглушения
            ApplyStunEffects(true);
            
            yield return new WaitForSeconds(duration);
            
            // Снимаем эффекты оглушения
            ApplyStunEffects(false);
            isStunned = false;
            
            Debug.Log("Stun ended");
        }
        
        private void ApplyStunEffects(bool apply)
        {
            // Базовая реализация эффектов оглушения
            if (apply)
            {
                // Можно добавить визуальные эффекты или звуки
                Debug.Log("STUN: Player lost control");
            }
            else
            {
                Debug.Log("STUN: Player regained control");
            }
        }
        
        // Метод для внешних систем (например, боевой системы)
        public float GetKnockbackMultiplier()
        {
            // Чем меньше стойкости, тем сильнее откидывание
            float normalizedEndurance = currentEndurance / maxEndurance;
            return 2f - normalizedEndurance; // От 1x до 2x множитель
        }
        
        // Метод для будущей интеграции с системой выносливости
        public void ConnectToStaminaSystem(float staminaValue)
        {
            baseStaminaForRegen = staminaValue;
            Debug.Log("Endurance system connected to stamina system");
        }
        
        // Метод для ручной установки выносливости (временно)
        public void SetBaseStamina(float stamina)
        {
            baseStaminaForRegen = Mathf.Max(0f, stamina);
        }
    }
}