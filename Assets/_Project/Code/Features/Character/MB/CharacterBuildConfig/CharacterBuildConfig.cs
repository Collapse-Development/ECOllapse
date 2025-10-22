using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "CharacterBuildConfig", menuName = "Character Systems/Build Config")]
public class CharacterBuildConfig : ScriptableObject
{
    [SerializeField] private List<SystemConfig> systems = new List<SystemConfig>();
    
    public IReadOnlyList<SystemConfig> Systems => systems;
    
    private void OnValidate()
    {
        ValidateConfigs();
        SortSystemsByOrder();
    }
    
    private void ValidateConfigs()
    {
        // Проверка на null ссылки
        var nullConfigs = systems.Where(s => s == null).ToList();
        foreach (var nullConfig in nullConfigs)
        {
            systems.Remove(nullConfig);
            Debug.LogWarning($"Removed null config from {name}");
        }
        
        // Проверка на дубликаты SystemType
        var duplicateGroups = systems
            .Where(s => s != null)
            .GroupBy(s => s.SystemType)
            .Where(g => g.Count() > 1);
            
        foreach (var group in duplicateGroups)
        {
            Debug.LogWarning($"Duplicate system configs for type {group.Key} found in {name}");
        }
    }
    
    private void SortSystemsByOrder()
    {
        systems = systems.OrderBy(s => s.Order).ToList();
    }
}