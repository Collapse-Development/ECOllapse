using UnityEngine;
using _Project.Code.Features.Character.MB;

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
        foreach (var config in cfg.Systems)
        {
            if (config == null) continue;
            
            BuildSystem(character, config);
        }
    }
    
    private static void BuildSystem(Character character, CharacterSystemConfig config)
    {
        var systemType = config.CharacterSystemType;
        
        // Создаем систему на GameObject
        var systemComponent = character.gameObject.AddComponent(systemType);
        var system = systemComponent as ICharacterSystem;

        if (systemComponent == null || system == null)
        {
            Debug.LogError($"Failed to create system of type {systemType}");
            return;
        }

        if (!system.TryInitialize(character, config))
        {
            Object.Destroy(systemComponent);
            Debug.LogError($"Character System({config.CharacterSystemType}) Initialization Failed");
        }
    }
}