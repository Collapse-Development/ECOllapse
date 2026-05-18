using UnityEngine;

// Мутация добавляет фиксированный бонус к базовой защите от урона.
[CreateAssetMenu(fileName = "DamageResistanceAddMutation", menuName = "Scriptable Objects/Character/Mutations/DamageResistanceAdd")]
public class DamageResistanceAddMutation : CharacterMutation
{
    public float bonusResistance = 0f;

    public override void Apply(CharacterBuildConfig cfg)
    {
        var resistanceSystem = cfg.Get<CharacterDamageResistanceSystemConfig>();
        if (resistanceSystem != null)
        {
            var clone = ScriptableObject.Instantiate(resistanceSystem);
            clone.BaseResistance += bonusResistance;
            cfg.AddOrReplace(clone);
        }
    }
}
