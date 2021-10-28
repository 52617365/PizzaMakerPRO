using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    /// Prefab of order UI Element.
    /// </summary>
    [SerializeField]
    private GameObject orderPrefab;

    /// <summary>
    /// Prefab of ingredient icon.
    /// </summary>
    [SerializeField]
    private GameObject ingredientIconPrefab;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    public GameObject IngredientIconPrefab { get { return ingredientIconPrefab; } }

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

        foreach (var order in orderScriptableObjects)
        {
            order.isInUse = false;
            order.UIElement = null;
        }

        GameDurationLeft = DefaultValues.gameDuration;
        scoreText.text = playerScore.ToString();
        InvokeRepeating("NewOrder", 0, 10f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            NewOrder();

        if (!GameOver)
        {
            if (!IsPaused)
                GameDurationLeft -= Time.deltaTime;

            if (GameDurationLeft <= 0)
                EndGame();
        }
    }

    #region Private methods
    /// <summary>
    /// Generates new order and instantiates UI Element that
    /// displays required ingredients and time to bake pizza within.
    /// </summary>
    private void NewOrder()
    {
        if (currentOrders.Count == orderScriptableObjects.Count)
        {
            Debug.Log("At maximum amount of orders");
            return;
        }

        int randomValue = Random.Range(1, 6);
        Debug.Log(randomValue);
        if (randomValue >= 4)
        {
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
        // TODO: Implement something that indicates that game is over.
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
    #endregion

    private void OnApplicationQuit()
    {
        if (PlayerPrefs.HasKey("HighScore"))
        {
            if (PlayerPrefs.GetInt("HighScore") < playerScore)
                PlayerPrefs.SetInt("HighScore", playerScore);
        }
        else
        {
            PlayerPrefs.SetInt("HighScore", playerScore);
        }

        foreach (var order in orderScriptableObjects)
        {

            order.isInUse = false;
            order.UIElement = null;
        }
    }
}
