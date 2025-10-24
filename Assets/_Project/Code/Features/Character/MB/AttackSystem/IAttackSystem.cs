public interface IAttackSystem : ICharacterSystem
{
    void Attack();
    bool IsAttacking { get; }
    float AttackDamage { get; }
    float AttackRange { get; }
}