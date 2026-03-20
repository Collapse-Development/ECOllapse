using _Project.Code.Features.Character.MB;

namespace CharacterSystems
{
    public interface ICharacterHealthRegenSystem : ICharacterSystem
    {
        float CurrentRegenRate { get; }
        bool IsRegenerating { get; }
    }
}
