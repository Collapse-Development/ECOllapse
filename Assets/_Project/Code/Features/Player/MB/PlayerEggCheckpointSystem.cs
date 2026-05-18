using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Player.MB;
using UnityEngine;
using Object = UnityEngine.Object;

public class PlayerEggCheckpointSystem
{
    private readonly Dictionary<Character, GameObject> _eggsByCharacter = new();

    private GameSceneContext _gameSceneContext;
    private CharacterConfig _characterConfig;
    private GameObject _eggPrefab;
    private List<CharacterMutation> _availableMutations = new();
    private PendingRespawn _pendingRespawn;

    public event Action<Character> OnEggStateChanged;

    public void Initialize(
        GameSceneContext gameSceneContext,
        CharacterConfig characterConfig,
        GameObject eggPrefab,
        IEnumerable<CharacterMutation> availableMutations)
    {
        _gameSceneContext = gameSceneContext;
        _characterConfig = characterConfig;
        _eggPrefab = eggPrefab;
        _availableMutations = availableMutations?
            .Where(mutation => mutation != null)
            .ToList() ?? new List<CharacterMutation>();
    }

    public bool CanLayEgg(Character character)
    {
        return character != null &&
               _eggPrefab != null &&
               !_eggsByCharacter.ContainsKey(character);
    }

    public bool TryLayEgg(Character character)
    {
        if (!CanLayEgg(character))
            return false;

        var egg = Object.Instantiate(
            _eggPrefab,
            character.transform.position,
            character.transform.rotation);

        _eggsByCharacter.Add(character, egg);
        OnEggStateChanged?.Invoke(character);
        return true;
    }

    public bool TryStartRespawnFromEgg(Character deadCharacter)
    {
        if (deadCharacter == null ||
            !_eggsByCharacter.TryGetValue(deadCharacter, out var egg) ||
            egg == null ||
            _gameSceneContext?.Player == null ||
            _characterConfig == null)
        {
            return false;
        }

        _eggsByCharacter.Remove(deadCharacter);
        deadCharacter.gameObject.SetActive(false);

        _pendingRespawn = new PendingRespawn(deadCharacter, egg);

        var mutationWindow = _gameSceneContext.MutationSelectionWindow;
        if (mutationWindow != null && _availableMutations.Count > 0)
        {
            mutationWindow.Show(_availableMutations, CompletePendingRespawn);
            return true;
        }

        CompletePendingRespawn(null);
        return true;
    }

    private void CompletePendingRespawn(CharacterMutation selectedMutation)
    {
        if (_pendingRespawn == null)
            return;

        var deadCharacter = _pendingRespawn.DeadCharacter;
        var egg = _pendingRespawn.Egg;
        _pendingRespawn = null;

        if (egg == null || _gameSceneContext?.Player == null || _characterConfig == null)
            return;

        var spawnPosition = egg.transform.position;
        var spawnRotation = egg.transform.rotation;
        var buildConfig = _characterConfig.GetBuildConfig();
        selectedMutation?.Apply(buildConfig);

        var newCharacter = CharacterBuilder.Build(buildConfig);
        if (newCharacter == null)
            return;

        newCharacter.transform.SetPositionAndRotation(spawnPosition, spawnRotation);

        var player = _gameSceneContext.Player;
        player.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        player.Character = newCharacter;

        Object.Destroy(egg);
        if (deadCharacter != null)
            Object.Destroy(deadCharacter.gameObject);

        OnEggStateChanged?.Invoke(deadCharacter);
        OnEggStateChanged?.Invoke(newCharacter);
    }

    private sealed class PendingRespawn
    {
        public PendingRespawn(Character deadCharacter, GameObject egg)
        {
            DeadCharacter = deadCharacter;
            Egg = egg;
        }

        public Character DeadCharacter { get; }
        public GameObject Egg { get; }
    }
}
