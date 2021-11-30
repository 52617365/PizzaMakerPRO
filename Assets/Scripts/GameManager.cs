using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
///     Manages state of the game
/// </summary>
public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        // Singleton pattern to only have single instance
        // of GameManager on scene.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        GameObject go = Instantiate(playerOnePrefab, playerOneSpawnPoint.transform.position,
                                    playerOnePrefab.transform.rotation);
        go.name = "Player1";
        if (!disablePlayerScoring)
        {
            playerOnePointsContainer.SetActive(true);
            player1ScoreText.text = go.GetComponent<Player>().PlayerScore.ToString("F0");
        }

        try
        {
            if (LevelChanger.Instance.PlayerCount == 2)
            {
                AddPlayerTwo();
            }
        }
        catch (Exception)
        {
            // Do nothing.
        }


        foreach (OrderSO order in orderScriptableObjects)
        {
            order.isInUse = false;
            order.UIElement = null;
        }

        GameDurationLeft = DefaultValues.gameDuration;
        scoreText.text = PlayerScore.ToString();
        StartCoroutine("Countdown");
    }

    private void Update()
    {
        if (!GameStarted || IsPaused)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            NewOrder();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            AddPlayerTwo();
        }

        if (!GameOver)
        {
            if (!IsPaused)
            {
                GameDurationLeft -= Time.deltaTime;
                float minutes = Mathf.FloorToInt(GameDurationLeft / 60);
                float seconds = Mathf.FloorToInt(GameDurationLeft % 60);
                gameDurationText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            if (GameDurationLeft <= 0)
            {
                EndGame();
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveScore();
        foreach (OrderSO order in orderScriptableObjects)
        {
            order.isInUse = false;
            order.UIElement = null;
        }
    }

    private void SaveScore()
    {
        string levelName = "HighScore Level" + SceneManager.GetActiveScene().buildIndex;
        if (PlayerPrefs.HasKey(levelName))
        {
            if (PlayerPrefs.GetInt(levelName) < PlayerScore)
            {
                PlayerPrefs.SetInt(levelName, PlayerScore);
            }
        }
        else
        {
            PlayerPrefs.SetInt(levelName, PlayerScore);
        }
    }

    #region Class attributes

    /// <summary>
    ///     Used in singleton pattern to ensure that there is only
    ///     one GameManager instance in scene.
    /// </summary>
    public static GameManager Instance { get; private set; }

    [SerializeField] private PizzaDatabase pizzaDatabase;

    [SerializeField] private List<OrderSO> orderScriptableObjects;

    [SerializeField] public List<OrderSO> currentOrders;

    #endregion

    #region Private attributes

    /// <summary>
    ///     GameObject that will be parent object of orderPrefab.
    /// </summary>
    [SerializeField] private GameObject container;

    /// <summary>
    ///     Location that Player 1 will be spawned on.
    /// </summary>
    [SerializeField] private GameObject playerOneSpawnPoint;

    /// <summary>
    ///     Location that Player 2 will be spawned on.
    /// </summary>
    [SerializeField] private GameObject playerTwoSpawnPoint;

    /// <summary>
    ///     Prefab of order UI Element.
    /// </summary>
    [Header("Prefabs")] [Space(5)] [SerializeField]
    private GameObject orderPrefab;

    /// <summary>
    ///     Prefab of ingredient icon.
    /// </summary>
    [SerializeField] private GameObject ingredientIconPrefab;

    /// <summary>
    ///     Prefab of pizza box.
    /// </summary>
    [SerializeField] private GameObject pizzaBoxPrefab;

    /// <summary>
    /// Prefab of pizza.
    /// </summary>
    [SerializeField] private GameObject pizzaPrefab;

    /// <summary>
    ///     Prefab of Player 1.
    /// </summary>
    [SerializeField] private GameObject playerOnePrefab;

    /// <summary>
    ///     Prefab of Player 2.
    /// </summary>
    [SerializeField] private GameObject playerTwoPrefab;

    // Text references
    [Header("Text references")] [Space(5)] [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField] private TextMeshProUGUI player1ScoreText;

    [SerializeField] private TextMeshProUGUI player2ScoreText;

    [SerializeField] private TextMeshProUGUI gameDurationText;

    [SerializeField] private TextMeshProUGUI gameEndPointsText;

    [SerializeField] private TextMeshProUGUI newHighScoreText;

    [Header("UI references")] [Space(10)] [SerializeField]
    private GameObject countdownContainer;

    [SerializeField] private List<GameObject> countdownObjects;

    [SerializeField] private GameObject gameEndScreen;

    [SerializeField] private GameObject leftButtonContainer;

    [SerializeField] private GameObject rightButtonContainer;

    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private GameObject playerTwoDashContainer;

    [SerializeField] private Image p1DashProgressBar;

    [SerializeField] private Image p2DashProgressBar;

    [SerializeField] private GameObject playerOnePointsContainer;

    [SerializeField] private GameObject playerTwoPointsContainer;

    public Image P1DashProgressBar => p1DashProgressBar;
    public Image P2DashProgressBar => p2DashProgressBar;
    public GameObject IngredientIconPrefab => ingredientIconPrefab;
    public GameObject PizzaBoxPrefab => pizzaBoxPrefab;
    public GameObject PizzaPrefab => pizzaPrefab;

    /// <summary>
    ///     Amount of points that player has earned during gameplay.
    /// </summary>
    public int PlayerScore { get; set; }

    /// <summary>
    ///     Time remaining on current game.
    /// </summary>
    public float GameDurationLeft { get; set; }

    // Used to disable player specific scoring in levels where
    // it doesn't really make sense to have it.
    public bool disablePlayerScoring = false;

    public bool IsPaused { get; private set; }
    public bool GameOver { get; private set; }
    public bool GameStarted { get; private set; }

    public List<OrderSO> CurrentOrders => currentOrders;

    #endregion

    #region Private methods

    private void AddPlayerTwo()
    {
        GameObject go = Instantiate(playerTwoPrefab, playerTwoSpawnPoint.transform.position,
                                    playerTwoPrefab.transform.rotation);
        go.name = "Player2";
        playerTwoDashContainer.SetActive(true);
        if (!disablePlayerScoring)
        {
            player2ScoreText.text = go.GetComponent<Player>().PlayerScore.ToString("F0");
            playerTwoPointsContainer.SetActive(true);
        }
    }

    /// <summary>
    ///     Generates new order and instantiates UI Element that
    ///     displays required ingredients and time to bake pizza within.
    /// </summary>
    private void NewOrder()
    {
        if (GameOver)
        {
            return;
        }

        if (currentOrders.Count == orderScriptableObjects.Count)
        {
            Debug.Log("At maximum amount of orders");
            return;
        }

        int randomValue = Random.Range(1, 6);

        int threshold = CurrentOrders.Count switch
        {
            4 => 3,
            3 => 3,
            2 => 3,
            1 => 2,
            0 => 1,
            _ => 6
        };

        Debug.Log("Random value: " + randomValue + ", Threshold:" + threshold);
        if (randomValue < threshold)
        {
            return;
        }

        GetComponent<AudioSource>().Play();
        PizzaSO randomPizza = pizzaDatabase.pizzas[Random.Range(0, pizzaDatabase.pizzas.Count)];

        float timeToBakePizza = Random.Range(DefaultValues.minTimerValue, DefaultValues.maxTimerValue);


        // Loops through all orders until it finds one that is not in use
        foreach (OrderSO order in orderScriptableObjects)
        {
            if (!order.isInUse)
            {
                GameObject go = Instantiate(orderPrefab, container.transform, false);
                go.GetComponent<PizzaUI>().UpdateElements(randomPizza, timeToBakePizza);
                order.UIElement = go.GetComponent<PizzaUI>();
                go.SetActive(true);
                order.isInUse = true;
                currentOrders.Add(order);
                break;
            }
        }
    }

    private void EndGame()
    {
        GameOver = true;
        CancelInvoke();
        gameEndPointsText.text = PlayerScore.ToString();
        string levelName = "HighScore Level" + SceneManager.GetActiveScene().buildIndex;
        if (PlayerPrefs.HasKey(levelName))
        {
            if (PlayerPrefs.GetInt(levelName) < PlayerScore)
            {
                newHighScoreText.gameObject.SetActive(true);
            }
        }
        else
        {
            newHighScoreText.gameObject.SetActive(true);
        }

        gameEndScreen.SetActive(true);
        SaveScore();
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(2);

        foreach (GameObject go in countdownObjects)
        {
            go.SetActive(true);
            go.GetComponent<AudioSource>().Play();
            if (go == countdownObjects[countdownObjects.Count - 1])
            {
                GameStarted = true;
            }

            yield return new WaitForSeconds(1);
            go.SetActive(false);
        }

        var repeatTime = 10f;
        try
        {
            if (LevelChanger.Instance.PlayerCount == 2)
            {
                repeatTime = 7f;
            }
        }
        catch (Exception)
        {
            // Do nothing.
        }

        InvokeRepeating("NewOrder", 0, repeatTime);
        gameDurationText.gameObject.SetActive(true);
    }

    #endregion

    #region Public methods

    /// <summary>
    ///     Increases playerScore by value of amount
    /// </summary>
    /// <param name="amount"></param>
    public static void UpdateScore(float timeLeftPercentage)
    {
        float amount = DefaultValues.pizzaPointValue * timeLeftPercentage;
        if (timeLeftPercentage >= DefaultValues.fastDeliveryThreshold)
        {
            amount += DefaultValues.fastDeliveryBonus;
        }

        Instance.PlayerScore += (int) amount;
        Instance.scoreText.text = Instance.PlayerScore.ToString("F0");
    }

    public void UpdatePlayerScoreText(int playerNumber, int scoreAmount)
    {
        switch (playerNumber)
        {
            case 1: // Player 2
                player2ScoreText.text = scoreAmount.ToString("F0");
                break;
            case 0: // Player 1
                player1ScoreText.text = scoreAmount.ToString("F0");
                break;
        }
    }

    public static void ClearOrder(OrderSO order)
    {
        Destroy(order.UIElement.gameObject);

        order.isInUse = false;
        order.UIElement = null;
    }

    public void Pause()
    {
        Time.timeScale = 0;
        IsPaused = true;
        pauseMenu.SetActive(true);
        PauseMenu.Instance.Show();
    }

    public void Unpause()
    {
        pauseMenu.SetActive(false);
        IsPaused = false;
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        try
        {
            LevelChanger.Instance.FadeToLevel(0);
        }
        catch (Exception)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void Restart()
    {
        rightButtonContainer.GetComponent<Image>().enabled = true;
        var wait = 1f;
        while (wait > 0)
        {
            wait -= Time.deltaTime;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion
}
