using UnityEngine;
using System.Collections;
using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.Character.MB.HungerSystem
{
    public interface IHungerSystem : ICharacterSystem
    {
        float CurrentHunger { get; }
        float MaxHunger { get; }
        HungerStatus Status { get; }
        
        void AddHunger(float amount);
        void ConsumeFood(float nutritionValue, FoodType foodType);
        void SetHungerDecrementMultiplier(float multiplier);
    }

    public enum HungerStatus
    {
        Normal,           
        Malnutrition,   
        Exhaustion,       
        CriticalStarvation 
    }

    public enum FoodType
    {
        Plant,
        Meat
    }
}