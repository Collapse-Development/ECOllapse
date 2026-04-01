using UnityEngine;
using UnityEngine.UI;

public class TemperatureUIManager : MonoBehaviour
{
    [Header("Player Temperature UI")]
    public Slider PlayerTemperatureSlider;
    public Text PlayerTemperatureText;

    [Header("Environment Temperature UI")]
    public Text EnvironmentTemperatureText;

    [Header("Temperature Colors")]
    public Color ColdColor = Color.cyan;
    public Color NormalColor = Color.green;
    public Color HotColor = Color.red;

    private void Start()
    {
        if (PlayerTemperatureSlider != null)
        {
            // Настраиваем визуальное отображение слайдера
            PlayerTemperatureSlider.minValue = 0;
            PlayerTemperatureSlider.maxValue = 1;
        }
    }

    public void UpdateTemperatureColor(float temperature, float minTemp, float maxTemp)
    {
        if (PlayerTemperatureSlider == null) return;

        var fillArea = PlayerTemperatureSlider.fillRect;
        if (fillArea == null) return;

        var image = fillArea.GetComponent<Image>();
        if (image == null) return;

        float normalizedTemp = (temperature - minTemp) / (maxTemp - minTemp);

        if (normalizedTemp < 0.3f)
            image.color = ColdColor;
        else if (normalizedTemp > 0.7f)
            image.color = HotColor;
        else
            image.color = NormalColor;
    }
}