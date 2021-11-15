using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum PlayerNumber { Player1, Player2 }
    [SerializeField]
    private PlayerNumber playerNumber;

    #region Class attributes
    /// <summary>
    /// Ingredient that CheckForCloseIngredients() method has detected.
    /// </summary>
    private Ingredient closeIngredient;

    /// <summary>
    /// Ingredient that player is currently holding.
    /// </summary>
    [SerializeField]
    private IngredientSO heldIngredient;

    /// <summary>
    /// Pizza that player is currently close by.
    /// </summary>
    private Pizza closePizza;

    /// <summary>
    /// Pizza oven that player is currently close by.
    /// </summary>
    private PizzaOven closePizzaOven;

    /// <summary>
    /// Delivery point that player is currently close by.
    /// </summary>
    private DeliveryPoint closeDeliveryPoint;

    /// <summary>
    /// Pizza that player is currently holding.
    /// </summary>
    private HeldPizzaSO heldPizza;

    /// <summary>
    /// Scriptable object that contains list of ingredients from pizza that player
    /// has picked up.
    /// </summary>
    [Tooltip("Scriptable object reference of HeldPizzaSO.")]
    [SerializeField]
    private HeldPizzaSO heldPizzaSO;

    private PlayerController controller;
    #endregion

    #region Private attributes
    [Tooltip("Range that player detects close ingredients")]
    [Range(0, 3)]
    [SerializeField]
    /// <summary>
    /// Range of player being able to detect pizzas and ingredients.
    /// </summary>
    private float detectionRange;
    [SerializeField]
    private Image ingredientIcon;

    /// <summary>
    /// Icons that are currently actively displayed over player.
    /// </summary>
    [SerializeField]
    private List<GameObject> activeIcons;

    private GameObject closeDonationBox;
    [SerializeField]
    private GameObject pizzaBoxContainer;
    public GameObject instantiatedGameObject { get; set; }
    #endregion

    #region Getters and setters
    public PlayerNumber GetPlayerNumber
    {
        get { return playerNumber; }
    }
    public Ingredient CloseIngredient
    {
        get { return closeIngredient; }
    }

    public IngredientSO HeldIngredient
    {
        get { return heldIngredient; }
        set { heldIngredient = value; }
    }

    public Pizza ClosePizza
    {
        get { return closePizza; }
    }

    public PizzaOven ClosePizzaOven
    {
        get { return closePizzaOven; }
    }

    public HeldPizzaSO HeldPizza
    {
        get { return heldPizza; }
        set { heldPizza = value; }
    }

    public HeldPizzaSO HeldPizzaSO
    {
        get { return heldPizzaSO; }
    }

    public Image IngredientIcon
    {
        get { return ingredientIcon; }
        set { ingredientIcon = value; }
    }

    public DeliveryPoint CloseDeliveryPoint
    {
        get { return closeDeliveryPoint; }
    }

    public List<GameObject> ActiveIcons
    {
        get { return activeIcons; }
        set { activeIcons = value; }
    }

    public GameObject CloseTrashBin
    {
        get { return closeDonationBox; }
    }
    public GameObject PizzaBoxContainer { get { return pizzaBoxContainer; } }
    #endregion

    private void Awake()
    {
        // Gets PlayerController component if it is not assigned.
        if (controller == null)
            controller = GetComponent<PlayerController>();

        heldPizzaSO.ingredients.Clear();
        heldPizzaSO.cookState = HeldPizzaSO.CookState.Uncooked;

        // Loops CheckForInteractables method every 0.15 seconds.
        InvokeRepeating("CheckForInteractables", 0, 0.15f);
    }

    private void Update()
    {
        if (heldIngredient == null && ingredientIcon.gameObject.activeSelf == true)
            ingredientIcon.gameObject.SetActive(false);
        else if (heldIngredient != null && ingredientIcon.gameObject.activeSelf == false)
            ingredientIcon.gameObject.SetActive(true);
    }

    #region Interactable detection
    /// <summary>
    /// Checks if there are any GameObjects tagged as "Ingredient", "Pizza",
    /// "PizzaOven", "DeliveryPoint" or "TrashBin" and if so then it sets closeIngredient to be that.
    /// </summary>
    private void CheckForInteractables()
    {
        if (!GameManager.Instance.GameStarted)
            return;

        // Calculates position where Ray will be cast.
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        RaycastHit hit;
        Ray forwardRay = new Ray(pos, transform.forward);
        Debug.DrawRay(pos, transform.forward);

        if (Physics.Raycast(forwardRay, out hit))
        {
            
            if (hit.distance < detectionRange)
            {
                switch (hit.collider.tag)
                {
                    case "Pizza":
                        Pizza detectedPizza = hit.transform.GetComponent<Pizza>();
                        if (closePizza != detectedPizza)
                        {
                            if (closePizza != null && closePizza.outline.enabled)
                                closePizza.outline.enabled = false;
                            closePizza = detectedPizza;
                            closePizza.outline.enabled = true;
                        }
                        break;
                    case "Ingredient":
                        Ingredient detectedIngredient = hit.transform.GetComponent<Ingredient>();
                        if (closeIngredient != detectedIngredient)
                        {
                            if (closeIngredient != null && closeIngredient.HighLightMaterial.GetTexture("_MainTex") != closeIngredient.DefaultTexture)
                                closeIngredient.HighLightMaterial.SetTexture("_MainTex", closeIngredient.DefaultTexture);
                            closeIngredient = detectedIngredient;
                            closeIngredient.HighLightMaterial.SetTexture("_MainTex", closeIngredient.HighLightTexture);
                        }
                        break;
                    case "PizzaOven":
                        PizzaOven detectedPizzaOven = hit.transform.GetComponent<PizzaOven>();
                        if (closePizzaOven != detectedPizzaOven)
                        {
                            if (closePizzaOven != null && closePizzaOven.highlightMaterial.GetColor("_Color") != closePizzaOven.TopMaterialColor[0])
                                closePizzaOven.highlightMaterial.SetColor("_Color", closePizzaOven.TopMaterialColor[0]);
                            closePizzaOven = detectedPizzaOven;
                            closePizzaOven.highlightMaterial.SetColor("_Color", closePizzaOven.TopMaterialColor[1]);
                        }
                        break;
                    case "DeliveryPoint":
                        DeliveryPoint detectedDeliveryPoint = hit.transform.GetComponent<DeliveryPoint>();
                        if (closeDeliveryPoint != detectedDeliveryPoint)
                        {
                            if (closeDeliveryPoint != null && closeDeliveryPoint.highlightMaterial.GetColor("_Color") != closeDeliveryPoint.TopMaterialColor[0])
                                closeDeliveryPoint.highlightMaterial.SetColor("_Color", closeDeliveryPoint.TopMaterialColor[0]);
                            closeDeliveryPoint = detectedDeliveryPoint;
                            closeDeliveryPoint.highlightMaterial.SetColor("_Color", closeDeliveryPoint.TopMaterialColor[1]);
                        }
                        break;
                    case "DonationBox":
                        GameObject detectedDonationBox = hit.transform.gameObject;
                        if (closeDonationBox != detectedDonationBox)
                        {
                            if (closeDonationBox != null && closeDonationBox.GetComponent<Outline>().enabled)
                                closeDonationBox.GetComponent<Outline>().enabled = false;
                            closeDonationBox = detectedDonationBox;
                            closeDonationBox.GetComponent<Outline>().enabled = true;
                        }
                        break;
                    default:
                        break;
                }
            }
            
            // If hit distance is greater than detection range
            // this will then set closeIngredient and closePizza to null;
            if (hit.distance > detectionRange)
            {
                if (closeIngredient != null)
                {
                    closeIngredient.HighLightMaterial.SetTexture("_MainTex", closeIngredient.DefaultTexture);
                    closeIngredient = null;
                }

                if (closePizza != null)
                {
                    closePizza.outline.enabled = false;
                    closePizza = null;
                }

                if (closePizzaOven != null)
                {
                    closePizzaOven.highlightMaterial.SetColor("_Color", closePizzaOven.TopMaterialColor[0]);
                    closePizzaOven = null;
                }

                if (closeDeliveryPoint != null)
                {
                    closeDeliveryPoint.highlightMaterial.SetColor("_Color", closeDeliveryPoint.TopMaterialColor[0]);
                    closeDeliveryPoint = null;
                }

                if (closeDonationBox != null)
                {
                    closeDonationBox = null;
                }
            }
        }
    }
    #endregion

    public void PickUpPizza()
    {
        if (heldIngredient != null)
            heldIngredient = null;

        heldPizzaSO.ingredients.Clear();
        foreach (var ingredient in closePizza.Ingredients)
        {
            heldPizzaSO.ingredients.Add(ingredient);
        }
        heldPizza = heldPizzaSO;

        foreach (var ingredient in heldPizza.ingredients)
        {
            GameObject go = Instantiate(GameManager.Instance.IngredientIconPrefab);
            go.transform.SetParent(gameObject.transform.Find("Canvas").transform.Find("IconContainer"), false);
            go.GetComponent<Image>().sprite = ingredient.icon;
            activeIcons.Add(go);
            go.SetActive(true);
        }

        // Destroys icon gameobjects on closePizza and then clears ActiveIcons list.
        foreach (var icon in closePizza.ActiveIcons)
        {
            Destroy(icon);
        }
        closePizza.ActiveIcons.Clear();
        closePizza.Ingredients.Clear();
        if (closePizza.IsCooked)
        {
            heldPizzaSO.cookState = HeldPizzaSO.CookState.Cooked;
            closePizza.IsCooked = false;
        }
        instantiatedGameObject = Instantiate(GameManager.Instance.PizzaBoxPrefab);
        instantiatedGameObject.transform.SetParent(pizzaBoxContainer.transform, false);
        GetComponent<Animator>().SetFloat("Holding", 1);
    }

    public void PutDownPizza()
    {
        // Loops through all of ingredients in pizza that player is currently holding.
        foreach (var ingredient in heldPizzaSO.ingredients)
        {
            closePizza.Ingredients.Add(ingredient);
            GameObject go = Instantiate(GameManager.Instance.IngredientIconPrefab);
            closePizza.ActiveIcons.Add(go);
            go.transform.SetParent(closePizza.gameObject.transform.parent.Find("Canvas").Find("IconContainer"), false);
            go.GetComponent<Image>().sprite = ingredient.icon;
            go.SetActive(true);
        }
        if (heldPizzaSO.cookState == HeldPizzaSO.CookState.Cooked)
            closePizza.IsCooked = true;
        heldPizzaSO.cookState = HeldPizzaSO.CookState.Uncooked;
        heldPizza = null;
        heldPizzaSO.ingredients.Clear();
        Destroy(instantiatedGameObject);
        instantiatedGameObject = null;
        GetComponent<Animator>().SetFloat("Holding", 0);
        ClearActiveIcons();
    }

    public void DiscardPizza()
    {
        heldPizza = null;
        heldPizzaSO.ingredients.Clear();
        heldPizzaSO.cookState = HeldPizzaSO.CookState.Uncooked;
        Destroy(instantiatedGameObject);
        instantiatedGameObject = null;
        GetComponent<Animator>().SetFloat("Holding", 0);
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

    // ActiveIcons getter and setter wouldn't work correctly when adding icons in PizzaOven class,
    // so I had to add this method.
    public void AddToActiveIcons(GameObject icon) => activeIcons.Add(icon);

}
