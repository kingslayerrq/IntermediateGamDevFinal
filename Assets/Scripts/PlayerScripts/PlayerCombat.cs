using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerCombat : MonoBehaviour
{
    private Player player;
    [SerializeField] private int attackDamage;
    [SerializeField] private float knockbackForceSelf;

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
        int attackFromX = player.isFacingRight ? 1 : -1;
        // Knock enemy back horizontally
        Vector2 attackFrom = new Vector2(attackFromX, 0);
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
            Debug.Log("hit enemy");
            var enemy = target.GetComponent<BaseEnemy>();
            if (enemy)
            {
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
