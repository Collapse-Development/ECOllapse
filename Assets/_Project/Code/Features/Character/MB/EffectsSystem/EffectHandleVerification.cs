using UnityEngine;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    /// <summary>
    /// Verification script to test EffectHandle cancellation behavior.
    /// Attach to a GameObject and check the console for verification results.
    /// </summary>
    public class EffectHandleVerification : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("=== Starting EffectHandle Cancellation Verification ===");
            
            VerifyModifierCancellation();
            VerifyRemovalCallback();
            VerifyNullSafety();
            
            Debug.Log("=== EffectHandle Cancellation Verification Complete ===");
        }

        /// <summary>
        /// Verifies that EffectHandle.Cancel() invokes Cancel() on the stored modifier reference.
        /// </summary>
        private void VerifyModifierCancellation()
        {
            Debug.Log("\n[Test 1] Verifying modifier cancellation...");
            
            var effect = new MovementSpeedEffect();
            var modifier = new ConstantMultiplier(1.5f);
            
            // Add modifier and get handle
            var handle = effect.Add(modifier);
            
            // Verify modifier is not expired before cancellation
            if (modifier.IsExpired)
            {
                Debug.LogError("FAILED: Modifier should not be expired before cancellation");
                return;
            }
            
            // Cancel via handle
            handle.Cancel();
            
            // Verify modifier is now expired
            if (!modifier.IsExpired)
            {
                Debug.LogError("FAILED: Modifier should be expired after cancellation");
                return;
            }
            
            Debug.Log("PASSED: EffectHandle.Cancel() successfully invokes Cancel() on modifier");
        }

        /// <summary>
        /// Verifies that EffectHandle.Cancel() invokes the removal callback to remove modifier from effect.
        /// </summary>
        private void VerifyRemovalCallback()
        {
            Debug.Log("\n[Test 2] Verifying removal callback...");
            
            var effect = new MovementSpeedEffect();
            var modifier = new ConstantMultiplier(1.5f);
            
            // Add modifier and get handle
            var handle = effect.Add(modifier);
            
            // Verify modifier is in the effect (SpeedMultiplier should be 1.5)
            effect.Tick(0f);
            if (Mathf.Approximately(effect.SpeedMultiplier, 1.5f) == false)
            {
                Debug.LogError($"FAILED: Expected SpeedMultiplier to be 1.5, got {effect.SpeedMultiplier}");
                return;
            }
            
            // Cancel via handle
            handle.Cancel();
            
            // Tick to recompute (modifier should be removed)
            effect.Tick(0f);
            
            // Verify modifier was removed (SpeedMultiplier should be 1.0)
            if (Mathf.Approximately(effect.SpeedMultiplier, 1.0f) == false)
            {
                Debug.LogError($"FAILED: Expected SpeedMultiplier to be 1.0 after removal, got {effect.SpeedMultiplier}");
                return;
            }
            
            Debug.Log("PASSED: EffectHandle.Cancel() successfully invokes removal callback");
        }

        /// <summary>
        /// Verifies that null-safety checks prevent errors on invalid handles.
        /// </summary>
        private void VerifyNullSafety()
        {
            Debug.Log("\n[Test 3] Verifying null-safety...");
            
            try
            {
                // Create default (invalid) handle
                var defaultHandle = new EffectHandle();
                
                // This should not throw an exception
                defaultHandle.Cancel();
                
                Debug.Log("PASSED: Null-safety checks prevent errors on invalid handles");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"FAILED: Exception thrown on invalid handle: {ex.Message}");
            }
        }
    }
}
