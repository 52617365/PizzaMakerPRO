using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> highScores;

    private void Awake()
    {
        int sceneIndex = 1;
        foreach (var text in highScores)
        {
            if (sceneIndex <= SceneManager.sceneCountInBuildSettings - 1)
            {
                string query = "HighScore Level" + sceneIndex;
                Debug.Log(query);
                if (PlayerPrefs.HasKey(query))
                    text.text = "Highscore : " + PlayerPrefs.GetInt(query).ToString();
                else
                    text.text = "Highscore : 0";
            }
            else
                text.text = "Highscore : 0";
            sceneIndex++;
        }
    }
}
