using UnityEngine;
using System.Collections;
using CharacterSystems;

namespace _Project.Code.Features.Character.MB.HungerSystem
{
    public class CharacterHungerSystem : MonoBehaviour, IHungerSystem
    {
        [Header("Hunger Settings")]
        [SerializeField] private float maxHunger = 100f;
        [SerializeField] private float baseDecrementRate = 1f;
        
        [Header("Current State")]
        [SerializeField] private float currentHunger;
        [SerializeField] private HungerStatus status;
        
        private Character _character;
        private float _decrementMultiplier = 1f;
        private bool _isInitialized = false;
        
        // ICharacterSystem implementation
        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            var hungerCfg = cfg as CharacterHungerSystemConfig;
            if (hungerCfg == null) return false;
            
            _character = character;
            if (!_character.TryRegisterSystem<IHungerSystem>(this)) return false;
            
            maxHunger = hungerCfg.MaxHunger;
            baseDecrementRate = hungerCfg.BaseDecrementRate;
            currentHunger = maxHunger;
            
            StartCoroutine(HungerDecrementCoroutine());
            _isInitialized = true;
            
            Debug.Log($"HungerSystem initialized: MaxHunger={maxHunger}");
            return true;
        }
        
        public void Initialize(Character character)
        {
            _character = character;
            _character.TryRegisterSystem<IHungerSystem>(this);
            currentHunger = maxHunger;
            StartCoroutine(HungerDecrementCoroutine());
            _isInitialized = true;
        }
        
        // IHungerSystem implementation
        public float CurrentHunger => currentHunger;
        public float MaxHunger => maxHunger;
        public HungerStatus Status => status;
        
        public void AddHunger(float amount)
        {
            currentHunger = Mathf.Clamp(currentHunger + amount, 0f, maxHunger);
            UpdateHungerStatus();
        }
        
        public void ConsumeFood(float nutritionValue)
        {
            if (!_isInitialized) return;
            
            AddHunger(nutritionValue);
            Debug.Log($"Consumed food. Nutrition: {nutritionValue}, Current hunger: {currentHunger}");
            
            // Можно добавить эффекты от разных типов еды
        }
        
        public void SetHungerDecrementMultiplier(float multiplier)
        {
            _decrementMultiplier = Mathf.Max(0f, multiplier);
        }
        
        private IEnumerator HungerDecrementCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f); // Каждую секунду
                
                if (_isInitialized)
                {
                    // Уменьшаем голод со временем
                    float decrement = baseDecrementRate * _decrementMultiplier * Time.deltaTime;
                    currentHunger = Mathf.Max(0f, currentHunger - decrement);
                    
                    UpdateHungerStatus();
                    ApplyStatusEffects();
                }
            }
        }
        
        private void UpdateHungerStatus()
        {
            float hungerPercentage = (currentHunger / maxHunger) * 100f;
            
            if (hungerPercentage >= 80f)
                status = HungerStatus.Full;
            else if (hungerPercentage >= 50f)
                status = HungerStatus.Normal;
            else if (hungerPercentage >= 20f)
                status = HungerStatus.Hungry;
            else if (hungerPercentage >= 5f)
                status = HungerStatus.VeryHungry;
            else
                status = HungerStatus.Starving;
        }
        
        private void ApplyStatusEffects()
        {
            // Интеграция с другими системами для применения эффектов голода
            switch (status)
            {
                case HungerStatus.Hungry:
                    // Легкие дебаффы: небольшое снижение регенерации выносливости
                    break;
                case HungerStatus.VeryHungry:
                    // Средние дебаффы: снижение скорости, урона
                    break;
                case HungerStatus.Starving:
                    // Серьезные дебаффы: потеря здоровья, сильное снижение характеристик
                    ApplyStarvationEffects();
                    break;
            }
        }
        
        private void ApplyStarvationEffects()
        {
            // Потеря здоровья при голодании
            // Можно интегрировать с системой здоровья
            var healthSystem = _character?.GetSystem<ICharacterHealthSystem>();
            if (healthSystem != null)
            {
                // healthSystem.TakeDamage(starvationDamage * Time.deltaTime);
            }
        }
        
        // Метод для внешних систем (например, активности)
        public void ApplyActivityMultiplier(bool isActive)
        {
            _decrementMultiplier = isActive ? 1.5f : 1f;
        }
        
        // Метод для температурных эффектов (из ГДД)
        public void ApplyTemperatureMultiplier(float multiplier)
        {
            _decrementMultiplier *= multiplier;
        }
    }
}