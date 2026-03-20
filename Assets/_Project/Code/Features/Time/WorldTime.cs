using System;

namespace _Project.Code.Features.Time
{
    [Serializable]
    public struct WorldTime : IEquatable<WorldTime>
    {
        public int Year;   
        public int Season;   
        public int Day;    
        public int Hour;      
        public int Minute;     

        public const int MinutesPerDay = 24 * 60;

        public const int MinutesPerSeason = MinutesPerDay * 8;

        public const int MinutesPerYear = MinutesPerSeason * 4;

        public WorldTime(int year, int season, int day, int hour, int minute)
        {
            Year = year;
            Season = season;
            Day = day;
            Hour = hour;
            Minute = minute;
        }

        public int ToTotalMinutes()
        {
            int seasonMinutes = Season * MinutesPerSeason;
            int dayMinutes = (Day - 1) * MinutesPerDay;
            int currentMinutes = Hour * 60 + Minute;
            return seasonMinutes + dayMinutes + currentMinutes;
        }

        public static WorldTime FromTotalMinutes(int totalMinutes, int year)
        {
            int season = totalMinutes / MinutesPerSeason;
            int remainingAfterSeason = totalMinutes % MinutesPerSeason;

            int day = remainingAfterSeason / MinutesPerDay;
            int remainingAfterDay = remainingAfterSeason % MinutesPerDay;

            int hour = remainingAfterDay / 60;
            int minute = remainingAfterDay % 60;

            return new WorldTime(year, season, day + 1, hour, minute);
        }

        public WorldTime AddMinutes(int minutes)
        {
            int total = ToTotalMinutes() + minutes;
            int newYear = Year;
            while (total >= MinutesPerYear)
            {
                total -= MinutesPerYear;
                newYear++;
            }
            var newTime = FromTotalMinutes(total, newYear);
            return newTime;
        }

        public override string ToString()
        {
            string seasonName = Season switch
            {
                0 => "Весна",
                1 => "Лето",
                2 => "Осень",
                3 => "Зима",
                _ => "?"
            };
            return $"Год {Year}, {seasonName}, День {Day}, {Hour:00}:{Minute:00}";
        }

        public bool Equals(WorldTime other)
        {
            return Year == other.Year && Season == other.Season && Day == other.Day && Hour == other.Hour && Minute == other.Minute;
        }

        public override bool Equals(object obj)
        {
            return obj is WorldTime other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Year, Season, Day, Hour, Minute);
        }

        public static bool operator ==(WorldTime left, WorldTime right) => left.Equals(right);
        public static bool operator !=(WorldTime left, WorldTime right) => !left.Equals(right);
    }
}