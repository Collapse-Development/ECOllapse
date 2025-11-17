using UnityEngine;
using _Project.Code.Features.Character.MB;

[CreateAssetMenu(fileName = "TemperatureResistanceMutation", menuName = "Scriptable Objects/Character/Mutations/TemperatureResistance")]
public class TemperatureResistanceMutation : CharacterMutation
{
    public float coldResistanceBonus = 0f;
    public float heatResistanceBonus = 0f;

    public override void Apply(CharacterBuildConfig cfg)
    {
        var tempSystem = cfg.Get<CharacterTemperatureSystemConfig>();
        if (tempSystem != null)
        {
            var clone = ScriptableObject.Instantiate(tempSystem);
            // Мутация может изменять максимальное сопротивление
            // Или добавлять бонусы к сопротивлению
            cfg.AddOrReplace(clone);
        }
    }
}