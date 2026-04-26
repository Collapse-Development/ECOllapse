using UnityEngine;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.MB.Pickup;

namespace _Project.Code.Features.Character.Configurations.Systems
{
    [CreateAssetMenu(fileName = "New PickupSystemConfig", menuName = "Scriptable Objects/Character/Systems/Pickup/PickupSystem")]
    public class CharacterPickupSystemConfig : CharacterSystemConfig<CharacterPickupSystem>
    {
        [Header("Настройки подбора")]
        [Tooltip("Радиус вокруг персонажа, в котором он может подбирать предметы")]
        public float PickupRadius = 2f;
        
        [Tooltip("Слой, на котором лежат предметы (для оптимизации поиска)")]
        public LayerMask PickupLayer = ~0; // ~0 значит "Все слои"
    }
}