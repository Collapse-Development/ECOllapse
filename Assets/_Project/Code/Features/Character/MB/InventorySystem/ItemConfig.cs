using UnityEngine;

namespace _Project.Code.Features.Character.MB.InventorySystem
{
    /// <summary>
    /// Базовый ScriptableObject конфига предмета. Заглушка под систему предметов.
    /// </summary>
    public abstract class ItemConfig : ScriptableObject
    {
        [SerializeField] private string _id;
        public string Id => _id;

        public abstract Item CreateItem();
    }
}
