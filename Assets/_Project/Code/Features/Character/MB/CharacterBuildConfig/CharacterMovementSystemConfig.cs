using UnityEngine;

[CreateAssetMenu(fileName = "MovementSystemConfig", menuName = "Character Systems/Movement System Config")]
public class CharacterMovementSystemConfig : SystemConfig<CharacterMovementSystem>
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    
    [Header("Advanced Settings")] 
    [SerializeField] private bool enableAdvancedFeatures = false;
    [SerializeField] private float someOtherParameter = 1f;
    
    public float Speed => speed;
    public float RotationSpeed => rotationSpeed;
    public bool EnableAdvancedFeatures => enableAdvancedFeatures;
    public float SomeOtherParameter => someOtherParameter;
}