namespace _Project.Code.Features.Time
{
    public static class SeasonHelper
    {
        public static string GetSeasonName(int seasonIndex) => seasonIndex switch
        {
            0 => "Spring",
            1 => "Summer",
            2 => "Autumn",
            3 => "Winter",
            _ => "Unknown"
        };

        public static float GetSeasonDifficulty(int seasonIndex) => 1f + seasonIndex * 0.5f;
        public static int GetSleepBonusMinutes(int seasonIndex) => seasonIndex == 3 ? 120 : 0;
    }
}