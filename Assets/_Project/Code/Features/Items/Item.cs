namespace _Project.Code.Features.Items
{
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
