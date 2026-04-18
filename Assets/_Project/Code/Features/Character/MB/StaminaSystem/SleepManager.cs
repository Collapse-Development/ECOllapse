using UnityEngine;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.Configurations.Systems;
using _Project.Code.Features.Character.Systems;

namespace _Project.Code.Features.Character.Sleep
{
    public class SleepManager : MonoBehaviour, ICharacterSystem
    {
        [SerializeField] private SleepQualityCalculator qualityCalculator;
        [SerializeField] private CircadianRhythm circadianRhythm;
        
        private _Project.Code.Features.Character.MB.Character _character;
        private IStaminaSystem _staminaSystem;
        private float _sleepStartTime;
        private bool _isSleeping;
        
        public bool IsSleeping => _isSleeping;
        
        public bool TryInitialize(_Project.Code.Features.Character.MB.Character character, CharacterSystemConfig cfg)
        {
            _character = character;
            
            _staminaSystem = _character.GetSystem<IStaminaSystem>();
            if (_staminaSystem == null) return false;
            
            qualityCalculator ??= new SleepQualityCalculator();
            circadianRhythm ??= new CircadianRhythm();
            
            _character.TryRegisterSystem<ISleepManager>(this as ISleepManager);
            return true;
        }
        
        private void Update()
        {
            if (_isSleeping)
            {
                float realTimeSlept = Time.time - _sleepStartTime;
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
                hasNoise: false,
                hasLight: false,
                hasFood: _character.HasFood,
                hasWater: _character.HasWater,
                hasSoftSurface: true,
                temperature: _character.BodyTemperature,
                timeDeviation: circadianRhythm.GetTimeDeviation(),
                season: 0 // временно, без TimeSystem
            );
            
            _staminaSystem.SetSleepQuality(quality);
            _staminaSystem.TrySleep(quality);
            _isSleeping = true;
            _sleepStartTime = Time.time;
        }
        
        public void WakeUp()
        {
            if (!_isSleeping) return;
            _staminaSystem.WakeUp();
            _isSleeping = false;
            circadianRhythm.UpdateRhythm();
        }
    }
    
    public interface ISleepManager : ICharacterSystem
    {
        bool IsSleeping { get; }
        void TrySleep();
        void WakeUp();
    }
}