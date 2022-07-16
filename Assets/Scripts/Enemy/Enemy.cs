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
        Retreating,
        Investigating
    }

    public StateMachine<MovementState> movementStateMachine = new StateMachine<MovementState>(MovementState.Idle);



    public MovementState movementState = MovementState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Collider>();
        _health = enemyType.health;

        CreateMovementStateTransitions();

        movementStateMachine.OnStateChange.AddListener(HandleMovementStateChanged);
    }

    private void HandleMovementStateChanged(MovementState oldState, MovementState newState)
    {
        if(newState == MovementState.Investigating)
        {
            navMeshAgent.destination = playerCollider.transform.position;
        }
        if(newState == MovementState.Idle || newState == MovementState.Staying)
        {
            StopMovement();
        }
    }

    private void CreateMovementStateTransitions()
    {
        // From idle
        movementStateMachine.AddStateTransition(MovementState.Idle, MovementState.Chasing, IsPlayerVisible, true, false);
        // From chasing
        movementStateMachine.AddStateTransition(MovementState.Chasing, MovementState.Staying, IsPlayerCloseEnough, true, true);
        movementStateMachine.AddStateTransition(MovementState.Chasing, MovementState.Investigating, IsPlayerVisible, false, false);
        // From staying
        movementStateMachine.AddStateTransition(MovementState.Staying, MovementState.Retreating, IsPlayerTooClose, true, true);
        movementStateMachine.AddStateTransition(MovementState.Staying, MovementState.Investigating, IsPlayerVisible, false, false);
        // From retreating
        movementStateMachine.AddStateTransition(MovementState.Retreating, MovementState.Investigating, IsPlayerVisible, false, false);
        // From investigating
        movementStateMachine.AddStateTransition(MovementState.Investigating, MovementState.Idle, InvestigationFinished, true, false);
        movementStateMachine.AddStateTransition(MovementState.Investigating, MovementState.Chasing, IsPlayerVisible, true, false);

    }

    private bool InvestigationFinished()
    {
        return navMeshAgent.remainingDistance <= 0.01f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        movementState = movementStateMachine.currentState;
        UpdateMovementState();
    }

    private void UpdateMovementState()
    {
        movementStateMachine.UpdateState();
        ActDependingOnState();
    }

    private void ActDependingOnState()
    {
        switch (movementStateMachine.currentState)
        {
            case MovementState.Idle:
                break;
            case MovementState.Chasing:
                MoveTowardsPlayer();
                break;
            case MovementState.Staying:
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

    /*private void CheckStateProgression()
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
    }*/

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
        //maybe throw event if other stuff needs to know about enemy death
        //do death animation
        Destroy(gameObject);
       
    }
}
