using _Project.Code.Features.Character.MB.Thirst;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.InventorySystem.Items
{
    [CreateAssetMenu(
        fileName = "New WaterBottle",
        menuName = "Scriptable Objects/Character/Inventory/Items/Water Bottle")]
    public class WaterBottleItemConfig : ItemConfig
    {
        [SerializeField, Min(0f)] private float _hydrationRestore = 35f;

        public float HydrationRestore => _hydrationRestore;

        public override bool Use(ItemUseContext context)
        {
            if (context?.User == null) return false;

            var thirstSystem = context.User.GetSystem<ICharacterThirstSystem>();
            if (thirstSystem == null) return false;

            thirstSystem.AddValue(_hydrationRestore);
            return true;
        }
    }
}
