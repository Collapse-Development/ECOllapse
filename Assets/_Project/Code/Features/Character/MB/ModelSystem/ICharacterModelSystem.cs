using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.MB.Model;

namespace CharacterSystems
{
    public interface ICharacterModelSystem : ICharacterSystem
    {
        CharacterModel Model { get; }
    }
}