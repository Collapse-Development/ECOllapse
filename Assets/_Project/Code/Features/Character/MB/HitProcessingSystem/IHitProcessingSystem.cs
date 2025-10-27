using System;
using _Project.Code.Features.Character.MB;

namespace Project.Code.Features.Character.MB.HitProcessingSystem
{
    public interface ICharacterHitProcessingSystem : ICharacterSystem
    {
        event Action OnHit;
        void ProcessHit(float damage);
    }
}
