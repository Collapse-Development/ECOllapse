using UnityEngine;
using System.Collections;
using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.Character.MB.TemperatureSystem
{
    public interface ITemperatureSystem : ICharacterSystem
    {
        float PlayerTemperature { get; }
        float EnvironmentTemperature { get; }
        TemperatureStatus Status { get; }
        float ColdResistance { get; }
        float HeatResistance { get; }
        
        void SetEnvironmentTemperature(float temperature, float humidity);
        void SetResistance(float coldResist, float heatResist);
    }

    public enum TemperatureStatus
    {
        FreezingDeath,      // <28°C
        SevereHypothermia,  // 28-30°C
        ModerateHypothermia,// 30-33°C
        MildHypothermia,    // 33-35°C
        Normal,             // 35-37.5°C
        MildHyperthermia,   // 37.5-38.5°C
        ModerateHyperthermia,// 38.5-39.5°C
        SevereHyperthermia, // 39.5-41°C
        HeatStrokeDeath     // >41°C
    }
}