using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TemperatureComponent : IComponentData
{
    public float CurrentTemperature; // от 28 до 41
    public float MinTemperature;
    public float MaxTemperature;
    public float ChangeRate; // скорость изменени€ температуры в секунду
    public bool IsDead;
}