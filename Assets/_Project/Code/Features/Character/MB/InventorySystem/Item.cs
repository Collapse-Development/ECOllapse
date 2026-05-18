namespace _Project.Code.Features.Character.MB.InventorySystem
{
    public class Item
    {
        public ItemConfig Config { get; }
        public string Id => Config.Id;

        public Item(ItemConfig config)
        {
            Config = config;
        }

        public virtual bool Use(ItemUseContext context)
        {
            return Config.Use(context);
        }
    }
}
