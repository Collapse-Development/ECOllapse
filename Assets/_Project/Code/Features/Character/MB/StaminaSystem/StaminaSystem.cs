using System;
using UnityEngine;
using UnityEngine.Events;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.Configurations.Systems;

namespace _Project.Code.Features.Character.Systems
{
    public class StaminaSystem : MonoBehaviour, ICharacterSystem
    {
        [Header("Текущее состояние")]
        [SerializeField] private float currentStamina = 100f;
        [SerializeField] private bool isAsleep = false;
        [SerializeField] private float sleepQuality = 1f;
        [SerializeField] private int daysAtZero = 0;
        
        [Header("События")]
        public UnityEvent<float> OnStaminaChanged = new();
        public UnityEvent OnStaminaWarning = new();
        public UnityEvent OnStaminaTired = new();
        public UnityEvent OnStaminaExhausted = new();
        public UnityEvent OnStaminaCritical = new();
        public UnityEvent OnFallAsleep = new();
        public UnityEvent OnWakeUp = new();
        
        private StaminaSystemConfig _config;
        private _Project.Code.Features.Character.MB.Character _character;
        private float _decayMultiplier = 1f;
        private float _lastZeroTime;
        private float _lastUpdateTime;
        
        public float CurrentStamina => currentStamina;
        public float MaxStamina => _config?.maxStamina ?? 100f;
        public bool IsAsleep => isAsleep;
        public bool CanSleep => currentStamina < (_config?.minStaminaToSleep ?? 50f) && !isAsleep;
        
        public bool TryInitialize(_Project.Code.Features.Character.MB.Character character, CharacterSystemConfig cfg)
        {
            _config = cfg as StaminaSystemConfig;
            if (_config == null) return false;
            
            _character = character;
            currentStamina = _config.maxStamina;
            _lastUpdateTime = Time.time;
            
            _character.TryRegisterSystem<IStaminaSystem>(this as IStaminaSystem);
            
            Debug.Log($"StaminaSystem initialized: MaxStamina={_config.maxStamina}");
            return true;
        }
        
        private void Update()
        {
            if (isAsleep) return;
            
            float currentTime = Time.time;
            float deltaTime = currentTime - _lastUpdateTime;
            _lastUpdateTime = currentTime;
            
            // Трата бодрости: 4 ед в час, deltaTime в секундах -> 4 / 3600 * deltaTime
            float decay = _config.decayPerHour * _decayMultiplier * (deltaTime / 3600f);
            currentStamina = Mathf.Max(0, currentStamina - decay);
            
            if (currentStamina <= 0)
            {
                HandleZeroStamina();
            }
            
            CheckThresholds();
            
            // Обновление множителя от движения
            _decayMultiplier = (_character != null && _character.IsMoving) ? 1.5f : 1f;
        }
        
        private void CheckThresholds()
        {
            if (currentStamina <= _config.tiredThreshold && currentStamina > 0)
            {
                OnStaminaTired?.Invoke();
            }
            else if (currentStamina <= _config.warningThreshold && currentStamina > _config.tiredThreshold)
            {
                OnStaminaWarning?.Invoke();
            }
        }
        
        private void HandleZeroStamina()
        {
            if (_lastZeroTime == 0)
            {
                _lastZeroTime = Time.time;
            }
            
            float secondsAtZero = Time.time - _lastZeroTime;
            float daysAtZero = secondsAtZero / 86400f; // 24*60*60
            
            if (daysAtZero >= 3f)
            {
                OnStaminaCritical?.Invoke();
            }
            else if (daysAtZero >= 1f)
            {
                OnStaminaExhausted?.Invoke();
            }
        }
        
        public void TrySleep(float quality = 1f)
        {
            if (!CanSleep) return;
            
            isAsleep = true;
            sleepQuality = Mathf.Clamp(quality, 0.7f, 1.2f);
            OnFallAsleep?.Invoke();
        }
        
        public void UpdateSleep(float realSecondsSlept)
        {
            if (!isAsleep) return;
            
            float staminaGain = _config.baseSleepRecovery * sleepQuality * (realSecondsSlept / 3600f);
            currentStamina = Mathf.Min(_config.maxStamina, currentStamina + staminaGain);
            OnStaminaChanged?.Invoke(currentStamina);
        }
        
        public void WakeUp()
        {
            if (!isAsleep) return;
            
            isAsleep = false;
            
            if (currentStamina > _config.tiredThreshold)
            {
                _lastZeroTime = 0;
            }
            
            _decayMultiplier = sleepQuality switch
            {
                < 0.8f => 1.2f,
                > 1.05f => 0.8f,
                _ => 1f
            };
            
            OnWakeUp?.Invoke();
        }
        
        public void ModifyStamina(float delta)
        {
            currentStamina = Mathf.Clamp(currentStamina + delta, 0, _config.maxStamina);
            OnStaminaChanged?.Invoke(currentStamina);
        }
        
        public void SetSleepQuality(float quality)
        {
            sleepQuality = Mathf.Clamp(quality, 0.7f, 1.2f);
        }
        
        private void OnDestroy()
        {
            // очистка
        }
    }
    
    public interface IStaminaSystem : ICharacterSystem
    {
        float CurrentStamina { get; }
        float MaxStamina { get; }
        bool IsAsleep { get; }
        bool CanSleep { get; }
        void TrySleep(float quality = 1f);
        void UpdateSleep(float realSecondsSlept);
        void WakeUp();
        void ModifyStamina(float delta);
        void SetSleepQuality(float quality);
    }
}