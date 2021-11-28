using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuController : SerialController
{
    [Header("Main Menu Controller specific variables")]
    [Space(15)]
    // Menu object references.
    [SerializeField]
    private GameObject mainMenu;

    [SerializeField] private GameObject optionsMenu;

    [SerializeField] private GameObject playerCountMenu;

    [SerializeField] private GameObject levelMenu;

    [SerializeField] private GameObject helpMenu;

    [Space(10)] [SerializeField] private AudioClip[] audioClips;

    // List of button in each menu container.
    [Space(10)] [SerializeField] private List<GameObject> mainMenuButtons;

    [SerializeField] private List<GameObject> optionsMenuButtons;

    [SerializeField] private List<GameObject> helpMenuButtons;

    [SerializeField] private List<GameObject> playerCountButtons;

    [SerializeField] private List<GameObject> levelMenuButtons;

    [SerializeField] private Toggle verticalSyncToggle;

    // UI Text references
    [SerializeField] private TextMeshProUGUI musicVolumeText;

    [SerializeField] private TextMeshProUGUI soundVolumeText;

    private AudioSource audioSource;
    private float buttonCooldown;

    private int menuIndex;

    private float musicValue = 1;
    private float soundValue = 1;

    private void Awake()
    {
        LoadVerticalSync();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicValue = PlayerPrefs.GetFloat("MusicVolume");
        }

        if (PlayerPrefs.HasKey("SoundVolume"))
        {
            soundValue = PlayerPrefs.GetFloat("SoundVolume");
        }

        musicVolumeText.text = (musicValue * 100).ToString("F0");
        soundVolumeText.text = (soundValue * 100).ToString("F0");
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(mainMenuButtons[0]);
            EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().localScale =
                DefaultValues.selectedButtonScale;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        var message = (string) serialThread.ReadMessage();

        if (buttonCooldown > 0)
        {
            buttonCooldown -= Time.deltaTime;
            return;
        }

        if (message == null)
        {
            return;
        }

        switch (message)
        {
            case "-3": // Joystick up
                SelectButton(false);
                break;
            case "-1": // Joystick down
                SelectButton(true);
                break;
            case "1": // Joystick left
                SelectLeftRightButton(false);
                break;
            case "3": // Joystick right
                SelectLeftRightButton(true);
                break;
            case "5": // Button 1
                //SelectButton(true);
                Click();
                break;
            case "6": // Button 2
                //Click();
                break;
            case "7": // Button 3
                break;
        }

        if (ReferenceEquals(message, SERIAL_DEVICE_CONNECTED))
        {
            messageListener.SendMessage("OnConnectionEvent", true);
        }
        else if (ReferenceEquals(message, SERIAL_DEVICE_DISCONNECTED))
        {
            messageListener.SendMessage("OnConnectionEvent", false);
        }
        else
        {
            messageListener.SendMessage("OnMessageArrived", message);
        }
    }

    private static void Click()
    {
        // Checks if it was button that was clicked.
        if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>() != null)
        {
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
            Debug.Log(EventSystem.current.currentSelectedGameObject.name);
            return;
        }

        // Checks if it was toggle that was clicked.
        if (EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>() == null)
        {
            return;
        }

        var toggle = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();
        toggle.isOn = !toggle.isOn;
    }

    public void SetPlayerCount(int count)
    {
        LevelChanger.Instance.PlayerCount = count;
    }

    public void SetButtonScale()
    {
        EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
    }

    public void ResetButtonScale()
    {
        EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.defaultButtonScale;
    }

    public void SaveAudioSettings()
    {
        AudioManager.SaveAudioSettings(musicValue, soundValue);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ResetMenuIndex()
    {
        menuIndex = 0;
    }

    #region Joystick navigation

    private void SelectButton(bool downwards)
    {
        buttonCooldown = 0.25f;

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.defaultButtonScale;
        }

        audioSource.clip = audioClips[0];
        audioSource.Play();

        if (optionsMenu.activeSelf)
        {
            if (downwards)
            {
                if (menuIndex != 3)
                {
                    menuIndex++;
                }
                else
                {
                    menuIndex = 0;
                }
            }
            else
            {
                if (menuIndex != 0)
                {
                    menuIndex--;
                }
                else
                {
                    menuIndex = 3;
                }
            }

            EventSystem.current.SetSelectedGameObject(optionsMenuButtons[menuIndex]);
            EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
            return;
        }

        if (playerCountMenu.activeSelf)
        {
            if (menuIndex == playerCountButtons.Count - 1)
            {
                menuIndex--;
            }
            else
            {
                menuIndex++;
            }

            EventSystem.current.SetSelectedGameObject(playerCountButtons[menuIndex]);
            EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
            return;
        }

        if (levelMenu.activeSelf)
        {
            if (downwards)
            {
                menuIndex = menuIndex switch
                {
                    4 => 0,
                    3 => 4,
                    2 => 4,
                    1 => 4,
                    0 => 4,
                    _ => menuIndex
                };
            }
            else
            {
                menuIndex = menuIndex switch
                {
                    4 => 0,
                    3 => 4,
                    2 => 4,
                    1 => 4,
                    0 => 4,
                    _ => menuIndex
                };
            }

            EventSystem.current.SetSelectedGameObject(levelMenuButtons[menuIndex]);
            EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
            return;
        }

        if (helpMenu.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(helpMenuButtons[0]);
            EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
            return;
        }

        if (!mainMenu.activeSelf)
        {
            return;
        }

        if (downwards)
        {
            if (menuIndex != mainMenuButtons.Count - 1)
            {
                menuIndex++;
            }
            else
            {
                menuIndex = 0;
            }
        }
        else
        {
            if (menuIndex != 0)
            {
                menuIndex--;
            }
            else
            {
                menuIndex = mainMenuButtons.Count - 1;
            }
        }

        EventSystem.current.SetSelectedGameObject(mainMenuButtons[menuIndex]);
        EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
    }

    private void SelectLeftRightButton(bool right)
    {
        buttonCooldown = 0.25f;
        if (optionsMenu.activeSelf)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.defaultButtonScale;
            }

            if (right)
            {
                if (EventSystem.current.currentSelectedGameObject == optionsMenuButtons[0])
                {
                    EventSystem.current.SetSelectedGameObject(optionsMenuButtons[4]);
                }

                if (EventSystem.current.currentSelectedGameObject == optionsMenuButtons[1])
                {
                    EventSystem.current.SetSelectedGameObject(optionsMenuButtons[5]);
                }
            }
            else
            {
                if (EventSystem.current.currentSelectedGameObject == optionsMenuButtons[4])
                {
                    EventSystem.current.SetSelectedGameObject(optionsMenuButtons[0]);
                }

                if (EventSystem.current.currentSelectedGameObject == optionsMenuButtons[5])
                {
                    EventSystem.current.SetSelectedGameObject(optionsMenuButtons[1]);
                }
            }

            audioSource.clip = audioClips[0];
            audioSource.Play();
            EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
            return;
        }

        if (!levelMenu.activeSelf)
        {
            return;
        }

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.defaultButtonScale;
        }

        if (right)
        {
            menuIndex = menuIndex switch
            {
                4 => 0,
                3 => 4,
                2 => 3,
                1 => 2,
                0 => 1,
                _ => menuIndex
            };
        }
        else
        {
            menuIndex = menuIndex switch
            {
                4 => 3,
                3 => 2,
                2 => 1,
                1 => 0,
                0 => 4,
                _ => menuIndex
            };
        }

        audioSource.clip = audioClips[0];
        audioSource.Play();
        EventSystem.current.SetSelectedGameObject(levelMenuButtons[menuIndex]);
        EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
    }

    #endregion


    #region Sound settings & Vertical sync

    public void DecreaseMusicVolume()
    {
        if (musicValue > 0)
        {
            musicValue -= .1f;
        }

        if (musicValue <= 0)
        {
            musicValue = 0.0001f;
        }

        Debug.Log(Mathf.Log10(musicValue) * 20);
        AudioManager.SetMusicValues(Mathf.Log10(musicValue) * 20);
        musicVolumeText.text = (musicValue * 100).ToString("F0");
    }

    public void IncreaseMusicVolume()
    {
        if (musicValue < 1)
        {
            musicValue += .1f;
        }

        if (musicValue > 1)
        {
            musicValue = 1;
        }

        Debug.Log(Mathf.Log10(musicValue) * 20);
        AudioManager.SetMusicValues(Mathf.Log10(musicValue) * 20);
        musicVolumeText.text = (musicValue * 100).ToString("F0");
    }

    public void DecreaseSoundVolume()
    {
        if (soundValue > 0)
        {
            soundValue -= .1f;
        }

        if (soundValue <= 0)
        {
            soundValue = 0.0001f;
        }

        AudioManager.SetSoundValues(Mathf.Log10(soundValue) * 20);
        soundVolumeText.text = (soundValue * 100).ToString("F0");
    }

    public void IncreaseSoundVolume()
    {
        if (soundValue < 1)
        {
            soundValue += .1f;
        }

        if (soundValue > 1)
        {
            soundValue = 1;
        }

        AudioManager.SetSoundValues(Mathf.Log10(soundValue) * 20);
        soundVolumeText.text = (soundValue * 100).ToString("F0");
    }

    #endregion

    #region Vertical sync

    private static void ChangeVerticalSync(int value)
    {
        switch (value)
        {
            case 1:
                // vsync on
                QualitySettings.vSyncCount = 1;
                PlayerPrefs.SetInt("VerticalSync", 1);
                break;
            case 0:
                // vsync off
                QualitySettings.vSyncCount = 0;
                PlayerPrefs.SetInt("VerticalSync", 0);
                break;
        }
    }

    public void VerticalSyncListener(Toggle toggle)
    {
        int value = toggle.isOn ? 1 : 0;

        ChangeVerticalSync(value);
    }

    private void LoadVerticalSync()
    {
        if (!PlayerPrefs.HasKey("VerticalSync"))
        {
            return;
        }

        var vsyncValue = PlayerPrefs.GetInt("VerticalSync");
        if (vsyncValue == 0)
        {
            verticalSyncToggle.isOn = false;
        }

        QualitySettings.vSyncCount = vsyncValue;
    }

    #endregion
}
