using System;
using System.Collections.Generic;
using _Project.Code.Features.Character.Configurations.Systems;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public class CharacterEffectsSystem : MonoBehaviour, ICharacterEffectsSystem
    {
        public event Action<ICharacterEffect> OnEffectAdded;

        private List<ICharacterEffect> _effects;
        private Character _character;
        
        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterEffectsSystemConfig systemCfg) return false;
            
            _character = character;
            if (!_character.TryRegisterSystem<ICharacterEffectsSystem>(this)) return false;
            
            _effects = new List<ICharacterEffect>();
            
            Debug.Log($"EffectsSystem initialized");
            return true;
        }
        
        private void Update()
        {
            if (_effects == null) return;
            
            float dt = Time.deltaTime;
            
            foreach (var effect in _effects)
            {
                if (effect != null)
                {
                    effect.Tick(dt);
                }
            }
        }
        
        public List<T> GetEffectsOfType<T>() where T : class, ICharacterEffect
        {
            var typeEffects = new List<T>();
            foreach (var effect in _effects)
            {
                if (effect is T characterEffect)
                {
                    typeEffects.Add(characterEffect);
                }
            }
            
            return typeEffects;
        }

        public void AddEffect(ICharacterEffect effect)
        {
            effect.Initialize(_character);
            _effects.Add(effect);
            effect.OnEffectEnded += RemoveEffect;
            OnEffectAdded?.Invoke(effect);
        }

        private void RemoveEffect(ICharacterEffect effect)
        { 
            _effects.Remove(effect);
        }
    }
}
