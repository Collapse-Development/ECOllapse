using UnityEngine;
using _Project.Code.Features.Character.MB;

[CreateAssetMenu(fileName = "MaxHungerMutation", menuName = "Scriptable Objects/Character/Mutations/MaxHunger")]
public class MaxHungerMutation : CharacterMutation
{
    public float maxHungerMultiplier = 1f;

    public override void Apply(CharacterBuildConfig cfg)
    {
        var hungerSystem = cfg.Get<CharacterHungerSystemConfig>();
        if (hungerSystem != null)
        {
            var clone = ScriptableObject.Instantiate(hungerSystem);
            clone.MaxHunger *= maxHungerMultiplier;
            cfg.AddOrReplace(clone);
        }
    }
}