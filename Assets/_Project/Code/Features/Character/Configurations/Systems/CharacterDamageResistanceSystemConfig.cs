using CharacterSystems;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterDamageResistanceSystemConfig", menuName = "Scriptable Objects/Character/Systems/DamageResistance/DefaultDamageResistanceSystem")]
public class CharacterDamageResistanceSystemConfig : CharacterSystemConfig<CharacterDamageResistanceSystem>
{
    public float BaseResistance = 0f;

    [Tooltip("Коэффициент затухания урона по сопротивлению (k в формуле e^(-kR))")]
    public float ResistanceDecayK = 0.05f;

    [Tooltip("Абсолютное поглощение урона (Dabs). При R=0 и уроне=10 нанесётся 9 урона → Dabs=1")]
    public float AbsorbedDamage = 1f;
}
