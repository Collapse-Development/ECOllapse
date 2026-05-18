using CharacterSystems;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.InventorySystem.Items
{
    [CreateAssetMenu(
        fileName = "New PoisonousBerries",
        menuName = "Scriptable Objects/Character/Inventory/Items/Poisonous Berries")]
    public class PoisonousBerriesItemConfig : ItemConfig
    {
        [SerializeField, Min(0f)] private float _damage = 10f;

        public float Damage => _damage;

        public override bool Use(ItemUseContext context)
        {
            if (context?.User == null) return false;

            var healthSystem = context.User.GetSystem<ICharacterHealthSystem>();
            if (healthSystem == null) return false;

            healthSystem.TakeDamage(_damage);
            return true;
        }
    }
}
