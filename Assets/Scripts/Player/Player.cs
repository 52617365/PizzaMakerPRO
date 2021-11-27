using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum PlayerNumber
    {
        Player1,
        Player2
    }

    [SerializeField] private PlayerNumber playerNumber;

    public GameObject interactTarget;

    private void Awake()
    {
        // Gets PlayerController component if it is not assigned.
        if (controller == null)
        {
            controller = GetComponent<PlayerController>();
        }

        heldPizzaSO.ingredients.Clear();
        heldPizzaSO.cookState = HeldPizzaSO.CookState.Uncooked;

        // Loops CheckForInteractables method every 0.15 seconds.
        InvokeRepeating("CheckForInteractables", 0, 0.15f);
        StartCoroutine(DisplayPlayerNumber());
    }

    private void Update()
    {
        if (heldIngredient == null && ingredientIcon.gameObject.activeSelf)
        {
            ingredientIcon.gameObject.SetActive(false);
        }
        else if (heldIngredient != null && ingredientIcon.gameObject.activeSelf == false)
        {
            ingredientIcon.gameObject.SetActive(true);
        }
    }

    #region Interactable detection

    /// <summary>
    ///     Checks if there are any GameObjects tagged as "Ingredient", "Pizza",
    ///     "PizzaOven", "DeliveryPoint" or "DonationBox" and if so then it sets interactable to be that.
    /// </summary>
    private void CheckForInteractables()
    {
        if (!GameManager.Instance.GameStarted)
        {
            return;
        }

        // Calculates position where Ray will be cast.
        var pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        RaycastHit hit;
        var forwardRay = new Ray(pos, transform.forward);
        Debug.DrawRay(pos, transform.forward);

        if (Physics.Raycast(forwardRay, out hit))
        {
            if (hit.distance < detectionRange)
            {
                switch (hit.collider.tag)
                {
                    case "Pizza":
                        var detectedPizza = hit.transform.GetComponent<Pizza>();
                        if (ClosePizza != detectedPizza)
                        {
                            if (ClosePizza != null && ClosePizza.HighlightMaterial.GetColor("_Color") !=
                                ClosePizza.TopMaterialColor[0])
                            {
                                ClosePizza.HighlightMaterial.SetColor("_Color", ClosePizza.TopMaterialColor[0]);
                            }

                            ClosePizza = detectedPizza;
                            interactTarget = detectedPizza.gameObject;
                            ClosePizza.HighlightMaterial.SetColor("_Color", ClosePizza.TopMaterialColor[1]);
                        }

                        break;
                    case "Ingredient":
                        var detectedIngredient = hit.transform.GetComponent<Ingredient>();
                        if (CloseIngredient != detectedIngredient)
                        {
                            if (CloseIngredient != null && CloseIngredient.HighLightMaterial.GetTexture("_MainTex") !=
                                CloseIngredient.DefaultTexture)
                            {
                                CloseIngredient.HighLightMaterial.SetTexture("_MainTex",
                                    CloseIngredient.DefaultTexture);
                            }

                            CloseIngredient = detectedIngredient;
                            interactTarget = detectedIngredient.gameObject;
                            CloseIngredient.HighLightMaterial.SetTexture("_MainTex", CloseIngredient.HighLightTexture);
                        }

                        break;
                    case "PizzaOven":
                        var detectedPizzaOven = hit.transform.GetComponent<PizzaOven>();
                        if (ClosePizzaOven != detectedPizzaOven)
                        {
                            if (ClosePizzaOven != null && ClosePizzaOven.HighlightMaterial.GetColor("_Color") !=
                                ClosePizzaOven.TopMaterialColor[0])
                            {
                                ClosePizzaOven.HighlightMaterial.SetColor("_Color", ClosePizzaOven.TopMaterialColor[0]);
                            }

                            ClosePizzaOven = detectedPizzaOven;
                            interactTarget = detectedPizzaOven.gameObject;
                            ClosePizzaOven.HighlightMaterial.SetColor("_Color", ClosePizzaOven.TopMaterialColor[1]);
                        }

                        break;
                    case "DeliveryPoint":
                        var detectedDeliveryPoint = hit.transform.GetComponent<DeliveryPoint>();
                        if (CloseDeliveryPoint != detectedDeliveryPoint)
                        {
                            if (CloseDeliveryPoint != null && CloseDeliveryPoint.HighlightMaterial.GetColor("_Color") !=
                                CloseDeliveryPoint.TopMaterialColor[0])
                            {
                                CloseDeliveryPoint.HighlightMaterial.SetColor("_Color",
                                    CloseDeliveryPoint.TopMaterialColor[0]);
                            }

                            CloseDeliveryPoint = detectedDeliveryPoint;
                            interactTarget = detectedDeliveryPoint.gameObject;
                            CloseDeliveryPoint.HighlightMaterial.SetColor("_Color",
                                CloseDeliveryPoint.TopMaterialColor[1]);
                        }

                        if (HeldPizza != null)
                        {
                            if (HeldPizza.cookState == HeldPizzaSO.CookState.Burnt)
                            {
                                detectedDeliveryPoint.ShowBurntPizzaError();
                            }

                            if (HeldPizza.cookState == HeldPizzaSO.CookState.Uncooked)
                            {
                                detectedDeliveryPoint.ShowNotCookedError();
                            }
                        }

                        break;
                    case "DonationBox":
                        var detectedDonationBox = hit.transform.gameObject;
                        if (CloseTrashBin != detectedDonationBox)
                        {
                            CloseTrashBin = detectedDonationBox;
                            interactTarget = detectedDonationBox.gameObject;
                        }

                        break;
                }
            }

            ClearInteractables();

            // If hit distance is greater than detection range
            // this will then set interactTarget to null;
            if (hit.distance > detectionRange)
            {
                interactTarget = null;
            }
        }
    }

    private void ClearInteractables()
    {
        if (CloseIngredient != null && CloseIngredient.gameObject != interactTarget)
        {
            CloseIngredient.HighLightMaterial.SetTexture("_MainTex", CloseIngredient.DefaultTexture);
            CloseIngredient = null;
        }

        if (ClosePizza != null && ClosePizza.gameObject != interactTarget)
        {
            ClosePizza.HighlightMaterial.SetColor("_Color", ClosePizza.TopMaterialColor[0]);
            ClosePizza = null;
        }

        if (ClosePizzaOven != null && ClosePizzaOven.gameObject != interactTarget)
        {
            ClosePizzaOven.HighlightMaterial.SetColor("_Color", ClosePizzaOven.TopMaterialColor[0]);
            ClosePizzaOven = null;
        }

        if (CloseDeliveryPoint != null && CloseDeliveryPoint.gameObject != interactTarget)
        {
            CloseDeliveryPoint.HighlightMaterial.SetColor("_Color", CloseDeliveryPoint.TopMaterialColor[0]);
            CloseDeliveryPoint = null;
        }

        if (CloseTrashBin != null && CloseTrashBin.gameObject != interactTarget)
        {
            CloseTrashBin = null;
        }
    }
    #endregion

    public void PickUpPizza()
    {
        //if (heldIngredient != null)
        heldIngredient = null;

        heldPizzaSO.ingredients.Clear();
        foreach (var ingredient in ClosePizza.Ingredients)
        {
            heldPizzaSO.ingredients.Add(ingredient);
        }

        Destroy(ClosePizza.InstantiatedObject);

        HeldPizza = heldPizzaSO;

        foreach (var ingredient in HeldPizza.ingredients)
        {
            var go = Instantiate(GameManager.Instance.IngredientIconPrefab,
                gameObject.transform.Find("Canvas").transform.Find("IconContainer"), false);
            go.GetComponent<Image>().sprite = ingredient.icon;
            activeIcons.Add(go);
            go.SetActive(true);
        }

        // Destroys icon gameobjects on closePizza and then clears ActiveIcons list.
        foreach (var icon in ClosePizza.ActiveIcons)
        {
            Destroy(icon);
        }

        ClosePizza.ActiveIcons.Clear();
        ClosePizza.Ingredients.Clear();
        if (ClosePizza.IsCooked)
        {
            heldPizzaSO.cookState = HeldPizzaSO.CookState.Cooked;
            ClosePizza.IsCooked = false;
            InstantiatedGameObject = Instantiate(GameManager.Instance.PizzaBoxPrefab, pizzaBoxContainer.transform, false);
            GetComponent<Animator>().SetFloat("HoldingPizzaBox", 1);
        }
        else
        {
            InstantiatedGameObject = Instantiate(GameManager.Instance.PizzaPrefab, pizzaBoxContainer.transform, false);
            GetComponent<Animator>().SetFloat("HoldingPizza", 1);
        }

    }

    public void PutDownPizza()
    {
        // Loops through all of ingredients in pizza that player is currently holding.
        foreach (var ingredient in heldPizzaSO.ingredients)
        {
            ClosePizza.Ingredients.Add(ingredient);
            var go = Instantiate(GameManager.Instance.IngredientIconPrefab,
                ClosePizza.gameObject.transform.parent.Find("Canvas").Find("IconContainer"), false);

            ClosePizza.ActiveIcons.Add(go);
            go.GetComponent<Image>().sprite = ingredient.icon;
            go.SetActive(true);
        }

        if (heldPizzaSO.cookState == HeldPizzaSO.CookState.Cooked)
        {
            ClosePizza.IsCooked = true;
            ClosePizza.InstantiatePrefab(ClosePizza.PizzaBoxPrefab);
            GetComponent<Animator>().SetFloat("HoldingPizzaBox", 0);
        }
        else
        {
            ClosePizza.InstantiatePrefab(GameManager.Instance.PizzaPrefab);
            GetComponent<Animator>().SetFloat("HoldingPizza", 0);
        }

        heldPizzaSO.cookState = HeldPizzaSO.CookState.Uncooked;
        HeldPizza = null;
        heldPizzaSO.ingredients.Clear();
        Destroy(InstantiatedGameObject);
        InstantiatedGameObject = null;
        ClearActiveIcons();
    }

    public void DiscardPizza()
    {
        HeldPizza = null;
        heldPizzaSO.ingredients.Clear();
        if (heldPizzaSO.cookState == HeldPizzaSO.CookState.Cooked)
        {
            GetComponent<Animator>().SetFloat("HoldingPizzaBox", 0);
        }
        else
        {
            GetComponent<Animator>().SetFloat("HoldingPizza", 0);
        }
        heldPizzaSO.cookState = HeldPizzaSO.CookState.Uncooked;
        Destroy(InstantiatedGameObject);
        InstantiatedGameObject = null;
        ClearActiveIcons();
    }

    public void ClearActiveIcons()
    {
        // Destroys all of the icon gameobjects on activeIcons and clears list after that.
        foreach (var icon in activeIcons)
        {
            Destroy(icon.gameObject);
        }

        activeIcons.Clear();
    }

    // Updates player's personal score.
    public void UpdateScore(float timeLeftPercentage)
    {
        var amount = DefaultValues.pizzaPointValue * timeLeftPercentage;
        if (timeLeftPercentage >= DefaultValues.fastDeliveryThreshold)
        {
            amount += DefaultValues.fastDeliveryBonus;
        }

        PlayerScore += (int) amount;
        GameManager.Instance.UpdatePlayerScoreText((int) playerNumber, PlayerScore);
    }

    private IEnumerator DisplayPlayerNumber()
    {
        playerNumberIcon.SetActive(true);
        yield return new WaitForSeconds(5f);
        playerNumberIcon.SetActive(false);
    }

    // ActiveIcons getter and setter wouldn't work correctly when adding icons in PizzaOven class,
    // so I had to add this method.
    public void AddToActiveIcons(GameObject icon)
    {
        activeIcons.Add(icon);
    }

    #region Class attributes

    /// <summary>
    ///     Ingredient that player is currently holding.
    /// </summary>
    [SerializeField] private IngredientSO heldIngredient;

    /// <summary>
    ///     Scriptable object that contains list of ingredients from pizza that player
    ///     has picked up.
    /// </summary>
    [Tooltip("Scriptable object reference of HeldPizzaSO.")] [SerializeField]
    private HeldPizzaSO heldPizzaSO;

    private PlayerController controller;

    #endregion

    #region Private attributes

    [Tooltip("Range that player detects close ingredients")] [Range(0, 3)] [SerializeField]
    /// <summary>
    /// Range of player being able to detect pizzas and ingredients.
    /// </summary>
    private float detectionRange;

    [SerializeField] private Image ingredientIcon;

    /// <summary>
    ///     Icons that are currently actively displayed over player.
    /// </summary>
    [SerializeField] private List<GameObject> activeIcons;

    [SerializeField] private GameObject pizzaBoxContainer;

    public GameObject InstantiatedGameObject { get; set; }

    [SerializeField] private GameObject playerNumberIcon;

    #endregion

    #region Getters and setters

    public PlayerNumber GetPlayerNumber => playerNumber;

    /// <summary>
    ///     Ingredient that CheckForCloseIngredients() method has detected.
    /// </summary>
    public Ingredient CloseIngredient { get; private set; }

    public IngredientSO HeldIngredient
    {
        get => heldIngredient;
        set => heldIngredient = value;
    }

    /// <summary>
    ///     Pizza that player is currently close by.
    /// </summary>
    public Pizza ClosePizza { get; private set; }

    /// <summary>
    ///     Pizza oven that player is currently close by.
    /// </summary>
    public PizzaOven ClosePizzaOven { get; private set; }

    /// <summary>
    ///     Pizza that player is currently holding.
    /// </summary>
    public HeldPizzaSO HeldPizza { get; set; }

    public HeldPizzaSO HeldPizzaSO => heldPizzaSO;

    public Image IngredientIcon => ingredientIcon;

    /// <summary>
    ///     Delivery point that player is currently close by.
    /// </summary>
    public DeliveryPoint CloseDeliveryPoint { get; private set; }

    /*
    public List<GameObject> ActiveIcons
    {
        get => activeIcons;
        set => activeIcons = value;
    }
*/
    public GameObject CloseTrashBin { get; private set; }

    public GameObject PizzaBoxContainer => pizzaBoxContainer;

    public int PlayerScore { get; private set; }

    #endregion
}
