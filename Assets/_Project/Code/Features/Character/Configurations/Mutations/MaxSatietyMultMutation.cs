using UnityEngine;

[CreateAssetMenu(fileName = "MaxSatietyMultMutation", menuName = "Scriptable Objects/Character/Mutations/MaxSatietyMult")]
public class MaxFirmnessMultMutation : CharacterMutation
{
    [Header("Множитель максимальной сытости")]
    public float multiplier = 1f;

    public override void Apply(CharacterBuildConfig cfg)
    {
        
        var satietyConfig = cfg.Get<CharacterFirmnessSystemConfig>();
        
        if (satietyConfig != null)
        {
            var clone = ScriptableObject.Instantiate(satietyConfig);
            
            clone.MaxValue *= multiplier;
            
            cfg.AddOrReplace(clone);
            
            Debug.Log($"[Mutation] MaxSatiety умножена на {multiplier}. Новое значение: {clone.MaxValue}");
        }
        else
        {
            Debug.LogWarning("[Mutation] CharacterSatietySystemStubConfig не найден в конфиге персонажа");
        }
    }
}