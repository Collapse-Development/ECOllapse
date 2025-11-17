using UnityEngine;
using _Project.Code.Features.Character.MB;

[CreateAssetMenu(fileName = "EnduranceMutation", menuName = "Scriptable Objects/Character/Mutations/Endurance")]
public class EnduranceMutation : CharacterMutation
{
    public float maxEnduranceMultiplier = 1f;
    public float regenMultiplier = 1f;

    public override void Apply(CharacterBuildConfig cfg)
    {
        var enduranceSystem = cfg.Get<CharacterEnduranceSystemConfig>();
        if (enduranceSystem != null)
        {
            var clone = ScriptableObject.Instantiate(enduranceSystem);
            clone.MaxEndurance *= maxEnduranceMultiplier;
            cfg.AddOrReplace(clone);
        }
    }
}