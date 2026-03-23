namespace _Project.Code.Features.Character.MB.NeedsSystem.Satiety
{
    public interface ICharacterSatietySystem : ICharacterSystem
    {
        /// <summary>Текущий уровень сытости (0–100)</summary>
        float Satiety { get; }
    }
}
