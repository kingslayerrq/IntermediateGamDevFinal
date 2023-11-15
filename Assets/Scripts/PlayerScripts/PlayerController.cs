using UnityEngine;
using DG.Tweening;
using UnityEditor;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Player's Attributes")]
    [SerializeField] private float moveSpeed;
    [Tooltip("The time it takes for player to rotate its Y-axis")] [SerializeField] private float turnSpeed;
    [SerializeField] private float maxJmpTime;
    private float curJmpTime;
    [SerializeField] private float jmpForce;
    //[SerializeField] private Vector2 initJmpForce;
    [SerializeField] private float attackRange;
    [SerializeField] private float dashDistance;
    [Tooltip("Time it takes to perform a dash")][SerializeField] private float dashDuration;
    [SerializeField] private float playerKnockbackDistance;
    [SerializeField] private float playerKnockbackDuration;
    public Transform attackPoint;
    public LayerMask attackableLayers;
    public LayerMask groundLayers;
    private Rigidbody2D playerRb;
    private CapsuleCollider2D playerCollider;
    private float checkGroundDist;
    [Tooltip("Buffer distance for Ground Check")] [SerializeField] private float checkGroundBuffer;


    [Header("Player's Current Info")]
    public playerState curPlayerState;
    public bool isGrounded;
    public bool isJumping;
    public bool isFacingRight;



    [Header("Player's KeyBinds")]
    [SerializeField] private KeyCode leftKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode rightKey = KeyCode.RightArrow;
    [SerializeField] private KeyCode jmpKey = KeyCode.Z;
    [SerializeField] private KeyCode atkKey = KeyCode.X;
    [SerializeField] private KeyCode dashKey = KeyCode.C;

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
        isFacingRight = true;
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

        // Jump
        if (Input.GetKeyDown(jmpKey) && isGrounded)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, jmpForce);
        }
        // As soon as jmpkey released, set yvel => 0
        if (Input.GetKeyUp(jmpKey) && playerRb.velocity.y > 0)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, 0);
        }

        if (Input.GetKeyDown(dashKey))
        {
            dash();
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
        checkGroundDist = playerCollider.size.y * 0.5f + checkGroundBuffer;
        RaycastHit2D hit = Physics2D.CapsuleCast(transform.position, playerCollider.size, CapsuleDirection2D.Vertical, 90f ,-Vector2.up ,checkGroundDist, groundLayers);
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
        Debug.Log("attacking");
        // detect targets hit
        Collider2D[] targetsHit =  Physics2D.OverlapCircleAll(attackPoint.position, attackRange, attackableLayers);
        //TODO: render slash



        //player knockback should only occur when the nail hits something
        if (targetsHit.Length > 0)
        {
            float endX;
            if (isFacingRight)
            {
                endX = transform.position.x - playerKnockbackDistance;
            }
            else
            {
                endX = transform.position.x + playerKnockbackDistance;
            }
            transform.DOMoveX(endX, playerKnockbackDuration);
        }
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

    #region Dash
    void dash()
    {
        // end point of the dash
        float endX;
        // Dash towards facing direction
        if (isFacingRight)
        {
            endX= transform.position.x + dashDistance;
        }
        else
        {
            endX = transform.position.x - dashDistance;
        }

        transform.DOMoveX(endX, dashDuration);
    }
    #endregion


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
