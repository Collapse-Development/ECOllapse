using UnityEngine;
using System.Collections;
using CharacterSystems;

namespace _Project.Code.Features.Character.MB.HungerSystem
{
    public class CharacterHungerSystem : MonoBehaviour, IHungerSystem
    {
        [Header("Current State")]
        [SerializeField] private float currentHunger;  // Текущий уровень сытости (0-100)
        [SerializeField] private HungerStatus status;  // Текущий статус голода
        
        private Character _character;                   // Ссылка на главный класс персонажа
        private float _decrementMultiplier = 1f;       // Множитель скорости убывания голода
        private float _timeAtZeroHunger = 0f;          // Время проведенное с нулевой сытостью
        private bool _isInitialized = false;           // Флаг инициализации системы

        
        // Параметры из конфига
        private float _maxHunger;                      // Максимальный уровень сытости
        private float _baseDecrementRate;              // Базовая скорость убывания голода (единиц в час)
        private float _starvationDamage;               // Урон в секунду при критическом голодании
        
        // ICharacterSystem implementation
        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            // Пытаемся получить конфиг системы голода
            var hungerCfg = cfg as CharacterHungerSystemConfig;
            if (hungerCfg == null) return false;
            
            _character = character;
            // Регистрируем систему в главном классе персонажа
            if (!_character.TryRegisterSystem<IHungerSystem>(this)) return false;
            
            // Берем параметры из конфига
            _maxHunger = hungerCfg.MaxHunger;
            _baseDecrementRate = hungerCfg.BaseDecrementRate;
            _starvationDamage = hungerCfg.StarvationDamage;
            
            currentHunger = _maxHunger;  // Начинаем с полной сытости
            
            // Запускаем корутину убывания голода
            StartCoroutine(HungerDecrementCoroutine());
            _isInitialized = true;
            
            Debug.Log($"HungerSystem initialized: MaxHunger={_maxHunger}");
            return true;
        }
        
        // IHungerSystem implementation
        public float CurrentHunger => currentHunger;    // Текущая сытость
        public float MaxHunger => _maxHunger;          // Максимальная сытость
        public HungerStatus Status => status;          // Текущий статус
        
        public void AddHunger(float amount)
        {
            // Добавляем сытость с ограничением от 0 до максимума
            currentHunger = Mathf.Clamp(currentHunger + amount, 0f, _maxHunger);
            UpdateHungerStatus();  // Обновляем статус после изменения
        }
        
        public void ConsumeFood(float nutritionValue, FoodType foodType)
        {
            if (!_isInitialized) return;
            
            // Проверка переедания: если сытость >90 и мясная пища
            if (currentHunger > 90f && foodType == FoodType.Meat)
            {
                ApplyOvereatingEffects();
            }
            
            AddHunger(nutritionValue);
            Debug.Log($"Consumed {foodType} food. Nutrition: {nutritionValue}, Current hunger: {currentHunger}");
        }
        
        public void SetHungerDecrementMultiplier(float multiplier)
        {
            // Устанавливаем множитель скорости убывания голода
            _decrementMultiplier = Mathf.Max(0f, multiplier);
        }
        
        private IEnumerator HungerDecrementCoroutine()
        {
            // Основная корутина - уменьшает голод каждую секунду
            while (true)
            {
                yield return new WaitForSeconds(1f);
                
                if (_isInitialized)
                {
                    // Расчет убывания: (базовая_скорость / 3600) * множитель * время
                    // Делим на 3600 чтобы перевести из "в час" в "в секунду"
                    float decrement = (_baseDecrementRate / 3600f) * _decrementMultiplier * Time.deltaTime;
                    currentHunger = Mathf.Max(0f, currentHunger - decrement);
                    
                    // Отслеживаем время с нулевой сытостью
                    if (currentHunger <= 0f)
                    {
                        _timeAtZeroHunger += Time.deltaTime;
                    }
                    else
                    {
                        _timeAtZeroHunger = 0f;  // Сбрасываем если сытость > 0
                    }
                    
                    UpdateHungerStatus();    // Обновляем статус голода
                    ApplyStatusEffects();    // Применяем эффекты статуса
                }
            }
        }
        
        private void UpdateHungerStatus()
        {
            // Переводим время в игровые дни (24 игровых часа = 24 * 3600 секунд)
            float daysAtZero = _timeAtZeroHunger / (24f * 3600f);
            
            // Определяем статус по времени с нулевой сытостью и текущему уровню
            if (daysAtZero > 3f)
            {
                status = HungerStatus.CriticalStarvation;    // Критическое голодание (>3 дней)
            }
            else if (daysAtZero > 1f)
            {
                status = HungerStatus.Exhaustion;            // Истощение (>1 дня)
            }
            else if (currentHunger <= 20f)
            {
                status = HungerStatus.Malnutrition;          // Недоедание (0-20 единиц)
            }
            else
            {
                status = HungerStatus.Normal;                // Норма (20-100 единиц)
            }
        }
        
        private void ApplyStatusEffects()
        {
            // Применяем эффекты в зависимости от статуса голода
            switch (status)
            {
                case HungerStatus.Malnutrition:
                    // Дебаффы недоедания (нужно реализовать интеграцию с другими системами)
                    // - Уменьшение восстановления выносливости
                    // - Уменьшение восстановления здоровья  
                    // - Увеличение расхода выносливости
                    // - Уменьшение урона
                    // - Ухудшение терморегуляции
                    break;
                case HungerStatus.Exhaustion:
                    // Дебаффы истощения (нужно реализовать)
                    // - Все дебаффы недоедания +
                    // - Уменьшение максимальной выносливости
                    // - Замедление скорости передвижения
                    // - Замедление анимаций
                    break;
                case HungerStatus.CriticalStarvation:
                    // Критическое голодание - наносим урон здоровью
                    ApplyStarvationDamage();
                    break;
            }
        }
        
        private void ApplyStarvationDamage()
        {
            // Получаем систему здоровья и наносим урон от голодания
            var healthSystem = _character?.GetSystem<ICharacterHealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(_starvationDamage * Time.deltaTime);
            }
        }
        
        private void ApplyOvereatingEffects()
        {
            // Эффекты переедания (нужно реализовать дебаффы)
            Debug.Log("Overeating effects applied");
            StartCoroutine(ResetOvereating());
        }
        
        private IEnumerator ResetOvereating()
        {
            // Сбрасываем эффекты переедания через 10 минут (600 секунд)
            yield return new WaitForSeconds(600f);
            // Здесь нужно снять дебаффы переедания
        }
        
        public void ApplyActivityMultiplier(bool isActive)
        {
            // Увеличиваем расход голода при физической активности
            _decrementMultiplier = isActive ? 1.5f : 1f;
        }
    }
}