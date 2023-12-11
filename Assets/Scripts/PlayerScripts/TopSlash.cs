using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TopSlash : MonoBehaviour
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
        pCombat.OnUpSlash += TopSlashTrigger;
    }


    void TopSlashTrigger(int dir)
    {
        topSlashAnimator.SetTrigger("topSlash");
    }


    private void OnDestroy()
    {
        pCombat.OnUpSlash -= TopSlashTrigger;
    }
}
