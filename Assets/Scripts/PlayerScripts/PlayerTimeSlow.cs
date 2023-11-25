using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerTimeSlow : MonoBehaviour
{
    private Player player;

    [Header("Time Slow Attributes")]
    private TimeSlow playerTimeSlow;
    [SerializeField] private float playerTimeSlowFactor;
    [SerializeField] private float maxSlowGauge;
    [SerializeField] private float curSlowGauge;
    [SerializeField] private float slowGaugeRecoverFactor;

    private void Awake()
    {
        playerTimeSlow = GetComponent<TimeSlow>();
    }

    private void Start()
    {
        player = GetComponent<Player>();
        playerTimeSlow.slowDownFactor = playerTimeSlowFactor;
        curSlowGauge = maxSlowGauge;

    }

    private void Update()
    {
        // Recover Time Slow Gauge, not exceeding max
        if (Time.timeScale == 1f)
        {
            curSlowGauge += slowGaugeRecoverFactor;
        }
        curSlowGauge = Mathf.Clamp(curSlowGauge, 0, maxSlowGauge);


        #region TimeSlow
        if (Input.GetKeyDown(player.timeSlowKey))
        {
            if (curSlowGauge >= playerTimeSlow.minSlowDownDuration)
            {
                playerTimeSlow.slowDownDuration = curSlowGauge;
                playerTimeSlow.slow();
            }
        }

        // When we are slowing down time, decrease the gauge
        if (Time.timeScale < 1f)
        {
            Debug.Log("slowed");
            curSlowGauge -= Time.unscaledDeltaTime;
        }
        #endregion
    }
}
