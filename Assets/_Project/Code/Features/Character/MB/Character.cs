using System;
using System.Collections.Generic;
using _Project.Code.Features.Character.MB;
using JetBrains.Annotations;
using UnityEngine;

namespace _Project.Code.Features.Character.MB
{
    public class Character : MonoBehaviour
    {
        private readonly Dictionary<Type, ICharacterSystem> _systems = new();
        
        // ========== ПОТРЕБНОСТИ И СОСТОЯНИЯ ==========
        [Header("Потребности и состояние")]
        [SerializeField] private bool _hasFood = true;
        [SerializeField] private bool _hasWater = true;
        [SerializeField] private bool _isMoving = false;
        [SerializeField] private float _bodyTemperature = 36.6f;
        
        // ========== ПУБЛИЧНЫЕ СВОЙСТВА ==========
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
        
        public bool IsMoving 
        { 
            get => _isMoving; 
            set => _isMoving = value; 
        }
        
        public float BodyTemperature 
        { 
            get => _bodyTemperature; 
            set => _bodyTemperature = Mathf.Clamp(value, 28f, 41f); 
        }
        
        // ========== РЕГИСТРАЦИЯ СИСТЕМ ==========
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
            {
                Debug.LogError("Нельзя запрашивать ICharacterSystem напрямую.");
                return null;
            }

            if (_systems.TryGetValue(type, out var system))
                return (T)system;

            Debug.LogError($"Система типа {type.Name} не найдена.");
            return null;
        }
        
        // ========== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ==========
        public void UpdateMovingState(Vector3 velocity)
        {
            IsMoving = velocity.sqrMagnitude > 0.1f;
        }
        
        public void SetNeeds(bool hasFood, bool hasWater)
        {
            _hasFood = hasFood;
            _hasWater = hasWater;
        }
        
        public void ModifyTemperature(float delta)
        {
            BodyTemperature += delta;
        }
    }
}