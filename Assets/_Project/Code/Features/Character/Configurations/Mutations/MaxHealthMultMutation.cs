using UnityEngine;
using _Project.Code.Features.Character.MB;

[CreateAssetMenu(fileName = "MaxHealthMultMutation", menuName = "Scriptable Objects/Character/Mutations/MaxHealthMult")]
public class MaxHealthMultMutation : CharacterMutation
{
    public float multiplier = 1f;

    public override void Apply(CharacterBuildConfig cfg)
    {
        var healthSystem = cfg.Get<CharacterHealthSystemConfig>();
        if (healthSystem != null)
        {
            var clone = ScriptableObject.Instantiate(healthSystem);
            clone.MaxHealth *= multiplier;
            cfg.AddOrReplace(clone);
        }
    }
}