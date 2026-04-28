using CharacterMB = _Project.Code.Features.Character.MB.Character;

namespace _Project.Code.Features.Items
{
    /// <summary>
    /// Контекст использования предмета — кто использует, где, при каких условиях
    /// </summary>
    public class ItemUseContext
    {
        public CharacterMB User { get; }

        public ItemUseContext(CharacterMB user)
        {
            User = user;
        }
    }
}
