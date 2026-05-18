using UnityEngine;
using _Project.Code.Features.Character.MB;


public abstract class CharacterMutation : ScriptableObject, ICharacterMutation
{
    [TextArea]
    public string Description;

    public abstract void Apply(CharacterBuildConfig cfg);
}
