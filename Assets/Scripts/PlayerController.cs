using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float restitution;
    [SerializeField] private Vector2 jmpForce;
    [SerializeField] private float attackRange;
    public Transform attackPoint;
    public LayerMask attackableLayers;
    public LayerMask groundLayers;
    private Rigidbody2D playerRb;
    private CapsuleCollider2D playerCollider;
    private float groundCheckDist;
    [Tooltip("Buffer distance for raycasting towards the ground")][SerializeField] private float groundCheckBuffer;
    

    private Rigidbody2D playerRb;
    private CapsuleCollider2D playerCollider;
    private bool isGrounded;

    private RaycastHit2D hit;
    


    private KeyCode leftKey = KeyCode.A;
    private KeyCode rightKey = KeyCode.D;
    private KeyCode jmpKey = KeyCode.Space;

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


    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        curPlayerState = playerState.IDLE;
    }

    private void Update()
    {

        checkGrounded();
        // Move
        move();

        // Attack (animation determines how often we can detect the input of attack?)
        if (Input.GetKeyDown(atkKey))
        {
            attack();
        }
    }

    // Update Player State
    void updatePlayerState(playerState newState)
    {
        if (newState != curPlayerState)
        {
            // Set the curPlayerState to newState
            curPlayerState = newState;
            switch (newState)
            {
                case playerState.IDLE:
                    handleIdle();
                    break;
                case playerState.RUNNING:
                    handleRunning();
                    break;
                case playerState.AIRBORNE:
                    handleAirborne();
                    break;
                case playerState.FOCUS:
                    handleFocus();
                    break;
                case playerState.ATTACK:
                    handleAttack();
                    break;
                case playerState.HURT:
                    handleHurt();
                    break;
                case playerState.DEAD:
                    handleDead();
                    break;
                default:
                    break;
            }
        }
    }

    

    #region Player State Logic
    void handleIdle()
    {
        Debug.Log("Idling");
    }

    void handleRunning()
    {
        Debug.Log("Running");
    }

    void handleAirborne()
    {
        Debug.Log("In Air!!");
    }

    void handleFocus()
    {

    }

    void handleAttack()
    {

    }

    void handleHurt()
    {

    }

    void handleDead()
    {
        
    }
    #endregion


    // Basic Movement
    void move()
    {
        // Horizontal movement
        if (Input.GetKeyDown(leftKey) || Input.GetKey(leftKey))
        {
            transform.position = new Vector3(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        }
        else if (Input.GetKeyDown(rightKey) || Input.GetKey(rightKey))
        {
            transform.position = new Vector3(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        }

        // Check for Grounded
        if (isGrounded)
        {
            // Jump
            if (Input.GetKeyDown(jmpKey))
            {
                playerRb.AddForce(jmpForce, ForceMode2D.Impulse);
            }
        }
    }

    // Rotate the player's Y
    void turn()
    {
        Vector3 turn;
        if (isFacingRight)
        {
            turn = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
        }
        else
        {
            turn = new Vector3(transform.rotation.x, 0f, transform.rotation.z);   
        }
        // Rotate
        transform.DORotate(turn, turnSpeed);
        isFacingRight = !isFacingRight;
    }

    #region Ground Check

    void checkGrounded()
    {
        groundCheckDist = playerCollider.size.y * 0.5f + groundCheckBuffer;
        //Debug.DrawRay(transform.position, Vector3.down * groundCheckDist, Color.yellow);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, groundCheckDist, groundLayers);
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


    void attack()
    {
        // detect targets hit
        Collider2D[] targetsHit =  Physics2D.OverlapCircleAll(attackPoint.position, attackRange, attackableLayers);

        foreach(Collider2D target in targetsHit)
        {
            if (target.gameObject.layer == 3)
            {
                Debug.Log("we hit a platform");
            }
            else if(target.gameObject.layer == 7)
            {
                Debug.Log("we hit an enemy");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {

        if (!attackPoint)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }


}
