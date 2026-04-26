using UnityEngine;
using Code.Core.Chunks; // Твой неймспейс с математикой

namespace _Project.Code.Features.World
{
    // Базовый класс для всего, что спавнит ObjectGenerator
    public abstract class WorldObject : MonoBehaviour
    {
        [Header("Базовые настройки")]
        public string objectName; // Например, "Обычный камень"
        
        // Позиционирование в твоей системе чанков
        protected ChunkIndex _currentChunk;
        protected LocalPos _localPos;
        protected bool _isInitialized;

        /// <summary>
        /// Вызывается генератором сразу после создания объекта
        /// </summary>
        public virtual void Initialize(ChunkIndex chunkIndex, LocalPos localPos)
        {
            _currentChunk = chunkIndex;
            _localPos = localPos;
            _isInitialized = true;
        }

        // Общий метод для взаимодействия (игрок кликнул/нажал 'E')
        public virtual void Interact(GameObject interactor)
        {
            if (!_isInitialized) return;
            Debug.Log($"Взаимодействие с {objectName} в чанке ({_currentChunk.X}, {_currentChunk.Y})");
        }
    }
}