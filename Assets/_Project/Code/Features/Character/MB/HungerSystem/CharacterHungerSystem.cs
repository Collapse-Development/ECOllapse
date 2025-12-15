using System;
using CharacterSystems;
using UnityEngine;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.Configurations.Systems;

namespace _Project.Code.Features.Character.MB.HungerSystem
{
    /// <summary>
    /// Реализация системы голода.
    /// Отвечает за:
    /// - пассивный расход сытости за игровой день
    /// - доп. расход при активности
    /// - стадии голода (недоедание / истощение / критическое голодание)
    /// - переедание
    /// - пищевое отравление
    /// </summary>
    public class CharacterHungerSystem : MonoBehaviour, ICharacterHungerSystem
    {
        [Header("Satiety")]
        [SerializeField] private float maxSatiety = 100f;
        [SerializeField] private float startSatiety = 100f;

        [Tooltip("Длительность одного игрового дня в секундах реального времени (для расчета расхода сытости).")]
        [SerializeField] private float gameDayDurationSeconds = 1200f;

        [Tooltip("Порог сытости, ниже которого начинается недоедание.")]
        [SerializeField] private float undernourishedThreshold = 20f;

        [Header("Status durations (seconds)")]
        [Tooltip("Длительность эффекта переедания (10 игровых минут ≈ половина дня).")]
        [SerializeField] private float overeatingDurationSeconds = 600f;

        [Tooltip("Минимальная длительность пищевого отравления.")]
        [SerializeField] private float minFoodPoisoningDurationSeconds = 60f;

        [Tooltip("Максимальная длительность пищевого отравления (1 игровой день).")]
        [SerializeField] private float maxFoodPoisoningDurationSeconds = 1200f;

        [Header("Health damage per second")]
        [SerializeField] private float criticalStarvationHealthLossPerSecond = 1f;
        [SerializeField] private float foodPoisoningHealthLossPerSecond = 0.5f;

        [Header("Activity satiety costs (per second)")]
        [SerializeField] private float runSatietyCostPerSecond = 2f;
        [SerializeField] private float harvestSatietyCostPerSecond = 1.5f;
        [SerializeField] private float attackSatietyCostPerSecond = 1f;
        [SerializeField] private float otherWorkSatietyCostPerSecond = 1f;

        [Header("Status multipliers")]
        [Tooltip("Дополнительный множитель расхода сытости при пищевом отравлении.")]
        [SerializeField] private float foodPoisoningSatietyMultiplier = 0.5f;

        [Header("Overeating chances")]
        [Range(0f, 1f)]
        [SerializeField] private float plantBaseOvereatingChance = 0.1f;

        [Range(0f, 1f)]
        [SerializeField] private float meatBaseOvereatingChance = 0.5f;

        [Tooltip("Порог сытости, выше которого начинается риск переедания.")]
        [SerializeField] private float overeatingSatietyThreshold = 90f;

        private Character _character;
        private ICharacterHealthSystem _healthSystem; // для урона от голода/пищ. отравления

        private float _currentSatiety;
        private HungerStage _currentStage = HungerStage.Normal;

        private float _timeAtZeroSatietySeconds;
        private float _overeatingTimeLeft;
        private float _foodPoisoningTimeLeft;
        private float _pendingActivitySatietyCost;

        public float CurrentSatiety => _currentSatiety;
        public float MaxSatiety => maxSatiety;
        public float NormalizedSatiety => maxSatiety > 0f ? _currentSatiety / maxSatiety : 0f;
        public HungerStage CurrentStage => _currentStage;
        public bool IsOvereatingActive => _overeatingTimeLeft > 0f;
        public bool IsFoodPoisoningActive => _foodPoisoningTimeLeft > 0f;
        public float TimeAtZeroSatietySeconds => _timeAtZeroSatietySeconds;

        public event Action<float, float> OnSatietyChanged;
        public event Action<HungerStage> OnStageChanged;

        /// <summary>
        /// Базовый расход сытости в секунду, чтобы за 1 игровой день
        /// нужно было "съесть" 100 сытости.
        /// </summary>
        private float BaseSatietyDecayPerSecond =>
            gameDayDurationSeconds > 0f ? maxSatiety / gameDayDurationSeconds : 0f;

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterHungerSystemConfig hungerCfg)
            {
                Debug.LogError("Invalid config type for CharacterHungerSystem");
                return false;
            }

            _character = character;

            if (!_character.TryRegisterSystem<ICharacterHungerSystem>(this))
            {
                Debug.LogError("Failed to register CharacterHungerSystem");
                return false;
            }

            // Применяем конфиг
            maxSatiety = Mathf.Max(0f, hungerCfg.MaxSatiety);
            startSatiety = Mathf.Clamp(hungerCfg.StartSatiety, 0f, maxSatiety);
            gameDayDurationSeconds = Mathf.Max(1f, hungerCfg.GameDayDurationSeconds);
            undernourishedThreshold = Mathf.Clamp(hungerCfg.UndernourishedThreshold, 0f, maxSatiety);

            overeatingDurationSeconds = Mathf.Max(0f, hungerCfg.OvereatingDurationSeconds);
            minFoodPoisoningDurationSeconds = Mathf.Max(0f, hungerCfg.MinFoodPoisoningDurationSeconds);
            maxFoodPoisoningDurationSeconds = Mathf.Max(
                minFoodPoisoningDurationSeconds,
                hungerCfg.MaxFoodPoisoningDurationSeconds
            );

            criticalStarvationHealthLossPerSecond = Mathf.Max(0f, hungerCfg.CriticalStarvationHealthLossPerSecond);
            foodPoisoningHealthLossPerSecond = Mathf.Max(0f, hungerCfg.FoodPoisoningHealthLossPerSecond);

            runSatietyCostPerSecond = Mathf.Max(0f, hungerCfg.RunSatietyCostPerSecond);
            harvestSatietyCostPerSecond = Mathf.Max(0f, hungerCfg.HarvestSatietyCostPerSecond);
            attackSatietyCostPerSecond = Mathf.Max(0f, hungerCfg.AttackSatietyCostPerSecond);
            otherWorkSatietyCostPerSecond = Mathf.Max(0f, hungerCfg.OtherWorkSatietyCostPerSecond);

            foodPoisoningSatietyMultiplier = hungerCfg.FoodPoisoningSatietyMultiplier;
            plantBaseOvereatingChance = Mathf.Clamp01(hungerCfg.PlantBaseOvereatingChance);
            meatBaseOvereatingChance = Mathf.Clamp01(hungerCfg.MeatBaseOvereatingChance);
            overeatingSatietyThreshold = Mathf.Clamp(hungerCfg.OvereatingSatietyThreshold, 0f, maxSatiety);

            _currentSatiety = startSatiety;
            OnSatietyChanged?.Invoke(_currentSatiety, maxSatiety);

            // Берём систему здоровья, чтобы наносить урон от голода и пищ. отравления
            _healthSystem = _character.GetSystem<ICharacterHealthSystem>();

            Debug.Log("HungerSystem initialized with config");
            return true;
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            UpdateSatiety(dt);
            UpdateStatuses(dt);
        }

        public void AddSatiety(float value)
        {
            ChangeSatiety(value);
        }

        public void ConsumeFood(FoodConsumptionInfo food)
        {
            float satietyBefore = _currentSatiety;

            float spoilageRatio = Mathf.Max(0f, food.SpoilageRatio);
            float nutritionMultiplier = 1f;

            if (spoilageRatio > 0f)
            {
                // Чем сильнее продукт испорчен, тем менее он питателен:
                // SpoilageRatio = 0   → 100% ценности
                // SpoilageRatio = 1   → 50% ценности
                // SpoilageRatio >= 2  → 0% ценности
                nutritionMultiplier = Mathf.Clamp01(1f - 0.5f * spoilageRatio);
            }

            float satietyGain = food.SatietyRestore * nutritionMultiplier;

            ChangeSatiety(satietyGain);

            TryApplyOvereating(food, satietyBefore, satietyGain);
            TryApplyFoodPoisoning(food, spoilageRatio);
        }

        public void ReportPhysicalActivity(HungerActivityType activityType, float durationSeconds)
        {
            if (durationSeconds <= 0f) return;

            float costPerSecond = 0f;
            switch (activityType)
            {
                case HungerActivityType.Run:
                    costPerSecond = runSatietyCostPerSecond;
                    break;
                case HungerActivityType.Harvest:
                    costPerSecond = harvestSatietyCostPerSecond;
                    break;
                case HungerActivityType.Attack:
                    costPerSecond = attackSatietyCostPerSecond;
                    break;
                case HungerActivityType.OtherHeavyWork:
                    costPerSecond = otherWorkSatietyCostPerSecond;
                    break;
            }

            _pendingActivitySatietyCost += Mathf.Max(0f, costPerSecond) * durationSeconds;
        }

        private void UpdateSatiety(float dt)
        {
            if (dt <= 0f) return;

            float decay = BaseSatietyDecayPerSecond * dt;

            float statusMultiplier = 1f;
            if (IsFoodPoisoningActive)
            {
                // Пищевое отравление увеличивает расход сытости.
                statusMultiplier += foodPoisoningSatietyMultiplier;
            }

            float totalDecay = decay * statusMultiplier + _pendingActivitySatietyCost;
            _pendingActivitySatietyCost = 0f;

            if (totalDecay > 0f)
            {
                ChangeSatiety(-totalDecay);
            }

            if (_currentSatiety <= 0.01f)
            {
                _timeAtZeroSatietySeconds += dt;
            }
            else
            {
                _timeAtZeroSatietySeconds = 0f;
            }

            UpdateStage();
        }

        private void UpdateStatuses(float dt)
        {
            if (dt <= 0f) return;

            // --- Переедание ---
            if (IsOvereatingActive)
            {
                _overeatingTimeLeft -= dt;
                if (_overeatingTimeLeft <= 0f)
                {
                    _overeatingTimeLeft = 0f;
                    // TODO: снять дебаффы переедания:
                    // - замедление передвижения (например, через MoveSpeedMultiplierEffect)
                    // - повышенный декремент бодрости
                    // - снижение максимальной выносливости
                    // - уменьшение урона
                }
            }

            // --- Пищевое отравление ---
            if (IsFoodPoisoningActive)
            {
                _foodPoisoningTimeLeft -= dt;

                // Потеря здоровья со временем от пищевого отравления:
                if (_healthSystem != null && foodPoisoningHealthLossPerSecond > 0f)
                {
                    _healthSystem.TakeDamage(foodPoisoningHealthLossPerSecond * dt);
                }

                // TODO: снижение восстановления выносливости,
                // увеличение декремента гидратации и сытости,
                // замедленное передвижение,

                if (_foodPoisoningTimeLeft <= 0f)
                {
                    _foodPoisoningTimeLeft = 0f;
                    // TODO: снять дебаффы пищевого отравления.
                }
            }

            // --- Критическое голодание ---
            if (_currentStage == HungerStage.CriticalStarvation)
            {
                // Постепенная потеря здоровья от критического голодания:
                if (_healthSystem != null && criticalStarvationHealthLossPerSecond > 0f)
                {
                    _healthSystem.TakeDamage(criticalStarvationHealthLossPerSecond * dt);
                }

                // TODO:
                // (постобработка / шейдер / тряска камеры и т.п.).
            }
        }

        private void ChangeSatiety(float delta)
        {
            float oldSatiety = _currentSatiety;
            _currentSatiety = Mathf.Clamp(oldSatiety + delta, 0f, maxSatiety);

            if (!Mathf.Approximately(oldSatiety, _currentSatiety))
            {
                OnSatietyChanged?.Invoke(_currentSatiety, maxSatiety);
            }
        }

        private void UpdateStage()
        {
            HungerStage newStage;

            if (_currentSatiety <= 0.01f)
            {
                if (_timeAtZeroSatietySeconds >= gameDayDurationSeconds * 3f)
                    newStage = HungerStage.CriticalStarvation;
                else if (_timeAtZeroSatietySeconds >= gameDayDurationSeconds)
                    newStage = HungerStage.Exhaustion;
                else
                    newStage = HungerStage.Undernourished;
            }
            else if (_currentSatiety <= undernourishedThreshold)
            {
                newStage = HungerStage.Undernourished;
            }
            else
            {
                newStage = HungerStage.Normal;
            }

            if (newStage == _currentStage) return;

            _currentStage = newStage;
            OnStageChanged?.Invoke(_currentStage);

            // TODO:
            // - Normal: убрать все дебаффы голода.
            // - Undernourished:
            //      уменьшить реген стамины/здоровья (RegenerationEffect),
            //      увеличить расход стамины,
            //      уменьшить урон,
            //      ухудшить терморегуляцию,
            //      ухудшить качество сна.
            // - Exhaustion:
            //      всё выше + уменьшить максимальную стамину,
            //      замедлить скорость передвижения и анимаций
            //      (MoveSpeedMultiplierEffect).
            // - CriticalStarvation:
            //      всё выше + периодический урон и галлюцинации.
        }

        private void TryApplyOvereating(FoodConsumptionInfo food, float satietyBefore, float satietyGain)
        {
            if (IsOvereatingActive)
                return;

            float satietyAfter = Mathf.Clamp(satietyBefore + satietyGain, 0f, maxSatiety);

            bool wasHighBefore = satietyBefore >= overeatingSatietyThreshold;
            bool overflowed = satietyAfter >= maxSatiety;

            if (!wasHighBefore && !overflowed)
                return;

            float baseChance = 0f;
            switch (food.FoodType)
            {
                case FoodType.Plant:
                    baseChance = plantBaseOvereatingChance;
                    break;
                case FoodType.Meat:
                    baseChance = meatBaseOvereatingChance;
                    break;
            }

            // Дополнительно усиливаем шанс, если сильно перехватили.
            float overflowAmount = Mathf.Max(0f, satietyAfter - overeatingSatietyThreshold);
            // При +10 пунктов сверх порога — +100% к базовому шансу (ограничено 1).
            float overflowFactor = Mathf.Clamp01(overflowAmount / 10f);

            float chance = Mathf.Clamp01(baseChance + overflowFactor);

            if (UnityEngine.Random.value <= chance)
            {
                _overeatingTimeLeft = overeatingDurationSeconds;

                // TODO: включить дебаффы переедания (через систему эффектов).
            }
        }

        private void TryApplyFoodPoisoning(FoodConsumptionInfo food, float spoilageRatio)
        {
            float chance = 0f;

            if (food.IsAlwaysPoisoned)
            {
                chance = 1f;
            }
            else if (spoilageRatio > 0f && food.BaseFoodPoisoningChance > 0f)
            {
                // Чем больше просрочен продукт, тем выше шанс:
                // SpoilageRatio 0..1 → 0..100% базового шанса,
                // >1 → чуть выше базового (обрезаем до 1).
                float spoilageFactor = Mathf.Clamp(spoilageRatio, 0f, 2f);
                chance = Mathf.Clamp01(food.BaseFoodPoisoningChance * spoilageFactor);
            }

            if (chance <= 0f)
                return;

            if (UnityEngine.Random.value <= chance)
            {
                float t = Mathf.Clamp01(spoilageRatio);
                float duration = Mathf.Lerp(
                    minFoodPoisoningDurationSeconds,
                    maxFoodPoisoningDurationSeconds,
                    t
                );
                _foodPoisoningTimeLeft = duration;

                // TODO: эффекты пищевого отравления
            }
        }
    }
}
