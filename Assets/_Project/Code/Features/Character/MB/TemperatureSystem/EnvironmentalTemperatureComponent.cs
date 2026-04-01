using Unity.Entities;

[GenerateAuthoringComponent]
public struct EnvironmentalTemperatureComponent : IComponentData
{
    public EnvironmentalZoneType ZoneType; // Cold, Neutral, Hot
    public float BaseTemperature; // базовая температура окружения
    public float InfluenceStrength; // сила влияния на игрока (0-1)
}

public enum EnvironmentalZoneType
{
    Cold,
    Neutral,
    Hot
}