using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PizzaOven : MonoBehaviour
{
    public Material highlightMaterial { get; private set; } 
    public Material lightMaterial { get; private set; }
    [SerializeField]
    private Image progressBar;
    [SerializeField]
    private Color[] progressBarColor;
    [SerializeField]
    private Color[] defaultColors;
    [SerializeField]
    private GameObject UI;

    private float remainingTime;
    private float maxTime;

    [SerializeField]
    private bool pizzaInOven;
    private bool pizzaIsReady;

    [SerializeField]
    private List<GameObject> activeIcons;

    [SerializeField]
    private List<IngredientSO> ingredients;

    private bool pizzaIsBurnt;

    #region Getters and setters
    public bool PizzaInOven
    {
        get { return pizzaInOven; }
    }

    public bool PizzaIsReady
    {
        get { return pizzaIsReady; }
    }
    public Color[] TopMaterialColor { get { return defaultColors; } }
    #endregion

    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        foreach (var material in renderer.materials)
        {
            if (material.name == "Top (Instance)")
                highlightMaterial = material;
            if (material.name == "Light (Instance)")
                lightMaterial = material;
        }
        lightMaterial.EnableKeyword("_EMISSION");

        // Checks if ui is active and disables if it is.
        if (UI.activeSelf == true)
            UI.SetActive(false);
        progressBar.color = progressBarColor[0];
    }

    private void Update()
    {
        if (pizzaInOven)
        {
            if (!pizzaIsReady)
            {
                remainingTime -= Time.deltaTime;
                float percent = remainingTime / maxTime;
                progressBar.fillAmount = Mathf.Lerp(0, 1, percent);
                if (remainingTime <= 0)
                {
                    GetComponent<AudioSource>().Play();
                    pizzaIsReady = true;
                    remainingTime = 0;
                }
            }
            // Burn timer for pizza after it is ready.
            else
            {
                if (progressBar.color != progressBarColor[1])
                    progressBar.color = progressBarColor[1];

                remainingTime += Time.deltaTime;
                float percent = remainingTime / maxTime;
                progressBar.fillAmount = Mathf.Lerp(0, 1, percent);
            }

            if (!pizzaIsBurnt && pizzaIsReady && remainingTime >= maxTime)
                pizzaIsBurnt = true;
        }
    }

    public void AddPizzaToOven(HeldPizzaSO pizza, Player player)
    {
        if (player.HeldPizza != null)
            player.HeldPizza = null;

        ingredients.Clear();

        lightMaterial.color = defaultColors[3];
        lightMaterial.SetColor("_EmissionColor", Color.red);

        maxTime = Random.Range(10, 16);
        remainingTime = maxTime;
        progressBar.fillAmount = 0;

        foreach (var ingredient in pizza.ingredients)
        {
            ingredients.Add(ingredient);
        }

        // Instantiates IngredientIconPrefabs to display what type of
        // pizza is currently in oven.
        foreach (var ingredient in ingredients)//pizzaSO.ingredients)
        {
            GameObject go = Instantiate(GameManager.Instance.IngredientIconPrefab);
            activeIcons.Add(go);
            go.transform.SetParent(gameObject.transform.Find("Canvas").Find("Container").Find("IconContainer"), false);
            go.GetComponent<Image>().sprite = ingredient.icon;
            go.SetActive(true);
        }
        // Clears the list of ingredients in pizza scriptable object.
        pizza.ingredients.Clear();

        player.ClearActiveIcons();
        Destroy(player.instantiatedGameObject);
        player.instantiatedGameObject = null;
        player.GetComponent<Animator>().SetFloat("Holding", 0);

        pizzaInOven = true;
        progressBar.color = progressBarColor[0];
        UI.SetActive(true);
    }

    public void TakePizza(Player player)
    {
        pizzaInOven = false;
        lightMaterial.color = defaultColors[2];
        lightMaterial.SetColor("_EmissionColor", Color.green);
        foreach (var icon in activeIcons)
        {
            icon.transform.SetParent(player.transform.Find("Canvas").Find("IconContainer"), false);
            player.AddToActiveIcons(icon);
        }
        player.HeldPizza = player.HeldPizzaSO;
        foreach (var ingredient in ingredients)
        {
            player.HeldPizza.ingredients.Add(ingredient);
        }

        if (pizzaIsReady)
        {
            if (!pizzaIsBurnt)
                player.HeldPizza.cookState = HeldPizzaSO.CookState.Cooked;
            else
                player.HeldPizza.cookState = HeldPizzaSO.CookState.Burnt;
            pizzaIsReady = false;
        }
        player.instantiatedGameObject = Instantiate(GameManager.Instance.PizzaBoxPrefab);
        player.instantiatedGameObject.transform.SetParent(player.PizzaBoxContainer.transform, false);
        player.GetComponent<Animator>().SetFloat("Holding", 1);
        ingredients.Clear();
        activeIcons.Clear();
        UI.SetActive(false);
        pizzaIsBurnt = false;
    }
}
