namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public interface IMovementSpeedEffect : ICharacterEffect
    {
        float SpeedMultiplier { get; }
    }
}
