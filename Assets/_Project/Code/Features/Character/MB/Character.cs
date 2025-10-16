using System;
using System.Collections.Generic;
using _Project.Code.Features.Character.MB;
using JetBrains.Annotations;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Dictionary<Type, ICharacterSystem> _systems;
    
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

        // Запрещаем прямой запрос ICharacterSystem
        if (type == typeof(ICharacterSystem))
            Debug.LogError("Нельзя запрашивать ICharacterSystem напрямую.");

        if (_systems.TryGetValue(type, out var system))
            return (T)system;

        Debug.LogError($"Система типа {type.Name} не найдена.");

        return null;
    }
}
