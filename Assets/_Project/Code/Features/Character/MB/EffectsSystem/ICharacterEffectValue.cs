namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public interface ICharacterEffectValue<out T>
    {
        void Tick(float dt);
        T GetValue();
    }
}