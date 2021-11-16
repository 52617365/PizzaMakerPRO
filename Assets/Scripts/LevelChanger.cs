using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    private static LevelChanger _instance;
    public static LevelChanger Instance { get { return _instance; } }

    // Animator reference
    [SerializeField]
    private Animator animator;

    private int levelToLoad;

    private int playerCount;

    public int PlayerCount
    {
        get { return playerCount; }
        set { playerCount = value; }
    }

    private void Awake()
    {
        // Singleton pattern to only have single instance
        // of LevelChanger on scene.
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void FadeToLevel (int levelIndex)
    {
        Debug.Log("ASD");
        levelToLoad = levelIndex;

        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
        animator.SetTrigger("FadeIn");
    }
}
