using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerTimeSlow : MonoBehaviour
{
    private Player player;

    [Header("Time Slow Attributes")]
    private TimeSlow playerTimeSlow;
    [SerializeField] private float playerTimeSlowFactor;

    [SerializeField] private float slowGaugeRecoverFactor;

    private void Awake()
    {
        playerTimeSlow = GetComponent<TimeSlow>();
    }

    private void Start()
    {
        player = GetComponent<Player>();
        playerTimeSlow.slowDownFactor = playerTimeSlowFactor;
        

    }

    private void Update()
    {
        // Recover Time Slow Gauge when not slowing, not exceeding max
        if (Time.timeScale == 1f && player.curSlowGauge < player.maxSlowGauge)
        {
            
            player.gainResource(slowGaugeRecoverFactor);
        }
        player.curSlowGauge = Mathf.Clamp(player.curSlowGauge, 0, player.maxSlowGauge);


        #region TimeSlow
        if (Input.GetKeyDown(player.timeSlowKey) && Time.timeScale == 1f)
        {
            if (player.curSlowGauge >= playerTimeSlow.minSlowDownDuration)
            {
                playerTimeSlow.slowDownDuration = player.curSlowGauge;
                playerTimeSlow.slow();

            }
        }

        // When we are slowing down time, decrease the gauge
        if (Time.timeScale < 1f && player.curSlowGauge > 0)
        {
            Debug.Log("slowed");
            player.useResource(Time.unscaledDeltaTime);
            
        }
        #endregion
    }
}
