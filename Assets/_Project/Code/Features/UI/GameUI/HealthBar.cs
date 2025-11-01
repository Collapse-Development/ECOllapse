using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using _Project.Code.Features.Player.MB;       // Player
using _Project.Code.Features.Character.MB;   // Character
using CharacterSystems;                      // ICharacterHealthSystem, CharacterHealthSystem (конкретный класс)

[RequireComponent(typeof(CanvasGroup))]
public class CharacterHealthHUD : MonoBehaviour
{
    [Header("Источник через GameUI")]
    [SerializeField] private GameUI _gameUI;                  // можно не задавать — найдётся
    [SerializeField] private bool _autoFindGameUI = true;

    [Header("Отладка/инфо")]
    [SerializeField] private Player _player;                  // текущий Player
    [SerializeField] private Character _character;            // текущий Character

    [Header("Виджеты HUD")]
    [SerializeField] private Slider _slider;                  // опционально (0..1)
    [SerializeField] private Image _fillImage;                // опционально (Image type = Filled)
    [SerializeField] private Gradient _colorByHealth;         // опционально (0=красн, 1=зел)
    [SerializeField] private Text _uiText;                    // опционально
    // [SerializeField] private TMPro.TMP_Text _tmpText;      // если используешь TMP — раскомментируй

    [Header("Поведение")]
    [SerializeField, Min(0f)] private float _smoothSpeed = 10f;
    [SerializeField] private bool _autoHide = false;
    [SerializeField, Min(0f)] private float _hideDelay = 2f;
    [SerializeField, Range(0f,1f)] private float _hiddenAlpha = 0f;
    [SerializeField] private string _textFormat = "{0}/{1}";  // {0}=cur, {1}=max, {2}=ratio (0..1)

    private CanvasGroup _group;
    private Coroutine _hideRoutine;
    private ICharacterHealthSystem _healthSystem;

    private float _displayed01 = 1f;
    private float _target01 = 1f;

    private void Reset()
    {
        _slider = GetComponentInChildren<Slider>();
        if (_slider != null && _slider.fillRect != null)
            _fillImage = _slider.fillRect.GetComponent<Image>();

        if (_gameUI == null)
            _gameUI = FindObjectOfType<GameUI>();
    }

    private void Awake()
    {
        _group = GetComponent<CanvasGroup>();

        if (_gameUI == null && _autoFindGameUI)
            _gameUI = FindObjectOfType<GameUI>();

        if (_gameUI != null)
            _gameUI.OnInitialized += HandleGameUIInitialized;

        // Если GameUI уже инициализирован
        if (_gameUI != null && _gameUI.GameSceneContext != null)
            ResolveFromGameUI();
        else
            Debug.Log("[CharacterHealthHUD] Жду GameUI.Initialize(...)");
    }

    private void OnDestroy()
    {
        if (_gameUI != null)
            _gameUI.OnInitialized -= HandleGameUIInitialized;

        UnhookPlayer();
        UnhookHealthSystem();
    }

    private void Update()
    {
        // экспоненциальный lerp
        _displayed01 = Mathf.Lerp(_displayed01, _target01, 1f - Mathf.Exp(-_smoothSpeed * Time.deltaTime));
        ApplyVisuals();
    }

    // Позволяет задать GameUI из кода при необходимости
    public void SetGameUI(GameUI gui)
    {
        if (_gameUI == gui) return;

        if (_gameUI != null)
            _gameUI.OnInitialized -= HandleGameUIInitialized;

        _gameUI = gui;

        if (_gameUI != null)
        {
            _gameUI.OnInitialized += HandleGameUIInitialized;
            if (_gameUI.GameSceneContext != null)
                ResolveFromGameUI();
        }
    }

    private void HandleGameUIInitialized()
    {
        ResolveFromGameUI();
    }

    private void ResolveFromGameUI()
    {
        var ctx = _gameUI.GameSceneContext;
        if (ctx == null)
        {
            Debug.LogWarning("[CharacterHealthHUD] GameSceneContext == null");
            return;
        }

        HookPlayer(ctx.Player);
    }

    private void HookPlayer(Player player)
    {
        if (_player == player) return;

        UnhookPlayer();

        _player = player;
        if (_player == null)
        {
            Debug.LogWarning("[CharacterHealthHUD] Player == null в GameSceneContext");
            HookCharacter(null);
            return;
        }

        _player.OnCharacterUpdated += OnPlayerCharacterUpdated;
        HookCharacter(_player.Character); // моментально привязываемся к текущему персонажу
    }

    private void UnhookPlayer()
    {
        if (_player != null)
            _player.OnCharacterUpdated -= OnPlayerCharacterUpdated;
        _player = null;
    }

    private void OnPlayerCharacterUpdated(Character oldChar, Character newChar)
    {
        HookCharacter(newChar);
    }

    private void HookCharacter(Character character)
    {
        _character = character;

        // найти систему здоровья через Character.GetSystem<ICharacterHealthSystem>()
        ICharacterHealthSystem hs = null;
        if (_character != null)
            hs = _character.GetSystem<ICharacterHealthSystem>();

        HookHealthSystem(hs);
    }

    private void HookHealthSystem(ICharacterHealthSystem hs)
    {
        if (_healthSystem == hs) return;

        UnhookHealthSystem();
        _healthSystem = hs;

        if (_healthSystem == null)
        {
            Debug.LogWarning("[CharacterHealthHUD] ICharacterHealthSystem не найден на Character.");
            SetSnapshot(1f, 1f); // сбросить HUD в «полное» состояние
            return;
        }

        _healthSystem.OnHealthChanged += OnHealthChanged;
        _healthSystem.OnDeath += OnDeath;

        // Снимок текущего состояния (если интерфейс не даёт прямого доступа — пытаемся через конкретный класс)
        float cur = 1f, max = 1f;
        if (_healthSystem is CharacterHealthSystem concrete)
        {
            cur = concrete.CurrentHealth;
            max = concrete.MaxHealth;
        }
        // если интерфейс содержит свойства — раскомментируй:
        // else
        // {
        //     cur = _healthSystem.CurrentHealth;
        //     max = _healthSystem.MaxHealth;
        // }

        SetSnapshot(cur, max);
    }

    private void UnhookHealthSystem()
    {
        if (_healthSystem != null)
        {
            _healthSystem.OnHealthChanged -= OnHealthChanged;
            _healthSystem.OnDeath -= OnDeath;
        }
        _healthSystem = null;
    }

    private void SetSnapshot(float current, float max)
    {
        _target01 = _displayed01 = SafeRatio(current, max);
        ApplyVisuals(force: true);
        UpdateText(current, max);
    }

    private void OnHealthChanged(float current, float max)
    {
        _target01 = SafeRatio(current, max);
        UpdateText(current, max);

        if (_autoHide)
        {
            Show();
            if (_hideRoutine != null) StopCoroutine(_hideRoutine);
            _hideRoutine = StartCoroutine(HideLater());
        }
    }

    private void OnDeath()
    {
        _target01 = 0f;
        UpdateText(0f, (_healthSystem is CharacterHealthSystem hs) ? hs.MaxHealth : 0f);

        if (_autoHide)
        {
            Show();
            if (_hideRoutine != null) StopCoroutine(_hideRoutine);
            _hideRoutine = StartCoroutine(HideLater());
        }
    }

    private IEnumerator HideLater()
    {
        yield return new WaitForSeconds(_hideDelay);
        Hide();
    }

    private void ApplyVisuals(bool force = false)
    {
        if (_slider != null)
        {
            if (force) { _slider.minValue = 0f; _slider.maxValue = 1f; }
            _slider.value = _displayed01;
        }

        if (_fillImage != null)
        {
            if (_fillImage.type == Image.Type.Filled)
                _fillImage.fillAmount = _displayed01;

            if (_colorByHealth != null)
                _fillImage.color = _colorByHealth.Evaluate(_displayed01);
        }
    }

    private void UpdateText(float current, float max)
    {
        if (_uiText == null /* && _tmpText == null */) return;

        float ratio = SafeRatio(current, max);
        string s = _textFormat
            .Replace("{0}", Mathf.RoundToInt(current).ToString())
            .Replace("{1}", Mathf.RoundToInt(max).ToString())
            .Replace("{2}", ratio.ToString()); // для процентов: _textFormat = "{2:P0}"

        _uiText.text = s;
        // if (_tmpText != null) _tmpText.text = s;
    }

    private static float SafeRatio(float current, float max)
        => (max > 0f) ? Mathf.Clamp01(current / max) : 0f;

    public void Show() => _group.alpha = 1f;
    public void Hide() => _group.alpha = _hiddenAlpha;
}
