using System;
using UnityEngine;

namespace _Project.Code.Features.Player.MB
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Character.MB.Character _character;

        public Character.MB.Character Character
        {
            get => _character;
            set
            {
                var oldCharacter = _character;
                _character = value;

                _character.transform.position = this.transform.position; //Раньше Character спавнился всегда в нулевой точке (в точке скрипта Character), сейчас он спавнитсья в позиции Player.

                OnCharacterUpdated?.Invoke(oldCharacter, _character);
            }
        }

        public event Action<Character.MB.Character, Character.MB.Character> OnCharacterUpdated;
    }
}