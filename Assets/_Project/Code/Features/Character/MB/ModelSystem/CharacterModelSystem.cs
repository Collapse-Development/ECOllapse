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

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterModelSystemConfig modelCfg) return false;
            
            _character = character;
            if (!_character.TryRegisterSystem<ICharacterModelSystem>(this)) return false;
            
            _prefabPath = modelCfg.PrefabPath;
            
            Debug.Log($"ModelSystem initialized with config: PrefabPath={_prefabPath}");
            return true;
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