using System.Collections;
using TMPro;
using UnityEngine;

public class UITimeDisplay : MonoBehaviour
{
    public TMP_Text hourText;
    public TMP_Text minuteText;
    public TMP_Text dayText;
    public TMP_Text seasonText;
    public TMP_Text periodText;

    private void Start()
    {
        StartCoroutine(WaitForTimeManager());
    }

    private IEnumerator WaitForTimeManager()
    {
        while (TimeManager.Instance == null)
        {
            yield return null;
        }

        UpdateTimeDisplay();
    }

    private void Update()
    {
        if (TimeManager.Instance != null)
        {
            UpdateTimeDisplay();
        }
    }

    private void UpdateTimeDisplay()
    {
        hourText.text = TimeManager.Instance.GetCurrentHour().ToString("00");
        minuteText.text = TimeManager.Instance.GetCurrentMinute().ToString("00");
        dayText.text = TimeManager.Instance.GetCurrentDay();
        seasonText.text = TimeManager.Instance.GetCurrentSeason().ToString();
        periodText.text = TimeManager.Instance.GetCurrentPeriod();
    }
}
