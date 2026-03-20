using UnityEngine;
using _Project.Code.Features.NPC.Configurations;

namespace _Project.Code.Features.NPC
{
    public class NPCSpawner : MonoBehaviour
    {
        [SerializeField] private NPCConfig npcConfig;
        [SerializeField] private int spawnCount = 1;
        [SerializeField] private float spawnRadius = 10f;
        
        private void Start()
        {
            SpawnNPCs();
        }
        
        public void SpawnNPCs()
        {
            for (int i = 0; i < spawnCount; i++)
            {
                SpawnNPC();
            }
        }
        
        private void SpawnNPC()
        {
            if (npcConfig == null) return;
            
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * spawnRadius;
            spawnPos.y = transform.position.y;
            
            GameObject npcGO = new GameObject($"NPC_{npcConfig.npcType}_{Random.Range(0, 1000)}");
            npcGO.transform.position = spawnPos;
            
            // Добавляем компоненты
            NPCSystem npcSystem = npcGO.AddComponent<NPCSystem>();
            
            // TODO: добавить NavMeshAgent
            // NavMeshAgent agent = npcGO.AddComponent<NavMeshAgent>();
            
            // TODO: добавить коллайдер и модель
            // SphereCollider collider = npcGO.AddComponent<SphereCollider>();
            // collider.radius = 0.5f;
            
            // Инициализируем через CharacterBuilder
            // Character character = npcGO.AddComponent<Character>();
            // character.TryRegisterSystem<INPCSystem>(npcSystem);
            
            Debug.Log($"Спавн NPC: {npcConfig.npcType} на позиции {spawnPos}");
        }
    }
}