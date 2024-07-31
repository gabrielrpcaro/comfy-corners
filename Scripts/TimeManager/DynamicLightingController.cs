using UnityEngine;

namespace Modern2D
{
    public class DynamicLightingController : MonoBehaviour
    {
        private LightingSystem lightingSystem;
        private string lastPeriod;
        private bool wasDirectional;

        [Header("Morning Period Settings")]
        public float morningShadowLengthStart = 3f;
        public float morningShadowLengthEnd = 1f;
        public float morningShadowNarrowingStart = 0.4f;
        public float morningShadowNarrowingEnd = 0.8f;
        public float morningShadowAlphaStart = 0.5f;
        public float morningShadowAlphaEnd = 0.3f;
        public float morningDirectionalLightAngleStart = 320f;
        public float morningDirectionalLightAngleEnd = 180f;

        [Header("Afternoon Period Settings")]
        public float afternoonShadowLengthStart = 1f;
        public float afternoonShadowLengthEnd = 2f;
        public float afternoonShadowNarrowingStart = 0.8f;
        public float afternoonShadowNarrowingEnd = 0.6f;
        public float afternoonShadowAlphaStart = 0.3f;
        public float afternoonShadowAlphaEnd = 0.35f;
        public float afternoonDirectionalLightAngleStart = 180f;
        public float afternoonDirectionalLightAngleEnd = 90f;

        [Header("Evening Period Settings")]
        public float eveningShadowLengthStart = 2f;
        public float eveningShadowLengthEnd = 4f;
        public float eveningShadowNarrowingStart = 0.6f;
        public float eveningShadowNarrowingEnd = 0.4f;
        public float eveningShadowAlphaStart = 0.35f;
        public float eveningShadowAlphaEnd = 0.4f;
        public float eveningDirectionalLightAngleStart = 90f;
        public float eveningDirectionalLightAngleEnd = 140f;

        [Header("Night Period Settings")]
        public float nightShadowLength = 4f;
        public float nightShadowNarrowing = 1f;
        public float nightShadowAlpha = 0.5f;
        public float nightShadowFalloff = 15f;
        public Color nightAmbientColor = Color.black;

        private void Awake()
        {
            // Find the LightingSystem component on the same GameObject
            lightingSystem = GetComponent<LightingSystem>();
            if (lightingSystem == null)
            {
                Debug.LogError("LightingSystem component not found on the GameObject.");
            }
        }

        private void Start()
        {
            // Apply initial settings based on the current period
            ApplyInitialSettings();
        }

        private void Update()
        {
            if (TimeManager.Instance != null && lightingSystem != null)
            {
                UpdateLightingSettings();

                // Check if the directional setting has changed
                if (lightingSystem.isLightDirectional.value != wasDirectional)
                {
                    UpdateDirectionalSettings();
                    wasDirectional = lightingSystem.isLightDirectional.value;
                }
            }
        }

        private void SetMorningSettings()
        {
            lightingSystem._shadowLength.value = morningShadowLengthStart;
            lightingSystem._shadowNarrowing.value = morningShadowNarrowingStart;
            lightingSystem._shadowAlpha.value = morningShadowAlphaStart;
            lightingSystem.directionalLightAngle.value = morningDirectionalLightAngleStart;
            lightingSystem.isLightDirectional.value = true;

            // Trigger the shadow settings change
            lightingSystem.OnShadowSettingsChanged();
        }

        private void SetNightSettings()
        {
            lightingSystem._shadowLength.value = nightShadowLength;
            lightingSystem._shadowNarrowing.value = nightShadowNarrowing;
            lightingSystem._shadowAlpha.value = nightShadowAlpha;
            lightingSystem._shadowFalloff.value = nightShadowFalloff;
            lightingSystem._shadowColor.value = nightAmbientColor;
            lightingSystem.isLightDirectional.value = false;

            // Trigger the shadow settings change
            lightingSystem.OnShadowSettingsChanged();
        }

        private void UpdateLightingSettings()
        {
            string currentPeriod = TimeManager.Instance.GetCurrentPeriod();
            float timeOfDay = (TimeManager.Instance.GetCurrentHour() + TimeManager.Instance.GetCurrentMinute() / 60f) / 24f;
            float periodProgression = GetPeriodProgression(currentPeriod, timeOfDay);

            if (currentPeriod != lastPeriod)
            {
                if (currentPeriod == "Night")
                {
                    SetNightSettings();
                }
                else if (lastPeriod == "Night" && currentPeriod == "Morning")
                {
                    SetMorningSettings();
                }
                lastPeriod = currentPeriod;
            }

            if (currentPeriod != "Night")
            {
                if (currentPeriod == "Morning")
                {
                    UpdateSettings(morningShadowLengthStart, morningShadowLengthEnd, morningShadowNarrowingStart, morningShadowNarrowingEnd, morningShadowAlphaStart, morningShadowAlphaEnd, morningDirectionalLightAngleStart, morningDirectionalLightAngleEnd, periodProgression);
                }
                else if (currentPeriod == "Afternoon")
                {
                    UpdateSettings(afternoonShadowLengthStart, afternoonShadowLengthEnd, afternoonShadowNarrowingStart, afternoonShadowNarrowingEnd, afternoonShadowAlphaStart, afternoonShadowAlphaEnd, afternoonDirectionalLightAngleStart, afternoonDirectionalLightAngleEnd, periodProgression);
                }
                else if (currentPeriod == "Evening")
                {
                    UpdateSettings(eveningShadowLengthStart, eveningShadowLengthEnd, eveningShadowNarrowingStart, eveningShadowNarrowingEnd, eveningShadowAlphaStart, eveningShadowAlphaEnd, eveningDirectionalLightAngleStart, eveningDirectionalLightAngleEnd, periodProgression);
                }

                // Trigger the shadow settings change
                lightingSystem.OnShadowSettingsChanged();
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
            }

            float periodDuration = (periodEnd - periodStart) * 24f;
            float currentTimeInPeriod = (timeOfDay - periodStart) * 24f;

            return currentTimeInPeriod / periodDuration;
        }

        private void UpdateSettings(float lengthStart, float lengthEnd, float narrowingStart, float narrowingEnd, float alphaStart, float alphaEnd, float angleStart, float angleEnd, float progression)
        {
            lightingSystem._shadowLength.value = Mathf.Lerp(lengthStart, lengthEnd, progression);
            lightingSystem._shadowNarrowing.value = Mathf.Lerp(narrowingStart, narrowingEnd, progression);
            lightingSystem._shadowAlpha.value = Mathf.Lerp(alphaStart, alphaEnd, progression);
            lightingSystem.directionalLightAngle.value = Mathf.Lerp(angleStart, angleEnd, progression);
        }

        private void UpdateDirectionalSettings()
        {
            if (lightingSystem.isLightDirectional.value)
            {
                lightingSystem.distMinMax.value = new Vector2(1, 1);
                lightingSystem.shadowLengthMinMax.value = new Vector2(1, 1);
            }
            else
            {
                lightingSystem.distMinMax.value = new Vector2(1, 1);
                lightingSystem.shadowLengthMinMax.value = new Vector2(1, 1);
            }

            // Trigger the shadow settings change
            lightingSystem.OnShadowSettingsChanged();
        }

        private void ApplyInitialSettings()
        {
            string currentPeriod = TimeManager.Instance.GetCurrentPeriod();
            float timeOfDay = (TimeManager.Instance.GetCurrentHour() + TimeManager.Instance.GetCurrentMinute() / 60f) / 24f;
            float periodProgression = GetPeriodProgression(currentPeriod, timeOfDay);

            if (currentPeriod == "Night")
            {
                SetNightSettings();
            }
            else if (currentPeriod == "Morning")
            {
                SetMorningSettings();
                UpdateSettings(morningShadowLengthStart, morningShadowLengthEnd, morningShadowNarrowingStart, morningShadowNarrowingEnd, morningShadowAlphaStart, morningShadowAlphaEnd, morningDirectionalLightAngleStart, morningDirectionalLightAngleEnd, periodProgression);
            }
            else if (currentPeriod == "Afternoon")
            {
                UpdateSettings(afternoonShadowLengthStart, afternoonShadowLengthEnd, afternoonShadowNarrowingStart, afternoonShadowNarrowingEnd, afternoonShadowAlphaStart, afternoonShadowAlphaEnd, afternoonDirectionalLightAngleStart, afternoonDirectionalLightAngleEnd, periodProgression);
            }
            else if (currentPeriod == "Evening")
            {
                UpdateSettings(eveningShadowLengthStart, eveningShadowLengthEnd, eveningShadowNarrowingStart, eveningShadowNarrowingEnd, eveningShadowAlphaStart, eveningShadowAlphaEnd, eveningDirectionalLightAngleStart, eveningDirectionalLightAngleEnd, periodProgression);
            }

            // Trigger the shadow settings change
            lightingSystem.OnShadowSettingsChanged();
        }
    }
}
