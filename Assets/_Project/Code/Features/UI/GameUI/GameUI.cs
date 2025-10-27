using System;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public GameSceneContext GameSceneContext { get; private set; }

    public event Action OnInitialized;
    
    public void Initialize(GameSceneContext gameSceneContext)
    {
        GameSceneContext = gameSceneContext;
        
        OnInitialized?.Invoke();
    }
}
