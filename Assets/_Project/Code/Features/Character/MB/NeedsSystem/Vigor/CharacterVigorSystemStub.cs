using UnityEngine;

namespace _Project.Code.Features.Character.MB.NeedsSystem.Vigor
{
    /// <summary>
    /// Заглушка системы бодрости. Возвращает фиксированное значение.
    /// Заменить на полноценную реализацию когда система будет готова.
    /// </summary>
    public class CharacterVigorSystemStub : MonoBehaviour, ICharacterVigorSystem
    {
        [SerializeField] private float _vigor = 100f;

        public float Vigor => _vigor;

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterVigorSystemStubConfig) return false;

            if (!character.TryRegisterSystem<ICharacterVigorSystem>(this)) return false;

            Debug.Log("VigorSystem (Stub) initialized");
            return true;
        }
    }
}
