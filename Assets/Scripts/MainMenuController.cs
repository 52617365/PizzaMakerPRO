using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : SerialController
{
    [Header("Main Menu Controller specific variables")]
    [Space(15)]
    // Menu object references.
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject optionsMenu;
    [SerializeField]
    private GameObject videoMenu;
    [SerializeField]
    private GameObject soundMenu;

    private AudioSource audioSource;
    [Space(10)]
    [SerializeField]
    private AudioClip[] audioClips;

    // List of button in each menu container.
    [Space(10)]
    [SerializeField]
    private List<GameObject> mainMenuButtons;
    [SerializeField]
    private List<GameObject> optionsMenuButtons;
    [SerializeField]
    private List<GameObject> soundMenuButtons;

    // UI Text references
    [SerializeField]
    private TextMeshProUGUI musicVolumeText;
    [SerializeField]
    private TextMeshProUGUI soundVolumeText;

    private float musicValue = 1;
    private float soundValue = 1;

    private int menuIndex = 0;
    private float buttonCooldown;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey("MusicVolume"))
            musicValue = PlayerPrefs.GetFloat("MusicVolume");

        if (PlayerPrefs.HasKey("SoundVolume"))
            soundValue = PlayerPrefs.GetFloat("SoundVolume");

        musicVolumeText.text = (musicValue * 100).ToString("F0");
        soundVolumeText.text = (soundValue * 100).ToString("F0");
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(mainMenuButtons[0]);
            EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().localScale = DefaultValues.selectedButtonScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (EventSystem.current.currentSelectedGameObject.transform.localScale != DefaultValues.selectedButtonScale)
        {
            if (EventSystem.current.currentSelectedGameObject == mainMenuButtons[0] || EventSystem.current.currentSelectedGameObject == optionsMenuButtons[0])
                EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
        }
        */

        string message = (string)serialThread.ReadMessage();

        if (buttonCooldown > 0)
        {
            buttonCooldown -= Time.deltaTime;
            return;
        }

        if (message == null)
            return;

        switch (message)
        {
            case "-3":  // Joystick up
                SelectButton(false);
                break;
            case "-1":  // Joystick down
                SelectButton(true);
                break;
            case "1": // Joystick left
                SelectLeftRightButton(false);
                break;
            case "3": // Joystick right
                SelectLeftRightButton(true);
                break;
            case "5":   // Button 1
                SelectButton(true);
                break;
            case "6":   // Button 2
                Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
                button.onClick.Invoke();
                break;
            case "7":   // Button 3
                break;
            default:
                break;
        }

        if (ReferenceEquals(message, SERIAL_DEVICE_CONNECTED))
            messageListener.SendMessage("OnConnectionEvent", true);
        else if (ReferenceEquals(message, SERIAL_DEVICE_DISCONNECTED))
            messageListener.SendMessage("OnConnectionEvent", false);
        else
            messageListener.SendMessage("OnMessageArrived", message);
    }

    private void SelectButton(bool downwards)
    {
        buttonCooldown = 0.25f;

        if (EventSystem.current.currentSelectedGameObject != null)
            EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.defaultButtonScale;

        audioSource.clip = audioClips[0];
        audioSource.Play();

        if (mainMenu.activeSelf == true)
        {
            if (downwards)
            {
                if (menuIndex != mainMenuButtons.Count - 1)
                    menuIndex++;
                else
                    menuIndex = 0;
            }
            else
            {
                if (menuIndex != 0)
                    menuIndex--;
                else
                    menuIndex = mainMenuButtons.Count - 1;
            }
            EventSystem.current.SetSelectedGameObject(mainMenuButtons[menuIndex]);
            EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
            return;
        }

        if (soundMenu.activeSelf == true)
        {
            if (downwards)
            {
                if (menuIndex != 2)
                    menuIndex++;
                else
                    menuIndex = 0;
            }
            else
            {
                if (menuIndex != 0)
                    menuIndex--;
                else
                    menuIndex = 2;
            }
            EventSystem.current.SetSelectedGameObject(soundMenuButtons[menuIndex]);
            EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
            return;
        }

        if (optionsMenu.activeSelf == true)
        {
            if (downwards)
            {
                if (menuIndex != optionsMenuButtons.Count - 1)
                    menuIndex++;
                else
                    menuIndex = 0;
            }
            else
            {
                if (menuIndex != 0)
                    menuIndex--;
                else
                    menuIndex = optionsMenuButtons.Count - 1;
            }
            EventSystem.current.SetSelectedGameObject(optionsMenuButtons[menuIndex]);
            EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
            return;
        }


        Debug.Log(menuIndex);
    }

    private void SelectLeftRightButton(bool right)
    {
        buttonCooldown = 0.25f;
        if (soundMenu.activeSelf == true)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
                EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.defaultButtonScale;

            if (right)
            {
                if (EventSystem.current.currentSelectedGameObject == soundMenuButtons[0])
                    EventSystem.current.SetSelectedGameObject(soundMenuButtons[3]);
                if (EventSystem.current.currentSelectedGameObject == soundMenuButtons[1])
                    EventSystem.current.SetSelectedGameObject(soundMenuButtons[4]);
            }
            else
            {
                if (EventSystem.current.currentSelectedGameObject == soundMenuButtons[3])
                    EventSystem.current.SetSelectedGameObject(soundMenuButtons[0]);
                if (EventSystem.current.currentSelectedGameObject == soundMenuButtons[4])
                    EventSystem.current.SetSelectedGameObject(soundMenuButtons[1]);
            }

            EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
        }
    }

    public void DecreaseMusicVolume()
    {
        if (musicValue > 0)
            musicValue -= .05f;
        if (musicValue <= 0)
            musicValue = 0.0001f;
        Debug.Log(Mathf.Log10(musicValue) * 20);
        AudioManager.Instance.SetMusicValues(Mathf.Log10(musicValue) * 20);
        musicVolumeText.text = (musicValue * 100).ToString("F0");
    }

    public void IncreaseMusicVolume()
    {
        if (musicValue < 1)
            musicValue += .05f;
        if (musicValue > 1)
            musicValue = 1;
        Debug.Log(Mathf.Log10(musicValue) * 20);
        AudioManager.Instance.SetMusicValues(Mathf.Log10(musicValue) * 20);
        musicVolumeText.text = (musicValue * 100).ToString("F0");
    }

    public void DecreaseSoundVolume()
    {
        if (soundValue > 0)
            soundValue -= .05f;
        if (soundValue <= 0)
            soundValue = 0.0001f;
        AudioManager.Instance.SetSoundValues(Mathf.Log10(soundValue) * 20);
        soundVolumeText.text = (soundValue * 100).ToString("F0");
    }

    public void IncreaseSoundVolume()
    {
        if (soundValue < 1)
            soundValue += .05f;
        if (soundValue > 1)
            soundValue = 1;
        AudioManager.Instance.SetSoundValues(Mathf.Log10(soundValue) * 20);
        soundVolumeText.text = (soundValue * 100).ToString("F0");
    }

    public void SetButtonScale() => EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;

    public void ResetButtonScale() => EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.defaultButtonScale;

    public void SaveAudioSettings() => AudioManager.Instance.SaveAudioSettings(musicValue, soundValue);

    public void ResetMenuIndex() => menuIndex = 0;
}
