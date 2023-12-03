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
        if (Time.timeScale == 1f && player.curGauge < player.maxGauge)
        {
            
            player.gainResource(slowGaugeRecoverFactor);
        }
        player.curGauge = Mathf.Clamp(player.curGauge, 0, player.maxGauge);


        #region TimeSlow
        if (Input.GetKeyDown(player.timeSlowKey) && Time.timeScale == 1f)
        {
            if (player.curGauge >= playerTimeSlow.minSlowDownDuration)
            {
                playerTimeSlow.slowDownDuration = player.curGauge;
                Debug.Log("slowing down " + player.curGauge);
                playerTimeSlow.slow();

            }
        }

        // When we are slowing down time, decrease the gauge
        if (Time.timeScale < 1f && player.curGauge > 0)
        {
            Debug.Log("slowed");
            player.useResource(Time.unscaledDeltaTime);
            
        }
        #endregion
    }
}
