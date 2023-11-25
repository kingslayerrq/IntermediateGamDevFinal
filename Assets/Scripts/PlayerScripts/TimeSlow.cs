using UnityEngine;

public class TimeSlow : MonoBehaviour
{
    public float slowDownFactor { get; set; }
    public float slowDownDuration { get; set; }
    [SerializeField] private float slowDownRecoverOffset;

    //private float recoverTime = 0;

    public float minSlowDownDuration = 2f;
    private void Update()
    {
        if (slowDownDuration != 0)
        {
            
            Time.timeScale += (1f / (slowDownDuration + slowDownRecoverOffset)) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1);
            /*if (recoverTime == 1)
            {
                recover();
            }*/
        }
    }
    /*public void recover()
    {
        Time.timeScale = Mathf.Lerp(Time.timeScale, 1, 0.5f);
    }*/
    public void slow()
    {
        Time.timeScale = slowDownFactor;
        Time.fixedDeltaTime = Time.timeScale * slowDownFactor;
        //recoverTime = slowDownFactor;
    }

    
}
