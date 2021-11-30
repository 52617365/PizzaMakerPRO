using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> highScores;

    private void Awake()
    {
        var sceneIndex = 1;
        foreach (TextMeshProUGUI text in highScores)
        {
            if (sceneIndex <= SceneManager.sceneCountInBuildSettings - 1)
            {
                string query = "HighScore Level" + sceneIndex;
                if (PlayerPrefs.HasKey(query))
                {
                    text.text = "Highscore : " + PlayerPrefs.GetInt(query);
                }
                else
                {
                    text.text = "Highscore : 0";
                }
            }
            else
            {
                text.text = "Highscore : 0";
            }

            sceneIndex++;
        }
    }

    public void LoadLevel(int levelIndex)
    {
        LevelChanger.Instance.FadeToLevel(levelIndex);
    }
}
