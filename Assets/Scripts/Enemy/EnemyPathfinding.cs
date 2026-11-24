using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Makes an enemy continuously find and follow the Player using a baked NavMesh.
/// Add this component to the Enemy object.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float repathInterval = 0.15f;
    [SerializeField] private float stoppingDistance = 1.5f;

    private NavMeshAgent agent;
    private float repathTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
    }

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        if (target == null)
        {
            Debug.LogWarning("EnemyPathfinding could not find a target. Assign Target or tag the player as Player.");
        }
    }

    private void Update()
    {
        if (target == null || !agent.isOnNavMesh)
        {
            return;
        }

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            agent.SetDestination(target.position);
            repathTimer = repathInterval;
        }
    }
}
