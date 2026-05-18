using CharacterSystems;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.InventorySystem.Items
{
    [CreateAssetMenu(
        fileName = "New HealingItem",
        menuName = "Scriptable Objects/Character/Inventory/Items/Healing Item")]
    public class HealingItemConfig : ItemConfig
    {
        [SerializeField, Min(0f)] private float _healAmount = 25f;

        public float HealAmount => _healAmount;

        public override bool Use(ItemUseContext context)
        {
            if (context?.User == null) return false;

            var healthSystem = context.User.GetSystem<ICharacterHealthSystem>();
            if (healthSystem == null) return false;

            healthSystem.AddHealth(_healAmount);
            return true;
        }
    }
}
