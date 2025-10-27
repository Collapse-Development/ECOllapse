using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using _Project.Code.Features.Character.MB;

public static class CharacterBuildConfigExtensions
{
    // СТАРЫЕ МЕТОДЫ
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

    // НОВЫЕ МЕТОДЫ ДЛЯ МУТАЦИЙ
    public static CharacterBuildConfig DeepClone(this CharacterBuildConfig original)
    {
        if (original == null)
            return null;
        
        var clone = ScriptableObject.Instantiate(original);
        clone.Systems.Clear();
        
        foreach (var system in original.Systems)
        {
            if (system != null)
            {
                var systemClone = ScriptableObject.Instantiate(system);
                clone.Systems.Add(systemClone);
            }
        }
        
        return clone;
    }

    public static T Get<T>(this CharacterBuildConfig cfg) where T : CharacterSystemConfig
    {
        return cfg.Systems.OfType<T>().FirstOrDefault();
    }

    public static CharacterSystemConfig GetBySystemType(this CharacterBuildConfig cfg, Type t)
    {
        return cfg.Systems.FirstOrDefault(s => s != null && s.GetType() == t);
    }

    public static void Add(this CharacterBuildConfig cfg, CharacterSystemConfig sc)
    {
        if (sc != null)
            cfg.Systems.Add(sc);
    }

    public static void AddOrReplace(this CharacterBuildConfig cfg, CharacterSystemConfig sc)
    {
        if (sc == null) return;

        var existing = cfg.Systems.FirstOrDefault(s => s.GetType() == sc.GetType());
        if (existing != null)
            cfg.Systems.Remove(existing);

        cfg.Systems.Add(sc);
    }

    public static bool RemoveBySystemType(this CharacterBuildConfig cfg, Type t)
    {
        var item = cfg.Systems.FirstOrDefault(s => s.GetType() == t);
        if (item != null)
        {
            cfg.Systems.Remove(item);
            return true;
        }
        return false;
    }
}