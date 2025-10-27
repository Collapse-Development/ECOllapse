using UnityEngine;
using System;
using _Project.Code.Features.Character.MB;

[CreateAssetMenu(fileName = "RemoveSystemByTypeMutation", menuName = "Scriptable Objects/Character/Mutations/RemoveSystemByType")]
public class RemoveSystemByTypeMutation : CharacterMutation
{
    [SerializeField] private string systemTypeName;

    public override void Apply(CharacterBuildConfig cfg)
    {
        if (string.IsNullOrEmpty(systemTypeName))
            return;

        var t = Type.GetType(systemTypeName);
        if (t == null)
        {
            Debug.LogWarning($"Type not found: {systemTypeName}");
            return;
        }

        cfg.RemoveBySystemType(t);
    }
}
