using UnityEngine;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.MB.NeedsSystem.Satiety;
using _Project.Code.Features.Character.MB.Thirst;
using _Project.Code.Features.Character.MB.Vigor;

namespace CharacterSystems
{
    public class CharacterHealthRegenSystem : BaseCharacterSystem, ICharacterHealthRegenSystem
    {
        // Пороги, ниже которых регенерация не работает
        private const float SatietyThreshold = 70f;
        private const float HydrationThreshold = 70f;
        private const float VigorThreshold = 40f;

        private ICharacterHealthSystem _healthSystem;
        private ICharacterSatietySystem _satietySystem;
        private ICharacterThirstSystem _hydrationSystem;
        private ICharacterVigorSystem _vigorSystem;

        private float _maxRegen;
        private float _minRegen;

        public float CurrentRegenRate { get; private set; }
        public bool IsRegenerating { get; private set; }

        public override bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (!base.TryInitialize(character, cfg)) return false;

            if (cfg is not CharacterHealthRegenSystemConfig regenCfg) return false;

            if (!character.TryRegisterSystem<ICharacterHealthRegenSystem>(this)) return false;

            _healthSystem = character.GetSystem<ICharacterHealthSystem>();
            _satietySystem = character.GetSystem<ICharacterSatietySystem>();
            _hydrationSystem = character.GetSystem<ICharacterThirstSystem>();
            _vigorSystem = character.GetSystem<ICharacterVigorSystem>();

            _maxRegen = regenCfg.MaxRegenPerSecond;
            _minRegen = _maxRegen / 2f;

            Debug.Log($"HealthRegenSystem initialized: MaxRegen={_maxRegen} HP/s. IsActive={IsActive}");
            return true;
        }

        private void Update()
        {
            if (!IsActive) 
            {
                IsRegenerating = false;
                CurrentRegenRate = 0f;
                return;
            }
            
            if (_healthSystem == null) return;

            float satiety = _satietySystem?.Satiety ?? 0f;
            float hydration = _hydrationSystem?.Hydration ?? 0f;
            float vigor = _vigorSystem?.Vigor ?? 0f;

            IsRegenerating = CanRegenerate(satiety, hydration, vigor);

            if (!IsRegenerating)
            {
                CurrentRegenRate = 0f;
                return;
            }

            CurrentRegenRate = CalculateRegen(satiety, hydration, vigor);
            _healthSystem.AddHealth(CurrentRegenRate * Time.deltaTime);
        }

        /// <summary>
        /// Регенерация возможна только если все показатели выше порогов
        /// </summary>
        private bool CanRegenerate(float satiety, float hydration, float vigor)
        {
            return satiety >= SatietyThreshold
                && hydration >= HydrationThreshold
                && vigor >= VigorThreshold;
        }

        /// <summary>
        /// Regen = Min(MaxRegen, MinRegen + (dSat/30)*0.5 + (dHyd/30)*0.3 + (dVig/20)*0.2)
        /// </summary>
        private float CalculateRegen(float satiety, float hydration, float vigor)
        {
            float dSat = satiety - SatietyThreshold;
            float dHyd = hydration - HydrationThreshold;
            float dVig = vigor - VigorThreshold;

            float regen = _minRegen
                + (dSat / 30f) * 0.5f
                + (dHyd / 30f) * 0.3f
                + (dVig / 20f) * 0.2f;

            return Mathf.Min(_maxRegen, regen);
        }
    }
}
