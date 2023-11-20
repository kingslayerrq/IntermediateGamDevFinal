using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerCamera : MonoBehaviour
{
    public CinemachineVirtualCamera cam1;
    public CinemachineVirtualCamera cam2;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name != "MC")
            return;
        print("enter");
        cam2.enabled = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name != "MC")
            return;
        print("exit");
        cam2.enabled = false;
    }
}
