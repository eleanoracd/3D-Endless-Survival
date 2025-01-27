using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Line of Sight Settings")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float fieldOfViewAngle = 90f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float patrolSpeed = 2f;

    private Transform playerTransform;
    private NavMeshAgent navMeshAgent;
    private Transform[] patrolPoints;
    private int currentWaypointIndex;
    private bool isChasing;

    private Animator animator;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        DetectPlayer();

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        UpdateAnimation();
    }

    public void SetWaypoints(Transform[] newWaypoints)
    {
        patrolPoints = newWaypoints;
        currentWaypointIndex = 0;
    }

    private void DetectPlayer()
    {
        Collider[] detectedObjects = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        foreach (var obj in detectedObjects)
        {
            Transform target = obj.transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < fieldOfViewAngle / 2 || isChasing)
            {
                if (!Physics.Raycast(transform.position, directionToTarget, detectionRadius, obstacleLayer))
                {
                    playerTransform = target;
                    isChasing = true;
                    return;
                }
            }
        }

        if (isChasing)
        {
            isChasing = false;
            playerTransform = null;

            navMeshAgent.ResetPath();
            GoToNextPatrolPoint();
        }
    }

    private void ChasePlayer()
    {
        if (playerTransform == null) return;

        navMeshAgent.speed = chaseSpeed;
        navMeshAgent.SetDestination(playerTransform.position);
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        navMeshAgent.speed = patrolSpeed;

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolPoints.Length;
            navMeshAgent.SetDestination(patrolPoints[currentWaypointIndex].position);
        }
    }

    private void UpdateAnimation()
    {
        bool isMoving = navMeshAgent.velocity.magnitude > 0.1f;

        animator.SetBool("isMoving", isMoving);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void OnDisable()
    {
        isChasing = false;
        playerTransform = null;

        if (navMeshAgent != null)
        {
            navMeshAgent.ResetPath();
        }
    }
}
