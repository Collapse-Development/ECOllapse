using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.Character.MB.AttackSystem
{
    public interface IAttackSystem : ICharacterSystem
    {
        void Attack();
        bool IsAttacking { get; }
        float AttackDamage { get; }
        float AttackRange { get; }
    }
}