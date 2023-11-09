using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float restitution;
    [SerializeField] private Vector2 jmpForce;
    public float maxHitDist;

    public playerState curPlayerState;

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

        // Move
        move();
        
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
    
    
}
