using UnityEngine;

public class GameUI : MonoBehaviour
{
    public GameSceneContext GameSceneContext { get; private set; }

    public void Initialize(GameSceneContext gameSceneContext)
    {
        GameSceneContext = gameSceneContext;
    }
}
