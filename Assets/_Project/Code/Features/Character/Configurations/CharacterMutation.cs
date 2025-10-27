using UnityEngine;
using _Project.Code.Features.Character.MB;


public abstract class CharacterMutation : ScriptableObject, ICharacterMutation
{
    public abstract void Apply(CharacterBuildConfig cfg);
}
