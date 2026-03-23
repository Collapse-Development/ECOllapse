using UnityEngine;

namespace _Project.Code.Features.Character.MB.NeedsSystem.Hydration
{
    /// <summary>
    /// Заглушка системы гидратации. Возвращает фиксированное значение.
    /// Заменить на полноценную реализацию когда система будет готова.
    /// </summary>
    public class CharacterHydrationSystemStub : MonoBehaviour, ICharacterHydrationSystem
    {
        [SerializeField] private float _hydration = 100f;

        public float Hydration => _hydration;

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterHydrationSystemStubConfig) return false;

            if (!character.TryRegisterSystem<ICharacterHydrationSystem>(this)) return false;

            Debug.Log("HydrationSystem (Stub) initialized");
            return true;
        }
    }
}
