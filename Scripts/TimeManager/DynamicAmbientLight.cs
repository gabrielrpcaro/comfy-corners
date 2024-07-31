using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DynamicAmbientLight : MonoBehaviour
{
    public Gradient lightGradient;
    private Light2D globalLight;

    [Header("Morning Period Settings")]
    public float morningLightIntensityStart = 0.06f;
    public float morningLightIntensityEnd = 1f;

    [Header("Afternoon Period Settings")]
    public float afternoonLightIntensityStart = 1f;
    public float afternoonLightIntensityEnd = 0.3f;

    [Header("Evening Period Settings")]
    public float eveningLightIntensityStart = 0.3f;
    public float eveningLightIntensityEnd = 0.02f;

    [Header("Night Period Settings")]
    public float nightLightIntensityStart = 0.02f;
    public float nightLightIntensityEnd = 0.06f;

    private string lastPeriod = "";

    private void Awake()
    {
        globalLight = GetComponent<Light2D>();
        if (globalLight == null)
        {
            Debug.LogError("Light2D component not found on the GameObject.");
        }
    }

    private void Update()
    {
        if (TimeManager.Instance != null && globalLight != null)
        {
            UpdateLightIntensity();
            UpdateLightColor();
        }
    }

    private void UpdateLightIntensity()
    {
        string currentPeriod = TimeManager.Instance.GetCurrentPeriod();
        float timeOfDay = (TimeManager.Instance.GetCurrentHour() + TimeManager.Instance.GetCurrentMinute() / 60f) / 24f;
        float periodProgression = GetPeriodProgression(currentPeriod, timeOfDay);

        if (currentPeriod != lastPeriod)
        {
            if (currentPeriod == "Morning")
            {
                globalLight.intensity = morningLightIntensityStart;
            }
            else if (currentPeriod == "Afternoon")
            {
                globalLight.intensity = afternoonLightIntensityStart;
            }
            else if (currentPeriod == "Evening")
            {
                globalLight.intensity = eveningLightIntensityStart;
            }
            else if (currentPeriod == "Night")
            {
                globalLight.intensity = nightLightIntensityStart;
            }

            lastPeriod = currentPeriod;
        }

        if (currentPeriod == "Morning")
        {
            globalLight.intensity = Mathf.Lerp(morningLightIntensityStart, morningLightIntensityEnd, periodProgression);
        }
        else if (currentPeriod == "Afternoon")
        {
            globalLight.intensity = Mathf.Lerp(afternoonLightIntensityStart, afternoonLightIntensityEnd, periodProgression);
        }
        else if (currentPeriod == "Evening")
        {
            globalLight.intensity = Mathf.Lerp(eveningLightIntensityStart, eveningLightIntensityEnd, periodProgression);
        }
        else if (currentPeriod == "Night")
        {
            globalLight.intensity = Mathf.Lerp(nightLightIntensityStart, nightLightIntensityEnd, periodProgression);
        }
    }

    private float GetPeriodProgression(string period, float timeOfDay)
    {
        float periodStart = 0f;
        float periodEnd = 0f;

        switch (period)
        {
            case "Morning":
                periodStart = TimeManager.Instance.morningPeriod.startHour / 24f;
                periodEnd = TimeManager.Instance.morningPeriod.endHour / 24f;
                break;
            case "Afternoon":
                periodStart = TimeManager.Instance.afternoonPeriod.startHour / 24f;
                periodEnd = TimeManager.Instance.afternoonPeriod.endHour / 24f;
                break;
            case "Evening":
                periodStart = TimeManager.Instance.eveningPeriod.startHour / 24f;
                periodEnd = TimeManager.Instance.eveningPeriod.endHour / 24f;
                break;
            case "Night":
                periodStart = TimeManager.Instance.nightPeriod.startHour / 24f;
                periodEnd = TimeManager.Instance.nightPeriod.endHour / 24f;
                float midPeriod = (periodStart + periodEnd) / 2f;

                if (periodEnd > periodStart)
                {
                    if (timeOfDay >= midPeriod)
                    {
                        periodStart = midPeriod;
                    }
                    periodEnd = TimeManager.Instance.nightPeriod.endHour / 24f;
                }
                else
                {
                    if (timeOfDay >= midPeriod || timeOfDay < periodStart)
                    {
                        periodStart = midPeriod;
                        periodEnd = (periodEnd + 1f) % 1f;
                    }
                    else
                    {
                        periodEnd = midPeriod;
                    }
                }
                break;
        }

        return Mathf.InverseLerp(periodStart, periodEnd, timeOfDay);
    }


    private void UpdateLightColor()
    {
        // Calculate the time of day as a value between 0 and 1
        float timeOfDay = (TimeManager.Instance.GetCurrentHour() + TimeManager.Instance.GetCurrentMinute() / 60f) / 24f;
        // Get the color from the gradient based on the time of day
        Color newColor = lightGradient.Evaluate(timeOfDay);
        // Update the light color
        globalLight.color = newColor;
    }
}