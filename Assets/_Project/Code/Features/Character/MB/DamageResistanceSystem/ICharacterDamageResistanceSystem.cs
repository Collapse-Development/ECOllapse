using System;
using _Project.Code.Features.Character.MB;

namespace CharacterSystems
{
    public interface ICharacterDamageResistanceSystem : ICharacterSystem
    {
        float TotalResistance { get; }

        void AddResistance(string key, float value);
        bool RemoveResistance(string key);
        void ClearAllModifiers();
        bool TryGetResistance(string key, out float value);
    }
}
