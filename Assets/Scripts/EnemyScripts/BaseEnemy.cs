using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class BaseEnemy : MonoBehaviour, IDamageable
{
    [Header("Enemy Info")]
    [SerializeField] protected int attackDamage;
    public int maxHealth;
    public int curHealth;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float knockbackForce;
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackDuration ;
    [SerializeField] private float knockbackMultiplier ;

    [Header("Ground Check")]
    [SerializeField] protected LayerMask platformLayerMask;
    protected float checkGroundDist;
    [SerializeField] protected float checkGroundBuffer;
    protected CapsuleCollider2D enemyCollider;
    protected Rigidbody2D enemyRb;

    [Header("Status")]
    public bool isMovingRight;
    public bool isUnstoppable;
    public bool isGrounded;

    public PlayerAudioManager playerAudioManager;
    public UnityEvent onSlashEnemy = new UnityEvent();
    ///<summary>
    ///Get collider and rigidbody
    ///</summary>
    protected virtual void Awake()
    {
        enemyCollider = GetComponent<CapsuleCollider2D>();
        enemyRb = GetComponent<Rigidbody2D>();
        onSlashEnemy.AddListener(playerAudioManager.PlaySlashSFX);
    }

    ///<summary>
    ///Set Moving direction randomly
    ///Set Current health to Max
    ///</summary>
    protected virtual void Start()
    {
        
        isMovingRight = Random.Range(0, 1) == 1 ? true : false;
        curHealth = maxHealth;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    #region Enemy Methods
    protected virtual void Move()
    {
        Debug.Log("base enemy move");
    }
    protected virtual void Flip()
    {
        isMovingRight = !isMovingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    protected virtual void FlipSprite()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
    protected virtual void checkGrounded()
    {

    }
    #endregion

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // Change Direction, if we run into a wall
        if (collision.collider.CompareTag("Wall"))
        {
            Debug.Log("touched Wall!!");
            Flip();
        }
        // Damage Player, if we run into player
        if (collision.collider.CompareTag("Player"))
        {
            var p = collision.gameObject.GetComponent<Player>();
            int attackFromX = isMovingRight ? 1 : -1;
            Vector2 attackFrom = new Vector2(attackFromX, 1);
            p.takeDamage(attackDamage, attackFrom);
        }
    }
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        // Damage Player, if we stay contacting player
        if (collision.collider.CompareTag("Player"))
        {
            var p = collision.gameObject.GetComponent<Player>();
            int attackFromX = isMovingRight ? 1 : -1;
            Vector2 attackFrom = new Vector2(attackFromX, 1);
            p.takeDamage(attackDamage, attackFrom);
        }
    }
    #region Idamageable Methods
    public virtual void takeDamage(int damage, Vector2? from)
    {
        onSlashEnemy.Invoke();
        // Knockback enemy 
        if (!isUnstoppable)
        {
            Vector2 attackPos = from ?? Vector2.zero;
            StartCoroutine(KnockbackRoutine(attackPos * knockbackForce));
        }
        if (curHealth > damage)
        {
            curHealth -= damage;
            
        }
        else
        {
            Destroy(gameObject); // Destroy the enemy
        }
    }
    #endregion
    IEnumerator KnockbackRoutine(Vector2 knockback)
    {
        float elapsed = 0f;

        while (elapsed < knockbackDuration)
        {
            transform.position += (Vector3)(knockback * knockbackMultiplier) * Time.deltaTime / knockbackDuration;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
