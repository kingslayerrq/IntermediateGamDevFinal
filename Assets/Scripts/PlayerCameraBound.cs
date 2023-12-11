using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraBound : MonoBehaviour
{
    [SerializeField] private float minX;
   
   


    private void Start()
    {
       
    }
    // Update is called once per frame
    void Update()
    {
        var pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, minX, transform.position.x);
        transform.position = pos;
        //Debug.Log(transform.position);
    }
}
