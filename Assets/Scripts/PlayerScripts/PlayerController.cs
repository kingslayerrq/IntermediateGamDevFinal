using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Player's Attributes")]
    [SerializeField] private float moveSpeed;
    [Tooltip("The time it takes for player to rotate its Y-axis")] [SerializeField] private float turnSpeed;
    [SerializeField] private Vector2 jmpForce;
    [SerializeField] private float attackRange;
    public Transform attackPoint;
    public LayerMask attackableLayers;
    private Rigidbody2D playerRb;
    


    [Header("Player's Current Info")]
    public playerState curPlayerState;
    public bool isGrounded;
    public bool isFacingRight;



    [Header("Player's KeyBinds")]
    [SerializeField] private KeyCode leftKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode rightKey = KeyCode.RightArrow;
    [SerializeField] private KeyCode jmpKey = KeyCode.Z;
    [SerializeField] private KeyCode atkKey = KeyCode.X;

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
        curPlayerState = playerState.IDLE;
        isFacingRight = true;
    }

    private void Update()
    {

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
        // Move Left
        if (Input.GetKeyDown(leftKey) || Input.GetKey(leftKey))
        {
            transform.position = new Vector3(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            // Turn if currently facing right
            if (isFacingRight) turn();
        }
        // Move Right
        else if (Input.GetKeyDown(rightKey) || Input.GetKey(rightKey))
        {
            transform.position = new Vector3(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            // Turn if currently facing left
            if (!isFacingRight) turn();
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            Debug.Log("ground");
            isGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            Debug.Log("air");
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
