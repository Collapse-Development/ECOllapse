using UnityEngine;
using System.Collections.Generic;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.MB.MovementSystem;

public static class CharacterBuilder
{
    public static Character Build(CharacterBuildConfig cfg)
    {
        if (cfg == null)
        {
            Debug.LogError("CharacterBuildConfig is null!");
            return null;
        }

        // Создаем новый GameObject
        GameObject characterGO = new GameObject("Character");
        
        // Добавляем основной компонент Character
        Character character = characterGO.AddComponent<Character>();
        
        // Строим системы по конфигу
        BuildSystems(character, cfg);
        
        Debug.Log($"Character built successfully with {cfg.Systems.Count} systems");
        return character;
    }
    
    private static void BuildSystems(Character character, CharacterBuildConfig cfg)
    {
        foreach (var config in cfg.Systems)
        {
            if (config == null) continue;
            
            BuildSystem(character, config);
        }
    }
    
    private static void BuildSystem(Character character, SystemConfig config)
    {
        var systemType = config.SystemType;
        
        // Создаем систему на GameObject
        var systemComponent = character.gameObject.AddComponent(systemType) as ICharacterSystem;
        
        if (systemComponent == null)
        {
            Debug.LogError($"Failed to create system of type {systemType}");
            return;
        }
        
        // Инициализируем систему в зависимости от типа конфига
        InitializeSystemWithConfig(character, systemComponent, config);
        
        // Регистрируем систему в Character
        RegisterSystem(character, systemType, systemComponent);
    }
    
    private static void InitializeSystemWithConfig(Character character, ICharacterSystem system, SystemConfig config)
    {
        // Для MovementSystem используем специальную инициализацию
        if (config is CharacterMovementSystemConfig movementConfig)
        {
            system.Initialize(character, movementConfig);
        }
        else
        {
            // Для остальных систем - базовая инициализация
            system.Initialize(character);
        }
    }
    
    private static void RegisterSystem(Character character, System.Type systemType, ICharacterSystem system)
    {
        // Используем рефлексию для вызова generic метода
        var method = typeof(Character).GetMethod("TryRegisterSystem");
        var genericMethod = method.MakeGenericMethod(systemType);
        var result = (bool)genericMethod.Invoke(character, new object[] { system });
        
        if (!result)
        {
            Debug.LogError($"Failed to register system {systemType}");
        }
        else
        {
            Debug.Log($"Successfully registered system: {systemType.Name}");
        }
    }
}