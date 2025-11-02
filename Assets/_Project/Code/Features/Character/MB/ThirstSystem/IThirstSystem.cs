using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.Character.MB.ThirstSystem
{
    public interface IThirstSystem : ICharacterSystem
    {
        float CurrentHydration { get; }
        float MaxHydration { get; }
        ThirstStatus Status { get; }
        
        void Drink(float hydrationAmount, WaterQuality quality);
        void AddHydration(float amount);
        void SetHydrationDecrementMultiplier(float multiplier);
    }

    public enum ThirstStatus
    {
        Normal,
        MildDehydration,      // 20-0 пунктов
        Dehydration,          // 0 пунктов > 12 часов
        CriticalDehydration,  // 0 пунктов > 24 часов
        Overhydration         // >90 пунктов + частое питье
    }

    public enum WaterQuality
    {
        Clean,
        Contaminated,
        Toxic
    }
}