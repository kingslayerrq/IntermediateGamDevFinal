using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BotSlash : MonoBehaviour
{
    private Player player;
    private PlayerCombat pCombat;
    private Animator topSlashAnimator;


    private void Awake()
    {
        player = GetComponent<Player>();
        pCombat = GetComponentInParent<PlayerCombat>();
        topSlashAnimator = GetComponent<Animator>();

    }


    private void Start()
    {
        pCombat.OnDownSlash += DownSlashTrigger;
    }


    void DownSlashTrigger(int dir)
    {
        topSlashAnimator.SetTrigger("downSlash");
    }


    private void OnDestroy()
    {
        pCombat.OnDownSlash -= DownSlashTrigger;
    }
}
