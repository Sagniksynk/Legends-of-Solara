using System.Collections;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    public static FXManager instance;

    [Header("Hit Stop Defaults")]
    [SerializeField] private float defaultStopDuration = 0.2f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void HitStop(float duration)
    {
        if (duration <= 0) duration = defaultStopDuration;
        StartCoroutine(HitStopCo(duration));
    }

    private IEnumerator HitStopCo(float duration)
    {
        // 1. Freeze Time
        Time.timeScale = 0f;

        // 2. Wait (using unscaled time so we don't freeze forever)
        yield return new WaitForSecondsRealtime(duration);

        // 3. Unfreeze
        Time.timeScale = 1f;
    }
}