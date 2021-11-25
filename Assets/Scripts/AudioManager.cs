using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    private static AudioManager Instance { get; set; }

    private void Awake()
    {
        // Singleton pattern to only have single instance
        // of AudioManager on scene.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        Invoke("LoadValues", 0);
    }

    public static void SaveAudioSettings(float musicValue, float soundValue)
    {
        PlayerPrefs.SetFloat("MusicVolume", musicValue);
        PlayerPrefs.SetFloat("SoundVolume", soundValue);
    }

    public void LoadValues()
    {
        float musicValue;
        float soundValue;

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicValue = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            musicValue = 1;
        }

        if (PlayerPrefs.HasKey("SoundVolume"))
        {
            soundValue = PlayerPrefs.GetFloat("SoundVolume");
        }
        else
        {
            soundValue = 1;
        }

        Instance.mixer.SetFloat("MusicVolume", Mathf.Log10(musicValue) * 20);
        Instance.mixer.SetFloat("SoundVolume", Mathf.Log10(soundValue) * 20);
    }

    public static void SetMusicValues(float musicValue)
    {
        Instance.mixer.SetFloat("MusicVolume", musicValue);
    }

    public static void SetSoundValues(float soundValue)
    {
        Instance.mixer.SetFloat("SoundVolume", soundValue);
    }
}
