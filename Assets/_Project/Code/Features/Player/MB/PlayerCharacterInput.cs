using System;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.MB.MovementSystem;
using _Project.Code.Features.Character.MB.FirmnessSystem;
using _Project.Code.Features.Player.MB;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerCharacterInput : MonoBehaviour
{
    private Player _player;

    private InputAction _moveAction;
    private InputAction _runAction;

    private ICharacterMovementSystem _movementSystem;
    private ICharacterFirmnessSystem _firmnessSystem;
    private float _stunTimer;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _player.OnCharacterUpdated += OnCharacterUpdated;

        SetUpActions();
    }

    private void OnCharacterUpdated(Character old, Character current)
    {
        if (old != null && _firmnessSystem != null)
        {
            _firmnessSystem.OnStunned -= HandleStun;
        }
        _movementSystem = current?.GetSystem<ICharacterMovementSystem>();
        _firmnessSystem = current?.GetSystem<ICharacterFirmnessSystem>();

        // Подписываемся на событие оглушения нового персонажа
        if (_firmnessSystem != null)
        {
            _firmnessSystem.OnStunned += HandleStun;
        }

        Debug.Log("Movement: " + _movementSystem);
    }

    private void Start()
    {
        _movementSystem = _player.Character?.GetSystem<ICharacterMovementSystem>();
        _firmnessSystem = _player.Character?.GetSystem<ICharacterFirmnessSystem>();
        
        if (_firmnessSystem != null)
        {
            _firmnessSystem.OnStunned -= HandleStun; 
            _firmnessSystem.OnStunned += HandleStun;
        }
    }

    private void SetUpActions()
    {
        //Movement
        _moveAction = InputSystem.actions.FindAction("Move");
        
        _runAction = InputSystem.actions.FindAction("Run");
        _runAction.started += (ctx) => UpdateRunInput(true);
        _runAction.canceled += (ctx) => UpdateRunInput(false);
    }

    // Метод стана
    private void HandleStun(float stunDuration)
    {
        _stunTimer = stunDuration; // Запускаем таймер
        
        // Принудительно останавливаем персонажа
        if (_movementSystem != null)
        {
            _movementSystem.SetDirection(Vector3.zero);
            _movementSystem.SetRunning(false);
        }
        
        Debug.Log($"Персонаж оглушен на {stunDuration} секунд! Управление заблокировано.");
    }
    private void Update()
    {
        if (_stunTimer > 0)
        {
            _stunTimer -= Time.deltaTime;
            return; 
        }
        UpdateMoveDirection();
    }

    private void UpdateMoveDirection()
    {
        if (_movementSystem == null) return;
        
        var inputDirection = _moveAction.ReadValue<Vector2>();
        _movementSystem.SetDirection(new Vector3(inputDirection.x, 0, inputDirection.y));
    }
    private void UpdateRunInput(bool enabled)
    {
        if (_stunTimer > 0) return;

        Debug.Log("Enabled: " + enabled);
        if (_movementSystem == null) return;
        
        _movementSystem.SetRunning(enabled);
    }
    private void OnDestroy()
    {
        if (_player != null)
            _player.OnCharacterUpdated -= OnCharacterUpdated;

        if (_firmnessSystem != null)
            _firmnessSystem.OnStunned -= HandleStun;
    }
}
