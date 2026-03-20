using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Code.Features.Time
{
    public class TimeManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float realSecondsPerGameMinute = 1f;
        [SerializeField] private WorldTime startTime = new WorldTime(1, 0, 1, 6, 0);
        [SerializeField] private bool autoStart = true;

        [Header("Current State")]
        [SerializeField] private WorldTime currentTime;
        [SerializeField] private bool isTimeRunning = true;

        [Header("Events")]
        public UnityEvent<WorldTime> OnTimeChanged;
        public UnityEvent<WorldTime> OnHourChanged;
        public UnityEvent<WorldTime> OnDayChanged;
        public UnityEvent<WorldTime> OnSeasonChanged;
        public UnityEvent<WorldTime> OnYearChanged;
        public UnityEvent<float> OnDaylightChanged;
        public UnityEvent OnSunrise;
        public UnityEvent OnSunset;

        private float timer = 0f;
        private int lastHour = -1;
        private int lastDay = -1;
        private int lastSeason = -1;
        private int lastYear = -1;
        private bool wasNight = false;

        public WorldTime CurrentTime => currentTime;
        public bool IsTimeRunning => isTimeRunning;
        public float RealSecondsPerGameMinute => realSecondsPerGameMinute;
        public int CurrentSeason => currentTime.Season;
        public int CurrentDayInSeason => currentTime.Day;
        public float YearProgress => (float)currentTime.ToTotalMinutes() / WorldTime.MinutesPerYear;

        private void Awake()
        {
            var existing = FindObjectsOfType<TimeManager>();
            if (existing.Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            
            if (autoStart)
            {
                currentTime = startTime;
                UpdateLastValues();
            }
        }

        private void Start()
        {
            OnDaylightChanged?.Invoke(CalculateDaylight());
        }

        private void Update()
        {
            if (!isTimeRunning) return;

            timer += UnityEngine.Time.deltaTime;
            if (timer >= realSecondsPerGameMinute)
            {
                timer = 0f;
                AddMinute();
            }
        }

        private void AddMinute()
        {
            currentTime = currentTime.AddMinutes(1);
            OnTimeChanged?.Invoke(currentTime);

            if (currentTime.Hour != lastHour)
            {
                lastHour = currentTime.Hour;
                OnHourChanged?.Invoke(currentTime);
                CheckSunEvents();
            }

            if (currentTime.Day != lastDay)
            {
                lastDay = currentTime.Day;
                OnDayChanged?.Invoke(currentTime);
            }

            if (currentTime.Season != lastSeason)
            {
                lastSeason = currentTime.Season;
                OnSeasonChanged?.Invoke(currentTime);
            }

            if (currentTime.Year != lastYear)
            {
                lastYear = currentTime.Year;
                OnYearChanged?.Invoke(currentTime);
            }

            float daylight = CalculateDaylight();
            OnDaylightChanged?.Invoke(daylight);
        }

        private void CheckSunEvents()
        {
            bool isNightNow = currentTime.Hour < 6 || currentTime.Hour >= 20;
            if (wasNight && !isNightNow)
            {
                OnSunrise?.Invoke();
            }
            else if (!wasNight && isNightNow)
            {
                OnSunset?.Invoke();
            }
            wasNight = isNightNow;
        }

        private float CalculateDaylight()
        {
            float hourAngle = Mathf.PI * 2 * (currentTime.Hour + currentTime.Minute / 60f) / 24f;
            float daylight = (Mathf.Sin(hourAngle - Mathf.PI / 2) + 1) / 2f;
            return Mathf.Clamp01(daylight);
        }

        private void UpdateLastValues()
        {
            lastHour = currentTime.Hour;
            lastDay = currentTime.Day;
            lastSeason = currentTime.Season;
            lastYear = currentTime.Year;
        }

        public void SetTime(WorldTime newTime)
        {
            currentTime = newTime;
            UpdateLastValues();
            OnTimeChanged?.Invoke(currentTime);
            OnDaylightChanged?.Invoke(CalculateDaylight());
        }

        public void SetTimePaused(bool paused)
        {
            isTimeRunning = !paused;
        }

        public void SetTimeScale(float realSecondsPerMinute)
        {
            realSecondsPerGameMinute = Mathf.Max(0.1f, realSecondsPerMinute);
        }

        public float GetCurrentHourFloat()
        {
            return currentTime.Hour + currentTime.Minute / 60f;
        }

        public bool IsNightTime()
        {
            return currentTime.Hour < 6 || currentTime.Hour >= 20;
        }

        public int GetMinutesUntilSunrise()
        {
            if (!IsNightTime()) return 0;
            
            int sunriseHour = 6;
            if (currentTime.Hour < sunriseHour)
            {
                return (sunriseHour - currentTime.Hour) * 60 - currentTime.Minute;
            }
            else
            {
                return (24 - currentTime.Hour + sunriseHour) * 60 - currentTime.Minute;
            }
        }

        public int GetMinutesUntilSunset()
        {
            if (IsNightTime()) return 0;
            
            int sunsetHour = 20;
            if (currentTime.Hour < sunsetHour)
            {
                return (sunsetHour - currentTime.Hour) * 60 - currentTime.Minute;
            }
            
            return 0;
        }
    }
}