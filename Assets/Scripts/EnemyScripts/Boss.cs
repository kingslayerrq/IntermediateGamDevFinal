using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : BaseEnemy
{
    public Player player;
    [SerializeField] private float distToPlayer;
    [SerializeField] private float attackRange;

    public bool isInvincible;

    [SerializeField] private float enrageTransformDuration;
    [SerializeField] private float enrageSpeedMultiplier;
    [SerializeField] private int enrageDmgMultiplier;
    public BossState curBossState;
    public enum BossState
    {
        Spawn,
        ChasePlayer,
        Attack,
        Enrage,
        Death
    }

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (curHealth < maxHealth / 2)
        {
            UpdateBossState(BossState.Enrage);
        }
    }

    void UpdateBossState(BossState newState)
    {
        if(newState != curBossState)
        {
            curBossState = newState;
            switch(newState)
            {
                case BossState.Spawn:
                    break;
                case BossState.ChasePlayer:
                    break;
                case BossState.Attack:
                    break;
                case BossState.Enrage:
                    break;
                case BossState.Death:
                    break;
                default:
                    break;
            }
        }
    }
    
    void HandleBoss()
    {
        
    }

    void HandleBossSpawn()
    {
        // Initialize Boss

        UpdateBossState(BossState.ChasePlayer);
    }

    void HandleChasePlayer()
    {
        if(Vector2.Distance(transform.position, player.transform.position) <= attackRange)
        {
            UpdateBossState(BossState.Attack);
        }
    }

    void HandleAttack()
    {
        StartCoroutine(DoAttack());
    }
    private IEnumerator DoAttack()
    {
        yield return null;
        if (Vector2.Distance(transform.position, player.transform.position) > attackRange)
        {
            UpdateBossState(BossState.ChasePlayer);
        }
        else
        {
            StartCoroutine(DoAttack());
        }
    }
    void HandleTransform()
    {
        moveSpeed *= enrageSpeedMultiplier;
        attackDamage *= enrageDmgMultiplier;

    }
    private IEnumerator DoTransform()
    {
        Debug.Log("transforming");
        yield return new WaitForSeconds(enrageTransformDuration);
        Debug.Log("Done transforming");
    }
}
