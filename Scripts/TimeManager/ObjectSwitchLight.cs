using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ObjectSwitchLight : MonoBehaviour
{
    public enum Period { Morning, Afternoon, Evening, Night }

    [Header("Settings")]
    public Period enablePeriod;
    public Period disablePeriod;
    public float transitionDuration = 1f; // Duration for the light intensity transition

    private Light2D light2D;
    private float originalIntensity;
    private float transitionProgress;
    private bool isEnabled;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        if (light2D == null)
        {
            Debug.LogError("Light2D component not found on the GameObject.");
            enabled = false; // Disable this script if no Light2D is found
            return;
        }

        originalIntensity = light2D.intensity;
        light2D.intensity = 0f;
    }

    private void Start()
    {
        if (TimeManager.Instance != null)
        {
            string currentPeriod = TimeManager.Instance.GetCurrentPeriod();
            bool shouldBeEnabled = ShouldEnableLight(currentPeriod);

            if (shouldBeEnabled)
            {
                isEnabled = true;
                light2D.intensity = originalIntensity;
            }
        }
    }

    private void Update()
    {
        if (TimeManager.Instance != null)
        {
            UpdateLightState();
        }
    }

    private void UpdateLightState()
    {
        string currentPeriod = TimeManager.Instance.GetCurrentPeriod();
        bool shouldBeEnabled = ShouldEnableLight(currentPeriod);
        bool shouldBeDisabled = ShouldDisableLight(currentPeriod);

        if (shouldBeEnabled && !isEnabled)
        {
            isEnabled = true;
            StopAllCoroutines();
            StartCoroutine(TransitionLightIntensity(0f, originalIntensity));
        }
        else if (shouldBeDisabled && isEnabled)
        {
            isEnabled = false;
            StopAllCoroutines();
            StartCoroutine(TransitionLightIntensity(originalIntensity, 0f));
        }
    }

    public bool ShouldEnableLight(string currentPeriod)
    {
        Period currentPeriodEnum = (Period)System.Enum.Parse(typeof(Period), currentPeriod);
        return IsPeriodInRange(currentPeriodEnum, enablePeriod, disablePeriod, true);
    }

    public bool ShouldDisableLight(string currentPeriod)
    {
        Period currentPeriodEnum = (Period)System.Enum.Parse(typeof(Period), currentPeriod);
        return IsPeriodInRange(currentPeriodEnum, enablePeriod, disablePeriod, false);
    }

    private bool IsPeriodInRange(Period currentPeriod, Period startPeriod, Period endPeriod, bool enable)
    {
        if (startPeriod == endPeriod)
        {
            return currentPeriod == startPeriod;
        }

        if (enable)
        {
            if (startPeriod < endPeriod)
            {
                return currentPeriod >= startPeriod && currentPeriod < endPeriod;
            }
            else
            {
                return currentPeriod >= startPeriod || currentPeriod < endPeriod;
            }
        }
        else
        {
            if (startPeriod < endPeriod)
            {
                return currentPeriod >= endPeriod || currentPeriod < startPeriod;
            }
            else
            {
                return currentPeriod >= endPeriod && currentPeriod < startPeriod;
            }
        }
    }

    private System.Collections.IEnumerator TransitionLightIntensity(float from, float to)
    {
        transitionProgress = 0f;
        while (transitionProgress < 1f)
        {
            transitionProgress += Time.deltaTime / transitionDuration;
            light2D.intensity = Mathf.Lerp(from, to, transitionProgress);
            yield return null;
        }
        light2D.intensity = to;
    }
}
