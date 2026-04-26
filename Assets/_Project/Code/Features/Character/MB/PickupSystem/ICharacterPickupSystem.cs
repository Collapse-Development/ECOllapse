using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.Character.MB.Pickup
{
    public interface ICharacterPickupSystem : ICharacterSystem
    {
        // Метод попытки поднять предмет
        void TryPickup();
    }
}