using System.Linq;
using System.Text;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.MB.InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    private const string PanelName = "InventoryPanel";
    private const string TextName = "InventoryText";

    [Header("Layout")]
    [SerializeField] private TMP_FontAsset _font;
    [SerializeField] private int _fontSize = 24;
    [SerializeField] private Vector2 _panelSize = new(420f, 300f);
    [SerializeField] private Vector2 _panelOffset = new(-32f, 32f);

    [Header("Visibility")]
    [SerializeField] private bool _showOnStart;
    [SerializeField] private Key _toggleKey = Key.F4;

    private readonly StringBuilder _builder = new();

    private GameUI _gameUI;
    private Character _currentCharacter;
    private ICharacterInventorySystem _inventory;
    private RectTransform _panel;
    private TextMeshProUGUI _text;
    private bool _isVisible;
    private bool _isDirty = true;

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
        UnsubscribeInventory();

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

        if (!_isVisible || _text == null || !_isDirty)
        {
            return;
        }

        _text.text = BuildInventoryText();
        _isDirty = false;
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
        UnsubscribeInventory();

        _currentCharacter = currentCharacter;
        _inventory = _currentCharacter?.GetSystem<ICharacterInventorySystem>();

        SubscribeInventory();
        MarkDirty();
    }

    private void SubscribeInventory()
    {
        if (_inventory == null)
        {
            return;
        }

        _inventory.OnItemAdded += OnInventoryItemChanged;
        _inventory.OnItemRemoved += OnInventoryItemChanged;
        _inventory.OnWeightChanged += OnInventoryWeightChanged;
        _inventory.OnActiveSlotChanged += OnInventoryActiveSlotChanged;
    }

    private void UnsubscribeInventory()
    {
        if (_inventory == null)
        {
            return;
        }

        _inventory.OnItemAdded -= OnInventoryItemChanged;
        _inventory.OnItemRemoved -= OnInventoryItemChanged;
        _inventory.OnWeightChanged -= OnInventoryWeightChanged;
        _inventory.OnActiveSlotChanged -= OnInventoryActiveSlotChanged;
    }

    private void OnInventoryItemChanged(string itemId, int count) => MarkDirty();
    private void OnInventoryWeightChanged(float currentWeight) => MarkDirty();
    private void OnInventoryActiveSlotChanged(int slotIndex, string itemId) => MarkDirty();

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
        panel.anchorMin = new Vector2(1f, 0f);
        panel.anchorMax = new Vector2(1f, 0f);
        panel.pivot = new Vector2(1f, 0f);
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

        if (isVisible)
        {
            MarkDirty();
        }
    }

    private void MarkDirty()
    {
        _isDirty = true;
    }

    private string BuildInventoryText()
    {
        _builder.Clear();
        _builder.Append("Inventory");

        if (_toggleKey != Key.None)
        {
            _builder.Append(" [");
            _builder.Append(_toggleKey);
            _builder.Append(']');
        }

        _builder.AppendLine();

        if (_inventory == null)
        {
            _builder.AppendLine("inactive");
            return _builder.ToString();
        }

        _builder.Append("Weight: ");
        _builder.Append(FormatNumber(_inventory.CurrentWeight));
        _builder.Append(" / ");
        _builder.Append(FormatNumber(_inventory.MaxWeight));
        _builder.Append(" (");
        _builder.Append(FormatNumber(_inventory.WeightRatio * 100f));
        _builder.AppendLine("%)");

        _builder.Append("Pickup Range: ");
        _builder.AppendLine(FormatNumber(_inventory.PickupRange));

        var activeItemId = _inventory.ActiveItemId;
        _builder.Append("Active: ");
        _builder.AppendLine(activeItemId != null ? FormatItemName(activeItemId) : "none");

        _builder.AppendLine("Items:");

        if (_inventory.Stacks.Count == 0)
        {
            _builder.AppendLine("empty");
            return _builder.ToString();
        }

        for (var i = 0; i < _inventory.Stacks.Count; i++)
        {
            AppendSlotLine(i, _inventory.Stacks[i]);
        }

        return _builder.ToString();
    }

    private void AppendSlotLine(int slotIndex, InventoryStack stack)
    {
        var isActive = slotIndex == _inventory.ActiveSlotIndex;

        _builder.Append(isActive ? "> " : "  ");
        _builder.Append(slotIndex + 1);
        _builder.Append(". ");
        _builder.Append(FormatItemName(stack.ItemId));
        _builder.Append(" x");
        _builder.AppendLine(stack.Count.ToString());
    }

    private string FormatItemName(string itemId)
    {
        var itemConfig = _inventory?.GetItemConfig(itemId);
        if (itemConfig != null)
        {
            return itemConfig.DisplayName;
        }

        return string.IsNullOrWhiteSpace(itemId) ? "unknown" : itemId;
    }

    private string FormatNumber(float value) => value.ToString("0.##");
}

internal static class InventoryDisplayBootstrap
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

            if (gameUi.GetComponent<InventoryDisplay>() == null)
            {
                gameUi.gameObject.AddComponent<InventoryDisplay>();
            }

            return;
        }
    }
}
