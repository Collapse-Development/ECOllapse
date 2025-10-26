namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    /// <summary>
    /// Interface for the movement speed effect that manages speed modifiers.
    /// Provides access to the computed speed multiplier value.
    /// </summary>
    public interface IMovementSpeedEffect : IEffect<ISpeedModifier>
    {
        /// <summary>
        /// Gets the current speed multiplier computed from all active modifiers.
        /// Base value is 1.0f (normal speed). Values > 1.0f increase speed, values < 1.0f decrease speed.
        /// </summary>
        float SpeedMultiplier { get; }
    }
}
