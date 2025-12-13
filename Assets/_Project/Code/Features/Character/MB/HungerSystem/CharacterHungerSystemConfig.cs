using UnityEngine;
using _Project.Code.Features.Character.MB.HungerSystem;

[CreateAssetMenu(
    fileName = "New CharacterHungerSystemConfig",
    menuName = "Scriptable Objects/Character/Systems/Hunger/DefaultHungerSystem"
)]
public class CharacterHungerSystemConfig : CharacterSystemConfig<CharacterHungerSystem>
{
    [Header("Satiety")]
    [Min(0f)]
    public float MaxSatiety = 100f;

    [Min(0f)]
    public float StartSatiety = 100f;

    [Tooltip("Длительность одного игрового дня в секундах реального времени.")]
    [Min(1f)]
    public float GameDayDurationSeconds = 1200f;

    [Tooltip("Порог сытости, ниже которого начинается недоедание.")]
    [Min(0f)]
    public float UndernourishedThreshold = 20f;

    [Header("Status durations (seconds)")]
    [Tooltip("Длительность эффекта переедания (10 игровых минут ≈ половина дня).")]
    [Min(0f)]
    public float OvereatingDurationSeconds = 600f;

    [Tooltip("Минимальная длительность пищевого отравления.")]
    [Min(0f)]
    public float MinFoodPoisoningDurationSeconds = 60f;

    [Tooltip("Максимальная длительность пищевого отравления (1 игровой день).")]
    [Min(0f)]
    public float MaxFoodPoisoningDurationSeconds = 1200f;

    [Header("Health damage per second")]
    [Tooltip("Потеря здоровья в секунду при критическом голодании.")]
    [Min(0f)]
    public float CriticalStarvationHealthLossPerSecond = 1f;

    [Tooltip("Потеря здоровья в секунду при пищевом отравлении.")]
    [Min(0f)]
    public float FoodPoisoningHealthLossPerSecond = 0.5f;

    [Header("Activity satiety costs (per second)")]
    [Min(0f)]
    public float RunSatietyCostPerSecond = 2f;

    [Min(0f)]
    public float HarvestSatietyCostPerSecond = 1.5f;

    [Min(0f)]
    public float AttackSatietyCostPerSecond = 1f;

    [Min(0f)]
    public float OtherWorkSatietyCostPerSecond = 1f;

    [Header("Status multipliers")]
    [Tooltip("Дополнительный множитель расхода сытости при пищевом отравлении.")]
    public float FoodPoisoningSatietyMultiplier = 0.5f;

    [Header("Overeating chances")]
    [Range(0f, 1f)]
    public float PlantBaseOvereatingChance = 0.1f;

    [Range(0f, 1f)]
    public float MeatBaseOvereatingChance = 0.5f;

    [Tooltip("Порог сытости, выше которого начинается риск переедания.")]
    public float OvereatingSatietyThreshold = 90f;
}
