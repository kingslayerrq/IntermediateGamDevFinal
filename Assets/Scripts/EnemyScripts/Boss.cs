using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Boss : BaseEnemy
{

    [SerializeField] private float jumpForce;

    [Header("Target")]
    public Player player;
    [SerializeField] private float distToPlayer;

    [Header("Chase Player Variables")]
    [SerializeField] private float minChaseDuration;

    [Header("Attack Variables")]
    [SerializeField] private float attackRange;
    [SerializeField] private float missAttackOffset;
    [SerializeField] private float attackCoolDown;

    [Header("Snooze Variable")]
    [SerializeField] private float snoozeDuration;


    [Header("Dash Variable")]
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    [SerializeField] private float enragedDashCoolDown;

    [Header("Enrage Stats")]
    [SerializeField] private float enrageTransformDuration;
    [SerializeField] private float enrageSpeedMultiplier;
    [SerializeField] private int enrageDmgMultiplier;

    public bool isInvincible;
    public bool isEnraged;
    public BossState curBossState;

    public enum BossState
    {
        Spawn,
        Snooze,
        ChasePlayer,
        Attack,
        Dash,
        UseAbility,
        Transform,
        Enrage,
        Death,
        Init
    }

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        curHealth = maxHealth;
        isEnraged = false;
        curBossState = BossState.Init;
        UpdateBossState(BossState.Spawn);
    }

    protected override void Update()
    {
        if(curBossState == BossState.ChasePlayer)
        {
            if (isMovingRight)
            {
                transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            }
        }
        if (curHealth < maxHealth / 2 && !isEnraged)
        {
            UpdateBossState(BossState.Transform);
        }


        // isMoving Right, isGrounded
        UpdateBossStatus();

        

    }

    void UpdateBossState(BossState newState)
    {
        if(newState != curBossState)
        {
            curBossState = newState;
            switch(newState)
            {
                case BossState.Init:
                    break;
                case BossState.Spawn:
                    HandleBossSpawn();
                    break;
                case BossState.ChasePlayer:
                    HandleChasePlayer();
                    break;
                case BossState.Snooze:
                    HandleSnooze();
                    break;
                case BossState.Attack:
                    HandleAttack();
                    break;
                case BossState.Dash:
                    HandleDash();
                    break;
                case BossState.UseAbility:
                   
                    break;
                case BossState.Transform:
                    HandleTransform();
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
    
    void UpdateBossStatus()
    {
        isMovingRight = player.transform.position.x >= transform.position.x;
        distToPlayer = checkHorDiff();
        checkGrounded();
    }
    protected override void checkGrounded()
    {
        checkGroundDist = enemyCollider.size.y * 0.5f + checkGroundBuffer;
        RaycastHit2D hit = Physics2D.CapsuleCast(transform.position, enemyCollider.size, CapsuleDirection2D.Vertical, 90f, -Vector2.up, checkGroundDist, platformLayerMask);
        if (hit)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    // Spawn the boss -> ChasePlayer
    #region Spawn State
    void HandleBossSpawn()
    {
        // Initialize Boss
        //Debug.Log("Spawn");
        UpdateBossState(BossState.ChasePlayer);
    }
    #endregion

    #region Snooze State
    void HandleSnooze()
    {
        StartCoroutine(Snooze());
    }

    private IEnumerator Snooze()
    {
        //Debug.Log("Snoozing");
        yield return new WaitForSecondsRealtime(snoozeDuration);
        UpdateBossState(BossState.ChasePlayer);
    }

    #endregion

    // Move towards player attack if can, 
    #region Chase Player State
    void HandleChasePlayer()
    {
        Debug.Log("Chasing");
        StartCoroutine(EndChase());
    }

    private IEnumerator EndChase()
    {
        if (distToPlayer <= attackRange)
        {
            UpdateBossState(BossState.Attack);
            yield return null;
        }
        else
        {
            // Let Boss walk a bit
            yield return new WaitForSecondsRealtime(minChaseDuration);
            
            // Try to dash and slash, when far away
            if (distToPlayer >= (dashDistance + attackRange))
            {
                UpdateBossState(BossState.Dash);
                yield return null;
            }
            else
            {
                // When not enraged, either start the coroutine again, or enter snooze
                if(Random.Range(0, 11) > 0)
                {
                    StartCoroutine(EndChase());
                    yield return null;
                }
                else
                {
                    UpdateBossState(BossState.Snooze);
                    yield return null;
                }
            }
        }

    }
    #endregion

    // Dash based on state -> ChasePlayer
    #region Dash State
    void HandleDash()
    {
        if (isEnraged)
        {
            StartCoroutine(Dash());
        }
        else
        {
            StartCoroutine(EnragedDash());
        }

        // Attack when close
        if(distToPlayer <= (attackRange + missAttackOffset))
        {
            UpdateBossState(BossState.Attack);
        }
        else
        {
            UpdateBossState(BossState.ChasePlayer);
        }
        
    }

    private IEnumerator Dash()
    {
        var origG = enemyRb.gravityScale;
        enemyRb.gravityScale = 0;
        if (isMovingRight)
        {
            enemyRb.velocity = Vector2.right * dashForce;
        }
        else
        {
            enemyRb.velocity = Vector2.left * dashForce;
        }
        yield return new WaitForSeconds(dashDuration);
        enemyRb.gravityScale = origG;
        player.playerRb.velocity = Vector2.zero;
    }

    private IEnumerator EnragedDash()
    {
        var origG = enemyRb.gravityScale;
        enemyRb.gravityScale = 0;
        if (isMovingRight)
        {
            enemyRb.velocity = Vector2.right * dashForce;
        }
        else
        {
            enemyRb.velocity = Vector2.left * dashForce;
        }
        yield return new WaitForSeconds(dashDuration);
        player.playerRb.velocity = Vector2.zero;
        yield return new WaitForSecondsRealtime(enragedDashCoolDown);
        if (isMovingRight)
        {
            enemyRb.velocity = Vector2.right * dashForce;
        }
        else
        {
            enemyRb.velocity = Vector2.left * dashForce;
        }
        yield return new WaitForSeconds(dashDuration);
        enemyRb.gravityScale = origG;
        player.playerRb.velocity = Vector2.zero;
    }
    #endregion

    #region Attack State
    void HandleAttack()
    {
        //Debug.Log("attack state");
        StartCoroutine(DoAttack());
    }
    private IEnumerator DoAttack()
    {
        //Debug.Log("Doing attack");
        // Play Regular attack anim?
        yield return null;
        yield return new WaitForSecondsRealtime(attackCoolDown);
        if (distToPlayer > attackRange + missAttackOffset)
        {
            if (isEnraged)
            {
                // When enraged try to dash
                UpdateBossState(BossState.Dash);

            }
            else
            {
                // Approach player with grace
                UpdateBossState(BossState.ChasePlayer);
            }
        }
        else
        {
            StartCoroutine(DoAttack());
            //Debug.Log("Ending atk coroutine");
        }
    }
    #endregion

    #region Transform State
    // Set the value for Boss after transform
    void HandleTransform()
    {
        isEnraged = true;
        moveSpeed *= enrageSpeedMultiplier;
        attackDamage *= enrageDmgMultiplier;
        StartCoroutine(DoTransform());

    }
    // Invincible when transforming
    private IEnumerator DoTransform()
    {
        isInvincible = true;
        enemyRb.constraints = RigidbodyConstraints2D.FreezeAll;
        Debug.Log("transforming");
        yield return new WaitForSeconds(enrageTransformDuration);
        enemyRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        Debug.Log("Done transforming");
        isInvincible = false;
        UpdateBossState(BossState.ChasePlayer);
    }
    #endregion


    #region Regular Methods
    public void ReactPlayerJump()
    {
        if(curBossState == BossState.ChasePlayer)
        {
            //Debug.Log("React to jump");
            if (Random.Range(0, 3) > 0 && isGrounded)
            {
                StartCoroutine(Jump());
            }
        }
        else
        {
            //Debug.Log("not in chase !!");
        }
    }
    float checkHorDiff()
    {
        return Mathf.Abs(transform.position.x - player.transform.position.x);
    }
    IEnumerator Jump()
    {
        yield return new WaitForEndOfFrame();
        enemyRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    #endregion
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }

    public override void takeDamage(int damage, Vector2? from)
    {
        Debug.Log("Boss got hit");
        if (!isInvincible)
        {
            base.takeDamage(damage, from);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, dashDistance);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
