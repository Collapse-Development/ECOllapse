using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct TemperatureSystem : ISystem
{
    private float lastUpdateTime;
    private const float UPDATE_INTERVAL = 0.1f; // обновляем каждые 0.1 секунды для плавности

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float currentTime = (float)SystemAPI.Time.ElapsedTime;
        float deltaTime = SystemAPI.Time.DeltaTime;

        if (currentTime - lastUpdateTime >= UPDATE_INTERVAL)
        {
            lastUpdateTime = currentTime;

            // Получаем все сущности с температурой
            foreach (var (tempComp, envComp, healthComp) in
                     SystemAPI.Query<RefRW<TemperatureComponent>,
                                    RefRO<EnvironmentalTemperatureComponent>,
                                    RefRW<HealthComponent>>())
            {
                UpdatePlayerTemperature(ref tempComp.ValueRW, envComp.ValueRO, deltaTime);
                CheckTemperatureDeath(ref tempComp.ValueRW, ref healthComp.ValueRW);
            }
        }
    }

    private void UpdatePlayerTemperature(ref TemperatureComponent temp,
                                        EnvironmentalTemperatureComponent env,
                                        float deltaTime)
    {
        float temperatureChange = 0f;

        switch (env.ZoneType)
        {
            case EnvironmentalZoneType.Cold:
                temperatureChange = -temp.ChangeRate * env.InfluenceStrength;
                break;
            case EnvironmentalZoneType.Hot:
                temperatureChange = temp.ChangeRate * env.InfluenceStrength;
                break;
            case EnvironmentalZoneType.Neutral:
                // Возвращаем к нормальной температуре (36.6)
                float targetTemp = 36.6f;
                if (math.abs(temp.CurrentTemperature - targetTemp) > 0.1f)
                {
                    temperatureChange = math.sign(targetTemp - temp.CurrentTemperature) *
                                      temp.ChangeRate * 0.5f * deltaTime;
                }
                break;
        }

        temp.CurrentTemperature = math.clamp(temp.CurrentTemperature + temperatureChange,
                                            temp.MinTemperature,
                                            temp.MaxTemperature);
    }

    private void CheckTemperatureDeath(ref TemperatureComponent temp, ref HealthComponent health)
    {
        if (!temp.IsDead && (temp.CurrentTemperature <= temp.MinTemperature ||
            temp.CurrentTemperature >= temp.MaxTemperature))
        {
            temp.IsDead = true;
            health.CurrentHealth = 0; // убиваем персонажа
        }
    }
}