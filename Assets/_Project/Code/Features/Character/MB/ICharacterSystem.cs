namespace _Project.Code.Features.Character.MB
{
    public interface ICharacterSystem
    {
        public bool TryInitialize(Character character, CharacterSystemConfig cfg);
    }
}