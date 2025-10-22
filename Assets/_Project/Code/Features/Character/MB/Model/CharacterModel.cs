using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.Model
{
    [DisallowMultipleComponent]
    public class CharacterModel : MonoBehaviour
    {
        private readonly Dictionary<Type, ISystem> _systems = new();
        
        public bool TryRegisterSystem<T>(T system) where T : class, ISystem
        {
            var type = typeof(T);

            if (type == typeof(ISystem))
            {
                Debug.LogError("Нахуй ты напрямую ISystem регать пытаешься.");
                return false;
            }

            if (!_systems.TryAdd(type, system))
            {
                Debug.LogError($"{type.Name} уже зареган");
                return false;
            }

            return true;
        }

        public T GetSystem<T>() where T : class, ISystem
        {
            var type = typeof(T);

            if (type == typeof(ISystem))
                Debug.LogError("Нахуй ты напрямую ISystem запрашивать пытаешься.");

            if (_systems.TryGetValue(type, out var system))
                return (T)system;

            Debug.LogError($"{type.Name} нету");
            return null;
        }
    }
}
