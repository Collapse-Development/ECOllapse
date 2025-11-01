using UnityEngine;
using System.Collections;
using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.Character.MB.ThirstSystem
{
    public class CharacterThirstSystem : MonoBehaviour, IThirstSystem
    {
        [Header("Hydration Settings")]
        [SerializeField] private float maxHydration = 100f;
        [SerializeField] private float baseDecrementRate = 1f;
        
        [Header("Current State")]
        [SerializeField] private float currentHydration;
        [SerializeField] private ThirstStatus status;
        
        private Character _character;
        private float _decrementMultiplier = 1f;
        private float _timeAtZeroHydration = 0f;
        private bool _isInitialized = false;
        
        // ICharacterSystem implementation
        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            var thirstCfg = cfg as CharacterThirstSystemConfig;
            if (thirstCfg == null) return false;
            
            _character = character;
            if (!_character.TryRegisterSystem<IThirstSystem>(this)) return false;
            
            maxHydration = thirstCfg.MaxHydration;
            baseDecrementRate = thirstCfg.BaseDecrementRate;
            currentHydration = maxHydration;
            
            StartCoroutine(HydrationDecrementCoroutine());
            _isInitialized = true;
            
            Debug.Log($"ThirstSystem initialized: MaxHydration={maxHydration}");
            return true;
        }
        
        public void Initialize(Character character)
        {
            _character = character;
            _character.TryRegisterSystem<IThirstSystem>(this);
            currentHydration = maxHydration;
            StartCoroutine(HydrationDecrementCoroutine());
            _isInitialized = true;
        }
        
        // IThirstSystem implementation
        public float CurrentHydration => currentHydration;
        public float MaxHydration => maxHydration;
        public ThirstStatus Status => status;
        
        public void Drink(float hydrationAmount, WaterQuality quality)
        {
            if (!_isInitialized) return;
            
            // Проверка перепитья
            if (currentHydration > 90f)
            {
                ApplyOverhydrationEffects();
            }
            
            // Эффекты качества воды
            ApplyWaterQualityEffects(quality);
            
            AddHydration(hydrationAmount);
            Debug.Log($"Drank water. Hydration: {currentHydration}, Quality: {quality}");
        }
        
        public void AddHydration(float amount)
        {
            currentHydration = Mathf.Clamp(currentHydration + amount, 0f, maxHydration);
            UpdateThirstStatus();
        }
        
        public void SetHydrationDecrementMultiplier(float multiplier)
        {
            _decrementMultiplier = Mathf.Max(0f, multiplier);
        }
        
        private IEnumerator HydrationDecrementCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f); // Каждую секунду
                
                if (_isInitialized)
                {
                    float decrement = baseDecrementRate * _decrementMultiplier * Time.deltaTime;
                    currentHydration = Mathf.Max(0f, currentHydration - decrement);
                    
                    if (currentHydration <= 0f)
                    {
                        _timeAtZeroHydration += Time.deltaTime;
                    }
                    else
                    {
                        _timeAtZeroHydration = 0f;
                    }
                    
                    UpdateThirstStatus();
                    ApplyStatusEffects();
                }
            }
        }
        
        private void UpdateThirstStatus()
        {
            if (currentHydration > 90f)
            {
                status = ThirstStatus.Overhydration;
            }
            else if (_timeAtZeroHydration > 24f * 3600f) // 24 игровых часа
            {
                status = ThirstStatus.CriticalDehydration;
            }
            else if (_timeAtZeroHydration > 12f * 3600f) // 12 игровых часов
            {
                status = ThirstStatus.Dehydration;
            }
            else if (currentHydration <= 20f)
            {
                status = ThirstStatus.MildDehydration;
            }
            else
            {
                status = ThirstStatus.Normal;
            }
        }
        
        private void ApplyStatusEffects()
        {
            // Здесь будет интеграция с системами эффектов
            switch (status)
            {
                case ThirstStatus.MildDehydration:
                    // Блюр камеры, увеличение расхода выносливости
                    break;
                case ThirstStatus.Dehydration:
                    // Снижение макс. выносливости, увеличение температуры
                    break;
                case ThirstStatus.CriticalDehydration:
                    // Потеря здоровья, галлюцинации, замедление анимаций
                    break;
                case ThirstStatus.Overhydration:
                    // Снижение урона, шанс обезвоживания
                    break;
            }
        }
        
        private void ApplyOverhydrationEffects()
        {
            // Эффекты перепитья
            Debug.Log("Overhydration effects applied");
        }
        
        private void ApplyWaterQualityEffects(WaterQuality quality)
        {
            switch (quality)
            {
                case WaterQuality.Contaminated:
                    // Шанс питьевого отравления
                    break;
                case WaterQuality.Toxic:
                    // Серьезное отравление, возможна смерть
                    break;
            }
        }
    }
}