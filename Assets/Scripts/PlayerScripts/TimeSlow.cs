using UnityEngine;

public class TimeSlow : MonoBehaviour
{
    public float slowDownFactor { get; set; }
    public float slowDownDuration { get; set; }
   

    public float minSlowDownDuration = 2f;
    private void Update()
    {
        if (slowDownDuration != 0)
        {
            
            Time.timeScale += (1f / slowDownDuration) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1);
        }
    }

    public void slow()
    {
        Time.timeScale = slowDownFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }

    
}
