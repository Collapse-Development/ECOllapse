namespace _Project.Code.Features.Character.MB.NeedsSystem.Vigor
{
    public interface ICharacterVigorSystem : ICharacterSystem
    {
        /// <summary>Текущий уровень бодрости (0–100)</summary>
        float Vigor { get; }
    }
}
