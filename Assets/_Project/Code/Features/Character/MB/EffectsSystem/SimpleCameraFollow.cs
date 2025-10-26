using UnityEngine;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    /// <summary>
    /// Simple third-person camera that follows the character with smooth movement.
    /// </summary>
    public class SimpleCameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        
        [Header("Camera Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0, 5, -8);
        [SerializeField] private float smoothSpeed = 5f;
        [SerializeField] private float lookAtHeight = 1f;
        
        private void LateUpdate()
        {
            if (target == null) return;
            
            // Calculate desired position
            Vector3 desiredPosition = target.position + offset;
            
            // Smoothly move camera to desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            
            // Look at the target with offset
            Vector3 lookAtPosition = target.position + Vector3.up * lookAtHeight;
            transform.LookAt(lookAtPosition);
        }
        
        /// <summary>
        /// Sets the target for the camera to follow.
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
