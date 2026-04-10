using _Project.Code.Features.Character.MB.NeedsSystem.Satiety;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterSatietySystemStubConfig", menuName = "Scriptable Objects/Character/Systems/Needs/SatietySystemStub")]
public class CharacterSatietySystemStubConfig : CharacterSystemConfig<CharacterSatietySystemStub>
{
    [Header("Параметры сытости")]
    public float maxSatiety = 100f;
    public float satietyDecayRate = 1f;
}
