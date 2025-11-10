using UnityEngine;

namespace _Project.Code.Features.Character.MB.MovementSystem
{
    public interface ICharacterMovementSystem : ICharacterSystem
    {
        Vector3 Direction { get; }
        float Speed { get; set; }
        bool IsMoving { get; }
        void SetDirection(Vector3 direction);
        void SetRunning(bool isRunning);
        void ApplyFrameSpeedMultiplier(float multiplier);
    }
}