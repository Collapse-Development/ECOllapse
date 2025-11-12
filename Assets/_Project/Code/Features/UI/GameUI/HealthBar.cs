using UnityEngine;
using UnityEngine.UI;
using _Project.Code.Features.Character.MB;
using CharacterSystems;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameUI _gameUI;
    [SerializeField] private Image _filler;

    void Awake()
    {
        _gameUI.OnInitialized += Initialize;
    }

    void OnDestroy()
    {
        if (_gameUI) _gameUI.OnInitialized -= Initialize;
    }

    private void Initialize()
    {
        _gameUI.GameSceneContext.Player.OnCharacterUpdated += OnCharacterUpdated;
        OnCharacterUpdated(null, _gameUI.GameSceneContext.Player.Character);
    }

    private void OnCharacterUpdated(Character oldCharacter, Character currentCharacter)
    {
        if (oldCharacter != null)
        {
            oldCharacter.GetSystem<ICharacterHealthSystem>()!.OnHealthChanged -= OnHealthChanged;
        }

        currentCharacter!.GetSystem<ICharacterHealthSystem>()!.OnHealthChanged += OnHealthChanged;
    }

    private void OnHealthChanged(float cur, float max) => SetRatio(cur, max);

    private void SetRatio(float cur, float max) => _filler.fillAmount = (max > 0f) ? Mathf.Clamp01(cur / max) : 0f;
}