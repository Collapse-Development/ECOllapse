using UnityEngine;
using UnityEngine.AI;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.NPC.Configurations;

namespace _Project.Code.Features.NPC
{
    public class NPCSystem : MonoBehaviour, ICharacterSystem
    {
        [Header("Компоненты")]
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        
        [Header("Текущее состояние")]
        [SerializeField] private NPCType npcType;
        [SerializeField] private NPCState currentState;
        [SerializeField] private float currentHealth;
        [SerializeField] private bool isDead = false;
        
        private NPCConfig _config;
        private Transform _player;
        private _Project.Code.Features.Character.MB.Character _character;
        private float _alertTimer;
        private Vector3 _startPosition;
        private float _nextAttackTime;
        
        public NPCType Type => npcType;
        public NPCState State => currentState;
        public bool IsDead => isDead;
        
        public bool TryInitialize(_Project.Code.Features.Character.MB.Character character, CharacterSystemConfig cfg)
        {
            _config = cfg as NPCConfig;
            if (_config == null) return false;
            
            _character = character;
            _player = GameObject.FindGameObjectWithTag("Player")?.transform;
            _startPosition = transform.position;
            
            npcType = _config.npcType;
            currentHealth = _config.health;
            currentState = _config.startState;
            
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            if (agent != null) agent.speed = _config.speed;
            
            _character.TryRegisterSystem<INPCSystem>(this as INPCSystem);
            
            return true;
        }
        
        private void Update()
        {
            if (isDead) return;
            UpdateState();
            ExecuteState();
        }
        
        private void UpdateState()
        {
            if (_player == null) return;
            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            
            switch (npcType)
            {
                case NPCType.Hostile:
                    if (currentState != NPCState.Attack && distanceToPlayer < _config.detectionRange)
                        ChangeState(NPCState.Attack);
                    else if (currentState == NPCState.Attack && distanceToPlayer > _config.detectionRange * 1.5f)
                        ChangeState(NPCState.Idle);
                    break;
                    
                case NPCType.Neutral:
                    if (currentState == NPCState.Attack && distanceToPlayer > _config.detectionRange * 1.5f)
                        ChangeState(NPCState.Idle);
                    break;
                    
                case NPCType.Peaceful:
                    if (currentState != NPCState.Flee && distanceToPlayer < _config.fleeRange)
                        ChangeState(NPCState.Flee);
                    else if (currentState == NPCState.Flee && distanceToPlayer > _config.fleeRange * 2f)
                        ChangeState(NPCState.Idle);
                    break;
            }
            
            if (currentState == NPCState.Alert)
            {
                _alertTimer -= Time.deltaTime;
                if (_alertTimer <= 0) ChangeState(NPCState.Idle);
            }
        }
        
        private void ExecuteState()
        {
            switch (currentState)
            {
                case NPCState.Idle: IdleBehavior(); break;
                case NPCState.Alert: AlertBehavior(); break;
                case NPCState.Attack: AttackBehavior(); break;
                case NPCState.Flee: FleeBehavior(); break;
            }
        }
        
        private void IdleBehavior()
        {
            if (agent != null && agent.remainingDistance < 0.5f)
            {
                if (Random.Range(0, 100) < 2)
                {
                    Vector3 randomPoint = _startPosition + Random.insideUnitSphere * 5f;
                    randomPoint.y = transform.position.y;
                    agent.SetDestination(randomPoint);
                }
            }
            if (animator != null) animator.SetFloat("Speed", agent?.velocity.magnitude ?? 0);
        }
        
        private void AlertBehavior()
        {
            if (_player != null)
            {
                Vector3 direction = (_player.position - transform.position).normalized;
                direction.y = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
            }
            if (animator != null) animator.SetTrigger("Alert");
        }
        
        private void AttackBehavior()
        {
            if (_player == null) return;
            float distance = Vector3.Distance(transform.position, _player.position);
            
            if (distance < _config.attackRange)
            {
                if (Time.time >= _nextAttackTime)
                {
                    _nextAttackTime = Time.time + 1f;
                    PerformAttack();
                }
                if (animator != null) animator.SetTrigger("Attack");
            }
            else if (agent != null)
            {
                agent.SetDestination(_player.position);
            }
            if (animator != null) animator.SetFloat("Speed", agent?.velocity.magnitude ?? 0);
        }
        
        private void FleeBehavior()
        {
            if (_player == null || agent == null) return;
            Vector3 fleeDirection = (transform.position - _player.position).normalized;
            Vector3 fleePoint = transform.position + fleeDirection * 10f;
            agent.SetDestination(fleePoint);
            if (animator != null) animator.SetFloat("Speed", agent.velocity.magnitude);
        }
        
        private void PerformAttack()
        {
            if (_player == null) return;
            float finalDamage = _config.damage;
            
            switch (_config.mutationEffect)
            {
                case MutationEffect.Poisonous: finalDamage *= 0.8f; break;
                case MutationEffect.Gigantic: finalDamage *= 1.5f; break;
            }
            Debug.Log($"NPC атакует с уроном {finalDamage}");
        }
        
        public void TakeDamage(float damage, GameObject source)
        {
            if (isDead) return;
            currentHealth -= damage;
            
            if (npcType == NPCType.Neutral && source.CompareTag("Player"))
                ChangeState(NPCState.Attack);
            
            if (currentHealth <= 0) Die();
            else
            {
                ChangeState(NPCState.Alert);
                _alertTimer = 3f;
            }
        }
        
        private void Die()
        {
            isDead = true;
            currentState = NPCState.Dead;
            if (agent != null) agent.isStopped = true;
            if (animator != null) animator.SetTrigger("Die");
            DropLoot();
            Destroy(gameObject, 2f);
        }
        
        private void DropLoot()
        {
            if (_config.lootTable == null) return;
            foreach (var loot in _config.lootTable)
            {
                if (Random.Range(0f, 100f) <= loot.dropChance)
                {
                    int amount = Random.Range(loot.minAmount, loot.maxAmount + 1);
                    for (int i = 0; i < amount; i++)
                        Instantiate(loot.itemPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                }
            }
        }
        
        public void ChangeState(NPCState newState)
        {
            if (currentState == newState) return;
            currentState = newState;
            if (agent != null && newState == NPCState.Idle) agent.SetDestination(_startPosition);
        }
        
        public void AlertToPlayer()
        {
            if (npcType == NPCType.Peaceful) return;
            ChangeState(NPCState.Alert);
            _alertTimer = 5f;
        }
    }
    
    public interface INPCSystem : ICharacterSystem
    {
        NPCType Type { get; }
        NPCState State { get; }
        bool IsDead { get; }
        void TakeDamage(float damage, GameObject source);
        void AlertToPlayer();
    }
}