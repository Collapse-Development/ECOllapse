using UnityEngine;


namespace _Project.Code.Features.Character.MB
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Assets/_Project/Code/Features/Character/MB")]
    public class CharacterMovementSystem : MonoBehaviour, IMovementSystem
    {
        public Vector3 Direction => _direction;
        public float Speed
        {
            get => _speed;
            set
            {
                _speed = Mathf.Max(0f, value);
                UpdateIsMovingFlag();
            }
        }
        
        [SerializeField, Min(0f)] private float _speed = 3.5f;

        private Vector3 _direction = Vector3.zero;

        public bool IsMoving { get; private set; }
        
        private global::Character _character;
        private void Awake()
        {
            _character = GetComponentInParent<global::Character>();

            if (_character == null)
            {
                Debug.LogError($"[{nameof(CharacterMovementSystem)}] Character reference is not set.");
                enabled = false;
                return;
            }
            
            _character.TryRegisterSystem<IMovementSystem>(this);
        }

        private void Update()
        {
            if (!IsMoving) return;
            
            transform.position += _direction * _speed * Time.deltaTime;
        }

        public void SetDirection(Vector3 direction)
        {
            _direction = direction.sqrMagnitude > 1e-6f ? direction.normalized : Vector3.zero;
            UpdateIsMovingFlag();
        }

        private void UpdateIsMovingFlag()
        {
            IsMoving = _speed > 0f && _direction.sqrMagnitude > 0f;
        }
    }
}
