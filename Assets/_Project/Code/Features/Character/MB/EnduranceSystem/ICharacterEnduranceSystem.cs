using System;

namespace _Project.Code.Features.Character.MB.EnduranceSystem
{
    public interface ICharacterEnduranceSystem : ICharacterSystem
    {
        float CurrentValue { get; }
        float MaxValue { get; }

        event Action<float, float> OnCurrentValueChanged;
        event Action<float> OnMaxValueChanged;

        void AddValue(float value);
        void ReduceValue(float value);
        void SetCurrentValue(float value);
        void SetMaxValue(float value);
    }
}
