using System;
using System.Collections.Generic;
using _Project.Code.Features.Character.MB;
using JetBrains.Annotations;
using UnityEngine;

namespace _Project.Code.Features.Character.MB
{
    public class Character : MonoBehaviour
    {
        private readonly Dictionary<Type, ICharacterSystem> _systems = new ();

        public bool TryRegisterSystem<T>(T system) where T : class, ICharacterSystem
        {
            var type = typeof(T);

            if (type == typeof(ICharacterSystem))
            {
                Debug.LogError("Нельзя зарегистрировать ICharacterSystem напрямую.");
                return false;
            }

            if (!_systems.TryAdd(type, system))
            {
                Debug.LogError($"Система типа {type.Name} уже зарегистрирована.");
                return false;
            }

            return true;
        }

        [CanBeNull]
        public T GetSystem<T>() where T : class, ICharacterSystem
        {
            var type = typeof(T);

            if (type == typeof(ICharacterSystem))
                Debug.LogError("Нельзя запрашивать ICharacterSystem напрямую.");

            if (_systems.TryGetValue(type, out var system))
                return (T)system;

            Debug.LogError($"Система типа {type.Name} не найдена.");
            return null;
        }

        // ========== СВОЙСТВА ДЛЯ СИСТЕМЫ БОДРОСТИ ==========
        [Header("Потребности")]
        [SerializeField] private bool _hasFood = true;
        [SerializeField] private bool _hasWater = true;
        [SerializeField] private float _bodyTemperature = 36.6f;
        [SerializeField] private bool _isMoving = false;

        public bool HasFood 
        { 
            get => _hasFood; 
            set => _hasFood = value; 
        }

        public bool HasWater 
        { 
            get => _hasWater; 
            set => _hasWater = value; 
        }

        public float BodyTemperature 
        { 
            get => _bodyTemperature; 
            set => _bodyTemperature = Mathf.Clamp(value, 28f, 41f); 
        }

        public bool IsMoving 
        { 
            get => _isMoving; 
            set => _isMoving = value; 
        }
    }
}