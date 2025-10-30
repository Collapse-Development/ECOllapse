using UnityEngine;
using _Project.Code.Features.Character.MB;

[CreateAssetMenu(fileName = "AddOrReplaceSystemMutation", menuName = "Scriptable Objects/Character/Mutations/AddOrReplaceSystem")]
public class AddOrReplaceSystemMutation : CharacterMutation
{
    public CharacterSystemConfig systemToAddOrReplace;

    public override void Apply(CharacterBuildConfig cfg)
    {
        if (systemToAddOrReplace == null) return;
        var clone = ScriptableObject.Instantiate(systemToAddOrReplace);
        cfg.AddOrReplace(clone);
    }
}