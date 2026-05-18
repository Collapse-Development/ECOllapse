using _Project.Code.Features.Player.MB;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerEggInput : MonoBehaviour
{
    private Player _player;
    private InputAction _checkpointAction;

    private void Awake()
    {
        _player = GetComponent<Player>();

        _checkpointAction = InputSystem.actions.FindAction("Checkpoint");
        if (_checkpointAction != null)
            _checkpointAction.performed += OnCheckpoint;
    }

    private void OnDestroy()
    {
        if (_checkpointAction != null)
            _checkpointAction.performed -= OnCheckpoint;
    }

    private void OnCheckpoint(InputAction.CallbackContext _)
    {
        var checkpointSystem = FindAnyObjectByType<GameLoop>()?.GameSceneContext?.PlayerEggCheckpointSystem;
        checkpointSystem?.TryLayEgg(_player.Character);
    }
}
