using UnityEngine;
using UnityEngine.UI;
using _Project.Code.Features.Character.MB;

// Интерфейс для отображения уровня гидратации игрока
// Поведение аналогично HealthBar
public class HydrationBar : MonoBehaviour
{
    [Header("Ссылки на UI")]
    [SerializeField] private GameUI _gameUI; // Ссылка на общий UI-сцену
    [SerializeField] private Image _filler;  // Image полосы гидратации (fillAmount)

    private ICharacterHydrationSystem _hydrationSystem; // Система гидратации персонажа

    private void Awake()
    {
        // Подписываемся на событие инициализации UI
        // После этого получаем игрока и его систему
        if (_gameUI != null)
            _gameUI.OnInitialized += Initialize;
    }

    private void OnDestroy()
    {
        // Отписка от события, чтобы не было утечек памяти
        if (_gameUI != null)
            _gameUI.OnInitialized -= Initialize;

        if (_hydrationSystem != null)
            _hydrationSystemChanged(false); // очистка подписки
    }

    // Метод вызывается, когда UI готов
    private void Initialize()
    {
        var player = _gameUI.GameSceneContext.Player;

        // Подписываемся на событие смены персонажа
        player.OnCharacterUpdated += OnCharacterUpdated;

        // Инициализируем сразу текущего персонажа
        OnCharacterUpdated(null, player.Character);
    }

    // Вызывается при смене персонажа
    private void OnCharacterUpdated(Character oldCharacter, Character currentCharacter)
    {
        // Если был старый персонаж, отписываемся от старой системы
        if (oldCharacter != null)
        {
            var oldSystem = oldCharacter.GetSystem<ICharacterHydrationSystem>();
            if (oldSystem != null)
                oldSystemChanged(oldSystem, false);
        }

        // Получаем новую систему гидратации
        if (currentCharacter != null)
        {
            _hydrationSystem = currentCharacter.GetSystem<ICharacterHydrationSystem>();
            if (_hydrationSystem != null)
                oldSystemChanged(_hydrationSystem, true);
        }

        // Обновляем визуально сразу
        if (_hydrationSystem != null)
            UpdateUI(_hydrationSystem.CurrentHydration, _hydrationSystem.MaxHydration);
    }

    // Подписка/отписка на событие изменения гидратации
    private void oldSystemChanged(ICharacterHydrationSystem system, bool subscribe)
    {
        if (subscribe)
        {
            // Если система поддерживает события, можно сделать OnHydrationChanged
            // Для простоты мы будем обновлять UI в Update через CurrentHydration
        }
        else
        {
            // Очистка подписки при смене персонажа
        }
    }

    private void Update()
    {
        if (_hydrationSystem != null)
        {
            UpdateUI(_hydrationSystem.CurrentHydration, _hydrationSystem.MaxHydration);
        }
    }

    // Обновление fillAmount полосы
    private void UpdateUI(float cur, float max)
    {
        if (_filler != null)
            _filler.fillAmount = (max > 0f) ? Mathf.Clamp01(cur / max) : 0f;
    }
}