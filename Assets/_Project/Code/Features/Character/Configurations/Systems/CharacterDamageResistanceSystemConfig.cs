using CharacterSystems;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterDamageResistanceSystemConfig", menuName = "Scriptable Objects/Character/Systems/DamageResistance/DefaultDamageResistanceSystem")]
public class CharacterDamageResistanceSystemConfig : CharacterSystemConfig<CharacterDamageResistanceSystem>
{
    public float BaseResistance = 0f;
}
