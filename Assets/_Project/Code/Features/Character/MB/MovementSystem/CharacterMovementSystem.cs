using UnityEngine;

namespace _Project.Code.Features.Character.MB.MovementSystem
{
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class CharacterMovementSystem : MonoBehaviour, ICharacterMovementSystem
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
        
        public bool IsMoving { get; private set; }

        private Character _character;
        private Vector3 _direction = Vector3.zero;
        private Rigidbody _rb;
        
        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            var movementCfg = cfg as CharacterMovementSystemConfig;
            if (movementCfg == null)
            {
                Debug.Log("Fuck1");
                return false;
            }
            
            _character = character;
            if (!_character.TryRegisterSystem<ICharacterMovementSystem>(this))
            {
                Debug.Log("Fuck2");
                return false;
            }
            
            _speed = movementCfg.Speed;
            
            Debug.Log($"MovementSystem initialized with config: Speed={_speed}");
            return true;
        }
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;
            _rb.freezeRotation = true;
        }

        private void FixedUpdate()
        {
            if (!IsMoving) return;

            var dir = _direction;

            if (dir.sqrMagnitude <= 1e-6f) return;
            dir.Normalize();

            var delta = dir * (_speed * Time.fixedDeltaTime);
            _rb.MovePosition(_rb.position + delta);
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