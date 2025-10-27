namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public interface ICharacterEffectsSystem : ICharacterSystem
    {
        public T GetEffect<T>() where T : class, ICharacterEffect;
    }
}