using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ChangeBgm : MonoBehaviour
{
    public AudioSource bgm;
    public GameObject player;
    public EncounterTrigger trigger;

    public AudioClip levelBgm;
    public AudioClip bossBgm;
    private bool hasChangedBgm = false;

    private void Awake()
    {
        trigger.onEncounterStart += SwitchBGM;
    }
    // Start is called before the first frame update
    void Start()
    {
        bgm = GetComponent<AudioSource>();
        bgm.clip = levelBgm;
        bgm.Play();
    }

    void BossMusic()
    {
        bgm.Stop();
        bgm.clip = bossBgm;
        bgm.Play();
    }

    public void SwitchBGM(bool b)
    {
      
        BossMusic();
        hasChangedBgm = true;
        
    }

    public void Slow()
    {
        bgm.pitch = 0.5f;
    }

    public void Revert()
    {
        bgm.pitch = 1.0f;
    }

    private void OnDestroy()
    {
        trigger.onEncounterStart -= SwitchBGM;
    }
}
