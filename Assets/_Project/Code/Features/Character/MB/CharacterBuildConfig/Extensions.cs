using System;
using System.Collections.Generic;
using System.Linq;

public static class CharacterBuildConfigExtensions
{
    public static T GetConfig<T>(this CharacterBuildConfig cfg) where T : SystemConfig
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
    
    public static SystemConfig GetConfigBySystemType(this CharacterBuildConfig cfg, Type systemType)
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
        
        var config = cfg.Systems.FirstOrDefault(s => s.SystemType == systemType);
        
        if (config == null)
        {
            Debug.LogWarning($"Config for system type {systemType} not found in {cfg.name}");
        }
        
        return config;
    }
}