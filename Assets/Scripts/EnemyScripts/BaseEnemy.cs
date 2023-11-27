using UnityEngine;

public class BaseEnemy : MonoBehaviour, IDamageable
{
    [Header("Enemy Info")]
    [SerializeField] protected int attackDamage;
    public int maxHealth;
    public int curHealth;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float knockbackForce;


    protected LayerMask platformLayerMask;
    protected float checkGroundDist;
    [SerializeField] protected float checkGroundBuffer;
    protected CapsuleCollider2D enemyCollider;
    protected Rigidbody2D enemyRb;

    public bool movingRight;
    public bool isUnstoppable;

    protected virtual void Awake()
    {
        enemyCollider = GetComponent<CapsuleCollider2D>();
        enemyRb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        movingRight = Random.Range(0, 1) == 1 ? true : false;
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
        movingRight = !movingRight;
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
            int attackFromX = movingRight ? 1 : -1;
            Vector2 attackFrom = new Vector2(attackFromX, 1);
            p.takeDamage(attackDamage, attackFrom);
        }
    }
    public virtual void takeDamage(int damage, Vector2? from)
    {
        // Knockback enemy 
        if (!isUnstoppable)
        {
            Vector2 attackPos = from ?? Vector2.zero;
            enemyRb.AddForce(knockbackForce * attackPos, ForceMode2D.Impulse);
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

    
}
