using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(DieSpawner))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(ShooterController))]
public class Enemy : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private ShooterController shooterController;
    [SerializeField]
    private Transform weaponTransform;

    [SerializeField]
    private EnemyType enemyType;

    Collider playerCollider;

    [SerializeField]
    float _health;

    [SerializeField]
    const float MAX_SHOOTING_ANGLE = 30f;

    [SerializeField]
    float DICE_DROP_RATE = 0.2f;

    DieSpawner dieSpawner;
    
    public event Action<Enemy> OnDeath;
    public event Action<Enemy> OnDestroyed;

    bool _isDead = false;

    public float Health
    {
        get
        {
            return _health;
        }
    }


    public enum MovementState
    {
        Idle,
        Chasing,
        Staying,
        Retreating,
        Investigating
    }

    public StateMachine<MovementState> movementStateMachine = new StateMachine<MovementState>(MovementState.Idle);

    // Only used for visualization in the editor
    public MovementState movementState = MovementState.Idle;

    #region UnityFunctions
    private void Awake()
    {
        dieSpawner = GetComponent<DieSpawner>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        shooterController = GetComponent<ShooterController>();
        shooterController.WeaponSlots = new ShooterController.WeaponSlot[] { new ShooterController.WeaponSlot(enemyType.usedWeapon) };
    }

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Collider>();
        _health = enemyType.health;


        CreateMovementStateTransitions();

        movementStateMachine.OnStateChange.AddListener(HandleMovementStateChanged);

        StartCoroutine(UpdateMovementStateCoroutine());
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }

#endregion
    

    #region MovementStateFunctions
    private IEnumerator UpdateMovementStateCoroutine()
    {
        while(true)
        {
            movementStateMachine.UpdateState();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void HandleMovementStateChanged(MovementState oldState, MovementState newState)
    {
        if (_isDead) return;

        if(newState == MovementState.Investigating)
        {
            navMeshAgent.destination = playerCollider.transform.position;
        }
        if(newState == MovementState.Idle || newState == MovementState.Staying)
        {
            StopMovement();
        }
        movementState = newState;
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
        movementStateMachine.AddStateTransition(MovementState.Investigating, MovementState.Chasing, IsPlayerVisible, true, false);
        movementStateMachine.AddStateTransition(MovementState.Investigating, MovementState.Idle, NavMeshAgentAtDestination, true, false);
        
    }

    void FixedUpdate()
    {
        if (!_isDead)
        {
            HandleMovementState();
            TryToShoot();
        }
    }

    private void HandleMovementState()
    {
        MoveDependingOnState();
    }

    private void TryToShoot()
    {
        if (movementStateMachine.currentState == MovementState.Chasing
            || movementStateMachine.currentState == MovementState.Staying)
        {
            if(shooterController.IsOutOfAmmo())
            {
                shooterController.ReloadWeapon();
            }
            else if (DistanceToPlayer() < shooterController.CurrentWeapon.weaponRange.y 
                && (Mathf.Abs(Vector3.Angle(transform.forward, -weaponTransform.position + playerCollider.transform.position)) < MAX_SHOOTING_ANGLE))
            {
                Quaternion q = Quaternion.LookRotation(-weaponTransform.position + playerCollider.transform.position, Vector3.up);
                shooterController.FireWeapon(weaponTransform.position, q);
            }
        }
    }

    private void MoveDependingOnState()
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
            case MovementState.Investigating:
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

    #endregion

    #region Utilities
    private float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, playerCollider.transform.position);
    }

    private bool IsPlayerTooClose()
    {
        return DistanceToPlayer() < enemyType.preferredDistance * 0.5f;
    }

    private bool IsPlayerCloseEnough()
    {
        return DistanceToPlayer() <= enemyType.preferredDistance;
    }

    private bool NavMeshAgentAtDestination()
    {
        return navMeshAgent.remainingDistance <= 0.01f;
    }

    private bool IsPlayerVisible()
    {
        RaycastHit hit;

        LayerMask nonEnemyLayerMask = ~LayerMask.GetMask("Enemy");

        Vector3 direction = -transform.position + playerCollider.transform.position;
        Physics.Raycast(transform.position, direction, out hit, enemyType.visionRange, nonEnemyLayerMask);

        return hit.collider == playerCollider;
    }
    #endregion

    #region Damage
    public void DamageHealth(float damage)
    {
        if (_isDead)
            return;

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
        _isDead = true;
        navMeshAgent.enabled = false;
        shooterController.enabled = false;
        
        StartCoroutine(SimpleDeathAnim());

        OnDeath?.Invoke(this);
    }

    private IEnumerator SimpleDeathAnim()
    {
        float duration = 0.5f;
        float progress = 0f;
        Quaternion initialRot = transform.rotation;
        Vector3 initialPos = transform.position;
        Quaternion targetRot = initialRot * Quaternion.Euler(0, 0, 90);
        Vector3 targetPos = initialPos - new Vector3(0, 0.2f, 0);

        while (progress < 1f)
        {
            transform.rotation = Quaternion.Slerp(initialRot, targetRot, progress);
            transform.position = Vector3.Slerp(initialPos, targetPos, progress);
            yield return null;
            progress += Time.deltaTime / duration;
        }

        CheckIfDiceDrop();
        Destroy(gameObject);
    }

    private void CheckIfDiceDrop()
    {
        if (UnityEngine.Random.value <= DICE_DROP_RATE)
        {
            dieSpawner.SpawnRandomDice();
        }
    }
    #endregion
}
