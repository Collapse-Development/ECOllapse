using UnityEngine;
using _Project.Code.Features.Character.MB;

[CreateAssetMenu(fileName = "MovementSpeedMultMutation", menuName = "Scriptable Objects/Character/Mutations/MovementSpeedMult")]
public class MovementSpeedMultMutation : CharacterMutation
{
    public float multiplier = 1f;

    public override void Apply(CharacterBuildConfig cfg)
    {
        var movementSystem = cfg.Get<CharacterMovementSystemConfig>();
        if (movementSystem != null)
        {
            var clone = ScriptableObject.Instantiate(movementSystem);
            clone.Speed *= multiplier;
            cfg.AddOrReplace(clone);
        }
    }
}