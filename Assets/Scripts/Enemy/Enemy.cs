using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    public EnemyType enemyType;

    Collider playerCollider;

    float _health;

    public enum MovementState
    {
        Idle,
        Chasing,
        Staying,
        Retreating
    }


    public MovementState movementState = MovementState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Collider>();
        _health = enemyType.health;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMovementState();
    }

    private void UpdateMovementState()
    {
        CheckStateProgression();
        ActDependingOnState();
    }

    private void ActDependingOnState()
    {
        switch (movementState)
        {
            case MovementState.Idle:
                StopMovement();
                break;
            case MovementState.Chasing:
                MoveTowardsPlayer();
                break;
            case MovementState.Staying:
                StopMovement();
                LookAtPlayer();
                break;
            case MovementState.Retreating:
                MoveAwayFromPlayer();
                break;
        }
    }

    private void LookAtPlayer()
    {
        Vector3 dir = playerCollider.transform.position - transform.position;
        dir.y = 0;//This allows the object to only rotate on its y axis
        Quaternion rot = Quaternion.LookRotation(dir);
        float angleDiff = Quaternion.Angle(transform.rotation, rot);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, navMeshAgent.angularSpeed * Time.fixedDeltaTime / angleDiff);
    }

    private void StopMovement()
    {
        navMeshAgent.SetDestination(transform.position);
    }

    private void MoveAwayFromPlayer()
    {
        navMeshAgent.isStopped = false;

        Vector3 toPlayer = (-transform.position + playerCollider.transform.position).normalized;
        Vector3 optimalPosition = playerCollider.transform.position - toPlayer * enemyType.preferredDistance * 0.9f;

        navMeshAgent.SetDestination(optimalPosition);
    }

    private void MoveTowardsPlayer()
    {
        navMeshAgent.isStopped = false;
        Vector3 toPlayer = (-transform.position + playerCollider.transform.position).normalized;
        navMeshAgent.SetDestination(playerCollider.transform.position - toPlayer.normalized * enemyType.preferredDistance);
    }

    private void CheckStateProgression()
    {
        switch (movementState)
        {
            case MovementState.Idle:
                if (IsPlayerVisible())
                {
                    movementState = MovementState.Chasing;
                }
                break;
            case MovementState.Chasing:
                if (!IsPlayerVisible())
                {
                    movementState = MovementState.Idle;
                }
                else if (IsPlayerCloseEnough())
                {
                    movementState = MovementState.Staying;
                }
                break;
            case MovementState.Staying:
                if (!IsPlayerVisible())
                {
                    movementState = MovementState.Idle;
                }
                else if (!IsPlayerCloseEnough())
                {
                    movementState = MovementState.Chasing;
                }
                else if (IsPlayerTooClose())
                {
                    movementState = MovementState.Retreating;
                }
                break;
            case MovementState.Retreating:
                if (!IsPlayerVisible())
                {
                    movementState = MovementState.Idle;
                }
                else if (!IsPlayerTooClose())
                {
                    movementState = MovementState.Staying;
                }
                break;
        }
    }

    private bool IsPlayerTooClose()
    {
        return DistanceToPlayer() < enemyType.preferredDistance * 0.5f;
    }

    private bool IsPlayerCloseEnough()
    {
        return DistanceToPlayer() <= enemyType.preferredDistance;
    }

    private float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, playerCollider.transform.position);
    }

    private bool IsPlayerVisible()
    {
        RaycastHit hit;

        LayerMask nonEnemyLayerMask = ~LayerMask.GetMask("Enemy");

        Vector3 direction = -transform.position + playerCollider.transform.position;
        Physics.Raycast(transform.position, direction, out hit, enemyType.visionRange, nonEnemyLayerMask);

        return hit.collider == playerCollider;
    }

    public float Health
    {
        get
        {
            return _health;
        }
    }
    public void DamageHealth(float damage)
    {
        _health -= damage;
        if(_health <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        Destroy(gameObject);
        //maybe throw event if other stuff needs to know about enemy death
    }
}
