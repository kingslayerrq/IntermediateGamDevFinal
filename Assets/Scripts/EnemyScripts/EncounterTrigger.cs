using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterTrigger : MonoBehaviour
{
    private BoxCollider2D encounterZone;
    public Boss boss;
    public event Action<bool> onEncounterStart;
    private void Awake()
    {
        boss = GetComponentInParent<Boss>();
        encounterZone = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !boss.isAwake)
        {
            onEncounterStart?.Invoke(true);
        }
    }
}
