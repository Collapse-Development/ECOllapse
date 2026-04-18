using System;
using UnityEngine;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.Systems;

namespace _Project.Code.Features.Character.Configurations.Systems
{
    [CreateAssetMenu(fileName = "StaminaSystemConfig", menuName = "ECOllapse/Character/Systems/StaminaSystemConfig")]
    public class StaminaSystemConfig : CharacterSystemConfig
    {
        public float maxStamina = 100f;
        public float decayPerHour = 4f;
        public float baseSleepRecovery = 8f;
        public float warningThreshold = 40f;
        public float tiredThreshold = 35f;
        public float minStaminaToSleep = 50f;
        
        public override Type CharacterSystemType => typeof(StaminaSystem);
    }
}