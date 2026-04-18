using System;
using UnityEngine;
using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.NPC.Configurations
{
    [CreateAssetMenu(fileName = "NPCConfig", menuName = "ECOllapse/NPC/NPCConfig")]
    public class NPCConfig : CharacterSystemConfig
    {
        public NPCType npcType = NPCType.Neutral;
        public NPCState startState = NPCState.Idle;
        
        public float health = 100f;
        public float damage = 10f;
        public float attackRange = 2f;
        public float detectionRange = 10f;
        public float fleeRange = 5f;
        public float speed = 3f;
        
        public MutationEffect mutationEffect = MutationEffect.None;
        public LootItem[] lootTable;
        
        public override Type CharacterSystemType => typeof(NPCSystem);
    }
    
    public enum MutationEffect
    {
        None,
        ThickSkin,
        Poisonous,
        SharpSpikes,
        Camouflage,
        Gigantic,
        ArmoredShell
    }
    
    [Serializable]
    public class LootItem
    {
        public GameObject itemPrefab;
        [Range(0, 100)] public float dropChance = 50f;
        public int minAmount = 1;
        public int maxAmount = 1;
    }
}