using UnityEngine;

namespace _Project.Code.Features.Character.MB.MovementSystem
{
    public interface IMovementSystem : ICharacterSystem
    {
      
        Vector3 Direction { get; }
        float Speed { get; set; }
        bool IsMoving { get; }
        void SetDirection(Vector3 direction);
    }
}