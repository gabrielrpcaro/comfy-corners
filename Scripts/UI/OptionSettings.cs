using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class OptionSettings : MonoBehaviour
{
    public Slider MasterVolumeSettings;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private List<Resolution> availableResolutions;

    void OnEnable()
    {
        if (MasterVolumeSettings != null)
        {
            AudioManager.Instance.RegisterVolumeSlider(MasterVolumeSettings);
        }

        if (resolutionDropdown != null)
        {
            PopulateResolutionDropdown();
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }

    void PopulateResolutionDropdown()
    {
        availableResolutions = new List<Resolution>(Screen.resolutions);
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < availableResolutions.Count; i++)
        {
            Resolution resolution = availableResolutions[i];
            string option = resolution.width + " x " + resolution.height;
            options.Add(option);

            Debug.Log(option); // Debugging each resolution

            if (resolution.width == Screen.currentResolution.width &&
                resolution.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        if (options.Count == 0)
        {
            return;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int resolutionIndex)
    {
        if (availableResolutions == null || resolutionIndex >= availableResolutions.Count)
        {
            Debug.LogError("Invalid resolution index.");
            return;
        }

        Resolution resolution = availableResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}