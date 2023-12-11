using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Animator))]
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
    [SerializeField] private float attackAnimDuration;
    [SerializeField] private float missAttackOffset;
    [SerializeField] private float attackCoolDown;
    public Transform attackPoint;
    public event Action<bool> onAttack;

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
    [SerializeField] private float enrageDashForceMultiplier;
    [SerializeField] private float enrageDashDistanceMultiplier;

    private Animator bossAnimator;
    private EncounterTrigger encounterTrigger;

    public bool isAwake;
    public bool isInvincible;
    public bool isEnraged;
    public BossState curBossState;

    private SpriteRenderer bossSR;
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
        bossAnimator = GetComponent<Animator>();
        encounterTrigger = GetComponentInChildren<EncounterTrigger>();
        bossSR = GetComponent<SpriteRenderer>();
        encounterTrigger.onEncounterStart += startEncounter;
    }
    protected override void Start()
    {
        curHealth = maxHealth;
        isMovingRight = false;
        isEnraged = false;
        isAwake = false;
        curBossState = BossState.Init;
        
    }

    protected override void Update()
    {
        if(curBossState == BossState.ChasePlayer)
        {
            if (isMovingRight)
            {
                transform.localScale = new Vector2(-1, 1);
                transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.localScale = Vector2.one;
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            }
        }
        if (curHealth < maxHealth / 2 && !isEnraged)
        {
            Debug.Log("transform!!!");
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
    // Start the boss wake up anim
    void startEncounter(bool b)
    {
        isAwake = true;
        Debug.Log("boss triggered");
        StartCoroutine(waitForCameraZoom());
    }

    private IEnumerator waitForCameraZoom()
    {
        yield return new WaitForSecondsRealtime(2f);
        bossAnimator.SetTrigger("wakeUp");
        UpdateBossState(BossState.Spawn);
    }
    void HandleBossSpawn()
    {

        Debug.Log("Spawn");
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
        if (distToPlayer <= attackRange + missAttackOffset)
        {
            UpdateBossState(BossState.Attack);
            yield return null;
        }
        else
        {
            // Let Boss walk a bit
            yield return new WaitForSecondsRealtime(minChaseDuration);
            
            // Try to dash and slash, when far away
            if (distToPlayer >= (dashDistance + attackRange + missAttackOffset))
            {
                UpdateBossState(BossState.Dash);
                yield return null;
            }
            else
            {
                // When not enraged, either start the coroutine again, or enter snooze
                if(UnityEngine.Random.Range(0, 11) > 0)
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
        bossAnimator.SetTrigger("dash");
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
        bossAnimator.SetTrigger("dash");
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
        bossAnimator.SetTrigger("dash");
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
        onAttack?.Invoke(true);
        enemyRb.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(attackAnimDuration);
        enemyRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        yield return null;
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
        missAttackOffset *= 0.5f;
        dashForce *= enrageDashForceMultiplier;
        dashDistance *= enrageDashDistanceMultiplier;
        StartCoroutine(DoTransform());

    }
    // Invincible when transforming
    private IEnumerator DoTransform()
    {
        isInvincible = true;
        enemyRb.constraints = RigidbodyConstraints2D.FreezeAll;
        Debug.Log("transforming");
        bossSR.DOColor(Color.red, enrageTransformDuration);
        yield return new WaitForSecondsRealtime(enrageTransformDuration);
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
            if (UnityEngine.Random.Range(0, 3) > 0 && isGrounded)
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

    private void OnDestroy()
    {
        encounterTrigger.onEncounterStart -= startEncounter;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, dashDistance);
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawWireSphere(attackPoint.position, attackRange + missAttackOffset);
    }

}
