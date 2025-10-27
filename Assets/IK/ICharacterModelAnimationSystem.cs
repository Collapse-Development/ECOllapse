using UnityEngine;

namespace _Project.Code.Features.Character.MB.Model
{
    /// <summary>
    /// Интерфейс системы процедурной анимации модели персонажа.
    /// </summary>
    public interface ICharacterModelAnimationSystem : ISystem
    {
        /// <summary>Задать входное движение в локальных координатах модели (м/с).</summary>
        void SetMoveVelocity(Vector3 velocity);

        /// <summary>Принудительно выставить маску коллизий "земли".</summary>
        void SetGroundMask(LayerMask mask);

        /// <summary>Включить/выключить IK (на случай отладки).</summary>
        void SetEnabled(bool enabled);
    }
}
