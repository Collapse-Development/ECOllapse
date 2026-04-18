using UnityEngine;

namespace _Project.Code.Features.Time
{
    [CreateAssetMenu(fileName = "TimeZone", menuName = "ECOllapse/TimeZone")]
    public class TimeZone : ScriptableObject
    {
        [Range(-5, 5)] public int Offset = 0;
        public float Latitude = 0f;

        public WorldTime GetLocalTime(WorldTime worldTime)
        {
            int totalMinutes = worldTime.ToTotalMinutes() + Offset * 60;
            return WorldTime.FromTotalMinutes(totalMinutes, worldTime.Year);
        }

        public float GetDaylightIntensity(WorldTime worldTime)
        {
            WorldTime local = GetLocalTime(worldTime);
            float hourAngle = Mathf.PI * 2 * (local.Hour + local.Minute / 60f) / 24f;
            return Mathf.Clamp01((Mathf.Sin(hourAngle - Mathf.PI / 2) + 1) / 2f);
        }

        public int GetNightDurationMinutes() => 8 * 60;
    }
}