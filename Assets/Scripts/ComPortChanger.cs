using System;
using TMPro;
using UnityEngine;

public class ComPortChanger : MonoBehaviour
{
    [SerializeField] private bool isInMainMenu;

    [SerializeField] private GameObject player1Prefab;

    [SerializeField] private GameObject player2Prefab;

    // Dropdown menus for player 1 & player 2
    // COM ports.
    [SerializeField] private TMP_Dropdown player1Ports;

    [SerializeField] private TMP_Dropdown player2Ports;

    [SerializeField] private GameObject comPortContainer;

    // MainMenuController reference.
    private GameObject mainMenuController;

    // Player 1 & Player 2 references.
    private GameObject player1;
    private GameObject player2;

    /*
     *  COM PORT NUMBERS AS DROPDOWN VALUES
     *  COM1 = 0
     *  COM2 = 1
     *  COM3 = 2
     *  COM4 = 3
     *  COM5 = 4
     *  COM6 = 5
     *  COM7 = 6
     *  COM8 = 7
     *  COM9 = 8
     *  COM10 = 9
     *  COM11 = 10
     *  COM12 = 11
    */

    private void Awake()
    {
        if (isInMainMenu)
        {
            // Finds MainMenuController gameObject and its COM port.
            // Then sets current COM port on player1 dropdown
            // selection to be the same.
            try
            {
                mainMenuController = GameObject.Find("MainMenuController");
                mainMenuController.GetComponent<MainMenuController>().portName =
                    player1Prefab.GetComponent<PlayerController>().portName;
                switch (mainMenuController.GetComponent<MainMenuController>().portName)
                {
                    case "COM12":
                        player1Ports.value = 11;
                        break;
                    case "COM11":
                        player1Ports.value = 10;
                        break;
                    case "COM10":
                        player1Ports.value = 9;
                        break;
                    case "COM9":
                        player1Ports.value = 8;
                        break;
                    case "COM8":
                        player1Ports.value = 7;
                        break;
                    case "COM7":
                        player1Ports.value = 6;
                        break;
                    case "COM6":
                        player1Ports.value = 5;
                        break;
                    case "COM5":
                        player1Ports.value = 4;
                        break;
                    case "COM4":
                        player1Ports.value = 3;
                        break;
                    case "COM3":
                        player1Ports.value = 2;
                        break;
                    case "COM2":
                        player1Ports.value = 1;
                        break;
                    case "COM1":
                        player1Ports.value = 0;
                        break;
                }
            }
            catch (Exception)
            {
                player1Ports.interactable = false;
            }
        }
        else
        {
            // Finds Player1 & Player2 gameObjects and their COM ports.
            // Then sets current COM port on dropdown
            // selection to be the same.
            try
            {
                player1 = GameObject.Find("Player1");
                switch (player1.GetComponent<PlayerController>().portName)
                {
                    case "COM12":
                        player1Ports.value = 11;
                        break;
                    case "COM11":
                        player1Ports.value = 10;
                        break;
                    case "COM10":
                        player1Ports.value = 9;
                        break;
                    case "COM9":
                        player1Ports.value = 8;
                        break;
                    case "COM8":
                        player1Ports.value = 7;
                        break;
                    case "COM7":
                        player1Ports.value = 6;
                        break;
                    case "COM6":
                        player1Ports.value = 5;
                        break;
                    case "COM5":
                        player1Ports.value = 4;
                        break;
                    case "COM4":
                        player1Ports.value = 3;
                        break;
                    case "COM3":
                        player1Ports.value = 2;
                        break;
                    case "COM2":
                        player1Ports.value = 1;
                        break;
                    case "COM1":
                        player1Ports.value = 0;
                        break;
                }
            }
            catch (Exception)
            {
                player1Ports.interactable = false;
            }

            try
            {
                player2 = GameObject.Find("Player2");
                switch (player2.GetComponent<PlayerController>().portName)
                {
                    case "COM12":
                        player2Ports.value = 11;
                        break;
                    case "COM11":
                        player2Ports.value = 10;
                        break;
                    case "COM10":
                        player2Ports.value = 9;
                        break;
                    case "COM9":
                        player2Ports.value = 8;
                        break;
                    case "COM8":
                        player2Ports.value = 7;
                        break;
                    case "COM7":
                        player2Ports.value = 6;
                        break;
                    case "COM6":
                        player2Ports.value = 5;
                        break;
                    case "COM5":
                        player2Ports.value = 4;
                        break;
                    case "COM4":
                        player2Ports.value = 3;
                        break;
                    case "COM3":
                        player2Ports.value = 2;
                        break;
                    case "COM2":
                        player2Ports.value = 1;
                        break;
                    case "COM1":
                        player2Ports.value = 0;
                        break;
                }
            }
            catch (Exception)
            {
                player2Ports.interactable = false;
            }
        }
    }

    private void Update()
    {
        if (!isInMainMenu)
        {
            if (comPortContainer.activeSelf && !GameManager.Instance.IsPaused)
            {
                comPortContainer.SetActive(false);
            }

            if (GameManager.Instance.IsPaused)
            {
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    ShowUI();
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ShowUI();
            }
        }
    }

    private void ShowUI()
    {
        if (comPortContainer.activeSelf == false)
        {
            comPortContainer.SetActive(true);
        }
        else
        {
            comPortContainer.SetActive(false);
        }
    }

    #region Port changing

    public void ChangePlayer1Port()
    {
        var newPort = "COM" + (player1Ports.value + 1);
        if (isInMainMenu)
        {
            var mController = mainMenuController.GetComponent<MainMenuController>();

            if (mController.portName != newPort)
            {
                mController.portName = newPort;

                // Turns the component off and on to destroy serial thread and open new thread with new port number
                mController.enabled = false;
                mController.enabled = true;
            }
        }
        else
        {
            var pController = player1.GetComponent<PlayerController>();

            if (pController.portName != newPort)
            {
                pController.portName = newPort;

                // Turns the component off and on to destroy serial thread and open new thread with new port number
                pController.enabled = false;
                pController.enabled = true;
            }
        }

        // Changes port name on prefab to newPort.
        var prefabController = player1Prefab.GetComponent<PlayerController>();
        prefabController.portName = newPort;
    }

    public void ChangePlayer2Port()
    {
        var newPort = "COM" + (player2Ports.value + 1);
        var pController = player2.GetComponent<PlayerController>();
        var prefabController = player1Prefab.GetComponent<PlayerController>();

        if (pController.portName != newPort)
        {
            pController.portName = newPort;
            prefabController.portName = newPort;

            // Turns the component off and on to destroy serial thread and open new thread with new port number
            pController.enabled = false;
            pController.enabled = true;
        }
    }

    #endregion
}