using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Image meterFill;
    [SerializeField] private float tickInterval = 1f; // seconds between timer updates

    private int timerDuration;
    private float targetFill;
    private float fillSpeed;

    private void Awake()
    {
        Timer.Instance.OnTimerStarted += SetDuration;
        Timer.Instance.OnTimerUpdated += UpdateUI;
    }

    private void OnDestroy()
    {
        Timer.Instance.OnTimerStarted -= SetDuration;
        Timer.Instance.OnTimerUpdated -= UpdateUI;
    }

    public void SetDuration(int timer)
    {
        timerDuration = timer;
        meterFill.fillAmount = 1f;
        fillSpeed = (1f / timerDuration) / tickInterval;
        UpdateUI(timer); // start draining immediately
    }

    public void UpdateUI(int timer)
    {
        // Aim for where the bar should be at the NEXT tick
        targetFill = Mathf.Max(timer - 1, 0) / (float)timerDuration;
    }

    private void Update()
    {
        meterFill.fillAmount = Mathf.MoveTowards(meterFill.fillAmount, targetFill, fillSpeed * Time.deltaTime);
    }
}