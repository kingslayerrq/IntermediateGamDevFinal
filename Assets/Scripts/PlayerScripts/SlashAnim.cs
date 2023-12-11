using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SlashAnim : MonoBehaviour
{
    private Player player;
    private PlayerCombat pCombat;
    public Animator weaponAnimator;



    private void Awake()
    {
        player = GetComponent<Player>();
        weaponAnimator = GetComponent<Animator>();
        pCombat = GetComponentInParent<PlayerCombat>();
    }
    private void Start()
    {
        pCombat.OnHorSlash += HorSlash;
       
    }

    void HorSlash(int slash)
    {
        weaponAnimator.SetTrigger("horSlash");
    }

   

    private void OnDestroy()
    {
        pCombat.OnHorSlash -= HorSlash;
        
    }
}
