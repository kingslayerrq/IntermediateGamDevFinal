using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : BaseEnemy // Inherit from BaseEnemy
{
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackSpeed = 5f;
    [SerializeField] private float retreatDistance = 3f;
    [SerializeField] private float pauseAfterAttack = 1f;

    private Vector3 attackTargetPosition;
    public bool isAttackInProgress ;
    private Transform player;
    private Vector3 initialPosition;
    private float distanceToPlayer;

    private enum State
    {
        Idle,
        Aggro,
        Attack,
        Retreat
    }

    private State currentState;

    protected override void Start()
    {
        base.Start(); // Call the Start method of BaseEnemy
        player = GameObject.FindGameObjectWithTag("Player").transform;
        initialPosition = transform.position;
        currentState = State.Idle;
    }

    protected override void Update()
    {
        base.Update(); // Optionally call the Update method of BaseEnemy
        if (player == null) return;

        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
                HandleIdleState();
                break;
            case State.Aggro:
                HandleAggroState();
                break;
            case State.Attack:
                HandleAttackState();
                break;
            case State.Retreat:
                HandleRetreatState();
                break;
        }

        LayerMask platformLayerMask = LayerMask.GetMask("Platform");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, moveSpeed * Time.deltaTime, platformLayerMask);
        if (hit.collider != null)
        {
            Debug.Log("Raycast hit a platform.");
            HandleCollisionWithPlatform();
        }

    }


    void HandleCollisionWithPlatform()
    {
        // Handle the collision with the platform
        
        isAttackInProgress = false;
        currentState = State.Retreat;
    }


    void HandleIdleState()
    {
        if (distanceToPlayer < detectionRange)
        {
            currentState = State.Aggro;
        }
    }

    void HandleAggroState()
    {
        if (distanceToPlayer > detectionRange)
        {
            currentState = State.Idle;
        }
        else if (distanceToPlayer < attackRange)
        {
            currentState = State.Attack;
        }
    }

    void HandleAttackState()
    {
        if (!isAttackInProgress)
        {
            StartCoroutine(InitiateAttack());
        }
        else
        {
            AttackMovement();
        }
    }

    void HandleRetreatState()
    {
        transform.position = Vector3.MoveTowards(transform.position, initialPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, initialPosition) < retreatDistance)
        {
            currentState = State.Idle;
        }
    }

    IEnumerator InitiateAttack()
    {
        isAttackInProgress = true;
        yield return new WaitForSeconds(2f); // Wait for 2 seconds

        attackTargetPosition = player.position;
        AttackMovement();
    }

    void AttackMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, attackTargetPosition, attackSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, attackTargetPosition) < 0.1f)
        {
            StartCoroutine(PauseAfterAttack());
        }
    }

    IEnumerator PauseAfterAttack()
    {
        yield return new WaitForSeconds(pauseAfterAttack);
        isAttackInProgress = false; // Reset the attack flag
        currentState = State.Retreat; // Transition to Retreat state after pausing
    }

    
}
