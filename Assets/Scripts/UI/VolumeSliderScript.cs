using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderScript : MonoBehaviour
{
    public string VolumeSaveName;
    public TextMeshProUGUI SliderValue;
    public TextMeshProUGUI SliderValueAdditional;
    public Slider VolumeSlider;
    private SoundManager soundManager;
    private float DefaultValue = 0.75f;
    void Start()
    {
        soundManager = FindAnyObjectByType<SoundManager>();
        if (!PlayerPrefs.HasKey(VolumeSaveName))
        {
            PlayerPrefs.SetFloat(VolumeSaveName, 0.75f);
            LoadVolume();
        }
        else
        {
            LoadVolume();
        }
    }

    private void LoadVolume()
    {
        VolumeSlider.value = PlayerPrefs.GetFloat(VolumeSaveName);
    }

    public void SetDefaults()
    {
        PlayerPrefs.SetFloat(VolumeSaveName, 0.75f);
        VolumeSlider.value = DefaultValue;
        SliderValue.text = "75";
        SliderValueAdditional.text = "75";
    }
    public void SaveVolume()
    {
        int roundedValue = Mathf.RoundToInt(VolumeSlider.value * 100);
        SliderValue.text = roundedValue.ToString();
        SliderValueAdditional.text = roundedValue.ToString();
        PlayerPrefs.SetFloat(VolumeSaveName, VolumeSlider.value);
        soundManager.SetVolume();
    }
}
