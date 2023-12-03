using UnityEngine;

public class TimeSlow : MonoBehaviour
{
    public float slowDownFactor { get; set; }
    public float slowDownDuration { get; set; }
    [SerializeField] private float slowDownRecoverOffset;

    //private float recoverTime = 0;

    public float minSlowDownDuration = 0f;
    private void Update()
    {
        if (slowDownDuration != 0)
        {
            
            Time.timeScale += (1f / (slowDownDuration + slowDownRecoverOffset)) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1);
            
        }
    }

    public void slow()
    {
        Time.timeScale = slowDownFactor;
        Time.fixedDeltaTime = Time.timeScale * slowDownFactor;
        //recoverTime = slowDownFactor;
    }

    
}
