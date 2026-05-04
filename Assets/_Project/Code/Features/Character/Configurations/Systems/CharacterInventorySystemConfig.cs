using _Project.Code.Features.Character.MB.InventorySystem;
using UnityEngine;

[CreateAssetMenu(
    fileName = "New CharacterInventorySystemConfig",
    menuName = "Scriptable Objects/Character/Systems/Inventory/DefaultInventorySystem")]
public class CharacterInventorySystemConfig : CharacterSystemConfig<CharacterInventorySystem>
{
    [Tooltip("Максимальный суммарный вес инвентаря")]
    public float MaxWeight = 50f;

    [Tooltip("Максимальное расстояние подбора предмета")]
    public float PickupRange = 2f;

    [Tooltip("Конфиг инвентаря с параметрами предметов (вес, стак)")]
    public InventoryConfig InventoryConfig;
}
