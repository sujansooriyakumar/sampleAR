using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed;
    public float rotationSpeed;

    [Header("Patrol")]
    public float patrolRadius;
    private Vector3 patrolCenter;
    public Vector3 patrolPoint;
    public bool playerSpotted;
    public float patrolPointTolerance = 0.1f;

    [Header("Detection")]
    public float detectionRadius;
    public float stoppingDistance = 0.3f;

    public Transform player;


    private void Start()
    {
        patrolCenter = transform.position;
        patrolPoint = GetRandomPointInCircle();
    }

    private Vector3 GetRandomPointInCircle()
    {
        Vector3 randomPoint = patrolCenter + (UnityEngine.Random.insideUnitSphere * patrolRadius);
        return randomPoint;
    }

    private void Update()
    {
        PlayerCheck();
        if (playerSpotted)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
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

    private void ChasePlayer()
    {
        Vector3 targetPosition = player.position;
        RotateTowards(player.position);

        float distance = Vector3.Distance(transform.position, targetPosition);

        if(distance > stoppingDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        }
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

    private void RotateTowards(Vector3 target)
    {
        Vector3 direction = target -transform.position;
        if (direction.sqrMagnitude < 0.001f) return;

        float yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float horizontalDistance = new Vector2(direction.x, direction.z).magnitude;
        float pitch = 0f;
        pitch = -Mathf.Atan2(direction.y, horizontalDistance) * Mathf.Rad2Deg;
        pitch = Mathf.Clamp(pitch, -45f, 45f);

        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, patrolRadius); // draw a sphere to denote patrol radius

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // draw a sphere to denote detection radius
    }
}


