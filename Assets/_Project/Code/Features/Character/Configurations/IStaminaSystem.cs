using System;

namespace _Project.Code.Features.Character.MB.Systems
{    
    public interface IStaminaSystem : ICharacterSystem
        {
        float CurrentStamina { get; set; }
        float MaxStamina { get; }
        bool IsExhausted { get; }
        event Action<float> OnExhausted;
        event Action<float> OnRecovered;
        public void UpdateStamina();
        void DiminishStamina(float value);
        void AddStamina(float value);
    }

}