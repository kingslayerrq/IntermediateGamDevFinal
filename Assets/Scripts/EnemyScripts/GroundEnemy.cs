using UnityEngine;

public class GroundEnemy : BaseEnemy
{

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();

        if (!isMovingRight)
        {
            FlipSprite();
        }
    }

    protected override void Update()
    {
        Move();

        checkGrounded();
    }
    //detects edge and turn around 
    protected override void checkGrounded()
    {
        checkGroundDist = enemyCollider.size.y * 0.5f + checkGroundBuffer;
        // Add horizontal buffer 
        int sign = isMovingRight ? 1 : -1;
        var nextPosition = new Vector3(transform.position.x + sign * enemyCollider.size.x, transform.position.y, transform.position.z);
        RaycastHit2D groundInfo = Physics2D.CapsuleCast(nextPosition, enemyCollider.size, CapsuleDirection2D.Vertical, 90 , -Vector2.up, checkGroundDist, platformLayerMask);

        //Debug.DrawLine(groundDetection.position, groundDetection.position + Vector3.down, Color.red);

        if (groundInfo.collider == false)
        {
            Debug.Log("edge");
            this.Flip();
        }
    }
    protected override void Move()
    {
        Vector2 moveDirection = isMovingRight ? Vector2.right : Vector2.left;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
}
