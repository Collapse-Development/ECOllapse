using UnityEngine;
using System.Collections;
using System;

public class CharacterAttackSystem : MonoBehaviour, IAttackSystem
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask targetLayers = Physics.DefaultRaycastLayers;
    
    [Header("Attack Area")]
    [SerializeField] private Vector3 attackOffset = new Vector3(0, 1, 0);
    [SerializeField] private float attackRadius = 1.5f;
    
    [Header("Visual Effects")]
    [SerializeField] private bool showDebugGizmos = true;
    
    private void Start()
    {
        // Авторегистрация в системе Character
        var character = GetComponent<Character>();
        character?.TryRegisterSystem<IAttackSystem>(this);
    }
    
     // Reference to main Character class
    private Character character;
    private bool isAttacking = false;
    private Coroutine currentAttackCoroutine;
    
    // ICharacterSystem implementation
    public void Initialize(Character character)
    {
        this.character = character;
        Debug.Log("CharacterAttackSystem initialized");
    }
    
    // IAttackSystem implementation
    public void Attack()
    {
        if (isAttacking)
        {
            Debug.Log("Attack already in progress");
            return;
        }
        
        // Start attack sequence
        currentAttackCoroutine = StartCoroutine(AttackSequence());
    }
    
    public bool IsAttacking => isAttacking;
    public float AttackDamage => attackDamage;
    public float AttackRange => attackRange;
    
    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        
        Debug.Log($"Attack started! Damage: {attackDamage}, Delay: {attackDelay}s");
        
        // Wait for the attack delay
        yield return new WaitForSeconds(attackDelay);
        
        // Perform the actual attack
        PerformAttack();
        
        // Small cooldown after attack
        yield return new WaitForSeconds(0.1f);
        
        isAttacking = false;
        Debug.Log("Attack completed");
    }
    
    private void PerformAttack()
    {
        // Calculate attack position (in front of character)
        Vector3 attackPosition = transform.position + transform.forward * attackRange + attackOffset;
        
        // Find all colliders in the attack sphere
        Collider[] hitColliders = Physics.OverlapSphere(attackPosition, attackRadius, targetLayers);
        
        bool hitTarget = false;
        
        foreach (Collider collider in hitColliders)
        {
            // Skip if it's the character itself
            if (collider.gameObject == gameObject)
                continue;
            
            // Check if the object has IHitProcessingSystem
            IHitProcessingSystem hitSystem = collider.GetComponent<IHitProcessingSystem>();
            
            if (hitSystem != null)
            {
                // Apply damage to the target
                hitSystem.ProcessHit(attackDamage);
                hitTarget = true;
                
                Debug.Log($"Hit target: {collider.gameObject.name} with damage: {attackDamage}");
            }
        }
        
        if (!hitTarget)
        {
            Debug.Log("Attack missed - no valid targets in range");
        }
    }
    
    // Method to stop current attack
    public void StopAttack()
    {
        if (currentAttackCoroutine != null)
        {
            StopCoroutine(currentAttackCoroutine);
            currentAttackCoroutine = null;
        }
        isAttacking = false;
    }
    
    // Visualization in Scene view
    private void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;
        
        // Draw attack range
        Gizmos.color = Color.yellow;
        Vector3 attackPosition = transform.position + transform.forward * attackRange + attackOffset;
        Gizmos.DrawWireSphere(attackPosition, attackRadius);
        
        // Draw line to show attack direction
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + attackOffset, attackPosition);
    }
    
    // Cleanup
    private void OnDestroy()
    {
        StopAttack();
    }
}