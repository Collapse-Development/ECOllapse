using UnityEngine;
using UnityEngine.Events; // 🟩 ADDED

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
        [SerializeField, Min(0f)] private float _runMultiplier = 1.8f; // 🟩 ADDED — множитель скорости при беге

        public bool IsMoving { get; private set; }
        public bool IsRunning { get; private set; } // 🟩 ADDED — флаг бега

        private Character _character;
        private Vector3 _direction = Vector3.zero;
        private Rigidbody _rb;

        private void Awake()
        {
            _character = GetComponentInParent<Character>();

            if (_character == null)
            {
                Debug.LogError($"[{nameof(CharacterMovementSystem)}] Character reference is not set.");
                enabled = false;
                return;
            }

            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;
            _rb.freezeRotation = true;

            _character.TryRegisterSystem<ICharacterMovementSystem>(this);
        }

        private void FixedUpdate()
        {
            if (!IsMoving) return;

            var dir = _direction;

            if (dir.sqrMagnitude <= 1e-6f) return;
            dir.Normalize();

            float currentSpeed = IsRunning ? _speed * _runMultiplier : _speed; // 🟩 ADDED

            var delta = dir * (currentSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(_rb.position + delta);
        }

        public void SetDirection(Vector3 direction)
        {
            _direction = direction.sqrMagnitude > 1e-6f ? direction.normalized : Vector3.zero;
            UpdateIsMovingFlag();
        }

        public void SetRunning(bool isRunning) // 🟩 ADDED — метод управления бегом
        {
            IsRunning = isRunning;
        }

        private void UpdateIsMovingFlag()
        {
            IsMoving = _speed > 0f && _direction.sqrMagnitude > 0f;
        }
    }

    // ========================================================================
    // 🟩 ADDED — КЛАСС PLAYER INPUT (в том же файле)
    // ========================================================================
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private CharacterMovementSystem _movementSystem;

        public UnityEvent<bool> OnRunInput = new UnityEvent<bool>(); // 🟩 ADDED — событие Shift

        private void Awake()
        {
            if (_movementSystem != null)
            {
                OnRunInput.AddListener(_movementSystem.SetRunning); // 🟩 ADDED
            }
        }

        private void Update()
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift); // 🟩 ADDED
            OnRunInput.Invoke(isRunning); // 🟩 ADDED
        }
    }
}

