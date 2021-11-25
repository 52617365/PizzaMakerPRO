using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PizzaUI : MonoBehaviour
{
    private void Awake()
    {
        progressBar.color = progressBarColor;
    }

    private void Update()
    {
        if (GameManager.Instance.GameOver)
        {
            return;
        }

        remainingTime -= Time.deltaTime;
        var percent = remainingTime / maxTime;
        timerText.text = remainingTime.ToString("F0");
        progressBar.fillAmount = Mathf.Lerp(0, 1, percent);
        if (progressBar.fillAmount < 0.35f && lerpTime < 1)
        {
            progressBar.color = Color.Lerp(progressBarColor, Color.red, lerpTime);
            lerpTime += DefaultValues.lerpTimeIncrement;
        }
        else if (progressBar.fillAmount > 0.35f)
        {
            progressBar.color = progressBarColor;
        }

        if (!(remainingTime <= 0))
        {
            return;
        }

        foreach (var order in GameManager.Instance.CurrentOrders)
        {
            if (order.UIElement == this)
            {
                order.isInUse = false;
                order.UIElement = null;
                GameManager.Instance.CurrentOrders.Remove(order);
                break;
            }
        }

        Destroy(gameObject);
    }

    #region Public methods

    /// <summary>
    ///     Updates UI elements
    /// </summary>
    /// <param name="pizza"></param>
    /// <param name="time"></param>
    public void UpdateElements(PizzaSO pizza, float time)
    {
        nameText.text = pizza.pizzaName;
        remainingTime = time;
        maxTime = time;
        foreach (var ingredient in pizza.ingredients)
        {
            ingredients.Add(ingredient);
            var go = Instantiate(ingredientIconPrefab, ingredientContainer.transform, false);
            go.GetComponent<Image>().sprite = ingredient.icon;
            go.SetActive(true);
        }
    }

    #endregion

    #region Private attributes

    [SerializeField] private Image progressBar;

    [SerializeField] private Color progressBarColor;

    /// <summary>
    ///     Parent GameObject of ingredient icons.
    /// </summary>
    [SerializeField] private GameObject ingredientContainer;

    /// <summary>
    ///     Prefab of ingredient icon.
    /// </summary>
    [SerializeField] private GameObject ingredientIconPrefab;

    /// <summary>
    ///     List of ingredients.
    /// </summary>
    [HideInInspector] [SerializeField] private List<IngredientSO> ingredients;

    /// <summary>
    ///     T value of Color.Lerp method.
    /// </summary>
    [SerializeField] [Range(0f, 1f)] private float lerpTime;

    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private TextMeshProUGUI nameText;

    /// <summary>
    ///     Time remaining to bake pizza.
    /// </summary>
    [SerializeField] private float remainingTime;

    /// <summary>
    ///     Amount of time that order starts with.
    /// </summary>
    [SerializeField] private float maxTime;

    #endregion

    #region Getters and setters

    public List<IngredientSO> Ingredients => ingredients;
    public float RemainingTime => remainingTime;
    public float MaxTime => maxTime;

    #endregion
}
