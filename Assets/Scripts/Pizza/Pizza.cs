using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pizza : MonoBehaviour
{
    private void Awake()
    {
        var rendererComponent = GetComponent<Renderer>();
        foreach (Material material in rendererComponent.materials)
        {
            if (material.name == "highlight (Instance)")
            {
                HighlightMaterial = material;
                break;
            }
        }
    }

    #region Public methods

    /// <summary>
    ///     Adds ingredientSO to list of ingredients and then
    ///     nulls player's held ingredient.
    /// </summary>
    /// <param name="ingredientSO"></param>
    /// <param name="player"></param>
    public void AddIngredient(IngredientSO ingredientSO, Player player)
    {
        if (!ingredients.Contains(ingredientSO))
        {
            if (instantiatedObject == null)
            {
                InstantiatePrefab(GameManager.Instance.PizzaPrefab);
            }

            ingredients.Add(ingredientSO);

            // Instantiates icon prefab that displays icon of what ingredient
            // was just added on to pizza.
            GameObject icon = Instantiate(GameManager.Instance.IngredientIconPrefab);
            activeIcons.Add(icon);
            icon.GetComponent<Image>().sprite = ingredientSO.icon;

            // Sets the parent of instantiated GameObject to be iconPrefabParent
            var rectTransform = icon.GetComponent<RectTransform>();
            rectTransform.SetParent(iconPrefabParent.GetComponent<RectTransform>().transform, false);

            player.HeldIngredient = null;
        }
    }

    /// <summary>
    /// Instantiates prefab gameObject on table
    /// </summary>
    /// <param name="prefab"></param>
    public void InstantiatePrefab(GameObject prefab) => instantiatedObject = Instantiate(prefab, instantiatePoint.transform.position, prefab.transform.rotation);

    #endregion

    #region Scriptable object references

    #endregion

    #region Private attributes

    /// <summary>
    /// Prefab of pizza box.
    /// </summary>
    [SerializeField] private GameObject pizzaBoxPrefab;

    /// <summary>
    /// Position where instantiatedObject will be.
    /// </summary>
    [SerializeField] private GameObject instantiatePoint;
    /// <summary>
    /// Reference to instantiated pizzaPrefab or pizzaBoxPrefab.
    /// </summary>
    private GameObject instantiatedObject;

    /// <summary>
    ///     Default colors of highlight material.
    /// </summary>
    [SerializeField] private Color[] defaultColors;

    /// <summary>
    ///     List of ingredients that are on pizza.
    /// </summary>
    [SerializeField] private List<IngredientSO> ingredients;

    /// <summary>
    ///     Prefab of the icon that displays what ingredients
    ///     have been added on to pizza.
    /// </summary>
    //[SerializeField] private GameObject ingredientIconPrefab;

    /// <summary>
    ///     Parent GameObject of ingredientIconPrefab.
    /// </summary>
    [SerializeField] private GameObject iconPrefabParent;

    [SerializeField] private List<GameObject> activeIcons;

    #endregion

    #region Getters and setters

    public Material HighlightMaterial { get; private set; }

    public Color[] TopMaterialColor => defaultColors;

    public List<IngredientSO> Ingredients => ingredients;

    public List<GameObject> ActiveIcons => activeIcons;

    public GameObject InstantiatedObject => instantiatedObject;


    public GameObject PizzaBoxPrefab => pizzaBoxPrefab;

    public bool IsCooked { get; set; }

    #endregion
}
