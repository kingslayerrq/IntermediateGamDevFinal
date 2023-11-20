using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera cam1;
    public CinemachineVirtualCamera cam2;


    // Start is called before the first frame update
    void Start()
    {
        cam1.enabled = true;
        cam2.enabled = false;
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
        cam1.enabled = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name != "MC")
            return;
        print("exit");

        cam2.enabled = false;
        cam1.enabled = true;
    }
}
