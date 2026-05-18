using UnityEngine;

// Мутация умножает скорость потери жажды; значения меньше 1 замедляют расход.
[CreateAssetMenu(fileName = "ThirstDecreaseMultMutation", menuName = "Scriptable Objects/Character/Mutations/ThirstDecreaseMult")]
public class ThirstDecreaseMultMutation : CharacterMutation
{
    public float multiplier = 1f;

    public override void Apply(CharacterBuildConfig cfg)
    {
        var thirstSystem = cfg.Get<CharacterThirstSystemConfig>();
        if (thirstSystem != null)
        {
            var clone = ScriptableObject.Instantiate(thirstSystem);
            clone.DecreasePerSecond *= multiplier;
            cfg.AddOrReplace(clone);
        }
    }
}
