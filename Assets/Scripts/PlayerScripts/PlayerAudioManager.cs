using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudioManager : MonoBehaviour
{
    public AudioSource playerAudio;

    public AudioClip Jump;
    public AudioClip Dash;
    //public AudioSource Land;
    public AudioClip Slash;
    public AudioClip SlashWall;

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

    public void PlaySlashWallSFX()
    {
        playerAudio.clip = SlashWall;
        playerAudio.Play();
    }


}
