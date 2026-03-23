namespace _Project.Code.Features.Character.MB.NeedsSystem.Hydration
{
    public interface ICharacterHydrationSystem : ICharacterSystem
    {
        /// <summary>Текущий уровень гидратации (0–100)</summary>
        float Hydration { get; }
    }
}
