using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // patrol the surrounding area
    // if the enemy sees the player, starts following

    [Header("Movement")]
    public float movementSpeed;
    public float rotationSpeed;

    [Header("Patrol")]
    public float patrolRadius;
    private Vector3 patrolCenter;
    public Vector3 patrolPoint;
    public float patrolPointTolerance = 0.1f;

    [Header("Detection")]
    public float detectionRadius;
    public float stoppingDistance = 0.3f;
    public bool playerSpotted = false;

    public Transform player;

    [Header("Combat")]
    public float hp;
    public float strength;
    public event Action<GameObject> OnKilled;

    private void Start()
    {
        patrolCenter = transform.position;
        patrolPoint = GetRandomPointInCircle();
    }

    [ContextMenu("Debug")]
    public void DebugDamage()
    {
        TakeDamage(100);
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if(hp <= 0)
        {
            OnKilled?.Invoke(gameObject);
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        PlayerCheck();
        if (playerSpotted)
        {
            Chase();
        }
        else
        {
            Patrol();
        }
    }

    private Vector3 GetRandomPointInCircle()
    {
        Vector3 randomPoint = patrolCenter + (UnityEngine.Random.insideUnitSphere * patrolRadius);
        return randomPoint;
    }

    private void Patrol()
    {
        if(Vector3.Distance(transform.position, patrolPoint) <= patrolPointTolerance)
        {
            patrolPoint = GetRandomPointInCircle();
            return;
        }
        RotateTowards(patrolPoint);
        transform.position = Vector3.MoveTowards(transform.position, patrolPoint, movementSpeed * Time.deltaTime);
    }

    private void Chase()
    {
        Vector3 targetPosition = player.position;
        RotateTowards(player.position);
        float distance = Vector3.Distance(transform.position, targetPosition);
        if(distance > stoppingDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        }
    }
    
    private void PlayerCheck()
    {
        bool inRange = Vector3.Distance(transform.position, player.position) <= detectionRadius;
        if(playerSpotted && !inRange)
        {
            patrolPoint = GetRandomPointInCircle();
        }
        playerSpotted = inRange;
    }

    private void RotateTowards(Vector3 lookTarget)
    {
        Vector3 direction = lookTarget - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;

        float targetYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float newYaw = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetYaw, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, newYaw, transform.rotation.eulerAngles.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow; // yellow = patrol
        Gizmos.DrawWireSphere(Application.isPlaying ? patrolCenter:transform.position, patrolRadius);

        Gizmos.color = Color.red; // red = detection
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}


