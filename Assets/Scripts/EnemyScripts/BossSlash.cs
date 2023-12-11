using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossSlash : MonoBehaviour
{
    private Boss boss;
    private Animator weaponAnimator;


    private void Awake()
    {
        boss = GetComponentInParent<Boss>();
        weaponAnimator = GetComponent<Animator>();
        boss.onAttack += Slash;
    }

    void Slash(bool b)
    {
        weaponAnimator.SetTrigger("slash");
    }

    private void OnDestroy()
    {
        boss.onAttack -= Slash;
    }
}
