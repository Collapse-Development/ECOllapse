using System;
using _Project.Code.Features.Character.MB;

namespace CharacterSystems
{
    // Стадии голода.
    public enum HungerStage
    {
        Normal,             // > 20
        Undernourished,     // 0–20
        Exhaustion,         // 0 1 игровой день
        CriticalStarvation  // 0 3 игровых дня
    }

    // Типы пищи (если нужны)
    public enum FoodType
    {
        Plant, 
        Meat
    }

    // Тип физической активности, повышающей расход сытости.
    public enum HungerActivityType
    {
        Run,
        Harvest,
        Attack,
        OtherHeavyWork
    }

    // Данные о съеденной пище
    public struct FoodConsumptionInfo
    {
        // Базовое восстановление сытости (до учёта порчи), в "пунктах" (0–100).
        public float SatietyRestore;

        // Тип пищи: растительная/мясная.
        public FoodType FoodType;

        // Степень порчи:
        // 0   — свежая,
        // 1   — на грани срока годности,
        // > 1 — сильно просрочена.
        public float SpoilageRatio;

        // Базовый шанс пищевого отравления при SpoilageRatio == 1 (0–1).
        public float BaseFoodPoisoningChance;

        // Всегда даёт отравление (например, ядовитый гриб).
        public bool IsAlwaysPoisoned;
    }

    // Система голода
    public interface ICharacterHungerSystem : ICharacterSystem
    {
        float CurrentSatiety { get; }
        float MaxSatiety { get; }
        // Текущая сытость в диапазоне 0..1.
        float NormalizedSatiety { get; }
        HungerStage CurrentStage { get; }

        bool IsOvereatingActive { get; }
        bool IsFoodPoisoningActive { get; }

        // Сколько времени персонаж непрерывно находится на 0 сытости (в секундах игрового времени).
        float TimeAtZeroSatietySeconds { get; }

        // Событие изменения сытости: (текущее, максимум).
        event Action<float, float> OnSatietyChanged;

        // Событие изменения стадии голода.
        event Action<HungerStage> OnStageChanged;

        // Напрямую изменить сытость (положительное — добавить, отрицательное — убрать).
        void AddSatiety(float value);

        // Вызвать при поедании еды.
        void ConsumeFood(FoodConsumptionInfo food);

        // Сообщить системе о физической активности, которая тратит сытость.
        // durationSeconds — сколько длилась активность.
        void ReportPhysicalActivity(HungerActivityType activityType, float durationSeconds);
    }
}
