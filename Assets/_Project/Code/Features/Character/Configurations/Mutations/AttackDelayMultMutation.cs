using UnityEngine;

// Мутация умножает задержку между атаками; значения меньше 1 ускоряют атаки.
[CreateAssetMenu(fileName = "AttackDelayMultMutation", menuName = "Scriptable Objects/Character/Mutations/AttackDelayMult")]
public class AttackDelayMultMutation : CharacterMutation
{
    public float multiplier = 1f;

    public override void Apply(CharacterBuildConfig cfg)
    {
        var attackSystem = cfg.Get<CharacterAttackSystemConfig>();
        if (attackSystem != null)
        {
            var clone = ScriptableObject.Instantiate(attackSystem);
            clone.AttackDelay *= multiplier;
            cfg.AddOrReplace(clone);
        }
    }
}
