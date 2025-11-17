using UnityEngine;
using System.Collections;
using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.Character.MB.VigourSystem
{
    public interface IVigourSystem : ICharacterSystem
    {
        float CurrentVigour { get; }
        float MaxVigour { get; }
        VigourStatus Status { get; }
        
        void AddVigour(float amount);
        void SetVigourDecrementMultiplier(float multiplier);
        bool CanSleep { get; }
        float TimeAtZeroVigour { get; }
    }

    public enum VigourStatus
    {
        Normal,
        Warning,          // 35-40 пунктов
        Tired,            // 0-35 пунктов
        SleepDeprived,    // 0 пунктов > 1 день
        CriticalSleepDeprived // 0 пунктов > 3 дня
    }
}