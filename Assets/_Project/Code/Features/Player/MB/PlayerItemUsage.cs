using _Project.Code.Features.Items;
using UnityEngine;

namespace _Project.Code.Features.Player.MB
{
    /// <summary>
    /// Player никогда не знает какой именно предмет используется — только Item
    /// </summary>
    [RequireComponent(typeof(Player))]
    public class PlayerItemUsage : MonoBehaviour
    {
        private Player _player;

        private void Awake()
        {
            _player = GetComponent<Player>();
        }

        public void UseItem(Item item)
        {
            if (_player.Character == null) return;

            var context = new ItemUseContext(_player.Character);
            item.Use(context);
        }
    }
}
