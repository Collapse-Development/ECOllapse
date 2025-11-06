using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using _Project.Code.Features.Player.MB;       // Player
using _Project.Code.Features.Character.MB;   // Character
using CharacterSystems;                      // ICharacterHealthSystem, CharacterHealthSystem (конкретный класс)

[RequireComponent(typeof(CanvasGroup))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] GameUI gameUI;
    [SerializeField] Image fill;

    ICharacterHealthSystem hs;
    Character lastChar;

    void Start()
    {
        //if (gameUI) gameUI.OnInitialized += TryBind;
        TryBind();
    }

    void OnDestroy()
    {
        if (gameUI) gameUI.OnInitialized -= TryBind;
        Unhook();
    }

    void Update()
    {
        // если ещё не привязались или персонаж у Player сменился — привязываемся
        if (hs == null || gameUI?.GameSceneContext?.Player?.Character != lastChar)
            TryBind();
    }

    void TryBind()
    {
        var ch = gameUI?.GameSceneContext?.Player?.Character;
        if (ch == lastChar && hs != null) return;

        Unhook();
        lastChar = ch;
        hs = ch?.GetSystem<ICharacterHealthSystem>();

        if (hs == null) { fill.fillAmount = 1f; return; }

        hs.OnHealthChanged += OnHealthChanged;
        hs.OnDeath += OnDeath;
    }

    void Unhook()
    {
        if (hs != null)
        {
            hs.OnHealthChanged -= OnHealthChanged;
            hs.OnDeath -= OnDeath;
            hs = null;
        }
    }

    void OnHealthChanged(float cur, float max) => fill.fillAmount = (max > 0f) ? cur / max : 0f;
    void OnDeath() => fill.fillAmount = 0f;
}