using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PizzaOven : MonoBehaviour
{
    [SerializeField] private Image progressBar;

    [SerializeField] private Color[] progressBarColor;

    [SerializeField] private Color[] defaultColors;

    [SerializeField] private GameObject UI;

    [SerializeField] private ParticleSystem smokeEffect;

    [SerializeField] private bool pizzaInOven;

    [SerializeField] private List<GameObject> activeIcons;

    [SerializeField] private List<IngredientSO> ingredients;

    private float maxTime;

    private bool pizzaIsBurnt;

    private float remainingTime;
    public Material HighlightMaterial { get; private set; }
    private Material LightMaterial { get; set; }

    private void Awake()
    {
        var rendererComponent = GetComponent<Renderer>();
        foreach (var material in rendererComponent.materials)
        {
            switch (material.name)
            {
                case "Top (Instance)":
                    HighlightMaterial = material;
                    break;
                case "Light (Instance)":
                    LightMaterial = material;
                    break;
            }
        }

        LightMaterial.EnableKeyword("_EMISSION");

        // Checks if ui is active and disables if it is.
        if (UI.activeSelf)
        {
            UI.SetActive(false);
        }

        progressBar.color = progressBarColor[0];
    }

    private void Update()
    {
        if (!pizzaInOven)
        {
            return;
        }

        if (!PizzaIsReady)
        {
            remainingTime -= Time.deltaTime;
            var percent = remainingTime / maxTime;
            progressBar.fillAmount = Mathf.Lerp(0, 1, percent);
            if (remainingTime <= 0)
            {
                GetComponent<AudioSource>().Play();
                PizzaIsReady = true;
                remainingTime = 0;
            }
        }
        // Burn timer for pizza after it is ready.
        else
        {
            if (progressBar.color != progressBarColor[1])
            {
                progressBar.color = progressBarColor[1];
            }


            remainingTime += Time.deltaTime;
            var percent = remainingTime / maxTime;
            progressBar.fillAmount = Mathf.Lerp(0, 1, percent);
        }

        if (!pizzaIsBurnt && PizzaIsReady && remainingTime >= maxTime)
        {
            pizzaIsBurnt = true;
            if (smokeEffect.gameObject.activeSelf == false)
            {
                smokeEffect.gameObject.SetActive(true);
                smokeEffect.Play();
            }
        }
    }

    public void AddPizzaToOven(HeldPizzaSO pizza, Player player)
    {
        if (player.HeldPizza != null)
        {
            player.HeldPizza = null;
        }

        ingredients.Clear();

        LightMaterial.color = defaultColors[3];
        LightMaterial.SetColor("_EmissionColor", Color.red);

        maxTime = Random.Range(10, 16);
        remainingTime = maxTime;
        progressBar.fillAmount = 0;

        foreach (var ingredient in pizza.ingredients)
        {
            ingredients.Add(ingredient);
        }

        // Instantiates IngredientIconPrefabs to display what type of
        // pizza is currently in oven.
        foreach (var ingredient in ingredients) //pizzaSO.ingredients)
        {
            var go = Instantiate(GameManager.Instance.IngredientIconPrefab, gameObject.transform.Find("Canvas").Find("Container").Find("IconContainer"), false);
            activeIcons.Add(go);
            go.GetComponent<Image>().sprite = ingredient.icon;
            go.SetActive(true);
        }

        // Clears the list of ingredients in pizza scriptable object.
        pizza.ingredients.Clear();

        player.ClearActiveIcons();
        Destroy(player.InstantiatedGameObject);
        player.InstantiatedGameObject = null;
        player.GetComponent<Animator>().SetFloat("HoldingPizza", 0);

        pizzaInOven = true;
        progressBar.color = progressBarColor[0];
        UI.SetActive(true);
    }

    public void TakePizza(Player player)
    {
        pizzaInOven = false;
        LightMaterial.color = defaultColors[2];
        LightMaterial.SetColor("_EmissionColor", Color.green);
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

        if (PizzaIsReady)
        {
            if (!pizzaIsBurnt)
            {
                player.HeldPizza.cookState = HeldPizzaSO.CookState.Cooked;
            }
            else
            {
                player.HeldPizza.cookState = HeldPizzaSO.CookState.Burnt;
                smokeEffect.Stop();
                smokeEffect.gameObject.SetActive(false);
            }

            PizzaIsReady = false;
            player.InstantiatedGameObject = Instantiate(GameManager.Instance.PizzaBoxPrefab, player.PizzaBoxContainer.transform, false);
            player.GetComponent<Animator>().SetFloat("HoldingPizzaBox", 1);

        }
        else
        {
            player.InstantiatedGameObject = Instantiate(GameManager.Instance.PizzaPrefab, player.PizzaBoxContainer.transform, false);
            player.GetComponent<Animator>().SetFloat("HoldingPizza", 1);
        }

        ingredients.Clear();
        activeIcons.Clear();
        UI.SetActive(false);
        pizzaIsBurnt = false;
    }

    #region Getters and setters

    public bool PizzaInOven => pizzaInOven;

    public bool PizzaIsReady { get; private set; }

    public Color[] TopMaterialColor => defaultColors;

    #endregion
}
