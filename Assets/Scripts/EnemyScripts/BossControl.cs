using System.Collections;
using UnityEngine;

public class BossControl : BaseEnemy
{
    private enum State
    {
        Sleep,
        FirstAttack,
        SecondAttack
    }

    [SerializeField] private float attackSpeed = 10f;
    [SerializeField] private float pauseDuration = 2f;
    [SerializeField] private int maxBossHP = 21;
    private State currentState = State.Sleep;
    private float pauseTimer = 0f;

    protected override void Start()
    {
        base.Start();
        curHealth = maxBossHP;
    }

    protected override void Update()
    {
        base.Update();

        switch (currentState)
        {
            case State.Sleep:
                SleepBehavior();
                break;

            case State.FirstAttack:
                FirstAttackBehavior();
                break;

            case State.SecondAttack:
                // Implement Second Attack Behavior here
                break;
        }
    }

    private void FirstAttackBehavior()
    {
        if (pauseTimer > 0)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }

        Move();
    }

    private void SleepBehavior()
    {
        if (curHealth < maxBossHP)
        {
            currentState = State.FirstAttack;
        }
    }

    protected override void Move()
    {
        // Move the boss
        float moveDirection = isMovingRight ? 1 : -1;
        transform.Translate(Vector2.right * moveDirection * attackSpeed * Time.deltaTime);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        // Additional Boss specific collision logic
        if (collision.gameObject.CompareTag("Wall"))
        {
            pauseTimer = pauseDuration; // pause after hitting the wall
        }
    }

    // Add other methods and specific behaviors as needed
}
