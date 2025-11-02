using System;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.MovementSystem
{
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class CharacterMovementSystem : MonoBehaviour
    {

        public Vector3 Direction => _direction;
        public bool IsMoving { get; private set; }
        public bool IsRunning { get; private set; }

        public float Speed
        {
            get => _speed;
            set
            {
                _speed = Mathf.Max(0f, value);
                UpdateIsMovingFlag();
            }
        }


        [Header("Movement Settings")]
        [SerializeField, Min(0f)] private float _walkSpeed = 3.5f;
        [SerializeField, Min(0f)] private float _runSpeed = 6.5f;

  
        private Rigidbody _rb;
        private Vector3 _direction = Vector3.zero;
        private float _speed;
        private float _frameSpeedMultiplier = 1f;

 
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;
            _rb.freezeRotation = true;
            _speed = _walkSpeed;
        }

        private void Update()
        {
            // Пересчитываем скорость (в зависимости от того, бежим ли мы)
            Speed = (IsRunning ? _runSpeed : _walkSpeed) * _frameSpeedMultiplier;
            _frameSpeedMultiplier = 1f;
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

        public void SetRunning(bool isRunning)
        {
            IsRunning = isRunning;
        }

        public void ApplyFrameSpeedMultiplier(float multiplier)
        {
            _frameSpeedMultiplier *= multiplier;
        }

        private void UpdateIsMovingFlag()
        {
            IsMoving = _speed > 0f && _direction.sqrMagnitude > 0f;
        }



        [Header("Input Settings")]
        [SerializeField] private bool _useInput = true;

        private void UpdateInput()
        {
            if (!_useInput) return;

            // Направление движения (WASD)
            var moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            SetDirection(moveDir);

            // Спринт (Shift)
            if (Input.GetKeyDown(KeyCode.LeftShift))
                SetRunning(true);
            if (Input.GetKeyUp(KeyCode.LeftShift))
                SetRunning(false);
        }

        // Обновим Update, чтобы обрабатывать и ввод
        private void LateUpdate()
        {
            UpdateInput();
        }
    }
}
