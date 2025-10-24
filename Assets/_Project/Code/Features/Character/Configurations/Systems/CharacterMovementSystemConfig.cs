using _Project.Code.Features.Character.MB.MovementSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterMovementSystemConfig", menuName = "Scriptable Objects/Character/Systems/Movement/CharacterMovementSystem")]
public class CharacterMovementSystemConfig : CharacterSystemConfig<CharacterMovementSystem>
{
    public float Speed = 5f;
}