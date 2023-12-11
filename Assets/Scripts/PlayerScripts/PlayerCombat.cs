using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerCombat : MonoBehaviour
{
    private Player player;
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

    // Used to calculate time holding down
    private float holdTime = 0;

    private void Awake()
    {
        player = GetComponent<Player>();
       
    }
    private void Start()
    {
    }
    private void Update()
    {
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
                // Zoom in ?
                Debug.Log("holding");
                holdTime += Time.unscaledDeltaTime;
                if (holdTime >= healKeyHold)
                {
                    heal(1);
                }
            }
            else
            {
                // Does the player become movable if they just recovered a health and the regen key was never let go?
                player.canMove = true;
                holdTime = 0;
            }
        }
        // When key lifted reset the hold time and player is movable
        if (Input.GetKeyUp(player.healKey))
        {
            player.canMove = true;
            holdTime = 0;
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

        //TODO: render slash



        //TODO: player knockback should only occur when the nail hits anything
        if (targetsHit.Length > 0)
        {
            int kbDirection = player.isFacingRight ? -1 : 1;
            Vector2 kbFrom = new Vector2(kbDirection, 0);
            player.playerRb.AddForce(kbFrom * knockbackForceSelf, ForceMode2D.Impulse);
            Debug.Log("self kb");

        }

    }


    void heal(int amount)
    {
        holdTime = 0;
        if (player.curGauge >= healResourceReq)
        {
            player.useResource(healResourceReq);
            player.gainHealth(amount);
        }
        
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
