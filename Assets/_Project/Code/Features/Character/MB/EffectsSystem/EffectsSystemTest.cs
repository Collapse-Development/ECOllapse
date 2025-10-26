using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using _Project.Code.Features.Character.MB.MovementSystem;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    /// <summary>
    /// Test script demonstrating the Universal Effects System.
    /// Registers a MovementSpeedEffect and provides keyboard controls to add different modifier types.
    /// Integrates with CharacterMovementSystem to apply speed effects to actual character movement.
    /// </summary>
    public class EffectsSystemTest : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text speedMultiplierText;
        
        [Header("Test Configuration")]
        [SerializeField] private AnimationCurve testCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 2f);
        [SerializeField] private float baseSpeed = 5f;
        
        private IMovementSpeedEffect _movementSpeedEffect;
        private ICharacterMovementSystem _movementSystem;
        private EffectHandle _lastHandle;
        private Character _character;
        
        // New Input System
        private Keyboard _keyboard;
        
        private void Start()
        {
            // Initialize keyboard reference
            _keyboard = Keyboard.current;
            if (_keyboard == null)
            {
                Debug.LogWarning("EffectsSystemTest: No keyboard detected!");
            }
            
            // Get the Character component
            _character = GetComponent<Character>();
            if (_character == null)
            {
                Debug.LogError("EffectsSystemTest: Character component not found!");
                return;
            }
            else
            {
                Debug.Log("EffectsSystemTest: Character component found.");
            }
            
            // Get the EffectsSystem instance
            var effectsSystem = EffectsSystem.Instance;
            if (effectsSystem == null)
            {
                Debug.LogError("EffectsSystemTest: EffectsSystem.Instance is null!");
                return;
            }
            else
            {
                Debug.Log("EffectsSystemTest: EffectsSystem instance found.");
            }
            
            // Get the MovementSystem
            _movementSystem = _character.GetSystem<ICharacterMovementSystem>();
            if (_movementSystem == null)
            {
                Debug.LogError("EffectsSystemTest: CharacterMovementSystem not found! Speed effects won't affect movement.");
                Debug.LogError("Make sure the Character has a CharacterMovementSystem component.");
            }
            else
            {
                Debug.Log($"EffectsSystemTest: MovementSystem found. Current speed: {_movementSystem.Speed}");
            }
            
            // Create and register the MovementSpeedEffect
            _movementSpeedEffect = new MovementSpeedEffect();
            effectsSystem.Register(_movementSpeedEffect);
            
            Debug.Log("EffectsSystemTest: MovementSpeedEffect registered successfully.");
            Debug.Log("=== Controls ===");
            Debug.Log("  WASD - Move character");
            Debug.Log("  1 - Add ConstantMultiplier (1.5x permanent)");
            Debug.Log("  2 - Add MultiplyForDuration (2.0x for 3 seconds)");
            Debug.Log("  3 - Add MultiplyForDuration (0.5x for 2 seconds - slow effect)");
            Debug.Log("  4 - Add CurveMultiplier (ease-in-out over 4 seconds)");
            Debug.Log("  C - Cancel last added modifier");
        }
        
        private void Update()
        {
            // Apply speed multiplier to movement system
            if (_movementSpeedEffect != null && _movementSystem != null)
            {
                _movementSystem.Speed = baseSpeed * _movementSpeedEffect.SpeedMultiplier;
            }
            
            // Update UI display
            if (_movementSpeedEffect != null && speedMultiplierText != null)
            {
                float currentSpeed = _movementSystem != null ? _movementSystem.Speed : 0f;
                speedMultiplierText.text = $"Speed Multiplier: {_movementSpeedEffect.SpeedMultiplier:F2}x\nCurrent Speed: {currentSpeed:F2}";
            }
            
            // Handle movement input
            HandleMovementInput();
            
            // Handle effect modifier input
            if (_keyboard != null)
            {
                if (_keyboard.digit1Key.wasPressedThisFrame)
                {
                    AddConstantMultiplier();
                }
                else if (_keyboard.digit2Key.wasPressedThisFrame)
                {
                    AddMultiplyForDuration(2.0f, 3.0f);
                }
                else if (_keyboard.digit3Key.wasPressedThisFrame)
                {
                    AddMultiplyForDuration(0.5f, 2.0f);
                }
                else if (_keyboard.digit4Key.wasPressedThisFrame)
                {
                    AddCurveMultiplier();
                }
                else if (_keyboard.cKey.wasPressedThisFrame)
                {
                    CancelLastModifier();
                }
            }
        }
        
        private void HandleMovementInput()
        {
            if (_movementSystem == null)
            {
                if (Time.frameCount % 120 == 0)
                {
                    Debug.LogWarning("HandleMovementInput: MovementSystem is null!");
                }
                return;
            }
            
            if (_keyboard == null)
            {
                return;
            }
            
            Vector3 direction = Vector3.zero;
            bool anyKeyPressed = false;
            
            if (_keyboard.wKey.isPressed)
            {
                direction += Vector3.forward;
                anyKeyPressed = true;
            }
            if (_keyboard.sKey.isPressed)
            {
                direction += Vector3.back;
                anyKeyPressed = true;
            }
            if (_keyboard.aKey.isPressed)
            {
                direction += Vector3.left;
                anyKeyPressed = true;
            }
            if (_keyboard.dKey.isPressed)
            {
                direction += Vector3.right;
                anyKeyPressed = true;
            }
            
            _movementSystem.SetDirection(direction);
            
            // Debug logging every 2 seconds when moving
            if (anyKeyPressed && Time.frameCount % 120 == 0)
            {
                Debug.Log($"[Movement] Direction: {direction.normalized}, Speed: {_movementSystem.Speed:F2}, IsMoving: {_movementSystem.IsMoving}, Position: {transform.position}");
            }
        }
        
        private void AddConstantMultiplier()
        {
            if (_movementSpeedEffect == null) return;
            
            var modifier = new ConstantMultiplier(1.5f);
            _lastHandle = _movementSpeedEffect.Add(modifier);
            
            Debug.Log("Added ConstantMultiplier (1.5x permanent)");
        }
        
        private void AddMultiplyForDuration(float factor, float duration)
        {
            if (_movementSpeedEffect == null) return;
            
            var modifier = new MultiplyForDuration(factor, duration);
            _lastHandle = _movementSpeedEffect.Add(modifier);
            
            Debug.Log($"Added MultiplyForDuration ({factor}x for {duration} seconds)");
        }
        
        private void AddCurveMultiplier()
        {
            if (_movementSpeedEffect == null) return;
            
            var modifier = new CurveMultiplier(testCurve, 4.0f);
            _lastHandle = _movementSpeedEffect.Add(modifier);
            
            Debug.Log("Added CurveMultiplier (ease-in-out over 4 seconds)");
        }
        
        private void CancelLastModifier()
        {
            _lastHandle.Cancel();
            Debug.Log("Cancelled last modifier");
        }
    }
}
