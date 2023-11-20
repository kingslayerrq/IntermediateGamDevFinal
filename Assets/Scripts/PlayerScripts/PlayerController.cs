using UnityEngine;
using DG.Tweening;
using UnityEditor;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    private Player player;

    [Header("Movement Attributes")]
    [SerializeField] private float moveSpeed;
    [Tooltip("The time it takes for player to rotate its Y-axis")] [SerializeField] private float turnSpeed;
    [SerializeField] private float maxJmpTime;
    [SerializeField] private float jmpForce;
    [SerializeField] private float dashDistance;
    [Tooltip("Time it takes to perform a dash")][SerializeField] private float dashDuration;

   
    private Rigidbody2D playerRb;

  






    

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
    }
   

    private void Update()
    {
        #region Movement
        // Move
        move();
        #endregion

       

        #region Jump
        // Jump
        if (Input.GetKeyDown(player.jmpKey) && player.isGrounded)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, jmpForce);
        }
        // As soon as jmpkey released, set yvel => 0
        if (Input.GetKeyUp(player.jmpKey) && playerRb.velocity.y > 0)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, 0);
        }
        #endregion

        #region Dash
        if (Input.GetKeyDown(player.dashKey))
        {
            dash();
        }
        #endregion
          
        

    }

    // Basic Movement
    void move()
    {
        // Move Left
        if (Input.GetKeyDown(player.leftKey) || Input.GetKey(player.leftKey))
        {
            transform.position = new Vector3(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            // Turn if currently facing right
            if (player.isFacingRight) turn();
        }
        // Move Right
        else if (Input.GetKeyDown(player.rightKey) || Input.GetKey(player.rightKey))
        {
            transform.position = new Vector3(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
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
        transform.DORotate(turn, turnSpeed);
        player.isFacingRight = !player.isFacingRight;
    }

  
    #region Dash
    void dash()
    {
        // end point of the dash
        float endX;
        // Dash towards facing direction
        if (player.isFacingRight)
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


    

}
