using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared
{
    /// <summary>
    /// Manages damage resistance values with support for multiple sources.
    /// </summary>
    [Serializable]
    public class DamageResistance
    {
        [SerializeField] private float baseResistance;
        private Dictionary<string, float> resistanceModifiers = new Dictionary<string, float>();

        public float TotalResistance => CalculateTotalResistance();

        public DamageResistance(float baseResistance = 0f)
        {
            this.baseResistance = Mathf.Clamp(baseResistance, 0f, 1f);
        }

        /// <summary>
        /// Adds a resistance modifier with a unique key.
        /// </summary>
        /// <param name="key">Unique identifier for this resistance source</param>
        /// <param name="value">Resistance value (0-1 range recommended)</param>
        public void AddResistance(string key, float value)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("Cannot add resistance with null or empty key");
                return;
            }

            resistanceModifiers[key] = value;
        }

        /// <summary>
        /// Removes a resistance modifier by key.
        /// </summary>
        /// <param name="key">Unique identifier of the resistance to remove</param>
        /// <returns>True if the resistance was removed, false if not found</returns>
        public bool RemoveResistance(string key)
        {
            return resistanceModifiers.Remove(key);
        }

        /// <summary>
        /// Calculates damage after applying resistance.
        /// </summary>
        /// <param name="incomingDamage">Raw damage value</param>
        /// <returns>Damage after resistance is applied</returns>
        public float CalculateDamage(float incomingDamage)
        {
            float resistance = Mathf.Clamp01(TotalResistance);
            return incomingDamage * (1f - resistance);
        }

        /// <summary>
        /// Clears all resistance modifiers.
        /// </summary>
        public void ClearAllModifiers()
        {
            resistanceModifiers.Clear();
        }

        /// <summary>
        /// Gets the resistance value for a specific key.
        /// </summary>
        public bool TryGetResistance(string key, out float value)
        {
            return resistanceModifiers.TryGetValue(key, out value);
        }

        private float CalculateTotalResistance()
        {
            float total = baseResistance;
            foreach (var modifier in resistanceModifiers.Values)
            {
                total += modifier;
            }
            return Mathf.Clamp(total, 0f, 1f);
        }
    }
}
