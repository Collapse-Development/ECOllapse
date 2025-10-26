using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    /// <summary>
    /// Central manager for all effects on a character.
    /// Registers with the Character system and coordinates effect updates each frame.
    /// </summary>
    public class EffectsSystem : MonoBehaviour, ICharacterSystem
    {
        /// <summary>
        /// Singleton instance for convenient global access.
        /// </summary>
        public static EffectsSystem Instance { get; private set; }
        
        private Dictionary<Type, IEffect> _effects;
        private Character _character;
        
        private void Awake()
        {
            Instance = this;
            _effects = new Dictionary<Type, IEffect>();
            
            // Locate the Character component and register this system
            _character = GetComponentInParent<Character>();
            if (_character != null)
            {
                _character.TryRegisterSystem(this);
            }
            else
            {
                Debug.LogWarning("EffectsSystem: Character component not found in parent hierarchy.");
            }
        }
        
        private void Update()
        {
            if (_effects == null) return;
            
            float dt = Time.deltaTime;
            
            // Update all registered effects
            foreach (var effect in _effects.Values)
            {
                if (effect != null)
                {
                    effect.Tick(dt);
                }
            }
        }
        
        /// <summary>
        /// Retrieves a registered effect by type.
        /// </summary>
        /// <typeparam name="T">The effect type to retrieve</typeparam>
        /// <returns>The effect instance, or null if not registered</returns>
        public T GetEffect<T>() where T : class, IEffect
        {
            if (_effects.TryGetValue(typeof(T), out var effect))
            {
                return effect as T;
            }
            return null;
        }
        
        /// <summary>
        /// Registers an effect instance for later retrieval and automatic updates.
        /// </summary>
        /// <typeparam name="T">The effect type to register</typeparam>
        /// <param name="effect">The effect instance to register</param>
        public void Register<T>(T effect) where T : class, IEffect
        {
            _effects[typeof(T)] = effect;
        }
    }
}
