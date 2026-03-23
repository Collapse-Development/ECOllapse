using System;
using _Project.Code.Features.Character.MB;

namespace CharacterSystems
{
    public interface ICharacterDamageResistanceSystem : ICharacterSystem
    {
        float Resistance { get; set; }
    }
}
