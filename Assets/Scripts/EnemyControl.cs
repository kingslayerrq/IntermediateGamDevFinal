using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    public Transform groundDetection;
    private bool movingRight = true;

    // Layer mask to detect platforms
    private LayerMask platformLayerMask;

    void Start()
    {
        platformLayerMask = LayerMask.GetMask("Platform");
    }

    void Update()
    {
        Move();

        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 1f, platformLayerMask);
        Debug.DrawLine(groundDetection.position, groundDetection.position + new Vector3(0, -1f, 0), Color.red);

        if (groundInfo.collider == false)
        {
            Debug.Log("Edge");
            Flip();
        }
    }

    void Move()
    {
        Vector2 moveDirection = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    void Flip()
    {
        movingRight = !movingRight;

        // Flip the sprite by scaling
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
