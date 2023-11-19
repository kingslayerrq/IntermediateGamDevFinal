using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerCombat : MonoBehaviour
{

    #region Attack variables
    [SerializeField] private float attackRange;
    [SerializeField] private float playerKnockbackDistance;
    [SerializeField] private float playerKnockbackDuration;
    public Transform attackPoint;
    public LayerMask attackableLayers;
    #endregion

    private Player player;

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
        // detect targets hit
        Collider2D[] targetsHit = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, attackableLayers);
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

        if (!attackPoint)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    #endregion
}
