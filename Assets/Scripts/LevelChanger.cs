using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    // Animator reference
    [SerializeField] private Animator animator;

    private int levelToLoad;
    public static LevelChanger Instance { get; private set; }

    public int PlayerCount { get; set; }

    private void Awake()
    {
        // Singleton pattern to only have single instance
        // of LevelChanger on scene.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void FadeToLevel(int levelIndex)
    {
        levelToLoad = levelIndex;

        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
        animator.SetTrigger("FadeIn");
    }
}
