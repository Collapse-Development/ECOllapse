using CharacterSystems;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterHealthSystemConfig", menuName = "Scriptable Objects/Character/Systems/Health/DefaultHealthSystem")]
public class CharacterHealthSystemConfig : CharacterSystemConfig<CharacterHealthSystem>
{
    public float MaxHealth = 5f;
}