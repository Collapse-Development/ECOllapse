using System;
using _Project.Code.Features.Player.MB;
using CharacterSystems;
using UnityEngine;
using Character = _Project.Code.Features.Character.MB.Character;

public class GameLoop : MonoBehaviour
{
    private Player _player;
    private Character _currentCharacter;
    private ICharacterHealthSystem _healthSystem;

    public GameSceneContext GameSceneContext { get; private set; }
    public PlayerState PlayerState { get; private set; } = PlayerState.Unknown;

    public event Action<PlayerState> OnPlayerStateChanged;

    public void Initialize(GameSceneContext gameSceneContext)
    {
        CleanupSubscriptions();

        GameSceneContext = gameSceneContext;
        _player = GameSceneContext?.Player;

        if (_player == null)
        {
            SetPlayerState(PlayerState.Unknown);
            return;
        }

        _player.OnCharacterUpdated += OnCharacterUpdated;
        OnCharacterUpdated(null, _player.Character);
    }

    private void OnDestroy()
    {
        CleanupSubscriptions();
    }

    private void OnCharacterUpdated(
        Character oldCharacter,
        Character currentCharacter)
    {
        UnsubscribeFromCurrentCharacter();

        _currentCharacter = currentCharacter;
        _healthSystem = _currentCharacter?.GetSystem<ICharacterHealthSystem>();

        if (_healthSystem == null)
        {
            SetPlayerState(PlayerState.Unknown);
            return;
        }

        _healthSystem.OnDeath += OnPlayerDeath;
        SetPlayerState(_healthSystem.CurrentHealth > 0f ? PlayerState.Alive : PlayerState.Dead);
    }

    private void OnPlayerDeath()
    {
        if (GameSceneContext?.PlayerEggCheckpointSystem != null &&
            GameSceneContext.PlayerEggCheckpointSystem.TryRespawnFromEgg(_currentCharacter))
        {
            return;
        }

        SetPlayerState(PlayerState.Dead);
    }

    private void SetPlayerState(PlayerState state)
    {
        if (PlayerState == state) return;

        PlayerState = state;
        OnPlayerStateChanged?.Invoke(PlayerState);
    }

    private void CleanupSubscriptions()
    {
        if (_player != null)
            _player.OnCharacterUpdated -= OnCharacterUpdated;

        UnsubscribeFromCurrentCharacter();
        _player = null;
        GameSceneContext = null;
        SetPlayerState(PlayerState.Unknown);
    }

    private void UnsubscribeFromCurrentCharacter()
    {
        if (_healthSystem != null)
            _healthSystem.OnDeath -= OnPlayerDeath;

        _healthSystem = null;
        _currentCharacter = null;
    }
}
