using UnityEngine;

namespace _Project.Code.Features.Character.Sleep
{
    public class SleepQualityCalculator
    {
        public float CalculateQuality(bool hasNoise, bool hasLight, bool hasFood, bool hasWater, 
            bool hasSoftSurface, float temperature, float timeDeviation, int season)
        {
            float quality = 1f;
            if (hasNoise) quality -= 0.1f;
            if (hasLight) quality -= 0.1f;
            if (!hasFood) quality -= 0.1f;
            if (!hasWater) quality -= 0.1f;
            if (!hasSoftSurface) quality -= 0.05f;
            if (temperature < 18f || temperature > 28f) quality -= 0.1f;
            else if (temperature < 22f || temperature > 24f) quality -= 0.05f;
            if (timeDeviation > 1f) quality -= Mathf.Min(0.3f, timeDeviation * 0.05f);
            if (season == 3) quality += 0.1f;
            return Mathf.Clamp(quality, 0.7f, 1.2f);
        }
    }
}