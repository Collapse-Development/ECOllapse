using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.MB.InventorySystem;
using UnityEngine;

public class PickupTargetIndicator : MonoBehaviour
{
    [SerializeField] private GameUI _gameUI;
    [SerializeField] private GameObject _target;

    private Character _currentCharacter;
    private ICharacterInventorySystem _inventory;

    private void Awake()
    {
        SetTargetActive(false);

        if (_gameUI != null)
            _gameUI.OnInitialized += Initialize;
    }

    private void OnDestroy()
    {
        if (_gameUI != null)
            _gameUI.OnInitialized -= Initialize;

        if (_gameUI?.GameSceneContext?.Player != null)
            _gameUI.GameSceneContext.Player.OnCharacterUpdated -= OnCharacterUpdated;
    }

    private void Update()
    {
        SetTargetActive(HasPickupNearby());
    }

    private void Initialize()
    {
        var player = _gameUI.GameSceneContext.Player;
        player.OnCharacterUpdated += OnCharacterUpdated;
        OnCharacterUpdated(null, player.Character);
    }

    private void OnCharacterUpdated(Character oldCharacter, Character currentCharacter)
    {
        _currentCharacter = currentCharacter;
        _inventory = _currentCharacter?.GetSystem<ICharacterInventorySystem>();
    }

    private bool HasPickupNearby()
    {
        if (_currentCharacter == null || _inventory == null)
            return false;

        var colliders = Physics.OverlapSphere(_currentCharacter.transform.position, _inventory.PickupRange);
        foreach (var collider in colliders)
        {
            if (collider.GetComponentInParent<ItemPickupBase>() != null)
                return true;
        }

        return false;
    }

    private void SetTargetActive(bool active)
    {
        if (_target != null && _target.activeSelf != active)
            _target.SetActive(active);
    }
}
