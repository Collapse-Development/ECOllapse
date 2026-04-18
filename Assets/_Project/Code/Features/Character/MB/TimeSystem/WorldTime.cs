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
            return Season * MinutesPerSeason + (Day - 1) * MinutesPerDay + Hour * 60 + Minute;
        }

        public static WorldTime FromTotalMinutes(int totalMinutes, int year)
        {
            int season = totalMinutes / MinutesPerSeason;
            int remaining = totalMinutes % MinutesPerSeason;
            int day = remaining / MinutesPerDay;
            remaining %= MinutesPerDay;
            int hour = remaining / 60;
            int minute = remaining % 60;
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
            return FromTotalMinutes(total, newYear);
        }

        public override string ToString() => $"Год {Year}, Сезон {Season + 1}, День {Day}, {Hour:00}:{Minute:00}";
        public bool Equals(WorldTime other) => Year == other.Year && Season == other.Season && Day == other.Day && Hour == other.Hour && Minute == other.Minute;
        public override bool Equals(object obj) => obj is WorldTime other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Year, Season, Day, Hour, Minute);
        public static bool operator ==(WorldTime left, WorldTime right) => left.Equals(right);
        public static bool operator !=(WorldTime left, WorldTime right) => !left.Equals(right);
    }
}