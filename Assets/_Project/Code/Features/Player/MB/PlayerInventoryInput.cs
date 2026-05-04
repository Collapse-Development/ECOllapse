using _Project.Code.Features.Character.MB.InventorySystem;
using _Project.Code.Features.Player.MB;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Обрабатывает ввод игрока для инвентаря:
///   Interact  — подобрать ближайший предмет в радиусе PickupRange
///   Attack    — использовать активный предмет
///   Next      — следующий слот
///   Previous  — предыдущий слот
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerInventoryInput : MonoBehaviour
{
    private Player _player;
    private ICharacterInventorySystem _inventory;

    private InputAction _interactAction;
    private InputAction _attackAction;
    private InputAction _nextAction;
    private InputAction _previousAction;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _player.OnCharacterUpdated += OnCharacterUpdated;

        _interactAction = InputSystem.actions.FindAction("Interact");
        _attackAction   = InputSystem.actions.FindAction("Attack");
        _nextAction     = InputSystem.actions.FindAction("Next");
        _previousAction = InputSystem.actions.FindAction("Previous");

        _interactAction.performed += OnInteract;
        _attackAction.performed   += OnAttack;
        _nextAction.performed     += OnNext;
        _previousAction.performed += OnPrevious;
    }

    private void OnDestroy()
    {
        if (_interactAction != null) _interactAction.performed -= OnInteract;
        if (_attackAction   != null) _attackAction.performed   -= OnAttack;
        if (_nextAction     != null) _nextAction.performed     -= OnNext;
        if (_previousAction != null) _previousAction.performed -= OnPrevious;
    }

    private void OnCharacterUpdated(
        _Project.Code.Features.Character.MB.Character old,
        _Project.Code.Features.Character.MB.Character current)
    {
        _inventory = current?.GetSystem<ICharacterInventorySystem>();
    }

    private void OnInteract(InputAction.CallbackContext _) => TryPickupNearest();
    private void OnAttack(InputAction.CallbackContext _)   => _inventory?.UseActiveItem();
    private void OnNext(InputAction.CallbackContext _)     => _inventory?.SelectNextSlot();
    private void OnPrevious(InputAction.CallbackContext _) => _inventory?.SelectPreviousSlot();

    private void TryPickupNearest()
    {
        if (_inventory == null || _player.Character == null) return;

        var character = _player.Character;
        float range = _inventory.PickupRange;

        var colliders = Physics.OverlapSphere(character.transform.position, range);
        ItemPickupBase nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var col in colliders)
        {
            var pickup = col.GetComponent<ItemPickupBase>();
            if (pickup == null) continue;

            float dist = Vector3.Distance(character.transform.position, pickup.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = pickup;
            }
        }

        nearest?.TryPickup(character);
    }
}
