using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private Player player;

    [Header("UI Elements")]
    public Transform heartLayoutGroup;
    public GameObject heartPrefab;

    private List<GameObject> heartList;
    [SerializeField] private int curHeartNumDisplayed;
    

    private void Awake()
    {
        player = GetComponent<Player>();
        // Subscribe to player instance events
        player.onHit += removeHeart;
        player.onRecover += addHeart;
    }

    private void Start()
    {
        heartList = new List<GameObject>();
        // initialize the hearts
        Debug.Log(player.maxHealth);
        for(int i = 0; i < player.maxHealth; i++)
        {
            heartList.Add(Instantiate(heartPrefab, heartLayoutGroup));
        }
    }

    private void Update()
    {
        curHeartNumDisplayed = heartLayoutGroup.childCount;
    }

    // Add X heart to List
    void addHeart(int num)
    {
        for (int i = 0; i < num; i++)
        {
            heartList.Add(Instantiate(heartPrefab, heartLayoutGroup));
        }
    }
    // Remove X heart from List's End
    void removeHeart(int num)
    {
        for (int i = 0; i < num; i++)
        {
            var heartToRemove = heartList.LastOrDefault();
            if (heartToRemove)
            {
                heartList.Remove(heartToRemove);
            }
            else
            {
                Debug.LogWarning("No hearts to remove");
            }
        }
    }
    private void OnDestroy()
    {
        // Unsubscribe to player instance events
        player.onHit -= removeHeart;
        player.onRecover -= addHeart;
    }
}
