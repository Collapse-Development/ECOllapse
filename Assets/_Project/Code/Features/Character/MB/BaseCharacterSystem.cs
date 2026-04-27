using UnityEngine;

namespace _Project.Code.Features.Character.MB
{
    // Наследуем MonoBehaviour и реализуем интерфейс ICharacterSystem
    public abstract class BaseCharacterSystem : MonoBehaviour, ICharacterSystem
    {
        // Свойство состояния системы
        public bool IsActive { get; protected set; }

        // Виртуальный метод, чтобы наследники могли его вызвать через base.TryInitialize
        public virtual bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg == null) return false;
            
            IsActive = cfg.IsActive; 
            return true;
        }

        // Метод для динамического переключения (например, при получении мутации или дебаффа)
        public virtual void SetActiveState(bool state)
        {
            IsActive = state;
        }
    }
}