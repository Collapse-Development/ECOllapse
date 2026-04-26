using _Project.Code.Features.Character.MB.Thirst;
using UnityEngine;

[CreateAssetMenu(fileName = "New DefaultCharacterThirstSystemConfig", menuName = "Scriptable Objects/Character/Systems/Thirst/DefaultThirstSystem")]
public class CharacterThirstSystemConfig : CharacterSystemConfig<CharacterThirstSystem>
{
    [Header("Thirst")]
    public float MaxValue = 100f;
    public float CurrentValue = 100f;
    public bool StartFromMaxValue = true;

    [Tooltip("How much CurrentValue decreases per second.")]
    public float DecreasePerSecond = 1f;

    [Tooltip("Multiplier applied to DecreasePerSecond while character is running.")]
    public float RunningDecreaseMultiplier = 2f;
}
