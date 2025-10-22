using UnityEngine;

namespace _Project.Code.Features.Character.MB
{
    public interface ICharacterSystem
    {
        void Initialize(Character character);
        
        // Перегруженный метод для инициализации с конфигом движения
        void Initialize(Character character, CharacterMovementSystemConfig cfg);
    }
}