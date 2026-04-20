using _Project.Code.Features.Character.MB.Vigor;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterVigorSystemConfig", menuName = "Scriptable Objects/Character/Systems/Vigor/DefaultVigorSystem")]
public class CharacterVigorSystemConfig : CharacterSystemConfig<CharacterVigorSystem>
{
    [Header("Vigor")]
    public float MaxValue = 100f;
    public float CurrentValue = 100f;
    public bool StartFromMaxValue = true;

    [Tooltip("How much CurrentValue decreases per second.")]
    public float DecreasePerSecond = 1f;
}
