using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    [Header("Patrolling")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    [Header("Attacking")]
    public Vector3 timeBetweenAttacks;
    bool alreadyAttacked;

    [Header("States")]
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
