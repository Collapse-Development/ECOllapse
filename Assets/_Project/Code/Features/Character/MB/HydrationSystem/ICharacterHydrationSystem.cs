using _Project.Code.Features.Character.MB;

// Интерфейс для системы гидратации персонажа
public interface ICharacterHydrationSystem : ICharacterSystem
{
    // Текущий уровень гидратации (0 — 100)
    float CurrentHydration { get; }

    // Максимальный уровень гидратации
    float MaxHydration { get; }

    // Пополнить гидратацию, выпив воды
    void AddHydration(float value);

    // Статистика: максимальное время без питья (в секундах)
    float MaxTimeWithoutDrink { get; }
}