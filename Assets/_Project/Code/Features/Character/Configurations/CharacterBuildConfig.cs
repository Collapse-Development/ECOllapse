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
    }
}