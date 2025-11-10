using UnityEngine;
using System.Collections;
using CharacterSystems;

namespace _Project.Code.Features.Character.MB.HungerSystem
{
    public interface IHungerSystem : ICharacterSystem
    {
        float CurrentHunger { get; }
        float MaxHunger { get; }
        HungerStatus Status { get; }
        
        void AddHunger(float amount);
        void ConsumeFood(float nutritionValue);
        void SetHungerDecrementMultiplier(float multiplier);
    }

    public enum HungerStatus
    {
        Full,           // 80-100%
        Normal,         // 50-80%
        Hungry,         // 20-50%
        VeryHungry,     // 5-20%
        Starving        // 0-5%
    }
}