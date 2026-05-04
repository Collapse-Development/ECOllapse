namespace _Project.Code.Features.Character.MB.InventorySystem
{
    /// <summary>
    /// Результат попытки добавить предметы в инвентарь.
    /// </summary>
    public class InventoryAddResult
    {
        /// <summary>Сколько единиц удалось добавить.</summary>
        public int AddedCount { get; }

        /// <summary>Сколько единиц не вместилось (превышение веса или стака).</summary>
        public int RejectedCount { get; }

        public bool AllAdded => RejectedCount == 0;
        public bool NoneAdded => AddedCount == 0;

        public InventoryAddResult(int added, int rejected)
        {
            AddedCount = added;
            RejectedCount = rejected;
        }

        public static InventoryAddResult Rejected(int count) => new(0, count);
        public static InventoryAddResult Full(int count) => new(count, 0);
    }
}
