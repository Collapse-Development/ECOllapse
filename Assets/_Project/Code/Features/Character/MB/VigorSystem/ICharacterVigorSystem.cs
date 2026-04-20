using System;

namespace _Project.Code.Features.Character.MB.Vigor
{
    public interface ICharacterVigorSystem : ICharacterSystem
    {
        float CurrentValue { get; }
        float MaxValue { get; }

        float Vigor { get; }

        event Action<float, float> OnValueChanged;
        event Action<float> OnMaxValueChanged;

        void AddValue(float value);
        void ReduceValue(float value);
        void SetCurrentValue(float value);
        void SetMaxValue(float value);
    }
}
