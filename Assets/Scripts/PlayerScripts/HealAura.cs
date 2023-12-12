using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAura : MonoBehaviour
{
    private Player player;
    private PlayerCombat pCombat;

    private Light healLight;
    [SerializeField] private float dimIntensity = 0;
    [SerializeField] private float litUpIntensity;
    [SerializeField] private float litUpDuration;

    private void Awake()
    {
        healLight = GetComponent<Light>();
        player = GetComponentInParent<Player>();
        pCombat = GetComponentInParent<PlayerCombat>();
    }


    public void LightUp()
    {
        Debug.Log("lightup!");
        healLight.DOIntensity(litUpIntensity, litUpDuration).OnComplete(() =>
        {
            healLight.intensity = dimIntensity;
        });
    }

    
}
