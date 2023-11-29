using System;
using UnityEngine;


public class Player : MonoBehaviour, IDamageable, IResourceGauge
{
    [Header("Player's Current Info")]
    public int curHealth;
    public int maxHealth;
    public float maxSlowGauge;
    public float curSlowGauge;
    [SerializeField] private float knockbackForce;
    public playerState curPlayerState;
    public bool isGrounded;
    public bool isFacingRight;
    public bool isUnstoppable;
    public bool isDashing;

    [Header("Player's KeyBinds")]
    public KeyCode upKey = KeyCode.UpArrow;
    public KeyCode downKey = KeyCode.DownArrow;
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode jmpKey = KeyCode.Z;
    public KeyCode atkKey = KeyCode.X;
    public KeyCode dashKey = KeyCode.C;
    public KeyCode timeSlowKey = KeyCode.V;

    [Header("Ground Check")]
    public LayerMask groundLayers;
    private float checkGroundDist;
    [Tooltip("Buffer distance for Ground Check")][SerializeField] private float checkGroundBuffer;
    [HideInInspector] public CapsuleCollider2D playerCollider;
    [HideInInspector] public Rigidbody2D playerRb;

    // Events to trigger
    public event Action<int> onHitUI;
    public event Action<int> onRecoverUI;
    public event Action<float> onEnergySpentUI;
    public event Action<float> onEnergyRecoverUI;




    public enum playerState
    {
        IDLE,               // has to be Grounded
        RUNNING,            // has to be Grounded
        AIRBORNE,
        FOCUS,              // has to be Grounded
        ATTACK,
        HURT,
        DEAD
    }
    private void Awake()
    {
        playerCollider = GetComponent<CapsuleCollider2D>();
        playerRb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        isFacingRight = true;
        isDashing = false;
        curHealth = maxHealth;
        curSlowGauge = maxSlowGauge;
    }
    private void Update()
    {
        checkGrounded();
        
        
    }

    #region Ground Check
    void checkGrounded()
    {
        checkGroundDist = playerCollider.size.y * 0.5f + checkGroundBuffer;
        RaycastHit2D hit = Physics2D.CapsuleCast(transform.position, playerCollider.size, CapsuleDirection2D.Vertical, 90f, -Vector2.up, checkGroundDist, groundLayers);
        if (hit)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    #endregion

    #region IDamageable Methods
    public void takeDamage(int damage, Vector2? from = null)
    {
        Debug.Log("took " + damage + " damage!");
        onHitUI?.Invoke(damage);
        // Knock Player back a little from the position of attacker
        if (!isUnstoppable)
        {
            Vector2 attackPos = from ?? Vector2.zero;
            playerRb.AddForce(knockbackForce * attackPos, ForceMode2D.Impulse);
        }
        if (curHealth > damage)
        {
            curHealth -= damage;
        }
        else
        {
            Debug.Log("Player died!");
            Destroy(gameObject);
        }
    }
    #endregion

    #region IResourceGauge Methods
    public void gainResource(float gain)
    {
        curSlowGauge += gain;
        // Convert gain to percentage
        float gainPerc = gain / maxSlowGauge;
        onEnergyRecoverUI?.Invoke(gainPerc);
    }

    public void useResource(float amount)
    {
        curSlowGauge -= amount;
        // Convert amount to percentage
        float amountPerc = amount / maxSlowGauge;
        onEnergySpentUI?.Invoke(amountPerc);
    }
    #endregion
}
