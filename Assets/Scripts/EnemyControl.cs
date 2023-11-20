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

    // Start is called before the first frame update
    void Start()
    {
        platformLayerMask = LayerMask.GetMask("Platform");
    }

    // Update is called once per frame
    void Update()
    {
        if (movingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }
        else{
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }
        

        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 1f, platformLayerMask);
        Debug.DrawLine(groundDetection.position, groundDetection.position + new Vector3(0, -1f, 0), Color.red);

        if (groundInfo.collider == true)
        {
            Debug.Log("On Platform");
        }
        if (groundInfo.collider == false)
        {
            Debug.Log("On Edge");
            if (movingRight == true)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                movingRight = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                movingRight = true;
            }
        }
    }
}
