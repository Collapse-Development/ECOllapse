using UnityEngine;
using _Project.Code.Features.Character.MB.AttackSystem;

[CreateAssetMenu(fileName = "New CharacterAttackSystemConfig", menuName = "Scriptable Objects/Character/Systems/Attack/CharacterAttackSystem")]
public class CharacterAttackSystemConfig : CharacterSystemConfig<CharacterAttackSystem>
{
    [Header("Attack Settings")]
    public float AttackDamage = 10f;
    public float AttackDelay = 0.5f;
    public float AttackRange = 2f;
    public float AttackRadius = 1.5f;
    public LayerMask TargetLayers = Physics.DefaultRaycastLayers;
}