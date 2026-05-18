using _Project.Code.Features.Character.MB.EffectsSystem.Effects.Poison;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.InventorySystem.Items
{
    [CreateAssetMenu(
        fileName = "New Poison",
        menuName = "Scriptable Objects/Character/Inventory/Items/Poison")]
    public class PoisonItemConfig : ItemConfig
    {
        [SerializeField, Min(0f)] private float _damagePerTick = 1f;
        [SerializeField, Min(0.01f)] private float _tickInterval = 1f;
        [SerializeField, Min(0.01f)] private float _duration = 5f;

        public float DamagePerTick => _damagePerTick;
        public float TickInterval => _tickInterval;
        public float Duration => _duration;

        public override bool Use(ItemUseContext context)
        {
            if (context?.User == null) return false;

            PoisonUtility.ApplyPoison(context.User, _damagePerTick, _tickInterval, _duration);
            return true;
        }
    }
}
