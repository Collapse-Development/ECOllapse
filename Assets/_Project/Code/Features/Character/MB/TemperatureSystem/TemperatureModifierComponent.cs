using Unity.Entities;

[GenerateAuthoringComponent]
public struct TemperatureModifierComponent : IComponentData
{
    public float TemperatureDelta; // изменение температуры
    public bool IsPermanent; // постоянный модификатор или одноразовый
    public string Source; // источник модификатора (например, "огонь", "вода")
}