using _Project.Code.Features.Character.MB.FirmnessSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterFirmnessSystemConfig", menuName = "Scriptable Objects/Character/Systems/Firmness/CharacterFirmnessSystem")]
public class CharacterFirmnessSystemConfig : CharacterSystemConfig<CharacterFirmnessSystem>
{
    [Header("Стойкость")]
    public float MaxValue = 100f;
    public float MinValue = -20f;
    public float CurrentValue = 100f;
    public bool StartFromMaxValue = true;

    [Header("Параметры регенерации")]
    [Tooltip("Базовая регенерация в секунду при значении «Выносливость» равном 0")]
    public float MinRegenPerSecond = 10f;
    [Tooltip("Максимальная регенерация в секунду при максимальном значении параметра «Выносливость»")]
    public float MaxRegenPerSecond = 20f;

    [Header("Модификаторы урона и оглушения")]
    [Tooltip("Коэффициент усиления урона от ударов при прыжке персонажа")]
    public float JumpDamageMultiplier = 1.5f;
    [Tooltip("Сколько секунд оглушения за каждый пункт стойкости ниже нуля")]
    public float StunDurationPerNegativePoint = 0.2f;
}