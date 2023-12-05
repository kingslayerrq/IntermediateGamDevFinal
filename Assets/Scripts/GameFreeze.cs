using UnityEngine;
using System.Collections;

public class GameFreeze : MonoBehaviour
{
    public float freezeTimeOffset;
    public float playerHurtFreezeTime;

    private void Awake()
    {
        Player.onPlayerHurt += Freeze;
    }
    private void Freeze(float time)
    {
        StartCoroutine(FreezeGame(playerHurtFreezeTime));
    }
    private IEnumerator FreezeGame(float time)
    {
        yield return new WaitForSecondsRealtime(freezeTimeOffset);
        var curTimeScale = Time.timeScale;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = curTimeScale;
    }
    private void OnDestroy()
    {
        Player.onPlayerHurt -= Freeze;
    }
}
