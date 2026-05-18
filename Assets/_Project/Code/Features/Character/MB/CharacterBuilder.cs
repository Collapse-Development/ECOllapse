using UnityEngine;
using _Project.Code.Features.Character.MB;
using System.Collections.Generic;

public static class CharacterBuilder
{
    public static Character Build(CharacterBuildConfig cfg)
    {
        if (cfg == null)
        {
            Debug.LogError("CharacterBuildConfig is null!");
            return null;
        }
        
        GameObject characterGO = new GameObject("Character");
        
        Character character = characterGO.AddComponent<Character>();
        
        BuildSystems(character, cfg);
        
        Debug.Log($"Character built successfully with {cfg.Systems.Count} systems");
        return character;
    }
    
    private static void BuildSystems(Character character, CharacterBuildConfig cfg)
    {
        var systems = new List<BuiltCharacterSystem>();

        foreach (var config in cfg.Systems)
        {
            if (config == null) continue;
            
            var builtSystem = BuildSystem(character, config);
            if (builtSystem != null)
                systems.Add(builtSystem);
        }

        for (var i = systems.Count - 1; i >= 0; i--)
        {
            var builtSystem = systems[i];
            if (builtSystem.System.TryInitialize(character, builtSystem.Config)) continue;

            DestroyBuiltSystem(character, builtSystem);
            systems.RemoveAt(i);
            Debug.LogError($"Character System({builtSystem.Config.CharacterSystemType}) Initialization Failed");
        }

        for (var i = systems.Count - 1; i >= 0; i--)
        {
            var builtSystem = systems[i];
            if (builtSystem.System.TryResolveDependencies(character)) continue;

            DestroyBuiltSystem(character, builtSystem);
            systems.RemoveAt(i);
            Debug.LogError($"Character System({builtSystem.Config.CharacterSystemType}) Dependency Resolution Failed");
        }
    }
    
    private static BuiltCharacterSystem BuildSystem(Character character, CharacterSystemConfig config)
    {
        var systemType = config.CharacterSystemType;
        
        // Создаем систему на GameObject
        var systemComponent = character.gameObject.AddComponent(systemType);
        var system = systemComponent as ICharacterSystem;

        if (systemComponent == null || system == null)
        {
            Debug.LogError($"Failed to create system of type {systemType}");
            return null;
        }

        if (!system.TryRegister(character))
        {
            Object.Destroy(systemComponent);
            Debug.LogError($"Character System({config.CharacterSystemType}) Registration Failed");
            return null;
        }

        return new BuiltCharacterSystem(config, system, systemComponent);
    }

    private static void DestroyBuiltSystem(Character character, BuiltCharacterSystem builtSystem)
    {
        character.UnregisterSystem(builtSystem.System);
        Object.Destroy(builtSystem.Component);
    }

    private sealed class BuiltCharacterSystem
    {
        public BuiltCharacterSystem(CharacterSystemConfig config, ICharacterSystem system, Component component)
        {
            Config = config;
            System = system;
            Component = component;
        }

        public CharacterSystemConfig Config { get; }
        public ICharacterSystem System { get; }
        public Component Component { get; }
    }
}
