using System;
using _Project.Code.Features.Character.MB;
using UnityEngine;

public abstract class CharacterSystemConfig : ScriptableObject
{
    public abstract Type CharacterSystemType { get; }
}

public abstract class CharacterSystemConfig<T> : CharacterSystemConfig where T : class, ICharacterSystem
{
    public override Type CharacterSystemType => typeof(T);
}
