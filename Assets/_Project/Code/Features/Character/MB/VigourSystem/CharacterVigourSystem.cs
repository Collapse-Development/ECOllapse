using UnityEngine;
using System.Collections;
using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.Character.MB.VigourSystem
{
    public class CharacterVigourSystem : MonoBehaviour, IVigourSystem
    {
        [Header("Vigour Settings")]
        [SerializeField] private float maxVigour = 100f;
        [SerializeField] private float baseDecrementRate = 4f; // 100/25 = 4 в час
        
        [Header("Current State")]
        [SerializeField] private float currentVigour;
        [SerializeField] private VigourStatus status;
        [SerializeField] private float timeAtZeroVigour = 0f;
        
        private Character _character;
        private float _decrementMultiplier = 1f;
        private bool _isInitialized = false;
        
        // ICharacterSystem implementation
        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            var vigourCfg = cfg as CharacterVigourSystemConfig;
            if (vigourCfg == null) return false;
            
            _character = character;
            if (!_character.TryRegisterSystem<IVigourSystem>(this)) return false;
            
            maxVigour = vigourCfg.MaxVigour;
            baseDecrementRate = vigourCfg.BaseDecrementRate;
            currentVigour = maxVigour;
            
            StartCoroutine(VigourDecrementCoroutine());
            _isInitialized = true;
            
            Debug.Log($"VigourSystem initialized: MaxVigour={maxVigour}");
            return true;
        }
        
        public void Initialize(Character character)
        {
            _character = character;
            _character.TryRegisterSystem<IVigourSystem>(this);
            currentVigour = maxVigour;
            StartCoroutine(VigourDecrementCoroutine());
            _isInitialized = true;
        }
        
        // IVigourSystem implementation
        public float CurrentVigour => currentVigour;
        public float MaxVigour => maxVigour;
        public VigourStatus Status => status;
        public bool CanSleep => currentVigour < 50f;
        public float TimeAtZeroVigour => timeAtZeroVigour;
        
        public void AddVigour(float amount)
        {
            currentVigour = Mathf.Clamp(currentVigour + amount, 0f, maxVigour);
            UpdateVigourStatus();
        }
        
        public void SetVigourDecrementMultiplier(float multiplier)
        {
            _decrementMultiplier = Mathf.Max(0f, multiplier);
        }
        
        private IEnumerator VigourDecrementCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                
                if (_isInitialized)
                {
                    // 4 единицы в час = 4/3600 в секунду
                    float decrement = (baseDecrementRate / 3600f) * _decrementMultiplier * Time.deltaTime;
                    currentVigour = Mathf.Max(0f, currentVigour - decrement);
                    
                    if (currentVigour <= 0f)
                    {
                        timeAtZeroVigour += Time.deltaTime;
                    }
                    else
                    {
                        timeAtZeroVigour = 0f;
                    }
                    
                    UpdateVigourStatus();
                    ApplyStatusEffects();
                }
            }
        }
        
        private void UpdateVigourStatus()
        {
            float daysAtZero = timeAtZeroVigour / (24f * 3600f); // Перевод в игровые дни
            
            if (daysAtZero > 3f)
            {
                status = VigourStatus.CriticalSleepDeprived;
            }
            else if (daysAtZero > 1f)
            {
                status = VigourStatus.SleepDeprived;
            }
            else if (currentVigour <= 35f)
            {
                status = VigourStatus.Tired;
            }
            else if (currentVigour <= 40f)
            {
                status = VigourStatus.Warning;
            }
            else
            {
                status = VigourStatus.Normal;
            }
        }
        
        private void ApplyStatusEffects()
        {
            // Интеграция с другими системами для применения дебаффов
            switch (status)
            {
                case VigourStatus.Warning:
                    // UI подсказка о необходимости сна
                    ApplyWarningEffects();
                    break;
                case VigourStatus.Tired:
                    // Уменьшение регенерации выносливости, скорости анимаций и передвижения
                    ApplyTiredEffects();
                    break;
                case VigourStatus.SleepDeprived:
                    // Галлюцинации и пониженная терморегуляция, уменьшенная максимальная стойкость
                    ApplySleepDeprivedEffects();
                    break;
                case VigourStatus.CriticalSleepDeprived:
                    // Периодическая потеря контроля, случайные потери сознания
                    ApplyCriticalSleepDeprivedEffects();
                    break;
            }
        }
        
        private void ApplyWarningEffects()
        {
            // Только UI подсказка - без игровых дебаффов
        }
        
        private void ApplyTiredEffects()
        {
            // Уменьшение регенерации выносливости
            var staminaSystem = _character?.GetSystem<IStaminaSystem>();
            staminaSystem?.SetStaminaDecrementMultiplier(1.2f);
            
            // Уменьшение скорости анимаций и передвижения
            var movementSystem = _character?.GetSystem<ICharacterMovementSystem>();
            // movementSystem?.SetSpeedMultiplier(0.8f);
        }
        
        private void ApplySleepDeprivedEffects()
        {
            // Все эффекты усталости +
            // Галлюцинации и пониженная терморегуляция
            var tempSystem = _character?.GetSystem<ITemperatureSystem>();
            // tempSystem?.SetResistanceMultiplier(0.7f);
            
            // Уменьшенная максимальная стойкость
            var firmnessSystem = _character?.GetSystem<IFirmnessSystem>();
            // firmnessSystem?.SetMaxFirmnessMultiplier(0.8f);
        }
        
        private void ApplyCriticalSleepDeprivedEffects()
        {
            // Все эффекты недосыпа +
            // Увеличение коэффициента траты выносливости
            var staminaSystem = _character?.GetSystem<IStaminaSystem>();
            staminaSystem?.SetStaminaDecrementMultiplier(1.5f);
            
            // Сильное увеличение декремента сытости, жажды
            var thirstSystem = _character?.GetSystem<IThirstSystem>();
            thirstSystem?.SetHydrationDecrementMultiplier(1.3f);
            
            // Периодическая потеря контроля (реализовать через корутины)
            StartCoroutine(RandomBlackoutCoroutine());
        }
        
        private IEnumerator RandomBlackoutCoroutine()
        {
            // Случайные потери сознания при критическом недосыпе
            if (status == VigourStatus.CriticalSleepDeprived)
            {
                yield return new WaitForSeconds(Random.Range(300f, 600f)); // 5-10 минут
                TriggerAutoSleep();
            }
        }
        
        private void TriggerAutoSleep()
        {
            Debug.Log("Auto-sleep triggered due to critical sleep deprivation");
            // Здесь будет вызов системы сна для принудительного засыпания
        }
        
        // Метод для сезонных изменений (зимой больше сна)
        public void ApplySeasonalMultiplier(bool isWinter)
        {
            _decrementMultiplier = isWinter ? 1.2f : 1f; // Зимой бодрость тратится быстрее
        }
        
        // Метод для восстановления бодрости во время сна
        public void Sleep(float sleepQuality, float durationInHours)
        {
            // Восстановление бодрости: 8 бодрости за игровой час * качество сна
            float vigourGain = 8f * sleepQuality * durationInHours;
            AddVigour(vigourGain);
            
            Debug.Log($"Slept for {durationInHours} hours. Vigour gained: {vigourGain}");
        }
    }
}