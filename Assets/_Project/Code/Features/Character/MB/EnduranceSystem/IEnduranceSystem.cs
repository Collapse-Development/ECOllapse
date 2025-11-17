using UnityEngine;
using System.Collections;
using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.Character.MB.EnduranceSystem
{
    public interface IEnduranceSystem : ICharacterSystem
    {
        float CurrentEndurance { get; }
        float MaxEndurance { get; }
        bool IsStunned { get; }
        
        void TakeEnduranceDamage(float damage, EnduranceDamageType damageType);
        void ApplyStun(float duration);
    }

    public enum EnduranceDamageType
    {
        Standing,    // На обеих ногах - меньше урон
        Jumping,     // В прыжке - больше урон
        Falling      // Падение - особый расчет
    }
}