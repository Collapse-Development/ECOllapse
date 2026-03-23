using UnityEngine;

namespace _Project.Code.Features.Character.MB.NeedsSystem.Satiety
{
    /// <summary>
    /// Заглушка системы сытости. Возвращает фиксированное значение.
    /// Заменить на полноценную реализацию когда система будет готова.
    /// </summary>
    public class CharacterSatietySystemStub : MonoBehaviour, ICharacterSatietySystem
    {
        [SerializeField] private float _satiety = 100f;

        public float Satiety => _satiety;

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterSatietySystemStubConfig) return false;

            if (!character.TryRegisterSystem<ICharacterSatietySystem>(this)) return false;

            Debug.Log("SatietySystem (Stub) initialized");
            return true;
        }
    }
}
