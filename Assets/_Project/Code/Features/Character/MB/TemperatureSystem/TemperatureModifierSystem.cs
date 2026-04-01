using Unity.Entities;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public partial struct TemperatureModifierSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (tempComp, modifierComp, entity) in
                 SystemAPI.Query<RefRW<TemperatureComponent>,
                                RefRO<TemperatureModifierComponent>>()
                 .WithEntityAccess())
        {
            ApplyTemperatureModifier(ref tempComp.ValueRW, modifierComp.ValueRO);

            // Удаляем одноразовые модификаторы
            if (!modifierComp.ValueRO.IsPermanent)
            {
                ecb.RemoveComponent<TemperatureModifierComponent>(entity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private void ApplyTemperatureModifier(ref TemperatureComponent temp,
                                         TemperatureModifierComponent modifier)
    {
        temp.CurrentTemperature = math.clamp(temp.CurrentTemperature + modifier.TemperatureDelta,
                                            temp.MinTemperature,
                                            temp.MaxTemperature);
    }
}