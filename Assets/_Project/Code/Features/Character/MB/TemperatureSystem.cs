using UnityEngine;

public class TemperatureSystem : MonoBehaviour
{
    [Header("Body")]
    public float bodyTemperature = 36.6f;
    public float minBodyTemperature = 28f;
    public float maxBodyTemperature = 41f;

    [Header("Environment")]
    public float environmentTemperature = 22f;
    [Range(0, 100)]
    public float humidity = 0f;

    [Header("Comfort")]
    public float comfortTemperature = 22f;

    [Header("Resistance")]
    [Range(0, 50)]
    public float coldResist = 0f;

    [Range(0, 50)]
    public float heatResist = 0f;

    [Header("Config")]
    public float k = 0.0015f;
    public float maxDeltaPerSecond = 0.25f;

    private void Update()
    {
        UpdateTemperature(Time.deltaTime);

        if (bodyTemperature <= minBodyTemperature)
        {
            Die("Hypothermia");
        }

        if (bodyTemperature >= maxBodyTemperature)
        {
            Die("Hyperthermia");
        }
    }

    void UpdateTemperature(float dt)
    {
        float feelTemperature = CalculateFeelTemperature();

        float resist =
            feelTemperature < comfortTemperature
            ? coldResist
            : heatResist;

        float S = CalculateS(resist);

        float delta =
            k *
            Mathf.Sign(feelTemperature - comfortTemperature) *
            (Mathf.Exp(Mathf.Abs(feelTemperature - comfortTemperature) / S) - 1f);

        delta *= dt;

        delta = Mathf.Clamp(
            delta,
            -maxDeltaPerSecond * dt,
            maxDeltaPerSecond * dt
        );

        bodyTemperature += delta;
    }

    float CalculateFeelTemperature()
    {
        if (environmentTemperature > comfortTemperature)
        {
            return environmentTemperature + humidity * 12f / 100f;
        }

        return environmentTemperature - humidity * 8f / 100f;
    }

    float CalculateS(float r)
    {
        return
            0.00375f * r * r +
            0.0510714286f * r +
            8.03571429f;
    }

    void Die(string reason)
    {
        Debug.Log("Player died from: " + reason);
    }
}
