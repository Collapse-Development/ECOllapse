using System;
using System.Collections.Generic;
using _Project.Code.Features.Character.Configurations.Systems;
using JetBrains.Annotations;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public class CharacterEffectsSystem : MonoBehaviour, ICharacterEffectsSystem
    {
        private Dictionary<Type, ICharacterEffect> _effects;
        private Character _character;
        
        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterEffectsSystemConfig systemCfg) return false;
            
            _character = character;
            if (!_character.TryRegisterSystem<ICharacterEffectsSystem>(this)) return false;
            
            _effects = new Dictionary<Type, ICharacterEffect>();
            foreach (var effect in systemCfg.Effects)
            {
                AddEffect(CharacterEffectsHelper.CreateEffect(effect));
            }
            
            Debug.Log($"EffectsSystem initialized");
            return true;
        }
        
        private void Update()
        {
            if (_effects == null) return;
            
            float dt = Time.deltaTime;
            
            foreach (var effect in _effects.Values)
            {
                if (effect != null)
                {
                    effect.Tick(dt);
                }
            }
        }
        
        [CanBeNull]
        public T GetEffect<T>() where T : class, ICharacterEffect
        {
            if (_effects.TryGetValue(typeof(T), out var effect))
            {
                return effect as T;
            }
            return null;
        }
        
        private void AddEffect<T>(T effect) where T : class, ICharacterEffect
        {
            _effects[typeof(T)] = effect;
        }
    }
}
