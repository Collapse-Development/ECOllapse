using UnityEngine;
using _Project.Code.Features.Character.MB.EnduranceSystem;

[CreateAssetMenu(fileName = "New EnduranceSystemConfig", menuName = "Scriptable Objects/Character/Systems/Endurance/EnduranceSystem")]
public class CharacterEnduranceSystemConfig : CharacterSystemConfig<CharacterEnduranceSystem>
{
    [Header("Endurance Settings")]
    public float MaxEndurance = 100f;
    public float MinEndurance = -20f;
    
    [Header("Damage Multipliers")]
    public float StandingMultiplier = 0.7f;
    public float JumpingMultiplier = 1.5f;
    public float FallingMultiplier = 2f;
    
    [Header("Stun Settings")]
    public float StunDurationMultiplier = 0.1f;
    public float MaxStunDuration = 5f;
}