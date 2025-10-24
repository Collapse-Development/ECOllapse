using UnityEngine;
using _Project.Code.Features.Character.MB.Model;
using _Project.Code.Features.Character.MB;

namespace CharacterSystems
{
    public class CharacterModelSystem : MonoBehaviour, ICharacterModelSystem
    {
        [SerializeField] private string _prefabPath;
        [SerializeField] private Character _character;
        public CharacterModel Model { get; private set; }

        private void Awake()
        {
            _character.TryRegisterSystem<ICharacterModelSystem>(this);
        }

        private void Start()
        {
            if (string.IsNullOrEmpty(_prefabPath))
            {
                Debug.LogError("Путь к префабу не задан.");
                return;
            }

            GameObject prefab = Resources.Load<GameObject>(_prefabPath);

            if (_prefabPath == null)
            {
                Debug.LogError("Префаб по заданному пути не найден.");
                return;
            }

            Model = Instantiate(prefab).GetComponent<CharacterModel>();

            if (Model == null)
            {
                Debug.LogError("На указанном префабе отсутствует компонент CharacterModel.");
                return;
            }
        }
    }
}