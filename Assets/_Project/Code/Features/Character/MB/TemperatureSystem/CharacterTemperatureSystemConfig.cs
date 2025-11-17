using UnityEngine;
using _Project.Code.Features.Character.MB.TemperatureSystem;

[CreateAssetMenu(fileName = "New TemperatureSystemConfig", menuName = "Scriptable Objects/Character/Systems/Temperature/TemperatureSystem")]
public class CharacterTemperatureSystemConfig : CharacterSystemConfig<CharacterTemperatureSystem>
{
    [Header("Temperature Settings")]
    public float BasePlayerTemperature = 36.6f;
    public float ComfortTemperature = 22f;
    public float TemperatureChangeRate = 0.1f;
    
    [Header("Death Thresholds")]
    public float MinSurvivableTemperature = 28f;
    public float MaxSurvivableTemperature = 41f;
    
    [Header("Resistance Settings")]
    public float MaxColdResistance = 50f;
    public float MaxHeatResistance = 50f;
}