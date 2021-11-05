using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuController : SerialController
{
    [Header("Main Menu Controller specific variables")]
    [Space(15)]
    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private GameObject optionsMenu;

    [SerializeField]
    private List<GameObject> mainMenuButtons;
    [SerializeField]
    private List<GameObject> optionsMenuButtons;

    private int mainMenuIndex = 0;

    private void Start()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(mainMenuButtons[0]);
    }

    // Update is called once per frame
    void Update()
    {
        string message = (string)serialThread.ReadMessage();


        if (message == null)
            return;

        switch (message)
        {
            case "-3":
                break;
            case "-1":
                break;
            case "5":
                if (mainMenu.activeSelf == true)
                {
                    if (mainMenuIndex != mainMenuButtons.Count)
                        mainMenuIndex++;
                    else
                        mainMenuIndex = 0;
                    EventSystem.current.SetSelectedGameObject(mainMenuButtons[mainMenuIndex]);
                }
                break;
            case "6":
                break;
            case "7":
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
}
