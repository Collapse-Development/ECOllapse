using UnityEngine;
using _Project.Code.Features.Character.MB.HungerSystem;

[CreateAssetMenu(fileName = "New HungerSystemConfig", menuName = "Scriptable Objects/Character/Systems/Hunger/HungerSystem")]
public class CharacterHungerSystemConfig : CharacterSystemConfig<CharacterHungerSystem>
{
    [Header("Hunger Settings")]
    public float MaxHunger = 100f;
    public float BaseDecrementRate = 1f; // Единиц в секунду
    
    [Header("Starvation Settings")]
    public float StarvationDamage = 5f; // Урон в секунду при голодании
    public float StarvationThreshold = 5f; // Порог голодания в %
}