using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private Player player;

    [Header("Health UI")]
    public Transform heartLayoutGroup;
    public GameObject heartPrefab;

    [Header("Soul Gauge UI")]
    public Image SoulGaugeFill;
    [SerializeField] private float curFillAmount;

    private List<GameObject> heartList;
    [SerializeField] private int curHeartNumDisplayed;
    

    private void Awake()
    {
        player = GetComponent<Player>();
        // Subscribe to player instance events
        player.onHitUI += removeHeart;
        player.onRecoverUI += addHeart;
        player.onEnergyRecoverUI += increaseGauge;
        player.onEnergySpentUI += decreaseGauge;
    }

    private void Start()
    {
        heartList = new List<GameObject>();
        // Initialize Hearts
        for(int i = 0; i < player.maxHealth; i++)
        {
            heartList.Add(Instantiate(heartPrefab, heartLayoutGroup));
        }
        // Initialize Soul Gauge
        SoulGaugeFill.fillAmount = player.curGauge;
    }

    private void Update()
    {
        curHeartNumDisplayed = heartLayoutGroup.childCount;
        curFillAmount = SoulGaugeFill.fillAmount;
    }

    #region Health UI
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
                Destroy(heartToRemove);
            }
            else
            {
                Debug.LogWarning("No hearts to remove");
            }
        }
    }
    #endregion
    #region Soul Gauge UI
    void increaseGauge(float amount)            // Parameter {0 - 1}
    {
        float endAmount = Mathf.Clamp(SoulGaugeFill.fillAmount + amount, 0.0f, 1.0f);
        SoulGaugeFill.DOFillAmount(endAmount, Time.deltaTime);                // we call this function each frame when not slowing down time and not at full gauge
    }
    void decreaseGauge(float amount)            // Parameter {0 - 1}
    {
        float endAmount = Mathf.Clamp(SoulGaugeFill.fillAmount - amount, 0.0f, 1.0f);
        Debug.Log("end amount : " + endAmount);
        SoulGaugeFill.DOFillAmount(endAmount, Time.deltaTime);
    }
    #endregion

    private void OnDestroy()                    
    {
        // Unsubscribe to player instance events
        player.onHitUI -= removeHeart;
        player.onRecoverUI -= addHeart;
        player.onEnergyRecoverUI -= increaseGauge;
        player.onEnergySpentUI -= decreaseGauge;
    }
}
