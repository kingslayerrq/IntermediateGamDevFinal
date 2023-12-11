using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    public EncounterTrigger trigger;
    public CinemachineVirtualCamera cam1;
    public CinemachineVirtualCamera cam2;


    private void Awake()
    {
        trigger.onEncounterStart += SwitchCam;
    }
    // Start is called before the first frame update
    void Start()
    {
        cam1.enabled = true;
        cam2.enabled = false;
    }

    void SwitchCam(bool b)
    {
        cam2.enabled = true;
        cam1.enabled = false;
    }

    private void OnDestroy()
    {
        trigger.onEncounterStart -= SwitchCam;
    }
}
