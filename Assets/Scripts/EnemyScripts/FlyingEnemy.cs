using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [SerializeField] public float detectionRange = 5f;
    [SerializeField] public float attackRange = 1f;
    [SerializeField] public float moveSpeed = 2f;
    [SerializeField] public float attackSpeed = 5f;
    [SerializeField] public float retreatDistance = 3f;
    [SerializeField] private float pauseAfterAttack = 1f;

    private Vector3 attackTargetPosition;
    private bool isAttackInProgress = false;

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

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        initialPosition = transform.position;
        currentState = State.Idle;
    }

    void Update()
    {
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
        // Move back to initial position or away from the player
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

        // Get player's position at this moment
        attackTargetPosition = player.position;

        // Move towards the target position
        AttackMovement();
    }

    void AttackMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, attackTargetPosition, attackSpeed * Time.deltaTime);

        // Check if reached target position
        if (Vector3.Distance(transform.position, attackTargetPosition) < 0.1f)
        {
            StartCoroutine(PauseAfterAttack());
        }
    }

    IEnumerator PauseAfterAttack()
    {
        yield return new WaitForSeconds(pauseAfterAttack);
        Debug.Log("Pause");

        isAttackInProgress = false; // Reset the attack flag
        currentState = State.Retreat; // Transition to Retreat state after pausing
    }
}
