using System;
using UnityEngine;
using _Project.Code.Features.Character.Configurations.Systems;
using _Project.Code.Features.Character.MB;

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
