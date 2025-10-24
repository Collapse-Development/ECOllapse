using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using _Project.Code.Features.Character.MB;

[CreateAssetMenu(fileName = "New CharacterBuildConfig", menuName = "Scriptable Objects/Character/BuildConfig")]
public class CharacterBuildConfig : ScriptableObject
{
    public List<CharacterSystemConfig> Systems = new();
    
    private void OnValidate()
    {
        ValidateConfigs();
    }
    
    private void ValidateConfigs()
    {
        // Проверка на null ссылки
        var nullConfigs = Systems.Where(s => s == null).ToList();
        foreach (var nullConfig in nullConfigs)
        {
            Systems.Remove(nullConfig);
            Debug.LogWarning($"Removed null config from {name}");
        }
        
        // Проверка на дубликаты SystemType
        var interfaceGroups = Systems
            .Where(s => s != null)
            .SelectMany(s => s.CharacterSystemType
                .GetInterfaces()
                .Where(i => typeof(ICharacterSystem).IsAssignableFrom(i) && i != typeof(ICharacterSystem))
                .Select(i => new { Config = s, Interface = i }))
            .GroupBy(x => x.Interface)
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var group in interfaceGroups)
        {
            Debug.LogWarning(
                $"Duplicate configs implementing {group.Key.Name} found in {name}: " +
                $"{string.Join(", ", group.Select(g => g.Config.CharacterSystemType))}");
        }
    }
}