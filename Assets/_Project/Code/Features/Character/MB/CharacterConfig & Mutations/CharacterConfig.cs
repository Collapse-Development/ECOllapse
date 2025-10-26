using UnityEngine;
using System.Collections.Generic;
using _Project.Code.Features.Character.MB;

[CreateAssetMenu(fileName = "New CharacterConfig", menuName = "Scriptable Objects/Character/CharacterConfig")]
public class CharacterConfig : ScriptableObject
{
    public CharacterBuildConfig baseBuildConfig;
    public List<CharacterMutation> mutations = new();

    public CharacterBuildConfig GetBuildConfig()
    {
        if (baseBuildConfig == null)
        {
            Debug.LogWarning($"{name}: baseBuildConfig is null!");
            return null;
        }

        // 1. Клонируем базу
        var buildCopy = baseBuildConfig.DeepClone();

        // 2. Применяем мутации последовательно
        foreach (var mutation in mutations)
        {
            if (mutation != null)
                mutation.Apply(buildCopy);
        }

        // 3. Возвращаем результат
        return buildCopy;
    }
}
