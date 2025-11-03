using UnityEngine;
using _Project.Code.Features.Character.MB.EffectsSystem;

namespace _Project.Code.Features.Character.MB.EffectsSystem.Examples
{
    /// <summary>
    /// Example script showing how to use the poison system
    /// </summary>
    public class PoisonExample : MonoBehaviour
    {
        [Header("Poison Settings")]
        [SerializeField] private float damagePerTick = 5f;
        [SerializeField] private float tickInterval = 1f;
        [SerializeField] private float duration = 10f;

        [Header("Target")]
        [SerializeField] private Character targetCharacter;

        private void Start()
        {
            // Find character if not assigned
            if (targetCharacter == null)
            {
                targetCharacter = FindFirstObjectByType<Character>();
            }
        }

        [ContextMenu("Apply Poison")]
        public void ApplyPoisonToTarget()
        {
            if (targetCharacter != null)
            {
                PoisonUtility.ApplyPoison(targetCharacter, damagePerTick, tickInterval, duration);
                Debug.Log($"Applied poison to {targetCharacter.name}: {damagePerTick} damage every {tickInterval}s for {duration}s");
            }
            else
            {
                Debug.LogWarning("No target character assigned!");
            }
        }

        [ContextMenu("Apply Quick Poison (1 damage/second for 5 seconds)")]
        public void ApplyQuickPoison()
        {
            if (targetCharacter != null)
            {
                PoisonUtility.ApplyPoison(targetCharacter, 1f, 5f);
                Debug.Log($"Applied quick poison to {targetCharacter.name}");
            }
        }
    }
}