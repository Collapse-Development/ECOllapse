using System;
using _Project.Code.Features.Character.MB.MovementSystem;
using _Project.Code.Features.Player.MB;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerCharacterInput : MonoBehaviour
{
    private Player _player;

    private InputAction _moveAction;

    private ICharacterMovementSystem _movementSystem;

    private void Awake()
    {
        _player = GetComponent<Player>();

        SetUpActions();
    }

    private void Start()
    {
        _movementSystem = _player.Character.GetSystem<ICharacterMovementSystem>();
    }

    private void SetUpActions()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        UpdateMoveDirection();
    }

    private void UpdateMoveDirection()
    {
        if (_movementSystem == null) return;
        
        var inputDirection = _moveAction.ReadValue<Vector2>();
        _movementSystem.SetDirection(new Vector3(inputDirection.x, 0, inputDirection.y));
    }
}
