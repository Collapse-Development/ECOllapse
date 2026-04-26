using System;
using System.Linq;
using System.Text;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.MB.AttackSystem;
using _Project.Code.Features.Character.MB.EffectsSystem;
using _Project.Code.Features.Character.MB.EnduranceSystem;
using _Project.Code.Features.Character.MB.MovementSystem;
using _Project.Code.Features.Character.MB.NeedsSystem.Satiety;
using _Project.Code.Features.Character.MB.Thirst;
using _Project.Code.Features.Character.MB.Vigor;
using CharacterSystems;
using Project.Code.Features.Character.MB.HitProcessingSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSystemsDisplay : MonoBehaviour
{
    private readonly struct KnownSystemInfo
    {
        public KnownSystemInfo(string label, Type systemType)
        {
            Label = label;
            SystemType = systemType;
        }

        public string Label { get; }
        public Type SystemType { get; }
    }

    // Mirrors types declared under Assets/_Project/Code/Features/Character/Configurations/Systems.
    private static readonly KnownSystemInfo[] KnownSystems =
    {
        new("Health", typeof(ICharacterHealthSystem)),
        new("Health Regen", typeof(ICharacterHealthRegenSystem)),
        new("Endurance", typeof(ICharacterEnduranceSystem)),
        new("Satiety", typeof(ICharacterSatietySystem)),
        new("Thirst", typeof(ICharacterThirstSystem)),
        new("Vigor", typeof(ICharacterVigorSystem)),
        new("Movement", typeof(ICharacterMovementSystem)),
        new("Attack", typeof(IAttackSystem)),
        new("Damage Resistance", typeof(ICharacterDamageResistanceSystem)),
        new("Effects", typeof(ICharacterEffectsSystem)),
        new("Hit Processing", typeof(ICharacterHitProcessingSystem)),
        new("Model", typeof(ICharacterModelSystem))
    };

    private const string PanelName = "CharacterSystemsPanel";
    private const string TextName = "CharacterSystemsText";

    [Header("Layout")]
    [SerializeField] private TMP_FontAsset _font;
    [SerializeField] private int _fontSize = 24;
    [SerializeField] private Vector2 _panelSize = new(420f, 420f);
    [SerializeField] private Vector2 _panelOffset = new(-32f, -32f);

    [Header("Visibility")]
    [SerializeField] private bool _showOnStart = true;
    [SerializeField] private Key _toggleKey = Key.F3;

    private readonly StringBuilder _builder = new();

    private GameUI _gameUI;
    private Character _currentCharacter;
    private ICharacterSystem[] _systems = Array.Empty<ICharacterSystem>();
    private RectTransform _panel;
    private TextMeshProUGUI _text;
    private bool _isVisible;

    private void Awake()
    {
        _gameUI = GetComponent<GameUI>();
        EnsurePanel();

        if (_gameUI == null)
        {
            return;
        }

        _gameUI.OnInitialized += OnGameUiInitialized;

        if (_gameUI.GameSceneContext != null)
        {
            OnGameUiInitialized();
        }
    }

    private void OnDestroy()
    {
        if (_gameUI != null)
        {
            _gameUI.OnInitialized -= OnGameUiInitialized;

            if (_gameUI.GameSceneContext?.Player != null)
            {
                _gameUI.GameSceneContext.Player.OnCharacterUpdated -= OnCharacterUpdated;
            }
        }
    }

    private void Update()
    {
        HandleVisibilityToggle();

        if (!_isVisible || _text == null)
        {
            return;
        }

        _text.text = BuildSystemsText();
    }

    private void OnGameUiInitialized()
    {
        var player = _gameUI.GameSceneContext?.Player;
        if (player == null)
        {
            return;
        }

        player.OnCharacterUpdated -= OnCharacterUpdated;
        player.OnCharacterUpdated += OnCharacterUpdated;
        OnCharacterUpdated(_currentCharacter, player.Character);
    }

    private void OnCharacterUpdated(Character oldCharacter, Character currentCharacter)
    {
        _currentCharacter = currentCharacter;
        _systems = currentCharacter == null
            ? Array.Empty<ICharacterSystem>()
            : currentCharacter.GetComponents<MonoBehaviour>().OfType<ICharacterSystem>().ToArray();
    }

    private void EnsurePanel()
    {
        if (_panel != null && _text != null)
        {
            return;
        }

        var parent = FindHudRoot();
        if (parent == null)
        {
            return;
        }

        _panel = parent.Find(PanelName) as RectTransform;
        if (_panel == null)
        {
            _panel = CreatePanel(parent);
        }

        var textTransform = _panel.Find(TextName) as RectTransform;
        if (textTransform == null)
        {
            textTransform = CreateTextTransform(_panel);
        }

        _text = textTransform.GetComponent<TextMeshProUGUI>();
        _text.font = ResolveFont();
        _text.fontSize = _fontSize;
        _text.color = Color.white;
        _text.alignment = TextAlignmentOptions.TopLeft;
        _text.textWrappingMode = TextWrappingModes.NoWrap;
        _text.overflowMode = TextOverflowModes.Overflow;
        _text.raycastTarget = false;

        SetVisible(_showOnStart);
    }

    private RectTransform FindHudRoot()
    {
        foreach (var rootObject in gameObject.scene.GetRootGameObjects())
        {
            var hud = rootObject.GetComponentsInChildren<RectTransform>(true)
                .FirstOrDefault(rectTransform => rectTransform.name == "HUD");

            if (hud != null)
            {
                return hud;
            }
        }

        foreach (var rootObject in gameObject.scene.GetRootGameObjects())
        {
            var canvas = rootObject.GetComponentInChildren<Canvas>(true);
            if (canvas != null)
            {
                return canvas.transform as RectTransform;
            }
        }

        return null;
    }

    private RectTransform CreatePanel(RectTransform parent)
    {
        var panelObject = new GameObject(PanelName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelObject.layer = parent.gameObject.layer;

        var panel = panelObject.GetComponent<RectTransform>();
        panel.SetParent(parent, false);
        panel.anchorMin = new Vector2(1f, 1f);
        panel.anchorMax = new Vector2(1f, 1f);
        panel.pivot = new Vector2(1f, 1f);
        panel.anchoredPosition = _panelOffset;
        panel.sizeDelta = _panelSize;

        var image = panelObject.GetComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.55f);
        image.raycastTarget = false;

        return panel;
    }

    private RectTransform CreateTextTransform(RectTransform parent)
    {
        var textObject = new GameObject(TextName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.layer = parent.gameObject.layer;

        var textTransform = textObject.GetComponent<RectTransform>();
        textTransform.SetParent(parent, false);
        textTransform.anchorMin = Vector2.zero;
        textTransform.anchorMax = Vector2.one;
        textTransform.offsetMin = new Vector2(16f, 16f);
        textTransform.offsetMax = new Vector2(-16f, -16f);

        return textTransform;
    }

    private TMP_FontAsset ResolveFont()
    {
        if (_font != null)
        {
            return _font;
        }

        return TMP_Settings.defaultFontAsset
               ?? Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
    }

    private void HandleVisibilityToggle()
    {
        if (_panel == null || _toggleKey == Key.None)
        {
            return;
        }

        var keyboard = Keyboard.current;
        if (keyboard == null || !keyboard[_toggleKey].wasPressedThisFrame)
        {
            return;
        }

        SetVisible(!_isVisible);
    }

    private void SetVisible(bool isVisible)
    {
        _isVisible = isVisible;

        if (_panel != null)
        {
            _panel.gameObject.SetActive(isVisible);
        }
    }

    private string BuildSystemsText()
    {
        _builder.Clear();
        _builder.Append("Systems");

        if (_toggleKey != Key.None)
        {
            _builder.Append(" [");
            _builder.Append(_toggleKey);
            _builder.Append(']');
        }

        _builder.AppendLine();

        foreach (var knownSystem in KnownSystems)
        {
            AppendKnownSystem(knownSystem);
        }

        AppendUnknownSystems();
        return _builder.ToString();
    }

    private void AppendKnownSystem(KnownSystemInfo knownSystem)
    {
        var system = _systems.FirstOrDefault(knownSystem.SystemType.IsInstanceOfType);
        if (system == null)
        {
            AppendLine(knownSystem.Label, "inactive");
            return;
        }

        AppendSystemState(system);
    }

    private void AppendUnknownSystems()
    {
        foreach (var system in _systems)
        {
            if (system == null)
            {
                continue;
            }

            if (KnownSystems.Any(knownSystem => knownSystem.SystemType.IsInstanceOfType(system)))
            {
                continue;
            }

            AppendLine(FormatSystemName(system.GetType().Name), "active");
        }
    }

    private void AppendSystemState(ICharacterSystem system)
    {
        switch (system)
        {
            case ICharacterHealthSystem healthSystem:
                AppendLine("Health", $"{FormatNumber(healthSystem.CurrentHealth)} / {FormatNumber(healthSystem.MaxHealth)}");
                return;
            case ICharacterHealthRegenSystem regenSystem:
                AppendLine("Health Regen",
                    $"{FormatNumber(regenSystem.CurrentRegenRate)}/s | {(regenSystem.IsRegenerating ? "active" : "idle")}");
                return;
            case ICharacterEnduranceSystem enduranceSystem:
                AppendLine("Endurance",
                    $"{FormatNumber(enduranceSystem.CurrentValue)} / {FormatNumber(enduranceSystem.MaxValue)}");
                return;
            case ICharacterSatietySystem satietySystem:
                AppendLine("Satiety", FormatNumber(satietySystem.Satiety));
                return;
            case ICharacterThirstSystem thirstSystem:
                AppendLine("Thirst",
                    $"{FormatNumber(thirstSystem.CurrentValue)} / {FormatNumber(thirstSystem.MaxValue)}");
                return;
            case ICharacterVigorSystem vigorSystem:
                AppendLine("Vigor",
                    $"{FormatNumber(vigorSystem.CurrentValue)} / {FormatNumber(vigorSystem.MaxValue)}");
                return;
            case ICharacterMovementSystem movementSystem:
                AppendLine("Movement",
                    $"{GetMovementState(movementSystem)} | speed {FormatNumber(movementSystem.Speed)}");
                return;
            case IAttackSystem attackSystem:
                AppendLine("Attack",
                    $"{FormatNumber(attackSystem.AttackDamage)} dmg | range {FormatNumber(attackSystem.AttackRange)} | {(attackSystem.IsAttacking ? "attacking" : "ready")}");
                return;
            case ICharacterDamageResistanceSystem resistanceSystem:
                AppendLine("Damage Resistance", FormatNumber(resistanceSystem.Resistance));
                return;
            case ICharacterEffectsSystem:
                AppendLine("Effects", "active");
                return;
            case ICharacterHitProcessingSystem:
                AppendLine("Hit Processing", "active");
                return;
            case ICharacterModelSystem modelSystem:
                AppendLine("Model", modelSystem.Model != null ? modelSystem.Model.name : "loading");
                return;
            default:
                AppendLine(FormatSystemName(system.GetType().Name), "active");
                return;
        }
    }

    private void AppendLine(string label, string value)
    {
        _builder.Append(label);
        _builder.Append(": ");
        _builder.AppendLine(value);
    }

    private string GetMovementState(ICharacterMovementSystem movementSystem)
    {
        if (movementSystem.IsRunning)
        {
            return "running";
        }

        if (movementSystem.IsMoving)
        {
            return "moving";
        }

        return "idle";
    }

    private string FormatNumber(float value) => value.ToString("0.##");

    private string FormatSystemName(string rawName)
    {
        var name = rawName
            .Replace("Character", string.Empty)
            .Replace("System", string.Empty)
            .Replace("Stub", string.Empty);

        if (string.IsNullOrWhiteSpace(name))
        {
            return rawName;
        }

        var builder = new StringBuilder(name.Length * 2);
        builder.Append(name[0]);

        for (var i = 1; i < name.Length; i++)
        {
            if (char.IsUpper(name[i]) && !char.IsWhiteSpace(name[i - 1]))
            {
                builder.Append(' ');
            }

            builder.Append(name[i]);
        }

        return builder.ToString();
    }
}

internal static class CharacterSystemsDisplayBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var rootObject in scene.GetRootGameObjects())
        {
            var gameUi = rootObject.GetComponentInChildren<GameUI>(true);
            if (gameUi == null)
            {
                continue;
            }

            if (gameUi.GetComponent<CharacterSystemsDisplay>() == null)
            {
                gameUi.gameObject.AddComponent<CharacterSystemsDisplay>();
            }

            return;
        }
    }
}
