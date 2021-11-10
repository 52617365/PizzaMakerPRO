using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    [SerializeField]
    private AudioMixer mixer;

    private void Awake()
    {
        // Singleton pattern to only have single instance
        // of AudioManager on scene.
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        Invoke("LoadValues", 0);
    }

    public void SaveAudioSettings(float musicValue, float soundValue)
    {
        PlayerPrefs.SetFloat("MusicVolume", musicValue);
        PlayerPrefs.SetFloat("SoundVolume", soundValue);
    }

    public void LoadValues()
    {
        float musicValue;
        float soundValue;

        if (PlayerPrefs.HasKey("MusicVolume"))
            musicValue = PlayerPrefs.GetFloat("MusicVolume");
        else
            musicValue = 1;
        if (PlayerPrefs.HasKey("SoundVolume"))
            soundValue = PlayerPrefs.GetFloat("SoundVolume");
        else
            soundValue = 1;

        _instance.mixer.SetFloat("MusicVolume", Mathf.Log10(musicValue) * 20);
        _instance.mixer.SetFloat("SoundVolume", Mathf.Log10(soundValue) * 20);
    }

    public void SetMusicValues(float musicValue)
    {
        _instance.mixer.SetFloat("MusicVolume", musicValue);
    }

    public void SetSoundValues(float soundValue)
    {
        _instance.mixer.SetFloat("SoundVolume", soundValue);
    }
}
