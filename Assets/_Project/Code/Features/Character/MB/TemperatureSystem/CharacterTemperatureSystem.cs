using UnityEngine;
using System.Collections;
using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.Character.MB.TemperatureSystem
{
    public class CharacterTemperatureSystem : MonoBehaviour, ITemperatureSystem
    {
        [Header("Temperature Settings")]
        [SerializeField] private float basePlayerTemperature = 36.6f;
        [SerializeField] private float comfortTemperature = 22f;
        
        [Header("Current State")]
        [SerializeField] private float playerTemperature;
        [SerializeField] private float environmentTemperature;
        [SerializeField] private float humidity;
        [SerializeField] private TemperatureStatus status;
        [SerializeField] private float coldResistance;
        [SerializeField] private float heatResistance;
        
        private Character _character;
        private bool _isInitialized = false;
        
        // ICharacterSystem implementation
        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            var tempCfg = cfg as CharacterTemperatureSystemConfig;
            if (tempCfg == null) return false;
            
            _character = character;
            if (!_character.TryRegisterSystem<ITemperatureSystem>(this)) return false;
            
            basePlayerTemperature = tempCfg.BasePlayerTemperature;
            comfortTemperature = tempCfg.ComfortTemperature;
            playerTemperature = basePlayerTemperature;
            
            StartCoroutine(TemperatureUpdateCoroutine());
            _isInitialized = true;
            
            Debug.Log($"TemperatureSystem initialized");
            return true;
        }
        
        // ITemperatureSystem implementation
        public float PlayerTemperature => playerTemperature;
        public float EnvironmentTemperature => environmentTemperature;
        public TemperatureStatus Status => status;
        public float ColdResistance => coldResistance;
        public float HeatResistance => heatResistance;
        
        public void SetEnvironmentTemperature(float temperature, float humidity)
        {
            this.environmentTemperature = temperature;
            this.humidity = Mathf.Clamp01(humidity);
        }
        
        public void SetResistance(float coldResist, float heatResist)
        {
            coldResistance = Mathf.Clamp(coldResist, 0f, 50f);
            heatResistance = Mathf.Clamp(heatResist, 0f, 50f);
        }
        
        private IEnumerator TemperatureUpdateCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                
                if (_isInitialized)
                {
                    UpdatePlayerTemperature();
                    UpdateTemperatureStatus();
                    ApplyTemperatureEffects();
                }
            }
        }
        
        private void UpdatePlayerTemperature()
        {
            // Рассчитываем ощутимую температуру с учетом влажности
            float feltTemperature = CalculateFeltTemperature();
            
            // Рассчитываем изменение температуры игрока
            float deltaT = CalculateTemperatureDelta(feltTemperature);
            
            playerTemperature += deltaT * Time.deltaTime;
            
            // Проверка на смерть от температуры
            if (playerTemperature < 28f || playerTemperature > 41f)
            {
                HandleTemperatureDeath();
            }
        }
        
        private float CalculateFeltTemperature()
        {
            // Формула ощутимой температуры: Тout + IF(Tout > Tcomf, H*12, -H*8)/100
            if (environmentTemperature > comfortTemperature)
            {
                return environmentTemperature + (humidity * 12f) / 100f;
            }
            else
            {
                return environmentTemperature - (humidity * 8f) / 100f;
            }
        }
        
        private float CalculateTemperatureDelta(float feltTemp)
        {
            // Формула из скриншота: ΔT = k * sign(Tout - Tcomf) * (|Tout - Tcomf| - S)
            float temperatureDiff = feltTemp - comfortTemperature;
            float sign = Mathf.Sign(temperatureDiff);
            float absDiff = Mathf.Abs(temperatureDiff);
            
            // Рассчитываем параметр влияния сопротивления S
            float S = CalculateResistanceInfluence(absDiff, temperatureDiff > 0);
            
            // Базовый коэффициент k (можно настроить в конфиге)
            float k = 0.1f;
            
            float deltaT = k * sign * (absDiff - S);
            return deltaT;
        }
        
        private float CalculateResistanceInfluence(float temperatureDiff, bool isHeat)
        {
            float resistance = isHeat ? heatResistance : coldResistance;
            
            // Параболическая зависимость S от сопротивления
            // S = a * resistance^2 + b * resistance + c
            // Коэффициенты нужно подобрать на основе таблицы из ГДД
            float a = isHeat ? 0.008f : 0.012f;
            float b = isHeat ? 0.2f : 0.3f;
            float c = isHeat ? 0f : 0f;
            
            return a * resistance * resistance + b * resistance + c;
        }
        
        private void UpdateTemperatureStatus()
        {
            if (playerTemperature >= 41f) status = TemperatureStatus.HeatStrokeDeath;
            else if (playerTemperature >= 39.5f) status = TemperatureStatus.SevereHyperthermia;
            else if (playerTemperature >= 38.5f) status = TemperatureStatus.ModerateHyperthermia;
            else if (playerTemperature >= 37.5f) status = TemperatureStatus.MildHyperthermia;
            else if (playerTemperature >= 35f) status = TemperatureStatus.Normal;
            else if (playerTemperature >= 33f) status = TemperatureStatus.MildHypothermia;
            else if (playerTemperature >= 30f) status = TemperatureStatus.ModerateHypothermia;
            else if (playerTemperature >= 28f) status = TemperatureStatus.SevereHypothermia;
            else status = TemperatureStatus.FreezingDeath;
        }
        
        private void ApplyTemperatureEffects()
        {
            // Применяем дебаффы в зависимости от статуса температуры
            switch (status)
            {
                case TemperatureStatus.MildHypothermia:
                    ApplyDebuff(0.15f, 0.10f, 0.10f, 0.15f, 0.05f, 0.10f);
                    break;
                case TemperatureStatus.ModerateHypothermia:
                    ApplyDebuff(0.25f, 0.20f, 0.20f, 0.30f, 0.10f, 0.25f);
                    break;
                case TemperatureStatus.SevereHypothermia:
                    ApplyDebuff(0.50f, 0.30f, 0.30f, 0.45f, 0.16f, 0.35f);
                    break;
                case TemperatureStatus.MildHyperthermia:
                    ApplyDebuff(0.10f, 0.15f, 0.10f, 0.15f, 0.05f, 0.05f);
                    break;
                case TemperatureStatus.ModerateHyperthermia:
                    ApplyDebuff(0.20f, 0.25f, 0.20f, 0.30f, 0.10f, 0.15f);
                    break;
                case TemperatureStatus.SevereHyperthermia:
                    ApplyDebuff(0.30f, 0.50f, 0.30f, 0.45f, 0.15f, 0.25f);
                    break;
                case TemperatureStatus.Normal:
                    // Нет дебаффов
                    break;
            }
        }
        
        private void ApplyDebuff(float hungerMultiplier, float thirstMultiplier, 
                               float staminaMultiplier, float damageReduction,
                               float resistanceReduction, float animationSpeedReduction)
        {
            // Интеграция с другими системами для применения дебаффов
            // Здесь будут вызовы методов других систем
        }
        
        private void HandleTemperatureDeath()
        {
            Debug.Log("Player died from temperature extremes");
            // Интеграция с системой смерти
        }
        
        // Метод для получения минимальной/максимальной переносимой температуры
        public float GetMinTolerableTemperature(bool withHumidity = false)
        {
            // Расчет на основе coldResistance и таблицы из ГДД
            // Упрощенная реализация - нужно доработать по таблице
            return -8f - (coldResistance * 0.9f);
        }
        
        public float GetMaxTolerableTemperature(bool withHumidity = false)
        {
            // Расчет на основе heatResistance и таблицы из ГДД
            // Упрощенная реализация - нужно доработать по таблице
            return 47f + (heatResistance * 0.74f);
        }
    }
}