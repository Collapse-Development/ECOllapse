using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.Character.MB.InventorySystem
{
    /// <summary>
    /// Контекст использования предмета — передаётся в Item.Use().
    /// </summary>
    public class ItemUseContext
    {
        public Character User { get; }

        public ItemUseContext(Character user)
        {
            User = user;
        }
    }
}
