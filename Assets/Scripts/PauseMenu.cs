using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    private static PauseMenu _instance;
    public static PauseMenu Instance { get { return _instance; } }

    [SerializeField]
    private List<GameObject> pauseMenuButtons;

    private int buttonIndex;

    private void Awake()
    {
        // Singleton pattern to only have single instance
        // of PauseMenu on scene.
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    public void Show()
    {
        EventSystem.current.SetSelectedGameObject(pauseMenuButtons[0]);
        EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
        buttonIndex = 0;
    }

    public void SelectButton()
    {
        switch (buttonIndex)
        {
            case 1:
                buttonIndex = 0;
                break;
            case 0:
                buttonIndex = 1;
                break;
            default:
                break;
        }

        EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.defaultButtonScale;
        EventSystem.current.SetSelectedGameObject(pauseMenuButtons[buttonIndex]);
        EventSystem.current.currentSelectedGameObject.transform.localScale = DefaultValues.selectedButtonScale;
    }

    public void Click()
    {
        EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
        return;
    }
}
