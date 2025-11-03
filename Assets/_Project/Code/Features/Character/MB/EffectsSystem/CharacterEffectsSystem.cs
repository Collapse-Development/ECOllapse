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
        private List<ICharacterEffect> _cancelledEffects;
        private Character _character;
        
        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterEffectsSystemConfig systemCfg) return false;
            
            _character = character;
            if (!_character.TryRegisterSystem<ICharacterEffectsSystem>(this)) return false;
            
            _effects = new List<ICharacterEffect>();
            _cancelledEffects = new List<ICharacterEffect>();
            
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

            foreach (var t in _cancelledEffects)
            {
                _effects.Remove(t);
            }
            _cancelledEffects.Clear();
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
            effect.OnEffectCanceled += OnEffectCancelled;
            OnEffectAdded?.Invoke(effect);
        }

        private void OnEffectCancelled(ICharacterEffect effect)
        {
            _cancelledEffects.Add(effect);
        }
    }
}
