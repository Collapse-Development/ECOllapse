namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public interface ICharacterEffectModifier
    {
        void Evaluate(float dt);

        void Apply(ref float value);
        
        bool IsExpired { get; }
        
        void Cancel();
    }
}
