using UnityEngine;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.Configurations.Systems;
// using _Project.Code.Features.Items;

namespace _Project.Code.Features.Character.MB.Pickup
{
    public class CharacterPickupSystem : MonoBehaviour, ICharacterPickupSystem
    {
        private float _pickupRadius;
        private LayerMask _pickupLayer;

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterPickupSystemConfig pickupCfg) return false;
            
            // Регистрируем систему в персонаже
            if (!character.TryRegisterSystem<ICharacterPickupSystem>(this)) return false;

            _pickupRadius = pickupCfg.PickupRadius;
            _pickupLayer = pickupCfg.PickupLayer;
            
            return true;
        }

        public void TryPickup()
        {
            // Создаем невидимую сферу вокруг персонажа и ищем все коллайдеры на нужном слое
            Collider[] hits = Physics.OverlapSphere(transform.position, _pickupRadius, _pickupLayer);
            
            Collider closestCollider = null;
            float minDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                // Игнорируем самого себя, если вдруг слой игрока совпадает с PickupLayer
                if (hit.gameObject == gameObject) continue;

                // Ищем самый ближайший объект
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestCollider = hit;
                }
            }

            // Если нашли хоть один объект
            if (closestCollider != null)
            {
                Debug.Log($"[PickupSystem] Система подбора сработала! Ближайший объект: {closestCollider.gameObject.name}");
                
                // === ЗАДЕЛ ПОД ПРЕДЫДУЩУЮ ЗАДАЧУ (пока закомментирован) ===
                // if (closestCollider.TryGetComponent<Item>(out var item))
                // {
                //     item.Interact(gameObject);
                // }
                // ==========================================================
                
                // ВРЕМЕННАЯ ЗАГЛУШКА: просто удаляем объект, чтобы было видно, что он "подобран"
                Destroy(closestCollider.gameObject);
            }
            else
            {
                Debug.Log("Рядом нет предметов для подбора.");
            }
        }

        // Рисуем желтую сферу в редакторе Unity для удобной настройки радиуса
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _pickupRadius);
        }
    }
}