using UnityEngine;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.MB.NeedsSystem.Satiety;

[CreateAssetMenu(fileName = "MaxSatietyMultMutation", menuName = "Scriptable Objects/Character/Mutations/MaxSatietyMult")]
public class MaxSatietyMultMutation : CharacterMutation
{
    [Header("Множитель максимальной сытости")]
    public float multiplier = 1f;

    public override void Apply(CharacterBuildConfig cfg)
    {
        
        var satietyConfig = cfg.Get<CharacterSatietySystemStubConfig>();
        
        if (satietyConfig != null)
        {
            var clone = ScriptableObject.Instantiate(satietyConfig);
            
            clone.maxSatiety *= multiplier;
            
            cfg.AddOrReplace(clone);
            
            Debug.Log($"[Mutation] MaxSatiety умножена на {multiplier}. Новое значение: {clone.maxSatiety}");
        }
        else
        {
            Debug.LogWarning("[Mutation] CharacterSatietySystemStubConfig не найден в конфиге персонажа");
        }
    }
}