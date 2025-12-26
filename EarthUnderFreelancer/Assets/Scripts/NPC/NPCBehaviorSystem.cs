using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.NPC
{
    public enum NPCState { Idle, Patrol, Work, Trade, Sleep, Social }
    
    public class NPCBehaviorSystem : MonoBehaviour
    {
        public NPCState currentState = NPCState.Idle;
        public float hunger = 50f;
        public float energy = 100f;
        public float social = 50f;
        
        [Header("Daily Routine")]
        public Vector3[] patrolPoints;
        private int currentPatrolIndex = 0;
        public float moveSpeed = 3f;
        
        [Header("Memory")]
        public Dictionary<string, float> playerRelationships = new Dictionary<string, float>();
        
        private void Update()
        {
            UpdateNeeds();
            ExecuteState();
        }
        
        private void UpdateNeeds()
        {
            hunger -= Time.deltaTime * 0.5f;
            energy -= Time.deltaTime * 0.3f;
            social -= Time.deltaTime * 0.2f;
            
            if (hunger < 20f) currentState = NPCState.Work; // Go find food
            else if (energy < 20f) currentState = NPCState.Sleep;
            else if (social < 20f) currentState = NPCState.Social;
        }
        
        private void ExecuteState()
        {
            switch (currentState)
            {
                case NPCState.Patrol:
                    if (patrolPoints != null && patrolPoints.Length > 0)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[currentPatrolIndex], moveSpeed * Time.deltaTime);
                        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex]) < 1f)
                            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                    }
                    break;
                case NPCState.Sleep:
                    energy += Time.deltaTime * 5f;
                    if (energy > 90f) currentState = NPCState.Idle;
                    break;
            }
        }
        
        public void InteractWithPlayer(string playerName)
        {
            if (!playerRelationships.ContainsKey(playerName)) playerRelationships[playerName] = 50f;
            playerRelationships[playerName] += 5f;
            social += 10f;
        }
    }
}
