using UnityEngine;


public class Player : MonoBehaviour, IDamageable
{
    [Header("Player's Current Info")]
    public int curHealth;
    public int maxHealth;
    public playerState curPlayerState;
    public bool isGrounded;
    public bool isFacingRight;


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
    private CapsuleCollider2D playerCollider;






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
    }
    private void Start()
    {
        isFacingRight = true;
    }
    private void Update()
    {
        checkGrounded();
        
        if (curHealth == 0)
        {
            Debug.Log("you died!");
            // respawn
        }
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


    public void Damage(int damage)
    {
        curHealth -= damage;
        Debug.Log("took " + damage + " damage!");
    }
}
