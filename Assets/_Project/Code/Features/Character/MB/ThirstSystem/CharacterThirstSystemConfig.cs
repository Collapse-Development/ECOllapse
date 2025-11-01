using UnityEngine;
using _Project.Code.Features.Character.MB.ThirstSystem;

[CreateAssetMenu(fileName = "New ThirstSystemConfig", menuName = "Scriptable Objects/Character/Systems/Thirst/ThirstSystem")]
public class CharacterThirstSystemConfig : CharacterSystemConfig<CharacterThirstSystem>
{
    [Header("Hydration Settings")]
    public float MaxHydration = 100f;
    public float BaseDecrementRate = 1f;
    
    [Header("Dehydration Thresholds")]
    public float MildDehydrationThreshold = 20f;
    public float DehydrationTimeThreshold = 12f; // часов
    public float CriticalDehydrationTimeThreshold = 24f; // часов
}