using UnityEngine;

// Мутация умножает скорость восстановления выносливости после бега.
[CreateAssetMenu(fileName = "EnduranceRestoreMultMutation", menuName = "Scriptable Objects/Character/Mutations/EnduranceRestoreMult")]
public class EnduranceRestoreMultMutation : CharacterMutation
{
    public float multiplier = 1f;

    public override void Apply(CharacterBuildConfig cfg)
    {
        var enduranceSystem = cfg.Get<CharacterEnduranceSystemConfig>();
        if (enduranceSystem != null)
        {
            var clone = ScriptableObject.Instantiate(enduranceSystem);
            clone.RestorePerSecond *= multiplier;
            cfg.AddOrReplace(clone);
        }
    }
}
