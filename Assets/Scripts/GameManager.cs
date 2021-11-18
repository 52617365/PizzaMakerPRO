using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages state of the game
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Class attributes
    /// <summary>
    /// Used in singleton pattern to ensure that there is only
    /// one GameManager instance in scene.
    /// </summary>
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [SerializeField]
    private PizzaDatabase pizzaDatabase;

    [SerializeField]
    private List<OrderSO> orderScriptableObjects;

    [SerializeField]
    public List<OrderSO> currentOrders;
    #endregion

    #region Private attributes
    /// <summary>
    /// GameObject that will be parent object of orderPrefab.
    /// </summary>
    [SerializeField]
    private GameObject container;

    /// <summary>
    /// Location that player 2 will be spawned on.
    /// </summary>
    [SerializeField]
    private GameObject playerSpawnPoint;
    /// <summary>
    /// Prefab of order UI Element.
    /// </summary>
    [Header("Prefabs")]
    [Space(5)]
    [SerializeField]
    private GameObject orderPrefab;

    /// <summary>
    /// Prefab of ingredient icon.
    /// </summary>
    [SerializeField]
    private GameObject ingredientIconPrefab;
    /// <summary>
    /// Prefab of pizza box
    /// </summary>
    [SerializeField]
    private GameObject pizzaBoxPrefab;
    /// <summary>
    /// Prefab of player 2.
    /// </summary>
    [SerializeField]
    private GameObject playerTwoPrefab;

    // Text references
    [Header("Text references")]
    [Space(5)]
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI gameDurationText;
    [SerializeField]
    private TextMeshProUGUI gameEndPointsText;

    [Space(10)]
    [SerializeField]
    private GameObject countdownContainer;
    [SerializeField]
    private List<GameObject> countdownObjects;

    [SerializeField]
    private GameObject gameEndScreen;
    [SerializeField]
    private GameObject leftButtonContainer;
    [SerializeField]
    private GameObject rightButtonContainer;

    [SerializeField]
    private GameObject pauseMenu;

    public GameObject IngredientIconPrefab { get { return ingredientIconPrefab; } }
    public GameObject PizzaBoxPrefab { get { return pizzaBoxPrefab; } }
    /// <summary>
    /// Amount of points that player has earned during gameplay.
    /// </summary>
    public int playerScore { get; set; }
    /// <summary>
    /// Time remaining on current game.
    /// </summary>
    public float GameDurationLeft { get; set; }
    public bool IsPaused { get; set; }
    public bool GameOver { get; set; }
    public bool GameStarted { get; set; }
    public List<OrderSO> CurrentOrders
    {
        get { return currentOrders; }
        set { currentOrders = value; }
    }
    #endregion

    private void Awake()
    {
        // Singleton pattern to only have single instance
        // of GameManager on scene.
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        try
        {
            if (LevelChanger.Instance.PlayerCount == 2)
                AddPlayerTwo();
        }
        catch (System.Exception)
        {
            // Do nothing.
        }
        

        foreach (var order in orderScriptableObjects)
        {
            order.isInUse = false;
            order.UIElement = null;
        }

        GameDurationLeft = DefaultValues.gameDuration;
        scoreText.text = playerScore.ToString();
        StartCoroutine("Countdown");
    }

    private void Update()
    {
        if (!GameStarted || IsPaused)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
            NewOrder();

        if (Input.GetKeyDown(KeyCode.Z))
            AddPlayerTwo();

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
                EndGame();
        }
    }

    #region Private methods
    private void AddPlayerTwo()
    {
        GameObject go = Instantiate(playerTwoPrefab, playerSpawnPoint.transform.position, playerTwoPrefab.transform.rotation);
        go.name = "Player2";
    }

    /// <summary>
    /// Generates new order and instantiates UI Element that
    /// displays required ingredients and time to bake pizza within.
    /// </summary>
    private void NewOrder()
    {
        if (GameOver)
            return;

        if (currentOrders.Count == orderScriptableObjects.Count)
        {
            Debug.Log("At maximum amount of orders");
            return;
        }

        int randomValue = Random.Range(1, 6);
        Debug.Log(randomValue);
        if (randomValue >= 4 || currentOrders.Count == 0)
        {
            GetComponent<AudioSource>().Play();
            PizzaSO randomPizza = pizzaDatabase.pizzas[Random.Range(0, pizzaDatabase.pizzas.Count())];

            float timeToBakePizza = Random.Range(DefaultValues.minTimerValue, DefaultValues.maxTimerValue);


            // Loops through all orders until it finds one that is not in use
            foreach (var order in orderScriptableObjects)
            {
                if (!order.isInUse)
                {

                    GameObject go = Instantiate(orderPrefab);
                    go.transform.SetParent(container.transform, false);
                    go.GetComponent<PizzaUI>().UpdateElements(randomPizza, timeToBakePizza);
                    order.UIElement = go.GetComponent<PizzaUI>();
                    go.SetActive(true);
                    order.isInUse = true;
                    currentOrders.Add(order);
                    break;
                }
            }
        }
    }

    private void EndGame()
    {
        GameOver = true;
        CancelInvoke();
        gameEndPointsText.text = playerScore.ToString();
        gameEndScreen.SetActive(true);
        // TODO: Implement something that indicates that game is over.
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(2);

        foreach (var go in countdownObjects)
        {
            go.SetActive(true);
            go.GetComponent<AudioSource>().Play();
            if (go == countdownObjects[countdownObjects.Count - 1])
                GameStarted = true;
            yield return new WaitForSeconds(1);
            go.SetActive(false);
        }
        InvokeRepeating("NewOrder", 0, 10f);
        gameDurationText.gameObject.SetActive(true);
    }
    #endregion

    #region Public methods
    /// <summary>
    /// Increases playerScore by value of amount
    /// </summary>
    /// <param name="amount"></param>
    public static void UpdateScore(float timeLeftPercentage)
    {
        float amount = DefaultValues.pizzaPointValue * timeLeftPercentage;
        if (timeLeftPercentage >= DefaultValues.fastDeliveryThreshold)
            amount += DefaultValues.fastDeliveryBonus;

        _instance.playerScore += (int)amount;
        _instance.scoreText.text = _instance.playerScore.ToString("F0");
    }

    public void ClearOrder(OrderSO order)
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
        catch (System.Exception)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void Restart()
    {
        rightButtonContainer.GetComponent<Image>().enabled = true;
        float wait = 1f;
        while (wait > 0)
            wait -= Time.deltaTime;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion

    private void OnApplicationQuit()
    {
        string levelName = "HighScore Level" + SceneManager.GetActiveScene().buildIndex;
        if (PlayerPrefs.HasKey(levelName))
        {
            if (PlayerPrefs.GetInt(levelName) < playerScore)
                PlayerPrefs.SetInt(levelName, playerScore);
        }
        else
        {
            PlayerPrefs.SetInt(levelName, playerScore);
        }

        foreach (var order in orderScriptableObjects)
        {

            order.isInUse = false;
            order.UIElement = null;
        }
    }
}
