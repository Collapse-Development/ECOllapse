using UnityEngine;
using UnityEngine.UI;
using _Project.Code.Features.Player.MB;       // Player
using _Project.Code.Features.Character.MB;   // Character
using CharacterSystems;                      // ICharacterHealthSystem, CharacterHealthSystem
public class HealthBar : MonoBehaviour
{
    [SerializeField] GameUI gameUI;
    [SerializeField] Image fill;

    Player player;
    ICharacterHealthSystem hs;
    Character lastChar;

    void Start()
    {
        gameUI.OnInitialized += OnGameUIInitialized;
    }

    void OnDestroy()
    {
        if (gameUI) gameUI.OnInitialized -= OnGameUIInitialized;
        UnhookPlayer();
        UnhookHealth();
    }

    // ----- Инициализация от GameUI / Player -----

    void OnGameUIInitialized()
    {
        HookPlayer(gameUI.GameSceneContext?.Player);
    }

    void HookPlayer(Player p)
    {
        if (player == p) return;

        UnhookPlayer();
        player = p;

        if (player != null)
        {
            player.OnCharacterUpdated += OnPlayerCharacterUpdated;
            OnPlayerCharacterUpdated(null, player.Character); // привязываемся к текущему
        }
        else
        {
            SetFull();
        }
    }

    void UnhookPlayer()
    {
        if (player != null)
        {
            player.OnCharacterUpdated -= OnPlayerCharacterUpdated;
            player = null;
        }
    }

    void OnPlayerCharacterUpdated(Character oldChar, Character newChar)
    {
        if (newChar == lastChar) return;

        UnhookHealth();
        lastChar = newChar;

        hs = newChar?.GetSystem<ICharacterHealthSystem>();
        if (hs == null) { SetFull(); return; }

        hs.OnHealthChanged += OnHealthChanged;
        hs.OnDeath += OnDeath;
    }

    // ----- Подписка на здоровье -----

    void UnhookHealth()
    {
        if (hs != null)
        {
            hs.OnHealthChanged -= OnHealthChanged;
            hs.OnDeath -= OnDeath;
            hs = null;
        }
    }

    void OnHealthChanged(float cur, float max) => SetRatio(cur, max);
    void OnDeath() => fill.fillAmount = 0f;

    // ----- Утилиты -----

    void SetRatio(float cur, float max) => fill.fillAmount = (max > 0f) ? Mathf.Clamp01(cur / max) : 0f;
    void SetFull() => fill.fillAmount = 1f;
}