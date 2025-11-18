using _Project.Code.Features.Character.MB;
using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace _Project.Code.Features.Character.MB.Systems
{
    [DisallowMultipleComponent]
    public class CharacterStaminaSystem : MonoBehaviour, IStaminaSystem
    {
        [Header("Stamina settings")]
        [SerializeField, Min(0f)] private float maxStamina = 100f;
        [SerializeField, Min(0f)] private float diminishPerSecond = 10f;
        [SerializeField, Min(0f)] private float recoverPerSecond = 8f;
        private float currentStamina;
        private bool wasExhausted;

        public float CurrentStamina => currentStamina;
        public float MaxStamina => maxStamina;
        public bool IsExhausted => wasExhausted;

        float IStaminaSystem.CurrentStamina { get => CurrentStamina; set => throw new NotImplementedException(); }

        public event Action<float> OnExhausted ;
        public event Action<float> OnRecovered;

        private Character _character;
        private CharacterSystemConfig _config;

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            _character = character;
            _config = cfg;

            currentStamina = maxStamina;
            wasExhausted = false;

            Debug.Log("[Stamina] Initialized for " + character.name);
            return true;
        }

        private void Start()
        {
            currentStamina = maxStamina;
            OnExhausted += AddStamina;
            OnRecovered += DiminishStamina;
        }
        private void Update()
        {
            UpdateStamina();

        }
        private void OnDestroy()
        {
            OnExhausted -= AddStamina;
            OnRecovered -= DiminishStamina;
        }

        public void UpdateStamina()
        {
            bool exhaustedNow = currentStamina <= 0.01f;
                Debug.Log(currentStamina);
                wasExhausted = exhaustedNow;

                if (exhaustedNow)
                {
                    OnExhausted?.Invoke(recoverPerSecond);
                    Debug.Log("[Stamina] Exhausted!");
                }
                else
                {
                    OnRecovered?.Invoke(diminishPerSecond);
                    Debug.Log("[Stamina] Recovered!");
                }
        }

        public void DiminishStamina(float value) 
        {
            currentStamina = Mathf.Clamp(currentStamina - value, 0f, maxStamina);
        }

        public void AddStamina(float value)
        {
            currentStamina = Mathf.Clamp(currentStamina + value, 0f, maxStamina);
        }
    }
}
