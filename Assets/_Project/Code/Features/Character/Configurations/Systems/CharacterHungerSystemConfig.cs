using UnityEngine;
using _Project.Code.Features.Character.MB.HungerSystem;

[CreateAssetMenu(fileName = "New HungerSystemConfig", menuName = "Scriptable Objects/Character/Systems/Hunger/HungerSystem")]
public class CharacterHungerSystemConfig : CharacterSystemConfig<CharacterHungerSystem>
{
    [Header("Hunger Settings")]
    public float MaxHunger = 100f;
    public float BaseDecrementRate = 4.17f;
    
    [Header("Starvation Settings")]
    public float StarvationDamage = 5f;
}