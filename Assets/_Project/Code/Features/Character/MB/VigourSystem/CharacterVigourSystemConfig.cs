using UnityEngine;
using _Project.Code.Features.Character.MB.VigourSystem;

[CreateAssetMenu(fileName = "New VigourSystemConfig", menuName = "Scriptable Objects/Character/Systems/Vigour/VigourSystem")]
public class CharacterVigourSystemConfig : CharacterSystemConfig<CharacterVigourSystem>
{
    [Header("Vigour Settings")]
    public float MaxVigour = 100f;
    public float BaseDecrementRate = 4f; // 100 единиц за 25 часов
    
    [Header("Sleep Thresholds")]
    public float WarningThreshold = 40f;
    public float TiredThreshold = 35f;
    public float CanSleepThreshold = 50f;
    public float SleepDeprivedTimeThreshold = 24f; // часов
    public float CriticalSleepDeprivedTimeThreshold = 72f; // часов
    
    [Header("Sleep Settings")]
    public float BaseSleepVigourGain = 8f; // за игровой час
}