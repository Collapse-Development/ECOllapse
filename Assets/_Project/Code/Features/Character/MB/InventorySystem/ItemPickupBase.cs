using UnityEngine;
using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.Character.MB.InventorySystem
{
    /// <summary>
    /// MonoBehaviour на GameObject предмета в мире.
    /// Хранит ItemConfig, создаёт Item через CreateItem() и кладёт в инвентарь персонажа.
    /// </summary>
    public class ItemPickupBase : MonoBehaviour
    {
        [SerializeField] private ItemConfig _itemConfig;
        [SerializeField] private int _count = 1;

        /// <summary>
        /// Попытка подобрать предмет персонажем.
        /// Проверяет дистанцию, создаёт Item и добавляет в инвентарь.
        /// </summary>
        public InventoryAddResult TryPickup(Character character)
        {
            if (character == null || _count <= 0) return InventoryAddResult.Rejected(_count);
            if (_itemConfig == null)
            {
                Debug.LogWarning($"{name}: ItemConfig is not assigned.");
                return InventoryAddResult.Rejected(_count);
            }

            var inventory = character.GetSystem<ICharacterInventorySystem>();
            if (inventory == null) return InventoryAddResult.Rejected(_count);

            float distance = Vector3.Distance(character.transform.position, transform.position);
            if (distance > inventory.PickupRange)
            {
                Debug.LogWarning($"Предмет слишком далеко ({distance:F1} > {inventory.PickupRange})");
                return InventoryAddResult.Rejected(_count);
            }

            var item = _itemConfig.CreateItem();
            var result = inventory.TryAddItem(item, _count);

            if (result.AddedCount > 0)
            {
                _count -= result.AddedCount;
                OnPickedUp(character, result);
            }

            if (_count <= 0)
                gameObject.SetActive(false);

            return result;
        }

        /// <summary>Вызывается после частичного или полного подбора.</summary>
        protected virtual void OnPickedUp(Character character, InventoryAddResult result) { }
    }
}
