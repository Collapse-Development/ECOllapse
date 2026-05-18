using UnityEngine;

// Мутация умножает урон атаки персонажа.
[CreateAssetMenu(fileName = "AttackDamageMultMutation", menuName = "Scriptable Objects/Character/Mutations/AttackDamageMult")]
public class AttackDamageMultMutation : CharacterMutation
{
    public float multiplier = 1f;

    public override void Apply(CharacterBuildConfig cfg)
    {
        var attackSystem = cfg.Get<CharacterAttackSystemConfig>();
        if (attackSystem != null)
        {
            var clone = ScriptableObject.Instantiate(attackSystem);
            clone.AttackDamage *= multiplier;
            cfg.AddOrReplace(clone);
        }
    }
}
