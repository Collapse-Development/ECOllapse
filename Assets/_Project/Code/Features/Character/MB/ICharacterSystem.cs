namespace _Project.Code.Features.Character.MB
{
    public interface ICharacterSystem
    {
        public bool TryRegister(Character character);
        public bool TryInitialize(Character character, CharacterSystemConfig cfg);
        public bool TryResolveDependencies(Character character);
    }
}
