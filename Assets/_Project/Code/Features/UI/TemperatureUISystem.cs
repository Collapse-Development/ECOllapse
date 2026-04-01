using Unity.Entities;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;

public partial class TemperatureUISystem : SystemBase
{
    private Slider playerTemperatureSlider;
    private Text playerTemperatureText;
    private Text environmentTemperatureText;

    protected override void OnCreate()
    {
        // Находим UI элементы в сцене
        var uiManager = Object.FindObjectOfType<TemperatureUIManager>();
        if (uiManager != null)
        {
            playerTemperatureSlider = uiManager.PlayerTemperatureSlider;
            playerTemperatureText = uiManager.PlayerTemperatureText;
            environmentTemperatureText = uiManager.EnvironmentTemperatureText;
        }
    }

    protected override void OnUpdate()
    {
        foreach (var (tempComp, envComp) in
                 SystemAPI.Query<RefRO<TemperatureComponent>,
                                RefRO<EnvironmentalTemperatureComponent>>())
        {
            UpdateUITemperature(tempComp.ValueRO, envComp.ValueRO);
        }
    }

    private void UpdateUITemperature(TemperatureComponent temp,
                                    EnvironmentalTemperatureComponent env)
    {
        if (playerTemperatureSlider != null)
        {
            // Нормализуем значение для слайдера (28-41 -> 0-1)
            float normalizedTemp = (temp.CurrentTemperature - temp.MinTemperature) /
                                  (temp.MaxTemperature - temp.MinTemperature);
            playerTemperatureSlider.value = normalizedTemp;
        }

        if (playerTemperatureText != null)
        {
            playerTemperatureText.text = $"Температура: {temp.CurrentTemperature:F1}°C";
        }

        if (environmentTemperatureText != null)
        {
            string zoneText = GetZoneText(env.ZoneType);
            environmentTemperatureText.text = $"Окружение: {zoneText} ({GetTemperatureDescription(env.ZoneType)})";
        }
    }

    private string GetZoneText(EnvironmentalZoneType zone)
    {
        return zone switch
        {
            EnvironmentalZoneType.Cold => "❄️ Холодно",
            EnvironmentalZoneType.Neutral => "🌡️ Нейтрально",
            EnvironmentalZoneType.Hot => "🔥 Жарко",
            _ => "❓ Неизвестно"
        };
    }

    private string GetTemperatureDescription(EnvironmentalZoneType zone)
    {
        return zone switch
        {
            EnvironmentalZoneType.Cold => "-10°C",
            EnvironmentalZoneType.Neutral => "+20°C",
            EnvironmentalZoneType.Hot => "+35°C",
            _ => "0°C"
        };
    }
}