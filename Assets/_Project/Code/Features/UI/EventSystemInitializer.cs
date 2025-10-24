using UnityEngine;

namespace _Project.Scripts.General.Instances
{
    public class EventSystemInitializer : MonoBehaviour
    {
        private static bool _initialized;
        
        [SerializeField] private GameObject _eventSystemPrefab;

        public void Awake()
        {
            if (_initialized) return;

            Instantiate(_eventSystemPrefab, transform);
            DontDestroyOnLoad(gameObject);
            _initialized = true;
        }
    }
}
