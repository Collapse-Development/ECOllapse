using System;

namespace Project.Code.Features.Character.MB
{
    public interface IHitProcessingSystem : ICharacterSystem
    {
        event Action OnHit;
        void ProcessHit(float damage);
    }
}
