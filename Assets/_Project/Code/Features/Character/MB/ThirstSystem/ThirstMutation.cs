using UnityEngine;
using _Project.Code.Features.Character.MB;

[CreateAssetMenu(fileName = "ThirstRateMutation", menuName = "Scriptable Objects/Character/Mutations/ThirstRate")]
public class ThirstRateMutation : CharacterMutation
{
    public float decrementMultiplier = 1f;

    public override void Apply(CharacterBuildConfig cfg)
    {
        var thirstSystem = cfg.Get<CharacterThirstSystemConfig>();
        if (thirstSystem != null)
        {
            var clone = ScriptableObject.Instantiate(thirstSystem);
            clone.BaseDecrementRate *= decrementMultiplier;
            cfg.AddOrReplace(clone);
        }
    }
}