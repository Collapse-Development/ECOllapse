namespace _Project.Code.Features.NPC
{
    public enum NPCType
    {
        Peaceful,   // Мирный — никогда не нападает
        Neutral,    // Нейтральный — нападает только если тронуть
        Hostile     // Враждебный — нападает первым
    }
}