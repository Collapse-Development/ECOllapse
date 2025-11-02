using UnityEngine;
using _Project.Code.Features.Character.MB;

[CreateAssetMenu(fileName = "VigourRateMutation", menuName = "Scriptable Objects/Character/Mutations/VigourRate")]
public class VigourRateMutation : CharacterMutation
{
    public float decrementMultiplier = 1f;

    public override void Apply(CharacterBuildConfig cfg)
    {
        var vigourSystem = cfg.Get<CharacterVigourSystemConfig>();
        if (vigourSystem != null)
        {
            var clone = ScriptableObject.Instantiate(vigourSystem);
            clone.BaseDecrementRate *= decrementMultiplier;
            cfg.AddOrReplace(clone);
        }
    }
}