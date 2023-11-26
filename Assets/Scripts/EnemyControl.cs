using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    public Transform groundDetection;

    private bool movingRight = true;
    private LayerMask platformLayerMask;

    // Enemy health
    private int health = 3;

    void Start()
    {
        platformLayerMask = LayerMask.GetMask("Platform");
    }

    void Update()
    {
        Move();

        //detects edge and turn around
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 1f, platformLayerMask);
        Debug.DrawLine(groundDetection.position, groundDetection.position + Vector3.down, Color.red);

        if (groundInfo.collider == false)
        {
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
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void TakeDamage()
    {
        health -= 1;
        if (health <= 0)
        {
            Destroy(gameObject); // Destroy the enemy
        }
    }
}
