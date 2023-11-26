using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerCombat : MonoBehaviour
{
    private Player player;

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
        if (Input.GetKeyDown(player.atkKey))
        {
            attack();
        }
        #endregion
    }

    void attack()
    {
        Debug.Log("attacking");
        // Get direction of attack
        if (Input.GetKey(player.upKey) || Input.GetKeyDown(player.upKey))
        {
            attackDir = attackPointTop;
        }
        else if (Input.GetKey(player.downKey) || Input.GetKeyDown(player.downKey))
        {
            attackDir = attackPointBot;
        }
        else
        {
            Debug.Log("default attack direction");
            attackDir = attackPointHor;
        }
        // detect targets hit
        Collider2D[] targetsHit = Physics2D.OverlapCircleAll(attackDir.position, attackRange, attackableLayers);

        // Call TakeDamage on the enemy script
        foreach (Collider2D target in targetsHit)
        {
            EnemyControl enemy = target.GetComponent<EnemyControl>();
            if (enemy != null)
            {
                enemy.TakeDamage(); 
            }
        }

        //TODO: render slash



        //player knockback should only occur when the nail hits something
        if (targetsHit.Length > 0)
        {
            float endX;
            if (player.isFacingRight)
            {
                endX = transform.position.x - playerKnockbackDistance;
            }
            else
            {
                endX = transform.position.x + playerKnockbackDistance;
            }
            transform.DOMoveX(endX, playerKnockbackDuration);
        }

        foreach (Collider2D target in targetsHit)
        {
            if (target.gameObject.layer == 3)
            {
                Debug.Log("we hit a platform");
            }
            else if (target.gameObject.layer == 7)
            {
                Debug.Log("we hit an enemy");
            }
        }
    }


    #region Debug
    private void OnDrawGizmosSelected()
    {

        if (!attackDir)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackDir.position, attackRange);
    }
    #endregion
}
