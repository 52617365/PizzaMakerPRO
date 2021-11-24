using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pizza : MonoBehaviour
{
    #region Scriptable object references

    #endregion

    #region Private attributes
    /// <summary>
    /// Default colors of highlight material.
    /// </summary>
    [SerializeField]
    private Color[] defaultColors;

    /// <summary>
    /// List of ingredients that are on pizza.
    /// </summary>
    [SerializeField]
    private List<IngredientSO> ingredients;

    /// <summary>
    /// Prefab of the icon that displays what ingredients
    /// have been added on to pizza.
    /// </summary>
    [SerializeField]
    private GameObject ingredientIconPrefab;

    /// <summary>
    /// Parent GameObject of ingredientIconPrefab.
    /// </summary>
    [SerializeField]
    private GameObject iconPrefabParent;

    [SerializeField]
    private List<GameObject> activeIcons;

    private bool isCooked;
    #endregion

    #region Getters and setters
    public Material highlightMaterial { get; private set; }

    public Color[] TopMaterialColor { get { return defaultColors; } }

    public List<IngredientSO> Ingredients
    {
        get { return ingredients; }
        set { ingredients = value; }
    }

    public List<GameObject> ActiveIcons
    {
        get { return activeIcons; }
        set { activeIcons = value; }
    }

    public bool IsCooked
    {
        get { return isCooked; }
        set { isCooked = value; }
    }
    #endregion

    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        foreach (var material in renderer.materials)
        {
            if (material.name == "highlight (Instance)")
            {
                highlightMaterial = material;
                break;
            }
        }
    }

    #region Public methods
    /// <summary>
    /// Adds ingredientSO to list of ingredients and then
    /// nulls player's held ingredient.
    /// </summary>
    /// <param name="ingredientSO"></param>
    /// <param name="player"></param>
    public void AddIngredient(IngredientSO ingredientSO, Player player)
    {
        if (!ingredients.Contains(ingredientSO))
        {
            ingredients.Add(ingredientSO);

            // Instantiates icon prefab that displays icon of what ingredient
            // was just added on to pizza.
            GameObject icon = Instantiate(GameManager.Instance.IngredientIconPrefab);
            activeIcons.Add(icon);
            icon.GetComponent<Image>().sprite = ingredientSO.icon;

            // Sets the parent of instantiated GameObject to be iconPrefabParent
            RectTransform rectTransform = icon.GetComponent<RectTransform>();
            rectTransform.SetParent(iconPrefabParent.GetComponent<RectTransform>().transform, false);

            player.HeldIngredient = null;
        }
        else
        {
            Debug.Log(ingredientSO + " is already on pizza");
            // TODO: Display on UI that player can't add ingredient on to pizza.
        }
    }
    #endregion
}
