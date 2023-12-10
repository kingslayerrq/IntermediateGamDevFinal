using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource playerAudio;

    public AudioClip Jump;
    public AudioClip Dash;
    public AudioClip Slash;

    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
    }

    public void PlayJumpSFX()
    {
        playerAudio.clip = Jump;
        playerAudio.Play();
    }

    public void PlayDashSFX()
    {
        playerAudio.clip = Dash;
        playerAudio.Play();
    }

    public void PlaySlashSFX()
    {
        playerAudio.clip = Slash;
        playerAudio.Play();
    }


}
