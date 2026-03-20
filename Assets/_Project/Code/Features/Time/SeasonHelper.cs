namespace _Project.Code.Features.Time
{
    public static class SeasonHelper
    {
        public static string GetSeasonName(int seasonIndex)
        {
            return seasonIndex switch
            {
                0 => "Spring",
                1 => "Summer",
                2 => "Autumn",
                3 => "Winter",
                _ => "Unknown"
            };
        }

        public static float GetSeasonDifficulty(int seasonIndex)
        {
            return 1f + seasonIndex * 0.5f;
        }

        public static int GetSleepBonusMinutes(int seasonIndex)
        {
            return seasonIndex == 3 ? 120 : 0;
        }
    }
}