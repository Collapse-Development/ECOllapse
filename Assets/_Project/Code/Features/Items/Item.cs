using UnityEngine;
using _Project.Code.Features.World;

namespace _Project.Code.Features.Items
{
    // Items наследуется от WorldObject
    public class Item : WorldObject
    {
        [Header("Параметры из ГДД")]
        public float weight = 1f;       // Вес предмета
        public int maxStackSize = 10;   // Стак
        public int durability = 100;    // Прочность

        // Переопределяем взаимодействие. Для предмета это — подбор.
        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor); // Оставит лог из WorldObject
            PickUp();
        }

        private void PickUp()
        {
            // Тут логика: если в инвентаре есть место, добавляем
            Debug.Log($"Предмет {objectName} (Вес: {weight}) поднят с земли!");
            
            // Удаляем физическую модель из мира (из чанка)
            Destroy(gameObject);
        }
    }
}