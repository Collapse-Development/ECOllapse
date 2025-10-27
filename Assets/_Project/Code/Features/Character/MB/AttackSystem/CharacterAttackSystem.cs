using UnityEngine;
using System.Collections;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.Configurations.Systems;
using Project.Code.Features.Character.MB.HitProcessingSystem;

namespace _Project.Code.Features.Character.MB.AttackSystem
{
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
        
        private Character _character;
        private bool isAttacking = false;
        private Coroutine currentAttackCoroutine;
        
        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            var attackCfg = cfg as CharacterAttackSystemConfig;
            if (attackCfg == null)
            {
                Debug.LogError("Invalid config type for CharacterAttackSystem");
                return false;
            }
            
            _character = character;
            if (!_character.TryRegisterSystem<IAttackSystem>(this))
            {
                Debug.LogError("Failed to register CharacterAttackSystem");
                return false;
            }
            
            attackDamage = attackCfg.AttackDamage;
            attackDelay = attackCfg.AttackDelay;
            attackRange = attackCfg.AttackRange;
            
            Debug.Log($"AttackSystem initialized with config");
            return true;
        }
        
        public void Attack()
        {
            if (isAttacking) return;
            currentAttackCoroutine = StartCoroutine(AttackSequence());
        }
        
        public bool IsAttacking => isAttacking;
        public float AttackDamage => attackDamage;
        public float AttackRange => attackRange;
        
        private IEnumerator AttackSequence()
        {
            isAttacking = true;
            yield return new WaitForSeconds(attackDelay);
            PerformAttack();
            yield return new WaitForSeconds(0.1f);
            isAttacking = false;
        }
        
        private void PerformAttack()
        {
            Vector3 attackPosition = transform.position + transform.forward * attackRange + attackOffset;
            Collider[] hitColliders = Physics.OverlapSphere(attackPosition, attackRadius, targetLayers);
            
            foreach (Collider collider in hitColliders)
            {
                if (collider.gameObject == gameObject) continue;
                
                var hitSystem = collider.GetComponentInParent<ICharacterHitProcessingSystem>();
                if (hitSystem != null)
                {
                    hitSystem.ProcessHit(attackDamage);
                    Debug.Log($"Hit target: {collider.gameObject.name}");
                }
            }
        }
        
        private void OnDestroy()
        {
            if (currentAttackCoroutine != null)
                StopCoroutine(currentAttackCoroutine);
        }
    }
}