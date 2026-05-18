using _Project.Code.Features.Character.MB;
using UnityEngine;

public class EggTargetIndicator : MonoBehaviour
{
    [SerializeField] private GameUI _gameUI;
    [SerializeField] private GameObject _target;

    private Character _currentCharacter;
    private PlayerEggCheckpointSystem _checkpointSystem;

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

        if (_checkpointSystem != null)
            _checkpointSystem.OnEggStateChanged -= OnEggStateChanged;
    }

    private void Update()
    {
        SetTargetActive(_checkpointSystem != null && _checkpointSystem.CanLayEgg(_currentCharacter));
    }

    private void Initialize()
    {
        var gameSceneContext = _gameUI.GameSceneContext;
        _checkpointSystem = gameSceneContext.PlayerEggCheckpointSystem;

        if (_checkpointSystem != null)
            _checkpointSystem.OnEggStateChanged += OnEggStateChanged;

        var player = gameSceneContext.Player;
        player.OnCharacterUpdated += OnCharacterUpdated;
        OnCharacterUpdated(null, player.Character);
    }

    private void OnCharacterUpdated(Character oldCharacter, Character currentCharacter)
    {
        _currentCharacter = currentCharacter;
        SetTargetActive(_checkpointSystem != null && _checkpointSystem.CanLayEgg(_currentCharacter));
    }

    private void OnEggStateChanged(Character character)
    {
        if (character == _currentCharacter)
            SetTargetActive(_checkpointSystem != null && _checkpointSystem.CanLayEgg(_currentCharacter));
    }

    private void SetTargetActive(bool active)
    {
        if (_target != null && _target.activeSelf != active)
            _target.SetActive(active);
    }
}
