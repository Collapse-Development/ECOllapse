using System;
using UnityEngine;
using _Project.Code.Features.Time;

namespace _Project.Code.Features.Character.Sleep
{
    [Serializable]
    public class CircadianRhythm
    {
        [SerializeField] private int usualSleepHour = 22;
        [SerializeField] private int usualWakeHour = 6;
        
        private float _rhythmStability = 0.5f;
        
        public void UpdateRhythm(WorldTime sleepTime, WorldTime wakeTime)
        {
            int sleepDiff = Mathf.Abs(sleepTime.Hour - usualSleepHour);
            int wakeDiff = Mathf.Abs(wakeTime.Hour - usualWakeHour);
            
            if (sleepDiff <= 1 && wakeDiff <= 1)
                _rhythmStability = Mathf.Min(1f, _rhythmStability + 0.1f);
            else
                _rhythmStability = Mathf.Max(0f, _rhythmStability - 0.05f);
        }
        
        public float GetTimeDeviation(WorldTime currentTime)
        {
            float target = usualSleepHour;
            float current = currentTime.Hour + currentTime.Minute / 60f;
            float deviation = Mathf.Abs(current - target);
            deviation = Mathf.Min(deviation, 24 - deviation);
            return deviation * (1f - _rhythmStability);
        }
    }
}