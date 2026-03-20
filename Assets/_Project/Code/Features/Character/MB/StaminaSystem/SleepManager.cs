using UnityEngine;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.Configurations.Systems;
using _Project.Code.Features.Time;
using _Project.Code.Features.Character.Systems;

namespace _Project.Code.Features.Character.Sleep
{
    public class SleepManager : MonoBehaviour, ICharacterSystem
    {
        [SerializeField] private SleepQualityCalculator qualityCalculator;
        [SerializeField] private CircadianRhythm circadianRhythm;
        
        private _Project.Code.Features.Character.MB.Character _character;
        private TimeManager _timeManager;
        private IStaminaSystem _staminaSystem;
        private float _sleepStartTime;
        private bool _isSleeping;
        
        public bool IsSleeping => _isSleeping;
        
        public bool TryInitialize(_Project.Code.Features.Character.MB.Character character, CharacterSystemConfig cfg)
        {
            _character = character;
            _timeManager = GameSceneContext.Instance?.TimeManager;
            if (_timeManager == null) return false;
            
            _staminaSystem = _character.GetSystem<IStaminaSystem>();
            if (_staminaSystem == null) return false;
            
            if (qualityCalculator == null)
                qualityCalculator = new SleepQualityCalculator();
                
            if (circadianRhythm == null)
                circadianRhythm = new CircadianRhythm();
            
            // Исправлено: регистрируем как ISleepManager
            _character.TryRegisterSystem<ISleepManager>(this as ISleepManager);
            
            return true;
        }
        
        private void Update()
        {
            if (_isSleeping)
            {
                float realTimeSlept = UnityEngine.Time.time - _sleepStartTime;
                _staminaSystem.UpdateSleep(realTimeSlept);
            }
        }
        
        public void TrySleep()
        {
            if (!_staminaSystem.CanSleep)
            {
                Debug.Log("Не могу уснуть — недостаточно устал");
                return;
            }
            
            float quality = qualityCalculator.CalculateQuality(
                hasNoise: CheckForNoise(),
                hasLight: CheckForLight(),
                hasFood: _character.HasFood,
                hasWater: _character.HasWater,
                hasSoftSurface: CheckSoftSurface(),
                temperature: _character.BodyTemperature,
                timeDeviation: circadianRhythm.GetTimeDeviation(_timeManager.CurrentTime),
                season: _timeManager.CurrentSeason
            );
            
            _staminaSystem.SetSleepQuality(quality);
            _staminaSystem.TrySleep(quality);
            
            _isSleeping = true;
            _sleepStartTime = UnityEngine.Time.time;
            
            Debug.Log($"Заснул. Качество сна: {quality:F2}");
        }
        
        public void WakeUp()
        {
            if (!_isSleeping) return;
            
            _staminaSystem.WakeUp();
            _isSleeping = false;
            
            circadianRhythm.UpdateRhythm(
                _timeManager.CurrentTime,
                _timeManager.CurrentTime.AddMinutes(480)
            );
            
            Debug.Log($"Проснулся. Бодрость: {_staminaSystem.CurrentStamina:F0}");
        }
        
        private bool CheckForNoise() => false;
        private bool CheckForLight() => false;
        private bool CheckSoftSurface() => true;
    }
    
    public interface ISleepManager : ICharacterSystem
    {
        bool IsSleeping { get; }
        void TrySleep();
        void WakeUp();
    }
}