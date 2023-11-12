using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D bigBody;
    public float m_Speed;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0,0,0);
        m_Speed = 0.01f;
        bigBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKey(KeyCode.A))
        {
            bigBody.AddForce(transform.right * -m_Speed, ForceMode2D.Impulse);
        }

        if (Input.GetKey(KeyCode.D))
        {
            bigBody.AddForce(transform.right * m_Speed, ForceMode2D.Impulse);
        }

        if(Input.GetKey(KeyCode.W))
        {
            bigBody.AddForce(transform.up * m_Speed, ForceMode2D.Impulse);
        }
    }
}
