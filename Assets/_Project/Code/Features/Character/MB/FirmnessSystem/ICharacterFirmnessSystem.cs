using System;
using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.Character.MB.FirmnessSystem
{
    public interface ICharacterFirmnessSystem : ICharacterSystem
    {
        float CurrentValue { get; }
        float MaxValue { get; }
        float MinValue { get; }

        event Action<float, float> OnCurrentValueChanged;
        event Action<float> OnMaxValueChanged;
        
        // Событие оглушения: передает длительность стана
        event Action<float> OnStunned; 

        void ReduceFirmness(float amount, bool isJumping);
        void SetMaxValue(float value);
    }
}