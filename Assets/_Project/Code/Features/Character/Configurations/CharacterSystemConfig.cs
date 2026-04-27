using System;
using _Project.Code.Features.Character.MB;
using UnityEngine;

public abstract class CharacterSystemConfig : ScriptableObject
{
    public abstract Type CharacterSystemType { get; }
    
    [Header("Base System Settings")]
    [Tooltip("Если выключено, система инициализируется, но игнорирует вызовы и Update")]
    public bool IsActive = true; 
}

public abstract class CharacterSystemConfig<T> : CharacterSystemConfig where T : class, ICharacterSystem
{
    public override Type CharacterSystemType => typeof(T);
}