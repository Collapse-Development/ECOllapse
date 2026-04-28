using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.Character.MB.InventorySystem
{
    /// <summary>
    /// Абстрактный базовый класс предмета. Заглушка под систему предметов.
    /// </summary>
    public abstract class Item
    {
        public string Id { get; }

        protected Item(string id)
        {
            Id = id;
        }

        public abstract void Use(ItemUseContext context);
    }
}
