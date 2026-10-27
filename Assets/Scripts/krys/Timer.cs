using System;
using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer Instance;
    public event Action<int>  OnTimerUpdated;
    public event Action<int> OnTimerStarted;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        StartTimer(5 * 60);

    }
    public void StartTimer(int durationInSeconds)
    {
        StartCoroutine(TimerCoroutine(durationInSeconds));
    }

    private IEnumerator TimerCoroutine(int durationInSeconds)
    {
        OnTimerStarted?.Invoke(durationInSeconds);

        int duration = durationInSeconds;
        while (durationInSeconds > 0)
        {
            Debug.Log(duration);
            yield return new WaitForSeconds(1.0f);
            duration--;
            OnTimerUpdated?.Invoke(duration);
            yield return null;
        }
    }
}
