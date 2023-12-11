using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Player))]
public class PlayerCombat : MonoBehaviour
{
    private Player player;
    private PlayerAudioManager playerAudioManager;
    private HealAura healAura;

    [SerializeField] private int attackDamage;
    [SerializeField] private float slashCoolDown;
    [SerializeField] private float knockbackForceSelf;
    [SerializeField] private float resourceGainOnHit;
    [SerializeField] private float healResourceReq;
    [SerializeField] private float healKeyHold;
    #region Attack variables
    [SerializeField] private float attackRange;
    [SerializeField] private float playerKnockbackDistance;
    [SerializeField] private float playerKnockbackDuration;
    [SerializeField] private float pogoForce;
    [SerializeField] private float pogoYVelLimit;
    #region Attack Points
    public Transform attackPointHor;
    public Transform attackPointTop;
    public Transform attackPointBot;
    public Transform attackDir;
    #endregion
    public LayerMask attackableLayers;
    #endregion

    public event Action<int> OnHorSlash;
    public event Action<int> OnUpSlash;
    public event Action<int> OnDownSlash;

    public UnityEvent onSlashPlatform = new UnityEvent();
    public UnityEvent onSlashMiss = new UnityEvent();
    public UnityEvent onHeal = new UnityEvent();
    

    // Used to calculate time holding down
    private float holdTime = 0;
    private Coroutine zoomCoroutine;
    private void Awake()
    {
        player = GetComponent<Player>();
        playerAudioManager = GetComponent<PlayerAudioManager>();
        healAura = GetComponentInChildren<HealAura>();
       
    }
    private void Start()
    {
        onSlashPlatform.AddListener(playerAudioManager.PlaySlashWallSFX);
        onSlashMiss.AddListener(playerAudioManager.PlaySlashMissSFX);
        onHeal.AddListener(playerAudioManager.PlayHealSFX);
        onHeal.AddListener(healAura.LightUp);
    }
    private void Update()
    {
        Debug.Log(player.playerRb.velocity.y);
        #region Attack
        // Attack (animation determines how often we can detect the input of attack?)
        if (player.canMove && player.canAtk)
        {
            if (Input.GetKeyDown(player.atkKey))
            {
                attack();
            }
        }
        #endregion

        #region Heal
        if (player.isGrounded)
        {
            if (Input.GetKey(player.healKey) && player.curGauge >= healResourceReq && player.curHealth < player.maxHealth)
            {
                player.canMove = false;
                player.playerAnimator.SetBool("isWalkingAnim", false);
                // Zoom in ?
                if (player.playerVC.enabled)
                {
                    if (zoomCoroutine != null)
                    {
                        StopCoroutine(zoomCoroutine);
                    }
                    zoomCoroutine = StartCoroutine(Zoom(player.playerVCFOVShrunk, player.playerVCZoomDuration));
                }
                Debug.Log("holding");
                holdTime += Time.unscaledDeltaTime;
                if (holdTime >= healKeyHold)
                {
                    heal(1);
                }
            }
            else
            {
                // TODO: Refactor the condition so it doesnt trigger every frame!!!
                player.canMove = true;
                holdTime = 0;
                if (zoomCoroutine != null)
                {
                    StopCoroutine(zoomCoroutine);
                }
                zoomCoroutine = StartCoroutine(Zoom(player.playerVCFOV, player.playerVCZoomDuration));
            }
        }
        // When key lifted reset the hold time and player is movable
        if (Input.GetKeyUp(player.healKey))
        {
            player.canMove = true;
            holdTime = 0;
            if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
            }
            zoomCoroutine = StartCoroutine(Zoom(player.playerVCFOV, player.playerVCZoomDuration));
        }
        #endregion
    }

    void attack()
    {
        StartCoroutine("slashCD");
        //Debug.Log("attacking");
        // Get direction of attack
        int attackFromX = player.isFacingRight ? 1 : -1;
        // Knock enemy back horizontally
        Vector2 attackFrom = new Vector2(attackFromX, 0);
        if (Input.GetKey(player.upKey) || Input.GetKeyDown(player.upKey))
        {
            Debug.Log("upslash");
            OnUpSlash?.Invoke(1);
            attackDir = attackPointTop;
        }
        else if ((Input.GetKey(player.downKey) || Input.GetKeyDown(player.downKey)) && !player.isGrounded)
        {
            Debug.Log("downslash");
            OnDownSlash?.Invoke(-1);
            attackDir = attackPointBot;
        }
        else
        {
            player.playerAnimator.SetTrigger("isHorSlash");
            float dir = player.isFacingRight ? 1f : -1f;
            OnHorSlash?.Invoke(1);
            //Debug.Log("default attack direction");
            attackDir = attackPointHor;
        }

        // detect targets hit
        Collider2D[] targetsHit = Physics2D.OverlapCircleAll(attackDir.position, attackRange, attackableLayers);

        // Call TakeDamage on the enemy script
        foreach (Collider2D target in targetsHit)
        {
            Debug.Log("hit enemy");
            var enemy = target.GetComponent<BaseEnemy>();
            if (enemy)
            {
                player.gainResource(resourceGainOnHit);
                enemy.takeDamage(attackDamage, attackFrom);
            }
        }

  



        //TODO: player knockback should only occur when the nail hits anything
        if (targetsHit.Length > 0)
        {
            if (targetsHit[0].transform.tag == "Ground" || targetsHit[0].transform.tag == "Wall")
            {
                onSlashPlatform.Invoke();
            }
            int kbDirection;
            Vector2 kbFrom;
            kbDirection = player.isFacingRight ? -1 : 1;
            if (attackDir == attackPointHor)
            {
                kbFrom = new Vector2(kbDirection, 0);
            }
            else if (attackDir == attackPointBot)
            {
                Debug.Log("bothit!!!");
                kbFrom = new Vector2(kbDirection, pogoForce);
            }
            else
            {
                kbFrom = new Vector2(kbDirection, -1);
            }
            player.playerRb.AddForce(kbFrom * knockbackForceSelf, ForceMode2D.Impulse);
            // limit Y positive Vel
            float clampedYVelocity = Mathf.Min(player.playerRb.velocity.y, pogoYVelLimit);
            player.playerRb.velocity = new Vector2(player.playerRb.velocity.x, clampedYVelocity);
            //Debug.Log("self kb");
        }
        // When we miss
        else
        {
            onSlashMiss.Invoke();
        }

    }


    // Spend Resource -> Gain Heart -> Play Sound
    void heal(int amount)
    {
        holdTime = 0;
        if (player.curGauge >= healResourceReq)
        {
            onHeal.Invoke();
            player.useResource(healResourceReq);
            player.gainHealth(amount);
        }
        
    }

    private IEnumerator Zoom(float fov, float duration)
    {
        Debug.Log("in zoom");
        float curFov = player.playerVC.m_Lens.OrthographicSize;
        float step = fov - curFov;
        float time = 0;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            player.playerVC.m_Lens.OrthographicSize += step * Time.unscaledDeltaTime;
            yield return null;
        }
        player.playerVC.m_Lens.OrthographicSize = fov;
    }
    private IEnumerator slashCD()
    {
        player.canAtk = false;
        yield return new WaitForSecondsRealtime(slashCoolDown);
        player.canAtk = true;
    }
    #region Debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPointBot.position, attackRange);
        Gizmos.DrawWireSphere(attackPointTop.position, attackRange);
        Gizmos.DrawWireSphere(attackPointHor.position, attackRange);
    }
    #endregion
}
