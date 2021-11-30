using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private List<GameObject> pauseMenuButtons;

    private float buttonCooldown;

    private int buttonIndex;
    public static PauseMenu Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern to only have single instance
        // of PauseMenu on scene.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (buttonCooldown > 0f)
        {
            buttonCooldown -= 0.005f;
        }
    }

    public void Show()
    {
        EventSystem.current.SetSelectedGameObject(pauseMenuButtons[0]);
        EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
        buttonIndex = 0;
    }

    public void SelectButton()
    {
        if (!(buttonCooldown <= 0))
        {
            return;
        }

        buttonCooldown = 0.25f;
        switch (buttonIndex)
        {
            case 1:
                buttonIndex = 0;
                break;
            case 0:
                buttonIndex = 1;
                break;
            default:
                buttonIndex = buttonIndex;
                break;
        }

        EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.defaultButtonScale;
        EventSystem.current.SetSelectedGameObject(pauseMenuButtons[buttonIndex]);
        EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
    }

    public static void Click()
    {
        EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
    }
}
