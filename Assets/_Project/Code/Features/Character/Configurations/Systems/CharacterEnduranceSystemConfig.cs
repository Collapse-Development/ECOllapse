using _Project.Code.Features.Character.MB.EnduranceSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterEnduranceSystemConfig", menuName = "Scriptable Objects/Character/Systems/Endurance/CharacterEnduranceSystem")]
public class CharacterEnduranceSystemConfig : CharacterSystemConfig<CharacterEnduranceSystem>
{
    [Header("Endurance")]
    public float MaxValue = 100f;
    public float CurrentValue = 100f;
    public bool StartFromMaxValue = true;

    [Tooltip("How much CurrentValue decreases per second while character is running.")]
    public float DecreasePerSecond = 10f;

    [Tooltip("How much CurrentValue restores per second while character is not running.")]
    public float RestorePerSecond = 15f;

    [Tooltip("Delay in seconds before restoration starts after running ends.")]
    public float RestoreDelay = 1f;

    [Header("Штраф за истощение")]
    [Tooltip("Продолжительность штрафа при падении выносливости до 0")]
    public float ExhaustionDuration = 10f;
    
    [Tooltip("Сколько CurrentValue восстанавливается в секунду во время истощения")]
    public float ExhaustionRestorePerSecond = 5f;
}
