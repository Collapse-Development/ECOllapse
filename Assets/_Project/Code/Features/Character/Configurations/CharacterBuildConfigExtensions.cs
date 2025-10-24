using System;
using System.Linq;
using UnityEngine;

public static class CharacterBuildConfigExtensions
{
    public static T GetConfig<T>(this CharacterBuildConfig cfg) where T : CharacterSystemConfig
    {
        if (cfg == null)
        {
            Debug.LogError("CharacterBuildConfig is null");
            return null;
        }
        
        var config = cfg.Systems.FirstOrDefault(s => s is T) as T;
        
        if (config == null)
        {
            Debug.LogWarning($"Config of type {typeof(T)} not found in {cfg.name}");
        }
        
        return config;
    }
    
    public static CharacterSystemConfig GetConfigBySystemType(this CharacterBuildConfig cfg, Type systemType)
    {
        if (cfg == null)
        {
            Debug.LogError("CharacterBuildConfig is null");
            return null;
        }
        
        if (systemType == null)
        {
            Debug.LogError("SystemType is null");
            return null;
        }
        
        var config = cfg.Systems.FirstOrDefault(s => s.CharacterSystemType == systemType);
        
        if (config == null)
        {
            Debug.LogWarning($"Config for system type {systemType} not found in {cfg.name}");
        }
        
        return config;
    }
}