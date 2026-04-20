using System;

namespace _Project.Code.Features.Character.MB.Thirst
{
    public interface ICharacterThirstSystem : ICharacterSystem
    {
        float CurrentValue { get; }
        float MaxValue { get; }

        float Hydration { get; }

        event Action<float, float> OnCurrentValueChanged;
        event Action<float> OnMaxValueChanged;

        void AddValue(float value);
        void ReduceValue(float value);
        void SetCurrentValue(float value);
        void SetMaxValue(float value);
    }
}
