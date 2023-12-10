using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.Events;
using System.Collections;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    private Player player;

    public ChangeBgm changeBgmScript;

    [Header("Movement Attributes")]
    [SerializeField] private float moveSpeed;
    [Tooltip("The time it takes for player to rotate its Y-axis")] [SerializeField] private float turnSpeed;
    [SerializeField] private float maxJmpTime;
    [SerializeField] private float jmpForce;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashCoolDown;
    [Tooltip("Time it takes to perform a dash")][SerializeField] private float dashDuration;


    private PlayerAudioManager playerAudioManager;
    public UnityEvent onJump = new UnityEvent();
    public UnityEvent onDash = new UnityEvent();
  



    private void Awake()
    {
        playerAudioManager = GetComponent<PlayerAudioManager>();
        player = GetComponent<Player>();
    }

    void Start()
    {
        onJump.AddListener(playerAudioManager.PlayJumpSFX);

        onDash.AddListener(playerAudioManager.PlayDashSFX);

    }
   

    private void Update()
    {
        if (player.canMove)
        {
            #region Movement
            // Move
            move();
            #endregion

            #region Jump
            // Jump
            if (Input.GetKeyDown(player.jmpKey) && player.isGrounded)
            {
                onJump.Invoke();
                player.playerRb.velocity = new Vector2(player.playerRb.velocity.x, jmpForce);
            }
            // As soon as jmpkey released, set yvel => 0
            if (Input.GetKeyUp(player.jmpKey) && player.playerRb.velocity.y > 0)
            {
                player.playerRb.velocity = new Vector2(player.playerRb.velocity.x, 0);
            }
            #endregion

            #region Dash
            if (Input.GetKeyDown(player.dashKey) && !player.isDashing && player.canDash)
            {
                Debug.Log("dashing");
                onDash.Invoke();
                StartCoroutine("dash");
            }
            #endregion            
        }

        #region SwitchMusic
            if (changeBgmScript != null)
            {
                changeBgmScript.SwitchBGM();
            }       
        #endregion

    }

    // Basic Movement
    void move()
    {
        // Move Left
        if (Input.GetKeyDown(player.leftKey) || Input.GetKey(player.leftKey))
        {
            transform.position = new UnityEngine.Vector3(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            // Turn if currently facing right
            if (player.isFacingRight) turn();
        }
        // Move Right
        else if (Input.GetKeyDown(player.rightKey) || Input.GetKey(player.rightKey))
        {
            transform.position = new UnityEngine.Vector3(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            // Turn if currently facing left
            if (!player.isFacingRight) turn();
        }

    }



    // Rotate the player's Y
    void turn()
    {
        Vector3 turn;
        if (player.isFacingRight)
        {
            turn = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
        }
        else
        {
            turn = new Vector3(transform.rotation.x, 0f, transform.rotation.z);   
        }
        // Rotate
        transform.DORotate(turn, turnSpeed * Time.unscaledDeltaTime);
        player.isFacingRight = !player.isFacingRight;
    }

  
    #region Dash
    
    private IEnumerator dash()
    {
        player.isDashing = true;
        var originG = player.playerRb.gravityScale;
        player.playerRb.gravityScale = 0f;
        if (player.isFacingRight)
        {
            player.playerRb.velocity = Vector2.right * dashForce;
        }
        else
        {
            player.playerRb.velocity = Vector2.left * dashForce;
        }
        yield return new WaitForSecondsRealtime(dashDuration);
        player.playerRb.gravityScale = originG;
        player.playerRb.velocity = Vector2.zero;
        player.isDashing = false;

        // Start the cooldown coroutine after the dash coroutine has completed
        yield return StartCoroutine("dashCD");

    }

    private IEnumerator dashCD()
    {
        player.canDash = false;
        yield return new WaitForSecondsRealtime(dashCoolDown);
        player.canDash = true;
    }
    #endregion



    

}
