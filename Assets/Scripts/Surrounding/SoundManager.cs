using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<AudioSource> MusicSounds = new List<AudioSource>();
    public List<AudioSource> AmbientSounds = new List<AudioSource>();
    public List<AudioSource> SoundEffects = new List<AudioSource>();

    private float _masterVolume;
    private float _musicVolume;
    private float _ambientVolume;
    private float _sfxVolume;

    private void Awake()
    {
        SetVolume();
    }

    public void SetVolume()
    {
        _masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        _musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        _ambientVolume = PlayerPrefs.GetFloat("AmbientVolume", 0.5f);
        _sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.5f);
        Debug.Log(" music: " + _musicVolume * _masterVolume + " ambient: " + _ambientVolume * _masterVolume + " sfx: " + _sfxVolume * _masterVolume);
        SetMusicVolume();
        SetAmbientVolume();
        SetSfxVolume();
    }

    private void SetMusicVolume()
    {
        foreach (AudioSource musicSound in MusicSounds)
        {
            if (musicSound != null)
                musicSound.volume = _masterVolume * _musicVolume;
        }
    }

    private void SetAmbientVolume()
    {
        foreach (AudioSource ambientSound in AmbientSounds)
        {
            if (ambientSound != null)
                ambientSound.volume = _masterVolume * _ambientVolume;
        }
    }

    private void SetSfxVolume()
    {
        foreach (AudioSource sfxSound in SoundEffects)
        {
            if (sfxSound != null)
                sfxSound.volume = _masterVolume * _sfxVolume;
        }
    }
}
