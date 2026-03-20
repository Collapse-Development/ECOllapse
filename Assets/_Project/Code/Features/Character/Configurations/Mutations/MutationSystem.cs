using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Project.Code.Features.Character.MB;

namespace _Project.Code.Systems
{
    public class MutationSystem : MonoBehaviour
    {
        [Header("Настройки накопления очков")]
        [SerializeField] private float realHoursPerPoint = 1f; // 1 очко за реальный час
        [SerializeField] private int maxPointsPerLife = 6; // максимум 6 очков за одну жизнь
        
        [Header("Ссылки")]
        [SerializeField] private CharacterBuildConfig playerConfig; // конфиг игрока
        
        [Header("База мутаций")]
        [SerializeField] private List<CharacterMutation> allMutations; // все возможные мутации
        
        // Текущее состояние
        private float _lifeStartTime;
        private int _currentLifePoints; // очки, накопленные за текущую жизнь
        private int _totalPoints; // общее количество очков (сохраняется между жизнями)
        private List<CharacterMutation> _acquiredMutations; // уже приобретенные мутации
        
        // События для связи с UI
        public event Action<int> OnPointsEarned; // событие при получении очка (передает текущее количество за жизнь)
        public event Action<int> OnPlayerDiedWithPoints; // событие при смерти (передает накопленные очки)
        public event Action<CharacterMutation> OnMutationApplied; // событие при применении мутации
        public event Action<CharacterMutation, bool> OnMutationPurchaseAttempt; // событие при попытке покупки (мутация, успех)
        
        private void Awake()
        {
            _acquiredMutations = new List<CharacterMutation>();
            LoadData();
        }
        
        private void Start()
        {
            // Время начала жизни 
            _lifeStartTime = Time.realtimeSinceStartup;
            _currentLifePoints = 0;
            
            Debug.Log($"[MutationSystem] Инициализирован. Всего очков: {_totalPoints}");
        }
        
        private void Update()
        {
            UpdatePointsFromTime();
        }
        
        // Обновление количества очков на основе прошедшего времени
        private void UpdatePointsFromTime()
        {
            float elapsedSeconds = Time.realtimeSinceStartup - _lifeStartTime;
            float elapsedHours = elapsedSeconds /(realHoursPerPoint * 3600f);
            
            int newPoints = Mathf.FloorToInt(elapsedHours);
            newPoints = Mathf.Min(newPoints, maxPointsPerLife);
            
            if (newPoints > _currentLifePoints)
            {
                int earned = newPoints - _currentLifePoints;
                _currentLifePoints = newPoints;
                OnPointsEarned?.Invoke(_currentLifePoints);
                Debug.Log($"[MutationSystem] Заработано {earned} очко(ов) мутаций. Всего за жизнь: {_currentLifePoints}");
            }
        }
        
        public void OnPlayerDeath()
        {
            if (_currentLifePoints > 0)
            {
                // Уведомляем о смерти с количеством очков
                OnPlayerDiedWithPoints?.Invoke(_currentLifePoints);
                Debug.Log($"[MutationSystem] Игрок умер. Заработано {_currentLifePoints} очков мутаций за эту жизнь.");
            }
            else
            {
                Debug.Log($"[MutationSystem] Игрок умер, но не заработал очков мутаций.");
                OnPlayerDiedWithPoints?.Invoke(0);
            }
        }
        
        // Завершить жизнь и сохранить очки (вызывается после того, как игрок потратил очки)
        public void FinalizeLife()
        {
            if (_currentLifePoints > 0)
            {
                _totalPoints += _currentLifePoints;
                SaveData();
                Debug.Log($"[MutationSystem] Жизнь завершена. Добавлено {_currentLifePoints} очков. Всего: {_totalPoints}");
            }
            
            // Сброс счетсчика для следующей жизни
            _currentLifePoints = 0;
            _lifeStartTime = Time.realtimeSinceStartup;
        }
        
        // Получить случайный набор мутаций для выбора
        // Количество мутаций для выбора
        // Список мутаций для выбора
        public List<CharacterMutation> GetRandomMutations(int count = 3)
        {
            if (allMutations == null || allMutations.Count == 0)
            {
                Debug.LogWarning("[MutationSystem] Нет доступных мутаций!");
                return new List<CharacterMutation>();
            }
            
            // Получаем доступные мутации (не купленные)
            var availableMutations = allMutations
                .Where(m => m != null && !IsMutationAcquired(m))
                .ToList();
            
            if (availableMutations.Count == 0)
            {
                Debug.Log("[MutationSystem] Нет доступных для покупки мутаций");
                return new List<CharacterMutation>();
            }
            
            // Перемешиваем и берем первые count
            var shuffled = availableMutations.OrderBy(x => UnityEngine.Random.value).ToList();
            var selected = shuffled.Take(Mathf.Min(count, shuffled.Count)).ToList();
            
            Debug.Log($"[MutationSystem] Сгенерировано {selected.Count} случайных мутаций");
            return selected;
        }
        
        // Купить и применить мутацию (использует текущие очки за жизнь)
        // Мутация для покупки
        // Успешность покупки
        public bool PurchaseMutation(CharacterMutation mutation)
        {
            if (mutation == null)
            {
                Debug.LogError("[MutationSystem] Попытка купить null мутацию");
                OnMutationPurchaseAttempt?.Invoke(null, false);
                return false;
            }
            
            // Проверяем достаточно ли очков
            if (_currentLifePoints < mutation.cost)
            {
                Debug.Log($"[MutationSystem] Недостаточно очков! Нужно: {mutation.cost}, есть: {_currentLifePoints}");
                OnMutationPurchaseAttempt?.Invoke(mutation, false);
                return false;
            }
            
            // Проверяем не куплена ли уже эта мутация
            if (IsMutationAcquired(mutation))
            {
                // Если есть улучшенная версия можно предложить улучшить
                if (mutation.nextUpgrade != null && !IsMutationAcquired(mutation.nextUpgrade))
                {
                    Debug.Log($"[MutationSystem] У вас уже есть {mutation.mutationName}. Предлагаем улучшить до {mutation.nextUpgrade.mutationName}");
                    OnMutationPurchaseAttempt?.Invoke(mutation, false);
                    return false;
                }
                else
                {
                    Debug.Log($"[MutationSystem] Мутация {mutation.mutationName} уже приобретена");
                    OnMutationPurchaseAttempt?.Invoke(mutation, false);
                    return false;
                }
            }
            
            _currentLifePoints -= mutation.cost;
            
            //Применение мутации
            bool applied = ApplyMutation(mutation);
            
            if (applied)
            {
                _acquiredMutations.Add(mutation);
                
                SaveData();
                
                Debug.Log($"[MutationSystem] Куплена мутация: {mutation.mutationName}. Осталось очков за жизнь: {_currentLifePoints}");
                OnMutationPurchaseAttempt?.Invoke(mutation, true);
                return true;
            }
            else
            {

                _currentLifePoints += mutation.cost;
                Debug.LogError($"[MutationSystem] Не удалось применить мутацию {mutation.mutationName}");
                OnMutationPurchaseAttempt?.Invoke(mutation, false);
                return false;
            }
        }
        
        // Применить мутацию к персонажу
        private bool ApplyMutation(CharacterMutation mutation)
        {
            if (playerConfig == null)
            {
                Debug.LogError("[MutationSystem] PlayerConfig не назначен!");
                return false;
            }
            
            try
            {
                //Копия кофига
                CharacterBuildConfig newConfig = playerConfig.DeepClone();
                
                if (newConfig == null)
                {
                    Debug.LogError("[MutationSystem] Не удалось клонировать конфиг!");
                    return false;
                }
                
                // применение мутации
                mutation.Apply(newConfig);
                
                // Замена конфига
                playerConfig = newConfig;
                
                // Уведомляе о применении
                OnMutationApplied?.Invoke(mutation);
                
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[MutationSystem] Ошибка при применении мутации: {e.Message}");
                return false;
            }
        }
        
        // Проверить приобретена ли мутация
        public bool IsMutationAcquired(CharacterMutation mutation)
        {
            if (mutation == null) return false;
            return _acquiredMutations.Contains(mutation);
        }
        

        // Получить все приобретенные мутации
        public List<CharacterMutation> GetAcquiredMutations()
        {
            return new List<CharacterMutation>(_acquiredMutations);
        }
        
        // Получить текущее количество очков за жизнь
        public int GetCurrentLifePoints()
        {
            return _currentLifePoints;
        }
        
        // Получить общее количество очков
        public int GetTotalPoints()
        {
            return _totalPoints;
        }
        
        // Получить максимальное количество очков за жизнь
        public int GetMaxPointsPerLife()
        {
            return maxPointsPerLife;
        }
        
        // Получить время до следующего очка в секундах
        public float GetTimeUntilNextPoint()
        {
            if (_currentLifePoints >= maxPointsPerLife)
                return 0f;
                
            float elapsedSeconds = Time.realtimeSinceStartup - _lifeStartTime;
            float elapsedHours = elapsedSeconds / 3600f;
            float currentPointsFloat = elapsedHours;
            float nextPointAt = Mathf.Ceil(currentPointsFloat);
            float hoursUntilNext = nextPointAt - currentPointsFloat;
            
            return hoursUntilNext * 3600f;
        }
        
        // Получить прогресс до следующего очка (0-1)
        public float GetProgressToNextPoint()
        {
            if (_currentLifePoints >= maxPointsPerLife)
                return 1f;
                
            float elapsedSeconds = Time.realtimeSinceStartup - _lifeStartTime;
            float elapsedHours = elapsedSeconds / 3600f;
            float progress = elapsedHours - _currentLifePoints;
            
            return Mathf.Clamp01(progress);
        }
        
        // Сохранить данные
        private void SaveData()
        {
            try
            {
                PlayerPrefs.SetInt("MutationSystem_TotalPoints", _totalPoints);

                List<string> mutationIds = _acquiredMutations
                    .Where(m => m != null)
                    .Select(m => m.name)
                    .ToList();
                
                string idsString = string.Join(",", mutationIds);
                PlayerPrefs.SetString("MutationSystem_AcquiredMutations", idsString);
                
                PlayerPrefs.Save();
                
                Debug.Log($"[MutationSystem] Данные сохранены: {_totalPoints} очков, {_acquiredMutations.Count} мутаций");
            }
            catch (Exception e)
            {
                Debug.LogError($"[MutationSystem] Ошибка при сохранении: {e.Message}");
            }
        }
        
        //Загрузка данных
        private void LoadData()
        {
            try
            {
                _totalPoints = PlayerPrefs.GetInt("MutationSystem_TotalPoints", 0);
                
                string savedIds = PlayerPrefs.GetString("MutationSystem_AcquiredMutations", "");
                if (!string.IsNullOrEmpty(savedIds))
                {
                    string[] ids = savedIds.Split(',');
                    _acquiredMutations.Clear();
                    
                    foreach (string id in ids)
                    {
                        if (string.IsNullOrEmpty(id)) continue;
                        
                        var mutation = allMutations?.Find(m => m != null && m.name == id);
                        if (mutation != null)
                        {
                            _acquiredMutations.Add(mutation);
                        }
                        else
                        {
                            Debug.LogWarning($"[MutationSystem] Мутация с именем {id} не найдена в базе");
                        }
                    }
                }
                
                Debug.Log($"[MutationSystem] Данные загружены: {_totalPoints} очков, {_acquiredMutations.Count} мутаций");
            }
            catch (Exception e)
            {
                Debug.LogError($"[MutationSystem] Ошибка при загрузке: {e.Message}");
                _totalPoints = 0;
                _acquiredMutations.Clear();
            }
        }
        
        // Сбросить все мутации (тестирование)
        [ContextMenu("Reset All Mutations")]
        public void ResetAllMutations()
        {
            _acquiredMutations.Clear();
            _totalPoints = 0;
            _currentLifePoints = 0;
            SaveData();
            Debug.Log("[MutationSystem] Все мутации сброшены");
        }
        
        // Добавить очки вручную (тестирование)
        [ContextMenu("Add Test Points")]
        public void AddTestPoints()
        {
            _currentLifePoints = Mathf.Min(_currentLifePoints + 3, maxPointsPerLife);
            OnPointsEarned?.Invoke(_currentLifePoints);
            Debug.Log($"[MutationSystem] Добавлено тестовых очков. Теперь: {_currentLifePoints}");
        }
    }
    
   
}