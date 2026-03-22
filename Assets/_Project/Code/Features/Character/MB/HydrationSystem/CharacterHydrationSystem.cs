using UnityEngine;
using _Project.Code.Features.Character.MB;

// —истема гидратации персонажа
// ќтвечает за: уменьшение гидратации со временем, увеличение убывани€ при беге/атаке,
// нанесение урона при 0 гидратации и ведение статистики времени без пить€
public class CharacterHydrationSystem : MonoBehaviour, ICharacterHydrationSystem
{
    // =====================
    // Ќастройки (из таблицы баланса)
    // =====================
    public float MaxHydration { get; private set; } = 100f; // базова€ гидратаци€
    private float decreasePerMinute = 10f; // декремент гидратации в единицах/мин

    // Ќаносимый урон, если гидратаци€ = 0
    private float damagePerSecondAtZero = 5f;

    // =====================
    // —осто€ние
    // =====================
    public float CurrentHydration { get; private set; }

    private float decreasePerSecond; // пересчитано дл€ Update() в секунды
    private Character _character; // ссылка на персонажа

    // =====================
    // —татистика
    // =====================
    private float timeWithoutDrink = 0f;
    public float MaxTimeWithoutDrink { get; private set; } = 0f;

    // =====================
    // Unity событи€
    // =====================
    private void Awake()
    {
        // ѕереводим скорость убывани€ из единиц/мин в единицы/сек
        decreasePerSecond = decreasePerMinute / 60f;
    }

    // =====================
    // »нициализаци€ системы
    // =====================
    public bool TryInitialize(Character character, CharacterSystemConfig cfg)
    {
        _character = character;

        // –егистрируем систему у персонажа
        if (!_character.TryRegisterSystem<ICharacterHydrationSystem>(this)) return false;

        // ”станавливаем начальный уровень гидратации
        CurrentHydration = MaxHydration;
        return true;
    }

    // =====================
    // ќбновление каждый кадр
    // =====================
    private void Update()
    {
        float delta = decreasePerSecond * Time.deltaTime;

        // ”величение убывани€ при беге
        var movement = _character.GetSystem<ICharacterMovementSystem>();
        if (movement != null && movement.IsRunning) delta *= 2f; // runMultiplier

        // ”величение убывани€ при атаке
        var attack = _character.GetSystem<IAttackSystem>();
        if (attack != null && attack.IsAttacking) delta *= 1.5f; // attackMultiplier

        // ”меньшаем текущий уровень гидратации
        CurrentHydration -= delta;

        // =====================
        // ¬едение статистики времени без пить€
        // =====================
        if (CurrentHydration > 0f)
        {
            timeWithoutDrink += Time.deltaTime;

            // ќбновл€ем максимальное врем€ без пить€
            if (timeWithoutDrink > MaxTimeWithoutDrink)
                MaxTimeWithoutDrink = timeWithoutDrink;
        }
        else
        {
            // =====================
            // Ќаносим урон, если гидратаци€ = 0
            // =====================
            var health = _character.GetSystem<ICharacterHealthSystem>();
            if (health != null)
                health.TakeDamage(damagePerSecondAtZero * Time.deltaTime);

            // Ќе даЄм уходить в отрицательное значение
            CurrentHydration = 0f;
        }
    }

    // =====================
    // ћетод дл€ восполнени€ гидратации (например, при питье)
    // =====================
    public void AddHydration(float value)
    {
        if (value <= 0f) return;

        // ќграничиваем максимум гидратации
        CurrentHydration = Mathf.Min(CurrentHydration + value, MaxHydration);

        if (value > 0f)
        {
            // —брос времени без пить€
            timeWithoutDrink = 0f;
        }
    }
}