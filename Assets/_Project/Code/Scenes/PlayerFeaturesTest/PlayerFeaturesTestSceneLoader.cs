using System.Collections;
using _Project.Code.Features.Player.MB;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerFeaturesTestSceneLoader : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private CharacterConfig _playerConfig;
    
    private IEnumerator Start()
    {
        var asyncLoadGameUI = SceneManager.LoadSceneAsync("GameUI", LoadSceneMode.Additive);
            
        while (asyncLoadGameUI is { isDone: false })
            yield return null;
        
        var playerCharacter = CharacterBuilder.Build(_playerConfig.GetBuildConfig());
        _player.Character = playerCharacter;

        var gameSceneContext = new GameSceneContext
        {
            Player = _player
        };

        var gameUI = FindAnyObjectByType<GameUI>();
        gameUI.Initialize(gameSceneContext);

        yield return null;
    }
}
