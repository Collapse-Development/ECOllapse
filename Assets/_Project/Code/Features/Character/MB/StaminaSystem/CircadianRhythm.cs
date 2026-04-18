using System;
using UnityEngine;

namespace _Project.Code.Features.Character.Sleep
{
    [Serializable]
    public class CircadianRhythm
    {
        [SerializeField] private int usualSleepHour = 22;
        [SerializeField] private int usualWakeHour = 6;
        private float _rhythmStability = 0.5f;
        private float _lastSleepTime;
        private float _lastWakeTime;
        
        public void UpdateRhythm()
        {
            float currentHour = DateTime.Now.Hour + DateTime.Now.Minute / 60f;
            int sleepDiff = Mathf.Abs((int)currentHour - usualSleepHour);
            int wakeDiff = Mathf.Abs((int)currentHour - usualWakeHour);
            
            if (sleepDiff <= 1 || wakeDiff <= 1)
                _rhythmStability = Mathf.Min(1f, _rhythmStability + 0.1f);
            else
                _rhythmStability = Mathf.Max(0f, _rhythmStability - 0.05f);
        }
        
        public float GetTimeDeviation()
        {
            float currentHour = DateTime.Now.Hour + DateTime.Now.Minute / 60f;
            float target = usualSleepHour;
            float deviation = Mathf.Abs(currentHour - target);
            deviation = Mathf.Min(deviation, 24 - deviation);
            return deviation * (1f - _rhythmStability);
        }
    }
}