using CharacterSystems;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterHealthRegenSystemConfig", menuName = "Scriptable Objects/Character/Systems/Health/HealthRegenSystem")]
public class CharacterHealthRegenSystemConfig : CharacterSystemConfig<CharacterHealthRegenSystem>
{
    [Tooltip("Максимальная скорость регенерации в HP/сек")]
    public float MaxRegenPerSecond = 1f;
}
