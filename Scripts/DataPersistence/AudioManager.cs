using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance => instance;

    public AudioMixer audioMixer;
    private const string MixerVolumeParam = "MasterVolume";
    private const string VolumePrefKey = "MainVolumeSetting";

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        LoadVolumeSetting();
    }

    public void RegisterVolumeSlider(Slider slider)
    {
        if (slider != null)
        {
            slider.minValue = 0.0001f;
            slider.maxValue = 1f;
            slider.onValueChanged.AddListener(SetVolume);

            float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 0.75f);
            slider.value = savedVolume;
        }
    }

    private void LoadVolumeSetting()
    {
        float volume = PlayerPrefs.GetFloat(VolumePrefKey, 0.75f);
        SetVolume(volume);
    }

    public void SetVolume(float volume)
    {
        float decibels = Mathf.Log10(volume) * 20;
        
        audioMixer.SetFloat(MixerVolumeParam, decibels);
        
        SaveVolumeSetting(volume);
    }

    private void SaveVolumeSetting(float volume)
    {
        PlayerPrefs.SetFloat(VolumePrefKey, volume);
        PlayerPrefs.Save();
    }
}