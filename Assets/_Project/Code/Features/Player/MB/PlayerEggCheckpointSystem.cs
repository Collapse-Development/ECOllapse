using System;
using System.Collections.Generic;
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

    public event Action<Character> OnEggStateChanged;

    public void Initialize(
        GameSceneContext gameSceneContext,
        CharacterConfig characterConfig,
        GameObject eggPrefab)
    {
        _gameSceneContext = gameSceneContext;
        _characterConfig = characterConfig;
        _eggPrefab = eggPrefab;
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

    public bool TryRespawnFromEgg(Character deadCharacter)
    {
        if (deadCharacter == null ||
            !_eggsByCharacter.TryGetValue(deadCharacter, out var egg) ||
            egg == null ||
            _gameSceneContext?.Player == null ||
            _characterConfig == null)
        {
            return false;
        }

        var spawnPosition = egg.transform.position;
        var spawnRotation = egg.transform.rotation;

        _eggsByCharacter.Remove(deadCharacter);

        deadCharacter.gameObject.SetActive(false);

        var newCharacter = CharacterBuilder.Build(_characterConfig.GetBuildConfig());
        if (newCharacter == null)
            return false;

        newCharacter.transform.SetPositionAndRotation(spawnPosition, spawnRotation);

        var player = _gameSceneContext.Player;
        player.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        player.Character = newCharacter;

        Object.Destroy(egg);
        Object.Destroy(deadCharacter.gameObject);

        OnEggStateChanged?.Invoke(deadCharacter);
        OnEggStateChanged?.Invoke(newCharacter);
        return true;
    }
}
